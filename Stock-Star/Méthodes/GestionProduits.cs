using Npgsql; //Commande SQL
using System.Data; //Type DataTable stocké des tables

namespace Stock_Star
{
    internal class GestionProduits
    {
        //On crée un champ qui va contenir le moyen de se connecter à la BDD
        private ConnectionBDD BDD = new ConnectionBDD();

        //On crée une méthode qui permettre de récupérer les données de la BDD (de type DataTable)
        /*
        R: Créer une DataTable qui va contenir tous nos produits présents dans la base de données sans redondance et en calculant la quantité,le prix d'achat et prix de vente réel
        E: Rien
        S: Une DataTable contenant tous les produits (sans redondance) avec les champs : categorie,nom,quantité,prix achat,prix de vente,emplacement,description
        */
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
                        //Coalesce pour remplacer par 0 en cas de Null
                        ROUND(COALESCE(sub_achats.prix_moyen_achat, 0), 2) AS "Prix achat",
                        ROUND(COALESCE(sub_ventes.prix_moyen_vente, 0), 2) AS "Prix de vente",
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

        //On crée une méthode qui permettre d'ajouter un achat dans la BDD
        /*
        R: Ajouter un produit / Simuler un achat dans la base de données 
        E: 4 string correspondant à la catégorie,le nom (du produit), l'emplacement et la description
           1 entier, la quantité achetée
           1 décimal, le prix d'achat
        S: Rien (ajout dans la base de données)
        */
        public void AjoutStock(string categorie,string nom,string emplacement,string description, int quantite, decimal prix_achat)
        {
            //Variable interne pour ne pas être connecté a la BDD en permanence et Fermeture de liaison une fois la méthode finie
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                //On définit la requête SQL qu'on va réaliser
                string SQL = """
                    WITH
                    categorie_id AS (
                        INSERT INTO categories (nom_categorie)
                        VALUES (@categorie)
                        ON CONFLICT (nom_categorie) DO UPDATE SET nom_categorie = EXCLUDED.nom_categorie
                        RETURNING id_categorie
                    ),
                    produit_id AS (
                        INSERT INTO produits (nom_produit,id_categorie,emplacement,description)
                        SELECT @nom, id_categorie, @emplacement,@description FROM categorie_id
                    ON CONFLICT (nom_produit) DO UPDATE SET nom_produit = EXCLUDED.nom_produit
                        RETURNING id_produit
                    )

                    INSERT INTO achats (id_produit,quantite_achetee,prix_achat_unitaire,date_achat)
                    SELECT id_produit, @quantite,@prix_achat, NOW() FROM produit_id;
                    """;
                using (NpgsqlCommand command = new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("categorie", categorie);
                    command.Parameters.AddWithValue("nom", nom);
                    command.Parameters.AddWithValue("quantite", quantite);
                    command.Parameters.AddWithValue("prix_achat", prix_achat);
                    command.Parameters.AddWithValue("emplacement", emplacement);
                    command.Parameters.AddWithValue("description", description);

                    command.ExecuteNonQuery();
                }

            }

        }

        //On crée une méthode qui va permettre de supprimer un produit déja existant dans la BDD
        /*
        R : Supprimer complétement un produit passé en paramètre (nom)
        E : 1 string, le nom du produit a supprimer
        S : Rien
        */
        public void SupprimerProduit(string nom)
        {
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();
                string SQL = """
                    -- Nous avons préalablement configurer le ON DELETE CASCADE !
                    DELETE FROM produits WHERE nom_produit=@nom; 
                    --On supprime la catégorie si il n'y a plus aucun produit associé
                    DELETE FROM categories 
                    WHERE id_categorie NOT IN (SELECT DISTINCT id_categorie FROM produits WHERE id_categorie IS NOT NULL);
                    """;
                using (NpgsqlCommand command= new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("nom", nom);
                    command.ExecuteNonQuery() ;
                }
            }
        }


        //On crée une méthode qui va permettre de modifier un produit déja existant dans la BDD
        /*
        R: Modifier un produit déjà existant ou bien la catégorie (associé à ce produit)
        E: 5 string, l'ancien nom du produit a modifié , la catégorie a modifié associé à ce produit, le nouveau nom du produit, le nouvelle emplacement du produit et la nouvelle description du produit
        S: Rien
        */
        public void ModifierProduit(string ancien_nom,string categorie, string nouveau_nom, string emplacement, string description)
        {
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();
                string SQL = """
                        --Modifier la catégorie
                        UPDATE categories
                        SET nom_categorie= CASE
                                                WHEN @categorie = ''
                                                THEN nom_categorie
                                                ELSE @categorie
                                            END
                        WHERE id_categorie=(SELECT id_categorie FROM produits
                        WHERE nom=@ancien_nom)
                        
                        --Modifier le produit
                        UPDATE produits
                        SET nom= CASE
                                    WHEN @nouveau_nom = ''
                                    THEN nom_produit
                                    ELSE @nouveau_nom
                                  END,
                            emplacement=CASE
                                            WHEN @emplacement = ''
                                            THEN emplacement
                                            ELSE @emplacement
                                        END,
                            description=CASE
                                            WHEN @description = ''
                                            THEN description
                                            ELSE @description
                                        END
                        WHERE nom=@ancien_nom
                        
                        """;
                using (NpgsqlCommand command = new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("ancien_nom", ancien_nom);
                    command.Parameters.AddWithValue("categorie", categorie);
                    command.Parameters.AddWithValue("nouveau_nom", nouveau_nom);
                    command.Parameters.AddWithValue("emplacement", emplacement);
                    command.Parameters.AddWithValue("description", description);
                    command.ExecuteNonQuery();
                }
            }



        }



        //On crée une méthode qui permettre d'ajouter un achat dans la BDD
        /*
        R: 
        E: 
        S: 
        */
        /*public void AjoutVente(string nom, string prixVente, string Quantité, int quantite, decimal prix_achat)
        {
            //Variable interne pour ne pas être connecté a la BDD en permanence et Fermeture de liaison une fois la méthode finie
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                //On définit la requête SQL qu'on va réaliser
                string SQL = """"
                    SELECT (id_produit) FROM produits WHERE nom_produit = @nom,
                    RETURNING id_produit

                    INSERT INTO ventes (id_produit,quantite_vendue,prix_vente_reel,date_vente)
                    SELECT id_produit, @quantite,@prixVente, NOW() FROM produit_id;
                                       
                    """";
                using (NpgsqlCommand command = new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("id_produit", id_produit);
                    command.Parameters.AddWithValue("quantite_vendue", quantite);
                    command.Parameters.AddWithValue("prix_achat", prix_achat);
                    command.Parameters.AddWithValue("prix_vente_reel", prixVente);

                    command.ExecuteNonQuery();
                }

            }

        }*/



        /*
        R : Ajouter une nouvelle vente dans la table vente
        E : String le nom du produit, int la quantité de produit vendue, decimal prix de Vente
        S : vide
         */
        public void AjoutVente(string nom, int quantiteVendue, decimal prixVente, DateTime dateVente)
        {
            using (var connection = BDD.GetConnection())
            {
                connection.Open();

                string SQL = """
                 WITH produit_id AS ( --on creer une table intermediaire produit_id dans laquel il y'as une seul ligne id_produit FROM produit where nom=@nom
                     SELECT id_produit
                     FROM produits
                     WHERE nom_produit = @nom
                     LIMIT 1
                 )
                 INSERT INTO ventes (id_produit, quantite_vendue, prix_vente_reel, date_vente)
                 SELECT id_produit, @quantiteVendue, @prixVente, @dateVente
                 FROM produit_id;
                 """;

                using (var command = new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("nom", nom);
                    command.Parameters.AddWithValue("quantiteVendue", quantiteVendue);
                    command.Parameters.AddWithValue("prixVente", prixVente);
                    command.Parameters.AddWithValue("dateVente", dateVente);


                    command.ExecuteNonQuery();
                }
            }
        }



        //On crée une méthode qui permettre de récupérer les données de la BDD (de type DataTable)
        /*
        R: Créer une DataTable qui va contenir tous nos produits présents dans la base de données sans redondance et en calculant la quantité,le prix d'achat et prix de vente réel
        E: Rien
        S: Une DataTable contenant tous les produits (sans redondance) avec les champs : categorie,nom,quantité,prix achat,prix de vente,emplacement,description
        */
        public DataTable ChargerLesVentes()
        {
            //Variable interne pour ne pas être connecté a la BDD en permanence et Fermeture de liaison une fois la méthode finie
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                //On définit la requête SQL qu'on va réaliser
                string SQL = """      
                    SELECT
                        id_vente AS "ID Vente",
                        id_produit AS "ID Produit",
                        quantite_vendue AS "Quantité Vendue",
                        prix_vente_reel AS "Prix de Vente Réel",
                        date_vente AS "Date de Vente"
                        FROM ventes
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

    }
}
