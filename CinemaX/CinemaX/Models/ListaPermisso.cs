using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{

    public partial class ListaPermisso
    {
        [Key, Column(Order = 0)]
        public int IdGrupo { get; set; }
        [Key, Column(Order = 1)]
        public int IdPermissao { get; set; }

        [ForeignKey(nameof(IdGrupo))]
        public virtual GrupoPermisso IdGrupoNavigation { get; set; }
        [ForeignKey(nameof(IdPermissao))]
        public virtual Permisso IdPermissaoNavigation { get; set; }
    }
}
