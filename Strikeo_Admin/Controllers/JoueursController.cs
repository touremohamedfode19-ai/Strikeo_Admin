using Microsoft.AspNetCore.Mvc;

namespace Strikeo_Admin.Controllers
{
    public class JoueursController : Controller
    {
        // Connexion BDD
        private readonly string serveur = "127.0.0.1";
        private readonly string bdd = "strikeo_admin";
        private readonly string user = "root";
        private readonly string mdp = "";

        // Vérification de la session
        private bool EstConnecte()
        {
            return HttpContext.Session.GetInt32("AdminId") != null;
        }

        // ===== GET : Liste des joueurs =====
        public IActionResult Index(string filtre = "")
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            ViewBag.LesJoueurs = monModele.SelectAllJoueurs(filtre);
            ViewBag.Filtre = filtre;

            return View();
        }

        // ===== GET : Formulaire d'ajout =====
        [HttpGet]
        public IActionResult Ajouter()
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            ViewBag.LesEquipes = monModele.SelectAllEquipes("");

            return View();
        }

        // ===== POST : Traiter l'ajout =====
        [HttpPost]
        public IActionResult Ajouter(string nom, string prenom, int age, string mail, string telephone, int idEquipe)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Joueur nouveauJoueur = new Joueur(nom, prenom, age, mail, telephone, idEquipe);

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.InsertJoueur(nouveauJoueur);

            return RedirectToAction("Index");
        }

        // ===== GET : Formulaire de modification =====
        [HttpGet]
        public IActionResult Modifier(int id)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            Joueur joueur = monModele.SelectJoueurById(id);
            ViewBag.LesEquipes = monModele.SelectAllEquipes("");

            if (joueur == null) return RedirectToAction("Index");

            ViewBag.Joueur = joueur;
            return View();
        }

        // ===== POST : Traiter la modification =====
        [HttpPost]
        public IActionResult Modifier(int id, string nom, string prenom, int age, string mail, string telephone, int idEquipe)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Joueur joueurModifie = new Joueur(id, nom, prenom, age, mail, telephone, idEquipe);

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.UpdateJoueur(joueurModifie);

            return RedirectToAction("Index");
        }

        // ===== GET : Supprimer =====
        public IActionResult Supprimer(int id)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.DeleteJoueur(id);

            return RedirectToAction("Index");
        }
    }
}
