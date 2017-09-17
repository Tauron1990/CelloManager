﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using Tauron.Application.CelloManager.Data.Core;

namespace Tauron.Application.CelloManager.Data.Migrations
{
    [DbContext(typeof(CoreDatabase))]
    partial class CoreDatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.0-rtm-26452");

            modelBuilder.Entity("Tauron.Application.CelloManager.Data.Core.CelloSpoolEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Amount");

                    b.Property<string>("Name");

                    b.Property<int>("Neededamount");

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("CelloSpools");
                });

            modelBuilder.Entity("Tauron.Application.CelloManager.Data.Core.OptionEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Value");

                    b.Property<string>("key");

                    b.HasKey("Id");

                    b.ToTable("OptionEntries");
                });

            modelBuilder.Entity("Tauron.Application.CelloManager.Data.Historie.CommittedRefill", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("SentTime");

                    b.HasKey("Id");

                    b.ToTable("CommittedRefills");
                });

            modelBuilder.Entity("Tauron.Application.CelloManager.Data.Historie.CommittedSpool", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CommittedRefillId");

                    b.Property<string>("Name");

                    b.Property<int>("OrderedCount");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CommittedRefillId");

                    b.ToTable("CommittedSpool");
                });

            modelBuilder.Entity("Tauron.Application.CelloManager.Data.Historie.CommittedSpool", b =>
                {
                    b.HasOne("Tauron.Application.CelloManager.Data.Historie.CommittedRefill")
                        .WithMany("CommitedSpools")
                        .HasForeignKey("CommittedRefillId");
                });
#pragma warning restore 612, 618
        }
    }
}
