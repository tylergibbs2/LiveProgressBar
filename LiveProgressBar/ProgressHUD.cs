using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Collections.Generic;

namespace LiveProgressBar
{
    class ProgressHUD : IClickableMenu
    {
        private float progress;
        private bool showExtra = false;

        private int extraWidth;
        private int extraHeight;

        private ClickableComponent percentClick;

        public ProgressHUD(float progress)
        {
            this.width = 200;
            this.height = 175;

            this.extraWidth = 550;
            this.extraHeight = 585;

            this.progress = progress;
            this.CalcPositions();
            this.percentClick = new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), "percent");
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.CalcPositions();
            this.percentClick = new ClickableComponent(new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), "percent");
        }

        private void CalcPositions()
        {
            this.xPositionOnScreen = Game1.uiViewport.Width - this.width;
            this.yPositionOnScreen = (Game1.uiViewport.Height / 2) - this.height;
        }
        public void SetProgress(float progress)
        {
            this.progress = progress;
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.showExtra = !this.showExtra;
        }

        private List<string> GetExtraStrings()
        {
            List<string> strings = new List<string>();

            float ItemsShippedPrct = Math.Min(0f + Utility.GetFarmCompletion((Farmer farmer) => Utility.getFarmerItemsShippedPercent(farmer)).Value, 1f);
            float ObelisksPrct = Math.Min(Utility.numObelisksOnFarm() / 4f, 1f);
            float GoldClockPrct = Math.Min((Game1.getFarm().isBuildingConstructed("Gold Clock") ? 1f : 0f), 1f);
            float SlayerQuestsPrct = Math.Min((Utility.GetFarmCompletion((Farmer farmer) => farmer.hasCompletedAllMonsterSlayerQuests.Value).Value ? 1f : 0f), 1f);
            float MaxFriendshipPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getMaxedFriendshipPercent(farmer)).Value, 1f);
            float LevelPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Math.Min(farmer.Level, 25f) / 25f).Value, 1f);
            float StardropsPrct = Math.Min((Utility.GetFarmCompletion((Farmer farmer) => Utility.foundAllStardrops(farmer)).Value ? 1f : 0f), 1f);
            float CookedRecipesPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getCookedRecipesPercent(farmer)).Value, 1f);
            float CraftedRecipesPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getCraftedRecipesPercent(farmer)).Value, 1f);
            float FishCaughtPrct = Math.Min(Utility.GetFarmCompletion((Farmer farmer) => Utility.getFishCaughtPercent(farmer)).Value, 1f);

            float totalNuts = 130f;
            float walnutsFound = Math.Min((int)Game1.netWorldState.Value.GoldenWalnutsFound, totalNuts);

            float WalnutsPrct = walnutsFound / totalNuts;

            strings.Add(string.Format("Items Shipped: {0:P2}", ItemsShippedPrct));
            strings.Add(string.Format("Obelisks: {0:P2}", ObelisksPrct));
            strings.Add(string.Format("Gold Clock: {0:P2}", GoldClockPrct));
            strings.Add(string.Format("Slayer Quests: {0:P2}", SlayerQuestsPrct));
            strings.Add(string.Format("Max Friendships: {0:P2}", MaxFriendshipPrct));
            strings.Add(string.Format("Farmer Level: {0:P2}", LevelPrct));
            strings.Add(string.Format("All Stardrops: {0:P2}", StardropsPrct));
            strings.Add(string.Format("Cooked Recipes: {0:P2}", CookedRecipesPrct));
            strings.Add(string.Format("Crafted Recipes: {0:P2}", CraftedRecipesPrct));
            strings.Add(string.Format("Fish Caught: {0:P2}", FishCaughtPrct));
            strings.Add(string.Format("Golden Walnuts: {0:P2}", WalnutsPrct));

            return strings;
        }

        public override void draw(SpriteBatch b)
        {
            // removes the leading space
            System.Globalization.CultureInfo newCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            newCulture.NumberFormat.PercentPositivePattern = 1;  // Avoid putting a space between a number and its percentage
            System.Threading.Thread.CurrentThread.CurrentCulture = newCulture;

            String percentString;
            if (this.progress >= 1f)
            {
                percentString = "100%";
            } else
            {
                percentString = string.Format("{0:P2}", this.progress);
            }
    
            Vector2 textPos = new Vector2(this.xPositionOnScreen + (this.width / 3) - 25, this.yPositionOnScreen + (this.height / 2) + 10);

            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);
            Utility.drawTextWithShadow(b, percentString, Game1.dialogueFont, textPos, Game1.textColor);

            if (this.showExtra)
            {
                int startingX = this.xPositionOnScreen - this.extraWidth;
                int startingY = (Game1.uiViewport.Height / 2) - this.height;
                Game1.drawDialogueBox(startingX, startingY, this.extraWidth, this.extraHeight, false, true);
                textPos = new Vector2(startingX + 40, startingY + 100);
                foreach (string stat in this.GetExtraStrings())
                {
                    Utility.drawTextWithShadow(b, stat, Game1.dialogueFont, textPos, Game1.textColor);
                    textPos.Y += 40;
                }
            }

            drawMouse(b);
        }
    }
}
