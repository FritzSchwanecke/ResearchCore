using Quandl.NET;

namespace DataManagement.LoadData
{
    public class EquityIndexData
    {
        private readonly string _apiKey = "uXgCyz9umr66c5QjgZ6d";
        private QuandlClient quandlClient;

        public void GetData()
        {
            this.quandlClient = new QuandlClient(this._apiKey);
            var test = this.quandlClient.Timeseries.GetDataAsync("CHRIS", "MGEX_IW1", 10).Result;
        }
    }
}
