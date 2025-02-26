using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PartyInvitationApp.Models.PartyModels
{
    public class Party
    {
        [Key]
        public int PartyId { get; set; }  // Primary Key

        [Required(ErrorMessage = "Description is required")]
        [StringLength(255)]
        public required string Description { get; set; }  // FIXED: Added 'required' modifier

        [Required(ErrorMessage = "Event Date is required")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required(ErrorMessage = "Location is required")]
        [StringLength(150)]
        public required string Location { get; set; }  // FIXED: Added 'required' modifier

        // Navigation property: One party can have multiple invitations
        public List<Invitation>? Invitations { get; set; } = new List<Invitation>();  // FIXED: Made nullable
    }
}
