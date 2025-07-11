using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SportsCenterApi.Models;

public partial class SportsCenterContext : DbContext
{
    public SportsCenterContext()
    {
    }

    public SportsCenterContext(DbContextOptions<SportsCenterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Facility> Facilities { get; set; }

    public virtual DbSet<FacilitySchedule> FacilitySchedules { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("day_of_week_enum", new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" })
            .HasPostgresEnum("facility_type_enum", new[] { "Soccer", "Tennis", "Padel", "Basketball", "Pool", "Gym" });

        modelBuilder.Entity<Facility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("facilities_pkey");
          
            entity.Property(e => e.CancellationHours).HasDefaultValue(1);
            entity.Property(e => e.MinReservationHours).HasDefaultValue(1);
        });

        modelBuilder.Entity<FacilitySchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("facility_schedules_pkey");

            entity.HasOne(d => d.Facility).WithMany(p => p.FacilitySchedules).HasConstraintName("fk_facility_schedules_facility");
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("members_pkey");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Penalized).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithOne(p => p.Member).HasConstraintName("fk_members_user");
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(e => e.PermissionId).HasName("permissions_pkey");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("reservations_pkey");

            entity.Property(e => e.NoShow).HasDefaultValue(false);
            entity.Property(e => e.PaymentCompleted).HasDefaultValue(false);

            entity.HasOne(d => d.Facility).WithMany(p => p.Reservations).HasConstraintName("fk_reservations_facility");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations).HasConstraintName("fk_reservations_user");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("roles_pkey");

            entity.HasMany(d => d.Permissions).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RolePermission",
                    r => r.HasOne<Permission>().WithMany()
                        .HasForeignKey("PermissionId")
                        .HasConstraintName("fk_role_permissions_permission"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_role_permissions_role"),
                    j =>
                    {
                        j.HasKey("RoleId", "PermissionId").HasName("role_permissions_pkey");
                        j.ToTable("role_permissions");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                        j.IndexerProperty<int>("PermissionId").HasColumnName("permission_id");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("users_pkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_users_role");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
