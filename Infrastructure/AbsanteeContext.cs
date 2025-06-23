using Domain.Models;
using Infrastructure.DataModel;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class AbsanteeContext : DbContext
    {
        public virtual DbSet<HolidayPlanDataModel> HolidayPlans { get; set; }
        public virtual DbSet<HolidayPeriodDataModel> HolidayPeriods { get; set; }
        public virtual DbSet<CollaboratorDataModel> Collaborators { get; set; }
        public virtual DbSet<AssociationProjectCollaboratorDataModel> AssociationProjCollab { get; set; }

        public AbsanteeContext(DbContextOptions<AbsanteeContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // HolidayPlans
            modelBuilder.Entity<HolidayPlanDataModel>(entity =>
            {
                entity.HasKey(h => h.Id);

                entity.HasMany(h => h.HolidayPeriods);
            });

            // HolidayPeriods
            modelBuilder.Entity<HolidayPeriodDataModel>(entity =>
            {
                entity.HasKey(h => h.Id);
                entity.Property(h => h.Id).ValueGeneratedNever();

                entity.OwnsOne(p => p.PeriodDate);
            });

            // Collaborators
            modelBuilder.Entity<CollaboratorDataModel>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedNever();
                entity.OwnsOne(c => c.PeriodDateTime);
            });

            // Associations
            modelBuilder.Entity<AssociationProjectCollaboratorDataModel>(entity =>
            {
                entity.HasKey(a => a.Id);
                entity.OwnsOne(a => a.PeriodDate);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
