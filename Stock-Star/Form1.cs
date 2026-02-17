using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

// Remarques :
// - Le préprocesseur C# n'a pas d'instruction `#include`. La ligne `#include "Class2.cs"` provoque l'erreur CS1024.
// - Pour inclure `Class2.cs`, ajoutez simplement le fichier au projet (Visual Studio > Ajouter > Élément existant) ; il sera compilé avec les autres fichiers.
// - Ici, on retire la directive invalide et on conserve le reste du fichier intact.


    


namespace Stock_Star
{
    public partial class Form1 : Form
    {
        BindingList<Produit> stock = new BindingList<Produit>(); //création d'une liste de produit qui va nous permettre de stocker les produits que l'on ajoute dans le DataGridView


        public Form1()
        {
            InitializeComponent();
        }

        string StringTxtBoxPrix = "Entrez un prix";
        string StringTxtBoxProduit = "Entrez un produit";

        /*private void Form1_Load(object sender, EventArgs e)
        {
            stock.Add(new Produit
            {
                Nom = "TEST",
                PrixAchat = 10,
                PrixVente = 20
            });

            this.FormBorderStyle = FormBorderStyle.None;

            TxtBoxPrice.Text = StringTxtBoxPrix; // Au démarage on vient charger "Entrez un prix"
            TxtBoxPrice.ForeColor = Color.Gray; //Couleur du texte en gris

            TxtBoxObjet.Text = StringTxtBoxProduit;
            TxtBoxObjet.ForeColor = Color.Gray;

            //guna2DataGridView1.AutoGenerateColumns = false;
            guna2DataGridView1.DataSource = stock;
        }*/ //Bug ici

        const int WM_NCHITTEST = 0x84; // Parametre pour changer la taille de la fenetre ne pas toucher
        const int HTCLIENT = 0x1;       // Parametre pour changer la taille de la fenetre ne pas toucher
        const int HTCAPTION = 0x2;      //parametre pour changer la taille de la fenetre ne pas toucher

        protected override void WndProc(ref Message m) // parametre pour changer la taille de la fenetre ne pas toucher 
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST)
            {
                if ((int)m.Result == HTCLIENT)
                {
                    m.Result = (IntPtr)HTCAPTION;
                }
            }
        }

        protected override CreateParams CreateParams
        { // Parametre pour changer la taille de la fenetre ne pas toucher
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // WS_MINIMIZEBOX
                cp.Style |= 0x40000; // WS_MAXIMIZEBOX
                return cp;
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)                                         // Permet de fermer la fentre lorsque on clique sur le bouton fermer
        {
            Application.Exit();
        }

        private void BtnAgrandirFenetre_Click(object sender, EventArgs e)                               //Permet d'agrandir la fentre lorsque on clique sur le bouton agrandir, et de la remettre à sa taille normal lorsque l'on reclique dessus
        {
            if (WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;

            }
            else
            {
                WindowState = FormWindowState.Maximized;
            }
        }

        /*private void form1_Load(object sender, EventArgs e) // demander des explication à chatGPT for this part
        {
            this.FormBorderStyle = FormBorderStyle.None;
        }*/

        private void BtnCacherFenetre_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            string Prix = TxtBoxPrice.Text;
            string Nom = TxtBoxObjet.Text;


                                                                                                            /*if (!decimal.TryParse(TxtBoxPrice.Text, out decimal prix))
                                                                                                            {
                                                                                                                MessageBox.Show("Prix invalide");
                                                                                                                return;
                                                                                                            }

                                                                                                            guna2DataGridView1.Rows.Add
                                                                                                            (
                                                                                                            TxtBoxObjet.Text,  // Nom
                                                                                                            Prix,              // Prix Achat
                                                                                                            null,              // Prix Vente
                                                                                                            null               // Benefice
                                                                                                            );

                                                                                                            TxtBoxObjet.Clear();
                                                                                                            TxtBoxPrice.Clear();*/

            if (!decimal.TryParse(TxtBoxPrice.Text, out decimal prix))                                      // On vérifie que le prix entré est bien un nombre décimal, sinon on affiche un message d'erreur et on arrête l'exécution de la fonction
            {
                MessageBox.Show("Prix invalide");
                return;
            }

            stock.Add(new Produit                                                                           // On ajoute un nouveau produit à la liste de stock
            {
                Nom = TxtBoxObjet.Text,
                PrixAchat = prix,
                PrixVente = null
            });

            TxtBoxObjet.Clear();
            TxtBoxPrice.Clear();
        }

        private void guna2DataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void TxtBoxPrice_Leave(object sender, EventArgs e)                                          // Les 4 fonction suivant sert à changer la couleur dans le text box pour indiquer à l'utilisateur qu'il doit entrer un prix et un produit, et aussi pour remettre le text "Entrez un prix" et "Entrez un produit"
        {
            if (TxtBoxPrice.Text == "")
            {
                TxtBoxPrice.Text = StringTxtBoxPrix;
                TxtBoxPrice.ForeColor = Color.Gray;
            }
        }

        private void TxtBoxPrice_Enter(object sender, EventArgs e)
        {
            if (TxtBoxPrice.Text == StringTxtBoxPrix)
            {
                TxtBoxPrice.Text = "";
                TxtBoxPrice.ForeColor = Color.Black;
            }
        }

        private void TxtBoxObjet_Leave(object sender, EventArgs e)
        {
            if (TxtBoxObjet.Text == "")
            {
                TxtBoxObjet.Text = StringTxtBoxProduit;
                TxtBoxObjet.ForeColor = Color.Gray;
            }

        }

        private void TxtBoxObjet_Enter(object sender, EventArgs e)
        {
            if (TxtBoxObjet.Text == StringTxtBoxProduit)
            {
                TxtBoxObjet.Text = "";
                TxtBoxObjet.ForeColor = Color.Black;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            {
                stock.Add(new Produit
                {
                    Nom = "TEST",
                    PrixAchat = 10,
                    PrixVente = 20
                });

                this.FormBorderStyle = FormBorderStyle.None;

                TxtBoxPrice.Text = StringTxtBoxPrix; // Au démarage on vient charger "Entrez un prix"
                TxtBoxPrice.ForeColor = Color.Gray; //Couleur du texte en gris

                TxtBoxObjet.Text = StringTxtBoxProduit;
                TxtBoxObjet.ForeColor = Color.Gray;

                //guna2DataGridView1.AutoGenerateColumns = false;
                guna2DataGridView1.DataSource = stock;
            }
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
