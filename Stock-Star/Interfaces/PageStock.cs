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
        BindingList<Produit> stock = new BindingList<Produit>(); //création d'une liste de produit qui va nous permettre de stocker les produits que l'on ajoute dans le DataGridView
        // On crée l'objet Gestion Produit
        GestionProduits gestion = new GestionProduits();
        public PageStock()
        {
            InitializeComponent();
            guna2DataGridView1.AutoGenerateColumns = true;
            
            // On remplie le DataGridView avec la méthode ChargerStock
            guna2DataGridView1.DataSource = gestion.ChargerStock();

            AidesSaisies();
        }

        //
        private void AidesSaisies()
        {
            TxtBoxCategorie.PlaceholderText = "Entrez une catégorie";
            TxtBoxNom.PlaceholderText = "Entrez un produit";
            TxtBoxQuantite.PlaceholderText = "Entrez une quantité";
            TxtBoxPrice.PlaceholderText = "Entrez un prix";
            TxtBoxEmplacement.PlaceholderText = "Entrez un emplacement (optionnel)";
            TxtBoxDescription.PlaceholderText = "Entrez une description (optionnel)";
        }


        // Méthode ViderLesChamps TextBox
        private void ViderChamps()
        {
            TxtBoxCategorie.Clear();
            TxtBoxNom.Clear();
            TxtBoxQuantite.Clear();
            TxtBoxPrice.Clear();
            TxtBoxEmplacement.Clear();
            TxtBoxDescription.Clear();
        }

        // Click sur le bouton Ajouter
        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            string Categorie = TxtBoxCategorie.Text;
            string Nom = TxtBoxNom.Text;
            string Emplacement = TxtBoxEmplacement.Text;
            string Description = TxtBoxDescription.Text;

            // Conversion sécurisée de la quantité
            if (!int.TryParse(TxtBoxQuantite.Text, out int Quantite))
            {
                MessageBox.Show("Veuillez saisir un nombre entier pour la quantité.");
                return; // Break
            }

            // .Replace('.', ',') permet d'accepter les points et les virgules
            if (!decimal.TryParse(TxtBoxPrice.Text.Replace('.', ','), out decimal Prix))
            {
                MessageBox.Show("Veuillez saisir un prix valide.");
                return; //Break
            }

            // Maintenant tu peux appeler ta méthode SQL avec les bonnes variables
            gestion.AjoutStock(Categorie, Nom, Emplacement, Description, Quantite, Prix);
            
            // On actualise le DataGridView pour afficher notre stock avec notre nouvelle élément
            guna2DataGridView1.DataSource = gestion.ChargerStock();
            
            // On vide tous les champs de texte
            ViderChamps();
        }

        private void BtnSupprimer_Click(object sender, EventArgs e)
        {
            if (guna2DataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Sélectionnez une ligne à supprimer.");
                return;
            }

            if (guna2DataGridView1.CurrentRow.DataBoundItem is Produit produit)
            {
                stock.Remove(produit);
            }
        }

        private void guna2DataGridView1_KeyDown(object sender, KeyEventArgs e) //Si on appuye su le Btn suprimmer on supprime la ligne selectionner dans le DataGridView
        {
            if (e.KeyCode == Keys.Delete)
            {
                BtnSupprimer_Click(sender, e);
            }
        }

        private void TxtBoxPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BtnAjouter_Click(sender, e);
                e.Handled = true; //Cette ligne empêche le "ding" sonore lorsque vous appuyez sur Entrée
                //Le bip provient seulment avec la touche entrée, les autres touche ne font pas de bruit car windows ne les considère pas comme des actions de validation c'est pour ça qu'on à pas ce parametre sur le suprimmer juste au dessus.
                e.SuppressKeyPress = true;
            }
        }
    }
}