using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace CinemaX.Models
{
    [Keyless]
    public partial class ListaPermisso
    {
        public int IdGrupo { get; set; }
        public int IdPermissao { get; set; }

        [ForeignKey(nameof(IdGrupo))]
        public virtual GrupoPermisso IdGrupoNavigation { get; set; }
        [ForeignKey(nameof(IdPermissao))]
        public virtual Permisso IdPermissaoNavigation { get; set; }
    }
}
