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

        // ===== GET : Afficher le formulaire de modification du profil =====
        [HttpGet]
        public IActionResult ModifierProfil()
        {
            int? adminId = HttpContext.Session.GetInt32("AdminId");
            
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                Modele monModele = new Modele(serveur, bdd, user, mdp);
                Admin admin = monModele.SelectAdminById(adminId.Value);
                
                if (admin == null)
                {
                    return RedirectToAction("Login");
                }

                ViewBag.Admin = admin;
                return View();
            }
            catch (Exception ex)
            {
                TempData["MessageErreur"] = "Erreur lors du chargement du profil : " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }

        // ===== POST : Traiter la modification du profil =====
        [HttpPost]
        public IActionResult ModifierProfil(string nomAdmin, string identifiant, string nouveauMotDePasse, string confirmMotDePasse)
        {
            int? adminId = HttpContext.Session.GetInt32("AdminId");
            
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            // Validation des champs obligatoires
            if (string.IsNullOrEmpty(nomAdmin) || string.IsNullOrEmpty(identifiant))
            {
                ViewBag.MessageErreur = "Le nom et l'identifiant sont obligatoires.";
                return RechargerProfil(adminId.Value);
            }

            // Si un nouveau mot de passe est fourni, vérifier la confirmation
            if (!string.IsNullOrEmpty(nouveauMotDePasse))
            {
                if (nouveauMotDePasse != confirmMotDePasse)
                {
                    ViewBag.MessageErreur = "Les mots de passe ne correspondent pas.";
                    return RechargerProfil(adminId.Value);
                }

                if (nouveauMotDePasse.Length < 4)
                {
                    ViewBag.MessageErreur = "Le mot de passe doit contenir au moins 4 caractères.";
                    return RechargerProfil(adminId.Value);
                }
            }

            try
            {
                Modele monModele = new Modele(serveur, bdd, user, mdp);
                Admin admin = monModele.SelectAdminById(adminId.Value);

                if (admin == null)
                {
                    return RedirectToAction("Login");
                }

                // Vérifier si l'identifiant est déjà utilisé par un autre admin
                if (identifiant != admin.Identifiant)
                {
                    List<Admin> admins = monModele.SelectAllAdmins(identifiant);
                    if (admins != null && admins.Any(a => a.Identifiant == identifiant && a.Idadmin != adminId.Value))
                    {
                        ViewBag.MessageErreur = "Cet identifiant est déjà utilisé par un autre compte.";
                        return RechargerProfil(adminId.Value);
                    }
                }

                // Mettre à jour les informations
                admin.Nom_admin = nomAdmin;
                admin.Identifiant = identifiant;
                
                // Mettre à jour le mot de passe seulement si un nouveau est fourni
                if (!string.IsNullOrEmpty(nouveauMotDePasse))
                {
                    admin.Mot_de_passe = nouveauMotDePasse;
                }

                monModele.UpdateAdmin(admin);

                // Mettre à jour la session
                HttpContext.Session.SetString("AdminNom", nomAdmin);
                HttpContext.Session.SetString("AdminIdentifiant", identifiant);

                TempData["MessageSucces"] = "Profil mis à jour avec succès.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.MessageErreur = "Erreur lors de la mise à jour : " + ex.Message;
                return RechargerProfil(adminId.Value);
            }
        }

        // Méthode helper pour recharger le profil en cas d'erreur
        private IActionResult RechargerProfil(int adminId)
        {
            try
            {
                Modele monModele = new Modele(serveur, bdd, user, mdp);
                Admin admin = monModele.SelectAdminById(adminId);
                ViewBag.Admin = admin;
            }
            catch { }
            return View();
        }

        // ===== POST : Supprimer le compte =====
        [HttpPost]
        public IActionResult DeleteAccount()
        {
            int? adminId = HttpContext.Session.GetInt32("AdminId");
            
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            try
            {
                Modele monModele = new Modele(serveur, bdd, user, mdp);
                monModele.DeleteAdmin(adminId.Value);
                
                HttpContext.Session.Clear();
                
                TempData["MessageSucces"] = "Votre compte a été supprimé avec succès.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                TempData["MessageErreur"] = "Erreur lors de la suppression : " + ex.Message;
                return RedirectToAction("Index", "Home");
            }
        }
    }
}
