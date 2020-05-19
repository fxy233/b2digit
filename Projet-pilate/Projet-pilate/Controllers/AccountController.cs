using System;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Projet_pilate.Models;

namespace Projet_pilate.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager )
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set 
            { 
                _signInManager = value; 
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Ceci ne comptabilise pas les échecs de connexion pour le verrouillage du compte
            // Pour que les échecs de mot de passe déclenchent le verrouillage du compte, utilisez shouldLockout: true
            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Tentative de connexion non valide.");
                    return View(model);
            }
        }

        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Nécessiter que l'utilisateur soit déjà connecté via un nom d'utilisateur/mot de passe ou une connexte externe
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Le code suivant protège des attaques par force brute contre les codes à 2 facteurs. 
            // Si un utilisateur entre des codes incorrects pendant un certain intervalle, le compte de cet utilisateur 
            // est alors verrouillé pendant une durée spécifiée. 
            // Vous pouvez configurer les paramètres de verrouillage du compte dans IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent:  model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Code non valide.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        [Route("Account/Register", Name = "Register")]
        public ActionResult Register()
        {
            return View();
        }


        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("Account/Register")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
        

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Position = Request.Form["Position"].ToString()
                };


                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var adminId = User.Identity.GetUserId();

                    await SignInManager.SignInAsync(user, isPersistent:false, rememberBrowser:false);

                    // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                    // Envoyer un message électronique avec ce lien
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirmez votre compte", "Confirmez votre compte en cliquant <a href=\"" + callbackUrl + "\">ici</a>");

                    //Temp code conditionné
                    var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    if (user.Position == "Administrateur")
                    {
                        await roleManager.CreateAsync(new IdentityRole("Administrateur"));
                        await UserManager.AddToRoleAsync(user.Id, "Administrateur");
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    }
                    //else if (user.Position == "Super-administrateur")
                    //{
                    //    await roleManager.CreateAsync(new IdentityRole("Super-administrateur"));
                    //    await UserManager.AddToRoleAsync(user.Id, "Super-administrateur");
                    //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    //}
                    //else if (user.Position == "Manager")
                    //{
                    //    await roleManager.CreateAsync(new IdentityRole("Manager"));
                    //    await UserManager.AddToRoleAsync(user.Id, "Manager");
                    //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    //}
                    else if (user.Position == "Administrateur-ventes")
                    {
                        await roleManager.CreateAsync(new IdentityRole("Administrateur-ventes"));
                        await UserManager.AddToRoleAsync(user.Id, "Administrateur-ventes");
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    }
                    //else if (user.Position == "Consultant-Manager")
                    //{
                    //    await roleManager.CreateAsync(new IdentityRole("Consultant-Manager"));
                    //    await UserManager.AddToRoleAsync(user.Id, "Consultant-Manager");
                    //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    //}
                    //else
                    //{
                    //    await roleManager.CreateAsync(new IdentityRole("Consultant"));
                    //    await UserManager.AddToRoleAsync(user.Id, "Consultant");
                    //    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    //}


                    var admin = await UserManager.FindByIdAsync(adminId);
                    await SignInManager.SignInAsync(admin, isPersistent: false, rememberBrowser: false);

              
                    return RedirectToAction("UsersBoard", "Account");
                }
                AddErrors(result);
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);
                //if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                if (user == null)
                {
                    // Ne révélez pas que l'utilisateur n'existe pas ou qu'il n'est pas confirmé
                    return View("ForgotPasswordConfirmation");
                }

                // Pour plus d'informations sur l'activation de la confirmation de compte et de la réinitialisation de mot de passe, visitez https://go.microsoft.com/fwlink/?LinkID=320771
                // Envoyer un message électronique avec ce lien
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                await UserManager.SendEmailAsync(user.Id, "Réinitialiser le mot de passe", "Réinitialisez votre mot de passe en cliquant <a href=\"" + callbackUrl + "\">ici</a>");
                return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Ne révélez pas que l'utilisateur n'existe pas
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Demandez une redirection vers le fournisseur de connexions externe
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Générer le jeton et l'envoyer
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Connecter cet utilisateur à ce fournisseur de connexion externe si l'utilisateur possède déjà une connexion
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // Si l'utilisateur n'a pas de compte, invitez alors celui-ci à créer un compte
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Obtenez des informations sur l’utilisateur auprès du fournisseur de connexions externe
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        // GET: /Account/Index
        [AllowAnonymous]
        [Route("Account/Index", Name = "Index")]
        public async Task<ActionResult> Index()
        {
            var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            return View(user);
        }


        // GET: /Account/Users
        [Authorize(Roles = "Administrateur, Super-Administrateur")]
        [Route("Account/Users", Name = "Users")]
        public ActionResult Users()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var user = User.Identity.Name;

            return View(db.Users.ToList());
        }

        //// GET: /Account/Edit
        [AllowAnonymous]
        public ActionResult Edit(string id)
        {

            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.SingleOrDefault(x => x.Id == id);

            UpdateViewModel model = new UpdateViewModel();
            model.Id = user.Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Position = user.Position;
            model.Email = user.Email;
            model.UserName = user.UserName;

            return View(model);
        }



        //// POST: /Account/Edit
        [HttpPost]
        [AllowAnonymous]
        public ActionResult Edit(UpdateViewModel model)
        {

            ApplicationDbContext db = new ApplicationDbContext();

            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(x => x.Id == model.Id);

                model.Position = Request.Form["Position"].ToString();

                //if (user.Position != model.Position)
                if (user.Position != model.Position)
                {
                    UserManager.RemoveFromRole(user.Id, user.Position);
                    UserManager.AddToRole(user.Id, model.Position);
                }

                if (user != null)
                {
                    user.FirstName = model.FirstName;
                    user.LastName = model.LastName;
                    user.Position = model.Position;
                    user.Email = model.Email;
                    db.SaveChanges();
                }
            }
            else
            {
                return View(model);
            }



            return RedirectToAction("Users", "Account");
        }


        // GET: /Account/Details
        [AllowAnonymous]
        public ActionResult Details(string id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var user = db.Users.SingleOrDefault(x => x.Id == id);

            DetailViewModel model = new DetailViewModel();
            model.Id = user.Id;
            model.FirstName = user.FirstName;
            model.LastName = user.LastName;
            model.Position = user.Position;
            model.Email = user.Email;

            return View(model);
        }

        // GET: /Account/Delete
        [AllowAnonymous]
        public ActionResult Delete(string id)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var user = db.Users.SingleOrDefault(u => u.Id == id);
           
            if (user.Position == "Manager")
            {
                try
                {
                    var manager = db.Managers.Single(m => m.UserID == user.Id);
                    manager.UserID = null;
                }
                catch (Exception){}            

            } else if (user.Position == "Consultant")
            {
                try
                {
                    var consultant = db.Consultants.Single(c => c.UserID == user.Id);
                    consultant.UserID = null;
                }
                catch (Exception){}
               
            }
        
            db.Entry(user).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("UsersBoard", "Account");

        }



        //GET: /Account/CreateUserManager
        [Route("Account/CreateUserManager", Name = "CreateUserManager")]
        public ActionResult CreateUserManager(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var manager = db.Managers.Single(m => m.ManagerID == id);

            RegisterViewModel model = new RegisterViewModel()
            {
                FirstName = manager.FirstName,
                LastName = manager.LastName,
                Email = manager.Email,
                ID = manager.ManagerID,
            };

            return View(model);
        }


        // POST: /Account/CreateUserManager
        [HttpPost]
        [AllowAnonymous]
        [Route("Account/CreateUserManager")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUserManager(RegisterViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var manager = db.Managers.Single(m => m.ManagerID == model.ID);

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Position = "Manager",
                };


                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var adminId = User.Identity.GetUserId();

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

           
                    var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                        await roleManager.CreateAsync(new IdentityRole("Manager"));
                        await UserManager.AddToRoleAsync(user.Id, "Manager");
                      
                        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    var admin = await UserManager.FindByIdAsync(adminId);
                    await SignInManager.SignInAsync(admin, isPersistent: false, rememberBrowser: false);

                    manager.UserID = user.Id;
                    db.SaveChanges();

                    return RedirectToAction("UsersBoard", "Account");
                }
                AddErrors(result);
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }


        //GET: /Account/CreateUserManager
        [Route("Account/CreateUserConsultant", Name = "CreateUserConsultant")]
        public ActionResult CreateUserConsultant(int id)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var consultant = db.Consultants.Single(c => c.ConsultantID == id);

            RegisterViewModel model = new RegisterViewModel()
            {
                FirstName = consultant.FirstName,
                LastName = consultant.LastName,
                Email = consultant.Email,
                ID = consultant.ConsultantID,
            };

            return View(model);
        }


        // POST: /Account/CreateUserConsultant
        [HttpPost]
        [AllowAnonymous]
        [Route("Account/CreateUserConsultant")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUserConsultant(RegisterViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var consultant = db.Consultants.Single(c => c.ConsultantID == model.ID);

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Position = "Consultant",
                };


                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var adminId = User.Identity.GetUserId();

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);


                    var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    await roleManager.CreateAsync(new IdentityRole("Consultant"));
                    await UserManager.AddToRoleAsync(user.Id, "Consultant");

                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    var admin = await UserManager.FindByIdAsync(adminId);
                    await SignInManager.SignInAsync(admin, isPersistent: false, rememberBrowser: false);

                    consultant.UserID = user.Id;
                    db.SaveChanges();

                   
                    return RedirectToAction("UsersBoard", "Account");
                }
                AddErrors(result);
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }


        //GET: /Account/UsersBoard
        [Route("Account/UsersBoard", Name = "UsersBoard")]
        public ActionResult UsersBoard()
        {

            return View();
        }


        // GET: /Account/CreateSuperAdministrateur
        [AllowAnonymous]
        [Route("Account/RegisterSuperAdmin", Name = "RegisterSuperAdmin")]
        public ActionResult RegisterSuperAdmin()
        {
            return View();
        }


        // POST: /Account/CreateUserConsultant
        [HttpPost]
        [AllowAnonymous]
        [Route("Account/RegisterSuperAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterSuperAdmin(RegisterViewModel model)
        {
            ApplicationDbContext db = new ApplicationDbContext();
         

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Position = "Super-Administrateur",
                };


                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var adminId = User.Identity.GetUserId();

                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);


                    var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                    var roleManager = new RoleManager<IdentityRole>(roleStore);

                    await roleManager.CreateAsync(new IdentityRole("Super-Administrateur"));
                    await UserManager.AddToRoleAsync(user.Id, "Super-Administrateur");

                    AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                    var admin = await UserManager.FindByIdAsync(adminId);
                    await SignInManager.SignInAsync(admin, isPersistent: false, rememberBrowser: false);

              
                    db.SaveChanges();


                    return RedirectToAction("UsersBoard", "Account");
                }
                AddErrors(result);
            }

            // Si nous sommes arrivés là, un échec s’est produit. Réafficher le formulaire
            return View(model);
        }


        #region Applications auxiliaires
        // Utilisé(e) pour la protection XSRF lors de l'ajout de connexions externes
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}





