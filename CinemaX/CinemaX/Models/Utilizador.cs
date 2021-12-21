using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    [Table("Utilizador")]
    public partial class Utilizador
    {
        public Utilizador()
        {
            Bilhetes = new HashSet<Bilhete>();
            CategoriasFavorita = new HashSet<CategoriasFavorita>();
        }

        [Required]
        [StringLength(30)]
        public string UserName { get; set; }
        [Required]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string UserPassWord { get; set; }
        [Key]
        public int IdUtilizador { get; set; }
        public int IdGrupo { get; set; } = 0;
        public string ActivationCode { get; set; }

        [ForeignKey(nameof(IdGrupo))]
        [InverseProperty(nameof(GrupoPermisso.Utilizadors))]
        public virtual GrupoPermisso IdGrupoNavigation { get; set; }
        [InverseProperty("IdUtilizadorNavigation")]
        public virtual Perfil Perfil { get; set; }
        [InverseProperty(nameof(Bilhete.IdUtilizadorNavigation))]
        public virtual ICollection<Bilhete> Bilhetes { get; set; }
        [InverseProperty("IdUtilizadorNavigation")]
        public virtual ICollection<CategoriasFavorita> CategoriasFavorita { get; set; }
    }
}
