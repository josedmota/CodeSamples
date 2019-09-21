using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockFrontend.Classes
{
    public class GrowthToDate : FundamentalAnalysisElement
    {
        public double? Growth { get; set; }

        public int Period { get; set; } = 3;

        private string ResponsePropertyName = "Growth";
        public GrowthToDate(int months)
        {
            this.Period = months;
        }

        public override void BuildRequest()
        {
            if (this.Ticker is null)
            {
                Console.WriteLine("No Ticker Defined");
                return;
            }

            this.BuiltUrl = API_root + "GetGrowth?name=" + this.Ticker+"&period=" + this.Period.ToString();
            Console.WriteLine("BuiltUrl: " + this.BuiltUrl);
        }

        public override void Configure(string ticker)
        {
            this.Ticker = ticker;
        }

        public async override Task MakeRequest()
        {
            string json;

            if (this.BuiltUrl is null)
            {
                return;
            }

            try
            {
                var Http = new HttpClient();
                json = await Http.GetStringAsync(this.BuiltUrl);
                Console.WriteLine("Raw Response: " + json);

                if (!ParseJsonIntoType(json))
                {
                    Console.WriteLine("Failed to Parse Json in GrahamValuation");
                }

            }
            catch (Exception)
            {
                Console.WriteLine("Failed Request");
            }
        }

        public override bool ParseJsonIntoType(string response)
        {
            try
            {
                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    this.ValueAsString = doc.RootElement.GetProperty(this.ResponsePropertyName).GetString();
                    this.ValueAsString = this.ValueAsString.Replace("%", "");
                    double Val;

                    if (ValueAsString is null)
                    {
                        return false;
                    }

                    if (double.TryParse(ValueAsString, out Val))
                    {
                        this.Growth = Val;
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
