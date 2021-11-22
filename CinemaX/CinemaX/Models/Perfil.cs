using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    [Table("Perfil")]
    public partial class Perfil
    {
        [Required]
        [StringLength(50)]
        public string Nome { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        [Column(TypeName = "date")]
        public DateTime DataNascimento { get; set; }
        [Key]
        public int IdUtilizador { get; set; }
        public int Telemovel { get; set; }

        [ForeignKey(nameof(IdUtilizador))]
        [InverseProperty(nameof(Utilizador.Perfil))]
        public virtual Utilizador IdUtilizadorNavigation { get; set; }
    }
}
