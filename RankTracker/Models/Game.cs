using System.Collections.Generic;

namespace RankTracker.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<RankEntry> RankEntries { get; set; } = new List<RankEntry>();
    }
}
