
using Newtonsoft.Json;

namespace CloudCoinCsharpSDK
{
    class DepositResponse : IBankResponse
    {
        [JsonProperty("bank_server")]
        public string bank_server { get; set; }

        [JsonProperty("time")]
        public string time { get; set; }

        [JsonProperty("status")]
        public string status { get; set; }

        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("receipt")]
        public string receipt { get; set; }

        public DepositResponse()
        {

        }
    }
}
