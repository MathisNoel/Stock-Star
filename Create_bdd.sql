-- Database: Stock-Star

-- DROP DATABASE IF EXISTS "Stock-Star";

CREATE DATABASE "Stock-Star"
    WITH
    OWNER = postgres
    ENCODING = 'UTF8'
    LC_COLLATE = 'French_France.1252'
    LC_CTYPE = 'French_France.1252'
    LOCALE_PROVIDER = 'libc'
    TABLESPACE = pg_default
    CONNECTION LIMIT = -1
    IS_TEMPLATE = False;	

	--Création table categories
	CREATE TABLE categories (
		id_categorie SERIAL PRIMARY KEY NOT NULL,
		nom_categorie TEXT NOT NULL
	)
	
	--Création table produits
	CREATE TABLE produits (
		id_produit SERIAL PRIMARY KEY NOT NULL,
		nom_produit TEXT NOT NULL,
		id_categorie INT REFERENCES categories(id_categorie), --Lié avec la table categorie
		description TEXT
	)

	--Création table achats
	CREATE TABLE achats (
		id_achat SERIAL PRIMARY KEY NOT NULL,
		id_produit INT REFERENCES produits(id_produit),
		quantite_achetee INT NOT NULL,
		prix_achat_unitaire DECIMAL(7,2) NOT NULL, --DECIMAL(7,2) implique deux chiffre après la virgule et 7 chiffres au total soit chiffre max 99999,99€
		date_achat DATE DEFAULT CURRENT_DATE
	)
	
	--Création table ventes
	CREATE TABLE ventes (
		id_vente SERIAL PRIMARY KEY NOT NULL,
		id_produit INT REFERENCES produits(id_produit),
		quantite_vendue INT NOT NULL,
		prix_vente_reel DECIMAL(7,2) NOT NULL,
		date_vente DATE DEFAULT CURRENT_DATE
	)
	-- Insérer du contenue dans les tables
	INSERT INTO categories (nom_categorie) -- Categorie
	VALUES ('Chaussure')
	
	INSERT INTO produits (nom_produit,id_categorie,description)
	VALUES ('Nike AirForce 1',1,'Chassures de la marque Nike')

	--On achète deux fois a des prix différents
	INSERT INTO achats (id_produit,quantite_achetee,prix_achat_unitaire)
	VALUES (1,50,20.5)
	
	INSERT INTO achats (id_produit,quantite_achetee,prix_achat_unitaire)
	VALUES (1,20,18)
	--On vend deux fois a des prix différents
	INSERT INTO ventes (id_produit,quantite_vendue,prix_vente_reel)
	VALUES (1,20,20.5)

	INSERT INTO ventes (id_produit,quantite_vendue,prix_vente_reel)
	VALUES (1,30,19.5)
	
	-- Afficher les tables
	SELECT * FROM categories
	SELECT * FROM produits
	SELECT * FROM achats
	SELECT * FROM ventes

	-- Test d'affichage des stocks (Méthode ChargerStock) --
	SELECT 
		c.nom_categorie AS categorie,
		p.nom_produit AS nom,
		-- Calcul du stock (Total achats - Total ventes)
		(COALESCE(sub_achats.total_qte_achats, 0) - COALESCE(sub_ventes.total_qte_ventes, 0)) AS quantite,
		
		-- Calcul du prix d'achat moyen (PAMP) avec conversion numeric pour le ROUND
		ROUND(COALESCE(sub_achats.prix_moyen_achat, 0)::numeric, 2) AS "Prix achat",
		
		-- Calcul du prix de vente moyen
		ROUND(COALESCE(sub_ventes.prix_moyen_vente, 0)::numeric, 2) AS "Prix de vente",
		
		p.emplacement AS emplacement, 
		p.description AS description
		FROM produits p
		INNER JOIN categories c ON p.id_categorie = c.id_categorie
		-- Sous-requête pour isoler le calcul des ACHATS
		LEFT JOIN (
			SELECT 
				id_produit, 
				SUM(quantite_achetee) as total_qte_achats,
				-- Somme(prix*qte) / Somme(qte)
				SUM(prix_achat_unitaire * quantite_achetee) / NULLIF(SUM(quantite_achetee), 0) as prix_moyen_achat
			FROM achats 
			GROUP BY id_produit
		) sub_achats ON p.id_produit = sub_achats.id_produit
		-- Sous-requête pour isoler le calcul des VENTES
		LEFT JOIN (
			SELECT 
				id_produit, 
				SUM(quantite_vendue) as total_qte_ventes,
				-- Somme(prix_vente*qte) / Somme(qte)
				SUM(prix_vente_reel * quantite_vendue) / NULLIF(SUM(quantite_vendue), 0) as prix_moyen_vente
			FROM ventes 
			GROUP BY id_produit
		) sub_ventes ON p.id_produit = sub_ventes.id_produit;
		
	---- COMMANDE DE BASE ----
	--Ajout d'une colonne a la table Produits
	ALTER TABLE produits
	ADD COLUMN emplacement TEXT

	--Replacer la colonne date tout a la fin
	ALTER TABLE produits ADD COLUMN date_new date;
	UPDATE produits SET date_new=date;
	ALTER TABLE produits DROP COLUMN date;
	ALTER TABLE produits RENAME COLUMN date_new TO date;
	
	--Modification colonne de la table Produits
	ALTER TABLE produits
	ALTER COLUMN type TYPE text

	--Renommer la colonne achat en prix
	ALTER TABLE produits RENAME achat TO prix

	--Affiche contenue table Produits
	SELECT * FROM produits

	--Insérer données dans la table Produits
	INSERT INTO produits (type,nom,quantite,date)
	VALUES('Objet','Chaussures',5,'2026-02-12');

	--Rajouter le prix d'un certain produit
	UPDATE produits
	SET prix = 50.99
	WHERE id_produit= 1;
	
	--Supprimer un produit
	DELETE FROM produits
	WHERE id_produit = 2;