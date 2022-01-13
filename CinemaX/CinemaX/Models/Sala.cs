using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    [Table("Sala")]
    public partial class Sala
    {
        public Sala()
        {
            Sessaos = new HashSet<Sessao>();
        }

        [Key]
        public int Numero { get; set; }
        [Required]
        public int Capacidade { get; set; }
        public int IdCreationUser { get; set; }
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime DataAdicionada { get; set; }

        [InverseProperty(nameof(Sessao.NumeroNavigation))]
        public virtual ICollection<Sessao> Sessaos { get; set; }
    }
}
