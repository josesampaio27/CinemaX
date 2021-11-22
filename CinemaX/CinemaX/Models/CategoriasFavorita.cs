using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    public partial class CategoriasFavorita
    {
        [Key, Column(Order = 0)]
        public int IdUtilizador { get; set; }
        [Key, Column(Order = 1)]
        public int IdCategoria { get; set; }

        [ForeignKey(nameof(IdCategoria))]
        [InverseProperty(nameof(Categorium.CategoriasFavorita))]
        public virtual Categorium IdCategoriaNavigation { get; set; }
        [ForeignKey(nameof(IdUtilizador))]
        [InverseProperty(nameof(Utilizador.CategoriasFavorita))]
        public virtual Utilizador IdUtilizadorNavigation { get; set; }
    }
}
