using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using StockFrontend.Classes;

namespace StockFrontend.Pages
{
    public class FundamentalAnalysisBase:ComponentBase
    {

        private FundamentalAnalysisResults ResultsAgregator = new FundamentalAnalysisResults();

        private List<FundamentalAnalysisElement> DataPoints = new List<FundamentalAnalysisElement>();

        [Parameter]
        public GrahamValuation Graham { get; set; } = new GrahamValuation();

        [Parameter]
        public CurrentPrice SharePrice { get; set; } = new CurrentPrice();

        [Parameter]
        public GrowthToDate Growth { get; set; } = new GrowthToDate(3);

        [Parameter]
        public FundamentalRatios Ratios { get; set; } = new FundamentalRatios();

        /// <summary>
        /// Component startup tasks
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitAsync()
        {
            //Add elements to a list to then iterate throught calling update tasks
            DataPoints.Add(Graham);
            DataPoints.Add(SharePrice);
            DataPoints.Add(Growth);
            DataPoints.Add(Ratios);
        }

        /// <summary>
        /// Event to be called when the refresh command is given
        /// </summary>
        public async void Update()
        {
            List<Task> CalledTasks = new List<Task>();

            //Call all the tasks
            foreach (FundamentalAnalysisElement elem in DataPoints)
            {
                elem.BuildRequest();
                CalledTasks.Add(elem.MakeRequest());
            }

            //Send an update UI when all finish
            await Task.WhenAll(CalledTasks);

            //TODO: Could probably make this mode debugable by adding a while loop and Task.WhenAny() Log ("task x finished")
            //See MSFT docs

            //Graham.BuildRequest();
            //await Graham.MakeRequest();
            StateHasChanged();
            UpdateResultOBJ();
        }

        public void UpdateTickerOnChildren(string stock)
        {
            if(stock is null)
            {
                //Probably the check will be if it exists not if is null
                return;
            }

            foreach(FundamentalAnalysisElement elem in DataPoints)
            {
                elem.Ticker = stock.Replace("$","");
            }
        }

        /// <summary>
        /// Record values on the Results Object and call any update vereditct functions that are needed inside the obj
        /// </summary>
        public void UpdateResultOBJ()
        {
            //Update results
            ResultsAgregator.GrahamValue = (Graham.Valuation!=null)? (double)Graham.Valuation : 0;
            ResultsAgregator.StockPrice = (SharePrice.Price!=null)?(double)SharePrice.Price:0;
            ResultsAgregator.PriceGrowth = (Growth.Growth != null) ? (double)Growth.Growth : 0;
            ResultsAgregator.ROE = (Ratios.ROE!=null)?(double)Ratios.ROE:0;
            ResultsAgregator.ROTA = (Ratios.ROTA != null) ? (double)Ratios.ROTA : 0;
            ResultsAgregator.DeToEq = (Ratios.DebtToEquity != null) ? (double)Ratios.DebtToEquity : 0;
            ResultsAgregator.CurrentRatio = (Ratios.CurrentRatio != null) ? (double)Ratios.CurrentRatio : 0;

            //Update global Veredict on fundamentals
            ResultsAgregator.UpdateDecision();
        }

    }
}
