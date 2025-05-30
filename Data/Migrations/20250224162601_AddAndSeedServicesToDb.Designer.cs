﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ReservationApp.Data;

#nullable disable

namespace ReservationApp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250224162601_AddAndSeedServicesToDb")]
    partial class AddAndSeedServicesToDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ReservationApp.Models.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Tattoo"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Barber"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Physiotherapy"
                        });
                });

            modelBuilder.Entity("ReservationApp.Models.Company", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Zip")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Companies");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Address = "1234 Tattoo St",
                            CategoryId = 1,
                            City = "Tattoo City",
                            Description = "Tattoo Description",
                            Email = "ink@gmail.com",
                            Name = "Ink Master",
                            Phone = "123-456-7890",
                            State = "Tattoo State",
                            Zip = "12345"
                        },
                        new
                        {
                            Id = 2,
                            Address = "1234 Barber St",
                            CategoryId = 2,
                            City = "Barber City",
                            Description = "Barber Shop Description",
                            Email = "barber@gmail.com",
                            Name = "Barber Shop",
                            Phone = "123-456-7890",
                            State = "Barber State",
                            Zip = "12345"
                        },
                        new
                        {
                            Id = 3,
                            Address = "1234 Physiotherapy St",
                            CategoryId = 3,
                            City = "Physiotherapy City",
                            Description = "Physiotherapy Clinic Description",
                            Email = "physio@gmail.com",
                            Name = "Physiotherapy Clinic",
                            Phone = "123-456-7890",
                            State = "Physiotherapy State",
                            Zip = "12345"
                        });
                });

            modelBuilder.Entity("ReservationApp.Models.Service", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("CompanyId")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<TimeSpan>("DurationMinutes")
                        .HasColumnType("time");

                    b.Property<bool>("IsPrepaymentRequired")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("Id");

                    b.HasIndex("CompanyId");

                    b.ToTable("Services");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CompanyId = 1,
                            Description = "Tattoo Service Description",
                            DurationMinutes = new TimeSpan(0, 0, 30, 0, 0),
                            IsPrepaymentRequired = true,
                            Name = "Tattoo Service 1",
                            Price = 100m
                        },
                        new
                        {
                            Id = 2,
                            CompanyId = 1,
                            Description = "Tattoo Service Description",
                            DurationMinutes = new TimeSpan(0, 0, 30, 0, 0),
                            IsPrepaymentRequired = true,
                            Name = "Tattoo Service 2",
                            Price = 200m
                        },
                        new
                        {
                            Id = 3,
                            CompanyId = 1,
                            Description = "Tattoo Service Description",
                            DurationMinutes = new TimeSpan(0, 0, 30, 0, 0),
                            IsPrepaymentRequired = true,
                            Name = "Tattoo Service 2",
                            Price = 200m
                        },
                        new
                        {
                            Id = 4,
                            CompanyId = 2,
                            Description = "Barber Service Description",
                            DurationMinutes = new TimeSpan(0, 0, 30, 0, 0),
                            IsPrepaymentRequired = true,
                            Name = "Barber Service 1",
                            Price = 50m
                        },
                        new
                        {
                            Id = 5,
                            CompanyId = 2,
                            Description = "Barber Service Description",
                            DurationMinutes = new TimeSpan(0, 0, 45, 0, 0),
                            IsPrepaymentRequired = true,
                            Name = "Barber Service 2",
                            Price = 75m
                        },
                        new
                        {
                            Id = 6,
                            CompanyId = 3,
                            Description = "Physiotherapy Service Description",
                            DurationMinutes = new TimeSpan(0, 1, 0, 0, 0),
                            IsPrepaymentRequired = true,
                            Name = "Physiotherapy Service 1",
                            Price = 150m
                        },
                        new
                        {
                            Id = 7,
                            CompanyId = 3,
                            Description = "Physiotherapy Service Description",
                            DurationMinutes = new TimeSpan(0, 0, 45, 0, 0),
                            IsPrepaymentRequired = true,
                            Name = "Physiotherapy Service 2",
                            Price = 200m
                        });
                });

            modelBuilder.Entity("ReservationApp.Models.Company", b =>
                {
                    b.HasOne("ReservationApp.Models.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("ReservationApp.Models.Service", b =>
                {
                    b.HasOne("ReservationApp.Models.Company", "Company")
                        .WithMany()
                        .HasForeignKey("CompanyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Company");
                });
#pragma warning restore 612, 618
        }
    }
}
