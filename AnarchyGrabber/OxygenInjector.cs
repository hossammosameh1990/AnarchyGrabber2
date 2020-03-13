using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace AnarchyGrabber
{
    public class OxygenInjector
    {
        public static bool Inject(DiscordBuild build, string dirName, string indexFile, string prependJS, params OxygenFile[] files)
        {
            if (!TryGetDiscordPath(build, out string path))
                return false;

            try
            {
                DirectoryInfo epicDir = Directory.CreateDirectory($"{path}/{dirName}");

                string newContents = File.ReadAllText(path + "/index.js").Replace("require(process.env.modDir + '\\\\inject')", "");

                newContents += $@"
{prependJS}
process.env.modDir = '{epicDir.FullName.Replace("\\", "\\\\")}'
require(process.env.modDir + '\\{indexFile}')";

                File.WriteAllText(path + "/index.js", newContents);

                foreach (var file in files)
                    File.WriteAllText($"{epicDir.FullName}/{file.Path}", file.Contents);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool Eject(DiscordBuild build, string dirName)
        {
            if (!TryGetDiscordPath(build, out string path))
                return false;

            try
            {
                File.WriteAllText(path + "/index.js", "module.exports = require('./core.asar');");

                Directory.Delete($"{path}/{dirName}", true);

                return true;
            }
            catch
            {
                return false;
            }
        }


        public static bool TryGetDiscordPath(DiscordBuild build, out string path)
        {
            string buildStr;

            switch (build)
            {
                case DiscordBuild.Discord:
                    buildStr = "Discord";
                    break;
                case DiscordBuild.DiscordCanary:
                    buildStr = "discordcanary";
                    break;
                case DiscordBuild.DiscordPTB:
                    buildStr = "discordptb";
                    break;
                default:
                    path = null;
                    return false;
            }

            try
            {
                path = new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{buildStr}").GetDirectories()
                    .First(d => Regex.IsMatch(d.Name, @"\d.\d.\d{2}(\d|$)"))
                    .GetDirectories().First(d => d.Name == "modules")
                    .GetDirectories().First(d => d.Name == "discord_desktop_core").FullName;

                return true;
            }
            catch
            {
                path = null;
                return false;
            }
        }


        public static void RestartDiscord()
        {
            foreach (var proc in Process.GetProcessesByName("Discord"))
                proc.Kill();

            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Microsoft\Windows\Start Menu\Programs\Discord Inc\Discord.lnk");
        }
    }
}
