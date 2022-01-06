using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    public partial class GrupoPermisso
    {
        public GrupoPermisso()
        {
            Utilizadors = new HashSet<Utilizador>();          
        }

        [Key]
        public int IdGrupo { get; set; }
        [Required]
        [StringLength(30)]
        public string NomeGrupo { get; set; }

        [InverseProperty(nameof(Utilizador.IdGrupoNavigation))]
        public virtual ICollection<Utilizador> Utilizadors { get; set; }     
    }
}
