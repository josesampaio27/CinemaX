#pragma checksum "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "067e974a3c849f879a21f808418081a9a6fb77bf"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_BackOffice_Details), @"mvc.1.0.view", @"/Views/BackOffice/Details.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#nullable restore
#line 1 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\_ViewImports.cshtml"
using CinemaX;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\_ViewImports.cshtml"
using CinemaX.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"067e974a3c849f879a21f808418081a9a6fb77bf", @"/Views/BackOffice/Details.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b57ff00a6f8c88acaf7c771060f89d2b4e988f5d", @"/Views/_ViewImports.cshtml")]
    public class Views_BackOffice_Details : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<CinemaX.Models.Filme>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
  
    ViewData["Title"] = "Details";

    string duracao = Model.Duracao.ToString().Replace(",", ":") + "h";

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>Details</h1>\r\n\r\n<div>\r\n    <h4>Filme</h4>\r\n    <hr />\r\n    <dl class=\"row\">\r\n        <dt class=\"col-sm-2\">\r\n            ");
#nullable restore
#line 16 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Nome));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
#nullable restore
#line 19 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayFor(model => model.Nome));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt class=\"col-sm-2\">\r\n            ");
#nullable restore
#line 22 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Realizador));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
#nullable restore
#line 25 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayFor(model => model.Realizador));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt class=\"col-sm-2\">\r\n            ");
#nullable restore
#line 28 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Data));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
#nullable restore
#line 31 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayFor(model => model.Data));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt class=\"col-sm-2\">\r\n            ");
#nullable restore
#line 34 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Descrição));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
#nullable restore
#line 37 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayFor(model => model.Descrição));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt class=\"col-sm-2\">\r\n            ");
#nullable restore
#line 40 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(Html.DisplayNameFor(model => model.Duracao));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n            ");
#nullable restore
#line 43 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
       Write(duracao);

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </dd>\r\n        <dt class=\"col-sm-2\">\r\n            Categorias:\r\n        </dt>\r\n        <dd class=\"col-sm-10\">\r\n");
#nullable restore
#line 49 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
              int CategoriasCount = Model.CategoriasFilmes.Count; 

#line default
#line hidden
#nullable disable
#nullable restore
#line 50 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
             foreach (var Categoria in Model.CategoriasFilmes)
            {
                CategoriasCount--;
                

#line default
#line hidden
#nullable disable
#nullable restore
#line 53 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
           Write(Categoria.IdCategoriaNavigation.Nome);

#line default
#line hidden
#nullable disable
#nullable restore
#line 53 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
                                                     ;
                if (CategoriasCount != 0)
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("<label>,</label> ");
#nullable restore
#line 55 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
                                  }
                else 
                {

#line default
#line hidden
#nullable disable
            WriteLiteral("<label>.</label>");
#nullable restore
#line 57 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
                                 }

            }

#line default
#line hidden
#nullable disable
            WriteLiteral(@"        </dd>
    </dl>
    <br/>
    <h4>Trailer:</h4>
    <div id=""player""></div>

    <script>
        // 2. This code loads the IFrame Player API code asynchronously.
        var tag = document.createElement('script');

        tag.src = ""https://www.youtube.com/iframe_api"";
        var firstScriptTag = document.getElementsByTagName('script')[0];
        firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

        // 3. This function creates an <iframe> (and YouTube player)
        //    after the API code downloads.
        var player;
        function onYouTubeIframeAPIReady() {
            player = new YT.Player('player', {
                height: '360',
                width: '640',
                videoId: '");
#nullable restore
#line 81 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\BackOffice\Details.cshtml"
                     Write(Model.LinkTrailer.Remove(0,32));

#line default
#line hidden
#nullable disable
            WriteLiteral(@"',
                events: {
                    'onStateChange': onPlayerStateChange,
                    'autoplay': 0
                }
            });
        }

        // 4. The API will call this function when the video player is ready.
        function onPlayerReady(event) {
            event.target.playVideo();
        }

        // 5. The API calls this function when the player's state changes.
        //    The function indicates that when playing a video (state=1),
        //    the player should play for six seconds and then stop.
        var done = false;
        function onPlayerStateChange(event) {
            if (event.data == YT.PlayerState.PLAYING && !done) {
                setTimeout(stopVideo, 6000);
                done = true;
            }
        }
        function stopVideo() {
            player.stopVideo();
        }
    </script>
</div>
");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<CinemaX.Models.Filme> Html { get; private set; }
    }
}
#pragma warning restore 1591