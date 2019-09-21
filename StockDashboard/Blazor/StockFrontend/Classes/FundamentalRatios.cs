using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace StockFrontend.Classes
{
    public class FundamentalRatios : FundamentalAnalysisElement
    {
        /// <summary>
        /// Return on total assets
        /// </summary>
        public double? ROTA { get; set; }
        private string ROTA_key = "ROTA";

        /// <summary>
        /// Return on Equity
        /// </summary>
        public double? ROE { get; set; }
        private string ROE_key = "ROE";

        public double? DebtToEquity { get; set; }
        private string DE_key = "DEratio";

        public double? CurrentRatio { get; set; }
        private string CurRat_key = "Cratio";

        public override void BuildRequest()
        {
            if (this.Ticker is null)
            {
                Console.WriteLine("No Ticker Defined");
                return;
            }

            this.BuiltUrl = API_root + "ROE_ROTA_Cratio?name=" + this.Ticker;
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

                this.ROE = ParseMultiple(response, this.ROE_key);
                this.ROTA = ParseMultiple(response, this.ROTA_key);
                this.DebtToEquity = ParseMultiple(response, this.DE_key);
                this.CurrentRatio = ParseMultiple(response, this.CurRat_key);

                return true;
            }
            catch (Exception)
            {
                Console.WriteLine("Failed To Parse Json Response");
                return false;
            }
        }

        public double? ParseMultiple(string response,string key)
        {
            using (JsonDocument doc = JsonDocument.Parse(response))
            {
                this.ValueAsString = doc.RootElement.GetProperty(key).GetString();
                this.ValueAsString = this.ValueAsString.Replace("%", "");
                double Val;

                if (ValueAsString is null)
                {
                    return null;
                }

                if (double.TryParse(ValueAsString, out Val))
                {
                    return Val;
                }
                return null;
            }
        }
    }
}
