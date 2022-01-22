using CinemaX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaX.Data
{
    public class DbInitializer
    {

        public static void Initialize(CinemaXContext context)
        {
            context.Database.EnsureCreated();

            if (context.ListaPermissoes.Any() || context.GrupoPermissoes.Any() || context.Permissoes.Any())
            {
                return;
            }

            var permissoes = new Permisso[]
            {
                new Permisso{NomePermissao="BackOffice",IdPermissao=1},
                new Permisso{NomePermissao="EditFilme",IdPermissao=2},
                new Permisso{NomePermissao="EditUser",IdPermissao=3},
                new Permisso{NomePermissao="EditSala",IdPermissao=4},
                new Permisso{NomePermissao="EditSessao",IdPermissao=5},
                new Permisso{NomePermissao="EditCategorias",IdPermissao=7},
                new Permisso{NomePermissao="EditPermissoes",IdPermissao=8},
            };
            foreach(Permisso p in permissoes)
            {
                context.Permissoes.Add(p);
            }
            context.SaveChanges();

            var Grupos = new GrupoPermisso[]
            {
                new GrupoPermisso{NomeGrupo="Geral"},
                new GrupoPermisso{NomeGrupo="Admin"},
                new GrupoPermisso{NomeGrupo="Funcionario"},
            };
            foreach (GrupoPermisso g in Grupos)
            {
                context.GrupoPermissoes.Add(g);
            }
            context.SaveChanges();

            var Lista = new ListaPermisso[]
            {
                new ListaPermisso{IdGrupo=2,IdPermissao=1},
                new ListaPermisso{IdGrupo=2,IdPermissao=2},
                new ListaPermisso{IdGrupo=2,IdPermissao=3},
                new ListaPermisso{IdGrupo=2,IdPermissao=4},
                new ListaPermisso{IdGrupo=2,IdPermissao=5},
                new ListaPermisso{IdGrupo=2,IdPermissao=7},
                new ListaPermisso{IdGrupo=2,IdPermissao=8},
                new ListaPermisso{IdGrupo=3,IdPermissao=1},
                new ListaPermisso{IdGrupo=3,IdPermissao=2},
            };
            foreach (ListaPermisso l in Lista)
            {
                context.ListaPermissoes.Add(l);
            }
            context.SaveChanges();

            Utilizador utilizador = new Utilizador { UserName = "Admin", ActivationCode = "Activated", IdGrupo = 2, UserPassWord = "3875034E17855BAC03A3CC9E107B1D28A9B44313D381C3335588525B4E70B55B" };
                      
            context.Utilizadors.Add(utilizador);           
            context.SaveChanges();

        }
    }
}
