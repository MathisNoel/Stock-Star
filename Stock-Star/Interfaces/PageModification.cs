using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Stock_Star.Interfaces
{
    public partial class PageModification : UserControl
    {
        private Form1 _parent;
        private string _nomInitial;

        // On ajoute les arguments manquants ici
        public PageModification(Form1 parent, string nomProduit, string categorie, string emplacement, string description)
        {
            InitializeComponent();

            _parent = parent;
            _nomInitial = nomProduit; // On garde précieusement l'ancien nom pour le WHERE du SQL

            // On remplit directement les TextBox avec ce qu'on a reçu
            TxtBoxNom.Text = nomProduit;
            TxtBoxCategorie.Text = categorie;
            TxtBoxEmplacement.Text = emplacement;
            TxtBoxDescription.Text = description;

            // On affiches les placeholders si il y aucun text
            AidesSaisies();
        }

        private void AidesSaisies()
        {
            TxtBoxCategorie.PlaceholderText = "Entrez une catégorie";
            TxtBoxNom.PlaceholderText = "Entrez un produit";
            TxtBoxEmplacement.PlaceholderText = "Entrez un emplacement (optionnel)";
            TxtBoxDescription.PlaceholderText = "Entrez une description (optionnel)";
        }

        // Appuie sur le bouton Modifier
        private void BtnModifier_Click(object sender, EventArgs e)
        {
            // On enregistre le contenue dans les TextBox dans des variables string
            string nouveauNom = TxtBoxNom.Text;
            string nouvelleCategorie = TxtBoxCategorie.Text;
            string nouvelEmplacement = TxtBoxEmplacement.Text;
            string nouvelleDescription = TxtBoxDescription.Text;
            // Vérification qu'on a pas renseigné un produit avec un nom vide ou un espace
            if (string.IsNullOrWhiteSpace(nouveauNom))
            {
                MessageBox.Show("Le nom du produit est obligatoire.");
                return;
            }

            // On initialise une variable gestion qui pourra appeller les méthodes de la classe GestionProduits
            GestionProduits gestion = new GestionProduits();
            // On appelle la méthode ModifierProduit définie dans la classe GestionProduits
            gestion.ModifierProduit(_nomInitial, nouvelleCategorie, nouveauNom, nouvelEmplacement, nouvelleDescription);

            // On reviens a la PageStock si on appuie sur le bouton Modifier
            _parent.LoadPage(new PageStock(_parent));
        }

        // Appuie sur le bouton Annuler
        private void BtnAnnuler_Click(object sender, EventArgs e)
        {
            // On reviens a la PageStock si on appuie sur le bouton Annuler
            _parent.LoadPage(new PageStock(_parent));
        }
    }
}
