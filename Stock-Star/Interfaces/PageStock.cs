using Stock_Star.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

// Remarques :
// - Le préprocesseur C# n'a pas d'instruction `#include`. La ligne `#include "Class2.cs"` provoque l'erreur CS1024.
// - Pour inclure `Class2.cs`, ajoutez simplement le fichier au projet (Visual Studio > Ajouter > Élément existant) ; il sera compilé avec les autres fichiers.
// - Ici, on retire la directive invalide et on conserve le reste du fichier intact.

namespace Stock_Star
{
    public partial class PageStock : UserControl
    {
        // On crée l'objet Gestion Produit
        GestionProduits gestion = new GestionProduits();
        Form1 _parent; // Création d'une variable pour stocker la référence au Form1

        public PageStock(Form1 parent)
        {
            InitializeComponent();
            _parent = parent;

            ActualiserGrille();
            AidesSaisies();
        }

        // Méthode pour actualiser la DataGridView
        public void ActualiserGrille()
        {
            guna2DataGridView1.DataSource = gestion.ChargerStock();

            // On met les boutons supprimer tout à droite, colonne N
            var ColonneSupprimer = guna2DataGridView1.Columns["BoutonSupprimer"];
            if (ColonneSupprimer != null)
            {
                ColonneSupprimer.DisplayIndex = guna2DataGridView1.ColumnCount - 1;
                ColonneSupprimer.Width = 80;
            }

            // On met les boutons éditer tout à droite -1, colonne N-1
            var ColonneEditer = guna2DataGridView1.Columns["BoutonEditer"];
            if (ColonneEditer != null)
            {
                ColonneEditer.DisplayIndex = guna2DataGridView1.ColumnCount - 2;
                ColonneEditer.Width = 80;
            }
        }

        // Aides a la saisies pour les différents champs a renseigner d'un produit
        private void AidesSaisies()
        {
            TxtBoxCategorie.PlaceholderText = "catégorie";
            TxtBoxNom.PlaceholderText = "produit";
            TxtBoxQuantite.PlaceholderText = "quantité";
            TxtBoxPrice.PlaceholderText = "prix";
            TxtBoxEmplacement.PlaceholderText = "emplacement (optionnel)";
            TxtBoxDescription.PlaceholderText = "description (optionnel)";
        }

        // Méthode pour vider les champs éventuellement renseigner par l'utilisateur des TxtBox (catégorie,nom,...)
        private void ViderChamps()
        {
            TxtBoxCategorie.Clear();
            TxtBoxNom.Clear();
            TxtBoxQuantite.Clear();
            TxtBoxPrice.Clear();
            TxtBoxEmplacement.Clear();
            TxtBoxDescription.Clear();
        }

        // Méthode suite a un click sur le bouton Ajouter
        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            string Categorie = TxtBoxCategorie.Text;
            string Nom = TxtBoxNom.Text;
            string Emplacement = TxtBoxEmplacement.Text;
            string Description = TxtBoxDescription.Text;

            // Conversion sécurisée de la quantité en un entier
            if (!int.TryParse(TxtBoxQuantite.Text, out int Quantite))
            {
                MessageBox.Show("Veuillez saisir un nombre entier pour la quantité.");
                return; // Break
            }

            // .Replace('.', ',') permet d'accepter les points et les virgules pour le chiffre décimal
            if (!decimal.TryParse(TxtBoxPrice.Text.Replace('.', ','), out decimal Prix))
            {
                MessageBox.Show("Veuillez saisir un prix valide.");
                return; //Break
            }

            // Appeler la méthode SQL avec les bonnes variables
            gestion.AjoutStock(Categorie, Nom, Emplacement, Description, Quantite, Prix);

            // On actualise le DataGridView pour afficher notre stock avec notre nouvelle élément
            ActualiserGrille();

            // On vide tous les champs de texte
            ViderChamps();
        }

        // Méthode lors d'un click sur un bouton/colonne du DataGridView en particulier le bouton Supprimer/Editer
        private void ClickOnDataGridView(object sender, DataGridViewCellEventArgs e)
        {
            // On vérifie si on a cliqué sur la colonne BoutonSupprimer
            if (e.ColumnIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "BoutonSupprimer" && e.RowIndex >= 0)
            {
                // On récupère le nom du produit sur la ligne | ?. et ?? "" signifie que si il n'y a aucune valeur (null) on le transforme en caractère vide ''.
                string nom = guna2DataGridView1.Rows[e.RowIndex].Cells["Nom"].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(nom))
                {
                    gestion.SupprimerProduit(nom);
                    ActualiserGrille();
                }
            }
            // On vérifie si on a cliqué sur la colonne BoutonEditer
            if (e.ColumnIndex >= 0 && guna2DataGridView1.Columns[e.ColumnIndex].Name == "BoutonEditer" && e.RowIndex >= 0)
            {
                // On récupère le nom du produit sur la ligne | ?. et ?? "" signifie que si il n'y a aucune valeur (null) on le transforme en caractère vide ''.
                string nom = guna2DataGridView1.Rows[e.RowIndex].Cells["Nom"].Value?.ToString() ?? "";
                string categorie = guna2DataGridView1.Rows[e.RowIndex].Cells["Catégorie"].Value?.ToString() ?? "";
                string emplacement = guna2DataGridView1.Rows[e.RowIndex].Cells["Emplacement"].Value?.ToString() ?? "";
                string description = guna2DataGridView1.Rows[e.RowIndex].Cells["Description"].Value?.ToString() ?? "";
                if (!string.IsNullOrEmpty(nom))
                {
                    //On affiche la page PageModification.cs avec le nom récupéré précedemment afin de supprimer le produit
                    PageModification pagemodification = new PageModification(_parent, nom , categorie, emplacement, description);
                    // On affiche la page
                    _parent.LoadPage(pagemodification);
                }
            }
        }
        private void TxtBoxQuantite_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnAjouter.PerformClick();   // Simule un clic sur le bouton Ajouter
                e.SuppressKeyPress = true;   // Empêche le "ding" Windows
            }    
        }
    }

}
