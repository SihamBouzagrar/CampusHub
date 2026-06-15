using CampusHub.Models;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CampusHub.Data
{
    

public class CampusDbContext : IdentityDbContext<ApplicationUser>
    {
      

        public CampusDbContext(DbContextOptions<CampusDbContext> options)
      : base(options)
        {
        }
        public DbSet<Club> Clubs { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<Announcement> Announcements { get; set; }
    public DbSet<RoomReservation> RoomReservations { get; set; }
    public DbSet<ClubMembership> ClubMemberships { get; set; }
    public DbSet<EventParticipation> EventParticipations { get; set; }
    public DbSet<Document> Documents { get; set; }

       protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ClubMembership>()
            .HasOne(cm => cm.Club)
            .WithMany(c => c.Members)
            .HasForeignKey(cm => cm.ClubId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<EventParticipation>()
            .HasOne(ep => ep.Event)
            .WithMany(e => e.Participants)
            .HasForeignKey(ep => ep.EventId)
            .OnDelete(DeleteBehavior.Cascade);

    builder.Entity<RoomReservation>()
    .HasOne(r => r.User)
    .WithMany()
    .HasForeignKey(r => r.UserId);
    }
    }

}
