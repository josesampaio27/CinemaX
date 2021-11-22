using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    public partial class Categorium
    {
        public Categorium()
        {
            CategoriasFavorita = new HashSet<CategoriasFavorita>();
            CategoriasFilmes = new HashSet<CategoriasFilme>();
        }

        [Key]
        public int IdCategoria { get; set; }
        [Required]
        [StringLength(30)]
        public string Nome { get; set; }

        [InverseProperty("IdCategoriaNavigation")]
        public virtual ICollection<CategoriasFavorita> CategoriasFavorita { get; set; }
        [InverseProperty(nameof(CategoriasFilme.IdCategoriaNavigation))]
        public virtual ICollection<CategoriasFilme> CategoriasFilmes { get; set; }
    }
}
