using Microsoft.AspNetCore.Mvc;

namespace Strikeo_Admin.Controllers
{
    public class ParticipationsController : Controller
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

        // ===== GET : Liste des participations =====
        public IActionResult Index(string filtre = "")
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            ViewBag.LesParticipations = monModele.SelectAllParticipations(filtre);
            ViewBag.Filtre = filtre;

            return View();
        }

        // ===== GET : Formulaire d'ajout =====
        [HttpGet]
        public IActionResult Ajouter()
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            ViewBag.LesTournois = monModele.SelectAllTournois("");
            ViewBag.LesEquipes = monModele.SelectAllEquipes("");

            return View();
        }

        // ===== POST : Traiter l'ajout =====
        [HttpPost]
        public IActionResult Ajouter(DateTime dateInscription, string statut, int idTournoi, int idEquipe)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Participation nouvelleParticipation = new Participation(dateInscription, statut, idTournoi, idEquipe);

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.InsertParticipation(nouvelleParticipation);

            return RedirectToAction("Index");
        }

        // ===== GET : Formulaire de modification =====
        [HttpGet]
        public IActionResult Modifier(int id)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            Participation participation = monModele.SelectParticipationById(id);
            ViewBag.LesTournois = monModele.SelectAllTournois("");
            ViewBag.LesEquipes = monModele.SelectAllEquipes("");

            if (participation == null) return RedirectToAction("Index");

            ViewBag.Participation = participation;
            return View();
        }

        // ===== POST : Traiter la modification =====
        [HttpPost]
        public IActionResult Modifier(int id, DateTime dateInscription, string statut, int idTournoi, int idEquipe)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Participation participationModifiee = new Participation(id, dateInscription, statut, idTournoi, idEquipe);

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.UpdateParticipation(participationModifiee);

            return RedirectToAction("Index");
        }

        // ===== GET : Supprimer =====
        public IActionResult Supprimer(int id)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.DeleteParticipation(id);

            return RedirectToAction("Index");
        }
    }
}
