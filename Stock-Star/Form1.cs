using Stock_Star.Interfaces;
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
        private PageAchatEtVente pageAchatEtVente;
        string StringTxtBoxPrix = "Entrez un prix";
        string StringTxtBoxProduit = "Entrez un produit";
        public Form1()
        {
            InitializeComponent();
            pageStock = new PageStock(this);// Initialisation de l'instance de PageStock
            pageGraphique = new PageGraphique();// Initialisation de l'instance de PageGraphique
            pageAchatEtVente = new PageAchatEtVente(); // Initialisation de l'instance de PageAchatEtVente
            LoadPage(pageStock); // Chargement de la page PageStock dans le Form1 Page par default

        }

        public void LoadPage(UserControl page)
        {
            Panel_Main.Controls.Clear();
            page.Dock = DockStyle.Fill;
            Panel_Main.Controls.Add(page);
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


        private void BtnCacherFenetre_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

        private void BtnMenuGraphique_Click(object sender, EventArgs e)
        {
            LoadPage(pageGraphique); // Chargement de la page PageGraphique dans le Form1 lorsque l'on clique sur le bouton graphique
        }

        private void BtnMenuStock_Click(object sender, EventArgs e)
        {
            LoadPage(pageStock);
        }

        private void BtnMenuAchatEtVente_Click(object sender, EventArgs e)
        {
            LoadPage(pageAchatEtVente);
        }
    }
}
