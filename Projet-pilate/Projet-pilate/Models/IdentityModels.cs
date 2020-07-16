using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Projet_pilate.Entities;

namespace Projet_pilate.Models
{
    // Vous pouvez ajouter des données de profil pour l'utilisateur en ajoutant d'autres propriétés à votre classe ApplicationUser. Pour en savoir plus, consultez https://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Position { get; set; }



        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Notez qu'authenticationType doit correspondre à l'élément défini dans CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Ajouter les revendications personnalisées de l’utilisateur ici
            return userIdentity;
        }

        public static implicit operator List<object>(ApplicationUser v)
        {
            throw new NotImplementedException();
        }
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

            modelBuilder.Entity<ApplicationUser>().ToTable("User");
            modelBuilder.Entity<IdentityRole>().ToTable("Role");
            modelBuilder.Entity<IdentityUserRole>().ToTable("UserRole");
            modelBuilder.Entity<IdentityUserClaim>().ToTable("UserClaim");
            modelBuilder.Entity<IdentityUserLogin>().ToTable("UserLogin");


            modelBuilder.Entity<CompanyContact>()
             .HasRequired(t => t.Company)
             .WithMany(t => t.CompanyContacts)
             .HasForeignKey(d => d.CompanyID)
             .WillCascadeOnDelete(false);


            modelBuilder.Entity<CompanyContact>()
               .HasRequired(t => t.Manager)
               .WithMany(t => t.CompanyContacts)
               .HasForeignKey(d => d.ManagerID)
               .WillCascadeOnDelete(false);


            modelBuilder.Entity<Manager>()
                .HasRequired(t => t.Subsidiary)
                .WithMany(t => t.Managers)
                .HasForeignKey(d => d.SubsidiaryID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Manager>()
              .HasMany(t => t.ProfitCenters)
              .WithMany(t => t.Managers);


            modelBuilder.Entity<Consultant>()
               .HasRequired(t => t.ProfitCenter)
               .WithMany(t => t.Consultants)
               .HasForeignKey(d => d.ProfitCenterID)
               .WillCascadeOnDelete(false);

            modelBuilder.Entity<Consultant>()
                .HasRequired(t => t.Subsidiary)
                .WithMany(t => t.Consultants)
                .HasForeignKey(d => d.SubsidiaryID)
                .WillCascadeOnDelete(false);


            modelBuilder.Entity<Mission>()
             .HasRequired(t => t.Consultant)
             .WithMany(t => t.Missions)
             .HasForeignKey(d => d.ConsultantID)
             .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mission>()
             .HasRequired(t => t.CompanyContact)
             .WithMany(t => t.Missions)
             .HasForeignKey(d => d.CompanyContactID)
             .WillCascadeOnDelete(false);

            /*modelBuilder.Entity<Mission>()
             .HasRequired(t => t.Cra)
             .WithMany(t => t.Missions)
             .HasForeignKey(d => d.CraID)
             .WillCascadeOnDelete(false);*/


            modelBuilder.Entity<Cra>()
               .HasRequired(t => t.Consultant)
               .WithMany(t => t.Cras)
               .HasForeignKey(d => d.ConsultantID)
               .WillCascadeOnDelete(false);


            modelBuilder.Entity<Activity>()
               .HasRequired(t => t.Cra)
               .WithMany(t => t.Activities)
               .HasForeignKey(d => d.CraID);



            // relation de type "self referencing" pour la table ProfitCenter
            modelBuilder.Entity<ProfitCenter>()
             .HasOptional(t => t.FatherProfitCenter)
             .WithMany()
             .HasForeignKey(c => c.FatherProfitCenterID);

        }

        public DbSet<Company> Companies { get; set; }
        public DbSet<CompanyContact> CompanyContacts { get; set; }
        public DbSet<Subsidiary> Subsidiaries { get; set; }
        public DbSet<Manager> Managers { get; set; }
        public DbSet<ProfitCenter> profitCenters { get; set; }
        public DbSet<Mission> Missions { get; set; }
        public DbSet<Consultant> Consultants { get; set; }
        public DbSet<Cra> Cras { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<MonthActivation> MonthActivations { get; set; }
        public DbSet<MonthCloture> MonthClotures { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Facture> Factures { get; set; }
        public DbSet<Suivi> Suivis { get; set; }
        public DbSet<Info> Infos { get; set; }

    }


}

