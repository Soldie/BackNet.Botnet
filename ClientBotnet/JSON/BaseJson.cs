namespace ClientBotnet
{
    internal abstract class BaseRequestJson
    {
        public string key { get; set; }

        public string command { get; set; }

        protected BaseRequestJson(string command)
        {
            key = BotnetManager.KEY;
            this.command = command;
        }
    }
}
