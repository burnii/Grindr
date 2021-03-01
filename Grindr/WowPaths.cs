using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grindr
{
    static class WowPaths
    {
        static WowPaths()
        {
            if (WowBasePath == null)
            {
                var wowExePaths = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Wow.exe", System.IO.SearchOption.AllDirectories);

                if (wowExePaths.Length > 1)
                {
                    var paths = string.Join(", ", wowExePaths);
                    throw new Exception($"Es konnte mehr als eine Wow.exe gefunden werden: {paths}");
                }
                else if (wowExePaths.Length == 0)
                {
                    throw new Exception("Es konnte keine Wow.exe gefunden werden");
                }
                else
                {
                    WowExePath = wowExePaths.Single();

                    var pathElements = WowExePath.Split('\\');
                    var basePath = pathElements.Take(pathElements.Length - 1).ToArray();
                    WowBasePath = Path.Combine(basePath);

                    WowConfigPath = WowBasePath + "\\WTF\\Config.wtf";
                    WowAccountsPath = WowBasePath + "\\WTF\\Account\\";
                }
            }
        }

        public static string WowBasePath { get; set; }

        public static string WowExePath { get; set; }

        public static string WowConfigPath { get; set; }

        public static string WowAccountsPath { get; set; }
    }
}
