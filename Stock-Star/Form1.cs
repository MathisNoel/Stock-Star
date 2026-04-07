using Stock_Star.Interfaces;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Stock_Star
{
    public partial class Form1 : Form
    {
        // On déclare les pages pour pouvoir basculer de l'une à l'autre
        private PageStock pageStock;
        private PageGraphique pageGraphique;
        private PageTransaction pageAchatEtVente;

        // Paramètres Windows pour pouvoir déplacer la fenêtre à la souris
        const int WM_NCHITTEST = 0x84;
        const int HTCLIENT = 0x1;
        const int HTCAPTION = 0x2;
        //ne pas toucher les ligne en haut


        public Form1()
        {
            InitializeComponent();

            // Gestion d'erreur
            try
            {
                // Initialisation des pages
                pageStock = new PageStock(this);
                pageGraphique = new PageGraphique();
                pageAchatEtVente = new PageTransaction();

                // On affiche la page Stock dès l'ouverture
                LoadPage(pageStock);
            }
            catch (Exception ex) // En cas d'erreur, on affiche un message box avec l'erreur
            {
                MessageBox.Show("Erreur au chargement : " + ex.Message);
            }
        }

        // Méthode pour afficher une page dans le panel central
        public void LoadPage(UserControl page)
        {
            if (page != null)
            {
                Panel_Main.Controls.Clear(); // On vide le panel avant d'afficher la nouvelle page
                page.Dock = DockStyle.Fill; // On fait en sorte que la page prenne tout l'espace du panel
                Panel_Main.Controls.Add(page); // On ajoute la page au panel
            }
        }

        // --- NAVIGATION DU MENU (Logo a gauche)---

        private void BtnMenuStock_Click(object sender, EventArgs e)
        {
            LoadPage(pageStock);
        }

        private void BtnMenuGraphique_Click(object sender, EventArgs e)
        {
            LoadPage(pageGraphique);
        }

        private void BtnMenuAchatEtVente_Click(object sender, EventArgs e)
        {
            LoadPage(pageAchatEtVente);
        }

        // --- BOUTONS DE LA BARRE DE TITRE (Bouton Réduire,Agrandir,Quitter)---

        private void BtnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnAgrandirFenetre_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
                WindowState = FormWindowState.Normal;
            else
                WindowState = FormWindowState.Maximized;
        }

        private void BtnCacherFenetre_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        // --- FONCTIONS SYSTÈME (DÉPLACEMENT DE LA FENÊTRE) --- // Ne pas toucher les lignes en dessous

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == WM_NCHITTEST && (int)m.Result == HTCLIENT)
            {
                m.Result = (IntPtr)HTCAPTION;
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x20000; // Permet de réduire via la barre des tâches
                cp.Style |= 0x40000; // Permet d'agrandir via la barre des tâches
                return cp;
            }
        }

        // --- FONCTIONS SYSTÈME (DÉPLACEMENT DE LA FENÊTRE) --- //


    }
}