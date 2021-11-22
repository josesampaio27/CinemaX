using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    public partial class Permisso
    {
        [Key]
        public int IdPermissao { get; set; }
        [Required]
        [StringLength(30)]
        public string NomePermissao { get; set; }
    }
}
