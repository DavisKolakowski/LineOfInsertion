﻿// <auto-generated />
using System;
using LOI.Domain.NFXFileSystemEventWatcherManager.API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Migrations
{
    [DbContext(typeof(LineOfInsertionDbContext))]
    partial class LineOfInsertionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities.NFXFileSystemWorker", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Connection")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsConnected")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdateUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Machine")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.ToTable("NFXFileSystemWorkers");
                });

            modelBuilder.Entity("LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities.WatchFolder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("DateAddedUTC")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DateModifiedUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Filter")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("LastFileEventUTC")
                        .HasColumnType("datetime2");

                    b.Property<string>("Machine")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("Machine");

                    b.ToTable("WatchFolders");
                });

            modelBuilder.Entity("LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities.WatchFolder", b =>
                {
                    b.HasOne("LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities.NFXFileSystemWorker", "NFXFileSystemWorker")
                        .WithMany("WatchFolders")
                        .HasForeignKey("Machine")
                        .HasPrincipalKey("Machine")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("NFXFileSystemWorker");
                });

            modelBuilder.Entity("LOI.Domain.NFXFileSystemEventWatcherManager.API.Data.Entities.NFXFileSystemWorker", b =>
                {
                    b.Navigation("WatchFolders");
                });
#pragma warning restore 612, 618
        }
    }
}