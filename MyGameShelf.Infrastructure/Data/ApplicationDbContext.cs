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

        // Seeding Platforms
        //modelBuilder.Entity<Platform>().HasData(
        //    new Platform { Id = 1, Name = "PC" },
        //    new Platform { Id = 2, Name = "PlayStation 5" },
        //    new Platform { Id = 3, Name = "Xbox One" },
        //    new Platform { Id = 4, Name = "PlayStation 4" },
        //    new Platform { Id = 5, Name = "Xbox Series S/X" },
        //    new Platform { Id = 6, Name = "Nintendo Switch" },
        //    new Platform { Id = 7, Name = "iOS" },
        //    new Platform { Id = 8, Name = "Android" },
        //    new Platform { Id = 9, Name = "Nintendo 3DS" },
        //    new Platform { Id = 10, Name = "Nintendo DS" },
        //    new Platform { Id = 11, Name = "Nintendo DSi" },
        //    new Platform { Id = 12, Name = "macOS" },
        //    new Platform { Id = 13, Name = "Linux" },
        //    new Platform { Id = 14, Name = "Xbox 360" },
        //    new Platform { Id = 15, Name = "Xbox" },
        //    new Platform { Id = 16, Name = "PlayStation 3" },
        //    new Platform { Id = 17, Name = "PlayStation 2" },
        //    new Platform { Id = 18, Name = "PlayStation" },
        //    new Platform { Id = 18, Name = "PS Vita" },
        //    new Platform { Id = 18, Name = "PSP" },
        //    new Platform { Id = 18, Name = "Wii U" },
        //    new Platform { Id = 18, Name = "Wii" },
        //    new Platform { Id = 18, Name = "GameCube" },
        //    new Platform { Id = 18, Name = "Nintendo 64" },
        //    new Platform { Id = 18, Name = "Game Boy Advance" },
        //    new Platform { Id = 18, Name = "Game Boy Color" },
        //    new Platform { Id = 18, Name = "Game Boy" },
        //    new Platform { Id = 18, Name = "SNES" },
        //    new Platform { Id = 18, Name = "NES" },
        //    new Platform { Id = 18, Name = "Classic Macintosh" },
        //    new Platform { Id = 18, Name = "Apple II" },
        //    new Platform { Id = 18, Name = "Commodore / Amiga" },
        //    new Platform { Id = 18, Name = "Atari 7800" },
        //    new Platform { Id = 18, Name = "Atari 5200" },
        //    new Platform { Id = 18, Name = "Atari 2600" },
        //    new Platform { Id = 18, Name = "Atari Flashback" },
        //    new Platform { Id = 18, Name = "Atari 8-bit" },
        //    new Platform { Id = 18, Name = "Atari ST" },
        //    new Platform { Id = 18, Name = "Atari Lynx" },
        //    new Platform { Id = 18, Name = "Atari XEGS" },
        //    new Platform { Id = 18, Name = "Genesis" },
        //    new Platform { Id = 18, Name = "SEGA Saturn" },
        //    new Platform { Id = 18, Name = "SEGA CD" },
        //    new Platform { Id = 18, Name = "SEGA 32X" },
        //    new Platform { Id = 18, Name = "SEGA Master System" },
        //    new Platform { Id = 18, Name = "Dreamcast" },
        //    new Platform { Id = 18, Name = "3DO" },
        //    new Platform { Id = 18, Name = "Jaguar" },
        //    new Platform { Id = 18, Name = "Game Gear" },
        //    new Platform { Id = 18, Name = "Neo Geo" }
        //);


        // Composite keys for join tables
        modelBuilder.Entity<GamePlatform>().HasKey(x => new { x.GameId, x.PlatformId });
        modelBuilder.Entity<GameDeveloper>().HasKey(x => new { x.GameId, x.DeveloperId });
        modelBuilder.Entity<GamePublisher>().HasKey(x => new { x.GameId, x.PublisherId });
        modelBuilder.Entity<GameGenre>().HasKey(x => new { x.GameId, x.GenreId });
        modelBuilder.Entity<GameTag>().HasKey(x => new { x.GameId, x.TagId });

        // Unique constraint for Review: one review per user per game
        modelBuilder.Entity<Game>()
            .HasIndex(g => g.RawgId)
            .IsUnique();

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
