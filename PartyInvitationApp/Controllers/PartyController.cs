using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PartyInvitationApp.Models.PartyModels;

namespace PartyInvitationApp.Controllers
{
    public class PartyController : Controller
    {
        private readonly PartyDbContext _context;

        public PartyController(PartyDbContext context)
        {
            _context = context;
        }

        // ✅ GET: Party (List all parties)
        public async Task<IActionResult> Index()
        {
            return View(await _context.Parties.ToListAsync());
        }

        // ✅ UPDATED: GET Party/Details/5 (Loads Invitations Too)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var party = await _context.Parties
                .Include(p => p.Invitations) // ✅ Load Invitations List
                .FirstOrDefaultAsync(m => m.PartyId == id);

            if (party == null)
            {
                return NotFound();
            }

            return View(party);
        }

        // ✅ GET: Party/Create
        public IActionResult Create()
        {
            return View();
        }

        // ✅ POST: Party/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PartyId,Description,EventDate,Location")] Party party)
        {
            if (ModelState.IsValid)
            {
                _context.Add(party);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(party);
        }

        // ✅ GET: Party/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var party = await _context.Parties.FindAsync(id);
            if (party == null)
            {
                return NotFound();
            }
            return View(party);
        }

        // ✅ POST: Party/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartyId,Description,EventDate,Location")] Party party)
        {
            if (id != party.PartyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(party);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartyExists(party.PartyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(party);
        }

        // ✅ GET: Party/Delete/5 (Load Invitations Before Deleting)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var party = await _context.Parties
                .Include(p => p.Invitations) // ✅ Load Invitations before Deleting
                .FirstOrDefaultAsync(m => m.PartyId == id);

            if (party == null)
            {
                return NotFound();
            }

            return View(party);
        }

        // ✅ POST: Party/Delete/5 (Deletes Related Invitations First)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var party = await _context.Parties
                .Include(p => p.Invitations) // ✅ Load Invitations before deleting
                .FirstOrDefaultAsync(p => p.PartyId == id);

            if (party != null)
            {
                // ✅ Remove invitations before deleting the party
                _context.Invitations.RemoveRange(party.Invitations);
                _context.Parties.Remove(party);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> GetInvitations(int partyId)
        {
            var invitations = await _context.Invitations
                .Where(i => i.PartyId == partyId)
                .ToListAsync();

            return PartialView("_InvitationListPartial", invitations);
        }


        private bool PartyExists(int id)
        {
            return _context.Parties.Any(e => e.PartyId == id);
        }
    }
}
