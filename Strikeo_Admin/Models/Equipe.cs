// Importation de la bibliothèque System qui contient les classes de base .NET
using System;

// Définition du namespace (espace de noms) pour organiser le code du projet
namespace Strikeo_Admin
{
    // Déclaration de la classe publique Equipe qui représente une équipe dans la base de données
    public class Equipe
    {
        // ===== ATTRIBUTS PRIVÉS =====
        // Ces attributs correspondent aux colonnes de la table "equipe" dans la BDD

        // Identifiant unique de l'équipe (clé primaire auto-incrémentée)
        private int idequipe;

        // Nom de l'équipe (varchar 50 dans la BDD)
        private string nom_equipe;

        // Nombre de joueurs dans l'équipe
        private int nb_joueur;

        // ===== PROPRIÉTÉS PUBLIQUES (GETTERS/SETTERS) =====
        // Permettent d'accéder et modifier les attributs privés de manière contrôlée

        // Propriété pour l'identifiant de l'équipe
        public int Idequipe
        {
            // Getter : retourne la valeur de l'attribut privé idequipe
            get { return idequipe; }
            // Setter : permet de modifier la valeur de idequipe
            set { idequipe = value; }
        }

        // Propriété pour le nom de l'équipe
        public string Nom_equipe
        {
            // Getter : retourne le nom de l'équipe
            get { return nom_equipe; }
            // Setter : permet de modifier le nom de l'équipe
            set { nom_equipe = value; }
        }

        // Propriété pour le nombre de joueurs
        public int Nb_joueur
        {
            // Getter : retourne le nombre de joueurs
            get { return nb_joueur; }
            // Setter : permet de modifier le nombre de joueurs
            set { nb_joueur = value; }
        }

        // ===== CONSTRUCTEURS =====

        // Constructeur par défaut (sans paramètres)
        // Utilisé quand on crée une équipe vide qu'on remplira plus tard
        public Equipe()
        {
            // Corps vide - les attributs auront leurs valeurs par défaut
        }

        // Constructeur avec tous les paramètres
        // Utilisé quand on veut créer une équipe avec toutes ses informations
        public Equipe(int idequipe, string nom_equipe, int nb_joueur)
        {
            // Affectation des paramètres aux attributs de l'objet
            this.idequipe = idequipe;       // "this" fait référence à l'instance actuelle
            this.nom_equipe = nom_equipe;
            this.nb_joueur = nb_joueur;
        }

        // Constructeur sans l'id (utile pour l'insertion en BDD car l'id est auto-généré)
        public Equipe(string nom_equipe, int nb_joueur)
        {
            // On n'a pas besoin de l'id car MySQL le génère automatiquement
            this.nom_equipe = nom_equipe;
            this.nb_joueur = nb_joueur;
        }
    }
}
