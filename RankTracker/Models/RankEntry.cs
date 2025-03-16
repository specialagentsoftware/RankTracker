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
        [Range(1, 5000, ErrorMessage = "Rank must be between 1 and 5000.")]
        public int Rank { get; set; }
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
