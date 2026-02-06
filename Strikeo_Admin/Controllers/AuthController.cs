using Microsoft.AspNetCore.Mvc;

namespace Strikeo_Admin.Controllers
{
    public class AuthController : Controller
    {
        // Connexion BDD
        private readonly string serveur = "127.0.0.1";
        private readonly string bdd = "strikeo_admin";
        private readonly string user = "root";
        private readonly string mdp = "";

        // ===== GET : Afficher la page de connexion =====
        [HttpGet]
        public IActionResult Login()
        {
            // Si déjà connecté, rediriger vers le dashboard
            if (HttpContext.Session.GetInt32("AdminId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // ===== POST : Traiter la connexion =====
        [HttpPost]
        public IActionResult Login(string identifiant, string motDePasse)
        {
            if (string.IsNullOrEmpty(identifiant) || string.IsNullOrEmpty(motDePasse))
            {
                ViewBag.MessageErreur = "Veuillez remplir tous les champs.";
                return View();
            }

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            Admin admin = monModele.Authentifier(identifiant, motDePasse);

            if (admin != null)
            {
                HttpContext.Session.SetInt32("AdminId", admin.Idadmin);
                HttpContext.Session.SetString("AdminNom", admin.Nom_admin ?? "Admin");
                HttpContext.Session.SetString("AdminIdentifiant", admin.Identifiant);

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.MessageErreur = "Identifiant ou mot de passe incorrect.";
                ViewBag.Identifiant = identifiant;
                return View();
            }
        }

        // ===== POST : Traiter l'inscription =====
        [HttpPost]
        public IActionResult Signup(string nomAdmin, string newIdentifiant, string newMotDePasse, string confirmMotDePasse)
        {
            // Validation des champs
            if (string.IsNullOrEmpty(nomAdmin) || string.IsNullOrEmpty(newIdentifiant) || 
                string.IsNullOrEmpty(newMotDePasse) || string.IsNullOrEmpty(confirmMotDePasse))
            {
                ViewBag.MessageErreur = "Veuillez remplir tous les champs.";
                ViewBag.ShowSignupTab = true;
                return View("Login");
            }

            // Vérifier que les mots de passe correspondent
            if (newMotDePasse != confirmMotDePasse)
            {
                ViewBag.MessageErreur = "Les mots de passe ne correspondent pas.";
                ViewBag.ShowSignupTab = true;
                return View("Login");
            }

            // Vérifier la longueur du mot de passe
            if (newMotDePasse.Length < 4)
            {
                ViewBag.MessageErreur = "Le mot de passe doit contenir au moins 4 caractères.";
                ViewBag.ShowSignupTab = true;
                return View("Login");
            }

            try
            {
                Modele monModele = new Modele(serveur, bdd, user, mdp);

                // Vérifier si l'identifiant existe déjà
                List<Admin> admins = monModele.SelectAllAdmins(newIdentifiant);
                if (admins != null && admins.Any(a => a.Identifiant == newIdentifiant))
                {
                    ViewBag.MessageErreur = "Cet identifiant est déjà utilisé.";
                    ViewBag.ShowSignupTab = true;
                    return View("Login");
                }

                // Créer le nouvel admin
                Admin nouvelAdmin = new Admin(newIdentifiant, newMotDePasse, nomAdmin);
                monModele.InsertAdmin(nouvelAdmin);
                
                // Succès
                ViewBag.MessageSucces = "Inscription réussie ! Vous pouvez maintenant vous connecter.";
            }
            catch (Exception ex)
            {
                ViewBag.MessageErreur = "Erreur BDD : " + ex.Message + " - Vérifiez que MySQL/XAMPP est lancé et que la base 'strikeo_admin' existe.";
                ViewBag.ShowSignupTab = true;
            }
            
            return View("Login");
        }

        // ===== GET : Déconnexion =====
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
