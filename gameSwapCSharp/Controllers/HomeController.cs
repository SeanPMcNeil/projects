using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using gameSwapCSharp.Models;

namespace gameSwapCSharp.Controllers;

public class HomeController : Controller
{
    private MyContext _context;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, MyContext context)
    {
        _logger = logger;
        _context = context;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("useractions")]
    public IActionResult UserActions()
    {
        if (HttpContext.Session.GetInt32("user") != null)
        {
            return RedirectToAction("Dashboard");
        }
        return View();
    }

    [HttpPost("user/register")]
    public IActionResult Register(User newUser)
    {
        if (ModelState.IsValid)
        {
            if(_context.Users.Any(u => u.Email == newUser.Email))
            {
                ModelState.AddModelError("Email", "Email already in use!");
                return View("UserActions");
            }

            PasswordHasher<User> Hasher = new PasswordHasher<User>();
            newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
            
            _context.Add(newUser);
            _context.SaveChanges();
            HttpContext.Session.SetInt32("user", newUser.UserId);

            return RedirectToAction("Dashboard");
        }
        else
        {
            return View("UserActions");
        }
    }

    [HttpPost("user/login")]
    public IActionResult Login(LogUser loginUser)
    {
        if (ModelState.IsValid)
        {
            User userInDb = _context.Users.FirstOrDefault(u => u.Email == loginUser.LogEmail);
            if (userInDb == null)
            {
                ModelState.AddModelError("LogEmail", "Invalid Login Attempt");
                return View("UserActions");
            }
            PasswordHasher<LogUser> hasher = new PasswordHasher<LogUser>();
            var result = hasher.VerifyHashedPassword(loginUser, userInDb.Password, loginUser.LogPassword);
            if(result == 0)
            {
                ModelState.AddModelError("LogEmail", "Invalid Login Attempt");
                return View("UserActions");
            }
            else
            {
                HttpContext.Session.SetInt32("user", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
        }
        else
        {
            return View("UserActions");
        }
    }

    [HttpGet("dashboard")]
    public IActionResult Dashboard()
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        ViewBag.LoggedUser = _context.Users.Include(g => g.OwnedGames).FirstOrDefault(a => a.UserId == (int)HttpContext.Session.GetInt32("user"));
        return View();
    }

    [HttpGet("logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index");
    }

    [HttpGet("browse")]
    public IActionResult Browse()
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        ViewBag.AllGames = _context.Games.Include(o => o.Owner).OrderBy(p => p.Platform).ToList();
        return View();
    }

    [HttpGet("game/new")]
    public IActionResult NewGame()
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        Console.WriteLine(HttpContext.Session.GetInt32("user"));
        return View();
    }

    [HttpPost("game/add")]
    public IActionResult AddGame(Game newGame)
    {
        newGame.UserId = (int)HttpContext.Session.GetInt32("user");
        if (ModelState.IsValid)
        {
            if (newGame.UserId != HttpContext.Session.GetInt32("user"))
            {
                ModelState.AddModelError("Title", "Invalid Listing Addition");
                return View("NewGame");
            }
            _context.Add(newGame);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        else
        {
            ModelState.AddModelError("Title", "Invalid Listing Addition");
            return View("NewGame");
        }
    }

    [HttpGet("game/{gameId}")]
    public IActionResult OneGame(int gameId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        Game singleGame = _context.Games.Include(o => o.Owner).SingleOrDefault(g => g.GameId == gameId);
        return View(singleGame);
    }

    [HttpGet("game/edit/{gameId}")]
    public IActionResult EditGame(int gameId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        Game singleGame = _context.Games.Include(o => o.Owner).SingleOrDefault(g => g.GameId == gameId);
        if (singleGame.UserId != HttpContext.Session.GetInt32("user"))
        {
            return RedirectToAction("Dashboard");
        }
        return View(singleGame);
    }

    [HttpPost("game/update/{gameId}")]
    public IActionResult UpdateGame(int gameId, Game updatedGame)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        Game oldGame = _context.Games.FirstOrDefault(g => g.GameId == gameId);
        if (oldGame.UserId != HttpContext.Session.GetInt32("user"))
        {
            return RedirectToAction("Dashboard");
        }
        if (ModelState.IsValid)
        {
            oldGame.Title = updatedGame.Title;
            oldGame.Platform = updatedGame.Platform;
            oldGame.Quality = updatedGame.Quality;
            oldGame.Price = updatedGame.Price;
            oldGame.UpdatedAt = DateTime.Now;
            _context.SaveChanges();

            return RedirectToAction("OneGame", oldGame);
        }
        return View("EditGame", new{ gameId = updatedGame.GameId });
    }

    [HttpGet("game/delete/{gameId}")]
    public IActionResult DeleteGame(int gameId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        Game gameToDelete = _context.Games.SingleOrDefault(g => g.GameId == gameId);
        if (gameToDelete.UserId != HttpContext.Session.GetInt32("user"))
        {
            return RedirectToAction("Dashboard");
        }
        _context.Remove(gameToDelete);
        _context.SaveChanges();
        return RedirectToAction("Dashboard");
    }

    [HttpGet("game/message/{gameId}")]
    public IActionResult Message(int gameId)
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        ViewBag.SingleGame = _context.Games.Include(o => o.Owner).SingleOrDefault(g => g.GameId == gameId);
        return View();
    }

    [HttpPost("game/message/{gameId}/send")]
    public IActionResult SendMessage(int gameId, Message newMessage)
    {
        Game requestedGame = _context.Games.Include(o => o.Owner).FirstOrDefault(g => g.GameId == gameId);
        User buyer = _context.Users.FirstOrDefault(u => u.UserId == HttpContext.Session.GetInt32("user"));
        newMessage.SenderId = buyer.UserId;
        newMessage.RecipientId = requestedGame.Owner.UserId;
        newMessage.GameId = requestedGame.GameId;
        if (ModelState.IsValid)
        {
            _context.Add(newMessage);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        else
        {
            return View("Message");
        }
    }

    [HttpGet("inbox")]
    public IActionResult Inbox()
    {
        if (HttpContext.Session.GetInt32("user") == null)
        {
            return RedirectToAction("UserActions");
        }
        ViewBag.Messages = _context.Messages.Include(r => r.Responses).Include(s => s.Sender).Include(r => r.Recipient).Where(i => i.RecipientId == HttpContext.Session.GetInt32("user")).ToList();
        List<Game> MessagedGames = new List<Game>();
        foreach (Message m in ViewBag.Messages)
        {
            Game oneGame = _context.Games.FirstOrDefault(g => g.GameId == m.GameId);
            MessagedGames.Add(oneGame);
        }
        ViewBag.RelevantGames = MessagedGames;
        return View();
    }

    [HttpGet("message/{messageId}")]
    public IActionResult OneMessage(int messageId)
    {
        Message oneMessage = _context.Messages.Include(s => s.Sender).Include(r => r.Responses).FirstOrDefault(m => m.MessageId == messageId);
        return View(oneMessage);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
