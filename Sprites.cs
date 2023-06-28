using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ALIENgame
{
    public abstract class Sprite
    {
        public int Xpos;
        public int Ypos;
        public int Width = 1;
        public int Height = 1;
        public bool StopsPlayer;
        public abstract bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state);
        public abstract void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle);
        public abstract void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game);
    }
    public class LoadingZone : Sprite
    {
        public string DestinationMap;
        public int DestinationXpos;
        public int DestinationYpos;
        public LoadingZone(int xpos, int ypos, int width, int height, string destinationMap, int destinationXpos, int destinationYpos)
        {
            StopsPlayer = false;

            Xpos = xpos;
            Ypos = ypos;
            Width = width;
            Height = height;
            DestinationMap = destinationMap;
            DestinationXpos = destinationXpos;
            DestinationYpos = destinationYpos;
        }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game) { }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            if (mapDrawer.Xposition - Xpos < Width && mapDrawer.Xposition - Xpos >= 0 && mapDrawer.Yposition - Ypos < Height && mapDrawer.Yposition - Ypos >= 0)
                return true;
            return false;
        }
        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle)
        {
            mapDrawer.LoadMap(DestinationMap, DestinationXpos, DestinationYpos, state);
        }
    }

    public class EventTrigger : Sprite
    {
        public string EventToTrigger;
        public string FlagToSet;

        public EventTrigger(int xpos, int ypos, int width, int height, string eventToTrigger, string flagToSet, GameState state)
        {
            StopsPlayer = false;

            Xpos = xpos;
            Ypos = ypos;
            Width = width;
            Height = height;
            EventToTrigger = eventToTrigger;
            FlagToSet = flagToSet;
            state.StoryFlags.TryAdd(FlagToSet, false);
        }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game) { }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            if (state.StoryFlags[FlagToSet] == true)
                return false;
            if (mapDrawer.Xposition - Xpos < Width && mapDrawer.Xposition - Xpos >= 0 && mapDrawer.Yposition - Ypos < Height && mapDrawer.Yposition - Ypos >= 0)
                return true;
            return false;
        }
        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle)
        {
            mapDrawer.Event.StartEvent(EventToTrigger);
            state.StoryFlags[FlagToSet] = true;
        }

    }

    public class RandomEncounter : Sprite
    {
        public string Formation;
        public int Chanse;
        private Random rng;

        public RandomEncounter(int xpos, int ypos, int width, int height, string formation, int chanse)
        {
            Xpos = xpos;
            Ypos = ypos;
            Width = width;
            Height = height;
            Formation = formation;
            Chanse = chanse;
            rng = new Random();
        }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game) { }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            if (mapDrawer.SteppedOnNewTile && mapDrawer.Xposition - Xpos < Width && mapDrawer.Xposition - Xpos >= 0 && mapDrawer.Yposition - Ypos < Height && mapDrawer.Yposition - Ypos >= 0 && !mapDrawer.Event.EventIsActive)
                return true;
            return false;
        }

        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle)
        {
            if (rng.Next(100) + 1 <= Chanse)
            {
                battle.StartBattle(Formation);
            }
        }

    }

    public class NPC : Sprite
    {
        public string EventWhenTalkedTo;
        private Texture2D texture;
        public NPC(int Xpos, int Ypos, string eventWhenTalkedTo, string appearance, ContentManager content)
        {
            StopsPlayer = true;

            this.Xpos = Xpos;
            this.Ypos = Ypos;
            EventWhenTalkedTo = eventWhenTalkedTo;
            texture = content.Load<Texture2D>("Content/Sprites/" + appearance);
        }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            if (!control.ConfirmPressed)
                return false;
            if (mapDrawer.WalkingDirection == 0)
                return mapDrawer.Xposition == Xpos && mapDrawer.Yposition - 1 == Ypos;
            if (mapDrawer.WalkingDirection == 1)
                return mapDrawer.Xposition == Xpos && mapDrawer.Yposition + 1 == Ypos;
            if (mapDrawer.WalkingDirection == 2)
                return mapDrawer.Xposition - 1 == Xpos && mapDrawer.Yposition == Ypos;
            if (mapDrawer.WalkingDirection == 3)
                return mapDrawer.Xposition + 1 == Xpos && mapDrawer.Yposition == Ypos;
            return false;
        }
        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle)
        {
            if (!mapDrawer.Event.EventIsActive)
                mapDrawer.Event.StartEvent(EventWhenTalkedTo);
        }

        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game)
        {
            spriteBatch.Draw(texture, mapDrawer.GetDrawingRectangle(Xpos, Ypos, texture.Width, texture.Height, game), Color.White);
        }
    }
    public class RockWall : Sprite
    {
        // this assumes that there is only one rock wall in the game
        private const string fileName = "Content/Sprites/RockWall";
        private const string flagToSet = "RockWallDestroyed";
        private Texture2D texture;
        public bool WallIsUp;
        public RockWall(int x, int y, GameState state, ContentManager contentManager)
        {
            Width = 5;
            Height = 2;
            Xpos = x;
            Ypos = y;
            state.StoryFlags.TryAdd(flagToSet, false);
            WallIsUp = !state.StoryFlags[flagToSet];
            StopsPlayer = WallIsUp;
            texture = contentManager.Load<Texture2D>(fileName);
        }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            //This assumes that the room has a specific shape, too lazy to make it bettter
            return WallIsUp && mapDrawer.Yposition == Ypos - 1;
        }

        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle)
        {
            if (state.PlayersActive >= 2)
            {
                WallIsUp = false;
                state.StoryFlags[flagToSet] = true;
                StopsPlayer = false;
            }
        }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game)
        {
            if (WallIsUp)
                spriteBatch.Draw(texture, mapDrawer.GetDrawingRectangle(Xpos, Ypos, texture.Width, texture.Height, game), Color.White);
        }
    }

    public class MagicBarrier : Sprite
    {
        private const string fileName = "Content/Sprites/MagicBarrier";
        private const string flagToSet = "MagicBarrierDestroyed";
        private Texture2D texture;
        public bool BarrierIsUp;
        public MagicBarrier(int x, int y, GameState state, ContentManager contentManager)
        {
            Width = 2;
            Height = 5;
            Xpos = x;
            Ypos = y;
            state.StoryFlags.TryAdd(flagToSet, false);
            BarrierIsUp = !state.StoryFlags[flagToSet];
            StopsPlayer = BarrierIsUp;
            texture = contentManager.Load<Texture2D>(fileName);
        }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            //This assumes that the room has a specific shape, too lazy to make it bettter
            return BarrierIsUp && mapDrawer.Xposition == Xpos - 1;
        }
        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle)
        {
            if (state.PlayersActive >= 3)
            {
                BarrierIsUp = false;
                state.StoryFlags[flagToSet] = true;
                StopsPlayer = false;
            }
        }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game)
        {
            if (BarrierIsUp)
                spriteBatch.Draw(texture, mapDrawer.GetDrawingRectangle(Xpos, Ypos, texture.Width, texture.Height, game), Color.White);
        }
    }
    public class GoldenKey : Sprite
    {
        public bool PickedUp;
        private const string flagToSetWhenPickedUp = "PickedUpGoldenKey";
        private const string fileName = "Content/Sprites/GoldenKey";
        private Texture2D texture;
        public GoldenKey(int xpos, int ypos, GameState state, ContentManager contentManager)
        {
            StopsPlayer = false;
            Xpos = xpos;
            Ypos = ypos;
            Width = 1;
            Height = 1;
            state.StoryFlags.TryAdd(flagToSetWhenPickedUp, false);
            PickedUp = state.StoryFlags[flagToSetWhenPickedUp];
            texture = contentManager.Load<Texture2D>(fileName);
        }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            return !PickedUp && mapDrawer.Xposition == Xpos && mapDrawer.Yposition == Ypos;
        }
        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle)
        {
            PickedUp = true;
            state.StoryFlags[flagToSetWhenPickedUp] = true;
        }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game)
        {
            if (!PickedUp)
                spriteBatch.Draw(texture, mapDrawer.GetDrawingRectangle(Xpos, Ypos, texture.Width, texture.Height, game), Color.White);
        }
    }
    public class GoldenGate : Sprite
    {
        public bool GateIsActive;
        private const string flagToCheck = "PickedUpGoldenKey";
        private const string gateIsOpenFlag = "RemovedGoldenGate";
        private const string fileName = "Content/Sprites/GoldenGate";
        private Texture2D texture;
        public GoldenGate(int xpos, int ypos, GameState state, ContentManager content)
        {
            Width = 5;
            Height = 2;
            Xpos = xpos;
            Ypos = ypos;
            state.StoryFlags.TryAdd(flagToCheck, false);
            state.StoryFlags.TryAdd(gateIsOpenFlag, false);
            GateIsActive = !state.StoryFlags[gateIsOpenFlag];
            StopsPlayer = GateIsActive;
            texture = content.Load<Texture2D>(fileName);
        }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            return GateIsActive && state.StoryFlags[flagToCheck] && mapDrawer.Yposition == Ypos - 1;
        }
        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle)
        {
            state.StoryFlags[gateIsOpenFlag] = true;
            StopsPlayer = false;
            GateIsActive = false;
        }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game)
        {
            if (GateIsActive)
                spriteBatch.Draw(texture, mapDrawer.GetDrawingRectangle(Xpos, Ypos, texture.Width, texture.Height, game), Color.White);
        }
    }
    public class Steve : Sprite
    {
        private Texture2D texture;
        public Steve(int xPos, int yPos, ContentManager content)
        {
            texture = content.Load<Texture2D>("Content/Sprites/Steve");
            StopsPlayer = false;
            Xpos = xPos;
            Ypos = yPos;
            Width = 1;
            Height = 1;
        }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            return false;
        }
        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle) { }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game)
        {
            if (game.State.PlayersActive == 1)
                spriteBatch.Draw(texture, mapDrawer.GetDrawingRectangle(Xpos, Ypos, texture.Width, texture.Height, game), Color.White);
        }
    }
    public class Rosetta : Sprite
    {
        private Texture2D texture;
        public Rosetta(int xPos, int yPos, ContentManager content)
        {
            texture = content.Load<Texture2D>("Content/Sprites/Rosetta");
            StopsPlayer = false;
            Xpos = xPos;
            Ypos = yPos;
            Width = 1;
            Height = 1;
        }
        public override bool IsActivated(MapDrawer mapDrawer, ControlUpdater control, GameState state)
        {
            return false;
        }
        public override void Activate(MapDrawer mapDrawer, GameState state, BattleSystem battle) { }
        public override void Draw(SpriteBatch spriteBatch, MapDrawer mapDrawer, Game1 game)
        {
            if (game.State.PlayersActive == 2)
                spriteBatch.Draw(texture, mapDrawer.GetDrawingRectangle(Xpos, Ypos, texture.Width, texture.Height, game), Color.White);
        }
    }
}
