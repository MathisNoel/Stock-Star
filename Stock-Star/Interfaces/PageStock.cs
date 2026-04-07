using Stock_Star.Interfaces;

namespace Stock_Star
{
    public partial class PageStock : UserControl
    {
        // On crée l'objet Gestion Produit
        GestionProduits gestion = new GestionProduits();
        Form1 _parent; //Un parent c'est la page principale, on en a besoin pour pouvoir appeler la page de modification depuis la page stock (pour envoyer les infos du produit à modifier)

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
                TxtBoxCategorie.Clear();

                ComboBoxCategorie.TextOffset = new Point(50000, 0);

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

            try                                                                                             //try on essaye d'exécuter le code, catch si il y a une erreur on l'attrape et on  affiche un message box avec l'erreur
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

        // Détection lors d'un click sur la DataGridView (pour détecter bouton supprimer ou éditer)
        // Détection lors d'un click sur la DataGridView (pour détecter bouton supprimer ou éditer)
        private void ClickOnDataGridView(object sender, DataGridViewCellEventArgs e)
        {
            // On ignore le clic sur l'en-tête (index -1)
            if (e.RowIndex < 0) return;

            // Récupération de la ligne cliquée
            DataGridViewRow row = guna2DataGridView1.Rows[e.RowIndex];

            // Récupération du nom (obligatoire pour identifier le produit)
            string nom = row.Cells["Nom"].Value?.ToString() ?? "";

            // CAS 1 : BOUTON SUPPRIMER
            if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "BoutonSupprimer")
            {
                if (!string.IsNullOrEmpty(nom))
                {
                    gestion.SupprimerProduit(nom);
                    ActualiserGrille();
                }
            }

            // CAS 2 : BOUTON ÉDITER
            if (guna2DataGridView1.Columns[e.ColumnIndex].Name == "BoutonEditer")
            {
                if (!string.IsNullOrEmpty(nom))
                {
                    // Récupération sécurisée des autres colonnes (on gère le null avec ??)
                    string cat = row.Cells["Catégorie"].Value?.ToString() ?? "";
                    string emp = row.Cells["Emplacement"].Value?.ToString() ?? "";
                    string desc = row.Cells["Description"].Value?.ToString() ?? "";

                    // On envoie TOUTES les infos à la page de modification
                    _parent.LoadPage(new PageModification(_parent, nom, cat, emp, desc));//parent pour pouvoir recharger la page stock après modification
                }                                                                        //Demande à Form1 de Charger la page de modification et lui passe les infos du produit.
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

        private void PageStock_VisibleChanged(object sender, EventArgs e)
        {
            // On vérifie que la page devient visible (et pas l'inverse)
            if (this.Visible)
            {
                ActualiserGrille();
            }
        }
    }
}