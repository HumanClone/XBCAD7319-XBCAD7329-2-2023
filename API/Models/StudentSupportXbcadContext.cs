using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api.Models;

public partial class StudentSupportXbcadContext : DbContext
{
    public StudentSupportXbcadContext()
    {
    }

    public StudentSupportXbcadContext(DbContextOptions<StudentSupportXbcadContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<TeamDev> TeamDevs { get; set; }

    public virtual DbSet<TicketDetail> TicketDetails { get; set; }

    public virtual DbSet<TicketResponse> TicketResponses { get; set; }

    public virtual DbSet<UserInfo> UserInfos { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

        => optionsBuilder.UseSqlServer("Server=tcp:xbcad-server.database.windows.net,1433;Initial Catalog=StudentSupport_XBCAD;Persist Security Info=False;User ID=admin@st10083751@xbcad-server;Password=yusra@24;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TeamDev>(entity =>
        {
            entity.HasKey(e => e.DevId);
        });

        modelBuilder.Entity<TicketDetail>(entity =>
        {
            entity.HasKey(e => e.TicketId);
        });

        modelBuilder.Entity<TicketResponse>(entity =>
        {
            entity.HasKey(e => e.ResponseId);

            entity.Property(e => e.date).HasColumnName("date");
            entity.Property(e => e.sender).HasColumnName("sender");
            entity.Property(e => e.TicketId).IsRequired();
        });

        modelBuilder.Entity<UserInfo>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.ToTable("UserInfo");

            entity.Property(e => e.Email).IsRequired();
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasKey(e => e.Email);

            entity.ToTable("UserLogin");

            entity.Property(e => e.Password).IsRequired();
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
