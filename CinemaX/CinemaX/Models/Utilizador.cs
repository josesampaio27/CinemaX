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

        [Required(ErrorMessage ="O campo {0} é obrigatorio")]
        [StringLength(30)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatorio")]      
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 10 ,ErrorMessage = "A {0} deve ter pelo menos {2} caracteres e um maximo de {1} caracteres")]
        [RegularExpression(@"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$",ErrorMessage ="A password deverá conter pelo menos uma letra minuscula, uma lestra maiuscula e um numero")]
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
