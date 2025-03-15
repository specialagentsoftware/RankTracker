using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RankTracker.Models;

namespace RankTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Game> Games { get; set; }
        public DbSet<RankEntry> RankEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            modelbuilder.Entity<Game>()
                .HasIndex(g => g.Name)
                .IsUnique();

            modelbuilder.Entity<RankEntry>()
                .HasOne(re => re.User)
                .WithMany()
                .HasForeignKey(re => re.UserId);
            modelbuilder.Entity<RankEntry>()
                .HasOne(re => re.Game)
                .WithMany(g => g.RankEntries)
                .HasForeignKey(re => re.GameId);
        }
    }
}
