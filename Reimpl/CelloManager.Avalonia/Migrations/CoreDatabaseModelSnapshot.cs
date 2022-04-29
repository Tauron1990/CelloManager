﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;
using CelloManager.Avalonia.Core.Comp;

namespace Tauron.Application.CelloManager.Data.Migrations
{
    [DbContext(typeof(CoreDatabase))]
    partial class CoreDatabaseModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ChangeDetector.SkipDetectChanges", "true")
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

            modelBuilder.Entity("CelloManager.Avalonia.Core.Comp.OldData.OptionEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Value");

                    b.Property<string>("key");

                    b.HasKey("Id");

                    b.ToTable("OptionEntries");
                });

            modelBuilder.Entity("CelloManager.Avalonia.Core.Comp.OldData.CommittedRefillEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CompledTime");

                    b.Property<bool>("IsCompleted");

                    b.Property<DateTime>("SentTime");

                    b.HasKey("Id");

                    b.ToTable("CommittedRefills");
                });

            modelBuilder.Entity("CelloManager.Avalonia.Core.Comp.OldData.CommittedSpoolEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CommittedRefillEntityId");

                    b.Property<string>("Name");

                    b.Property<int>("OrderedCount");

                    b.Property<int>("SpoolId");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.HasIndex("CommittedRefillEntityId");

                    b.ToTable("CommittedSpoolEntity");
                });

            modelBuilder.Entity("CelloManager.Avalonia.Core.Comp.OldData.CelloSpoolEntity", b =>
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

            modelBuilder.Entity("CelloManager.Avalonia.Core.Comp.OldData.CommittedSpoolEntity", b =>
                {
                    b.HasOne("CelloManager.Avalonia.Core.Comp.OldData.CommittedRefillEntity")
                        .WithMany("CommitedSpools")
                        .HasForeignKey("CommittedRefillEntityId");
                });
#pragma warning restore 612, 618
        }
    }
}
