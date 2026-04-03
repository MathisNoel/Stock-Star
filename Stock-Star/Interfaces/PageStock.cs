using Stock_Star.Interfaces;

namespace Stock_Star
{
    public partial class PageStock : UserControl
    {
        // On crée l'objet Gestion Produit
        GestionProduits gestion = new GestionProduits();
        Form1 _parent;

        public PageStock(Form1 parent)
        {
            InitializeComponent();

            // Configuration de la ComboBox
            _parent = parent;

            ActualiserGrille();
            AidesSaisies();
        }

        public void ActualiserGrille()
        {
            try
            {   
                //Remplir la DataGridView
                guna2DataGridView1.DataSource = gestion.ChargerStock();
                //Remplir la ComboBox
                gestion.RemplirCategorie(ComboBoxCategorie);

                ComboBoxCategorie.TextOffset = new Point(-500, 0);

                var ColonneSupprimer = guna2DataGridView1.Columns["BoutonSupprimer"];
                if (ColonneSupprimer != null)
                {
                    ColonneSupprimer.DisplayIndex = guna2DataGridView1.ColumnCount - 1;
                    ColonneSupprimer.Width = 80;
                }

                var ColonneEditer = guna2DataGridView1.Columns["BoutonEditer"];
                if (ColonneEditer != null)
                {
                    ColonneEditer.DisplayIndex = guna2DataGridView1.ColumnCount - 2;
                    ColonneEditer.Width = 80;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur au chargement : " + ex.Message);
            }
        }

        private void AidesSaisies()
        {
            // On utilise la propriété Text pour la ComboBox car PlaceholderText n'existe pas tjs sur Guna Combo
            TxtBoxCategorie.PlaceholderText = "Catégorie";
            TxtBoxNom.PlaceholderText = "Produit";
            TxtBoxEmplacement.PlaceholderText = "Emplacement (optionnel)";
            TxtBoxDescription.PlaceholderText = "Description (optionnel)";

        }

        private void ViderChamps()
        {
            ComboBoxCategorie.Text = ""; // On vide la combo
            TxtBoxCategorie.Text = "";
            TxtBoxNom.Clear();
            TxtBoxEmplacement.Clear();
            TxtBoxDescription.Clear();
        }

        private void BtnAjouter_Click(object sender, EventArgs e)
        {
            // IMPORTANT : On prend le texte de la TextBox (celle qui est devant)
            string Categorie = TxtBoxCategorie.Text.Trim();
            string Nom = TxtBoxNom.Text.Trim();
            string Emplacement = TxtBoxEmplacement.Text.Trim();
            string Description = TxtBoxDescription.Text.Trim();

            // Vérification de base
            if (string.IsNullOrEmpty(Nom) || string.IsNullOrEmpty(Categorie))
            {
                MessageBox.Show("Le nom et la catégorie sont obligatoires !");
                return;
            }

            try
            {
                // On appelle ta méthode de gestion (celle avec la grosse requête SQL simplifiée)
                // Note : On garde les 0, 0 à la fin si ta méthode attend toujours 6 paramètres,
                // mais ils ne seront pas utilisés par notre nouvelle requête SQL.
                gestion.AjoutStock(Categorie, Nom, Emplacement, Description);

                // On rafraîchit la grille ET la ComboBox (via ActualiserGrille)
                ActualiserGrille();

                // On nettoie l'interface
                ViderChamps();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de l'ajout : " + ex.Message);
            }
        }

        private void ClickOnDataGridView(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string nom = guna2DataGridView1.Rows[e.RowIndex].Cells["Nom"].Value?.ToString() ?? "";

            if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "BoutonSupprimer")
            {
                if (!string.IsNullOrEmpty(nom))
                {
                    gestion.SupprimerProduit(nom);
                    ActualiserGrille();
                }
            }

            if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "BoutonEditer")
            {
                if (!string.IsNullOrEmpty(nom))
                {
                    PageModification pagemodification = new PageModification(_parent, nom);
                    _parent.LoadPage(pagemodification);
                }
            }
        }

        // Synchronisation TextBoxCategorie et ComboBoxCategorie
        private void ComboBoxCategorie_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Si l'utilisateur choisit dans la liste, on le met dans la TextBox
            TxtBoxCategorie.Text = ComboBoxCategorie.Text;
        }

        private void TxtBoxCategorie_Click(object sender, EventArgs e)
        {
            // Si on clique sur le texte, on peut aussi ouvrir la liste
            ComboBoxCategorie.DroppedDown = true;
        }
    }
}