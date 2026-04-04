using Guna.Charts.WinForms;
using Npgsql; //Commande SQL
using System;
using System.Collections.Generic;
using System.Data; //Type DataTable stocké des tables
using System.Text;
using static System.Collections.Specialized.BitVector32;



namespace Stock_Star.Méthodes
{
    internal class GestionGraphique
    {
        private ConnectionBDD BDD = new ConnectionBDD();

        public DataTable ChargerBeneficesParDate()
        {
            using (var connection = BDD.GetConnection())
            {
                connection.Open();

                string SQL = @"
                    SELECT
                        v.date_vente AS Date,
                        SUM(v.prix_vente_reel - sub_achats.prix_moyen_achat) AS Benefice
                    FROM ventes AS v
                    LEFT JOIN (
                        SELECT
                            id_produit,
                            SUM(prix_achat_unitaire * quantite_achetee) / NULLIF(SUM(quantite_achetee), 0)
                                AS prix_moyen_achat
                        FROM achats
                        GROUP BY id_produit
                    ) sub_achats ON v.id_produit = sub_achats.id_produit
                    GROUP BY v.date_vente
                    ORDER BY v.date_vente;
                ";

                using (var adapter = new NpgsqlDataAdapter(SQL, connection))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }


        public DataTable ChargerTresorerieParDate()
        {
            DataTable dtTresorerie = new DataTable(); // Création d'une nouvelle DataTable pour stocker les données de trésorerie

            DataTable dtBenefices = ChargerBeneficesParDate(); //Appel de la méthode ChargerBeneficesParDate() pour obtenir les bénéfices par date


            dtTresorerie.Columns.Add("Date", typeof(DateOnly)); //Ajout de la colonne "Date" de type DateOnly à la DataTable de trésorerie
            dtTresorerie.Columns.Add("Tresorerie", typeof(decimal)); //Ajout de la colonne "Tresorerie" de type decimal à la DataTable de trésorerie

            decimal cumul = 0; //Valeur de départ de la trésorerie, initialisée à 0

            foreach (DataRow row in dtBenefices.Rows) //Boucle à travers chaque ligne de la DataTable des bénéfices
            {
                DateOnly date = (DateOnly)row["Date"];
                decimal benefice = Convert.ToDecimal(row["Benefice"]);

                cumul += benefice;

                dtTresorerie.Rows.Add(date, cumul);
            }

            return dtTresorerie;

        }



        public float CalculerBeneficeTotal()
        {
            using (var connection = BDD.GetConnection())
            {
                connection.Open();
                string SQL = @"
                    SELECT
                        ROUND(SUM(v.prix_vente_reel - sub_achats.prix_moyen_achat), 2) AS BeneficeTotal
                    FROM ventes AS v
                    LEFT JOIN (
                        SELECT
                            id_produit,
                            SUM(prix_achat_unitaire * quantite_achetee) / NULLIF(SUM(quantite_achetee), 0)
                                AS prix_moyen_achat
                        FROM achats
                        GROUP BY id_produit
                    ) sub_achats ON v.id_produit = sub_achats.id_produit;
                ";
                using (var command = new NpgsqlCommand(SQL, connection))
                {
                    object result = command.ExecuteScalar();                            //ExecuteScalar() est une méthode de la classe NpgsqlCommand qui exécute la requête SQL et retourne la première colonne de la première ligne du résultat. Dans ce cas, il retourne le bénéfice total calculé par la requête SQL.
                    return result != DBNull.Value ? Convert.ToSingle(result) : 0f;      //Cette ligne à été genrerez par copilote elle vérifie si le résultat de la requête est différent de DBNull.Value (ce qui signifie qu'il y a une valeur valide), et si c'est le cas, il convertit cette valeur en float. Sinon, il retourne 0f (0 en float) pour éviter les erreurs de conversion.
                }
            }
        }



        public float CalculerChiffreAffaire()
        {
            using (var connection = BDD.GetConnection())
            {
                connection.Open();
                string SQL = @"
                    SELECT
                        ROUND(SUM(v.prix_vente_reel), 2) AS ChiffreAffaire
                    FROM ventes AS v
                ";
                using (var command = new NpgsqlCommand(SQL, connection))
                {
                    object result = command.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToSingle(result) : 0f;
                }
            }
        }



        public DataTable ChargerBeneficesEntreDates(DateOnly debut, DateOnly fin)
        {
            using (var connection = BDD.GetConnection())
            {
                connection.Open();

                string SQL = @"
                    SELECT
                        v.date_vente AS Date,
                        SUM(v.prix_vente_reel - sub_achats.prix_moyen_achat) AS Benefice
                    FROM ventes AS v
                    LEFT JOIN (
                        SELECT
                            id_produit,
                            SUM(prix_achat_unitaire * quantite_achetee) / NULLIF(SUM(quantite_achetee), 0)
                                AS prix_moyen_achat
                        FROM achats
                        GROUP BY id_produit
                    ) sub_achats ON v.id_produit = sub_achats.id_produit
                    WHERE v.date_vente BETWEEN @debut AND @fin
                    GROUP BY v.date_vente
                    ORDER BY v.date_vente;
                 ";

                using (var cmd = new NpgsqlCommand(SQL, connection))
                {
                    cmd.Parameters.AddWithValue("@debut", debut.ToDateTime(TimeOnly.MinValue));
                    cmd.Parameters.AddWithValue("@fin", fin.ToDateTime(TimeOnly.MaxValue));

                    using (var adapter = new NpgsqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }




        public DataTable ChargerTresorerieEntreDate(DateOnly debut, DateOnly fin)
        {
            DataTable dtTresorerie = new DataTable(); // Création d'une nouvelle DataTable pour stocker les données de trésorerie

            DataTable dtBenefices = ChargerBeneficesEntreDates(debut, fin); //Appel de la méthode ChargerBeneficesParDate() pour obtenir les bénéfices par date


            dtTresorerie.Columns.Add("Date", typeof(DateOnly)); //Ajout de la colonne "Date" de type DateOnly à la DataTable de trésorerie
            dtTresorerie.Columns.Add("Tresorerie", typeof(decimal)); //Ajout de la colonne "Tresorerie" de type decimal à la DataTable de trésorerie

            decimal cumul = 0; //Valeur de départ de la trésorerie, initialisée à 0

            foreach (DataRow row in dtBenefices.Rows) //Boucle à travers chaque ligne de la DataTable des bénéfices
            {
                DateOnly date = (DateOnly)row["Date"];
                decimal benefice = Convert.ToDecimal(row["Benefice"]);

                cumul += benefice;

                dtTresorerie.Rows.Add(date, cumul);
            }

            return dtTresorerie;

        }










    }
}
