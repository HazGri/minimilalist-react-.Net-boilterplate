using Microsoft.EntityFrameworkCore;
using Backend.Models;
//PORTE D'ENTREE VERS LA BASE DE DONNEE (ICI ORACLE)
namespace Backend.Data
{
    
    public class ApplicationDbContext : DbContext
    {
        //on injecte la config bdd ici 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        //ici on vise la table voulu
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            

            modelBuilder.Entity<User>(entity =>
            {

                entity.Property(e => e.Id)
                    .UseIdentityColumn();
                    
    
                entity.HasIndex(e => e.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_USERS_EMAIL");
                    

                entity.HasIndex(e => e.Username)
                    .IsUnique()
                    .HasDatabaseName("IX_USERS_USERNAME");
            });
        }
    }
}