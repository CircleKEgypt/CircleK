using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CK.Models;

public partial class EasysoftContext : DbContext
{
    public EasysoftContext()
    {
    }

    public EasysoftContext(DbContextOptions<EasysoftContext> options)
        : base(options)
    {
    }

    public virtual DbSet<SalesInvTran> SalesInvTrans { get; set; }

    public  DbSet<SalesInvoice> SalesInvoices { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=192.168.1.76\\sql2016;User ID=mohamed;Password=P@ssw0rd12345;Database=Easysoft;Connect Timeout=150;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SalesInvTran>(entity =>
        {
            entity.HasKey(o => new { o.SortFi, o.Sn }).IsClustered(false);
            entity.ToTable("Sales_InvTrans", tb => tb.HasTrigger("TR_ITEM_LOG"));

            entity.Property(e => e.BranchId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.ChangedId)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.CreateBranchId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.CstId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.DiscTax).HasColumnName("Disc_Tax");
            entity.Property(e => e.DocNo)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.EdafaTax).HasColumnName("Edafa_Tax");
            entity.Property(e => e.ExpDate)
                .HasDefaultValueSql("('')")
                .HasColumnType("datetime");
            entity.Property(e => e.F9qty).HasColumnName("F9Qty");
            entity.Property(e => e.ItemNotes)
                .HasMaxLength(2000)
                .HasDefaultValue("");
            entity.Property(e => e.NodeId)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.OrderBranchId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.OrderDocNo)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.ProdDate)
                .HasDefaultValueSql("('')")
                .HasColumnType("datetime");
            entity.Property(e => e.ProdId)
                .HasMaxLength(100)
                .HasDefaultValue("");
            entity.Property(e => e.ProdOtherId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.ProdOtherName)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.ProdSerialId)
                .HasMaxLength(200)
                .HasDefaultValue("");
            entity.Property(e => e.RelatedSn).HasColumnName("RelatedSN");
            entity.Property(e => e.SelComm).HasColumnName("Sel_Comm");
            entity.Property(e => e.SerialId)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.Sn).HasColumnName("SN");
            entity.Property(e => e.StampingTax).HasColumnName("Stamping_Tax");
            entity.Property(e => e.StoreId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.TrType).HasColumnName("Tr_Type");
            entity.Property(e => e.UnitId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
        });

        modelBuilder.Entity<SalesInvoice>(entity =>
        {
            entity.HasKey(e => new { e.SortFi, e.BranchId }).HasName("aaaaaInvoice_PK");

            entity.ToTable("Sales_Invoice");

            entity.Property(e => e.BranchId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.AccCode)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.BankId)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.ChangeInWords)
                .HasMaxLength(500)
                .HasDefaultValue("");
            entity.Property(e => e.ClassId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.CreateBranchId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((1))");
            entity.Property(e => e.CstId)
                .HasMaxLength(30)
                .HasDefaultValue("");
            entity.Property(e => e.CurrId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.CustId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.DocDate)
                .HasDefaultValueSql("('')")
                .HasColumnType("datetime");
            entity.Property(e => e.DocNo)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.DocTime)
                .HasDefaultValueSql("('')")
                .HasColumnType("datetime");
            entity.Property(e => e.Documents)
                .HasMaxLength(2000)
                .HasDefaultValue("");
            entity.Property(e => e.DueDate)
                .HasDefaultValueSql("('')")
                .HasColumnType("datetime");
            entity.Property(e => e.LrbranchId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((1))")
                .HasColumnName("LRBranchId");
            entity.Property(e => e.Lrno)
                .HasMaxLength(30)
                .HasDefaultValue("")
                .HasColumnName("LRNO");
            entity.Property(e => e.Lrsortfi).HasColumnName("LRSortfi");
            entity.Property(e => e.MTax).HasColumnName("M_Tax");
            entity.Property(e => e.Notes)
                .HasMaxLength(2000)
                .HasDefaultValue("");
            entity.Property(e => e.PartInWords)
                .HasMaxLength(500)
                .HasDefaultValue("");
            entity.Property(e => e.Payment)
                .HasMaxLength(2000)
                .HasDefaultValue("");
            entity.Property(e => e.SerialId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.StaffId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.StockDoc)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
            entity.Property(e => e.SumAmt).HasDefaultValue(0.0);
            entity.Property(e => e.SumEdafaTax)
                .HasDefaultValue(0.0)
                .HasColumnName("SumEdafa_Tax");
            entity.Property(e => e.TotInWords)
                .HasMaxLength(255)
                .HasDefaultValue("");
            entity.Property(e => e.TrType).HasColumnName("Tr_Type");
            entity.Property(e => e.Uuid)
                .HasMaxLength(50)
                .HasDefaultValue("")
                .HasColumnName("UUID");
            entity.Property(e => e.VisaId)
                .HasMaxLength(30)
                .HasDefaultValueSql("((0))");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
