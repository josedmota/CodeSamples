using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockFrontend.Classes
{
    public class GrahamValuation : FundamentalAnalysisElement
    {
        public double gRate { get; set; } = 3;

        public double? Valuation { get; set; }

        public string ResponsePropertyName = "Valuation";

        public GrahamValuation() { }
        /// <summary>
        /// Power user Configure Method allows to set custom growth rates
        /// </summary>
        /// <param name="ticker"></param>
        /// <param name="growthRate">growth rate for nexxt years</param>
        public void Configure(string ticker,double growthRate)
        {
            this.Ticker = ticker;
            this.gRate = growthRate;
        }

        /// <summary>
        /// Simple valuation request with default growth rate
        /// </summary>
        /// <param name="ticker"></param>
        public override void Configure(string ticker)
        {
            this.Ticker = ticker;
        }

        /// <summary>
        /// Builds the complete request using the stored config parameters
        /// </summary>
        public override void BuildRequest()
        {
            if(this.Ticker is null)
            {
                Console.WriteLine("No Ticker Defined");
                return;
            }

            this.BuiltUrl = API_root + "GrahamValuation?name=" + this.Ticker + "&gRate=" + this.gRate;
            Console.WriteLine("BuiltUrl: "+this.BuiltUrl);
        }

        /// <summary>
        /// Send the request and receive raw json string
        /// </summary>
        public async override Task MakeRequest()
        {
            string json;

            if(this.BuiltUrl is null)
            {
                return;
            }

            try
            {
                var Http = new HttpClient();
                json = await Http.GetStringAsync(this.BuiltUrl);
                Console.WriteLine("Raw Response: "+json);

                if(!ParseJsonIntoType(json))
                {
                    Console.WriteLine("Failed to Parse Json in GrahamValuation");
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Failed Request");
            }
        }

        /// <summary>
        /// Parse Json and store as string value and double
        /// Returns true when succesfull
        /// False otherwise
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public override bool ParseJsonIntoType(string response)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    this.ValueAsString = doc.RootElement.GetProperty(this.ResponsePropertyName).GetString();
                    double Val;

                    if(ValueAsString is null)
                    {
                        return false;
                    }

                    if(double.TryParse(ValueAsString, out Val))
                    {
                        this.Valuation = Val;
                        return true;
                    }
                    return false;
                }
            }
            catch (Exception)
            {
               Console.WriteLine("Failed To Parse Json Response");
               return false;
            }
        }
    }
}
