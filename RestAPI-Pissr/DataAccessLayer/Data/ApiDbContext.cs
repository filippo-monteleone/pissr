using DataAccessLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccessLayer.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        /* protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dispositivo>().HasOne<Campo>(d => d.Campo).WithMany(c => c.Dispositivi)
                .HasForeignKey(d => d.CampoId);

            base.OnModelCreating(modelBuilder);
        }*/



        public virtual DbSet<AziendaAgricola> AziendeAgricole { get; set; }
        public virtual DbSet<AziendaIdrica> AziendeIdriche { get; set; }
        public virtual DbSet<Campo> Campi { get; set; }
        public virtual DbSet<Dispositivo> Dispositivi { get; set; }
        public virtual DbSet<Evento> Eventi { get; set; }
        public virtual DbSet<Contratto> Contratti { get; set; }

    }
}
