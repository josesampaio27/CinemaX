using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    [Table("Sessao")]
    public partial class Sessao
    {
        public Sessao()
        {
            Bilhetes = new HashSet<Bilhete>();
        }

        public int IdFilme { get; set; }
        public int Numero { get; set; }
        [Key]
        public int IdSessao { get; set; }
        [Column("Data_", TypeName = "datetime")]
        public DateTime Data { get; set; }
        [Column(TypeName = "money")]
        public decimal Preço { get; set; }
        [NotMapped]
        [Range(0, 99.99)]             
        public string Preço_string { get; set; }
        public int Vagas { get; set; }

        [ForeignKey(nameof(IdFilme))]
        [InverseProperty(nameof(Filme.Sessaos))]
        public virtual Filme IdFilmeNavigation { get; set; }
        [ForeignKey(nameof(Numero))]
        [InverseProperty(nameof(Sala.Sessaos))]
        public virtual Sala NumeroNavigation { get; set; }
        [InverseProperty(nameof(Bilhete.IdSessaoNavigation))]
        public virtual ICollection<Bilhete> Bilhetes { get; set; }
    }
}
