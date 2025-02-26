using System;
using System.Collections.Generic;

namespace PartyInvitationApp.Models.InvitationModels;

public partial class Party
{
    public int PartyId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime EventDate { get; set; }

    public string Location { get; set; } = null!;

    public virtual ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
}
