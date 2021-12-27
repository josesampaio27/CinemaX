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
        [Required(ErrorMessage = "O campo {0} é obrigatorio")]
        [StringLength(50)]
        public string Nome { get; set; }
        [StringLength(200)]
        public string Foto { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatorio")]
        [StringLength(50)]
        public string Realizador { get; set; }
        [Column("Data_", TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime Data { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatorio")]
        [StringLength(250)]
        [DataType(DataType.Url)]
        public string LinkTrailer { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatorio")]
        [StringLength(500)]
        public string Descrição { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatorio")]
        public decimal Duracao { get; set; }
        public int IdCreationUser { get; set; }
        [Column(TypeName = "date")]
        public DateTime DataAdicionado { get; set; }

        [InverseProperty(nameof(CategoriasFilme.IdFilmeNavigation))]
        public virtual ICollection<CategoriasFilme> CategoriasFilmes { get; set; }
        [InverseProperty(nameof(Sessao.IdFilmeNavigation))]
        public virtual ICollection<Sessao> Sessaos { get; set; }
    }
}
