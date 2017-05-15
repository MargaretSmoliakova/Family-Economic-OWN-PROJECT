namespace ClassLibraryEF
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using ClassLibraryEF.Models;

    public partial class FamilyEconomicEntities : DbContext
    {
        public FamilyEconomicEntities()
            : base("name=FamilyEconomicEntities")
        {
        }

        public virtual DbSet<CurrentPrice> CurrentPrices { get; set; }
        public virtual DbSet<MonthlyPrice> MonthlyPrices { get; set; }
        public virtual DbSet<PermanentPrice> PermanentPrices { get; set; }
        public virtual DbSet<UserData> UserDatas { get; set; }
        public virtual DbSet<UserInfo> UserInfoes { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MonthlyPrice>()
                .HasMany(e => e.CurrentPrices)
                .WithOptional(e => e.MonthlyPrice)
                .HasForeignKey(e => e.Id_monthly_prices)
                .WillCascadeOnDelete();

            modelBuilder.Entity<PermanentPrice>()
                .HasMany(e => e.CurrentPrices)
                .WithOptional(e => e.PermanentPrice)
                .HasForeignKey(e => e.Id_permanent_prices)
                .WillCascadeOnDelete();
        }
    }
}
