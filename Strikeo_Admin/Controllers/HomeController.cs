using Microsoft.AspNetCore.Mvc;

namespace Strikeo_Admin.Controllers
{
    public class HomeController : Controller
    {
        // Connexion BDD
        private readonly string serveur = "127.0.0.1";
        private readonly string bdd = "strikeo_admin";
        private readonly string user = "root";
        private readonly string mdp = "";

        // ===== GET : Dashboard =====
        public IActionResult Index()
        {
            // Vérifier si connecté
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Récupérer les statistiques
            Modele monModele = new Modele(serveur, bdd, user, mdp);

            ViewBag.NbEquipes = monModele.SelectAllEquipes("").Count;
            ViewBag.NbJoueurs = monModele.SelectAllJoueurs("").Count;
            ViewBag.NbTournois = monModele.SelectAllTournois("").Count;
            ViewBag.NbParticipations = monModele.SelectAllParticipations("").Count;

            return View();
        }

        // ===== GET : Page d'erreur =====
        public IActionResult Error()
        {
            return View();
        }
    }
}
