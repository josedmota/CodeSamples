#pragma checksum "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\Pages\Index.razor" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "300de3b975fb25ac2592c9abce0926687b9a4d70"
// <auto-generated/>
#pragma warning disable 1591
namespace StockFrontend.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#line 1 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#line 2 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#line 3 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\_Imports.razor"
using Microsoft.AspNetCore.Components.Layouts;

#line default
#line hidden
#line 4 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#line 5 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#line 6 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\_Imports.razor"
using StockFrontend;

#line default
#line hidden
#line 7 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\_Imports.razor"
using StockFrontend.Shared;

#line default
#line hidden
#line 2 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\Pages\_Imports.razor"
using MatBlazor;

#line default
#line hidden
    [Microsoft.AspNetCore.Components.Layouts.LayoutAttribute(typeof(AltLayout))]
    [Microsoft.AspNetCore.Components.RouteAttribute("/")]
    public class Index : IndexBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.RenderTree.RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", "HeaderContainer");
            builder.AddMarkupContent(2, "\r\n    ");
            builder.OpenComponent<StockFrontend.Shared.FullHeader>(3);
            builder.AddAttribute(4, "OnClick", Microsoft.AspNetCore.Components.RuntimeHelpers.TypeCheck<Microsoft.AspNetCore.Components.EventCallback>(Microsoft.AspNetCore.Components.EventCallback.Factory.Create(this, 
#line 6 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\Pages\Index.razor"
                                        NavBarClickEvent

#line default
#line hidden
            )));
            builder.AddComponentReferenceCapture(5, (__value) => {
#line 6 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\Pages\Index.razor"
                      NavBar = (StockFrontend.Shared.FullHeader)__value;

#line default
#line hidden
            }
            );
            builder.CloseComponent();
            builder.AddMarkupContent(6, "\r\n");
            builder.CloseElement();
            builder.AddMarkupContent(7, "\r\n\r\n\r\n");
            builder.OpenElement(8, "div");
            builder.AddAttribute(9, "class", "CardContainer");
            builder.AddMarkupContent(10, "\r\n    ");
            builder.OpenComponent<StockFrontend.Pages.FundamentalAnalysis>(11);
            builder.AddComponentReferenceCapture(12, (__value) => {
#line 11 "C:\code\StockApp\StockDashboard\Blazor\StockFrontend\Pages\Index.razor"
                               FundamentalsCard = (StockFrontend.Pages.FundamentalAnalysis)__value;

#line default
#line hidden
            }
            );
            builder.CloseComponent();
            builder.AddMarkupContent(13, "\r\n");
            builder.CloseElement();
        }
        #pragma warning restore 1998
    }
}
#pragma warning restore 1591
