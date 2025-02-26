using Microsoft.EntityFrameworkCore;

namespace PartyInvitationApp.Models.PartyModels
{
    public class PartyDbContext : DbContext
    {
        // Constructor to pass configuration options
        public PartyDbContext(DbContextOptions<PartyDbContext> options) : base(options) { }

        // Table for Parties
        public DbSet<Party> Parties { get; set; }

        // Table for Invitations
        public DbSet<Invitation> Invitations { get; set; }

        // Configuring Entity Relationships using Fluent API (Optional)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // One Party can have many Invitations
            modelBuilder.Entity<Invitation>()
                .HasOne(i => i.Party)  // Each Invitation has one Party
                .WithMany(p => p.Invitations) // Each Party can have multiple Invitations
                .HasForeignKey(i => i.PartyId) // Foreign Key
                .OnDelete(DeleteBehavior.Cascade); // Cascade Delete: If a party is deleted, its invitations are too
        }
    }
}
