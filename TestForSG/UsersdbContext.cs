﻿using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using TestForSG.Models;

namespace TestForSG;

public partial class UsersdbContext : DbContext
{
    private readonly bool debug;
    private readonly string connectionString;

    public UsersdbContext(string connString, bool debug = false)
    {
        if (connString != null)
        {
            this.connectionString = connString;
        }
        if (debug)
        {
            Database.EnsureDeleted();
        }
        Database.EnsureCreated();
        this.debug = debug;


    }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<JobTitle> JobTitles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (debug)
        {
            optionsBuilder.EnableSensitiveDataLogging(true);
        }
        if (connectionString != null)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }
        //optionsBuilder.LogTo(System.Console.WriteLine);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Department>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ManagerId).HasColumnName("ManagerID");
            entity.Property(e => e.ParentId).HasColumnName("ParentID");

            entity.HasOne(d => d.Manager).WithMany(p => p.Departments)
                .HasForeignKey(d => d.ManagerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("departments_employees_fk");
                //.IsRequired();

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent)
                .HasForeignKey(d => d.ParentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("departments_departments_fk");
                //.IsRequired();
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");

            entity.HasOne(d => d.DepartmentNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Department)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employees_departments_fk");

            entity.HasOne(d => d.JobTitleNavigation).WithMany(p => p.Employees)
                .HasForeignKey(d => d.JobTitle)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employees_jobtitles_fk");
        });

        modelBuilder.Entity<JobTitle>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("ID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
