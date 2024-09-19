DROP DATABASE IF EXISTS fleuriste;
CREATE DATABASE IF NOT EXISTS fleuriste;
USE fleuriste;


-- table client
CREATE TABLE client (adresse_mail varchar(50) NOT NULL,
 nom varchar(50), 
 prenom varchar(50), 
 telephone varchar(50), 
 mot_de_passe varchar(50), 
 adresse_facturation varchar(50), 
 carte_bancaire varchar(50));  
ALTER TABLE client ADD CONSTRAINT PK_client PRIMARY KEY (adresse_mail);
 
 -- table statut
  CREATE TABLE statut (adresse_mail varchar(50) NOT NULL,
  niveau ENUM('ADMIN','OR','BRONZE','AUCUN'), 
  nb_achats int);  
  ALTER TABLE statut ADD CONSTRAINT PK_statut PRIMARY KEY (adresse_mail);
  ALTER TABLE statut ADD CONSTRAINT FK_statut_adresse_mail FOREIGN KEY (adresse_mail) REFERENCES client(adresse_mail);


-- table commande
CREATE TABLE commande (num_commande varchar(50) NOT NULL,
date_com date, 
adresse_mail varchar(50),
prix_commande decimal(5,2));  
ALTER TABLE commande ADD CONSTRAINT PK_commande PRIMARY KEY (num_commande);  
ALTER TABLE commande ADD CONSTRAINT FK_commande_adresse_mail FOREIGN KEY (adresse_mail) REFERENCES client (adresse_mail);  


-- table bouquet
 CREATE TABLE bouquet (nom_bouquet varchar(50) NOT NULL, 
 occasion varchar(50), 
 type ENUM('standard','personalisé'), 
 fleurs_bouquet varchar(500), 
 accessoires_bouquet varchar(500),
 prix_bouquet decimal(5,2),
 disponible boolean);  
 ALTER TABLE bouquet ADD CONSTRAINT PK_bouquet PRIMARY KEY (nom_bouquet);
  

 
 CREATE TABLE accessoire (nom_accessoire varchar(50) NOT NULL, 
 nombre_accessoire_stock int, 
 disponibilite_accessoire varchar(50), 
 prix_accessoire decimal(5,2));  
 ALTER TABLE accessoire ADD CONSTRAINT PK_accessoire PRIMARY KEY (nom_accessoire);  
 
-- table bon de commande
 CREATE TABLE bon_de_commande (num_bon_com varchar(50) NOT NULL, 
 etat_commande ENUM ('VINV','CC','CPAV','CAL','CL'), 
 date_livraison date, 
 adresse_livraison varchar(50), 
 num_commande varchar(50),
 nom_bouquet varchar(50));  
 ALTER TABLE bon_de_commande ADD CONSTRAINT PK_bon_de_commande PRIMARY KEY (num_bon_com);  
 ALTER TABLE bon_de_commande ADD CONSTRAINT FK_bon_de_commande_num_commande FOREIGN KEY (num_commande) REFERENCES commande (num_commande);  
 ALTER TABLE bon_de_commande ADD CONSTRAINT FK_bon_de_commande_nom_bouquet FOREIGN KEY (nom_bouquet) REFERENCES bouquet (nom_bouquet);

 
 CREATE TABLE fleur (nom_fleur varchar(50) NOT NULL,
 nombre_fleur_stock int, 
 disponibilite_fleur varchar(50), 
 prix_fleur decimal(5,2));  
 ALTER TABLE fleur ADD CONSTRAINT PK_fleur PRIMARY KEY (nom_fleur);  

 
