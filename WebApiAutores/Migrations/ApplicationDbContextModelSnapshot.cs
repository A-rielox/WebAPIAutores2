﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WebApiAutores;

#nullable disable

namespace WebApiAutores.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("WebApiAutores.Entidades.Autor", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Nombre")
                        .IsRequired()
                        .HasMaxLength(120)
                        .HasColumnType("nvarchar(120)");

                    b.HasKey("Id");

                    b.ToTable("Autores");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.AutorLibro", b =>
                {
                    b.Property<int>("AutorId")
                        .HasColumnType("int");

                    b.Property<int>("LibroId")
                        .HasColumnType("int");

                    b.Property<int>("Orden")
                        .HasColumnType("int");

                    b.HasKey("AutorId", "LibroId");

                    b.HasIndex("LibroId");

                    b.ToTable("AutoresLibros");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Comentario", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Contenido")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LibroId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("LibroId");

                    b.ToTable("Comentarios");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Libro", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.HasKey("Id");

                    b.ToTable("Libros");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.AutorLibro", b =>
                {
                    b.HasOne("WebApiAutores.Entidades.Autor", "Autor")
                        .WithMany("AutoresLibros")
                        .HasForeignKey("AutorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApiAutores.Entidades.Libro", "Libro")
                        .WithMany("AutoresLibros")
                        .HasForeignKey("LibroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Autor");

                    b.Navigation("Libro");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Comentario", b =>
                {
                    b.HasOne("WebApiAutores.Entidades.Libro", "Libro")
                        .WithMany("Comentarios")
                        .HasForeignKey("LibroId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Libro");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Autor", b =>
                {
                    b.Navigation("AutoresLibros");
                });

            modelBuilder.Entity("WebApiAutores.Entidades.Libro", b =>
                {
                    b.Navigation("AutoresLibros");

                    b.Navigation("Comentarios");
                });
#pragma warning restore 612, 618
        }
    }
}
