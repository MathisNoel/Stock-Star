using System.Data;
using Npgsql;

namespace Stock_Star
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            LoadData();
        }


        private void BtnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnAgrandirFenetre_Click(object sender, EventArgs e)
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

        private void BtnCacherFenetre_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        //Charger les données présentes dans la Database
        private void LoadData()
        {
            //On appelle la classe Connection BDD pour se connecter a celle-ci
            ConnectionBDD bdd = new ConnectionBDD();

            NpgsqlConnection con = bdd.GetConnection();

            //On ouvre la Database  
            con.Open();

            if (con.State == ConnectionState.Open)
            {
                //Commande SQL a effectuer pour afficher les données présent dans la table Produits (sauf l'id)
                string cmd_sql = "SELECT type,nom,quantite,prix,date FROM produits";

                NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(cmd_sql, con);

                DataTable table = new DataTable();

                adapter.Fill(table);

                con.Close();
            }
        }
    }
}
