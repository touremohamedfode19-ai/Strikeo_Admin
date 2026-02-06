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
            var lesEquipes = monModele.SelectAllEquipes("");
            ViewBag.LesEquipes = lesEquipes;

            // Calculer les places disponibles par équipe
            Dictionary<int, int> placesDisponibles = new Dictionary<int, int>();
            foreach (var equipe in lesEquipes)
            {
                int joueursActuels = monModele.CountJoueursByEquipe(equipe.Idequipe);
                placesDisponibles[equipe.Idequipe] = equipe.Nb_joueur - joueursActuels;
            }
            ViewBag.PlacesDisponibles = placesDisponibles;

            return View();
        }

        // ===== POST : Traiter l'ajout =====
        [HttpPost]
        public IActionResult Ajouter(string nom, string prenom, int age, string mail, string telephone, int? idEquipe)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);

            // Vérifier si l'équipe est complète (seulement si une équipe est sélectionnée)
            if (idEquipe.HasValue)
            {
                var equipe = monModele.SelectEquipeById(idEquipe.Value);
                if (equipe != null)
                {
                    int joueursActuels = monModele.CountJoueursByEquipe(idEquipe.Value);
                    if (joueursActuels >= equipe.Nb_joueur)
                    {
                        ViewBag.MessageErreur = "Cette équipe est complète. Impossible d'ajouter de nouveaux joueurs.";
                        
                        var lesEquipes = monModele.SelectAllEquipes("");
                        ViewBag.LesEquipes = lesEquipes;
                        
                        Dictionary<int, int> placesDisponibles = new Dictionary<int, int>();
                        foreach (var eq in lesEquipes)
                        {
                            int joueurs = monModele.CountJoueursByEquipe(eq.Idequipe);
                            placesDisponibles[eq.Idequipe] = eq.Nb_joueur - joueurs;
                        }
                        ViewBag.PlacesDisponibles = placesDisponibles;
                        
                        return View();
                    }
                }
            }

            Joueur nouveauJoueur = new Joueur(nom, prenom, age, mail, telephone, idEquipe);
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
            var lesEquipes = monModele.SelectAllEquipes("");
            ViewBag.LesEquipes = lesEquipes;

            if (joueur == null) return RedirectToAction("Index");

            // Calculer les places disponibles par équipe
            Dictionary<int, int> placesDisponibles = new Dictionary<int, int>();
            foreach (var equipe in lesEquipes)
            {
                int joueursActuels = monModele.CountJoueursByEquipe(equipe.Idequipe);
                // Si c'est l'équipe actuelle du joueur, on compte une place de plus (lui-même)
                if (joueur.Idequipe.HasValue && equipe.Idequipe == joueur.Idequipe.Value)
                {
                    placesDisponibles[equipe.Idequipe] = equipe.Nb_joueur - joueursActuels + 1;
                }
                else
                {
                    placesDisponibles[equipe.Idequipe] = equipe.Nb_joueur - joueursActuels;
                }
            }
            ViewBag.PlacesDisponibles = placesDisponibles;

            ViewBag.Joueur = joueur;
            return View();
        }

        // ===== POST : Traiter la modification =====
        [HttpPost]
        public IActionResult Modifier(int id, string nom, string prenom, int age, string mail, string telephone, int? idEquipe)
        {
            if (!EstConnecte()) return RedirectToAction("Login", "Auth");

            Modele monModele = new Modele(serveur, bdd, user, mdp);
            
            // Récupérer le joueur actuel pour vérifier si l'équipe change
            Joueur joueurActuel = monModele.SelectJoueurById(id);
            
            // Si l'équipe change et qu'une nouvelle équipe est sélectionnée, vérifier si elle a de la place
            if (joueurActuel != null && idEquipe.HasValue && joueurActuel.Idequipe != idEquipe)
            {
                var equipe = monModele.SelectEquipeById(idEquipe.Value);
                if (equipe != null)
                {
                    int joueursActuels = monModele.CountJoueursByEquipe(idEquipe.Value);
                    if (joueursActuels >= equipe.Nb_joueur)
                    {
                        ViewBag.MessageErreur = "Cette équipe est complète. Impossible de transférer le joueur.";
                        
                        var lesEquipes = monModele.SelectAllEquipes("");
                        ViewBag.LesEquipes = lesEquipes;
                        
                        Dictionary<int, int> placesDisponibles = new Dictionary<int, int>();
                        foreach (var eq in lesEquipes)
                        {
                            int joueurs = monModele.CountJoueursByEquipe(eq.Idequipe);
                            if (joueurActuel.Idequipe.HasValue && eq.Idequipe == joueurActuel.Idequipe.Value)
                            {
                                placesDisponibles[eq.Idequipe] = eq.Nb_joueur - joueurs + 1;
                            }
                            else
                            {
                                placesDisponibles[eq.Idequipe] = eq.Nb_joueur - joueurs;
                            }
                        }
                        ViewBag.PlacesDisponibles = placesDisponibles;
                        ViewBag.Joueur = joueurActuel;
                        
                        return View();
                    }
                }
            }

            Joueur joueurModifie = new Joueur(id, nom, prenom, age, mail, telephone, idEquipe);
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
