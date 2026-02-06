// Importation de la bibliothèque System qui contient les classes de base .NET
using System;

// Définition du namespace pour organiser le code du projet
namespace Strikeo_Admin
{
    // Déclaration de la classe publique Tournoi qui représente un tournoi dans la base de données
    public class Tournoi
    {
        // ===== ATTRIBUTS PRIVÉS =====
        // Ces attributs correspondent aux colonnes de la table "tournoi" dans la BDD

        // Identifiant unique du tournoi (clé primaire auto-incrémentée)
        private int idtournoi;

        // Nom/désignation du tournoi (varchar 100 dans la BDD)
        private string designation;

        // Date à laquelle le tournoi a lieu (type DATE dans la BDD)
        private DateTime date_tournoi;

        // Description détaillée du tournoi (type TEXT dans la BDD)
        private string description;

        // ===== PROPRIÉTÉS PUBLIQUES (GETTERS/SETTERS) =====
        // Permettent d'accéder et modifier les attributs privés de manière contrôlée

        // Propriété pour l'identifiant du tournoi
        public int Idtournoi
        {
            get { return idtournoi; }
            set { idtournoi = value; }
        }

        // Propriété pour la désignation du tournoi
        public string Designation
        {
            get { return designation; }
            set { designation = value; }
        }

        // Propriété pour la date du tournoi
        // On utilise DateTime en C# pour représenter le type DATE de MySQL
        public DateTime Date_tournoi
        {
            get { return date_tournoi; }
            set { date_tournoi = value; }
        }

        // Propriété pour la description du tournoi
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        // ===== CONSTRUCTEURS =====

        // Constructeur par défaut (sans paramètres)
        // Utilisé quand on crée un tournoi vide qu'on remplira plus tard
        public Tournoi()
        {
            // Corps vide - les attributs auront leurs valeurs par défaut
        }

        // Constructeur avec tous les paramètres (incluant l'id)
        // Utilisé quand on récupère un tournoi depuis la BDD
        public Tournoi(int idtournoi, string designation, DateTime date_tournoi, string description)
        {
            // Affectation de chaque paramètre à l'attribut correspondant
            this.idtournoi = idtournoi;
            this.designation = designation;
            this.date_tournoi = date_tournoi;
            this.description = description;
        }

        // Constructeur sans l'id (utile pour l'insertion en BDD)
        // L'id sera généré automatiquement par MySQL (AUTO_INCREMENT)
        public Tournoi(string designation, DateTime date_tournoi, string description)
        {
            this.designation = designation;
            this.date_tournoi = date_tournoi;
            this.description = description;
        }
    }
}
