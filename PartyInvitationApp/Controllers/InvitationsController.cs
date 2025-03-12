using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PartyInvitationApp.Models.PartyModels;
using PartyInvitationApp.Services; // ✅ Import EmailService

namespace PartyInvitationApp.Controllers
{
    public class InvitationsController : Controller
    {
        private readonly PartyDbContext _context;
        private readonly EmailService _emailService; // ✅ Inject EmailService

        public InvitationsController(PartyDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService; // ✅ Assign EmailService
        }

        // ✅ GET: Invitations (Lists all invitations)
        public async Task<IActionResult> Index()
        {
            var partyDbContext = _context.Invitations.Include(i => i.Party);
            return View(await partyDbContext.ToListAsync());
        }

        // ✅ GET: Invitations/Details/{id}
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

        // ✅ GET: Invitations/Create (Ensures Party ID is Passed)
        public IActionResult Create(int? partyId)
        {
            if (partyId == null)
            {
                return NotFound("Party ID is required to send an invitation.");
            }

            ViewBag.PartyId = partyId; // ✅ Pass Party ID to View
            return View();
        }

        // ✅ POST: Invitations/Create (Now Sends Email & Redirects)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("InvitationId,GuestName,GuestEmail,Status,PartyId")] Invitation invitation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(invitation);
                await _context.SaveChangesAsync();

                // ✅ Fetch Party Details to Include in Email
                var party = await _context.Parties
                    .Include(p => p.Invitations)
                    .FirstOrDefaultAsync(p => p.PartyId == invitation.PartyId);

                if (party != null)
                {
                    string rsvpLink = Url.Action("RSVP", "Invitations", new { id = invitation.InvitationId }, Request.Scheme);
                    string subject = $"You're Invited to {party.Description}!";
                    string body = $@"
                        <h3>Hello {invitation.GuestName},</h3>
                        <p>You are invited to <strong>{party.Description}</strong> at {party.Location} on {party.EventDate:MMMM dd, yyyy}.</p>
                        <p>Please confirm your attendance by clicking the link below:</p>
                        <p><a href='{rsvpLink}' target='_blank' style='color:blue;'>Click here to RSVP</a></p>
                        <p>Thank you!</p>";

                    await _emailService.SendEmailAsync(invitation.GuestEmail, subject, body);
                }

                // ✅ Redirect back to the Party Details page to show the updated invitations list
                return RedirectToAction("Details", "Party", new { id = invitation.PartyId });
            }

            ViewBag.PartyId = invitation.PartyId; // ✅ Ensure PartyId is retained
            return View(invitation);
        }

        // ✅ GET: Invitations/Edit/{id}
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

            return View(invitation);
        }

        // ✅ POST: Invitations/Edit/{id}
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
            return View(invitation);
        }

        // ✅ GET: Invitations/Delete/{id}
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

        // ✅ POST: Invitations/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation != null)
            {
                _context.Invitations.Remove(invitation);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool InvitationExists(int id)
        {
            return _context.Invitations.Any(e => e.InvitationId == id);
        }

        // ✅ GET: RSVP Page (Ensures Party Details Are Loaded)
        public async Task<IActionResult> RSVP(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var invitation = await _context.Invitations
                .Include(i => i.Party) // ✅ Ensures Party data is included
                .FirstOrDefaultAsync(i => i.InvitationId == id);

            if (invitation == null)
            {
                return NotFound();
            }

            return View(invitation);
        }

        // ✅ POST: RSVP (Updates Status & Redirects)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RSVP(int id, string response)
        {
            var invitation = await _context.Invitations.FindAsync(id);
            if (invitation == null)
            {
                return NotFound();
            }

            // ✅ Update RSVP Status (With Color Coding)
            invitation.Status = response switch
            {
                "RespondedYes" => InvitationStatus.RespondedYes,
                "RespondedNo" => InvitationStatus.RespondedNo,
                _ => InvitationStatus.InviteSent
            };

            _context.Update(invitation);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Party", new { id = invitation.PartyId });
        }
    }
}
