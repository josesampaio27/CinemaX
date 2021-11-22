using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    public partial class CategoriasFilme
    {
        [Key, Column(Order = 0)]
        public int IdFilme { get; set; }
        [Key, Column(Order = 1)]
        public int IdCategoria { get; set; }

        [ForeignKey(nameof(IdCategoria))]
        [InverseProperty(nameof(Categorium.CategoriasFilmes))]
        public virtual Categorium IdCategoriaNavigation { get; set; }
        [ForeignKey(nameof(IdFilme))]
        [InverseProperty(nameof(Filme.CategoriasFilmes))]
        public virtual Filme IdFilmeNavigation { get; set; }
    }
}
