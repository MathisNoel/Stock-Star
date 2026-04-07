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
            TxtBoxQuantitePageVente.PlaceholderText = "Quantité";
            TxtBoxPricePageVente.PlaceholderText = "Prix de vente (/u)";
            TxtBoxDatePageVente.PlaceholderText = "Date jj/mm/aaaa";
            //Ligne Achat
            TxtBoxQuantitePageAchat.PlaceholderText = "Quantité";
            TxtBoxPricePageAchat.PlaceholderText = "Prix d'achat (/u)";
            TxtBoxDatePageAchat.PlaceholderText = "Date jj/mm/aaaa";
        }

        private void ViderChamps()
        {
            // Ligne Vente
            ComboBoxNomPageVente.SelectedIndex = -1;
            TxtBoxDatePageVente.Clear();
            TxtBoxQuantitePageVente.Clear();
            TxtBoxPricePageVente.Clear();

            // Ligne Achat
            ComboBoxNomPageAchat.SelectedIndex = -1;
            TxtBoxDatePageAchat.Clear();
            TxtBoxQuantitePageAchat.Clear();
            TxtBoxPricePageAchat.Clear();
        }
        public void ChargerProduits(ComboBox cb_produit)
        {
            gestion.RemplirNomProduit(cb_produit);
            // On remplit la combo box nom produit avec les différents nom produit qui existent dans la BDD.
            cb_produit.DisplayMember = "nom_produit"; 
            // On séléctionne aucun item (par défaut)
            cb_produit.SelectedIndex = -1;
        }
        public void ActualiserGrille()
        {
            // Charger les deux Combobox Nom_Produit (Achat et Ventes)
            ChargerProduits(ComboBoxNomPageAchat);
            ChargerProduits(ComboBoxNomPageVente);

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
            DataGridView_Achats.DataSource = gestion.ChargerLesAchats();
            // On met les boutons supprimer tout à droite
            var ColonneSupprimerA = DataGridView_Achats.Columns["BoutonSupprimerAchat"];
            if (ColonneSupprimerA != null)
            {
                ColonneSupprimerA.DisplayIndex = DataGridView_Achats.ColumnCount - 1;
                ColonneSupprimerA.Width = 80;
            }
        }

        //######################
        // Bouton Vendre / Achat
        //######################

        private void BtnVendre_Click(object sender, EventArgs e)
        {
            // 1. Récupération brute des textes
            string nomVente = ComboBoxNomPageVente.Text;
            // Et vérification qu'on a séléctionné quelque chose
            if (string.IsNullOrEmpty(nomVente) || nomVente == "Nom") // "Nom" = placeholder de la combo box
            {
                MessageBox.Show("Veuillez sélectionner un produit valide avant de vendre !");
                return;
            }

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
            if (!decimal.TryParse(prixTexte, out decimal prixVente) || prixVente < 0)
            {
                MessageBox.Show("Veuillez saisir un prix valide.");
                return;
            }

            // 6. Exécution
            try
            {
                // On appelle la méthode SQL avec les bonnes variables castées
                bool succes = gestion.AjoutVente(nomVente, qteFinale, prixVente, dateVente);
                if (succes)
                {
                    ViderChamps();
                    ActualiserGrille();
                }
                else
                {
                    MessageBox.Show("Stock insuffisant pour réaliser cette vente !");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout : " + ex.Message);
            }
        }

        private void BtnAcheter_Click(object sender, EventArgs e)
        {
            // 1. Récupération du nom (ComboBox spécifique aux Achats)
            string nomAchat = ComboBoxNomPageAchat.Text;

            // Vérification de la sélection
            if (string.IsNullOrEmpty(nomAchat) || nomAchat == "Nom")
            {
                MessageBox.Show("Veuillez sélectionner un produit valide pour l'achat !");
                return;
            }

            // 2. Parsing de la Date
            DateTime dateAchat;
            string dateTexte = TxtBoxDatePageAchat.Text.Trim();

            if (string.IsNullOrWhiteSpace(dateTexte))
            {
                dateAchat = DateTime.Now; // Date du jour par défaut
            }
            else if (!DateTime.TryParse(dateTexte, out dateAchat))
            {
                MessageBox.Show("La date d'achat est invalide.");
                return;
            }

            // 3. Parsing de la Quantité (int)
            if (!int.TryParse(TxtBoxQuantitePageAchat.Text, out int qteAchat))
            {
                MessageBox.Show("Veuillez saisir un nombre entier pour la quantité achetée.");
                return;
            }

            // 4. Parsing du Prix d'achat (decimal)
            string prixTexte = TxtBoxPricePageAchat.Text.Replace('.', ',');
            if ((!decimal.TryParse(prixTexte, out decimal prixAchat)) || prixAchat<0)
            {
                MessageBox.Show("Veuillez saisir un prix d'achat valide.");
                return;
            }

            // 5. Exécution
            try
            {
                // On appelle la méthode SQL dédiée aux achats
                gestion.AjoutAchat(nomAchat, qteAchat, prixAchat, dateAchat);

                // 6. Nettoyage et Mise à jour de l'interface
                ViderChamps();
                ActualiserGrille();

                // Petit message pour confirmer que c'est bon
                // MessageBox.Show("Achat enregistré avec succès !"); 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'enregistrement de l'achat : " + ex.Message);
            }
        }

        // ################
        // Bouton Supprimer
        // ################

        // DataGridView Ventes
        private void ClickOnGrilleVentes(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // On ignore le clic sur l'en-tête (index -1)

            // On vérifie si c'est la colonne du bouton supprimer
            if (DataGridView_Ventes.Columns[e.ColumnIndex].Name == "BoutonSupprimerVente")
            {
                // On récupère l'ID de la ligne
                int idVente = Convert.ToInt32(DataGridView_Ventes.Rows[e.RowIndex].Cells["ID Vente"].Value);

                // Suppression directe et rafraîchissement
                gestion.SupprimerVente(idVente);
                ActualiserGrille();
            }
        }

        // DataGridView Achat
        private void ClickOnGrilleAchats(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return; // On ignore le clic sur l'en-tête (index -1)

            // On vérifie si c'est la colonne du bouton supprimer
            if (DataGridView_Achats.Columns[e.ColumnIndex].Name == "BoutonSupprimerAchat")
            {
                // On récupère l'ID de la ligne
                int idAchat = Convert.ToInt32(DataGridView_Achats.Rows[e.RowIndex].Cells["ID Achat"].Value);

                // Suppression directe et rafraîchissement
                gestion.SupprimerAchat(idAchat);
                ActualiserGrille();
            }
        }

        private void PageTransaction_VisibleChanged(object sender, EventArgs e)
        {
            // On vérifie que la page devient visible (et pas l'inverse)
            if (this.Visible)
            {
                ActualiserGrille();
            }
        }
    }
}
