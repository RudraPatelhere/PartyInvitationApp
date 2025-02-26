using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PartyInvitationApp.Models.PartyModels;

namespace PartyInvitationApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly PartyDbContext _context;

        public HomeController(PartyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var parties = _context.Parties.ToList(); // This gets all parties
            return View(parties);
        }
    }
}
