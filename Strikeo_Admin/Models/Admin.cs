// Importation de la bibliothèque System qui contient les classes de base .NET
using System;

// Définition du namespace (espace de noms) pour organiser le code du projet
namespace Strikeo_Admin
{
    // Déclaration de la classe publique Admin qui représente un administrateur dans la base de données
    public class Admin
    {
        // ===== ATTRIBUTS PRIVÉS =====
        // Ces attributs correspondent aux colonnes de la table "admin" dans la BDD

        // Identifiant unique de l'admin (clé primaire auto-incrémentée)
        private int idadmin;

        // Identifiant de connexion (varchar 50, unique dans la BDD)
        private string identifiant;

        // Mot de passe de l'admin (varchar 255 dans la BDD)
        private string mot_de_passe;

        // Nom de l'administrateur (varchar 100 dans la BDD)
        private string nom_admin;

        // Date de création du compte admin
        private DateTime date_creation;

        // ===== PROPRIÉTÉS PUBLIQUES (GETTERS/SETTERS) =====
        // Permettent d'accéder et modifier les attributs privés de manière contrôlée

        // Propriété pour l'identifiant unique de l'admin
        public int Idadmin
        {
            // Getter : retourne la valeur de l'attribut privé idadmin
            get { return idadmin; }
            // Setter : permet de modifier la valeur de idadmin
            set { idadmin = value; }
        }

        // Propriété pour l'identifiant de connexion
        public string Identifiant
        {
            // Getter : retourne l'identifiant de connexion
            get { return identifiant; }
            // Setter : permet de modifier l'identifiant
            set { identifiant = value; }
        }

        // Propriété pour le mot de passe
        public string Mot_de_passe
        {
            // Getter : retourne le mot de passe
            get { return mot_de_passe; }
            // Setter : permet de modifier le mot de passe
            set { mot_de_passe = value; }
        }

        // Propriété pour le nom de l'administrateur
        public string Nom_admin
        {
            // Getter : retourne le nom de l'admin
            get { return nom_admin; }
            // Setter : permet de modifier le nom de l'admin
            set { nom_admin = value; }
        }

        // Propriété pour la date de création du compte
        public DateTime Date_creation
        {
            // Getter : retourne la date de création
            get { return date_creation; }
            // Setter : permet de modifier la date de création
            set { date_creation = value; }
        }

        // ===== CONSTRUCTEURS =====

        // Constructeur par défaut (sans paramètres)
        // Utilisé quand on crée un admin vide qu'on remplira plus tard
        public Admin()
        {
            // Corps vide - les attributs auront leurs valeurs par défaut
        }

        // Constructeur avec tous les paramètres
        // Utilisé quand on récupère un admin depuis la base de données
        public Admin(int idadmin, string identifiant, string mot_de_passe, string nom_admin, DateTime date_creation)
        {
            // Affectation des paramètres aux attributs de l'objet
            this.idadmin = idadmin;             // "this" fait référence à l'instance actuelle
            this.identifiant = identifiant;
            this.mot_de_passe = mot_de_passe;
            this.nom_admin = nom_admin;
            this.date_creation = date_creation;
        }

        // Constructeur sans l'id ni la date (utile pour l'insertion en BDD)
        // L'id est auto-généré et la date a une valeur par défaut dans MySQL
        public Admin(string identifiant, string mot_de_passe, string nom_admin)
        {
            // On n'a pas besoin de l'id car MySQL le génère automatiquement
            this.identifiant = identifiant;
            this.mot_de_passe = mot_de_passe;
            this.nom_admin = nom_admin;
        }
    }
}
