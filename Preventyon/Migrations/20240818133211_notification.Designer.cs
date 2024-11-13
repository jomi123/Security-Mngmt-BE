﻿// <auto-generated />
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Preventyon.Data;

#nullable disable

namespace Preventyon.Migrations
{
    [DbContext(typeof(ApiContext))]
    [Migration("20240818133211_notification")]
    partial class notification
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Preventyon.Models.Admin", b =>
                {
                    b.Property<int>("AdminId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("AdminId"));

                    b.Property<int>("AssignedBy")
                        .HasColumnType("integer");

                    b.Property<DateTime>("AssignedOn")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<bool>("Status")
                        .HasColumnType("boolean");

                    b.HasKey("AdminId");

                    b.HasIndex("EmployeeId")
                        .IsUnique();

                    b.ToTable("Admins");
                });

            modelBuilder.Entity("Preventyon.Models.AssignedIncidents", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("Accepted")
                        .HasColumnType("integer");

                    b.Property<string>("AssignedTo")
                        .HasColumnType("text");

                    b.Property<int>("IncidentId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("IncidentId");

                    b.ToTable("AssignedIncidents");
                });

            modelBuilder.Entity("Preventyon.Models.Employee", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Department")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Designation")
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<int>("RoleId")
                        .HasMaxLength(100)
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("Preventyon.Models.Incident", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ActionAssignedTo")
                        .HasColumnType("text");

                    b.Property<string>("AssociatedImpacts")
                        .HasColumnType("text");

                    b.Property<string>("Category")
                        .HasColumnType("text");

                    b.Property<string>("CollectionOfEvidence")
                        .HasColumnType("text");

                    b.Property<string>("Correction")
                        .HasColumnType("text");

                    b.Property<DateTime>("CorrectionActualCompletionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("CorrectionCompletionTargetDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("CorrectionDetailsTimeTakenToCloseIncident")
                        .HasColumnType("numeric");

                    b.Property<string>("CorrectiveAction")
                        .HasColumnType("text");

                    b.Property<DateTime>("CorrectiveActualCompletionDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("CorrectiveDetailsTimeTakenToCloseIncident")
                        .HasColumnType("numeric");

                    b.Property<string>("DeptOfAssignee")
                        .HasColumnType("text");

                    b.Property<List<string>>("DocumentUrls")
                        .HasColumnType("text[]");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<string>("IncidentDescription")
                        .HasColumnType("text");

                    b.Property<string>("IncidentNo")
                        .HasColumnType("text");

                    b.Property<DateTime>("IncidentOccuredDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("IncidentStatus")
                        .HasColumnType("text");

                    b.Property<string>("IncidentTitle")
                        .HasColumnType("text");

                    b.Property<string>("IncidentType")
                        .HasColumnType("text");

                    b.Property<string>("InvestigationDetails")
                        .HasColumnType("text");

                    b.Property<bool>("IsDraft")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsSubmittedForReview")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("MonthYear")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Priority")
                        .HasColumnType("text");

                    b.Property<string>("ReportedBy")
                        .HasColumnType("text");

                    b.Property<string>("RoleOfReporter")
                        .HasColumnType("text");

                    b.Property<DateTime>("createdAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Incident");
                });

            modelBuilder.Entity("Preventyon.Models.Notification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("EmployeeId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("EmployeeId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("Preventyon.Models.Permission", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IncidentCreateOnly")
                        .HasColumnType("boolean");

                    b.Property<bool>("IncidentManagement")
                        .HasColumnType("boolean");

                    b.Property<string>("PermissionName")
                        .HasColumnType("text");

                    b.Property<bool>("UserManagement")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Permission");
                });

            modelBuilder.Entity("Preventyon.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<int>("PermissionID")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PermissionID")
                        .IsUnique();

                    b.ToTable("Role");
                });

            modelBuilder.Entity("Preventyon.Models.Admin", b =>
                {
                    b.HasOne("Preventyon.Models.Employee", "Employee")
                        .WithOne("Admin")
                        .HasForeignKey("Preventyon.Models.Admin", "EmployeeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Preventyon.Models.AssignedIncidents", b =>
                {
                    b.HasOne("Preventyon.Models.Incident", "Incident")
                        .WithMany("AssignedIncidents")
                        .HasForeignKey("IncidentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Incident");
                });

            modelBuilder.Entity("Preventyon.Models.Employee", b =>
                {
                    b.HasOne("Preventyon.Models.Role", "Role")
                        .WithMany("Employees")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Preventyon.Models.Incident", b =>
                {
                    b.HasOne("Preventyon.Models.Employee", null)
                        .WithMany("Incident")
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Preventyon.Models.Notification", b =>
                {
                    b.HasOne("Preventyon.Models.Employee", "Employee")
                        .WithMany()
                        .HasForeignKey("EmployeeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Employee");
                });

            modelBuilder.Entity("Preventyon.Models.Role", b =>
                {
                    b.HasOne("Preventyon.Models.Permission", "Permission")
                        .WithOne("Role")
                        .HasForeignKey("Preventyon.Models.Role", "PermissionID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");
                });

            modelBuilder.Entity("Preventyon.Models.Employee", b =>
                {
                    b.Navigation("Admin");

                    b.Navigation("Incident");
                });

            modelBuilder.Entity("Preventyon.Models.Incident", b =>
                {
                    b.Navigation("AssignedIncidents");
                });

            modelBuilder.Entity("Preventyon.Models.Permission", b =>
                {
                    b.Navigation("Role");
                });

            modelBuilder.Entity("Preventyon.Models.Role", b =>
                {
                    b.Navigation("Employees");
                });
#pragma warning restore 612, 618
        }
    }
}
