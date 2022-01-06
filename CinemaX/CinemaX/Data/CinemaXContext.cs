using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using CinemaX.Models;

#nullable disable

namespace CinemaX.Data
{
    public partial class CinemaXContext : DbContext
    {
        public CinemaXContext()
        {
        }

        public CinemaXContext(DbContextOptions<CinemaXContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bilhete> Bilhetes { get; set; }
        public virtual DbSet<CategoriasFavorita> CategoriasFavoritas { get; set; }
        public virtual DbSet<CategoriasFilme> CategoriasFilmes { get; set; }
        public virtual DbSet<Categorium> Categoria { get; set; }
        public virtual DbSet<Filme> Filmes { get; set; }
        public virtual DbSet<GrupoPermisso> GrupoPermissoes { get; set; }
        public virtual DbSet<ListaPermisso> ListaPermissoes { get; set; }
        public virtual DbSet<Perfil> Perfils { get; set; }
        public virtual DbSet<Permisso> Permissoes { get; set; }
        public virtual DbSet<Sala> Salas { get; set; }
        public virtual DbSet<Sessao> Sessaos { get; set; }
        public virtual DbSet<Utilizador> Utilizadors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("name=CinemaXContext");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Bilhete>(entity =>
            {
                entity.HasKey(e => new { e.IdSessao, e.IdUtilizador, e.NumBilhete })
                    .HasName("PK__Bilhete__CCF7906203084CBD");

                entity.Property(e => e.NumBilhete).ValueGeneratedOnAdd();

                entity.HasOne(d => d.IdSessaoNavigation)
                    .WithMany(p => p.Bilhetes)
                    .HasForeignKey(d => d.IdSessao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bilhete__IdSessa__44FF419A");

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.Bilhetes)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Bilhete__IdUtili__440B1D61");
            });

            modelBuilder.Entity<CategoriasFavorita>(entity =>
            {
                entity.HasKey(e => new { e.IdUtilizador, e.IdCategoria })
                    .HasName("PK__Categori__602B0FD5AB8C0EA7");

                entity.HasOne(d => d.IdCategoriaNavigation)
                    .WithMany(p => p.CategoriasFavorita)
                    .HasForeignKey(d => d.IdCategoria)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Categoria__IdCat__398D8EEE");

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithMany(p => p.CategoriasFavorita)
                    .HasForeignKey(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Categoria__IdUti__3A81B327");
            });

            modelBuilder.Entity<CategoriasFilme>(entity =>
            {
                entity.HasKey(e => new { e.IdFilme, e.IdCategoria })
                    .HasName("PK__Categori__74B328D779BE9F8D");

                entity.HasOne(d => d.IdCategoriaNavigation)
                    .WithMany(p => p.CategoriasFilmes)
                    .HasForeignKey(d => d.IdCategoria)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Categoria__IdCat__36B12243");

                entity.HasOne(d => d.IdFilmeNavigation)
                    .WithMany(p => p.CategoriasFilmes)
                    .HasForeignKey(d => d.IdFilme)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Categoria__IdFil__35BCFE0A");
            });

            modelBuilder.Entity<Categorium>(entity =>
            {
                entity.HasKey(e => e.IdCategoria)
                    .HasName("PK__Categori__A3C02A10BADA8F3E");

                entity.Property(e => e.Nome).IsUnicode(false);
            });

            modelBuilder.Entity<Filme>(entity =>
            {
                entity.HasKey(e => e.IdFilme)
                    .HasName("PK__Filme__6E8F2A76B84828DB");

                entity.Property(e => e.DataAdicionado).HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Descrição).IsUnicode(false);

                entity.Property(e => e.Foto).IsUnicode(false);

                entity.Property(e => e.LinkTrailer).IsUnicode(false);

                entity.Property(e => e.Nome).IsUnicode(false);

                entity.Property(e => e.Realizador).IsUnicode(false);
            });

            modelBuilder.Entity<GrupoPermisso>(entity =>
            {
                entity.HasKey(e => e.IdGrupo)
                    .HasName("PK__GrupoPer__303F6FD9CAA9D64B");

                entity.Property(e => e.NomeGrupo).IsUnicode(false);
            });

            modelBuilder.Entity<ListaPermisso>(entity =>
            {
                entity.HasKey(e => new { e.IdGrupo, e.IdPermissao })
                .HasName("PK__ListaPer__83699CC0A13190B1");


                entity.HasOne(d => d.IdGrupoNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdGrupo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ListaPerm__IdGru__267ABA7A");

                entity.HasOne(d => d.IdPermissaoNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdPermissao)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ListaPerm__IdPer__276EDEB3");
            });

            modelBuilder.Entity<Perfil>(entity =>
            {
                entity.HasKey(e => e.IdUtilizador)
                    .HasName("PK__Perfil__7A170D74A4059108");

                entity.Property(e => e.IdUtilizador).ValueGeneratedNever();

                entity.Property(e => e.Email).IsUnicode(false);

                entity.Property(e => e.Nome).IsUnicode(false);

                entity.HasOne(d => d.IdUtilizadorNavigation)
                    .WithOne(p => p.Perfil)
                    .HasForeignKey<Perfil>(d => d.IdUtilizador)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Perfil__IdUtiliz__2E1BDC42");
            });

            modelBuilder.Entity<Permisso>(entity =>
            {
                entity.HasKey(e => e.IdPermissao)
                    .HasName("PK__Permisso__356F319A9BD8471C");

                entity.Property(e => e.NomePermissao).IsUnicode(false);
            });

            modelBuilder.Entity<Sala>(entity =>
            {
                entity.HasKey(e => e.Numero)
                    .HasName("PK__Sala__7E532BC78B29FAF9");

                entity.Property(e => e.DataAdicionada).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Sessao>(entity =>
            {
                entity.HasKey(e => e.IdSessao)
                    .HasName("PK__Sessao__4641A64DAB9CB1D6");

                entity.HasOne(d => d.IdFilmeNavigation)
                    .WithMany(p => p.Sessaos)
                    .HasForeignKey(d => d.IdFilme)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sessao__IdFilme__403A8C7D");

                entity.HasOne(d => d.NumeroNavigation)
                    .WithMany(p => p.Sessaos)
                    .HasForeignKey(d => d.Numero)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Sessao__Numero__412EB0B6");
            });

            modelBuilder.Entity<Utilizador>(entity =>
            {
                entity.HasKey(e => e.IdUtilizador)
                    .HasName("PK__Utilizad__7A170D74F94A2D4E");

                entity.Property(e => e.UserName).IsUnicode(false);

                entity.Property(e => e.UserPassWord).IsUnicode(false);

                entity.HasOne(d => d.IdGrupoNavigation)
                    .WithMany(p => p.Utilizadors)
                    .HasForeignKey(d => d.IdGrupo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Utilizado__IdGru__2A4B4B5E");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}

