-- remplir clients
INSERT INTO client (adresse_mail, nom, prenom, telephone, mot_de_passe, adresse_facturation, carte_bancaire)
VALUES 
('johndoe@gmail.com', 'Doe', 'John', '01 23 45 67 89', 'motdepasse', '123 rue des fleurs, 75001 Paris', '1234567890123456'),
('jane.doe@gmail.com', 'Doe', 'Jane', '01 23 45 67 90', 'password', '456 rue des plantes, 75002 Paris', '2345678901234567'),
('bob.smith@hotmail.com', 'Smith', 'Bob', '01 23 45 67 91', '123456', '789 rue des roses, 75003 Paris', '3456789012345678'),
('mary.johnson@yahoo.com', 'Johnson', 'Mary', '01 23 45 67 92', 'abcdef', '321 avenue des orchidées, 75004 Paris', '4567890123456789'),
('samuel.wilson@outlook.com', 'Wilson', 'Samuel', '01 23 45 67 93', 'ghijkl', '987 boulevard des tulipes, 75005 Paris', '5678901234567890');

-- remplir les statuts
INSERT INTO statut (adresse_mail, niveau, nb_achats)
VALUES 
('johndoe@gmail.com', 'BRONZE', 3),
('jane.doe@gmail.com', 'AUCUN', 0),
('bob.smith@hotmail.com', 'OR', 5),
('mary.johnson@yahoo.com', 'AUCUN', 0),
('samuel.wilson@outlook.com', 'BRONZE', 2);

-- remplir les commandes
INSERT INTO commande (num_commande, date_com, adresse_mail,prix_commande)
VALUES 
('CMD0001', '2023-04-22', 'johndoe@gmail.com', 10.00),
('CMD0002', '2023-04-23', 'jane.doe@gmail.com',18.50),
('CMD0003', '2023-04-24', 'bob.smith@hotmail.com',55.00),
('CMD0004', '2023-04-25', 'mary.johnson@yahoo.com',96.00),
('CMD0005', '2023-04-26', 'samuel.wilson@outlook.com',70.00);

-- remplir les bouquets
INSERT INTO bouquet (nom_bouquet, occasion, type, fleurs_bouquet, accessoires_bouquet,prix_bouquet,disponible)
VALUES
  ('Bouquet de roses', 'Mariage', 'standard', '4 Roses rouges', '1 Ruban blanc', 10.00,true),
  ('Bouquet printanier', 'Anniversaire', 'standard', '3 Pivoines,3 tulipes, 5 jonquilles', '1 Papier kraft',18.50,true),
  ('Bouquet de lys', 'Funérailles', 'standard', '20 Lys blancs', '1 Ruban noir',55.00,true),
  ('Bouquet tropical', 'Réception', 'standard', '4 Gingembre,10 oiseau de paradis,20 feuilles d_ananas', '1 Raphia',96.00,true),
  ('Bouquet de marguerites', 'Fête des mères', 'standard', '20 Marguerites roses,20 Marguerites blanches', '1 Feuilles de fougère',70.00,true);

-- remplir fleurs
INSERT INTO fleur (nom_fleur, nombre_fleur_stock, disponibilite_fleur, prix_fleur)
VALUES
  ('Roses rouges', 100, 'Disponible', 2.50),
  ('Pivoines', 60, 'Disponible', 3.00),
  ('Tulipes', 80, 'Disponible', 1.50),
  ('Jonquilles', 40, 'Disponible', 1.00),
  ('Lys blancs', 70, 'Disponible', 2.75),
  ('Gingembre', 30, 'Disponible', 4.00),
  ('Oiseau de paradis', 20, 'Disponible', 5.50),
  ('Feuilles d_ananas', 50, 'Disponible', 1.25),
  ('Marguerites roses', 90, 'Disponible', 1.75),
  ('Marguerites blanches', 80, 'Disponible', 1.75),
  ('Gerbera', 80, 'Disponible', 1.55),
  ('Ginger', 80, 'Disponible', 1.50),
  ('Glaïeul', 80, 'Disponible', 1.55);

-- remplir accessoires
INSERT INTO accessoire (nom_accessoire, nombre_accessoire_stock, disponibilite_accessoire, prix_accessoire)
VALUES
  ('Ruban', 100,'Disponible', 0.50),
  ('Vase en verre', 50, 'Disponible', 5.00),
  ('Papier d_emballage', 150, 'Disponible', 0.25),
  ('Paille', 30, 'Disponible', 1.00),
  ('Papillon décoratif', 90, 'Disponible', 1.75),
  ('Bougie parfumée', 60, 'Disponible', 3.50);

-- remplir bons de commande
INSERT INTO bon_de_commande (num_bon_com, etat_commande, date_livraison, adresse_livraison, num_commande, nom_bouquet) 
VALUES ('BC001', 'CL', '2023-05-05', '2 rue des Fleurs, Paris', 'CMD0001', 'Bouquet de roses');

INSERT INTO bon_de_commande (num_bon_com, etat_commande, date_livraison, adresse_livraison, num_commande, nom_bouquet) 
VALUES ('BC002', 'CL', '2023-05-12', '8 avenue de la Gare, Lyon', 'CMD0002', 'Bouquet printanier');

INSERT INTO bon_de_commande (num_bon_com, etat_commande, date_livraison, adresse_livraison, num_commande, nom_bouquet) 
VALUES ('BC003', 'CL', '2023-05-22', '12 rue des Roses, Lille', 'CMD0003', 'Bouquet de lys');

INSERT INTO bon_de_commande (num_bon_com, etat_commande, date_livraison, adresse_livraison, num_commande, nom_bouquet) 
VALUES ('BC004', 'CL', '2023-05-30', '25 avenue des Lilas, Marseille', 'CMD0004', 'Bouquet tropical');

INSERT INTO client(adresse_mail,nom,mot_de_passe)
VALUES('admin.fleuriste@gmail.com','Admin','adminlog');
INSERT INTO statut (adresse_mail, niveau, nb_achats)
VALUES ('admin.fleuriste@gmail.com', 'ADMIN',0);

INSERT INTO bon_de_commande (num_bon_com, etat_commande, date_livraison, adresse_livraison, num_commande, nom_bouquet) 
VALUES ('BC005', 'CL', '2023-06-05', '5 rue des Violettes, Toulouse', 'CMD0005', 'Bouquet de marguerites');


SELECT * FROM commande;
DELETE FROM commande WHERE num_commande='CMD0010';
DELETE FROM bon_de_commande WHERE num_bon_com='BC006';
DELETE FROM bouquet where type='personalisé';
USE fleuriste;
SELECT * FROM statut;
SELECT * FROM client;
select * from fleur;

SELECT * FROM bon_de_commande;
SELECT * FROM commande;
SELECT * FROM bouquet;

SELECT c.adresse_mail, COUNT(*) as nb_achats, SUM(co.prix_commande) as total_achats 
FROM commande co 
JOIN client c ON c.adresse_mail = co.adresse_mail 
WHERE c.adresse_mail='johndoe@gmail.com'
GROUP BY c.adresse_mail;

select avg(bo.prix_bouquet)
from bon_de_commande as b, bouquet as bo
where b.nom_bouquet = bo.nom_bouquet;


SELECT * FROM commande;
SELECT nom_accessoire FROM accessoire WHERE disponibilite_accessoire =  'Disponible';
SELECT * FROM accessoire;
SELECT num_commande FROM commande ORDER BY num_commande DESC;
SELECT nom_bouquet,occasion FROM bouquet WHERE disponible>1;