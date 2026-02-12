using System;
using System.Data;
using System.Windows.Forms;
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
    }
}