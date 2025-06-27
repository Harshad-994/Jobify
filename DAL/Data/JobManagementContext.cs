using DAL.Data.Enums;
using DAL.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DAL.Data;

public class JobManagementContext : DbContext
{
    public JobManagementContext(DbContextOptions<JobManagementContext> options) : base(options)
    {

    }

    public JobManagementContext()
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<JobPosting> JobPostings { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=JobManagement;Username=postgres;Password=Tatva@123");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Password).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Role).IsRequired().HasDefaultValue((int)Role.Candidate);
            entity.Property(e => e.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<JobCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<JobApplication>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.CoverLetter).HasMaxLength(2000);
            entity.Property(e => e.AppliedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Title).HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.CompanyName).HasMaxLength(100);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.SalaryRange).HasMaxLength(100);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Message).IsRequired().HasMaxLength(500);
            entity.Property(e => e.IsRead).IsRequired().HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.ReadAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<JobApplication>()
        .HasOne(jp => jp.JobPosting)
        .WithMany(jc => jc.JobApplications)
        .HasForeignKey(jp => jp.JobPostingId)
        .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobPosting>()
        .HasOne(jp => jp.JobCategory)
        .WithMany(jc => jc.JobPostings)
        .HasForeignKey(jp => jp.CategoryId)
        .OnDelete(DeleteBehavior.Cascade);
    }

}