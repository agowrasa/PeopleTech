using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TimeSheetAuthAPI.Models;

public partial class DevContext : DbContext
{
    public DevContext()
    {
    }

    public DevContext(DbContextOptions<DevContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Employee> Employees { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
////#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("DbConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Employee>

        (entity =>
        {
            entity.HasKey(e => e.EmployeeKey).HasName("PK__Employee__EmployeeKey");

            entity.ToTable("Employee", "PTG");

            entity.Property(e => e.CreatedBy).HasMaxLength(255);
            entity.Property(e => e.EmployeeAlternateEmail).HasMaxLength(255);
            entity.Property(e => e.EmployeeDesignation).HasMaxLength(255);
            entity.Property(e => e.EmployeeEmail).HasMaxLength(255);
            entity.Property(e => e.EmployeeFirstName).HasMaxLength(255);
            entity.Property(e => e.EmployeeHireDate).HasColumnType("date");
            entity.Property(e => e.EmployeeLastName).HasMaxLength(255);
            entity.Property(e => e.UpdatedBy).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
