using compete_poco.Models;
using Compete_POCO_Models.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;
namespace Compete_POCO_Models.Infrastrcuture.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Match> Matches { get; set; } = null!;
        public DbSet<Config> Configs { get; set; } = null!;
        public DbSet<UserStat> UserStats { get; set; } = null!;
        public DbSet<PlatformEvent> PlatformEvents { get; set; } = null!;
        public DbSet<PayEvent> PayEvents { get; set; } = null!;
        public DbSet<UserBid> Bids { get; set; } = null!;
        public DbSet<Pay> Pays { get; set; } = null!;
        public DbSet<Report> Reports { get; set; } = null!;
        public DbSet<UserAward> Awards { get; set; } = null!;
        public DbSet<ChatMessage> ChatMessages { get; set; } = null!;
        public DbSet<AppealChatMessage> AppealChatMessages { get; set; } = null!;
        public DbSet<Server> Servers { get; set; } = null!;
        public DbSet<Lobby> Lobbies { get; set; } = null!;
        public Microsoft.EntityFrameworkCore.DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<AppealChat> AppealChats { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Lobby>()
                .Property(e => e.PickMaps)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<Map>>(v, new JsonSerializerOptions())!,
                    new ValueComparer<List<Map>>(
                        (c1, c2) => c1!.SequenceEqual(c2!),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));

            modelBuilder.Entity<Lobby>()
               .Property(e => e.MapActions)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                   v => JsonSerializer.Deserialize<List<MapActionInfo>>(v, new JsonSerializerOptions())!,
                   new ValueComparer<List<MapActionInfo>>(
                       (c1, c2) => c1!.SequenceEqual(c2!),
                       c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                       c => c.ToList()));

            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Team)
                .WithOne(t => t.Chat);
            modelBuilder.Entity<Lobby>()
                .HasOne(l => l.Server)
                .WithMany(s => s.Lobbies)
                .HasForeignKey(c => c.ServerId);

            modelBuilder.Entity<Server>()
               .Property(e => e.PlayingPorts)
               .HasConversion(
                   v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                   v => JsonSerializer.Deserialize<List<LobbyPort>>(v, new JsonSerializerOptions())!,
                   new ValueComparer<List<LobbyPort>>(
                       (c1, c2) => c1!.SequenceEqual(c2!),
                       c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                       c => c.ToList()));

            modelBuilder.Entity<User>()
                .HasIndex(u => u.SteamId)
                .IsUnique();

            modelBuilder.Entity<Config>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<PayEvent>()
                .HasIndex(p => new { p.PayState, p.PaymentId })
                .IsUnique()
                .HasFilter(@"""PaymentId"" IS NOT NULL");
        }
    }
}
