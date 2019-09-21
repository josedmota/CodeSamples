using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StockFrontend.Classes;
using StockFrontend.Shared;

namespace StockFrontend.Pages
{

    public class IndexBase : ComponentBase
    {

        public FullHeader NavBar;

        public FundamentalAnalysisBase FundamentalsCard;

        public async void NavBarClickEvent()
        {
            FundamentalsCard.UpdateTickerOnChildren(NavBar.ticker);
            FundamentalsCard.Update();
            Console.WriteLine(NavBar.ticker.ToString());
        }
    }
}
