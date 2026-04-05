using Npgsql; //Commande SQL
using System.Data; //Type DataTable stocké des tables

namespace Stock_Star
{
    internal class GestionProduits
    {
        //On crée un champ qui va contenir le moyen de se connecter à la BDD
        private ConnectionBDD BDD = new ConnectionBDD();

        // ############################
        // --        COMBOBOX        --
        // ############################

        // On créer une méthode qui va pouvoir compléter la ComboBox de catégorie
        /*
        R: Récupérer les catégories pour les affichers en forme de liste dans la combobox passé en paramètre
        E: Une comboxbox
        S: Rien
        */
        public void RemplirCategorie(ComboBox CategorieBox)
        {
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();
                string SQL = "SELECT nom_categorie FROM categories ORDER BY nom_categorie ASC;";

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(SQL, connection))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Comme pour un DataGridView, on lie la source
                    CategorieBox.DataSource = dt;

                    // INDISPENSABLE : On dit quelle colonne afficher dans la liste
                    CategorieBox.DisplayMember = "nom_categorie";

                    // On peut aussi définir la valeur interne (souvent l'ID, mais ici le nom suffit)
                    CategorieBox.ValueMember = "nom_categorie";
                }
            }

        }

        // On créer une méthode qui va pouvoir compléter la ComboBox de Nom_Produit
        public void RemplirNomProduit(ComboBox ProduitBox)
        {
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();
                string SQL = "SELECT nom_produit FROM produits ORDER BY nom_produit ASC;";

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(SQL, connection))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Comme pour un DataGridView, on lie la source
                    ProduitBox.DataSource = dt;

                    // INDISPENSABLE : On dit quelle colonne afficher dans la liste
                    ProduitBox.DisplayMember = "nom_produit";

                    // On peut aussi définir la valeur interne (souvent l'ID, mais ici le nom suffit)
                    ProduitBox.ValueMember = "nom_produit";
                }
            }

        }

        // ############################
        // --       PAGESTOCK        --
        // ############################

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
                        --Coalesce pour remplacer par 0 en cas de Null
                        ROUND(COALESCE(sub_achats.prix_moyen_achat, 0), 2) AS "Prix achat (/u)",
                        ROUND(COALESCE(sub_ventes.prix_moyen_vente, 0), 2) AS "Prix de vente (/u)",
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
                    ) sub_ventes ON p.id_produit = sub_ventes.id_produit
                    ORDER BY c.nom_categorie ASC;
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
        S: Rien (ajout dans la base de données)
        */
        public void AjoutStock(string categorie, string nom, string emplacement, string description)
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
                        ON CONFLICT (nom_categorie) DO UPDATE 
                            SET nom_categorie = EXCLUDED.nom_categorie 
                        RETURNING id_categorie
                    )
                    INSERT INTO produits (nom_produit, id_categorie, emplacement, description)
                    SELECT @nom, id_categorie, @emplacement, @description 
                    FROM categorie_id
                    ON CONFLICT (nom_produit) DO NOTHING;
                    """;
                using (NpgsqlCommand command = new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("categorie", categorie);
                    command.Parameters.AddWithValue("nom", nom);
                    command.Parameters.AddWithValue("emplacement", emplacement);
                    command.Parameters.AddWithValue("description", description);

                    command.ExecuteNonQuery();
                }

            }

        }

        // ##########################################
        // -- SUPPRIMER DES PRODUITS/ACHATS/VENTES --
        // ##########################################

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
                using (NpgsqlCommand command = new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("nom", nom);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void SupprimerVente(int idVente)
        {
            using (var conn = BDD.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM ventes WHERE id_vente = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", idVente);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SupprimerAchat(int idAchat)
        {
            using (var conn = BDD.GetConnection())
            {
                conn.Open();
                string sql = "DELETE FROM achats WHERE id_achat = @id";
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("id", idAchat);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        //On crée une méthode qui va permettre de modifier un produit déja existant dans la BDD
        /*
        R: Modifier un produit déjà existant ou bien la catégorie (associé à ce produit)
        E: 5 string, l'ancien nom du produit a modifié , la catégorie a modifié associé à ce produit, le nouveau nom du produit, le nouvelle emplacement du produit et la nouvelle description du produit
        S: Rien
        */
        public void ModifierProduit(string ancien_nom, string categorie, string nouveau_nom, string emplacement, string description)
        {
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                string SQL = """
            WITH cat_info AS (
                -- On insère la catégorie si elle n'existe pas et on récupère l'ID (existant ou nouveau)
                INSERT INTO categories (nom_categorie) 
                VALUES (@categorie)
                ON CONFLICT (nom_categorie) DO UPDATE SET nom_categorie = EXCLUDED.nom_categorie
                RETURNING id_categorie
            )
            UPDATE produits
            SET 
                id_categorie = (SELECT id_categorie FROM cat_info),
                nom_produit = CASE WHEN @nouveau_nom = '' THEN nom_produit ELSE @nouveau_nom END,
                emplacement = CASE WHEN @emplacement = '' THEN emplacement ELSE @emplacement END,
                description = CASE WHEN @description = '' THEN description ELSE @description END
            WHERE nom_produit = @ancien_nom;
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

        // ############################
        // --    PAGETRANSACTION     --
        // ############################

        /*
        R : Ajouter une nouvelle vente dans la table vente
        E : String le nom du produit, int la quantité de produit vendue, decimal prix de Vente, DateTime la date de vente
        S : vide
         */
        public bool AjoutVente(string nom, int quantite, decimal prix, DateTime date)
        {
            using (var connection = BDD.GetConnection())
            {
                connection.Open();

                string SQL = """
                 INSERT INTO ventes (id_produit, quantite_vendue, prix_vente_reel, date_vente, benefice)
                 SELECT 
                     p.id_produit, 
                     @quantite, 
                     @prix, 
                     @date,
                     (@prix - COALESCE((SELECT AVG(prix_achat_unitaire) FROM achats WHERE id_produit = p.id_produit), 0)) * @quantite
                 FROM produits p
                 WHERE p.nom_produit = @nom
                 AND (
                     -- Total Acheté
                     (SELECT COALESCE(SUM(quantite_achetee), 0) FROM achats WHERE id_produit = p.id_produit) 
                     - 
                     -- Total Vendu
                     (SELECT COALESCE(SUM(quantite_vendue), 0) FROM ventes WHERE id_produit = p.id_produit)
                 ) >= @quantite;
                 """;

                using (var command = new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("nom", nom);
                    command.Parameters.AddWithValue("quantite", quantite);
                    command.Parameters.AddWithValue("prix", prix);
                    command.Parameters.AddWithValue("date", date);

                    //Si aucune ligne na été modifié
                    int lignesAffectees = command.ExecuteNonQuery();
                    return lignesAffectees > 0; // Renvoie true si la vente a été enregistrée
                }
            }
        }

        /*
         R : Ajouter un nouvel achat dans la table achats (réapprovisionnement du stock)
         E : string le nom du produit, int la quantité achetée, decimal le prix d'achat unitaire, DateTime la date d'achat
         S : vide
        */
        public void AjoutAchat(string nom, int quantite, decimal prix, DateTime date)
        {
            using (var connection = BDD.GetConnection())
            {
                connection.Open();

                // Requête simple : on cherche l'ID du produit par son nom pour remplir la table achats
                string SQL = """
                INSERT INTO achats (id_produit, quantite_achetee, prix_achat_unitaire, date_achat)
                SELECT id_produit, @quantite, @prix, @date
                FROM produits
                WHERE nom_produit = @nom;
                """;

                using (var command = new NpgsqlCommand(SQL, connection))
                {
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@quantite", quantite);
                    command.Parameters.AddWithValue("@prix", prix);
                    command.Parameters.AddWithValue("@date", date);

                    command.ExecuteNonQuery();
                }
            }
        }

        //On crée une méthode qui permettre de récupérer les données de la BDD (de type DataTable)
        /*
        R: Créer une DataTable qui va contenir toutes nos ventes présent dans la table ventes et calcul le bénéfice associé a chacune
        E: Rien
        S: Une DataTable contenant toutes les ventes avec les champs suivant : id_vente,nom_produit,quantité,prix vente (/u),benefice,date_vente
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
                        p.nom_produit AS "Nom",
                        quantite_vendue AS "Quantité",
                        prix_vente_reel AS "Prix unitaire",
                        benefice AS "Bénéfice", -- On appelle juste la colonne
                        date_vente AS "Date de Vente"
                    FROM ventes v
                    LEFT JOIN produits p ON v.id_produit = p.id_produit
                    ORDER BY date_vente DESC;
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

        //On crée une méthode qui permettre de récupérer les données de la BDD (de type DataTable)
        /*
        R: Créer une DataTable qui va contenir tous nos achats présent dans la table achats et calcul le bénéfice associé a chacun
        E: Rien
        S: Une DataTable contenant tous les achats avec les champs suivant : id_achat,nom_produit,quantité,prix achat (/u),benefice,date_achat
        */
        public DataTable ChargerLesAchats()
        {
            using (NpgsqlConnection connection = BDD.GetConnection())
            {
                connection.Open();

                // On fait une JOINTURE (JOIN) pour afficher le nom du produit au lieu de l'ID
                string SQL = """      
                SELECT 
                    a.id_achat AS "ID Achat",
                    p.nom_produit AS "Produit",
                    a.quantite_achetee AS "Quantité",
                    a.prix_achat_unitaire AS "Prix Unitaire",
                    a.date_achat AS "Date d'Achat"
                FROM achats a
                JOIN produits p ON a.id_produit = p.id_produit
                WHERE a.quantite_achetee > 0 --On affiche pas les produits achetée 0 fois
                ORDER BY a.date_achat DESC;
                """;

                using (NpgsqlDataAdapter adapter = new NpgsqlDataAdapter(SQL, connection))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }
}
