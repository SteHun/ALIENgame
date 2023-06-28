using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace ALIENgame
{
    public class MapDrawer
    {
        public bool slowDebugMode = false;

        private const int stopBlock = 1;
        private const float walkSpeed = 0.25f;

        private readonly int blockWidth = 50;
        private readonly int blockHeight = 50;
        public int[,] MapLayout;
        private string[] spriteString;
        private List<Sprite> sprites = new List<Sprite>();

        public int Xposition;
        public int Yposition;
        public float Xoffset;
        public float Yoffset;
        // up=0, down=1, left=2, right=3
        public int WalkingDirection = 0;
        public bool SteppedOnNewTile = false;

        private Texture2D playerTexture;
        private Texture2D mapTexture;
        private Song backgroundMusic1;
        private Song backgroundMusic2;
        public bool MusicOn = true;
        private bool usingBackgroundMusic1 = false;
        private string[] mapString;
        public string MapName;
        public string DisplayMapName;
        public ContentManager Content;
        public EventPlayer Event;
        public MapDrawer(int XstartPosition, int YstartPosition)
        {
            Xposition = XstartPosition;
            Yposition = YstartPosition;

            // This is mostly irrelevant and can be removed
        }

        public void Load(ContentManager content, EventPlayer events)
        {
            Content = content;
            playerTexture = Content.Load<Texture2D>("Content/Sprites/Player");
            Event = events;


        }
        //This time stamp in milisecondsis used to calculate the map introduction text. Use absolute time here.
        //If it is 0, no map text is shown
        public void LoadMap(string mapName, int x, int y, GameState state)
        {
            MapName = mapName;
            mapTexture = Content.Load<Texture2D>("Content/Maps/" + mapName);
            string[] fileString = System.IO.File.ReadAllText("Content/Maps/" + mapName + ".txt").Replace("\r", "").Split('\n');
            spriteString = System.IO.File.ReadAllText("Content/Maps/" + mapName + ".sprites.txt").Replace("\r", "").Split('\n');
            mapString = fileString[2..];
            if (usingBackgroundMusic1)
            {
                backgroundMusic2 = Content.Load<Song>("Content/Music/" + fileString[1]);
                MediaPlayer.Play(backgroundMusic2);
                usingBackgroundMusic1 = false;
            }
            else
            {
                backgroundMusic1 = Content.Load<Song>("Content/Music/" + fileString[1]);
                MediaPlayer.Play(backgroundMusic1);
                usingBackgroundMusic1 = true;
            }
            Xposition = x;
            Yposition = y;
            sprites = new List<Sprite>();
            for (int i = 0; i < spriteString.Length; i++)
            {
                string[] instructions = spriteString[i].Split(' ');
                if (instructions[0] == "LoadingZone")
                {
                    sprites.Add(new LoadingZone(int.Parse(instructions[1]), int.Parse(instructions[2]), int.Parse(instructions[3]), int.Parse(instructions[4]), instructions[5], int.Parse(instructions[6]), int.Parse(instructions[7])));
                }
                else if (instructions[0] == "NPC")
                {
                    sprites.Add(new NPC(int.Parse(instructions[1]), int.Parse(instructions[2]), instructions[3], instructions[4], Content));
                }
                else if (instructions[0] == "EventTrigger")
                {
                    sprites.Add(new EventTrigger(int.Parse(instructions[1]), int.Parse(instructions[2]), int.Parse(instructions[3]), int.Parse(instructions[4]), instructions[5], instructions[6], state));
                }
                else if (instructions[0] == "RandomEncounter")
                {
                    sprites.Add(new RandomEncounter(int.Parse(instructions[1]), int.Parse(instructions[2]), int.Parse(instructions[3]), int.Parse(instructions[4]), instructions[5], int.Parse(instructions[6])));
                }
                else if (instructions[0] == "RockWall")
                {
                    sprites.Add(new RockWall(int.Parse(instructions[1]), int.Parse(instructions[2]), state, Content));
                }
                else if (instructions[0] == "MagicBarrier")
                {
                    sprites.Add(new MagicBarrier(int.Parse(instructions[1]), int.Parse(instructions[2]), state, Content));
                }
                else if (instructions[0] == "GoldenKey")
                {
                    sprites.Add(new GoldenKey(int.Parse(instructions[1]), int.Parse(instructions[2]), state, Content));
                }
                else if (instructions[0] == "GoldenGate")
                {
                    sprites.Add(new GoldenGate(int.Parse(instructions[1]), int.Parse(instructions[2]), state, Content));
                }
                else if (instructions[0] == "Steve")
                {
                    sprites.Add(new Steve(int.Parse(instructions[1]), int.Parse(instructions[2]), Content));
                }
                else if (instructions[0] == "Rosetta")
                {
                    sprites.Add(new Rosetta(int.Parse(instructions[1]), int.Parse(instructions[2]), Content));
                }
            }
        }

        public void Teleport(int x, int y)
        {
            Xposition = x;
            Yposition = y;

        }

        public void ScrollUp()
        {
            if (Xoffset != 0 || Yoffset != 0)
                return;
            WalkingDirection = 0;
            if (mapString[Yposition - 1][Xposition].ToString() == stopBlock.ToString() ||
                PositionBlockedBySprite(Xposition, Yposition - 1))
                return;
            Yposition--;
            Yoffset = -blockHeight;
        }
        public void ScrollDown()
        {
            if (Xoffset != 0 || Yoffset != 0)
                return;
            WalkingDirection = 1;
            if (mapString[Yposition + 1][Xposition].ToString() == stopBlock.ToString() ||
                PositionBlockedBySprite(Xposition, Yposition + 1))
                return;
            Yposition++;
            Yoffset = blockHeight;
        }
        public void ScrollLeft()
        {
            if (Xoffset != 0 || Yoffset != 0)
                return;
            WalkingDirection = 2;
            if (mapString[Yposition][Xposition - 1].ToString() == stopBlock.ToString() ||
                PositionBlockedBySprite(Xposition - 1, Yposition))
                return;
            Xposition--;
            Xoffset = -blockWidth;
        }
        public void ScrollRight()
        {
            if (Xoffset != 0 || Yoffset != 0)
                return;
            WalkingDirection = 3;
            if (mapString[Yposition][Xposition + 1].ToString() == stopBlock.ToString() ||
                PositionBlockedBySprite(Xposition + 1, Yposition))
                return;
            Xposition++;
            Xoffset = blockWidth;
        }

        public void Update(GameTime gameTime, ControlUpdater control, GameState state, BattleSystem battle)
        {
            if (Event.EventIsActive)
                return;
            HandleMovevent(gameTime, control);
            for (int i = 0; i < sprites.Count; i++)
            {
                if (sprites[i].IsActivated(this, control, state) && Xoffset == 0 && Yoffset == 0)
                {
                    sprites[i].Activate(this, state, battle);
                }
            }
        }
        public void UpdateMusic()
        {
            if (MediaPlayer.State == MediaState.Stopped && MusicOn)
            {
                if (usingBackgroundMusic1)
                    MediaPlayer.Play(backgroundMusic1);
                else
                    MediaPlayer.Play(backgroundMusic2);
            }
            else if (MediaPlayer.State == MediaState.Paused && MusicOn)
                MediaPlayer.Resume();
        }
        private void HandleMovevent(GameTime gameTime, ControlUpdater control)
        {
            if (SteppedOnNewTile)
                SteppedOnNewTile = false;
            //check input
            if (control.UpHeld || control.DownHeld || control.LeftHeld || control.RightHeld)
            {
                if (control.DirectionalButtonOrder.Peek().ToString() == "down")
                    ScrollDown();
                else if (control.DirectionalButtonOrder.Peek().ToString() == "up")
                    ScrollUp();
                else if (control.DirectionalButtonOrder.Peek().ToString() == "left")
                    ScrollLeft();
                else if (control.DirectionalButtonOrder.Peek().ToString() == "right")
                    ScrollRight();
            }

            //handle scrolling
            if (Xoffset > 0)
            {
                if (Xoffset - gameTime.ElapsedGameTime.Milliseconds * walkSpeed < 0)
                {
                    Xoffset = 0;
                    SteppedOnNewTile = true;
                }
                else
                    Xoffset -= gameTime.ElapsedGameTime.Milliseconds * walkSpeed;
            }
            else if (Xoffset < 0)
            {
                if (Xoffset + gameTime.ElapsedGameTime.Milliseconds * walkSpeed > 0)
                {
                    Xoffset = 0;
                    SteppedOnNewTile = true;
                }
                else
                    Xoffset += gameTime.ElapsedGameTime.Milliseconds * walkSpeed;
            }
            else if (Yoffset > 0)
            {
                if (Yoffset - gameTime.ElapsedGameTime.Milliseconds * walkSpeed < 0)
                {
                    Yoffset = 0;
                    SteppedOnNewTile = true;
                }
                else
                    Yoffset -= gameTime.ElapsedGameTime.Milliseconds * walkSpeed;
            }
            else if (Yoffset < 0)
            {
                if (Yoffset + gameTime.ElapsedGameTime.Milliseconds * walkSpeed > 0)
                {
                    Yoffset = 0;
                    SteppedOnNewTile = true;
                }
                else
                    Yoffset += gameTime.ElapsedGameTime.Milliseconds * walkSpeed;
            }

        }

        public void Draw(SpriteBatch spriteBatch, Game1 game)
        {
            Rectangle playerRectangle = game.MakeScaledRectangle(
                Game1.BaseWindowWidth / 2 - blockWidth / 2,
                Game1.BaseWindowHeight / 2 - blockHeight / 2,
                blockWidth,
                blockHeight);
            spriteBatch.Draw(mapTexture, game.MakeScaledRectangle(
                Game1.BaseWindowWidth / 2 - blockWidth / 2 - blockWidth * Xposition + MathF.Round(Xoffset),
                Game1.BaseWindowHeight / 2 - blockHeight / 2 - blockHeight * Yposition + MathF.Round(Yoffset),
                mapTexture.Width,
                mapTexture.Height), Color.White);
            spriteBatch.Draw(playerTexture, playerRectangle, Color.White);
            for (int i = 0; i < sprites.Count; i++)
            {
                sprites[i].Draw(spriteBatch, this, game);
            }

            if (!slowDebugMode)
                return;
        }

        public bool PositionBlockedBySprite(int xpos, int ypos)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                if (xpos >= sprites[i].Xpos && xpos < sprites[i].Xpos + (sprites[i].Width)
                    && ypos >= sprites[i].Ypos && ypos < sprites[i].Ypos + (sprites[i].Height)
                    && sprites[i].StopsPlayer)
                    return true;
            }
            return false;
        }

        public Rectangle GetDrawingRectangle(int xposition, int yposition, int widthInPixels, int heightInPixels, Game1 game)
        {
            Rectangle output = game.MakeScaledRectangle(
                Game1.BaseWindowWidth / 2 - blockWidth / 2 - blockWidth * Xposition + blockWidth * xposition + MathF.Round(Xoffset),
                Game1.BaseWindowHeight / 2 - blockHeight / 2 - blockHeight * Yposition + blockHeight * yposition + MathF.Round(Yoffset),
                widthInPixels, heightInPixels);
            return output;
        }

    }

}
