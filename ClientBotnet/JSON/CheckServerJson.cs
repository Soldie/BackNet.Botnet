using Newtonsoft.Json;

namespace ClientBotnet
{
    internal class CheckServerRequestJson : BaseRequestJson
    {
        public CheckServerRequestJson() : base(command:"ping") { }
    }

    internal class CheckServerResponseJson
    {
        [JsonProperty(Required = Required.Always)]
        public bool isAlive { get; set; }
    }
}
