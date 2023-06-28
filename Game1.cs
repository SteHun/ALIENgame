using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Media;
//NOTE: if your're using a debug function like Debug.WriteLine, include if if debug statement as shown here
#if DEBUG
using System.Diagnostics;
#endif

namespace ALIENgame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public const string SaveFileName = "SafeGame.sav";
    public bool InFileMenu = true;
    readonly RasterizerState rasterizerState = new RasterizerState() { MultiSampleAntiAlias = true };

    public ControlUpdater Controls;
    public DialogReader Dialogs;
    public EventPlayer Events;
    public MapDrawer Map;
    public GameState State;
    public BattleSystem Battle;
    public ItemManager Items;
    private SpriteFont determinationSans;
    public SpriteFont DeterminationMono;
    private Texture2D titleScreen;
    private Texture2D borderImage;
    public Texture2D testImage;
    private Texture2D credits;
    public Song CreditSong;

    public int XDrawOffset = 0;
    public int YDrawOffset = 0;
    public double ScalingFactor = 1;
    public Rectangle LeftBorder = new Rectangle(0, 0, 0, 0);
    public Rectangle RightBorder = new Rectangle(0, 0, 0, 0);
    public Color BorderColor = Color.Black;
    public const int BaseWindowWidth = 640;
    public const int BaseWindowHeight = 480;
    public bool GameWon = false;

    private const double exitTimerDuration = 2000;
    private double exitTimer = exitTimerDuration;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        _graphics.PreferredBackBufferWidth = BaseWindowWidth;
        _graphics.PreferredBackBufferHeight = BaseWindowHeight;
        _graphics.IsFullScreen = false;
        IsMouseVisible = false;
    }

    protected override void Initialize()
    {
        Controls = new ControlUpdater();
        Dialogs = new DialogReader(BaseWindowWidth, BaseWindowHeight);
        Events = new EventPlayer();
        Map = new MapDrawer(3, 3);
        MediaPlayer.Volume = 0.5f;
        //EnableFullscreen();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        Map.Load(Content, Events);
        determinationSans = Content.Load<SpriteFont>("Content/Fonts/DeterminationSans");
        DeterminationMono = Content.Load<SpriteFont>("Content/Fonts/DeterminationMono");
        borderImage = Content.Load<Texture2D>("Content/Colors/Black");
        titleScreen = Content.Load<Texture2D>("Content/UIAssets/TitleScreen");
        testImage = Content.Load<Texture2D>("Content/TestingAssets/WindowSizeTester");
        credits = Content.Load<Texture2D>("Content/UIAssets/Credits");
        CreditSong = Content.Load<Song>("Content/Music/Credits");
        Dialogs.Load(determinationSans, DeterminationMono, Content);
        Events.Initialize(Dialogs);
        Battle = new BattleSystem(Content);
        Items = new ItemManager(this);
    }

    protected override void Update(GameTime gameTime)
    {
        Controls.Update();
        if (Controls.ExitHeld)
        {
            exitTimer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (exitTimer <= 0)
                Exit();
        }
        else
        {
            exitTimer = exitTimerDuration;
        }

        if (Controls.FullscreenPressed)
        {
            FullscreenToggle();
        }
        if (GameWon)
            return;

        if (InFileMenu)
        {
            FileMenuUpdate();
            return;
        }

        if (Battle.BattleActive)
        {
            Dialogs.Update(Controls, gameTime);
            Battle.Update(this, gameTime);
        }
        else
        {
            Items.Update(this);
            Map.UpdateMusic();
            if (!Items.InventoryOpen)
                Map.Update(gameTime, Controls, State, Battle);
            Dialogs.Update(Controls, gameTime);
        }
        Events.Update(State, Battle, this);


        base.Update(gameTime);
    }

    protected void FileMenuUpdate()
    {
        if (Controls.ConfirmPressed && File.Exists(SaveFileName))
        {
            LoadGame();
            InFileMenu = false;
        }
        else if (Controls.CancelPressed)
        {
            NewGame();
            InFileMenu = false;
        }
    }


    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin(rasterizerState: this.rasterizerState, samplerState: SamplerState.PointClamp);
        if (GameWon)
        {
            _spriteBatch.Draw(credits, MakeScaledRectangle(0, 0, BaseWindowWidth, BaseWindowHeight), Color.White);
        }
        else if (InFileMenu)
        {
            _spriteBatch.Draw(titleScreen, MakeScaledRectangle(0, 0, BaseWindowWidth, BaseWindowHeight), Color.White);
        }
        else
        {
            if (Battle.BattleActive)
            {
                Battle.DrawBackground(_spriteBatch, this);
                Dialogs.Draw(_spriteBatch, this);
                Battle.Draw(_spriteBatch, this, gameTime);
            }
            else
            {
                Map.Draw(_spriteBatch, this);
                Items.Draw(_spriteBatch);
                Dialogs.Draw(_spriteBatch, this);
            }
        }

        _spriteBatch.Draw(borderImage, LeftBorder, Color.White);
        _spriteBatch.Draw(borderImage, RightBorder, Color.White);



        _spriteBatch.End();


        base.Draw(gameTime);
    }

    public Rectangle GetScaledRectangle(Rectangle rectangle)
    {
        rectangle.X = (int)Math.Round(rectangle.X * ScalingFactor);
        rectangle.Y = (int)Math.Round(rectangle.Y * ScalingFactor);
        rectangle.X += XDrawOffset;
        rectangle.Y += YDrawOffset;
        rectangle.Width = (int)Math.Round(rectangle.Width * ScalingFactor);
        rectangle.Height = (int)Math.Round(rectangle.Height * ScalingFactor);
        return rectangle;
    }

    public Rectangle MakeScaledRectangle(double x, double y, double width, double height)
    {
        int xAsInt = (int)Math.Round(x * ScalingFactor) + XDrawOffset;
        int yAsInt = (int)Math.Round(y * ScalingFactor) + YDrawOffset;
        int widthAsInt = (int)Math.Round(width * ScalingFactor);
        int heightAsInt = (int)Math.Round(height * ScalingFactor);
        return new Rectangle(xAsInt, yAsInt, widthAsInt, heightAsInt);
    }

    public Vector2 GetScaledVector(Vector2 vector)
    {
        vector.X *= (float)ScalingFactor;
        vector.Y *= (float)ScalingFactor;
        vector.X += XDrawOffset;
        vector.Y += YDrawOffset;

        return vector;
    }

    private void FullscreenToggle()
    {
        if (_graphics.IsFullScreen)
            DisableFullscreen();
        else
            EnableFullscreen();
    }

    private void DisableFullscreen()
    {
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
        _graphics.PreferredBackBufferWidth = BaseWindowWidth;
        _graphics.PreferredBackBufferHeight = BaseWindowHeight;
        _graphics.ApplyChanges();
        XDrawOffset = 0;
        YDrawOffset = 0;
        ScalingFactor = 1;
        LeftBorder = new Rectangle(0, 0, 0, 0);
        RightBorder = new Rectangle(0, 0, 0, 0);
    }

    private void EnableFullscreen()
    {
        int displayWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        int displayHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        if (displayWidth < BaseWindowWidth || displayHeight < BaseWindowHeight)
            return;
        _graphics.PreferredBackBufferWidth = displayWidth;
        _graphics.PreferredBackBufferHeight = displayHeight;
        _graphics.ApplyChanges();
        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();
        float windowWidthFactor = (float)displayWidth / (float)BaseWindowWidth;
        float windowHeightFactor = (float)displayHeight / (float)BaseWindowHeight;
        if (windowHeightFactor < windowWidthFactor)
        {
            ScalingFactor = windowHeightFactor;
            float newWindowWidth = BaseWindowWidth * windowHeightFactor;
            float newWindowHeight = displayHeight;
            XDrawOffset = (int)Math.Round((displayWidth - newWindowWidth) / 2);
            YDrawOffset = 0;
            LeftBorder = new Rectangle(0, 0, XDrawOffset, displayHeight);
            RightBorder = new Rectangle((int)Math.Round(newWindowWidth + XDrawOffset), 0, XDrawOffset, displayHeight);
        }
        else if (windowHeightFactor > windowWidthFactor)
        {
#if DEBUG
            Debug.WriteLine("Incompatible screen!!!");
            Debug.WriteLine(windowWidthFactor + ": " + displayWidth + "/" + BaseWindowWidth);
            Debug.WriteLine(windowHeightFactor + ": " + displayHeight + "/" + BaseWindowHeight);
#endif
            DisableFullscreen();
            return;
        }
        else
        {
#if DEBUG
            Debug.WriteLine("Perfect screen???");
            Debug.WriteLine(windowWidthFactor + ": " + displayWidth + "/" + BaseWindowWidth);
            Debug.WriteLine(windowHeightFactor + ": " + displayHeight + "/" + BaseWindowHeight);
#endif
            ScalingFactor = windowWidthFactor;
        }
#if DEBUG
        Debug.WriteLine("----------");
        Debug.WriteLine("Left border: X:" + LeftBorder.X + ", Y:" + LeftBorder.Y + ". " + LeftBorder.Width + "x" + LeftBorder.Height);
        Debug.WriteLine("Right border: X:" + RightBorder.X + ", Y:" + RightBorder.Y + ". " + RightBorder.Width + "x" + RightBorder.Height);
        Debug.WriteLine("---end----");
#endif
    }

#if DEBUG
    public void CrashHandler()
    {
        _graphics.IsFullScreen = false;
        _graphics.ApplyChanges();
    }
#endif

    public void GameOver()
    {
        InFileMenu = true;
    }

    // saving/loading "borrowed" from https://youtu.be/gYksT0d_xLM
    public void SaveGame()
    {
        State.Xposition = Map.Xposition;
        State.Yposition = Map.Yposition;
        State.MapName = Map.MapName;
        State.DisplayMapName = Map.DisplayMapName;
        State.Items = new List<int>();
        for (int i = 0; i < Items.Items.Count; i++)
        {
            State.Items.Add(Items.Items[i].Number);
        }
        string serializedText = JsonSerializer.Serialize(State);
        File.WriteAllText(SaveFileName, serializedText);
        Events.StartEvent("SaveDialog");
    }

    public void LoadGame()
    {
        string fileContent = File.ReadAllText(SaveFileName);
        State = JsonSerializer.Deserialize<GameState>(fileContent);
        Items.Items = ItemManager.GetItemList(State.Items);
        Map.LoadMap(State.MapName, State.Xposition, State.Yposition, State);
    }

    public void NewGame()
    {
        State = new GameState()
        {
            //you can add initial things here
            StoryFlags = new Dictionary<string, bool>(),
            Items = new List<int>(),
            Xposition = 8,
            Yposition = 7,
            MapName = "Spaceship",
            DisplayMapName = "New game",
            PlayersActive = 1,
            GaryMaxHealth = 100,
            GaryHealth = 100,
            GaryAttack = 10,
            SteveMaxHealth = 100,
            SteveHealth = 100,
            SteveAttack = 25,
            RosettaMaxHealth = 100,
            RosettaHealth = 100,
            RosettaAttack = 20,
        };
        // should be changed to be the starting location
        Map.LoadMap(State.MapName, State.Xposition, State.Yposition, State);
    }
}

public class GameState
{
    public Dictionary<string, bool> StoryFlags { get; set; }
    public List<int> Items { get; set; }
    public int Xposition { get; set; }
    public int Yposition { get; set; }
    public string MapName { get; set; }
    public string DisplayMapName { get; set; }
    public int PlayersActive { get; set; }
    public int GaryMaxHealth { get; set; }
    public int GaryHealth { get; set; }
    public int GaryAttack { get; set; }
    public int SteveMaxHealth { get; set; }
    public int SteveHealth { get; set; }
    public int SteveAttack { get; set; }
    public int RosettaMaxHealth { get; set; }
    public int RosettaHealth { get; set; }
    public int RosettaAttack { get; set; }

    public int GetHealth(int characterNumber)
    {
        if (characterNumber == 0)
            return GaryHealth;
        else if (characterNumber == 1)
            return SteveHealth;
        else if (characterNumber == 2)
            return RosettaHealth;
        else
            return -1;
    }

    public void DamagePlayer(int playerNumber, int damage)
    {
        if (playerNumber == 0)
        {
            GaryHealth -= damage;
            if (GaryHealth <= 0)
                GaryHealth = 0;
            else if (GaryHealth > GaryMaxHealth)
                GaryHealth = GaryMaxHealth;
        }
        else if (playerNumber == 1)
        {
            SteveHealth -= damage;
            if (SteveHealth < 0)
                SteveHealth = 0;
            else if (SteveHealth > SteveMaxHealth)
                SteveHealth = SteveMaxHealth;
        }
        else if (playerNumber == 2)
        {
            RosettaHealth -= damage;
            if (RosettaHealth < 0)
                RosettaHealth = 0;
            else if (RosettaHealth > RosettaMaxHealth)
                RosettaHealth = RosettaMaxHealth;
        }
    }

    public int GetRandomAlivePlayer(Random rng)
    {
        if (AllActivePlayersAreDefeated())
            return 0;
        // Watch out! if this is called with no players alive, it will freeze!!!
        int playerNumber = rng.Next(PlayersActive);
        if (GetHealth(playerNumber) <= 0)
        {
            bool addOrSubtract = rng.Next(2) == 0;
            if (addOrSubtract)
            {
                while (GetHealth(playerNumber) <= 0)
                {
                    playerNumber++;
                    if (playerNumber >= PlayersActive)
                        playerNumber = 0;
                }
            }
            else
            {
                while (GetHealth(playerNumber) <= 0)
                {
                    playerNumber--;
                    if (playerNumber <= 0)
                        playerNumber = PlayersActive - 1;
                }
            }
        }
        return playerNumber;
    }
    public bool AllActivePlayersAreDefeated()
    {
        if (PlayersActive == 1)
            return GaryHealth == 0;
        else if (PlayersActive == 2)
            return GaryHealth == 0 && SteveHealth == 0;
        else
            return GaryHealth == 0 && SteveHealth == 0 && RosettaHealth == 0;
    }
}