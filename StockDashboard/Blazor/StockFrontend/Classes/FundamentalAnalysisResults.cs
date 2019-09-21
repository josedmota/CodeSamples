using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockFrontend.Classes
{
    public class FundamentalAnalysisResults
    {

        //Properties
        public double StockPrice { get; set; }

        public double PriceGrowth { get; set; }

        public double ROTA { get; set; }

        public double ROE { get; set; }

        public double DeToEq { get; set; }
        public double CurrentRatio { get; set; }

        public double GrahamValue { get; set; }

        public string Conclusion { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public FundamentalAnalysisResults()
        {

        }

        public async void UpdateDecision()
        {
            //TODO: Decide here if should BUY or SELL based on fundamentals
        }


    }
}
