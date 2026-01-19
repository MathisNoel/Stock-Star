using System;
using System.Collections.Generic;
using System.Windows.Forms;
using project_TESTC_sharp; 


//les include sont automatique à ce qu'il parrait


namespace project_TESTC_sharp
{

    //List<T> nomDeLaListe = new List<T>(); prototype pour une liste

    public partial class OBSM : Form
    {
        public class Produit  //
        {
            public string Nom { get; set; }
            public decimal PrixAchat { get; set; }
        }

        List<Produit> listeProduits = new List<Produit>();


        string StringTxtBoxPrix = "Entrez un prix";
        string StringTxtBoxProduit = "Entrez un produit";


        public OBSM()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TxtBoxPrix.Text = StringTxtBoxPrix; // Au démarage on vient charger "Entrez un prix"
            TxtBoxPrix.ForeColor = Color.Gray; //Couleur du texte en gris

            TxtBoxNomDuProduit.Text = StringTxtBoxProduit;
            TxtBoxNomDuProduit.ForeColor = Color.Gray;
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged_2(object sender, EventArgs e)
        {

        }

        //TxtBoxNomDuProduit.Focus(); sert à selectioner TxtBoxProduit on click


        private void TxtBoxPrix_Enter(object sender, EventArgs e) //Enter si user click sur 
        {
            if (TxtBoxPrix.Text == StringTxtBoxPrix)
            {
                TxtBoxPrix.Text = "";
                TxtBoxPrix.ForeColor = Color.Black;
            }
        }

        private void TxtBoxPrix_Leave(object sender, EventArgs e)
        {
            if (TxtBoxPrix.Text == "")
            {
                TxtBoxPrix.Text = StringTxtBoxPrix;
                TxtBoxPrix.ForeColor = Color.Gray;
            }

        }

        private void TxtBoxNomDuProduit_Leave(object sender, EventArgs e)
        {
            if (TxtBoxNomDuProduit.Text == "")
            {
                TxtBoxNomDuProduit.Text = StringTxtBoxProduit;
                TxtBoxNomDuProduit.ForeColor = Color.Gray;
            }

        }

        private void TxtBoxNomDuProduit_Enter(object sender, EventArgs e)
        {
            if (TxtBoxNomDuProduit.Text == StringTxtBoxProduit)
            {
                TxtBoxNomDuProduit.Text = "";
                TxtBoxNomDuProduit.ForeColor = Color.Black;
            }

        }

        private void BtnAfficher_Click(object sender, EventArgs e)
        {
            //Récuperez le contenue des text box
            string nom = TxtBoxPrix.Text;
            string prixText = TxtBoxNomDuProduit.Text;

            decimal prix;
            MessageBox.Show("Texte reçu : [" + prixText + "]");
            if (!decimal.TryParse(prixText, out prix))
            {
                MessageBox.Show("Le prix doit être un nombre");
                return;
            }



            Produit p = new Produit
            {
                Nom = nom,
                PrixAchat = prix
            };

            listeProduits.Add(p);

            DataGridView1.DataSource = null; // reset
            DataGridView1.DataSource = listeProduits;


        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void OBSM_DoubleClick(object sender, EventArgs e)
        {
            //test
            ///MOI
        }
    }
}
