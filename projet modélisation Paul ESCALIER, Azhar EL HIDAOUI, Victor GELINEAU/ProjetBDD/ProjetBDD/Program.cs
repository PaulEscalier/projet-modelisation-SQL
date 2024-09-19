using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Sources;
using System.Transactions;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Asn1.X509;

namespace ProjetBDD
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            #region Connection Base de données
            // Connection à la base de données
            MySqlConnection maConnexion = null;
            try
            {
                string connexionString = "SERVER=localhost;PORT=3306;" +
                    "DATABASE=fleuriste;" +
                    "UID=root;PASSWORD=root";
                maConnexion = new MySqlConnection(connexionString);
                maConnexion.Open();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur connexion: " + e.ToString());
                return;
            }
            #endregion
            #region utilisateur
            Console.WriteLine(File.ReadAllText("Titre.txt"));
            // Demarage du site 
            Console.WriteLine("Bienvenue sur le site de Mr. Bellefleur Michel");
            Console.Write("Avant de vous connecter ou de créer un compte, veuillez entrer votre adresse email :\n\n-->");

            string email = Console.ReadLine();

            // Vérifier si l'utilisateur existe déjà dans la base de données
            MySqlDataReader reader = CommandeSQL("SELECT COUNT(*) FROM client WHERE adresse_mail = '"+email+"' ",maConnexion);
            int count = 0;
            if(reader.Read())
            {
                count = Convert.ToInt32(reader["COUNT(*)"].ToString());
            }
            reader.Close();


            if (count > 0) // L'utilisateur existe déjà
            {
                count = -1;
                
                if (count > 0)
                {
                    Console.WriteLine("Bienvenue sur notre site web !");
                    // Code pour accéder au compte de l'utilisateur
                }
                else
                {
                    while (count <= 0)
                    {
                        if (count == 0)
                        {

                            Console.WriteLine("Mot de passe incorrect.");
                        }
                        else
                        {
                            Console.WriteLine("Veuillez entrer votre mot de passe lié à l'adresse mail saisie:");

                        }
                        string mdp = Console.ReadLine();

                        // Vérifier si le mot de passe est correct
                        
                        
                        reader= CommandeSQL("SELECT COUNT(*) FROM client WHERE adresse_mail = '"+email+"' AND mot_de_passe =  '"+mdp+"' ",maConnexion);
                        if(reader.Read())
                        {
                            count=Convert.ToInt32(reader["COUNT(*)"].ToString());
                        }
                        reader.Close();
                        // Code pour gérer le cas où le mot de passe est incorrect
                    }
                }
                
            }
            else // L'utilisateur n'existe pas encore
            {
                Console.WriteLine("Ceci est votre première connexion, veuillez créer un compte.");
                Console.Write("adresse mail\n--> ");
                email = Console.ReadLine();
                Console.Write("Nom\n--> ");
                string nom = Console.ReadLine();
                Console.Write("Prénom\n--> ");
                string prenom = Console.ReadLine();
                Console.Write("Numéro de téléphone\n--> ");
                string telephone = Console.ReadLine();
                Console.Write("Mot de passe\n--> ");
                string mdp = Console.ReadLine();
                Console.Write("Adresse de facturation\n--> ");
                string adresse = Console.ReadLine();
                Console.Write("Carte de crédit\n--> ");
                string carte = Console.ReadLine();

                // Insérer le nouvel utilisateur dans la base de données
                string query = "INSERT INTO client (adresse_mail, nom, prenom, telephone, mot_de_passe, adresse_facturation, carte_bancaire) VALUES (@adresse_mail, @nom, @prenom, @numero_de_telephone, @mot_de_passe, @adresse_de_facturation, @carte_bancaire)";
                MySqlCommand command = new MySqlCommand(query, maConnexion);
                command.Parameters.AddWithValue("@adresse_mail", email);
                command.Parameters.AddWithValue("@nom", nom);
                command.Parameters.AddWithValue("@prenom", prenom);
                command.Parameters.AddWithValue("@numero_de_telephone", telephone);
                command.Parameters.AddWithValue("@mot_de_passe", mdp);
                command.Parameters.AddWithValue("@adresse_de_facturation", adresse);
                command.Parameters.AddWithValue("@carte_bancaire", carte);


                command.ExecuteNonQuery(); //exécute la commande SQL sur la base de données sans retourner de données

                query = "INSERT INTO statut(adresse_mail,niveau,nb_achat) VALUES (@adresse_mail,@niveau,@nb_achat)";
                command = new MySqlCommand(query, maConnexion);
                command.Parameters.AddWithValue("@adresse_mail", email);
                command.Parameters.AddWithValue("@niveau", "AUCUN");
                command.Parameters.AddWithValue("@nb_achat", 0);
                Console.WriteLine("Bravo, votre compte a éte créé avec succès");
            }
            reader = CommandeSQL("SELECT niveau FROM statut WHERE adresse_mail='"+email+"';", maConnexion);
            string statut="";
            if(reader.Read())
            {
                statut = reader["niveau"].ToString();
            }
            reader.Close();
            if(statut=="ADMIN")
            {
                Admin(maConnexion);
            }
            else
            {
                Achat(maConnexion,email);
            }


            #endregion
        }
        #region commande SQL
        static MySqlDataReader CommandeSQL(string commande, MySqlConnection maConnexion)
        {
            MySqlCommand command = maConnexion.CreateCommand();
            command.CommandText =commande;
            return command.ExecuteReader();
        }
        #endregion
        #region Achat
        static void Achat(MySqlConnection maConnexion, string email)
        {

            MySqlDataReader reader = CommandeSQL("SELECT date_com FROM commande WHERE adresse_mail='" + email + "';", maConnexion);
            int nbachat = 0;
            string nouveauStatut = "AUCUN";
            while (reader.Read())
            {
                if (Convert.ToDateTime(reader["date_com"]).Month == DateTime.Now.Month)
                {
                    nbachat++;
                }
            }
            reader.Close();
            if (nbachat >= 1)
            {
                nouveauStatut = "BRONZE";
            }
            else if (nbachat >= 5)
            {
                nouveauStatut = "OR";
            }
            string query = "UPDATE statut SET niveau = @niveau WHERE adresse_mail = @email;";
            MySqlCommand command = new MySqlCommand(query, maConnexion);
            command.Parameters.AddWithValue("@niveau", nouveauStatut);
            command.Parameters.AddWithValue("@email", email);
            command.ExecuteNonQuery();
            #region Initialisation du numéro de commande
            reader = CommandeSQL("SELECT num_commande FROM commande ORDER BY num_commande DESC;", maConnexion);
            string numéroCommande = "";
            if (reader.Read())
            {
                numéroCommande = reader["num_commande"].ToString();
            }
            reader.Close();
            numéroCommande = Convert.ToString(Convert.ToInt32(numéroCommande.Substring(3)) + 1);
            for (int i = 0; numéroCommande.Length < 4; i++)
            {
                numéroCommande = "0" + numéroCommande;
            }
            numéroCommande = "CMD" + numéroCommande;

            double PrixCommande = 0.00;
            query = "INSERT INTO commande (num_commande, date_com, adresse_mail,prix_commande) VALUES (@num, @date, @adresse, @prix)";
            command = new MySqlCommand(query, maConnexion);
            command.Parameters.AddWithValue("@num", numéroCommande);
            command.Parameters.AddWithValue("@date", DateTime.Now);
            command.Parameters.AddWithValue("@adresse", email);
            command.Parameters.AddWithValue("@prix", PrixCommande);
            command.ExecuteNonQuery();


            #endregion


            bool continuer = true;
            while (continuer)
            {
                reader = CommandeSQL("SELECT num_bon_com FROM bon_de_commande ORDER BY num_bon_com DESC;", maConnexion);
                string numéroBC = "";
                string prixBC = "";
                if (reader.Read())
                {
                    numéroBC = reader["num_bon_com"].ToString();
                }
                reader.Close();
                numéroBC = Convert.ToString(Convert.ToInt32(numéroBC.Substring(2)) + 1);
                for (int i = 0; numéroBC.Length < 3; i++)
                {
                    numéroBC = "0" + numéroBC;
                }
                numéroBC = "BC" + numéroBC;


                string choix = "";
                Console.WriteLine("Souhaitez vous un bouquet personalisé (1) ou standard (2)?");
                while (choix != "1" && choix != "2")
                {
                    choix = Console.ReadLine();
                }
                Console.Clear();
                if (choix == "2")
                {
                    Console.WriteLine("Voici les bouquets à votre disposition:\n");


                    reader = CommandeSQL("SELECT nom_bouquet,occasion FROM bouquet WHERE disponible=1 AND type='standard';", maConnexion);
                    List<string> listeBouquets = new List<string>();
                    int index = 1;
                    while (reader.Read())
                    {
                        Console.WriteLine(index + " - " + reader["nom_bouquet"].ToString() + " pour " + reader["occasion"].ToString());
                        listeBouquets.Add(reader["nom_bouquet"].ToString());
                        index++;
                    }
                    reader.Close();
                    Console.WriteLine("Lequel choisissez vous ?");
                    choix = "-3";
                    while (Convert.ToInt32(choix) <= 0 || Convert.ToInt32(choix) > listeBouquets.Count)
                        choix = Console.ReadLine();

                    string bouquetChoisis = listeBouquets[Convert.ToInt32(choix) - 1];
                    Console.Clear();
                    Console.WriteLine("Très bien, souhaitez vous un accessoire avec ? (oui/non)");
                    while (choix != "oui" && choix != "non")
                    {
                        choix = Console.ReadLine();
                    }
                    Console.Clear();
                    string AccessoiresCommande = null;
                    if (choix == "oui")
                    {
                        AccessoiresCommande = "";
                        reader = CommandeSQL("SELECT nom_accessoire,prix_accessoire from accessoire;", maConnexion);
                        while (reader.Read())
                        {
                            Console.WriteLine("Vous en êtes à " + prixBC + "€");
                            Console.WriteLine(" - " + reader["nom_accessoire"].ToString() + " prix: " + reader["prix_accessoire"].ToString());
                            Console.WriteLine("Combien en voulez vous ? --> ");
                            int nbAccessoire = Convert.ToInt32(Console.ReadLine());
                            if (nbAccessoire > 0)
                            {
                                AccessoiresCommande += nbAccessoire + " " + reader["nom_accessoire"].ToString() + ", ";
                                prixBC = prixBC + Convert.ToDouble(reader["prix_accessoire"])*nbAccessoire;
                            }
                            Console.Clear();
                        }
                        Console.Clear();
                    }
                    reader = CommandeSQL("SELECT prix_bouquet FROM bouquet WHERE nom_bouquet='" + bouquetChoisis + "';", maConnexion);
                    while (reader.Read())
                    {
                        prixBC = reader["prix_bouquet"].ToString();
                    }
                    reader.Close();

                    Console.WriteLine("Rentrez une adresse pour votre commande\n--> ");
                    string adresse = Console.ReadLine();
                    Console.WriteLine("Rentrez une date de livraison pour votre commande(jour/mois/année)\n--> ");
                    DateTime dateLivraison = DateTime.Now;
                    while (dateLivraison < DateTime.Now.AddDays(3))
                        dateLivraison = Convert.ToDateTime(Console.ReadLine());


                    Console.Clear();

                    // Requête SQL pour ajouter une ligne sur la table bon_de_commande
                    query = "INSERT INTO bon_de_commande (num_bon_com, etat_commande, date_livraison, adresse_livraison, num_commande, nom_bouquet) " +
                                   "VALUES (@num_bon_com, @etat_commande, @date_livraison, @adresse_livraison, @num_commande, @nom_bouquet)";
                    command = new MySqlCommand(query, maConnexion);
                    // Création de la commande SQL
                    command.Parameters.AddWithValue("@num_bon_com", numéroBC);
                    command.Parameters.AddWithValue("@etat_commande", "VINV");
                    command.Parameters.AddWithValue("@date_livraison", dateLivraison);
                    command.Parameters.AddWithValue("@adresse_livraison", adresse);
                    command.Parameters.AddWithValue("@num_commande", numéroCommande);
                    command.Parameters.AddWithValue("@nom_bouquet", bouquetChoisis);
                    command.ExecuteNonQuery();
                }
                else
                {
                    Console.WriteLine("Voici les fleurs à votre disposition, faites votre bouquet: \n");
                    string Fleursbouquet="";
                    double prixBouquet = 0;
                    reader = CommandeSQL("SELECT nom_fleur,prix_fleur from fleur;", maConnexion);
                    while (reader.Read())
                    {

                        Console.WriteLine("Vous en êtes à "+prixBouquet+"€");
                        Console.WriteLine(" - " + reader["nom_fleur"].ToString()+" prix: "+ reader["prix_fleur"].ToString() + " pièce");
                        Console.WriteLine("Combien en voulez vous ? --> ");
                        int nbFleurs = Convert.ToInt32(Console.ReadLine());
                        if (nbFleurs > 0)
                        {
                            Fleursbouquet += nbFleurs + " " + reader["nom_fleur"].ToString() + ", ";
                            prixBouquet += + Convert.ToDouble(reader["prix_fleur"])*nbFleurs;
                            
                        }
                        Console.Clear();   
                    }
                    reader.Close();
                    


                    choix = "";
                    Console.WriteLine("Très bien, souhaitez vous un accessoire avec ? (oui/non)");
                    while (choix != "oui" && choix != "non")
                    {
                        choix = Console.ReadLine();
                    }
                    Console.Clear();
                    string AccessoiresCommande = null;
                    if (choix == "oui")
                    {
                        AccessoiresCommande = "";
                        reader = CommandeSQL("SELECT nom_accessoire,prix_accessoire from accessoire;", maConnexion);
                        while (reader.Read())
                        {

                            Console.WriteLine("Vous en êtes à " + prixBouquet + "€");
                            Console.WriteLine(" - " + reader["nom_accessoire"].ToString() + " prix: " + reader["prix_accessoire"].ToString()+" pièce");
                            Console.WriteLine("Combien en voulez vous ? --> ");
                            int nbAccessoire = Convert.ToInt32(Console.ReadLine());
                            if (nbAccessoire > 0)
                            {
                                AccessoiresCommande += nbAccessoire + " " + reader["nom_accessoire"].ToString() + ", ";
                                prixBouquet += Convert.ToDouble(reader["prix_accessoire"])*nbAccessoire;
                                
                            }
                            Console.Clear();

                        }
                        reader.Close();
                        Console.Clear();
                    }
                    string nomBouquet = "";
                    reader = CommandeSQL("SELECT Count(*) from bouquet;", maConnexion);
                    if (reader.Read())
                    {
                        nomBouquet = reader["Count(*)"].ToString();
                    }
                    reader.Close();

                    query = "INSERT INTO bouquet (nom_bouquet, type, fleurs_bouquet, accessoires_bouquet, prix_bouquet) " +
                                   "VALUES (@nombouquet, @type, @fleurs_bouquet, @accessoire, @prix_bouquet)";
                    command = new MySqlCommand(query, maConnexion);
                    // Création de la commande SQL
                    command.Parameters.AddWithValue("@nombouquet", nomBouquet);
                    command.Parameters.AddWithValue("@type", "personalisé");
                    command.Parameters.AddWithValue("@fleurs_bouquet", Fleursbouquet);
                    command.Parameters.AddWithValue("@accessoire", AccessoiresCommande);
                    command.Parameters.AddWithValue("@prix_bouquet", prixBouquet);

                    command.ExecuteNonQuery();

                    Console.WriteLine("Rentrez une adresse pour votre commande\n--> ");
                    string adresse = Console.ReadLine();
                    Console.WriteLine("Rentrez une date de livraison pour votre commande(jour/mois/année)\n--> ");
                    DateTime dateLivraison=DateTime.Now;
                    while (dateLivraison < DateTime.Now.AddDays(3))
                        dateLivraison = Convert.ToDateTime(Console.ReadLine());


                    Console.Clear();


                    // Requête SQL pour ajouter une ligne sur la table bon_de_commande
                    query = "INSERT INTO bon_de_commande (num_bon_com, etat_commande, date_livraison, adresse_livraison, num_commande, nom_bouquet) " +
                                   "VALUES (@num_bon_com, @etat_commande, @date_livraison, @adresse_livraison, @num_commande, @nom_bouquet)";
                    command = new MySqlCommand(query, maConnexion);
                    // Création de la commande SQL
                    command.Parameters.AddWithValue("@num_bon_com", numéroBC);
                    command.Parameters.AddWithValue("@etat_commande", "VINV");
                    command.Parameters.AddWithValue("@date_livraison", dateLivraison);
                    command.Parameters.AddWithValue("@adresse_livraison", adresse);
                    command.Parameters.AddWithValue("@num_commande", numéroCommande);
                    command.Parameters.AddWithValue("@nom_bouquet", nomBouquet);
                    command.ExecuteNonQuery();
                    prixBC = prixBouquet.ToString();
                }
                PrixCommande += Convert.ToDouble(prixBC);
                Console.WriteLine("Souhaitez vous continuer votre commande ?(oui/non)");
                choix = "";
                while (choix != "oui" && choix != "non")
                    choix = Console.ReadLine();
                if (choix == "non")
                {
                    continuer = false;
                    string réductionStatut = "";
                    reader = CommandeSQL("SELECT niveau FROM statut WHERE adresse_mail='" + email + "';", maConnexion);
                    while (reader.Read())
                    {
                        réductionStatut = reader["niveau"].ToString();
                    }
                    reader.Close();
                    
                    if (réductionStatut == "OR")
                    {
                        PrixCommande = PrixCommande * 0.85;
                    }
                    else if (réductionStatut == "BRONZE")
                    {
                        PrixCommande = PrixCommande* 0.95;
                    }
                    query = "UPDATE commande SET prix_commande = @nouveauPrix WHERE num_commande = @numCommande;";
                    command = new MySqlCommand(query, maConnexion);
                    command.Parameters.AddWithValue("@nouveauPrix", PrixCommande); 
                    command.Parameters.AddWithValue("@numCommande", numéroCommande);
                    command.ExecuteNonQuery();

                    nbachat = 0;
                    reader = CommandeSQL("SELECT nb_achats FROM statut WHERE adresse_mail='" + email + "';", maConnexion);
                    while (reader.Read())
                    {
                        nbachat = Convert.ToInt32(reader["nb_achats"]);
                    }
                    reader.Close();

                    query = "UPDATE statut SET nb_achats = @nb_achat WHERE adresse_mail = @email;";
                    command = new MySqlCommand(query, maConnexion);
                    command.Parameters.AddWithValue("@nb_achat", nbachat+1);
                    command.Parameters.AddWithValue("@email", email);
                    command.ExecuteNonQuery();

                }
            }
        }
        #endregion
        #region Admin
        static void Admin(MySqlConnection maConnexion)
        {
            // calcul du prix moyen de bouquet acheté
            MySqlDataReader reader = CommandeSQL("SELECT avg(prix_bouquet) FROM bouquet; ",maConnexion);
            double prixMoyen = 0;
            if (reader.Read())
            {
                prixMoyen = Convert.ToDouble(reader["avg(prix_bouquet)"].ToString());
            }
            reader.Close();
            Console.WriteLine("Le prix moyen d'un bouquet acheté est de " + prixMoyen);
            Separation();

            //meilleur client du mois,
            reader = CommandeSQL("SELECT c.nom, c.prenom, c.adresse_mail, SUM(co.prix_commande) AS total_depense FROM commande co JOIN client c ON co.adresse_mail = c.adresse_mail WHERE co.date_com >= DATE_SUB(CURRENT_DATE(), INTERVAL 1 MONTH) GROUP BY c.adresse_mail ORDER BY total_depense DESC; ",maConnexion);
            string clientMois = "";
            if (reader.Read())
            {
                clientMois = reader.GetString("prenom")+" "+reader.GetString("nom");
            }
            reader.Close();
            Console.WriteLine("Le meilleur client du mois est " + clientMois);
            Separation();

            // meilleur client année
            reader = CommandeSQL("SELECT c.nom, c.prenom, c.adresse_mail, SUM(co.prix_commande) AS total_depense FROM commande co JOIN client c ON co.adresse_mail = c.adresse_mail WHERE co.date_com >= DATE_SUB(CURRENT_DATE(), INTERVAL 1 YEAR) GROUP BY c.adresse_mail ORDER BY total_depense DESC;",maConnexion);
            string clientAnnee = "";
            if (reader.Read())
            {
                clientAnnee = reader.GetString("prenom") + " " + reader.GetString("nom");
            }
            reader.Close();
            Console.WriteLine("Le meilleur client de l'année est " + clientAnnee);
            Separation();

            // bouquet standart le plus vendu
            string nomBouquet = "";
            reader = CommandeSQL("SELECT nom_bouquet, COUNT(*) AS total_ventes FROM bon_de_commande GROUP BY nom_bouquet ORDER BY total_ventes DESC LIMIT 1;",maConnexion);
            if (reader.Read())
            {
                nomBouquet = reader.GetString("nom_bouquet");
            }
            reader.Close();
            Console.WriteLine("Le bouquet le plus vendu est  " + nomBouquet);
            Separation();

            //clients ayant fait le plus de commande (pas nécesairement le plus dépensé)
            reader = CommandeSQL("SELECT c.nom, c.prenom, c.adresse_mail, COUNT(*) as nb_commandes FROM client c INNER JOIN commande cm ON c.adresse_mail = cm.adresse_mail WHERE cm.prix_commande > 50 GROUP BY c.adresse_mail ORDER BY nb_commandes DESC;", maConnexion);
            if(reader.Read())
            {
                Console.WriteLine("Le client ayant fait le plus de commande est " + reader["prenom"].ToString() + " " +reader["nom"].ToString());
            }
            reader.Close ();
            Separation();

            //bouquets dont le prix est supérieur à la moyenne de tous les bouquets proposés
            reader = CommandeSQL("SELECT b1.nom_bouquet FROM bouquet b1 INNER JOIN (SELECT AVG(prix_bouquet) as prix_moyen FROM bouquet) b2 WHERE b1.prix_bouquet > b2.prix_moyen;", maConnexion);
            Console.WriteLine("Les bouquets les plus chers sont:");
            while(reader.Read())
            {
                Console.WriteLine(reader["nom_bouquet"].ToString());
            }
            reader.Close();
            Separation();
            //le nom et la quantité en stock de tous les accessoires et de toutes les fleurs dont la disponibilité est "en stock" dans une seule requête
            reader = CommandeSQL("SELECT nom_accessoire as nom, nombre_accessoire_stock as quantite_stock, 'accessoire' as type FROM accessoire WHERE disponibilite_accessoire = 'disponible' UNION SELECT nom_fleur as nom, nombre_fleur_stock as quantite_stock, 'fleur' as type FROM fleur WHERE disponibilite_fleur = 'disponible' ORDER BY quantite_stock;", maConnexion);
            Console.WriteLine("Stock disponible:");
            while(reader.Read())
            {
                Console.WriteLine(reader["nom"].ToString() + " --> " + reader["quantite_stock"].ToString()+" en stock");
            }
            reader.Close();
            string choix = "";
            while(choix!="5")
            {
                Separation();
                Console.WriteLine("Si vous voulez ajouter une nouvelle fleur, tapez 1\nSi vous voulez ajouter un nouvel accessoire, tapez 2\nSi vous voulez refournir les stocks de fleur, tapez 3\nSi vous voulez refournir les stocks d'accessoire, tapez 4\nsinon tapez 5");
                choix = "";
                while (choix != "1" && choix != "2" && choix != "3" && choix != "4" && choix != "5")
                    choix = Console.ReadLine();
                if (choix == "1")
                    AjoutFleur(maConnexion);
                else if (choix == "2")
                    AjoutAccessoire(maConnexion);
                else if (choix == "3")
                    StockFleur(maConnexion);
                else if (choix == "4")
                    StockAccessoire(maConnexion);
            }
        }
        #endregion
        #region Separation

        static void Separation()
        {
            for (int i = 0; i < 30; i++)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }
        #endregion
        #region Ajout nouvelle fleur
        static void AjoutFleur(MySqlConnection maConnexion)
        {
            string choix = "oui";
            while(choix=="oui")
            {
                Console.WriteLine("Entrez le nom de la fleur à ajouter :");
                string nomFleur = Console.ReadLine();

                Console.WriteLine("Entrez le nombre de fleurs à ajouter :");
                int nombreFleurs = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Entrez le prix de la fleur à ajouter :");
                decimal prixFleur = Convert.ToDecimal(Console.ReadLine());

                MySqlCommand command = new MySqlCommand();
                command.Connection = maConnexion;
                try
                {
                    command.CommandText = "INSERT INTO fleur (nom_fleur, nombre_fleur_stock, disponibilite_fleur, prix_fleur) " +
                    "VALUES (@nomFleur, @nombreFleurs, @disponibiliteFleur, @prixFleur)";
                    command.Parameters.AddWithValue("@nomFleur", nomFleur);
                    command.Parameters.AddWithValue("@nombreFleurs", nombreFleurs);
                    command.Parameters.AddWithValue("@disponibiliteFleur", "Disponible");
                    command.Parameters.AddWithValue("@prixFleur", prixFleur);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Les fleurs ont été ajoutées au stock avec succès !");
                }
                catch(Exception e)
                {
                    Console.WriteLine("Erreur : " + e.ToString());
                }
                
                Console.WriteLine("Voulez vous continuer à ajouter des fleurs ?");
                choix = "";
                while(choix!="oui" && choix!="non")
                    choix=Console.ReadLine().ToLower();
            }
            
        }
        #endregion
        #region Ajout nouvel accessoire
        static void AjoutAccessoire(MySqlConnection maConnexion)
        {
            string choix = "oui";
            while (choix == "oui")
            {
                Console.WriteLine("Entrez le nom de l'accessoire à ajouter :");
                string nom = Console.ReadLine();

                Console.WriteLine("Entrez le nombre d'accessoires à ajouter :");
                int nombre = Convert.ToInt32(Console.ReadLine());


                Console.WriteLine("Entrez le prix de l'accessoire à ajouter :");
                decimal prix = Convert.ToDecimal(Console.ReadLine());

                MySqlCommand command = new MySqlCommand();
                command.Connection=maConnexion;
                try
                {
                    command.CommandText = "INSERT INTO accessoire (nom_accessoire, nombre_accessoire_stock, disponibilite_accessoire, prix_accessoire) VALUES (@nom, @nombre, @disponibilite, @prix)";
                    command.Parameters.AddWithValue("@nom", nom);
                    command.Parameters.AddWithValue("@nombre", nombre);
                    command.Parameters.AddWithValue("@disponibilite", "Disponible");
                    command.Parameters.AddWithValue("@prix", prix);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Accessoire ajouté avec succès.");
                }catch(Exception e)
                {
                    Console.WriteLine("Erreur : " + e.ToString());
                }
                Console.WriteLine("Souhaitez vous continuer à ajouter des accessoires ?");
                choix = "";
                while (choix != "oui" && choix != "non")
                    choix = Console.ReadLine().ToLower();

            }
        }
        #endregion
        #region Changement stock fleur
        static void StockFleur(MySqlConnection maConnexion)
        {
            string continuer = "oui";
            while (continuer == "oui")
            {
                MySqlDataReader reader = CommandeSQL("SELECT nom_fleur FROM fleur;", maConnexion);
                List<string> listeFleur = new List<string>();
                int index = 1;
                Console.WriteLine("Quelle fleur voulez vous approvisionner ?");
                while (reader.Read())
                {
                    Console.WriteLine(index + " - " + reader["nom_fleur"].ToString());
                    listeFleur.Add(reader["nom_fleur"].ToString());
                    index++;
                }
                reader.Close();
                Console.Write("--> ");
                string choix = "-3";
                while (Convert.ToInt32(choix) <= 0 || Convert.ToInt32(choix) > listeFleur.Count)
                    choix = Console.ReadLine();

                string fleurChoisie = listeFleur[Convert.ToInt32(choix) - 1];
                Console.WriteLine("Quelle est la nouvelle valeur de stock de cette fleur ?");
                string query = "UPDATE fleur SET nombre_fleur_stock = @nouvelleValeur WHERE nom_fleur = @nomFleur";
                MySqlCommand command = new MySqlCommand(query, maConnexion);
                command.Parameters.AddWithValue("@nouvelleValeur", Convert.ToInt32(Console.ReadLine()));
                command.Parameters.AddWithValue("@nomFleur", fleurChoisie);
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Valeur ajoutée avec succès");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur:" + e.ToString());
                }
                Console.WriteLine("Voulez vous continuer à ajouter des fleurs ?");
                continuer = "";
                while (continuer != "oui" && continuer != "non")
                    continuer = Console.ReadLine().ToLower();
            }            
        }
        #endregion
        #region Changement stock accessoire
        static void StockAccessoire(MySqlConnection maConnexion)
        {
            string continuer = "oui";
            while (continuer == "oui")
            {
                MySqlDataReader reader = CommandeSQL("SELECT nom_accessoire FROM accessoire;", maConnexion);
                List<string> listeAccessoire = new List<string>();
                int index = 1;
                Console.WriteLine("Quel accessoire voulez vous approvisionner ?");
                while (reader.Read())
                {
                    Console.WriteLine(index + " - " + reader["nom_accessoire"].ToString());
                    listeAccessoire.Add(reader["nom_accessoire"].ToString());
                    index++;
                }
                reader.Close();
                Console.Write("--> ");
                string choix = "-3";
                while (Convert.ToInt32(choix) <= 0 || Convert.ToInt32(choix) > listeAccessoire.Count)
                    choix = Console.ReadLine();

                string accessoireChoisis = listeAccessoire[Convert.ToInt32(choix) - 1];
                Console.WriteLine("Quelle est la nouvelle valeur de stock de cet accessoire ?");
                string query = "UPDATE accessoire SET nombre_accessoire_stock = @nouvelleValeur WHERE nom_accessoire = @nom_accessoire";
                MySqlCommand command = new MySqlCommand(query, maConnexion);
                command.Parameters.AddWithValue("@nouvelleValeur", Convert.ToInt32(Console.ReadLine()));
                command.Parameters.AddWithValue("@nom_accessoire", accessoireChoisis);
                try
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Valeur ajoutée avec succès");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Erreur:" + e.ToString());
                }
                Console.WriteLine("Voulez vous continuer à ajouter des accessoire ?");
                continuer = "";
                while (continuer != "oui" && continuer != "non")
                    continuer = Console.ReadLine().ToLower();
            }
            
        }
        #endregion
    }
}
