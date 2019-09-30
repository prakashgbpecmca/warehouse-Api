using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMS.Identity.IdentityModels;

namespace WMS.Identity.IdentityDataAccess
{
    public class WMSIdentityDbContext : IdentityDbContext<WMSIdentityUser, WMSIdentityRole, int, WMSIdentityUserLogin, WMSIdentityUserRole, WMSIdentityUserClaim>
    {
        #region constructors and destructors
        public WMSIdentityDbContext()
            : base("WMSIdentity")
        {

        }
        #endregion
        #region methods
        public static WMSIdentityDbContext Create()
        {
            return new WMSIdentityDbContext();
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Entities to their tables. 
            modelBuilder.Entity<WMSIdentityUser>().ToTable("WMSUser");
            modelBuilder.Entity<WMSIdentityRole>().ToTable("WMSRole");
            modelBuilder.Entity<WMSIdentityUserClaim>().ToTable("WMSUserClaim");
            modelBuilder.Entity<WMSIdentityUserLogin>().ToTable("WMSUserLogin");
            modelBuilder.Entity<WMSIdentityUserRole>().ToTable("WMSUserRole");

            //  Override some column mappings that do not match our default
            //  modelBuilder.Entity<WMSIdentityUser>().Property(r => r.Id).HasColumnName("UserId");
            //  modelBuilder.Entity<WMSIdentityRole>().Property(r => r.Id).HasColumnName("RoleId");
            //  modelBuilder.Entity<WMSIdentityRole>().Property(r => r.Name).HasColumnName("RoleName");

            // Set AutoIncrement-Properties
            modelBuilder.Entity<WMSIdentityUser>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<WMSIdentityUserClaim>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            modelBuilder.Entity<WMSIdentityRole>().Property(r => r.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
        }

        #endregion
    }
}
