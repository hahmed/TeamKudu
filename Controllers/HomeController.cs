using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Team.Models;
using Microsoft.Framework.OptionsModel;
using System.Security.Claims;
using Microsoft.AspNet.Authentication.Cookies;

namespace Team.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ISurventrixService _surventrixService;
        private readonly ApplicationDbContext _dbContext;
        private readonly AppSettings _appSettings;

        public HomeController(ISurventrixService surventrixService,
            IOptions<AppSettings> appSettings,
            ApplicationDbContext dbContext)
        {
            _surventrixService = surventrixService;
            _dbContext = dbContext;
            _appSettings = appSettings.Options;
        }

        [Route("/")]
        [AllowAnonymous]
        public IActionResult Index()
        {
            // if not authenticated show public view...
            if (!User.Identity.IsAuthenticated)
            {
                Console.WriteLine("Non authenticated user is accessing the team app...");
                return View("Home");
            }

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            Console.WriteLine("There has been an error!");
            return View("~/Views/Shared/Error.cshtml");
        }

        [HttpGet("orgs")]
        public IActionResult Orgs(string type)
        {
            List<Organization> list = null;

            if (!string.IsNullOrEmpty(type))
            {
                CompanyType result;

                if (Enum.TryParse(type, out result))
                {
                    list = _dbContext.Organizations
                        .Where(x => x.Type == result)
                        .ToList();
                }
                else
                {
                    list = _dbContext.Organizations.ToList();
                }
            }
            else
            {
                list = _dbContext.Organizations.ToList();
            }
            return View("Orgs", list);
        }


        [HttpGet("users")]
        public IActionResult Users(bool? isSurveyor)
        {
            List<User> list = null;
            if (isSurveyor.HasValue && isSurveyor.Value == true)
            {
                list = _dbContext.Users.Where(x => x.IsSurveyor).ToList();
            }
            else
            {
                list = _dbContext.Users.ToList();
            }
            return View("Users", list);
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("email", "Please enter a valid email address");
            }
            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "Please enter a valid password");
            }

            if (ModelState.ErrorCount > 0)
                return View("Home");

            Console.WriteLine("{0} is attempting to login to team app...", email);

            // authenticate the user...
            var user = _dbContext.Users.SingleOrDefault(x => x.Email == email);

            if (user == null)
            {
                ModelState.AddModelError("email", "Please enter a valid email");
                return View("Home");
            }

            var admins = _appSettings.AdminUsers.Split(',');

            if (!admins.Any(x => x == user.Email))
            {
                Console.WriteLine("{0} is not an admin", email);
                ModelState.AddModelError("email", "Please enter a valid email");
                return View("Home");
            }

            if (password != user.Password) // oh yes I did... <3
            {
                ModelState.AddModelError("password", "Please enter a valid password");
                return View("Home");
            }
            // success, sign me in...
            Console.WriteLine("{0} just signed in successfully", email);

            ClaimsIdentity identity = CreateIdentity(user, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await Context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await Context.Authentication.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            return RedirectToAction("Index");
        }

        [HttpGet("signout")]
        [AllowAnonymous]
        public async Task<IActionResult> Signout()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index");
            }
            Console.WriteLine("{0} is attempting to signout off team app...", User.Identity.Name);
            await Context.Authentication.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index");
        }

        private ClaimsIdentity CreateIdentity(User user, string authenticationType, params Claim[] additionalClaims)
        {
            var claims = new List<Claim>() {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimTypes.AuthenticationMethod, authenticationType),
                // Needed for anti-forgery token, also good practice to have a unique identifier claim
                new Claim(ClaimTypes.NameIdentifier, user.Email)
            };

            if (additionalClaims.Length > 0)
            {
                claims.AddRange(additionalClaims);
            }

            ClaimsIdentity identity = new ClaimsIdentity(
                claims,
                authenticationType,
                nameType: ClaimsIdentity.DefaultNameClaimType,
                roleType: ClaimsIdentity.DefaultRoleClaimType);
            return identity;
        }
    }
}