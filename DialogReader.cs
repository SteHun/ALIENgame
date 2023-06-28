using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ALIENgame
{
    public class DialogReader
    {
        private const int dialogBoxWidth = 640;
        private const int dialogBoxHeight = 150;
        private const int dialogBoxPaddingWidth = 20;
        private const int dialogBoxPaddingHeight = 20;
        private Vector2 origin = new Vector2(0, 0);

        private const double textCrawlTimeNormal = 20;
        private const double textCrawlTimeWait = 250;
        private const double textCrawlSoundTime = 35;

        private string[] dialog;
        private string currentDialog;
        private string currentDisplayerdDialog;
        public bool BoxIsActive = false;
        public bool DialogCrawlActive = false;
        private bool canExitBox = true;
        private int activeDialogNumber = 0;
        private int currentLetter = 0;
        private double timeToNextCrawl = 0;
        private double timeToNextCrawlSound = 0;

        private SpriteFont font;
        private SpriteFont normalFont;
        private SpriteFont monoFont;
        private Texture2D dialogBox;
        private SoundEffect crawlSFX;

        private Rectangle dialogBoxBox;//Chose this name because its funny, might regret later
        private Vector2 textPosition;
        public DialogReader(int baseWindowWidth, int baseWindowHeight)
        {
            dialogBoxBox = new Rectangle(0, baseWindowHeight - dialogBoxHeight, dialogBoxWidth, dialogBoxHeight);
            textPosition = new Vector2(dialogBoxBox.X + dialogBoxPaddingWidth, dialogBoxBox.Y + dialogBoxPaddingHeight);
        }

        public void Load(SpriteFont normalFont, SpriteFont monoFont, ContentManager content)
        {
            string[] tempDialog = System.IO.File.ReadAllText("Content/Dialog.txt").Replace("\r", "").Split('\n');
            dialog = new string[tempDialog.Length + 1];
            dialog[0] = "0 IS NOT A VALID INDEX";
            tempDialog.CopyTo(dialog, 1);
            currentDialog = tempDialog[0];

            this.normalFont = normalFont;
            this.monoFont = monoFont;
            font = normalFont;
            dialogBox = content.Load<Texture2D>("Content/UIAssets/DialogBox");
            crawlSFX = content.Load<SoundEffect>("Content/SFX/TextCrawl");
        }

        public void Update(ControlUpdater controls, GameTime gameTime)
        {
            if (!BoxIsActive)
                return;
            if (!DialogCrawlActive)
            {
                if (controls.ConfirmPressed && canExitBox)
                    CloseBox();
                return;
            }
            timeToNextCrawl -= gameTime.ElapsedGameTime.TotalMilliseconds;
            timeToNextCrawlSound -= gameTime.ElapsedGameTime.TotalMilliseconds;

            if (controls.CancelPressed)
            {
                currentDisplayerdDialog = currentDialog.Replace("&w", "");
                DialogCrawlActive = false;
                return;
            }

            if (timeToNextCrawl > 0)
                return;
            if (currentLetter + 1 < currentDialog.Length && currentDialog[currentLetter..(currentLetter + 2)] == "&w")
            {
                timeToNextCrawl = textCrawlTimeWait - textCrawlTimeNormal;
                currentLetter += 2;
                return;
            }
            if (timeToNextCrawlSound <= 0)
            {
                crawlSFX.Play();
                timeToNextCrawlSound = textCrawlSoundTime;
            }


            while (timeToNextCrawl <= 0)
            {
                currentDisplayerdDialog += currentDialog[currentLetter];

                currentLetter++;

                timeToNextCrawl += textCrawlTimeNormal;
                if (currentLetter >= currentDialog.Length)
                {
                    DialogCrawlActive = false;
                    break;
                }
            }

        }

        public void Draw(SpriteBatch spriteBatch, Game1 game)
        {
            if (!BoxIsActive)
                return;
            spriteBatch.Draw(dialogBox, game.GetScaledRectangle(dialogBoxBox), Color.White);
            spriteBatch.DrawString(font, currentDisplayerdDialog, game.GetScaledVector(textPosition), Color.White, 0, origin, (float)game.ScalingFactor, SpriteEffects.None, 1);
        }

        public void ActivateBox(int dialogNumber, bool canExitBox = true, bool textScroll = true, bool useMonoFont = false)
        {
            if (useMonoFont)
                font = monoFont;
            else
                font = normalFont;
            activeDialogNumber = dialogNumber;
            try
            {
                currentDialog = dialog[activeDialogNumber].Replace("&n", "\n");
            }
            catch (IndexOutOfRangeException)
            {
                throw new FormatException();
            }
            if (textScroll)
                currentDisplayerdDialog = "";
            else
                currentDisplayerdDialog = currentDialog;
            BoxIsActive = true;
            this.canExitBox = canExitBox;
            DialogCrawlActive = textScroll;
            currentLetter = 0;
            timeToNextCrawl = textCrawlTimeNormal;
            timeToNextCrawlSound = textCrawlSoundTime;
        }

        public void ActivateBoxCustom(string dialogString, bool canExitBox = true, bool textScroll = true, bool useMonoFont = false)
        {
            if (useMonoFont)
                font = monoFont;
            else
                font = normalFont;
            activeDialogNumber = 0;
            currentDialog = dialogString.Replace("&n", "\n");
            if (textScroll)
                currentDisplayerdDialog = "";
            else
                currentDisplayerdDialog = currentDialog;
            BoxIsActive = true;
            this.canExitBox = canExitBox;
            DialogCrawlActive = textScroll;
            currentLetter = 0;
            timeToNextCrawl = textCrawlTimeNormal;
            timeToNextCrawlSound = textCrawlSoundTime;
        }

        public void CloseBox()
        {
            BoxIsActive = false;
            DialogCrawlActive = false;
        }

        public string GetDialogEntry(int dialogNumber)
        {
            return dialog[dialogNumber];
        }


    }
}
