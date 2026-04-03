using Guna.UI2.WinForms.Suite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using static System.Collections.Specialized.BitVector32;


namespace Stock_Star.Interfaces
{
    public partial class PageTransaction : UserControl
    {
        public event Action VenteEffectuee;
        GestionProduits gestion = new GestionProduits();

        public PageTransaction()
        {
            InitializeComponent();
            AidesSaisies();
            ActualiserGrille();

        }

        private void AidesSaisies()
        {
            //Ligne Vendre
            TxtBoxNomPageVente.PlaceholderText = "Nom";
            TxtBoxQuantitePageVente.PlaceholderText = "Quantité";
            TxtBoxPricePageVente.PlaceholderText = "Prix de vente (/u)";
            TxtBoxDatePageVente.PlaceholderText = "Date jj/mm/aaaa";
            //Ligne Achat
            TxtBoxNomPageAchat.PlaceholderText = "Nom";
            TxtBoxQuantitePageAchat.PlaceholderText = "Quantité";
            TxtBoxPricePageAchat.PlaceholderText = "Prix d'achat (/u)";
            TxtBoxDatePageAchat.PlaceholderText = "Date jj/mm/aaaa";
        }

        private void ViderChamps()
        {
            //
            TxtBoxNomPageVente.Clear();
            TxtBoxDatePageVente.Clear();
            TxtBoxQuantitePageVente.Clear();
            TxtBoxPricePageVente.Clear();

            TxtBoxNomPageAchat.Clear();
            TxtBoxDatePageAchat.Clear();
            TxtBoxQuantitePageAchat.Clear();
            TxtBoxPricePageAchat.Clear();
        }

        public void ActualiserGrille()
        {
            // Tableau Historique des Ventes
            DataGridView_Ventes.DataSource = gestion.ChargerLesVentes();
            // On met les boutons supprimer tout à droite
            var ColonneSupprimerV = DataGridView_Ventes.Columns["BoutonSupprimerVente"];
            if (ColonneSupprimerV != null)
            {
                ColonneSupprimerV.DisplayIndex = DataGridView_Ventes.ColumnCount - 1;
                ColonneSupprimerV.Width = 80;
            }

            // Tableau Historique des Achats
            //DataGridView_Achats.DataSource = gestion.ChargerLesAchats();
            // On met les boutons supprimer tout à droite
            var ColonneSupprimerA = DataGridView_Achats.Columns["BoutonSupprimerAchat"];
            if (ColonneSupprimerA != null)
            {
                ColonneSupprimerA.DisplayIndex = DataGridView_Achats.ColumnCount - 1;
                ColonneSupprimerA.Width = 80;
            }
        }

        private void BtnVendre_Click(object sender, EventArgs e)
        {
            // 1. Récupération brute des textes
            string nomVente = TxtBoxNomPageVente.Text.Trim();

            // 2. Vérification du nom (évite d'envoyer du vide à la BDD)
            if (string.IsNullOrWhiteSpace(nomVente))
            {
                MessageBox.Show("Veuillez saisir un nom de produit.");
                return;
            }

            // 3. Parsing de la Date
            DateTime dateVente;
            string dateTexte = TxtBoxDatePageVente.Text.Trim();
            if (string.IsNullOrWhiteSpace(dateTexte))
            {
                dateVente = DateTime.Now;
            }
            else if (!DateTime.TryParse(dateTexte, out dateVente))
            {
                MessageBox.Show("La date saisie est invalide.");
                return;
            }

            // 4. Parsing de la Quantité (int)
            if (!int.TryParse(TxtBoxQuantitePageVente.Text, out int qteFinale))
            {
                MessageBox.Show("Veuillez saisir un nombre entier pour la quantité.");
                return;
            }

            // 5. Parsing du Prix (decimal)
            // On remplace le point par la virgule pour gérer les saisies FR
            string prixTexte = TxtBoxPricePageVente.Text.Replace('.', ',');
            if (!decimal.TryParse(prixTexte, out decimal prixFinal))
            {
                MessageBox.Show("Veuillez saisir un prix valide.");
                return;
            }

            // 6. Exécution
            try
            {
                // On appelle ta méthode SQL avec les bonnes variables castées
                gestion.AjoutVente(nomVente, qteFinale, prixFinal, dateVente);

                // 7. Nettoyage et Feedback
                ViderChamps();
                // VenteEffectuee?.Invoke(); // À décommenter si tu as l'event
                ActualiserGrille();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout : " + ex.Message);
            }
        }
    }
}
