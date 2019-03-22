using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CSharpExam.Models;
//added these line
using Microsoft.AspNetCore.Http;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;

namespace CSharpExam.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }

        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            Console.WriteLine("/1/1/1/1/1/1/1/1/1/1/1");
            return View();
        }
        [HttpPost("register")]
        public IActionResult Register(User user)
        {
            System.Console.WriteLine("----------------7---------------");
            if (ModelState.IsValid)
            {
                if (dbContext.Users.Any(u => u.Email == user.Email))
                {
                    // Manually add a ModelState error to the Email field, with provided
                    // error message
                    ModelState.AddModelError("Email", "Email already in use!");

                    // You may consider returning to the View at this point
                    return View("Index");
                }
                // User newUser = new User()
                // {
                //     FirstName = Request.Form["FirstName"],
                //     LastName = Request.Form["LastName"],
                //     Email = Request.Form["Email"],
                //     Password = Request.Form["Password"],
                // };
                // return View(newUser);
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                user.Password = Hasher.HashPassword(user, user.Password);
                dbContext.Add(user);
                dbContext.SaveChanges();
                HttpContext.Session.SetInt32("userInSession", user.UserId);
                Console.WriteLine("/./././././././././././././././././.");
                Console.WriteLine("redirecting to account page from register method");
                return RedirectToAction("Account", new { id = user.UserId });
            }
            else
            {
                return View("Index");
            }
        }
        [HttpPost("login")]
        public IActionResult Login(LoginUser userSubmission)
        {
            if (ModelState.IsValid)
            {
                // If inital ModelState is valid, query for a user with provided email
                var userInDb = dbContext.Users.FirstOrDefault(u => u.Email == userSubmission.Email);
                // If no user exists with provided email
                if (userInDb == null)
                {
                    // Add an error to ModelState and return to View!
                    ModelState.AddModelError("Email", "Invalid Email/Password");
                    return View("Index");
                }

                // Initialize hasher object
                var hasher = new PasswordHasher<LoginUser>();

                // varify provided password against hash stored in db
                var result = hasher.VerifyHashedPassword(userSubmission, userInDb.Password, userSubmission.Password);

                // result can be compared to 0 for failure
                if (result == 0)
                {
                    // handle failure (this should be similar to how "existing email" is handled)
                    ModelState.AddModelError("Password", "Invalid Email/Password");
                    return View("Index");
                }

                HttpContext.Session.SetInt32("userInSession", userInDb.UserId);
                // int id = userInDb.UserId;
                // return View("Account");
                return RedirectToAction("Account", new { id = userInDb.UserId });
            }
            else
            {
                return View("Index");
            }

        }
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
        [HttpGet("account/{id}")]
        public IActionResult Account(int id)
        {
            if (HttpContext.Session.GetInt32("userInSession") == null)
            {
                return View("Index");
            }
            User userBalance = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("userInSession"));
            List<Activity> listOfActs = dbContext.Activities.Include(a => a.listOfGuests).ThenInclude(b => b.User).OrderByDescending(c => c.ActivityDate).ToList();
            ViewBag.listOfActs = listOfActs;
            ViewBag.id = HttpContext.Session.GetInt32("userInSession");

            ViewBag.User = dbContext.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("userInSession"));
            return View();
        }
        [HttpGet("activityform")]
        public IActionResult ActivityForm()
        {
            if (HttpContext.Session.GetInt32("userInSession") == null)
            {
                return View("Index");
            }
            User userInSession = dbContext.Users.FirstOrDefault(user => user.UserId == HttpContext.Session.GetInt32("userInSession"));
            ViewBag.userInSession = userInSession;

            ViewBag.user = HttpContext.Session.GetInt32("userInSession");

            return View();
        }
        [HttpPost("addactivity")]
        public IActionResult AddActivity(Activity act)
        {
            DateTime CurrentTime = DateTime.Now;
            if (ModelState.IsValid)
            {
                if (DateTime.Parse(Request.Form["ActivityDate"]) < CurrentTime)
                {
                    ModelState.AddModelError("WeddingDate", "Date Cannot be in the past");
                    return View("ActivityForm");
                }
                else
                {
                    Activity newAct = act;
                    newAct.User = dbContext.Users.FirstOrDefault(u => u.UserId == act.UserId);
                    dbContext.Add(newAct);
                    dbContext.SaveChanges();
                    return RedirectToAction("ActivityInfo", new { aid = newAct.ActivityId });
                }
            }
            else
            {
                return View("ActivityForm");
            }
        }
        [HttpGet("activity/{aid}")]
        public IActionResult ActivityInfo(int aid)
        {
            if (HttpContext.Session.GetInt32("userInSession") == null)
            {
                return View("Index");
            }
            List<Activity> listOfActs = dbContext.Activities.Include(a => a.listOfGuests).ThenInclude(b => b.User).OrderByDescending(c => c.ActivityDate).ToList();
            ViewBag.listOfActs = listOfActs;
            ViewBag.id = HttpContext.Session.GetInt32("userInSession");
            ViewBag.User = dbContext.Users.FirstOrDefault(a => a.UserId == HttpContext.Session.GetInt32("userInSession"));
            ViewBag.act = dbContext.Activities.Include(a => a.listOfGuests).ThenInclude(b => b.User).FirstOrDefault(a => a.ActivityId == aid);
            return View();
        }
                [HttpGet("rsvp/{aid}/{uid}")]
        public IActionResult AddRSVP(int aid, int uid)
        {
            Activity newAct = dbContext.Activities.Include(a => a.listOfGuests).ThenInclude(b => b.User).FirstOrDefault(c => c.ActivityId == aid);
            User newUser = dbContext.Users.Include(a => a.listOfReservations).ThenInclude(b => b.Activity).FirstOrDefault(us => us.UserId == uid);
            foreach(var activity in newUser.listOfReservations)
            {
                if(activity.Activity.ActivityDate.Date == newAct.ActivityDate.Date)
                {
                    ModelState.AddModelError("WeddingDate", "Date Cannot be in the past");
                    return RedirectToAction("Account", new { id = HttpContext.Session.GetInt32("userInSession") });

                }
            }

            RSVP add = new RSVP();
            add.ActivityId = aid;
            add.UserId = uid;
            add.User = dbContext.Users.FirstOrDefault(a => a.UserId == uid);
            add.Activity = dbContext.Activities.FirstOrDefault(b => b.ActivityId == aid);
            dbContext.Add(add);
            dbContext.SaveChanges();
            return RedirectToAction("Account", new { id = HttpContext.Session.GetInt32("userInSession") });
        }
        [HttpGet("cancel/{id}")]
        public IActionResult DeleteRSVP(int id)
        {
            RSVP canceled = dbContext.RSVPs.FirstOrDefault(a => a.RSVPId == id);
            dbContext.Remove(canceled);
            dbContext.SaveChanges();
            return RedirectToAction("Account", new { id = HttpContext.Session.GetInt32("userInSession") });
        }
        [HttpGet("delete/{aid}")]
        public IActionResult Delete(int aid)
        {
            Activity deleted = dbContext.Activities.FirstOrDefault(a => a.ActivityId == aid);
            dbContext.Remove(deleted);
            dbContext.SaveChanges();

            return RedirectToAction("Account", new { id = HttpContext.Session.GetInt32("userInSession") });
        }

    }
}
