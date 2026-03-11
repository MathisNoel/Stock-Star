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
        private int _idProduit;
        public PageModification(Form1 parent, int idProduit, string categorie, string nom, int quantité, string emplacement, string description, decimal prixAchat)
        {
            InitializeComponent();

            _parent = parent;
            _idProduit = idProduit;

            // Pré-remplissage des champs
            TxtBoxCategorie.Text = categorie;
            TxtBoxNom.Text = nom;
            TxtBoxEmplacement.Text = emplacement;
            TxtBoxDescription.Text = description;
        }

        private void AidesSaisies()
        {
            TxtBoxCategorie.PlaceholderText = "Entrez une catégorie";
            TxtBoxNom.PlaceholderText = "Entrez un produit";
            TxtBoxEmplacement.PlaceholderText = "Entrez un emplacement (optionnel)";
            TxtBoxDescription.PlaceholderText = "Entrez une description (optionnel)";
        }

        private void BtnAnnuler_Click(object sender, EventArgs e)
        {
            _parent.LoadPage(new PageStock(_parent)); // Retour à la page de stock

        }

        private void BtnModifier_Click(object sender, EventArgs e)
        {
            // Validation simple
            if (string.IsNullOrWhiteSpace(TxtBoxNom.Text))
            {
                MessageBox.Show("Le nom du produit est obligatoire.");
                return;
            }

            if (string.IsNullOrWhiteSpace(TxtBoxCategorie.Text))
            {
                MessageBox.Show("La catégorie est obligatoire.");
                return;
            }

            // Appel à la méthode SQL existante
            GestionProduits gestion = new GestionProduits();

            gestion.ModifierProduit(
                _idProduit,
                TxtBoxCategorie.Text,
                TxtBoxNom.Text,
                TxtBoxEmplacement.Text,
                TxtBoxDescription.Text
            );

            MessageBox.Show("Produit modifié avec succès.");

            // Retour à la page stock
            _parent.LoadPage(new PageStock(_parent));

        }

    }
}
