using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RankTracker.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        public string UserId { get; set; }
        public IdentityUser? User { get; set; }
        public ICollection<RankEntry> RankEntries { get; set; }
    }

}
