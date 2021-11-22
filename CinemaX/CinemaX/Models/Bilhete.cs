using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    [Table("Bilhete")]
    public partial class Bilhete
    {
        [Key]
        [Column(Order = 0)]
        public int IdSessao { get; set; }
        [Key]
        [Column(Order = 1)]
        public int IdUtilizador { get; set; }
        [Key]
        [Column(Order = 2)]
        public int NumBilhete { get; set; }

        [ForeignKey(nameof(IdSessao))]
        [InverseProperty(nameof(Sessao.Bilhetes))]
        public virtual Sessao IdSessaoNavigation { get; set; }
        [ForeignKey(nameof(IdUtilizador))]
        [InverseProperty(nameof(Utilizador.Bilhetes))]
        public virtual Utilizador IdUtilizadorNavigation { get; set; }
    }
}
