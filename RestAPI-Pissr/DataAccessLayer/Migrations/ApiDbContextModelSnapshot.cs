﻿// <auto-generated />
using System;
using DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(ApiDbContext))]
    partial class ApiDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.15")
                .HasAnnotation("Proxies:ChangeTracking", false)
                .HasAnnotation("Proxies:CheckEquality", false)
                .HasAnnotation("Proxies:LazyLoading", true)
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("DataAccessLayer.Entities.AziendaAgricola", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Indirizzo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AziendeAgricole");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.AziendaIdrica", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Indirizzo")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AziendeIdriche");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Campo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Ac")
                        .HasColumnType("real");

                    b.Property<bool>("Adattivo")
                        .HasColumnType("boolean");

                    b.Property<int>("Ae")
                        .HasColumnType("integer");

                    b.Property<int>("Altitudine")
                        .HasColumnType("integer");

                    b.Property<bool>("Attivo")
                        .HasColumnType("boolean");

                    b.Property<int>("Aws")
                        .HasColumnType("integer");

                    b.Property<int>("AziendaAgricolaId")
                        .HasColumnType("integer");

                    b.Property<int>("Ettari")
                        .HasColumnType("integer");

                    b.Property<float>("Fr")
                        .HasColumnType("real");

                    b.Property<double>("Hum")
                        .HasColumnType("double precision");

                    b.Property<double>("Kc")
                        .HasColumnType("double precision");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Rd")
                        .HasColumnType("double precision");

                    b.Property<float>("S1")
                        .HasColumnType("real");

                    b.Property<float>("S2")
                        .HasColumnType("real");

                    b.Property<double?>("SoilMoisture")
                        .HasColumnType("double precision");

                    b.Property<string>("TipoSistemaIrrigazione")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("AziendaAgricolaId");

                    b.ToTable("Campi");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Contratto", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<float>("Acqua")
                        .HasColumnType("real");

                    b.Property<int>("AziendaAgricolaId")
                        .HasColumnType("integer");

                    b.Property<int>("AziendaIdricaId")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("Fine")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Stato")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Tempo")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AziendaAgricolaId");

                    b.HasIndex("AziendaIdricaId");

                    b.ToTable("Contratti");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Dispositivo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CampoId")
                        .HasColumnType("integer");

                    b.Property<int>("Tipo")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CampoId");

                    b.ToTable("Dispositivi");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Evento", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CampoId")
                        .HasColumnType("integer");

                    b.Property<int>("DispositivoId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Tempo")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Tipo")
                        .HasColumnType("integer");

                    b.Property<string>("Valore")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CampoId");

                    b.HasIndex("DispositivoId");

                    b.ToTable("Eventi");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Campo", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.AziendaAgricola", "AziendaAgricola")
                        .WithMany("Campi")
                        .HasForeignKey("AziendaAgricolaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AziendaAgricola");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Contratto", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.AziendaAgricola", "AziendaAgricola")
                        .WithMany()
                        .HasForeignKey("AziendaAgricolaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entities.AziendaIdrica", "AziendaIdrica")
                        .WithMany()
                        .HasForeignKey("AziendaIdricaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("AziendaAgricola");

                    b.Navigation("AziendaIdrica");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Dispositivo", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.Campo", "Campo")
                        .WithMany("Dispositivi")
                        .HasForeignKey("CampoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campo");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Evento", b =>
                {
                    b.HasOne("DataAccessLayer.Entities.Campo", "Campo")
                        .WithMany("Eventi")
                        .HasForeignKey("CampoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DataAccessLayer.Entities.Dispositivo", "Dispositivo")
                        .WithMany()
                        .HasForeignKey("DispositivoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campo");

                    b.Navigation("Dispositivo");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.AziendaAgricola", b =>
                {
                    b.Navigation("Campi");
                });

            modelBuilder.Entity("DataAccessLayer.Entities.Campo", b =>
                {
                    b.Navigation("Dispositivi");

                    b.Navigation("Eventi");
                });
#pragma warning restore 612, 618
        }
    }
}