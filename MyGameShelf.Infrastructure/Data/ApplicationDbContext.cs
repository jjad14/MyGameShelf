using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyGameShelf.Domain.Models;
using MyGameShelf.Infrastructure.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGameShelf.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DB Sets for your entities
    public DbSet<Game> Games { get; set; }
    public DbSet<UserGame> UserGames { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    public DbSet<Platform> Platforms { get; set; }
    public DbSet<GamePlatform> GamePlatforms { get; set; }

    public DbSet<Developer> Developers { get; set; }
    public DbSet<GameDeveloper> GameDevelopers { get; set; }

    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<GamePublisher> GamePublishers { get; set; }

    public DbSet<Genre> Genres { get; set; }
    public DbSet<GameGenre> GameGenres { get; set; }

    public DbSet<Tag> Tags { get; set; }
    public DbSet<GameTag> GameTags { get; set; }

    public DbSet<UserFollow> UserFollows { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { 
        base.OnModelCreating(modelBuilder);

        // Composite keys for join tables
        modelBuilder.Entity<GamePlatform>().HasKey(x => new { x.GameId, x.PlatformId });
        modelBuilder.Entity<GameDeveloper>().HasKey(x => new { x.GameId, x.DeveloperId });
        modelBuilder.Entity<GamePublisher>().HasKey(x => new { x.GameId, x.PublisherId });
        modelBuilder.Entity<GameGenre>().HasKey(x => new { x.GameId, x.GenreId });
        modelBuilder.Entity<GameTag>().HasKey(x => new { x.GameId, x.TagId });

        // Unique constraint for Review: one review per user per game
        modelBuilder.Entity<Review>()
            .HasIndex(r => new { r.UserId, r.GameId })
            .IsUnique();

        // Configure UserGame relationships
        modelBuilder.Entity<UserGame>()
            .HasIndex(ug => new { ug.UserId, ug.GameId })
            .IsUnique();

        modelBuilder.Entity<UserGame>()
            .HasOne(ug => ug.Game)
            .WithMany(g => g.UserGames)
            .HasForeignKey(ug => ug.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserGame>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(ug => ug.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        // Configure Favorite relationships
        modelBuilder.Entity<Favorite>()
            .HasOne(f => f.Game)
            .WithMany()
            .HasForeignKey(f => f.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Favorite>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // User - Follower - Following Relationship
        modelBuilder.Entity<UserFollow>()
            .HasKey(f => new { f.FollowerId, f.FolloweeId });

        modelBuilder.Entity<UserFollow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserFollow>()
            .HasOne(f => f.Followee)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict);

    }

}
