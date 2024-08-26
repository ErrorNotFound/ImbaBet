using ImbaBetWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ImbaBetWeb.Data;

public class ApplicationContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
                
    }

    public DbSet<Team> Teams { get; set; }

    public DbSet<MatchGroup> MatchGroups { get; set; }

    public DbSet<Match> Matches { get; set; }

    public DbSet<Community> Communities { get; set; }

    public DbSet<Bet> Bets { get; set; }

    public DbSet<Setting> Settings { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        
        builder.Entity<Community>()
                .HasMany<ApplicationUser>(c => c.Members)
                .WithOne(u => u.MemberOfCommunity)
                .HasForeignKey(u => u.MemberOfCommunityId);
        
        builder.Entity<ApplicationUser>()
            .HasOne<Community>(u => u.OwnerOfCommunity)
            .WithOne(c => c.Owner)
            .HasForeignKey<Community>(c => c.OwnerId);
        
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.EnableSensitiveDataLogging();
    }
}
