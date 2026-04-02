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
            string NomVente = TxtBoxNomPageVente.Text;
            //string QuantiteVendue = TxtBoxQuantitePageVente.Text;
            //string PrixVente = TxtBoxPricePageVente.Text;


            DateTime dateVente;
            string dateTexte = TxtBoxDatePageVente.Text;

            // Si vide → on utilise la date actuelle
            if (string.IsNullOrWhiteSpace(dateTexte))
            {
                dateVente = DateTime.Now;
            }
            else
            {
                // Si non vide → on vérifie que la date est valide
                if (!DateTime.TryParse(dateTexte, out dateVente))
                {
                    MessageBox.Show("La date saisie est invalide.");
                    return;
                }
            }

            if (!int.TryParse(TxtBoxQuantitePageVente.Text, out int QuantiteVendue))
            {
                MessageBox.Show("Veuillez saisir un nombre entier pour la quantité.");
                return; // Break
            }

            if (!decimal.TryParse(TxtBoxPricePageVente.Text.Replace('.', ','), out decimal PrixVente))
            {
                MessageBox.Show("Veuillez saisir un prix valide.");
                return; //Break
            }

            ViderChamps();



            gestion.AjoutVente(NomVente, QuantiteVendue, PrixVente, dateVente);
            VenteEffectuee?.Invoke(); // Previens les autres user controlel que la vente a été effectuée pour qu'ils puissent réagir en conséquence (actualiser le stock par exemple)
            ActualiserGrille();
        }
    }
}
