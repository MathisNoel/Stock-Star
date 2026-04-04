using Guna.Charts.WinForms;
using Stock_Star.Méthodes;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Stock_Star
{
    public partial class PageGraphique : UserControl
    {
        private GestionGraphique gestionGraphique = new GestionGraphique();

        public PageGraphique()
        {
            InitializeComponent();
        }

        private void PageGraphique_Load(object sender, EventArgs e)
        {
            AfficherGraphiqueBenefices();
            AfficherGraphiqueTresorerie();
            TextBoxBenefice.Text = gestionGraphique.CalculerBeneficeTotal().ToString("C2");
            TextBoxVariableCA.Text = gestionGraphique.CalculerChiffreAffaire().ToString("C2");
        }

        /*private void AfficherGraphiqueBenefices()
        {
            DataTable dt = gestionGraphique.ChargerBeneficesParDate();

            GunaGraphiqueBenefice.Datasets.Clear();
            GunaGraphiqueBenefice.Labels.Clear();

            var dataset = new GunaLineDataset();
            dataset.Label = "Bénéfice total";
            dataset.BorderWidth = 3;
            dataset.BorderColor = Color.RoyalBlue;

            foreach (DataRow row in dt.Rows)
            {
                DateTime date = Convert.ToDateTime(row["Date"]);
                decimal benefice = Convert.ToDecimal(row["Benefice"]);

                GunaGraphiqueBenefice.Labels.Add(date.ToString("dd/MM"));
                dataset.DataPoints.Add(benefice);
            }

            GunaGraphiqueBenefice.Datasets.Add(dataset);
            GunaGraphiqueBenefice.Update();
        }*/

        private void AfficherGraphiqueBenefices()
        {
            DataTable dt = gestionGraphique.ChargerBeneficesParDate();

            GunaGraphiqueBenefice.Datasets.Clear();

            var dataset = new GunaLineDataset();
            dataset.Label = "Bénéfice total";
            dataset.BorderWidth = 3;
            dataset.BorderColor = Color.RoyalBlue;

            // Personnalisation du graphique
            /*
            GunaGraphiqueBenefice.YAxes.GridLines.Color = Color.FromArgb(40, 40, 40);
            GunaGraphiqueBenefice.XAxes.GridLines.Color = Color.FromArgb(40, 40, 40);
            GunaGraphiqueBenefice.Legend.LabelForeColor = Color.White;
            GunaGraphiqueBenefice.Title.ForeColor = Color.White;
            GunaGraphiqueBenefice.BackColor = Color.FromArgb(30, 30, 30);
            */

            dataset.BorderColor = Color.RoyalBlue;
            dataset.BorderWidth = 3;
            dataset.PointStyle = PointStyle.Circle;
            dataset.PointBorderWidth = 1;


            ;



            foreach (DataRow row in dt.Rows)
            {
                DateOnly d = (DateOnly)row["Date"];
                DateTime date = d.ToDateTime(TimeOnly.MinValue);

                decimal benefice = Convert.ToDecimal(row["Benefice"]);

                dataset.DataPoints.Add(date.ToString("dd/MM"), (double)benefice);
            }

            GunaGraphiqueBenefice.Datasets.Add(dataset);
            GunaGraphiqueBenefice.Update();
        }

        private void AfficherGraphiqueTresorerie()
        {
            DataTable dt = gestionGraphique.ChargerTresorerieParDate();

            GunaGraphiqueTresorerie.Datasets.Clear();

            var dataset = new GunaLineDataset();
            dataset.Label = "Trésorerie cumulée";

            // 🎨 Couleur différente pour ce graphique
            dataset.BorderColor = Color.FromArgb(255, 180, 0); // OR
            dataset.FillColor = Color.FromArgb(60, 255, 180, 0); //fromArgb(Alpha, R, G, B) pour une couleur OR semi-transparente alpha correspond a la transparence de la couleur, plus elle est proche de 255 plus la couleur est opaque, plus elle est proche de 0 plus la couleur est transparente

            dataset.BorderWidth = 3;
            dataset.PointStyle = PointStyle.Circle;
            dataset.PointRadius = 4;

            dataset.PointFillColors.Add(Color.DarkGoldenrod);// OR
            dataset.PointBorderColors.Add(Color.DarkGoldenrod); // OR


            foreach (DataRow row in dt.Rows)
            {
                DateOnly d = (DateOnly)row["Date"];
                DateTime date = d.ToDateTime(TimeOnly.MinValue);

                decimal treso = Convert.ToDecimal(row["Tresorerie"]);

                dataset.DataPoints.Add(date.ToString("dd/MM"), (double)treso);
            }

            GunaGraphiqueTresorerie.Datasets.Add(dataset);
            GunaGraphiqueTresorerie.Update();
        }

        private void TextBoxVariableCA_Click(object sender, EventArgs e)
        {
            AfficherGraphiqueBenefices();
            AfficherGraphiqueTresorerie();
            TextBoxBenefice.Text = gestionGraphique.CalculerBeneficeTotal().ToString("C2");
            TextBoxVariableCA.Text = gestionGraphique.CalculerChiffreAffaire().ToString("C2");
        }

        private void guna2DateTimePicker1Debut_ValueChanged(object sender, EventArgs e)
        {
            FiltrerParPeriode();

        }

        private void guna2DateTimePicker1Fin_ValueChanged(object sender, EventArgs e)
        {
            FiltrerParPeriode();
        }

        private void FiltrerParPeriode()
        {
            DateOnly debut = DateOnly.FromDateTime(guna2DateTimePicker1Debut.Value);
            DateOnly fin = DateOnly.FromDateTime(guna2DateTimePicker1Fin.Value);

            AfficherGraphiqueBeneficesDateSpecifique(debut, fin);
            AfficherGraphiqueTresorerieDateSpecifique(debut, fin);

        }

        private void AfficherGraphiqueBeneficesDateSpecifique(DateOnly debut, DateOnly fin)
        {
            DataTable dt = gestionGraphique.ChargerBeneficesEntreDates(debut, fin);

            GunaGraphiqueBenefice.Datasets.Clear();

            var dataset = new GunaLineDataset();
            dataset.Label = "Bénéfice total";
            dataset.BorderWidth = 3;
            dataset.BorderColor = Color.RoyalBlue;


            dataset.BorderColor = Color.RoyalBlue;
            dataset.BorderWidth = 3;
            dataset.PointStyle = PointStyle.Circle;
            dataset.PointBorderWidth = 1;



            foreach (DataRow row in dt.Rows)
            {
                DateOnly d = (DateOnly)row["Date"];
                DateTime date = d.ToDateTime(TimeOnly.MinValue);

                decimal benefice = Convert.ToDecimal(row["Benefice"]);

                dataset.DataPoints.Add(date.ToString("dd/MM"), (double)benefice);
            }

            GunaGraphiqueBenefice.Datasets.Add(dataset);
            GunaGraphiqueBenefice.Update();
        }







        private void AfficherGraphiqueTresorerieDateSpecifique(DateOnly debut, DateOnly fin)
        {
            DataTable dt = gestionGraphique.ChargerTresorerieEntreDate(debut, fin);

            GunaGraphiqueTresorerie.Datasets.Clear();

            var dataset = new GunaLineDataset();
            dataset.Label = "Trésorerie cumulée";

            // 🎨 Couleur différente pour ce graphique
            dataset.BorderColor = Color.FromArgb(255, 180, 0); // OR
            dataset.FillColor = Color.FromArgb(60, 255, 180, 0); //fromArgb(Alpha, R, G, B) pour une couleur OR semi-transparente alpha correspond a la transparence de la couleur, plus elle est proche de 255 plus la couleur est opaque, plus elle est proche de 0 plus la couleur est transparente

            dataset.BorderWidth = 3;
            dataset.PointStyle = PointStyle.Circle;
            dataset.PointRadius = 4;

            dataset.PointFillColors.Add(Color.DarkGoldenrod);// OR
            dataset.PointBorderColors.Add(Color.DarkGoldenrod); // OR


            foreach (DataRow row in dt.Rows)
            {
                DateOnly d = (DateOnly)row["Date"];
                DateTime date = d.ToDateTime(TimeOnly.MinValue);

                decimal treso = Convert.ToDecimal(row["Tresorerie"]);

                dataset.DataPoints.Add(date.ToString("dd/MM"), (double)treso);
            }

            GunaGraphiqueTresorerie.Datasets.Add(dataset);
            GunaGraphiqueTresorerie.Update();
        }


    }
}