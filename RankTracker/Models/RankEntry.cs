using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RankTracker.Models
{
    public class RankEntry
    {
        public int Id { get; set; }

        [Required]
        public string Rank { get; set; } = string.Empty;
        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public string? Description {  get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public IdentityUser? User { get; set; }

        public int GameId { get; set; }
        [ForeignKey("GameId")]

        [Required]
        public Game Game { get; set; }
    }
}
