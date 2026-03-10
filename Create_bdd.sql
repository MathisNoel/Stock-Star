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

	INSERT INTO achats (id_produit,quantite_achetee,prix_achat_unitaire)
	VALUES (1,50,20.5)
	
	
	-- Afficher les tables
	SELECT * FROM categories
	SELECT * FROM produits
	SELECT * FROM achats
	SELECT * FROM ventes


	---- COMMANDE DE BASE ----
	--Ajout d'une colonne a la table Produits
	ALTER TABLE produits
	ADD COLUMN achat real

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