using System;
using CampusHub.Models;

public class EventParticipation
{
    public int Id { get; set; }

   public int EventId { get; set; }
        public Event? Event { get; set; }

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser? User { get; set; }

    public DateTime RegisteredAt { get; set; }= DateTime.Now;
}