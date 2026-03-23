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
    public partial class PageAchatEtVente : UserControl
    {
        public event Action VenteEffectuee;
        GestionProduits gestion = new GestionProduits();

        public PageAchatEtVente()
        {
            InitializeComponent();
            AidesSaisies();
            ActualiserGrille();

        }

        private void AidesSaisies()
        {
            TxtBoxNomPageVente.PlaceholderText = "Nom";
            TxtBoxDatePageVente.PlaceholderText = "Date jj/mm/aaaa";
            TxtBoxQuantitePageVente.PlaceholderText = "quantité";
            TxtBoxPricePageVente.PlaceholderText = "Prix de vente";
        }

        private void ViderChamps()
        {
            TxtBoxNomPageVente.Clear();
            TxtBoxDatePageVente.Clear();
            TxtBoxQuantitePageVente.Clear();
            TxtBoxPricePageVente.Clear();
        }

        public void ActualiserGrille()
        {
            guna2DataGridView1.DataSource = gestion.ChargerLesVentes();
            // On met les boutons supprimer tout à droite
            var ColonneSupprimer = guna2DataGridView1.Columns["BoutonSupprimer"];
            if (ColonneSupprimer != null)
            {
                ColonneSupprimer.DisplayIndex = guna2DataGridView1.ColumnCount - 1;
                ColonneSupprimer.Width = 80;
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
