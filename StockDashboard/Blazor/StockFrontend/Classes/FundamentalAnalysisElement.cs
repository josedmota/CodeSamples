using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace StockFrontend.Classes
{
    public abstract class FundamentalAnalysisElement
    {

        public static string API_root = "https://iexcloud.azurewebsites.net/api/";

        public string BuiltUrl { get; set; }

        public string Ticker { get; set; }

        public string ValueAsString { get; set; }

        public string ReponsePropertyName { get; set; }

        public abstract void Configure(string ticker);

        public abstract void BuildRequest();

        public abstract Task MakeRequest();

        public abstract bool ParseJsonIntoType(string response);

    }
}
