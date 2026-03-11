using System.Data; //Type DataTable stocké des tables
using Npgsql; //Commande SQL

namespace Stock_Star
{
    internal class GestionProduits
    {
        //On crée un champ qui va contenir le moyen de se connecter à la BDD
        private ConnectionBDD BDD = new ConnectionBDD();

        //On crée une méthode qui permettre de récupérer les données de la BDD (de type DataTable)
        public DataTable ChargerStock()
        {
            //Variable interne pour ne pas être connecté a la BDD en permanence et Fermeture de liaison une fois la méthode finie
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                //On définit la requête SQL qu'on va réaliser
                string SQL = """      
                    SELECT 
                        c.nom_categorie AS "Catégorie",
                        p.nom_produit AS "Nom",
                        (COALESCE(sub_achats.total_qte_achats, 0) - COALESCE(sub_ventes.total_qte_ventes, 0)) AS "Quantité",
                        ROUND(COALESCE(sub_achats.prix_moyen_achat, 0)::numeric, 2) AS "Prix achat",
                        ROUND(COALESCE(sub_ventes.prix_moyen_vente, 0)::numeric, 2) AS "Prix de vente",
                        p.emplacement AS "Emplacement", 
                        p.description AS "Description"
                    FROM produits p
                    INNER JOIN categories c ON p.id_categorie = c.id_categorie
                    LEFT JOIN (
                        SELECT 
                            id_produit, 
                            SUM(quantite_achetee) as total_qte_achats,
                            SUM(prix_achat_unitaire * quantite_achetee) / NULLIF(SUM(quantite_achetee), 0) as prix_moyen_achat
                        FROM achats 
                        GROUP BY id_produit
                    ) sub_achats ON p.id_produit = sub_achats.id_produit
                    LEFT JOIN (
                       SELECT 
                            id_produit, 
                            SUM(quantite_vendue) as total_qte_ventes,
                            SUM(prix_vente_reel * quantite_vendue) / NULLIF(SUM(quantite_vendue), 0) as prix_moyen_vente
                        FROM ventes 
                        GROUP BY id_produit
                    ) sub_ventes ON p.id_produit = sub_ventes.id_produit; 
                    """;

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(SQL, connection)) //On créer un adapter pour lier les informations de la requête SQL avec une DataTable
                {
                    //On créer un type DataTable
                    DataTable dt = new DataTable();
                    adapter.Fill(dt); //On remplie la DataTable avec notre requête SQL
                    return dt;
                }
            }

        }

        //On crée une méthode qui permettre de ajouter un achat dans la BDD
        public void AjoutStock(string catégorie,string nom,int quantite,decimal prix_achat,string emplacement,string description)
        {
            //Variable interne pour ne pas être connecté a la BDD en permanence et Fermeture de liaison une fois la méthode finie
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                int id_categorie;

                //On définit la requête SQL qu'on va réaliser
                string SQL = $"""
                    WITH categorie_id AS (
                        INSERT INTO categorie (nom_categorie)
                        VALUES ({catégorie})
                        ON CONFLICT (nom_categorie) DO UPDATE SET nom_categorie = EXCLUDED.nom_categorie
                        RETURNING id_categorie
                    ),
                    produit_id AS (
                        INSERT INTO produits (nom_produit,id_categorie,emplacement,description)
                        SELECT {nom}, id_categorie, {emplacement},{description} FROM categorie_id
                    )

                    INSERT INTO produits (nom_produit

                """;
                new NpgsqlCommand(SQL, connection);

            }

        }
    }
}
