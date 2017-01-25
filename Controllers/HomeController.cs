using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using wall.Models;
using wall.Factory;
// using System;

namespace wall.Controllers
{
    public class HomeController : Controller
    {
        private readonly MessageFactory messageFactory;
        public HomeController(MessageFactory message)
        {
            //Instantiate a UserFactory object that is immutable (READONLY)
            //This is establish the initial DB connection for us.
            messageFactory = message;
        }
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Dashboard()
        {
            if(HttpContext.Session.GetString("user") == null){
                return RedirectToAction("Index", "Login");
            }
            // ModelBundle ViewBundle = new ModelBundle{ MessageModel = UserObject, CommentModel = ProductList };
            User sessionUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.user = sessionUser;
            ViewBag.messages = messageFactory.FindAll();
            
            return View();
        }
        [HttpPost]
        [Route("addMessage")]
        public IActionResult AddMessage(Message newMessage)
        {   //will a newly registered user have an id when posting the first time after having just registered?
            if(HttpContext.Session.GetString("user") == null){
                return RedirectToAction("Index", "Login");
            }
            User sessionUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            ViewBag.user = sessionUser;
            newMessage.message = sessionUser.first + " says " + newMessage.message;

            if(ModelState.IsValid){
                // Save the new message
                messageFactory.Add(newMessage);
                return RedirectToAction("Dashboard");
            } else {
                ViewBag.item = ModelState.Values;
                return View("Dashboard");
            }
        }
        [HttpPost]
        [Route("deleteMessage/{id}")]
        public IActionResult DeleteMessage(int id)
        {
            messageFactory.Delete(@id);//should also delete all comments of this message
            return RedirectToAction("Dashboard");
        }
        [HttpPost]
        [Route("addComment")]
        public IActionResult AddComment(Comment newComment)
        {
            //will a newly registered user have an id when posting the first time after having just registered?
            User sessionUser = JsonConvert.DeserializeObject<User>(HttpContext.Session.GetString("user"));
            newComment.userid = sessionUser.id;
            messageFactory.AddComment(@newComment);
            return RedirectToAction("Dashboard");
        }
    }
}
