using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

// Remarques :
// - Le préprocesseur C# n'a pas d'instruction `#include`. La ligne `#include "Class2.cs"` provoque l'erreur CS1024.
// - Pour inclure `Class2.cs`, ajoutez simplement le fichier au projet (Visual Studio > Ajouter > Élément existant) ; il sera compilé avec les autres fichiers.
// - Ici, on retire la directive invalide et on conserve le reste du fichier intact.


    

using System.Data;
using System.Windows.Forms;
using Npgsql;

namespace Stock_Star
{
    public partial class Form1 : Form
    {

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
        { // Parametre pour changer la taille et crée des bord pour notre form de la fenetre ne pas toucher
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // WS_MINIMIZEBOX
                cp.Style |= 0x40000; // WS_MAXIMIZEBOX
                return cp;
            }
        }

        private PageStock pageStock; // Création d'une instance de la classe PageStock pour pouvoir l'utiliser dans le Form1
        private PageGraphique pageGraphique; // Création d'une instance de la classe PageGraphique pour pouvoir l'utiliser dans le Form1
        string StringTxtBoxPrix = "Entrez un prix";
        string StringTxtBoxProduit = "Entrez un produit";
        public Form1()
        {
            InitializeComponent();
            LoadData();
        }
        private void LoadData()
        {
            ConnectionBDD bdd = new ConnectionBDD();
            NpgsqlConnection con = bdd.GetConnection();
            con.Open();

            if (con.State == ConnectionState.Open)
            {
                string cmd_sql = "SELECT type,nom,quantite,prix,date FROM produits";
                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd_sql, con);
                DataTable table = new DataTable();
                adapter.Fill(table);

                // On lie les données au DataGridView standard
                dataGridView1.DataSource = table;

                con.Close();
            }
        }

        private void BtnMenuGraphique_Click(object sender, EventArgs e)
        {
            LoadPage(pageGraphique); // Chargement de la page PageGraphique dans le Form1 lorsque l'on clique sur le bouton graphique
        }

        private void BtnMenuStock_Click(object sender, EventArgs e)
        {
            LoadPage(pageStock);
        }
    }
}