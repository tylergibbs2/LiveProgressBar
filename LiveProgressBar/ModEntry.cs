using System;
using StardewModdingAPI;
using StardewValley;

namespace LiveProgressBar
{
    public class ModEntry : Mod
    {
        private ModConfig config;
        private bool menuVisible = true;
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
            this.config = this.Helper.ReadConfig<ModConfig>();
            this.lastProgress = 0f;
            this.progressHUD = new ProgressHUD(this.lastProgress);
            this.progressHUD.SetVisible(this.menuVisible);
            Game1.onScreenMenus.Add(this.progressHUD);
        }

        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (!String.IsNullOrEmpty(config.ToggleKey))
            {
                try
                {
                    SButton key = (SButton)Enum.Parse(typeof(SButton), config.ToggleKey);
                    SButtonState state = this.Helper.Input.GetState(key);
                    if (state == SButtonState.Released)
                    {
                        this.menuVisible = !this.menuVisible;
                        this.progressHUD.SetVisible(this.menuVisible);
                    }
                }
                catch (ArgumentNullException ex) { }
                catch (ArgumentException ex) { }
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
