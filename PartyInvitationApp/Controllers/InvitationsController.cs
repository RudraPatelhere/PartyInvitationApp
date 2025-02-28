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
    public class InvitationsController : Controller
    {
        private readonly PartyDbContext _context;

        public InvitationsController(PartyDbContext context)
        {
            _context = context;
        }

        // GET: Invitations
        public async Task<IActionResult> Index()
        {
            var partyDbContext = _context.Invitations.Include(i => i.Party);
            return View(await partyDbContext.ToListAsync());
        }

        // GET: Invitations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitations
                .Include(i => i.Party)
                .FirstOrDefaultAsync(m => m.InvitationId == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // GET: Invitations/Create
        public IActionResult Create()
        {
            ViewData["PartyId"] = new SelectList(_context.Parties, "PartyId", "Description");
            return View();
        }

        // POST: Invitations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvitationId,GuestName,GuestEmail,Status,PartyId")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invitation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PartyId"] = new SelectList(_context.Parties, "PartyId", "Description", invitation.PartyId);
            return View(invitation);
        }

        // GET: Invitations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation == null)
            {
                return NotFound();
            }
            ViewData["PartyId"] = new SelectList(_context.Parties, "PartyId", "Description", invitation.PartyId);
            return View(invitation);
        }

        // POST: Invitations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("InvitationId,GuestName,GuestEmail,Status,PartyId")] Invitation invitation)
        {
            if (id != invitation.InvitationId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(invitation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InvitationExists(invitation.InvitationId))
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
            ViewData["PartyId"] = new SelectList(_context.Parties, "PartyId", "Description", invitation.PartyId);
            return View(invitation);
        }

        // GET: Invitations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitations
                .Include(i => i.Party)
                .FirstOrDefaultAsync(m => m.InvitationId == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // POST: Invitations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation != null)
            {
                _context.Invitations.Remove(invitation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool InvitationExists(int id)
        {
            return _context.Invitations.Any(e => e.InvitationId == id);
        }

        // ===================== RSVP Actions =====================

        // GET: Invitations/RSVP/5
        public IActionResult RSVP(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = _context.Invitations.FirstOrDefault(i => i.InvitationId == id);
            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // POST: Invitations/RSVP/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RSVP(int id, string response)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation == null)
            {
                return NotFound();
            }

            // Update status based on the guest response
            if (response == "RespondedYes")
            {
                invitation.Status = InvitationStatus.RespondedYes;
            }
            else if (response == "RespondedNo")
            {
                invitation.Status = InvitationStatus.RespondedNo;
            }
            else
            {
                // Default case
                invitation.Status = InvitationStatus.InviteSent;
            }

            _context.Update(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = invitation.InvitationId });
        }
    }
}
