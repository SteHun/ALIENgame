using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ALIENgame
{
    internal static class DebugTools
    {
        public static void DrawControlDebugMessage(SpriteBatch spriteBatch, SpriteFont basicFont, ControlUpdater Controls)
        {
            float height = basicFont.MeasureString("Confirm").Y;
            float indicatorXPosition = basicFont.MeasureString("Confirm").X + 25;
            float indicator2XPosition = basicFont.MeasureString("Confirm").X + 50;
            spriteBatch.DrawString(basicFont, "Confirm:", new Vector2(0, 0), Color.White);
            spriteBatch.DrawString(basicFont, "Cancel:", new Vector2(0, height), Color.White);
            spriteBatch.DrawString(basicFont, "Menu:", new Vector2(0, height * 2), Color.White);
            spriteBatch.DrawString(basicFont, "Up:", new Vector2(0, height * 3), Color.White);
            spriteBatch.DrawString(basicFont, "Left:", new Vector2(0, height * 4), Color.White);
            spriteBatch.DrawString(basicFont, "Down:", new Vector2(0, height * 5), Color.White);
            spriteBatch.DrawString(basicFont, "Right:", new Vector2(0, height * 6), Color.White);

            if (Controls.ConfirmHeld)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicatorXPosition, 0), Color.White);
            if (Controls.CancelHeld)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicatorXPosition, height), Color.White);
            if (Controls.MenuHeld)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicatorXPosition, height * 2), Color.White);
            if (Controls.UpHeld)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicatorXPosition, height * 3), Color.White);
            if (Controls.LeftHeld)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicatorXPosition, height * 4), Color.White);
            if (Controls.DownHeld)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicatorXPosition, height * 5), Color.White);
            if (Controls.RightHeld)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicatorXPosition, height * 6), Color.White);

            if (Controls.ConfirmPressed)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicator2XPosition, 0), Color.White);
            if (Controls.CancelPressed)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicator2XPosition, height), Color.White);
            if (Controls.MenuPressed)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicator2XPosition, height * 2), Color.White);
            if (Controls.UpPressed)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicator2XPosition, height * 3), Color.White);
            if (Controls.LeftPressed)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicator2XPosition, height * 4), Color.White);
            if (Controls.DownPressed)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicator2XPosition, height * 5), Color.White);
            if (Controls.RightPressed)
                spriteBatch.DrawString(basicFont, "X", new Vector2(indicator2XPosition, height * 6), Color.White);

        }
        public static void TestResolution(SpriteBatch spriteBatch, Texture2D testImage, Game1 gameObject)
        {
            spriteBatch.Draw(testImage, gameObject.GetScaledRectangle(new Rectangle(0, 0, Game1.BaseWindowWidth, Game1.BaseWindowHeight)), Color.White);
        }
    }
}