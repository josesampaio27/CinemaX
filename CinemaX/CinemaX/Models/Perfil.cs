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
        [Required(ErrorMessage = "O campo {0} é obrigatorio")]
        [StringLength(50)]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo {0} é obrigatorio")]
        [StringLength(50)]
        [DataType(DataType.EmailAddress, ErrorMessage ="Introduza um endereço email valido")]
        public string Email { get; set; }
        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        public DateTime DataNascimento { get; set; }
        [Key]
        public int IdUtilizador { get; set; }
        [Range(900000000, 969999999,ErrorMessage ="Intruduza um numero valido")]
        public int Telemovel { get; set; }

        [ForeignKey(nameof(IdUtilizador))]
        [InverseProperty(nameof(Utilizador.Perfil))]
        public virtual Utilizador IdUtilizadorNavigation { get; set; }
    }
}
