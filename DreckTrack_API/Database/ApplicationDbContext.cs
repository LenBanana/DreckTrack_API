using DreckTrack_API.Models.Entities;
using DreckTrack_API.Models.Entities.Collectibles;
using DreckTrack_API.Models.Entities.Collectibles.Items;
using DreckTrack_API.Models.Entities.Collectibles.Items.Book;
using DreckTrack_API.Models.Entities.Collectibles.Items.Game;
using DreckTrack_API.Models.Entities.Collectibles.Items.Movie;
using DreckTrack_API.Models.Entities.Collectibles.Items.Show;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DreckTrack_API.Database;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<CollectibleItem> CollectibleItems { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<UserCollectibleItem> UserCollectibleItems { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure entity inheritance
        builder.Entity<CollectibleItem>()
            .HasDiscriminator<string>("ItemType")
            .HasValue<Book>("Book")
            .HasValue<Movie>("Movie")
            .HasValue<Show>("Show")
            .HasValue<Game>("Game");

        // Configure many-to-many relationship
        builder.Entity<UserCollectibleItem>()
            .HasOne(uci => uci.User)
            .WithMany(u => u.UserCollectibleItems)
            .HasForeignKey(uci => uci.UserId);

        builder.Entity<UserCollectibleItem>()
            .HasOne(uci => uci.CollectibleItem)
            .WithMany()
            .HasForeignKey(uci => uci.CollectibleItemId)
            .OnDelete(DeleteBehavior.Cascade); // Enable cascading delete

        builder.Entity<UserCollectibleItem>()
            .Property(u => u.Status)
            .HasConversion<string>();

        builder.Entity<Book>()
            .Property(b => b.Format)
            .HasConversion<string>();

        builder.Entity<Show>()
            .HasMany(s => s.Seasons);
        
        builder.Entity<Season>()
            .HasMany(s => s.Episodes);
    }
}