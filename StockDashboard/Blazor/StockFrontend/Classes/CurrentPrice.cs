using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace StockFrontend.Classes
{
    public class CurrentPrice : FundamentalAnalysisElement
    {

        public double? Price { get; set; }

        public string ResponsePropertyName = "SharePrice";

        public override void BuildRequest()
        {
            if (this.Ticker is null)
            {
                Console.WriteLine("No Ticker Defined");
                return;
            }

            this.BuiltUrl = API_root + "GetValue?name=" + this.Ticker;
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
                    double Val;

                    if (ValueAsString is null)
                    {
                        return false;
                    }

                    if (double.TryParse(ValueAsString, out Val))
                    {
                        this.Price = Val;
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
