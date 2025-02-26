using System;
using System.Collections.Generic;

namespace PartyInvitationApp.Models.InvitationModels;

public partial class Invitation
{
    public int InvitationId { get; set; }

    public string GuestName { get; set; } = null!;

    public string GuestEmail { get; set; } = null!;

    public int Status { get; set; }

    public int PartyId { get; set; }

    public virtual Party Party { get; set; } = null!;
}
