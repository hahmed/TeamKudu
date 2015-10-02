using Microsoft.Data.Entity;

namespace Team.Models
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("Users");          
            modelBuilder.Entity<User>().Key(c => c.UserId);
            modelBuilder.Entity<User>().Property(c => c.UserId).HasColumnName("Id");     
            modelBuilder.Entity<User>().Property(x=> x.Identifier).HasColumnName("Login");
            modelBuilder.Entity<User>().Property(x=> x.Name).HasColumnName("Name");
            modelBuilder.Entity<User>().Property(x=> x.Email).HasColumnName("Email");
            modelBuilder.Entity<User>().Property(x=> x.IsSurveyor).HasColumnName("isSurveyor");
            modelBuilder.Entity<User>().Property(x=> x.Password).HasColumnName("Password");
            modelBuilder.Entity<User>().Property(x=> x.Status).HasColumnName("Status");
            
            // mappings for orgs...
            
            modelBuilder.Entity<Organization>().ToTable("Organizations");
            modelBuilder.Entity<Organization>().Key(x => x.OrganizationId);
            modelBuilder.Entity<Organization>().Property(x => x.OrganizationId).HasColumnName("ID");
            modelBuilder.Entity<Organization>().Property(x => x.Identifier).HasColumnName("Identifier");
            modelBuilder.Entity<Organization>().Property(x => x.Name).HasColumnName("Name");
            modelBuilder.Entity<Organization>().Property(x => x.Email).HasColumnName("Email");
            modelBuilder.Entity<Organization>().Property(x => x.BillingEmail).HasColumnName("BillingEmail");
            modelBuilder.Entity<Organization>().Property(x => x.CreatedAt).HasColumnName("FirstAdded");
            modelBuilder.Entity<Organization>().Property(x => x.Type).HasColumnName("Type");
            modelBuilder.Entity<Organization>().Ignore(x => x.Status);//.HasColumnName("Status");
        }
    }
}