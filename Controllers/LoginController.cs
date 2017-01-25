using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using wall.Models;
using wall.Factory;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace wall.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserFactory userFactory;
        public LoginController(UserFactory uFactory){
            userFactory = uFactory;
        }
        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            ViewBag.view = "Login";
            ViewBag.other = "Register";
            ViewBag.showLink = "showRegister";
            ViewBag.err = new List<string>();
            ViewBag.confirmFail = "";
            return View();
        }
        [HttpGet]
        [Route("showRegister")]
        public IActionResult ShowRegister()
        {
            ViewBag.view = "Register";
            ViewBag.other = "Login";
            ViewBag.showLink = "showLogin";
            ViewBag.err = new List<string>();
            ViewBag.confirmFail = "";
            return View("Index");
        }
        [HttpGet]
        [Route("showLogin")]
        public IActionResult ShowLogin()
        {
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(string email, string password){
            var userResult = userFactory.Find(email).Single();

            if(userResult.email != null && password.Length > 7)
            {

                var Hasher = new PasswordHasher<User>();
                // Pass the user object, the hashed password, and the PasswordToCheck
                if(0 != Hasher.VerifyHashedPassword(userResult, userResult.password, password))
                {
                    //Handle success
                    ViewBag.user = userResult;

                    string jsonUser = JsonConvert.SerializeObject(userResult);
                    HttpContext.Session.SetString("user", jsonUser);
                    return RedirectToAction("Dashboard", "Home");
                } else {
                    ViewBag.view = "Login";
                    ViewBag.other = "Register";
                    ViewBag.showLink = "showRegister";
                    ViewBag.err = ModelState.Values;
                    ViewBag.confirmFail = "Email or password does not match.";
                    return View("Index");
                }
            } else {
                ViewBag.view = "Login";
                ViewBag.other = "Register";
                ViewBag.showLink = "showRegister";
                ViewBag.err = ModelState.Values;
                ViewBag.confirmFail = "Email or password does not match.";
                return View("Index");// combine with above?
            }
        }
        [HttpPost]
        [Route("Register")]
        public IActionResult Register(User user, string confirm){
            var emailCheck = userFactory.Find(user.email);
            User userResult = new User();

            if(emailCheck.Count() > 0){
                userResult = (User)emailCheck.Single();
            }

            if(userResult.email != null){
                ViewBag.view = "Register";
                ViewBag.other = "Login";
                ViewBag.showLink = "showLogin";
                ViewBag.err = ModelState.Values;
                ViewBag.confirmFail = "Email already registered";
                return View("Index");
            }

            if(user.password != confirm || !ModelState.IsValid){
                ViewBag.view = "Register";
                ViewBag.other = "Login";
                ViewBag.showLink = "showLogin";
                ViewBag.err = ModelState.Values;
                if(user.password != confirm){
                    ViewBag.confirmFail = "Passwords do not match.";
                } else {
                    ViewBag.confirmFail = "";
                }
                return View("Index");
            }

            if(ModelState.IsValid){// don't really need this check right?

                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.password = Hasher.HashPassword(user, user.password);

                userFactory.Add(user);
                ViewBag.user = user;

                //serialize user
                string jsonUser = JsonConvert.SerializeObject(user);
                HttpContext.Session.SetString("user", jsonUser);
            }
            return RedirectToAction("Dashboard", "Home");
        }
        [HttpGet]
        [Route("logout")]
        public IActionResult Logout(){
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
