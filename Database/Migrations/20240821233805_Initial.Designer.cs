﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Stopwatch;

#nullable disable

namespace Stopwatch.Database.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20240821233805_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Stopwatch.Domain.TimedSession", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<DateTimeOffset?>("EndTime")
                        .HasColumnType("datetimeoffset");

                    b.Property<Guid>("ParticipantId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("StartTime")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.HasIndex("ParticipantId");

                    b.ToTable("TimedSessions");
                });

            modelBuilder.Entity("Stopwatch.Domain.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<DateTimeOffset>("UpdatedOn")
                        .HasColumnType("datetimeoffset");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Stopwatch.Domain.UserRole", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTimeOffset>("CreatedOn")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("nvarchar(200)");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserRoles");

                    b.HasDiscriminator<string>("RoleName").HasValue("UserRole");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("Stopwatch.Domain.Administrator", b =>
                {
                    b.HasBaseType("Stopwatch.Domain.UserRole");

                    b.HasDiscriminator().HasValue("Administrator");
                });

            modelBuilder.Entity("Stopwatch.Domain.Participant", b =>
                {
                    b.HasBaseType("Stopwatch.Domain.UserRole");

                    b.HasDiscriminator().HasValue("Participant");
                });

            modelBuilder.Entity("Stopwatch.Domain.TimedSession", b =>
                {
                    b.HasOne("Stopwatch.Domain.Participant", "Participant")
                        .WithMany("TimedSessions")
                        .HasForeignKey("ParticipantId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Participant");
                });

            modelBuilder.Entity("Stopwatch.Domain.UserRole", b =>
                {
                    b.HasOne("Stopwatch.Domain.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Stopwatch.Domain.User", b =>
                {
                    b.Navigation("Roles");
                });

            modelBuilder.Entity("Stopwatch.Domain.Participant", b =>
                {
                    b.Navigation("TimedSessions");
                });
#pragma warning restore 612, 618
        }
    }
}
