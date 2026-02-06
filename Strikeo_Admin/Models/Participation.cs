// Importation de la bibliothèque System qui contient les classes de base .NET
using System;

// Définition du namespace pour organiser le code du projet
namespace Strikeo_Admin
{
    // Déclaration de la classe publique Participation
    // Cette classe représente l'inscription d'une équipe à un tournoi
    public class Participation
    {
        // ===== ATTRIBUTS PRIVÉS =====
        // Ces attributs correspondent aux colonnes de la table "participation" dans la BDD

        // Identifiant unique de la participation (clé primaire auto-incrémentée)
        private int idparticipation;

        // Date à laquelle l'équipe s'est inscrite au tournoi (type DATE dans la BDD)
        private DateTime date_inscription;

        // Statut de la participation : "en attente", "confirmee" ou "annulee"
        // En BDD c'est un ENUM, en C# on utilise un string
        private string statut;

        // Clé étrangère vers la table tournoi - identifie le tournoi concerné
        private int idtournoi;

        // Clé étrangère vers la table equipe - identifie l'équipe qui participe
        private int idequipe;

        // ===== PROPRIÉTÉS PUBLIQUES (GETTERS/SETTERS) =====
        // Permettent d'accéder et modifier les attributs privés de manière contrôlée

        // Propriété pour l'identifiant de la participation
        public int Idparticipation
        {
            get { return idparticipation; }
            set { idparticipation = value; }
        }

        // Propriété pour la date d'inscription
        public DateTime Date_inscription
        {
            get { return date_inscription; }
            set { date_inscription = value; }
        }

        // Propriété pour le statut de la participation
        // Les valeurs possibles sont : "en attente", "confirmee", "annulee"
        public string Statut
        {
            get { return statut; }
            set { statut = value; }
        }

        // Propriété pour la clé étrangère vers le tournoi
        public int Idtournoi
        {
            get { return idtournoi; }
            set { idtournoi = value; }
        }

        // Propriété pour la clé étrangère vers l'équipe
        public int Idequipe
        {
            get { return idequipe; }
            set { idequipe = value; }
        }

        // ===== CONSTRUCTEURS =====

        // Constructeur par défaut (sans paramètres)
        // Utilisé quand on crée une participation vide qu'on remplira plus tard
        public Participation()
        {
            // Corps vide - les attributs auront leurs valeurs par défaut
        }

        // Constructeur avec tous les paramètres (incluant l'id)
        // Utilisé quand on récupère une participation depuis la BDD
        public Participation(int idparticipation, DateTime date_inscription,
                            string statut, int idtournoi, int idequipe)
        {
            // Affectation de chaque paramètre à l'attribut correspondant
            this.idparticipation = idparticipation;
            this.date_inscription = date_inscription;
            this.statut = statut;
            this.idtournoi = idtournoi;     // Clé étrangère vers le tournoi
            this.idequipe = idequipe;       // Clé étrangère vers l'équipe
        }

        // Constructeur sans l'id (utile pour l'insertion en BDD)
        // L'id sera généré automatiquement par MySQL (AUTO_INCREMENT)
        public Participation(DateTime date_inscription, string statut,
                            int idtournoi, int idequipe)
        {
            this.date_inscription = date_inscription;
            this.statut = statut;
            this.idtournoi = idtournoi;
            this.idequipe = idequipe;
        }
    }
}
