using Api.Models;
using Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

namespace Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<BlacklistedToken> BlacklistedTokens { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Parking> Parkings { get; set; }
        public DbSet<Energie> Energies { get; set; }
        public DbSet<Materiau> Materiaux { get; set; }
        public DbSet<SiteMateriau> SiteMateriaux { get; set; }
        public DbSet<FacteurEnergie> FacteursEnergie { get; set; }
        public DbSet<EmissionMensuelle> EmissionsMensuelles { get; set; }
        public DbSet<EmissionSnapshot> EmissionSnapshots { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = (int)UserRole.User, Name = "User" },
                new Role { Id = (int)UserRole.Admin, Name = "Admin" }
            );

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Email = "admin@capgemini.com",
                    Password = "$2a$11$KNMCQAxUaFy8KR8PJFjCpuJKs1SGk62uPaY7fbuJUk9smD.8z0OwO",
                    RoleId = (int)UserRole.Admin
                }
            );

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(prt => prt.User)
                .WithMany()
                .HasForeignKey(prt => prt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(prt => prt.Token)
                .IsUnique();

            modelBuilder.Entity<BlacklistedToken>()
                .HasIndex(bt => bt.Token);

            modelBuilder.Entity<Parking>()
                .HasOne(p => p.Site)
                .WithMany(s => s.Parkings)
                .HasForeignKey(p => p.SiteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Energie>()
                .HasOne(e => e.Site)
                .WithMany(s => s.Energies)
                .HasForeignKey(e => e.SiteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SiteMateriau>()
                .HasOne(sm => sm.Site)
                .WithMany(s => s.SiteMateriaux)
                .HasForeignKey(sm => sm.SiteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SiteMateriau>()
                .HasOne(sm => sm.Materiau)
                .WithMany(m => m.SiteMateriaux)
                .HasForeignKey(sm => sm.MateriauId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmissionMensuelle>()
                .HasOne(em => em.Site)
                .WithMany(s => s.EmissionsMensuelles)
                .HasForeignKey(em => em.SiteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmissionMensuelle>()
                .HasIndex(em => new { em.SiteId, em.Annee, em.Mois })
                .IsUnique();

            modelBuilder.Entity<EmissionSnapshot>()
                .HasOne(es => es.Site)
                .WithMany(s => s.EmissionSnapshots)
                .HasForeignKey(es => es.SiteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmissionSnapshot>()
                .HasIndex(es => new { es.SiteId, es.PeriodeDebut, es.PeriodeFin });
        }
    }
}
