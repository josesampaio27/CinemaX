#pragma checksum "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2ff4a70aba24ac5cd7d49f8ca48c038fa911b064"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Utilizadors_EditarCategoriasFavoritas), @"mvc.1.0.view", @"/Views/Utilizadors/EditarCategoriasFavoritas.cshtml")]
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
#nullable restore
#line 2 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
using Microsoft.AspNetCore.Http;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2ff4a70aba24ac5cd7d49f8ca48c038fa911b064", @"/Views/Utilizadors/EditarCategoriasFavoritas.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"b57ff00a6f8c88acaf7c771060f89d2b4e988f5d", @"/Views/_ViewImports.cshtml")]
    public class Views_Utilizadors_EditarCategoriasFavoritas : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<CinemaX.Models.Perfil>
    {
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_0 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("class", new global::Microsoft.AspNetCore.Html.HtmlString("text-danger"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_1 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("asp-action", "EditarCategoriasFavoritas", global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        private static readonly global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute __tagHelperAttribute_2 = new global::Microsoft.AspNetCore.Razor.TagHelpers.TagHelperAttribute("enctype", new global::Microsoft.AspNetCore.Html.HtmlString("multipart/form-data"), global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
        #line hidden
        #pragma warning disable 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperExecutionContext __tagHelperExecutionContext;
        #pragma warning restore 0649
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner __tagHelperRunner = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperRunner();
        #pragma warning disable 0169
        private string __tagHelperStringValueBuffer;
        #pragma warning restore 0169
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __backed__tagHelperScopeManager = null;
        private global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager __tagHelperScopeManager
        {
            get
            {
                if (__backed__tagHelperScopeManager == null)
                {
                    __backed__tagHelperScopeManager = new global::Microsoft.AspNetCore.Razor.Runtime.TagHelpers.TagHelperScopeManager(StartTagHelperWritingScope, EndTagHelperWritingScope);
                }
                return __backed__tagHelperScopeManager;
            }
        }
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper;
        private global::Microsoft.AspNetCore.Mvc.TagHelpers.ValidationSummaryTagHelper __Microsoft_AspNetCore_Mvc_TagHelpers_ValidationSummaryTagHelper;
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 4 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
  
    ViewData["Title"] = "Editar Categorias Favoritas";
   

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n<h1>Editar categorias favoritas</h1>\r\n\r\n<hr />\r\n    <div class=\"row\">\r\n        <div class=\"col-md-4\">\r\n            ");
            __tagHelperExecutionContext = __tagHelperScopeManager.Begin("form", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2ff4a70aba24ac5cd7d49f8ca48c038fa911b0645248", async() => {
                WriteLiteral("\r\n                ");
                __tagHelperExecutionContext = __tagHelperScopeManager.Begin("div", global::Microsoft.AspNetCore.Razor.TagHelpers.TagMode.StartTagAndEndTag, "2ff4a70aba24ac5cd7d49f8ca48c038fa911b0645522", async() => {
                }
                );
                __Microsoft_AspNetCore_Mvc_TagHelpers_ValidationSummaryTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.ValidationSummaryTagHelper>();
                __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_ValidationSummaryTagHelper);
#nullable restore
#line 15 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
__Microsoft_AspNetCore_Mvc_TagHelpers_ValidationSummaryTagHelper.ValidationSummary = global::Microsoft.AspNetCore.Mvc.Rendering.ValidationSummary.ModelOnly;

#line default
#line hidden
#nullable disable
                __tagHelperExecutionContext.AddTagHelperAttribute("asp-validation-summary", __Microsoft_AspNetCore_Mvc_TagHelpers_ValidationSummaryTagHelper.ValidationSummary, global::Microsoft.AspNetCore.Razor.TagHelpers.HtmlAttributeValueStyle.DoubleQuotes);
                __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
                await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
                if (!__tagHelperExecutionContext.Output.IsContentModified)
                {
                    await __tagHelperExecutionContext.SetOutputContentAsync();
                }
                Write(__tagHelperExecutionContext.Output);
                __tagHelperExecutionContext = __tagHelperScopeManager.End();
                WriteLiteral("\r\n                    <div>\r\n                        <label>Categorias:</label>\r\n                        <ul>\r\n");
#nullable restore
#line 19 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
                             foreach (Categorium categorias in ViewBag.Categorias)
                            {
                                var Categoria = Model.IdUtilizadorNavigation.CategoriasFavorita.FirstOrDefault(c => c.IdUtilizador == Context.Session.GetInt32("IdUtilizador") && c.IdCategoria == categorias.IdCategoria);
                                
                                    if (Categoria != null)
                                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                        <li><input type=\"checkbox\" name=\"IdCategorias\" checked");
                BeginWriteAttribute("value", " value=\"", 1085, "\"", 1116, 1);
#nullable restore
#line 25 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
WriteAttributeValue("", 1093, categorias.IdCategoria, 1093, 23, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("> ");
#nullable restore
#line 25 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
                                                                                                                           Write(categorias.Nome);

#line default
#line hidden
#nullable disable
                WriteLiteral("</li>\r\n");
#nullable restore
#line 26 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
                                    }
                                    else
                                    {

#line default
#line hidden
#nullable disable
                WriteLiteral("                                        <li><input type=\"checkbox\" name=\"IdCategorias\"");
                BeginWriteAttribute("value", " value=\"", 1348, "\"", 1379, 1);
#nullable restore
#line 29 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
WriteAttributeValue("", 1356, categorias.IdCategoria, 1356, 23, false);

#line default
#line hidden
#nullable disable
                EndWriteAttribute();
                WriteLiteral("> ");
#nullable restore
#line 29 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
                                                                                                                   Write(categorias.Nome);

#line default
#line hidden
#nullable disable
                WriteLiteral("</li>\r\n");
#nullable restore
#line 30 "C:\Users\jldia\Documents\Source\Repos\GestorDeCinema\CinemaX\CinemaX\Views\Utilizadors\EditarCategoriasFavoritas.cshtml"
                                    }                                                                
                            }

#line default
#line hidden
#nullable disable
                WriteLiteral("                        </ul>                       \r\n                    </div>\r\n                <div class=\"form-group\">\r\n                    <input type=\"submit\" value=\"Adicionar\" class=\"btn btn-primary\" />\r\n                </div>\r\n            ");
            }
            );
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.FormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper = CreateTagHelper<global::Microsoft.AspNetCore.Mvc.TagHelpers.RenderAtEndOfFormTagHelper>();
            __tagHelperExecutionContext.Add(__Microsoft_AspNetCore_Mvc_TagHelpers_RenderAtEndOfFormTagHelper);
            __Microsoft_AspNetCore_Mvc_TagHelpers_FormTagHelper.Action = (string)__tagHelperAttribute_1.Value;
            __tagHelperExecutionContext.AddTagHelperAttribute(__tagHelperAttribute_1);
            __tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_2);
            await __tagHelperRunner.RunAsync(__tagHelperExecutionContext);
            if (!__tagHelperExecutionContext.Output.IsContentModified)
            {
                await __tagHelperExecutionContext.SetOutputContentAsync();
            }
            Write(__tagHelperExecutionContext.Output);
            __tagHelperExecutionContext = __tagHelperScopeManager.End();
            WriteLiteral("\r\n        </div>\r\n    </div>");
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
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<CinemaX.Models.Perfil> Html { get; private set; }
    }
}
#pragma warning restore 1591
