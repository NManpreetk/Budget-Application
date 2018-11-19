using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;

namespace BudgetApplication.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            CreatedHouseHolds = new HashSet<HouseHolds>();
            Transactions = new HashSet<Transactions>();
            HouseHolds = new HashSet<HouseHolds>();
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        [InverseProperty("Creator")]
        public virtual ICollection<HouseHolds> CreatedHouseHolds { get; set; }
        [InverseProperty("HouseHoldUser")]
        public virtual ICollection<HouseHolds> HouseHolds { get; set; }
        public virtual ICollection<Transactions> Transactions { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }
        
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Transactions>()
               .HasRequired(p => p.category)
               .WithMany(p => p.Transactions)
               .WillCascadeOnDelete(false);
        }

       

        public System.Data.Entity.DbSet<BudgetApplication.Models.HouseHolds> HouseHolds { get; set; }

        public System.Data.Entity.DbSet<BudgetApplication.Models.HouseHoldInvites> HouseHoldInvites { get; set; }

        public System.Data.Entity.DbSet<BudgetApplication.Models.Login> Logins { get; set; }

        public System.Data.Entity.DbSet<BudgetApplication.Models.Categories> Categories { get; set; }

        public System.Data.Entity.DbSet<BudgetApplication.Models.Accounts> Accounts { get; set; }

        public System.Data.Entity.DbSet<BudgetApplication.Models.Transactions> Transactions { get; set; }

    }
}