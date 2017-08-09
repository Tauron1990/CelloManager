using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Tauron.Application.CelloManager.Data.Core;

namespace Tauron.Application.CelloManager.Data.Migrations
{
    [DbContext(typeof(CoreDatabase))]
    [Migration("20170805175327_Version1")]
    partial class Version1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2");

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
        }
    }
}
