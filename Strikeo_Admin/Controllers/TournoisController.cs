using Microsoft.AspNetCore.Mvc;

namespace Strikeo_Admin.Controllers
{
    public class TournoisController : Controller
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

        // ===== GET : Liste des tournois =====
        public IActionResult Index(string filtre = "")
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            ViewBag.LesTournois = monModele.SelectAllTournois(filtre);
            ViewBag.Filtre = filtre;

            return View();
        }

        // ===== GET : Formulaire d'ajout =====
        [HttpGet]
        public IActionResult Ajouter()
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");
            return View();
        }

        // ===== POST : Traiter l'ajout =====
        [HttpPost]
        public IActionResult Ajouter(string designation, DateTime dateTournoi, string description)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Tournoi nouveauTournoi = new Tournoi(designation, dateTournoi, description);

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.InsertTournoi(nouveauTournoi);

            return RedirectToAction("Index");
        }

        // ===== GET : Formulaire de modification =====
        [HttpGet]
        public IActionResult Modifier(int id)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            Tournoi tournoi = monModele.SelectTournoiById(id);

            if (tournoi == null) return RedirectToAction("Index");

            ViewBag.Tournoi = tournoi;
            return View();
        }

        // ===== POST : Traiter la modification =====
        [HttpPost]
        public IActionResult Modifier(int id, string designation, DateTime dateTournoi, string description)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Tournoi tournoiModifie = new Tournoi(id, designation, dateTournoi, description);

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.UpdateTournoi(tournoiModifie);

            return RedirectToAction("Index");
        }

        // ===== GET : Supprimer =====
        public IActionResult Supprimer(int id)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.DeleteTournoi(id);

            return RedirectToAction("Index");
        }
    }
}
