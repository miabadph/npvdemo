using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NPVCalculatorDemo.Models
{
    public partial class DB_A4CAF7_npvContext : DbContext
    {
        public DB_A4CAF7_npvContext()
        {
        }

        public DB_A4CAF7_npvContext(DbContextOptions<DB_A4CAF7_npvContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CashflowTbl> CashflowTbl { get; set; }
        public virtual DbSet<NpvTbl> NpvTbl { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CashflowTbl>(entity =>
            {
                entity.ToTable("cashflow_tbl");

                entity.Property(e => e.CalculationId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.CashFlow).HasColumnType("text");
            });

            modelBuilder.Entity<NpvTbl>(entity =>
            {
                entity.ToTable("npv_tbl");

                entity.Property(e => e.CalculationId)
                    .IsRequired()
                    .HasMaxLength(14)
                    .IsUnicode(false);

                entity.Property(e => e.ComputedValue).HasColumnType("decimal(19, 4)");

                entity.Property(e => e.Rate).HasColumnType("decimal(5, 2)");
            });
        }
    }
}
