using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    [Table("Filme")]
    public partial class Filme
    {
        public Filme()
        {
            CategoriasFilmes = new HashSet<CategoriasFilme>();
            Sessaos = new HashSet<Sessao>();
        }

        [Key]
        public int IdFilme { get; set; }
        [Required]
        [StringLength(50)]
        public string Nome { get; set; }
        [Required]
        [StringLength(200)]
        public string Foto { get; set; }
        [Required]
        [StringLength(50)]
        public string Realizador { get; set; }
        [Column("Data_", TypeName = "date")]
        public DateTime Data { get; set; }
        [Required]
        [StringLength(250)]
        public string LinkTrailer { get; set; }
        [Required]
        [StringLength(500)]
        public string Descrição { get; set; }
        public int Duracao { get; set; }
        public int IdCreationUser { get; set; }
        [Column(TypeName = "date")]
        public DateTime DataAdicionado { get; set; }

        [InverseProperty(nameof(CategoriasFilme.IdFilmeNavigation))]
        public virtual ICollection<CategoriasFilme> CategoriasFilmes { get; set; }
        [InverseProperty(nameof(Sessao.IdFilmeNavigation))]
        public virtual ICollection<Sessao> Sessaos { get; set; }
    }
}
