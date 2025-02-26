using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PartyInvitationApp.Models.InvitationModels;

public partial class PartyDbContext : DbContext
{
    public PartyDbContext()
    {
    }

    public PartyDbContext(DbContextOptions<PartyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Invitation> Invitations { get; set; }

    public virtual DbSet<Party> Parties { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PartyDb;Trusted_Connection=True;MultipleActiveResultSets=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Invitation>(entity =>
        {
            entity.HasIndex(e => e.PartyId, "IX_Invitations_PartyId");

            entity.Property(e => e.GuestName).HasMaxLength(100);

            entity.HasOne(d => d.Party).WithMany(p => p.Invitations).HasForeignKey(d => d.PartyId);
        });

        modelBuilder.Entity<Party>(entity =>
        {
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.Property(e => e.Location).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
