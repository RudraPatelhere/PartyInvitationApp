using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PartyInvitationApp.Models.PartyModels
{
    public enum InvitationStatus
    {
        InviteNotSent,
        InviteSent,
        RespondedYes,
        RespondedNo
    }

    public class Invitation
    {
        [Key]
        public int InvitationId { get; set; }  // Primary Key

        [Required(ErrorMessage = "Guest Name is required")]
        [StringLength(100)]
        public required string GuestName { get; set; }  // FIXED: Added 'required' modifier

        [Required(ErrorMessage = "Guest Email is required")]
        [EmailAddress]
        public required string GuestEmail { get; set; }  // FIXED: Added 'required' modifier

        [Required]
        public InvitationStatus Status { get; set; } = InvitationStatus.InviteNotSent;

        // Foreign Key: Each invitation belongs to a party
        [ForeignKey("Party")]
        public int PartyId { get; set; }

        // Navigation Property (Nullable because it's automatically set when fetching related data)
        public Party? Party { get; set; }  // FIXED: Made nullable by adding '?'
    }
}
