using Microsoft.AspNetCore.Mvc;

namespace Strikeo_Admin.Controllers
{
    public class EquipesController : Controller
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

        // ===== GET : Liste des équipes =====
        public IActionResult Index(string filtre = "")
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            ViewBag.LesEquipes = monModele.SelectAllEquipes(filtre);
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
        public IActionResult Ajouter(string nomEquipe, int nbJoueur)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Equipe nouvelleEquipe = new Equipe(nomEquipe, nbJoueur);

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.InsertEquipe(nouvelleEquipe);

            return RedirectToAction("Index");
        }

        // ===== GET : Formulaire de modification =====
        [HttpGet]
        public IActionResult Modifier(int id)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            Equipe equipe = monModele.SelectEquipeById(id);

            if (equipe == null) return RedirectToAction("Index");

            ViewBag.Equipe = equipe;
            return View();
        }

        // ===== POST : Traiter la modification =====
        [HttpPost]
        public IActionResult Modifier(int id, string nomEquipe, int nbJoueur)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Equipe equipeModifiee = new Equipe(id, nomEquipe, nbJoueur);

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.UpdateEquipe(equipeModifiee);

            return RedirectToAction("Index");
        }

        // ===== GET : Supprimer =====
        public IActionResult Supprimer(int id)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            monModele.DeleteEquipe(id);

            return RedirectToAction("Index");
        }
    }
}
