using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CK.Models.TopSoft;

public partial class TopSoftContext : DbContext
{
    public TopSoftContext()
    {
    }

    public TopSoftContext(DbContextOptions<TopSoftContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CloseDay> CloseDays { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.1.208\\New;User ID=sa;Password=P@ssw0rd;Database=TopSoft;Connect Timeout=150;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CloseDay>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("CloseDay");

            entity.Property(e => e.Alert).HasMaxLength(500);
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
