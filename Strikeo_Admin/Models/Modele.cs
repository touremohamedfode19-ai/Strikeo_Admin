// Importation de la bibliothèque System (classes de base .NET)
using System;
// Importation du connecteur MySQL pour C#
using MySql.Data.MySqlClient;
// Importation pour les classes communes d'accès aux données
using System.Data.Common;
// Importation pour le débogage (afficher les erreurs dans la console)
using System.Diagnostics;

// Définition du namespace du projet
namespace Strikeo_Admin
{
    // Classe Modele : gère toutes les interactions avec la base de données MySQL
    public class Modele
    {
        // ===== ATTRIBUTS PRIVÉS =====
        // Informations de connexion à la base de données

        // Adresse du serveur MySQL (ex: "localhost" ou "127.0.0.1")
        private string serveur;
        // Nom de la base de données à utiliser
        private string bdd;
        // Nom d'utilisateur MySQL
        private string user;
        // Mot de passe MySQL
        private string mdp;
        // Objet de connexion MySQL - permet d'ouvrir/fermer la connexion
        private MySqlConnection maConnexion;

        // ===== CONSTRUCTEUR =====
        // Initialise la connexion à la base de données avec les paramètres fournis
        public Modele(string serveur, string bdd, string user, string mdp)
        {
            // Stockage des paramètres dans les attributs de l'objet
            this.serveur = serveur;
            this.bdd = bdd;
            this.user = user;
            this.mdp = mdp;

            // Construction de la chaîne de connexion (URL de connexion)
            string url = "Server=" + this.serveur;
            // Port par défaut de MySQL
            url += "; Port=3306";
            // Nom de la base de données
            url += "; Database=" + this.bdd;
            // Identifiant utilisateur
            url += "; User Id=" + this.user;
            // Mot de passe
            url += "; Password=" + this.mdp;

            // Tentative de création de l'objet de connexion
            try
            {
                // Création de la connexion avec l'URL construite
                this.maConnexion = new MySqlConnection(url);
                // Test de la connexion
                this.maConnexion.Open();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                // En cas d'erreur, affichage dans la console de débogage
                Debug.WriteLine("Erreur de connexion a : " + url);
                Debug.WriteLine("Message: " + exp.Message);
                throw new Exception("Impossible de se connecter à MySQL: " + exp.Message);
            }
        }

        // ========================================================
        // ================ MÉTHODES POUR EQUIPE ==================
        // ========================================================

        // ----- INSERT : Ajouter une nouvelle équipe -----
        // Paramètre : objet Equipe contenant les données à insérer
        public void InsertEquipe(Equipe uneEquipe)
        {
            // Requête SQL d'insertion avec des paramètres nommés (@nom, @nb)
            // "null" pour l'id car il est auto-incrémenté par MySQL
            string requete = "INSERT INTO equipe VALUES (null, @nom, @nb);";
            // Variable pour stocker la commande SQL
            MySqlCommand uneCmde = null;
            try
            {
                // Ouverture de la connexion à la base de données
                this.maConnexion.Open();
                // Création d'une commande SQL liée à cette connexion
                uneCmde = this.maConnexion.CreateCommand();
                // Affectation de la requête à la commande
                uneCmde.CommandText = requete;

                // Liaison des paramètres avec les valeurs de l'objet Equipe
                // @nom sera remplacé par la valeur de uneEquipe.Nom_equipe
                uneCmde.Parameters.AddWithValue("@nom", uneEquipe.Nom_equipe);
                // @nb sera remplacé par le nombre de joueurs
                uneCmde.Parameters.AddWithValue("@nb", uneEquipe.Nb_joueur);

                // Exécution de la requête (INSERT ne retourne pas de données)
                uneCmde.ExecuteNonQuery();
                // Fermeture de la connexion pour libérer les ressources
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                // En cas d'erreur, affichage dans la console de débogage
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- DELETE : Supprimer une équipe par son id -----
        // Paramètre : l'identifiant de l'équipe à supprimer
        public void DeleteEquipe(int idequipe)
        {
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();

                // 1. Supprimer toutes les participations de cette équipe
                uneCmde.CommandText = "DELETE FROM participation WHERE idequipe = @id;";
                uneCmde.Parameters.AddWithValue("@id", idequipe);
                uneCmde.ExecuteNonQuery();

                // 2. Mettre à NULL l'équipe des joueurs appartenant à cette équipe
                uneCmde.CommandText = "UPDATE joueur SET idequipe = NULL WHERE idequipe = @id;";
                uneCmde.ExecuteNonQuery();

                // 3. Supprimer l'équipe
                uneCmde.CommandText = "DELETE FROM equipe WHERE idequipe = @id;";
                uneCmde.ExecuteNonQuery();

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution DeleteEquipe : " + exp.Message);
                throw;
            }
            finally
            {
                if (this.maConnexion.State == System.Data.ConnectionState.Open)
                    this.maConnexion.Close();
            }
        }

        // ----- UPDATE : Modifier une équipe existante -----
        // Paramètre : objet Equipe avec les nouvelles valeurs
        public void UpdateEquipe(Equipe uneEquipe)
        {
            // Requête SQL de mise à jour - SET définit les nouvelles valeurs
            // WHERE identifie l'enregistrement à modifier
            string requete = "UPDATE equipe SET nom_equipe = @nom, nb_joueur = @nb WHERE idequipe = @id;";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                // Liaison de tous les paramètres
                uneCmde.Parameters.AddWithValue("@id", uneEquipe.Idequipe);
                uneCmde.Parameters.AddWithValue("@nom", uneEquipe.Nom_equipe);
                uneCmde.Parameters.AddWithValue("@nb", uneEquipe.Nb_joueur);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- SELECT ALL : Récupérer toutes les équipes (avec filtre optionnel) -----
        // Paramètre : chaîne de recherche (vide = toutes les équipes)
        // Retourne : une liste d'objets Equipe
        public List<Equipe> SelectAllEquipes(string filtre)
        {
            // Création d'une liste vide pour stocker les résultats
            List<Equipe> lesEquipes = new List<Equipe>();
            string requete;

            // Si aucun filtre n'est fourni, on récupère tout
            if (String.IsNullOrEmpty(filtre))
            {
                requete = "SELECT * FROM equipe;";
            }
            else
            {
                // Sinon, on filtre sur le nom de l'équipe avec LIKE
                // LIKE permet une recherche partielle (contient)
                requete = "SELECT * FROM equipe WHERE nom_equipe LIKE @filtre;";
            }

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                // Si un filtre est fourni, on l'ajoute avec les % pour LIKE
                // % = n'importe quels caractères avant et après
                if (!String.IsNullOrEmpty(filtre))
                {
                    uneCmde.Parameters.AddWithValue("@filtre", "%" + filtre + "%");
                }

                // Exécution de la requête SELECT - retourne un "reader"
                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    // Vérifie si des résultats ont été trouvés
                    if (unReader.HasRows)
                    {
                        // Parcourt chaque ligne de résultat
                        while (unReader.Read())
                        {
                            // Création d'un objet Equipe avec les données de la ligne
                            // GetInt32(0) = 1ère colonne (idequipe)
                            // GetString(1) = 2ème colonne (nom_equipe)
                            // GetInt32(2) = 3ème colonne (nb_joueur)
                            Equipe uneEquipe = new Equipe(
                                unReader.GetInt32(0),
                                unReader.GetString(1),
                                unReader.GetInt32(2)
                            );
                            // Ajout de l'équipe à la liste
                            lesEquipes.Add(uneEquipe);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Aucune équipe à extraire");
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            // Retourne la liste (peut être vide si aucun résultat)
            return lesEquipes;
        }

        // ========================================================
        // ================ MÉTHODES POUR JOUEUR ==================
        // ========================================================

        // ----- INSERT : Ajouter un nouveau joueur -----
        public void InsertJoueur(Joueur unJoueur)
        {
            // Requête d'insertion avec tous les champs du joueur
            // null pour l'id car auto-incrémenté
            string requete = "INSERT INTO joueur VALUES (null, @nom, @prenom, @age, @mail, @tel, @idequipe);";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                // Liaison des paramètres avec les propriétés de l'objet Joueur
                uneCmde.Parameters.AddWithValue("@nom", unJoueur.Nom_joueur);
                uneCmde.Parameters.AddWithValue("@prenom", unJoueur.Prenom_joueur);
                uneCmde.Parameters.AddWithValue("@age", unJoueur.Age_joueur);
                uneCmde.Parameters.AddWithValue("@mail", unJoueur.Mail_joueur);
                uneCmde.Parameters.AddWithValue("@tel", unJoueur.Telephone);
                // Clé étrangère vers l'équipe (peut être null)
                uneCmde.Parameters.AddWithValue("@idequipe", unJoueur.Idequipe.HasValue ? (object)unJoueur.Idequipe.Value : DBNull.Value);

                uneCmde.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete + " - " + exp.Message);
                throw;
            }
            finally
            {
                if (this.maConnexion.State == System.Data.ConnectionState.Open)
                    this.maConnexion.Close();
            }
        }

        // ----- DELETE : Supprimer un joueur par son id -----
        public void DeleteJoueur(int idjoueur)
        {
            string requete = "DELETE FROM joueur WHERE idjoueur = @id;";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", idjoueur);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- UPDATE : Modifier un joueur existant -----
        public void UpdateJoueur(Joueur unJoueur)
        {
            // Mise à jour de tous les champs sauf l'id
            string requete = "UPDATE joueur SET nom_joueur = @nom, prenom_joueur = @prenom, " +
                            "age_joueur = @age, mail_joueur = @mail, telephone = @tel, " +
                            "idequipe = @idequipe WHERE idjoueur = @id;";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                // Liaison de tous les paramètres
                uneCmde.Parameters.AddWithValue("@id", unJoueur.Idjoueur);
                uneCmde.Parameters.AddWithValue("@nom", unJoueur.Nom_joueur);
                uneCmde.Parameters.AddWithValue("@prenom", unJoueur.Prenom_joueur);
                uneCmde.Parameters.AddWithValue("@age", unJoueur.Age_joueur);
                uneCmde.Parameters.AddWithValue("@mail", unJoueur.Mail_joueur);
                uneCmde.Parameters.AddWithValue("@tel", unJoueur.Telephone);
                // Clé étrangère vers l'équipe (peut être null)
                uneCmde.Parameters.AddWithValue("@idequipe", unJoueur.Idequipe.HasValue ? (object)unJoueur.Idequipe.Value : DBNull.Value);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- SELECT ALL : Récupérer tous les joueurs (avec filtre optionnel) -----
        public List<Joueur> SelectAllJoueurs(string filtre)
        {
            List<Joueur> lesJoueurs = new List<Joueur>();
            string requete;

            if (String.IsNullOrEmpty(filtre))
            {
                requete = "SELECT * FROM joueur;";
            }
            else
            {
                // Recherche sur le nom, prénom ou email
                requete = "SELECT * FROM joueur WHERE nom_joueur LIKE @filtre " +
                         "OR prenom_joueur LIKE @filtre OR mail_joueur LIKE @filtre;";
            }

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                if (!String.IsNullOrEmpty(filtre))
                {
                    uneCmde.Parameters.AddWithValue("@filtre", "%" + filtre + "%");
                }

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        while (unReader.Read())
                        {
                            // Création d'un objet Joueur avec toutes les colonnes
                            Joueur unJoueur = new Joueur(
                                unReader.GetInt32(0),      // idjoueur
                                unReader.GetString(1),     // nom_joueur
                                unReader.GetString(2),     // prenom_joueur
                                unReader.GetInt32(3),      // age_joueur
                                unReader.GetString(4),     // mail_joueur
                                unReader.GetString(5),     // telephone
                                unReader.IsDBNull(6) ? null : unReader.GetInt32(6)  // idequipe (clé étrangère, peut être null)
                            );
                            lesJoueurs.Add(unJoueur);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Aucun joueur à extraire");
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return lesJoueurs;
        }

        // ========================================================
        // ================ MÉTHODES POUR TOURNOI =================
        // ========================================================

        // ----- INSERT : Ajouter un nouveau tournoi -----
        public void InsertTournoi(Tournoi unTournoi)
        {
            string requete = "INSERT INTO tournoi VALUES (null, @designation, @date, @description);";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                // Gérer les valeurs nulles
                uneCmde.Parameters.AddWithValue("@designation", unTournoi.Designation ?? "");
                uneCmde.Parameters.AddWithValue("@date", unTournoi.Date_tournoi);
                uneCmde.Parameters.AddWithValue("@description", unTournoi.Description ?? "");

                int rowsAffected = uneCmde.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new Exception("Aucune ligne insérée dans la base de données");
                }
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete + " - " + exp.Message);
                throw;
            }
            finally
            {
                if (this.maConnexion.State == System.Data.ConnectionState.Open)
                    this.maConnexion.Close();
            }
        }

        // ----- DELETE : Supprimer un tournoi par son id -----
        public void DeleteTournoi(int idtournoi)
        {
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();

                // 1. Supprimer toutes les participations de ce tournoi
                uneCmde.CommandText = "DELETE FROM participation WHERE idtournoi = @id;";
                uneCmde.Parameters.AddWithValue("@id", idtournoi);
                uneCmde.ExecuteNonQuery();

                // 2. Supprimer le tournoi
                uneCmde.CommandText = "DELETE FROM tournoi WHERE idtournoi = @id;";
                uneCmde.ExecuteNonQuery();

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution DeleteTournoi : " + exp.Message);
                throw;
            }
            finally
            {
                if (this.maConnexion.State == System.Data.ConnectionState.Open)
                    this.maConnexion.Close();
            }
        }

        // ----- UPDATE : Modifier un tournoi existant -----
        public void UpdateTournoi(Tournoi unTournoi)
        {
            string requete = "UPDATE tournoi SET designation = @designation, " +
                            "date_tournoi = @date, description = @description " +
                            "WHERE idtournoi = @id;";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", unTournoi.Idtournoi);
                uneCmde.Parameters.AddWithValue("@designation", unTournoi.Designation);
                uneCmde.Parameters.AddWithValue("@date", unTournoi.Date_tournoi);
                uneCmde.Parameters.AddWithValue("@description", unTournoi.Description);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- SELECT ALL : Récupérer tous les tournois (avec filtre optionnel) -----
        public List<Tournoi> SelectAllTournois(string filtre)
        {
            List<Tournoi> lesTournois = new List<Tournoi>();
            string requete;

            if (String.IsNullOrEmpty(filtre))
            {
                requete = "SELECT * FROM tournoi;";
            }
            else
            {
                // Recherche sur la désignation ou la description
                requete = "SELECT * FROM tournoi WHERE designation LIKE @filtre " +
                         "OR description LIKE @filtre;";
            }

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                if (!String.IsNullOrEmpty(filtre))
                {
                    uneCmde.Parameters.AddWithValue("@filtre", "%" + filtre + "%");
                }

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        while (unReader.Read())
                        {
                            // Création d'un objet Tournoi (gérer les valeurs NULL)
                            Tournoi unTournoi = new Tournoi(
                                unReader.GetInt32(0),      // idtournoi
                                unReader.IsDBNull(1) ? "" : unReader.GetString(1),     // designation
                                unReader.GetDateTime(2),   // date_tournoi
                                unReader.IsDBNull(3) ? "" : unReader.GetString(3)      // description (peut être NULL)
                            );
                            lesTournois.Add(unTournoi);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Erreur lecture tournois: " + exp.Message);
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return lesTournois;
        }

        // ========================================================
        // ============= MÉTHODES POUR PARTICIPATION ==============
        // ========================================================

        // ----- INSERT : Ajouter une nouvelle participation -----
        public void InsertParticipation(Participation uneParticipation)
        {
            string requete = "INSERT INTO participation VALUES (null, @date, @statut, @idtournoi, @idequipe);";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@date", uneParticipation.Date_inscription);
                // Le statut est un ENUM en BDD, on envoie la chaîne correspondante
                uneCmde.Parameters.AddWithValue("@statut", uneParticipation.Statut);
                // Clés étrangères
                uneCmde.Parameters.AddWithValue("@idtournoi", uneParticipation.Idtournoi);
                uneCmde.Parameters.AddWithValue("@idequipe", uneParticipation.Idequipe);

                uneCmde.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete + " - " + exp.Message);
                throw;
            }
            finally
            {
                if (this.maConnexion.State == System.Data.ConnectionState.Open)
                    this.maConnexion.Close();
            }
        }

        // ----- DELETE : Supprimer une participation par son id -----
        public void DeleteParticipation(int idparticipation)
        {
            string requete = "DELETE FROM participation WHERE idparticipation = @id;";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", idparticipation);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- UPDATE : Modifier une participation existante -----
        public void UpdateParticipation(Participation uneParticipation)
        {
            string requete = "UPDATE participation SET date_inscription = @date, " +
                            "statut = @statut, idtournoi = @idtournoi, idequipe = @idequipe " +
                            "WHERE idparticipation = @id;";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", uneParticipation.Idparticipation);
                uneCmde.Parameters.AddWithValue("@date", uneParticipation.Date_inscription);
                uneCmde.Parameters.AddWithValue("@statut", uneParticipation.Statut);
                uneCmde.Parameters.AddWithValue("@idtournoi", uneParticipation.Idtournoi);
                uneCmde.Parameters.AddWithValue("@idequipe", uneParticipation.Idequipe);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- SELECT ALL : Récupérer toutes les participations (avec filtre optionnel) -----
        public List<Participation> SelectAllParticipations(string filtre)
        {
            List<Participation> lesParticipations = new List<Participation>();
            string requete;

            if (String.IsNullOrEmpty(filtre))
            {
                requete = "SELECT * FROM participation;";
            }
            else
            {
                // Recherche sur le statut
                requete = "SELECT * FROM participation WHERE statut LIKE @filtre;";
            }

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                if (!String.IsNullOrEmpty(filtre))
                {
                    uneCmde.Parameters.AddWithValue("@filtre", "%" + filtre + "%");
                }

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        while (unReader.Read())
                        {
                            // Création d'un objet Participation
                            Participation uneParticipation = new Participation(
                                unReader.GetInt32(0),      // idparticipation
                                unReader.GetDateTime(1),   // date_inscription
                                unReader.GetString(2),     // statut
                                unReader.GetInt32(3),      // idtournoi
                                unReader.GetInt32(4)       // idequipe
                            );
                            lesParticipations.Add(uneParticipation);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Aucune participation à extraire");
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return lesParticipations;
        }

        // ========================================================
        // ============= MÉTHODES SELECT BY ID ====================
        // ========================================================

        // ----- SELECT BY ID : Récupérer une équipe par son id -----
        // Paramètre : l'identifiant de l'équipe recherchée
        // Retourne : l'objet Equipe correspondant ou null si non trouvé
        public Equipe SelectEquipeById(int idequipe)
        {
            // Variable pour stocker l'équipe trouvée (null par défaut)
            Equipe uneEquipe = null;
            // Requête SQL pour sélectionner une équipe par son id
            string requete = "SELECT * FROM equipe WHERE idequipe = @id;";
            MySqlCommand uneCmde = null;

            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                // Liaison du paramètre @id avec la valeur passée en argument
                uneCmde.Parameters.AddWithValue("@id", idequipe);

                // Exécution de la requête
                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    // Si un résultat est trouvé
                    if (unReader.HasRows)
                    {
                        // Lecture de la première (et unique) ligne
                        unReader.Read();
                        // Création de l'objet Equipe avec les données
                        uneEquipe = new Equipe(
                            unReader.GetInt32(0),      // idequipe
                            unReader.GetString(1),     // nom_equipe
                            unReader.GetInt32(2)       // nb_joueur
                        );
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Équipe non trouvée avec l'id : " + idequipe);
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            // Retourne l'équipe (ou null si non trouvée)
            return uneEquipe;
        }

        // ----- SELECT BY ID : Récupérer un joueur par son id -----
        // Paramètre : l'identifiant du joueur recherché
        // Retourne : l'objet Joueur correspondant ou null si non trouvé
        public Joueur SelectJoueurById(int idjoueur)
        {
            Joueur unJoueur = null;
            string requete = "SELECT * FROM joueur WHERE idjoueur = @id;";
            MySqlCommand uneCmde = null;

            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", idjoueur);

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        unReader.Read();
                        // Création de l'objet Joueur avec toutes les colonnes
                        unJoueur = new Joueur(
                            unReader.GetInt32(0),      // idjoueur
                            unReader.GetString(1),     // nom_joueur
                            unReader.GetString(2),     // prenom_joueur
                            unReader.GetInt32(3),      // age_joueur
                            unReader.GetString(4),     // mail_joueur
                            unReader.GetString(5),     // telephone
                            unReader.IsDBNull(6) ? null : unReader.GetInt32(6)  // idequipe (peut être null)
                        );
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Joueur non trouvé avec l'id : " + idjoueur);
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return unJoueur;
        }

        // ----- SELECT BY ID : Récupérer un tournoi par son id -----
        // Paramètre : l'identifiant du tournoi recherché
        // Retourne : l'objet Tournoi correspondant ou null si non trouvé
        public Tournoi SelectTournoiById(int idtournoi)
        {
            Tournoi unTournoi = null;
            string requete = "SELECT * FROM tournoi WHERE idtournoi = @id;";
            MySqlCommand uneCmde = null;

            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", idtournoi);

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        unReader.Read();
                        unTournoi = new Tournoi(
                            unReader.GetInt32(0),      // idtournoi
                            unReader.GetString(1),     // designation
                            unReader.GetDateTime(2),   // date_tournoi
                            unReader.GetString(3)      // description
                        );
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Tournoi non trouvé avec l'id : " + idtournoi);
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return unTournoi;
        }

        // ----- SELECT BY ID : Récupérer une participation par son id -----
        // Paramètre : l'identifiant de la participation recherchée
        // Retourne : l'objet Participation correspondant ou null si non trouvé
        public Participation SelectParticipationById(int idparticipation)
        {
            Participation uneParticipation = null;
            string requete = "SELECT * FROM participation WHERE idparticipation = @id;";
            MySqlCommand uneCmde = null;

            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", idparticipation);

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        unReader.Read();
                        uneParticipation = new Participation(
                            unReader.GetInt32(0),      // idparticipation
                            unReader.GetDateTime(1),   // date_inscription
                            unReader.GetString(2),     // statut
                            unReader.GetInt32(3),      // idtournoi
                            unReader.GetInt32(4)       // idequipe
                        );
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Participation non trouvée avec l'id : " + idparticipation);
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return uneParticipation;
        }

        // ========================================================
        // ============= MÉTHODES AVEC JOINTURES ==================
        // ========================================================

        // ----- Récupérer tous les joueurs avec le nom de leur équipe -----
        // Utilise une jointure (JOIN) pour combiner les tables joueur et equipe
        // Retourne une liste de tableaux de strings contenant les infos joueur + équipe
        public List<string[]> SelectJoueursAvecEquipe()
        {
            // Liste pour stocker les résultats (chaque élément est un tableau de strings)
            List<string[]> lesResultats = new List<string[]>();

            // Requête SQL avec INNER JOIN pour relier joueur et equipe
            // INNER JOIN : ne retourne que les joueurs qui ont une équipe existante
            string requete = "SELECT j.idjoueur, j.nom_joueur, j.prenom_joueur, j.age_joueur, " +
                            "j.mail_joueur, j.telephone, e.idequipe, e.nom_equipe " +
                            "FROM joueur j " +
                            "INNER JOIN equipe e ON j.idequipe = e.idequipe;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        while (unReader.Read())
                        {
                            // Création d'un tableau avec toutes les colonnes
                            string[] unResultat = new string[8];
                            unResultat[0] = unReader.GetInt32(0).ToString();   // idjoueur
                            unResultat[1] = unReader.GetString(1);              // nom_joueur
                            unResultat[2] = unReader.GetString(2);              // prenom_joueur
                            unResultat[3] = unReader.GetInt32(3).ToString();    // age_joueur
                            unResultat[4] = unReader.GetString(4);              // mail_joueur
                            unResultat[5] = unReader.GetString(5);              // telephone
                            unResultat[6] = unReader.GetInt32(6).ToString();    // idequipe
                            unResultat[7] = unReader.GetString(7);              // nom_equipe

                            lesResultats.Add(unResultat);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Aucun joueur avec équipe à extraire");
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return lesResultats;
        }

        // ----- Récupérer toutes les participations avec les détails tournoi et équipe -----
        // Double jointure pour avoir les infos complètes
        public List<string[]> SelectParticipationsDetaillees()
        {
            List<string[]> lesResultats = new List<string[]>();

            // Requête avec double INNER JOIN (participation -> tournoi et participation -> equipe)
            string requete = "SELECT p.idparticipation, p.date_inscription, p.statut, " +
                            "t.idtournoi, t.designation, t.date_tournoi, " +
                            "e.idequipe, e.nom_equipe " +
                            "FROM participation p " +
                            "INNER JOIN tournoi t ON p.idtournoi = t.idtournoi " +
                            "INNER JOIN equipe e ON p.idequipe = e.idequipe;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        while (unReader.Read())
                        {
                            string[] unResultat = new string[8];
                            unResultat[0] = unReader.GetInt32(0).ToString();    // idparticipation
                            unResultat[1] = unReader.GetDateTime(1).ToShortDateString(); // date_inscription
                            unResultat[2] = unReader.GetString(2);              // statut
                            unResultat[3] = unReader.GetInt32(3).ToString();    // idtournoi
                            unResultat[4] = unReader.GetString(4);              // designation (nom tournoi)
                            unResultat[5] = unReader.GetDateTime(5).ToShortDateString(); // date_tournoi
                            unResultat[6] = unReader.GetInt32(6).ToString();    // idequipe
                            unResultat[7] = unReader.GetString(7);              // nom_equipe

                            lesResultats.Add(unResultat);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Aucune participation détaillée à extraire");
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return lesResultats;
        }

        // ----- Récupérer les joueurs d'une équipe spécifique -----
        // Paramètre : l'id de l'équipe dont on veut les joueurs
        public List<Joueur> SelectJoueursByEquipe(int idequipe)
        {
            List<Joueur> lesJoueurs = new List<Joueur>();

            // Requête pour filtrer les joueurs par équipe
            string requete = "SELECT * FROM joueur WHERE idequipe = @idequipe;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@idequipe", idequipe);

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        while (unReader.Read())
                        {
                            Joueur unJoueur = new Joueur(
                                unReader.GetInt32(0),
                                unReader.GetString(1),
                                unReader.GetString(2),
                                unReader.GetInt32(3),
                                unReader.GetString(4),
                                unReader.GetString(5),
                                unReader.IsDBNull(6) ? null : unReader.GetInt32(6)  // idequipe (peut être null)
                            );
                            lesJoueurs.Add(unJoueur);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Aucun joueur trouvé pour l'équipe : " + idequipe);
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return lesJoueurs;
        }

        // ----- Récupérer les équipes inscrites à un tournoi spécifique -----
        public List<string[]> SelectEquipesByTournoi(int idtournoi)
        {
            List<string[]> lesResultats = new List<string[]>();

            // Jointure pour récupérer les équipes via la table participation
            string requete = "SELECT e.idequipe, e.nom_equipe, e.nb_joueur, p.date_inscription, p.statut " +
                            "FROM equipe e " +
                            "INNER JOIN participation p ON e.idequipe = p.idequipe " +
                            "WHERE p.idtournoi = @idtournoi;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@idtournoi", idtournoi);

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        while (unReader.Read())
                        {
                            string[] unResultat = new string[5];
                            unResultat[0] = unReader.GetInt32(0).ToString();    // idequipe
                            unResultat[1] = unReader.GetString(1);              // nom_equipe
                            unResultat[2] = unReader.GetInt32(2).ToString();    // nb_joueur
                            unResultat[3] = unReader.GetDateTime(3).ToShortDateString(); // date_inscription
                            unResultat[4] = unReader.GetString(4);              // statut

                            lesResultats.Add(unResultat);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Aucune équipe trouvée pour le tournoi : " + idtournoi);
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return lesResultats;
        }

        // ========================================================
        // ============= MÉTHODES DE COMPTAGE =====================
        // ========================================================

        // ----- Compter le nombre total d'équipes -----
        public int CountEquipes()
        {
            int nombre = 0;
            // COUNT(*) retourne le nombre de lignes dans la table
            string requete = "SELECT COUNT(*) FROM equipe;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                // ExecuteScalar() retourne la première colonne de la première ligne
                // Idéal pour les requêtes qui retournent une seule valeur
                nombre = Convert.ToInt32(uneCmde.ExecuteScalar());

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return nombre;
        }

        // ----- Compter le nombre total de joueurs -----
        public int CountJoueurs()
        {
            int nombre = 0;
            string requete = "SELECT COUNT(*) FROM joueur;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                nombre = Convert.ToInt32(uneCmde.ExecuteScalar());

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return nombre;
        }

        // ----- Compter le nombre total de tournois -----
        public int CountTournois()
        {
            int nombre = 0;
            string requete = "SELECT COUNT(*) FROM tournoi;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                nombre = Convert.ToInt32(uneCmde.ExecuteScalar());

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return nombre;
        }

        // ----- Compter le nombre total de participations -----
        public int CountParticipations()
        {
            int nombre = 0;
            string requete = "SELECT COUNT(*) FROM participation;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                nombre = Convert.ToInt32(uneCmde.ExecuteScalar());

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return nombre;
        }

        // ----- Compter le nombre de joueurs dans une équipe spécifique -----
        // Paramètre : l'id de l'équipe
        public int CountJoueursByEquipe(int idequipe)
        {
            int nombre = 0;
            // COUNT avec WHERE pour filtrer par équipe
            string requete = "SELECT COUNT(*) FROM joueur WHERE idequipe = @idequipe;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@idequipe", idequipe);

                nombre = Convert.ToInt32(uneCmde.ExecuteScalar());

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return nombre;
        }

        // ----- Compter le nombre d'équipes inscrites à un tournoi -----
        public int CountEquipesByTournoi(int idtournoi)
        {
            int nombre = 0;
            string requete = "SELECT COUNT(*) FROM participation WHERE idtournoi = @idtournoi;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@idtournoi", idtournoi);

                nombre = Convert.ToInt32(uneCmde.ExecuteScalar());

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return nombre;
        }

        // ----- Compter les participations par statut -----
        // Paramètre : le statut ("en attente", "confirmee", "annulee")
        public int CountParticipationsByStatut(string statut)
        {
            int nombre = 0;
            string requete = "SELECT COUNT(*) FROM participation WHERE statut = @statut;";

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@statut", statut);

                nombre = Convert.ToInt32(uneCmde.ExecuteScalar());

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return nombre;
        }

        // ========================================================
        // ================ MÉTHODES POUR ADMIN ===================
        // ========================================================

        // ----- INSERT : Ajouter un nouvel administrateur -----
        // Paramètre : objet Admin contenant les données à insérer
        public void InsertAdmin(Admin unAdmin)
        {
            // Requête SQL d'insertion avec des paramètres nommés
            // "null" pour l'id car il est auto-incrémenté par MySQL
            // La date_creation a une valeur par défaut dans MySQL
            string requete = "INSERT INTO admin (identifiant, mot_de_passe, nom_admin) VALUES (@identifiant, @mdp, @nom);";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                // Liaison des paramètres avec les valeurs de l'objet Admin
                uneCmde.Parameters.AddWithValue("@identifiant", unAdmin.Identifiant);
                uneCmde.Parameters.AddWithValue("@mdp", unAdmin.Mot_de_passe);
                uneCmde.Parameters.AddWithValue("@nom", unAdmin.Nom_admin);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete + " - " + exp.Message);
                throw; // Propager l'erreur pour la gérer dans le contrôleur
            }
        }

        // ----- DELETE : Supprimer un admin par son id -----
        // Paramètre : l'identifiant de l'admin à supprimer
        public void DeleteAdmin(int idadmin)
        {
            string requete = "DELETE FROM admin WHERE idadmin = @id;";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", idadmin);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- UPDATE : Modifier un admin existant -----
        // Paramètre : objet Admin avec les nouvelles valeurs
        public void UpdateAdmin(Admin unAdmin)
        {
            string requete = "UPDATE admin SET identifiant = @identifiant, mot_de_passe = @mdp, " +
                            "nom_admin = @nom WHERE idadmin = @id;";
            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", unAdmin.Idadmin);
                uneCmde.Parameters.AddWithValue("@identifiant", unAdmin.Identifiant);
                uneCmde.Parameters.AddWithValue("@mdp", unAdmin.Mot_de_passe);
                uneCmde.Parameters.AddWithValue("@nom", unAdmin.Nom_admin);

                uneCmde.ExecuteNonQuery();
                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }
        }

        // ----- SELECT ALL : Récupérer tous les admins (avec filtre optionnel) -----
        // Paramètre : chaîne de recherche (vide = tous les admins)
        // Retourne : une liste d'objets Admin
        public List<Admin> SelectAllAdmins(string filtre)
        {
            List<Admin> lesAdmins = new List<Admin>();
            string requete;

            if (String.IsNullOrEmpty(filtre))
            {
                requete = "SELECT * FROM admin;";
            }
            else
            {
                // Recherche sur l'identifiant ou le nom de l'admin
                requete = "SELECT * FROM admin WHERE identifiant LIKE @filtre OR nom_admin LIKE @filtre;";
            }

            MySqlCommand uneCmde = null;
            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                if (!String.IsNullOrEmpty(filtre))
                {
                    uneCmde.Parameters.AddWithValue("@filtre", "%" + filtre + "%");
                }

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        while (unReader.Read())
                        {
                            // Création d'un objet Admin avec les données de la ligne
                            Admin unAdmin = new Admin(
                                unReader.GetInt32(0),      // idadmin
                                unReader.GetString(1),     // identifiant
                                unReader.GetString(2),     // mot_de_passe
                                unReader.GetString(3),     // nom_admin
                                unReader.GetDateTime(4)    // date_creation
                            );
                            lesAdmins.Add(unAdmin);
                        }
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Aucun admin à extraire");
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return lesAdmins;
        }

        // ----- SELECT BY ID : Récupérer un admin par son id -----
        // Paramètre : l'identifiant de l'admin recherché
        // Retourne : l'objet Admin correspondant ou null si non trouvé
        public Admin SelectAdminById(int idadmin)
        {
            Admin unAdmin = null;
            string requete = "SELECT * FROM admin WHERE idadmin = @id;";
            MySqlCommand uneCmde = null;

            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@id", idadmin);

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    if (unReader.HasRows)
                    {
                        unReader.Read();
                        unAdmin = new Admin(
                            unReader.GetInt32(0),      // idadmin
                            unReader.GetString(1),     // identifiant
                            unReader.GetString(2),     // mot_de_passe
                            unReader.GetString(3),     // nom_admin
                            unReader.GetDateTime(4)    // date_creation
                        );
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Admin non trouvé avec l'id : " + idadmin);
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            return unAdmin;
        }

        // ----- AUTHENTIFIER : Vérifier les identifiants de connexion -----
        // Paramètres : identifiant et mot de passe saisis par l'utilisateur
        // Retourne : l'objet Admin si authentification réussie, null sinon
        public Admin Authentifier(string identifiant, string motDePasse)
        {
            Admin unAdmin = null;
            // Requête pour vérifier si l'identifiant ET le mot de passe correspondent
            string requete = "SELECT * FROM admin WHERE identifiant = @identifiant AND mot_de_passe = @mdp;";
            MySqlCommand uneCmde = null;

            try
            {
                this.maConnexion.Open();
                uneCmde = this.maConnexion.CreateCommand();
                uneCmde.CommandText = requete;

                uneCmde.Parameters.AddWithValue("@identifiant", identifiant);
                uneCmde.Parameters.AddWithValue("@mdp", motDePasse);

                DbDataReader unReader = uneCmde.ExecuteReader();

                try
                {
                    // Si un résultat est trouvé, les identifiants sont corrects
                    if (unReader.HasRows)
                    {
                        unReader.Read();
                        unAdmin = new Admin(
                            unReader.GetInt32(0),      // idadmin
                            unReader.GetString(1),     // identifiant
                            unReader.GetString(2),     // mot_de_passe
                            unReader.GetString(3),     // nom_admin
                            unReader.GetDateTime(4)    // date_creation
                        );
                    }
                }
                catch (Exception exp)
                {
                    Debug.WriteLine("Erreur lors de l'authentification");
                }

                this.maConnexion.Close();
            }
            catch (Exception exp)
            {
                Debug.WriteLine("Erreur execution : " + requete);
            }

            // Retourne l'admin si trouvé, null si identifiants incorrects
            return unAdmin;
        }
    }
}
