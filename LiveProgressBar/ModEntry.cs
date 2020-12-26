using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace LiveProgressBar
{
    public class ModEntry : Mod
    {
        private float lastProgress;
        private ProgressHUD progressHUD;
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;

            helper.ConsoleCommands.Add("progress", "Sets a fake progress percentage for testing.", this.SetProgressCmd);
        }


        private void SetProgressCmd(string command, string[] args)
        {
            this.progressHUD.SetProgress(float.Parse(args[0]));
        }


        private void OnSaveLoaded(object sender, EventArgs e)
        {
            this.lastProgress = 0f;
            this.progressHUD = new ProgressHUD(this.lastProgress);
            Game1.onScreenMenus.Add(this.progressHUD);
        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            float latestProgress = Utility.percentGameComplete();
            if (latestProgress == this.lastProgress)
            {
                return;
            }

            String percentString = String.Format("Progress Changed: {0:P2}.", latestProgress);

            this.Monitor.Log(percentString, LogLevel.Info);
            this.lastProgress = latestProgress;

            this.progressHUD.SetProgress(latestProgress);
        }
    }
}
