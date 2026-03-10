using System.Data; //Type DataTable stocké des tables
using Npgsql; //Commande SQL

namespace Stock_Star
{
    internal class GestionProduits
    {
        //On crée un champ qui va contenir le moyen de se connecter à la BDD
        private ConnectionBDD BDD=new ConnectionBDD();

        //On crée une méthode qui permettre de récupérer les données de la BDD (de type DataTable)
        public DataTable ChargerStock()
        {
            //Variable interne pour ne pas être connecté a la BDD en permanence et Fermeture de liaison une fois la méthode finie
            using(NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                //On définit la requête SQL qu'on va réaliser
                string SQL ="SELECT " +
                            "p.nom_produit AS nom, " +
                            "p.description AS description, " +
                            "c.nom_categorie AS categorie, " +
                            "a.prix_achat_unitaire, " +
                            //Stock actuel (Somme achat - Somme vente)
                            "(SUM(DISTINCT a.quantite_achetee) - COALESCE(SUM(v.quantite_vendue), 0)) AS Stock_Actuel " +
                            "FROM produits p " +
                            "INNER JOIN categories c ON p.id_categorie=c.id_categorie " +
                            "LEFT JOIN achats a ON p.id_produit = a.id_produit " +
                            "LEFT JOIN ventes v ON p.id_produit = v.id_produit " +
                            "GROUP BY p.id_produit, p.nom_produit, p.description, c.nom_categorie, a.prix_achat_unitaire;";

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(SQL, connection)) //On créer un adapter pour lier les informations de la requête SQL avec une DataTable
                {
                    //On créer un type DataTable
                    DataTable dt = new DataTable();
                    adapter.Fill(dt); //On remplie la DataTable avec notre requête SQL
                    return dt;
                }
            }
            
        }
    }
}
