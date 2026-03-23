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

        private void BtnVendre_Click(object sender, EventArgs e)
        {
            string NomVente = TxtBoxNomPageVente.Text;
            //string QuantiteVendue = TxtBoxQuantitePageVente.Text;
            //string PrixVente = TxtBoxPricePageVente.Text;
            string DateVente = TxtBoxDatePageVente.Text;


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
            gestion.AjoutVente(NomVente, QuantiteVendue, PrixVente);
            VenteEffectuee?.Invoke(); // Previens les autres user controlel que la vente a été effectuée pour qu'ils puissent réagir en conséquence (actualiser le stock par exemple)

        }
    }
}
