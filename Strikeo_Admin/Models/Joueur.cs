// Importation de la bibliothèque System qui contient les classes de base .NET
using System;

// Définition du namespace pour organiser le code du projet
namespace Strikeo_Admin
{
    // Déclaration de la classe publique Joueur qui représente un joueur dans la base de données
    public class Joueur
    {
        // ===== ATTRIBUTS PRIVÉS =====
        // Ces attributs correspondent aux colonnes de la table "joueur" dans la BDD

        // Identifiant unique du joueur (clé primaire auto-incrémentée)
        private int idjoueur;

        // Nom du joueur (varchar 50 dans la BDD)
        private string nom_joueur;

        // Prénom du joueur (varchar 50 dans la BDD)
        private string prenom_joueur;

        // Âge du joueur (entier)
        private int age_joueur;

        // Adresse email du joueur (varchar 50 dans la BDD)
        private string mail_joueur;

        // Numéro de téléphone du joueur (varchar 20 dans la BDD)
        private string telephone;

        // Clé étrangère vers la table equipe - identifie l'équipe du joueur
        private int idequipe;

        // ===== PROPRIÉTÉS PUBLIQUES (GETTERS/SETTERS) =====
        // Permettent d'accéder et modifier les attributs privés de manière contrôlée

        // Propriété pour l'identifiant du joueur
        public int Idjoueur
        {
            get { return idjoueur; }
            set { idjoueur = value; }
        }

        // Propriété pour le nom du joueur
        public string Nom_joueur
        {
            get { return nom_joueur; }
            set { nom_joueur = value; }
        }

        // Propriété pour le prénom du joueur
        public string Prenom_joueur
        {
            get { return prenom_joueur; }
            set { prenom_joueur = value; }
        }

        // Propriété pour l'âge du joueur
        public int Age_joueur
        {
            get { return age_joueur; }
            set { age_joueur = value; }
        }

        // Propriété pour l'email du joueur
        public string Mail_joueur
        {
            get { return mail_joueur; }
            set { mail_joueur = value; }
        }

        // Propriété pour le téléphone du joueur
        public string Telephone
        {
            get { return telephone; }
            set { telephone = value; }
        }

        // Propriété pour la clé étrangère (lien vers l'équipe)
        public int Idequipe
        {
            get { return idequipe; }
            set { idequipe = value; }
        }

        // ===== CONSTRUCTEURS =====

        // Constructeur par défaut (sans paramètres)
        // Utilisé quand on crée un joueur vide qu'on remplira plus tard
        public Joueur()
        {
            // Corps vide - les attributs auront leurs valeurs par défaut
        }

        // Constructeur avec tous les paramètres (incluant l'id)
        // Utilisé quand on récupère un joueur depuis la BDD
        public Joueur(int idjoueur, string nom_joueur, string prenom_joueur,
                      int age_joueur, string mail_joueur, string telephone, int idequipe)
        {
            // Affectation de chaque paramètre à l'attribut correspondant
            this.idjoueur = idjoueur;
            this.nom_joueur = nom_joueur;
            this.prenom_joueur = prenom_joueur;
            this.age_joueur = age_joueur;
            this.mail_joueur = mail_joueur;
            this.telephone = telephone;
            this.idequipe = idequipe;       // Clé étrangère vers l'équipe
        }

        // Constructeur sans l'id (utile pour l'insertion en BDD)
        // L'id sera généré automatiquement par MySQL (AUTO_INCREMENT)
        public Joueur(string nom_joueur, string prenom_joueur,
                      int age_joueur, string mail_joueur, string telephone, int idequipe)
        {
            this.nom_joueur = nom_joueur;
            this.prenom_joueur = prenom_joueur;
            this.age_joueur = age_joueur;
            this.mail_joueur = mail_joueur;
            this.telephone = telephone;
            this.idequipe = idequipe;
        }
    }
}
