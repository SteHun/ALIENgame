using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace ALIENgame
{
    public class BattleSystem
    {
        private const string oneEnemyDialog = "An enemy appeared!";
        private const string multipleEnemyDialog = "Enemies appeared!";
        private const string allEnemiesDialog = "All enemies";
        private const string attackDialog = "Attacked ";
        private const string attackAllDialog = "Attacked all enemies";
        private const string fatalBlowDialog = "Dealt the fatal blow to ";
        private const string commandSelectionDialog = "  Attack  Skill  Item";
        private const string victoryDialog = "YOU WON!";
        private const string itemDialog = "Got item: ";
        private const string gameOverDialog = "Your HP ran out!&n&wGame Over!";
        private const string enemyStunnedDialog = " was stunned";
        private const string healDialog = "Healed all party members";
        private const string itemUseMessage = "You used ";

        public Dictionary<string, List<Enemy>> Formations = new Dictionary<string, List<Enemy>>
        {
            {"1ScripulousFingore", new List<Enemy>{new ScripulousFingore() } },
            {"2ScripulousFingores", new List<Enemy>{new ScripulousFingore(), new ScripulousFingore() } },
            {"1HunterAlien", new List<Enemy>{new HunterAlien()} },
            {"2HunterAliens", new List<Enemy>{new HunterAlien(), new HunterAlien() } },
            {"3HunterAliens", new List<Enemy>{new HunterAlien(), new HunterAlien(), new HunterAlien() } },
            {"3WolfAliens", new List<Enemy>{new WolfAlien(), new WolfAlien(), new WolfAlien() } },
            {"2AlienServants", new List<Enemy>{new AlienServant(), new AlienServant() } },
            {"4GoatAliens", new List<Enemy>{new GoatAlien(), new GoatAlien(), new GoatAlien(), new GoatAlien() } },
            {"ALIEN", new List<Enemy>{new ALIEN()} }
        };
        public Dictionary<string, Item> ItemDrops = new Dictionary<string, Item>
        {
            {"1ScripulousFingore", new LavaBerry() },
            {"2ScripulousFingores", new LavaBerry() },
            {"1HunterAlien", new LavaBerry() },
            {"2HunterAliens", new CakeSlice() },
            {"3HunterAliens", new IceCream() },
            {"3WolfAliens", new LavaBerry() },
            {"2AlienServants", new CakeSlice() },
            {"4GoatAliens", new IceCream() },
            {"ALIEN", new LavaBerry() }
        };
        public Dictionary<string, int> ItemDropOdds = new Dictionary<string, int>
        {
            {"1ScripulousFingore", 100 },
            {"2ScripulousFingores", 100 },
            {"1HunterAlien", 100 },
            {"2HunterAliens", 100 },
            {"3HunterAliens", 100 },
            {"3WolfAliens", 100 },
            {"2AlienServants", 100 },
            {"4GoatAliens", 100 },
            {"ALIEN", 0 }
        };

        private readonly int[] mode2CommandArrowXPositions = { 30, 160, 275, 370, 495 };
        private const int mode2CommandArrowYPosition = 380;
        public bool BattleActive = false;
        private List<Enemy> activeEnemies;
        private Item currentItemDrop;
        private int currentItemDropChanse;
        private int xPosToStartDrawingEnemies;
        private const int mode5PointerXPos = 0;
        private const int mode5PointerMinimumYPos = 360;
        private const int mode5PointerYOffsetPerItem = 32;
        private bool firstFrameOfBattle = false;
        private int skillPoints;
        private int displaySkillPoints;
        private const int skillPointsPerTurn = 25;
        private const int maxSkillPoints = 100;
        private bool[] rosettaAttackMultiplyerActive;
        private const float rosettaAttackMultiplyer = 1.25f;
        private const int rosettaHealAmount = 50;

        /* Selection modes
         * Mode 0: OK, start next action
         * Mode 1: Waiting for dialog box to close
         * Mode 2: Commands
         * Mode 3: Enemy selection
         * Mode 4: Confirm select all enemies
         * Mode 5: Select item
         */
        private int selectionMode;

        /* Menu 1: 0(attack) 1(skill) 2(Item) 3(Defend) 4(Run)
* Menu 2: Selected enemy/item number (only for options 0, 1, 2)*/
        private List<int> chosenOptions;
        /* 0: first playable character
         * 1: second playable character (may be skipped)
         * 2: third playable characeter (may be skipped)
         * 3+: enemies*/
        private int mode2Selection;
        private int mode3Selection;
        private int mode5Selection;
        private const int mode5MaxItems = 4;
        private int activePlayer;
        private int winPhase;
        private int losePhase;

        private Texture2D battleBackground;
        private Texture2D selectionArrow;
        private Texture2D healthBarColor;
        private Texture2D skillPointsColor;
        private ContentManager contentManager;
        private Random rng;
        private Song battleMusic;
        private Song bossMusic;
        public BattleSystem(ContentManager content)
        {
            battleBackground = content.Load<Texture2D>("Content/Colors/Black");
            selectionArrow = content.Load<Texture2D>("Content/UIAssets/SelectionArrow");
            healthBarColor = content.Load<Texture2D>("Content/Colors/Red");
            skillPointsColor = content.Load<Texture2D>("Content/Colors/Green");
            contentManager = content;
            rng = new Random();
            battleMusic = content.Load<Song>("Content/Music/Battle");
            bossMusic = content.Load<Song>("Content/Music/InTheFinal");
        }


        private void InfoMessage(Game1 game, string message)
        {
            game.Dialogs.CloseBox();
            game.Dialogs.ActivateBoxCustom(message, canExitBox: false, textScroll: false, useMonoFont: true);
        }
        private void DialogMessage(Game1 game, string message)
        {
            game.Dialogs.CloseBox();
            game.Dialogs.ActivateBoxCustom(message, canExitBox: true, textScroll: true, useMonoFont: false);
        }
        public void Update(Game1 game, GameTime gameTime)
        {
            if (!BattleActive)
                return;
            if (firstFrameOfBattle)
            {
                if (activeEnemies.Count == 1)
                    ActivateMode1(game, oneEnemyDialog);
                else
                    ActivateMode1(game, multipleEnemyDialog);
                firstFrameOfBattle = false;
            }
            // OK, start next action
            if (selectionMode == 0)
            {

            }// Waiting for dialog box to close
            else if (selectionMode == 1)
            {
                Mode1Update(game);
            }// Commands
            else if (selectionMode == 2)
            {
                Mode2Update(game);
            }// Enemy selection
            else if (selectionMode == 3)
            {
                Mode3Update(game);
            }// Confirm select all enemies
            else if (selectionMode == 4)
            {
                Mode4Update(game);
            }
            else if (selectionMode == 5)
            {
                Mode5Update(game);
            }
            if (winPhase > 0)
            {
                WinBattleUpdate(game);
                return;
            }
            else if (losePhase > 0)
            {
                LoseBattleUpdate(game);
                return;
            }
            // a player is currently active
            if (activePlayer == 0 || activePlayer == 1 || activePlayer == 2)
            {
                int hp = game.State.GetHealth(activePlayer);
                // menu logic
                if (hp <= 0)
                    NextCharacter(game);
                if (chosenOptions.Count == 0)
                {
                    if (selectionMode == 0 || selectionMode == 3 || selectionMode == 4)
                    {
                        displaySkillPoints = skillPoints;
                        ActivateMode2(game);
                    }
                }
                else if (chosenOptions.Count == 1)
                {
                    if (chosenOptions[0] == 0 && selectionMode == 0)
                    {
                        // Choose all for Gary, else select enemy
                        if (activePlayer == 0)
                            ActivateMode4(game);
                        else
                            ActivateMode3(game);
                    }
                    else if (chosenOptions[0] == 1 && selectionMode == 0)
                    {
                        //Skill
                        if (skillPoints >= maxSkillPoints)
                        {
                            if (activePlayer == 0)
                                ActivateMode4(game);
                            else if (activePlayer == 1)
                                ActivateMode4(game);
                            else
                            {
                                RosettaSkill(gameTime, game);
                                NextCharacter(game);
                            }
                        }
                        else
                        {
                            ActivateMode2(game);
                            chosenOptions.RemoveAt(0);
                            mode2Selection = 1;
                        }
                    }
                    else if (chosenOptions[0] == 2 && selectionMode == 0)
                    {
                        //Item
                        if (game.Items.Items.Count != 0)
                            ActivateMode5(game);
                        else
                        {
                            ActivateMode2(game);
                            chosenOptions.RemoveAt(0);
                            mode2Selection = 2;
                        }
                    }
                    else if (chosenOptions[0] == 3 && selectionMode == 0)
                    {
                        //Defend
                        ActivateMode1(game, "You tried to defend yourself. &n&wBut you remembered that defending has not been &nimplemented yet. &wWhat a useless turn!");
                        NextCharacter(game);
                    }
                    else if (chosenOptions[0] == 4 && selectionMode == 0)
                    {
                        //Run
                        ActivateMode1(game, "You tried to run. &n&wBut you remembered that running has not been &nimplemented yet. &wWhat a useless turn!");
                        NextCharacter(game);
                    }
                }
                else if (chosenOptions.Count == 2)
                {
                    if (chosenOptions[0] == 0 && selectionMode == 0)
                    {
                        //Attack
                        if (activePlayer == 0)
                            GaryAttack(gameTime, game);
                        else if (activePlayer == 1)
                            SteveAttack(gameTime, game);
                        else if (activePlayer == 2)
                            RosettaAttack(gameTime, game);
                        NextCharacter(game);
                    }
                    else if (chosenOptions[0] == 1 && selectionMode == 0)
                    {
                        //Skill
                        if (activePlayer == 0)
                            GarySkill(gameTime, game);
                        else if (activePlayer == 1)
                            SteveSkill(gameTime, game);
                        NextCharacter(game);
                    }
                    else if (chosenOptions[0] == 2 && selectionMode == 0)
                    {
                        //Item
                        ActivateMode1(game, itemUseMessage + game.Items.Items[mode5Selection].DisplayName);
                        game.Items.UseItem(mode5Selection, activePlayer);
                        NextCharacter(game);
                    }
                }
            }// enemy active
            else
            {

                if (selectionMode == 0)
                {
                    if (activeEnemies[activePlayer - 3].Hp > 0)
                    {
                        if (activeEnemies[activePlayer - 3].isStunned)
                        {
                            activeEnemies[activePlayer - 3].isStunned = false;
                            ActivateMode1(game, activeEnemies[activePlayer - 3].Name + enemyStunnedDialog);
                        }
                        else
                        {
                            ActivateMode1(game, activeEnemies[activePlayer - 3].Turn(game.State));
                        }
                    }
                    else
                        //this is really janky but prevents the screen from flickering. Please forgive me
                        InfoMessage(game, "");
                    NextCharacter(game);
                }
                if (game.State.AllActivePlayersAreDefeated())
                    LoseBattle();
            }
        }

        private void NextCharacter(Game1 game)
        {
            chosenOptions = new List<int>();
            activePlayer++;
            if (activePlayer == game.State.PlayersActive)
                activePlayer = 3;
            if (activePlayer >= activeEnemies.Count + 3)
            {
                activePlayer = 0;
                skillPoints += skillPointsPerTurn;
            }

            for (int i = 0; i < activeEnemies.Count; i++)
            {
                if (activeEnemies[i].Hp != 0)
                    return;
            }
            WinBattle();
        }

        private void LoseBattle()
        {
            losePhase = 1;
        }

        private void LoseBattleUpdate(Game1 game)
        {
            if (losePhase == 1 && selectionMode == 0)
            {
                ActivateMode1(game, gameOverDialog);
                losePhase = 2;
            }
            else if (losePhase == 2 && selectionMode == 0)
            {
                BattleActive = false;
                MediaPlayer.Stop();
                game.GameOver();
            }
        }

        private void WinBattle()
        {
            winPhase = 1;
        }
        private void WinBattleUpdate(Game1 game)
        {
            if (winPhase == 1 && selectionMode == 0)
            {
                if (rng.Next(100) < currentItemDropChanse && !(game.Items.Items.Count >= ItemManager.InventorySpace))
                {
                    game.Items.Items.Add(currentItemDrop);
                    ActivateMode1(game, victoryDialog + "&w&n" + "&w&n" + itemDialog + currentItemDrop.DisplayName);
                }
                else
                {
                    ActivateMode1(game, victoryDialog + "&w&n");
                }
                winPhase = 2;
            }
            if (winPhase == 2 && selectionMode == 0)
            {
                BattleActive = false;
                MediaPlayer.Stop();
            }
        }


        private void ActivateMode1(Game1 game, string message)
        {
            selectionMode = 1;
            DialogMessage(game, message);
        }
        private void Mode1Update(Game1 game)
        {
            if (!game.Dialogs.BoxIsActive)
                selectionMode = 0;
        }

        private void ActivateMode2(Game1 game)
        {
            selectionMode = 2;
            mode2Selection = 0;
            InfoMessage(game, commandSelectionDialog);
        }
        private void Mode2Update(Game1 game)
        {
            if (game.Controls.LeftPressed)
            {
                mode2Selection--;
                if (mode2Selection < 0)
                    mode2Selection = 2;
            }
            else if (game.Controls.RightPressed)
            {
                mode2Selection++;
                if (mode2Selection > 2)
                    mode2Selection = 0;
            }
            if (game.Controls.ConfirmPressed)
            {
                chosenOptions.Add(mode2Selection);
                selectionMode = 0;
            }
        }

        private void ActivateMode3(Game1 game)
        {
            selectionMode = 3;
            mode3Selection = 0;
            // Freezes when there are no enemies
            while (activeEnemies[mode3Selection].Hp == 0)
            {
                mode3Selection++;
            }
            InfoMessage(game, activeEnemies[mode3Selection].Name);
        }
        private void Mode3Update(Game1 game)
        {
            if (game.Controls.LeftPressed)
            {
                do
                {
                    mode3Selection--;
                    if (mode3Selection < 0)
                        mode3Selection = activeEnemies.Count - 1;
                } while (activeEnemies[mode3Selection].Hp == 0);
                InfoMessage(game, activeEnemies[mode3Selection].Name);
            }
            else if (game.Controls.RightPressed)
            {
                do
                {
                    mode3Selection++;
                    if (mode3Selection >= activeEnemies.Count)
                    {
                        mode3Selection = 0;
                    }
                } while (activeEnemies[mode3Selection].Hp == 0);
                InfoMessage(game, activeEnemies[mode3Selection].Name);
            }
            if (game.Controls.ConfirmPressed)
            {
                chosenOptions.Add(mode3Selection);
                selectionMode = 0;
            }
            else if (game.Controls.CancelPressed)
            {
                chosenOptions.RemoveAt(0);
                selectionMode = 0;
            }
        }

        private void ActivateMode4(Game1 game)
        {
            selectionMode = 4;
            InfoMessage(game, allEnemiesDialog);
        }
        private void Mode4Update(Game1 game)
        {
            if (game.Controls.ConfirmPressed)
            {
                chosenOptions.Add(0);
                selectionMode = 0;
            }
            else if (game.Controls.CancelPressed)
            {
                chosenOptions.RemoveAt(0);
                selectionMode = 0;
            }
        }
        private void ActivateMode5(Game1 game)
        {
            selectionMode = 5;
            mode5Selection = 0;
            InfoMessage(game, game.Items.GetBattleItemString(mode5Selection, numberOfItems: mode5MaxItems));
        }
        private void Mode5Update(Game1 game)
        {
            if (game.Controls.UpPressed)
            {
                mode5Selection--;
                if (mode5Selection < 0)
                    mode5Selection = game.Items.Items.Count - 1;
            }
            else if (game.Controls.DownPressed)
            {
                mode5Selection++;
                if (mode5Selection >= game.Items.Items.Count)
                    mode5Selection = 0;
            }
            else if (game.Controls.CancelPressed)
            {
                chosenOptions.RemoveAt(0);
                selectionMode = 0;
            }
            else if (game.Controls.ConfirmPressed)
            {
                chosenOptions.Add(mode5Selection);
                selectionMode = 0;
            }
            if (game.Controls.UpPressed || game.Controls.DownPressed)
            {
                InfoMessage(game, game.Items.GetBattleItemString((mode5Selection / mode5MaxItems) * mode5MaxItems, numberOfItems: mode5MaxItems));
            }
        }

        public void DrawBackground(SpriteBatch spriteBatch, Game1 game)
        {
            if (!BattleActive)
                return;
            spriteBatch.Draw(battleBackground, game.MakeScaledRectangle(0, 0, Game1.BaseWindowWidth, Game1.BaseWindowHeight), Color.White);
            spriteBatch.Draw(skillPointsColor, game.MakeScaledRectangle(0, 0, Game1.BaseWindowWidth * ((float)displaySkillPoints / (float)maxSkillPoints), Game1.BaseWindowWidth), Color.White);
        }
        public void Draw(SpriteBatch spriteBatch, Game1 game, GameTime gameTime)
        {
            if (!BattleActive)
                return;
            int xPosToDraw = xPosToStartDrawingEnemies;
            //draw enemies
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                if (activeEnemies[i].Hp == 0)
                    spriteBatch.Draw(activeEnemies[i].Appearance, game.MakeScaledRectangle(xPosToDraw, (int)Math.Round(((double)Game1.BaseWindowHeight - activeEnemies[i].Height) / 2), activeEnemies[i].Width, activeEnemies[i].Height), Color.White * MathF.Max(0, (float)activeEnemies[i].DefeatTimeStamp - (float)gameTime.TotalGameTime.TotalSeconds + 1));
                else
                    spriteBatch.Draw(activeEnemies[i].Appearance, game.MakeScaledRectangle(xPosToDraw, (int)Math.Round(((double)Game1.BaseWindowHeight - activeEnemies[i].Height) / 2), activeEnemies[i].Width, activeEnemies[i].Height), Color.White);
                xPosToDraw += activeEnemies[i].Width;
            }
            //draw stats
            int health;
            int maxHealth;
            int barHeight;
            int barWidth = Game1.BaseWindowWidth / game.State.PlayersActive;
            for (int i = 0; i < game.State.PlayersActive; ++i)
            {
                if (i == 0)
                {
                    health = game.State.GaryHealth;
                    maxHealth = game.State.GaryMaxHealth;
                }
                else if (i == 1)
                {
                    health = game.State.SteveHealth;
                    maxHealth = game.State.SteveMaxHealth;
                }
                else
                {
                    health = game.State.RosettaHealth;
                    maxHealth = game.State.RosettaMaxHealth;
                }
                if (i == activePlayer && (selectionMode != 0 && selectionMode != 1))
                    barHeight = 50;
                else
                    barHeight = 25;

                spriteBatch.Draw(healthBarColor, game.MakeScaledRectangle(i * barWidth, 0, barWidth * ((double)health / maxHealth), barHeight), Color.White);
            }


            //draw pointer
            if (selectionMode == 2)
            {
                spriteBatch.Draw(selectionArrow, game.MakeScaledRectangle(mode2CommandArrowXPositions[mode2Selection], mode2CommandArrowYPosition, selectionArrow.Width, selectionArrow.Height), Color.White);
            }
            else if (selectionMode == 3)
            {
                xPosToDraw = xPosToStartDrawingEnemies;
                for (int i = 0; i < mode3Selection; i++)
                {
                    xPosToDraw += activeEnemies[i].Width;
                }
                spriteBatch.Draw(selectionArrow, game.MakeScaledRectangle(xPosToDraw - selectionArrow.Width,
                    (int)Math.Round(((double)Game1.BaseWindowHeight + activeEnemies[mode3Selection].Height) / 2),
                    selectionArrow.Height, selectionArrow.Width), Color.White);
            }
            else if (selectionMode == 4)
            {
                xPosToDraw = xPosToStartDrawingEnemies;
                for (int i = 0; i < activeEnemies.Count; i++)
                {
                    if (activeEnemies[i].Hp == 0)
                    {
                        xPosToDraw += activeEnemies[i].Width;
                        continue;
                    }
                    spriteBatch.Draw(selectionArrow, game.MakeScaledRectangle(xPosToDraw - selectionArrow.Width,
                        (int)Math.Round(((double)Game1.BaseWindowHeight + activeEnemies[i].Height) / 2),
                        selectionArrow.Height, selectionArrow.Width), Color.White);
                    xPosToDraw += activeEnemies[i].Width;
                }
            }
            else if (selectionMode == 5)
            {
                Rectangle pointerBox = game.MakeScaledRectangle(mode5PointerXPos, mode5PointerMinimumYPos + (mode5Selection % mode5MaxItems) * mode5PointerYOffsetPerItem, selectionArrow.Width, selectionArrow.Height);
                spriteBatch.Draw(selectionArrow, pointerBox, Color.White);
            }
        }

        public void StartBattle(string formation)
        {
            winPhase = 0;
            losePhase = 0;
            firstFrameOfBattle = true;
            activePlayer = 0;
            skillPoints = 0;
            displaySkillPoints = 0;
            chosenOptions = new List<int>();
            BattleActive = true;
            int widthOfAllEnemies = 0;
            activeEnemies = new List<Enemy>(Formations[formation]);
            currentItemDrop = ItemDrops[formation];
            currentItemDropChanse = ItemDropOdds[formation];
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].Restore();
                activeEnemies[i].Load(contentManager);
                widthOfAllEnemies += activeEnemies[i].Width;
            }
            xPosToStartDrawingEnemies = (int)Math.Round(((double)Game1.BaseWindowWidth - widthOfAllEnemies) / 2);
            selectionMode = 0;
            rosettaAttackMultiplyerActive = new bool[activeEnemies.Count];
            MediaPlayer.Stop();
            if (activeEnemies[0].IsBoss)
                MediaPlayer.Play(bossMusic);
            else
                MediaPlayer.Play(battleMusic);
            MediaPlayer.IsRepeating = true;
        }
        private void GaryAttack(GameTime gameTime, Game1 game)
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                if (activeEnemies[i].IsAlive)
                    activeEnemies[i].TakeDamage(game.State.GaryAttack, gameTime);
            }
            ActivateMode1(game, attackAllDialog);
        }
        private void SteveAttack(GameTime gameTime, Game1 game)
        {
            activeEnemies[chosenOptions[1]].TakeDamage(game.State.SteveAttack, gameTime);
            if (activeEnemies[chosenOptions[1]].Hp == 0)
                ActivateMode1(game, fatalBlowDialog + activeEnemies[chosenOptions[1]].Name);
            else
                ActivateMode1(game, attackDialog + activeEnemies[chosenOptions[1]].Name);
        }
        private void RosettaAttack(GameTime gameTime, Game1 game)
        {
            if (rosettaAttackMultiplyerActive[chosenOptions[1]])
                activeEnemies[chosenOptions[1]].TakeDamage((int)Math.Round(game.State.RosettaAttack * rosettaAttackMultiplyer), gameTime);
            else
                activeEnemies[chosenOptions[1]].TakeDamage(game.State.RosettaAttack, gameTime);
            rosettaAttackMultiplyerActive[chosenOptions[1]] = true;
            if (activeEnemies[chosenOptions[1]].Hp == 0)
                ActivateMode1(game, fatalBlowDialog + activeEnemies[chosenOptions[1]].Name);
            else
                ActivateMode1(game, attackDialog + activeEnemies[chosenOptions[1]].Name);
        }
        private void GarySkill(GameTime gameTime, Game1 game)
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                if (activeEnemies[i].IsAlive)
                {
                    activeEnemies[i].TakeDamage((int)Math.Round(game.State.GaryAttack * 0.5), gameTime);
                    activeEnemies[i].isStunned = true;
                }
            }
            ActivateMode1(game, attackAllDialog);
            skillPoints = 0;
            displaySkillPoints = 0;
        }
        private void SteveSkill(GameTime gameTime, Game1 game)
        {
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                if (activeEnemies[i].IsAlive)
                    activeEnemies[i].TakeDamage(game.State.GaryAttack, gameTime);
            }
            ActivateMode1(game, attackAllDialog);
            skillPoints = 0;
            displaySkillPoints = 0;
        }
        private void RosettaSkill(GameTime gameTime, Game1 game)
        {
            game.State.GaryHealth = Math.Min(game.State.GaryHealth + rosettaHealAmount, game.State.GaryMaxHealth);
            game.State.SteveHealth = Math.Min(game.State.SteveHealth + rosettaHealAmount, game.State.SteveMaxHealth);
            game.State.RosettaHealth = Math.Min(game.State.RosettaHealth + rosettaHealAmount, game.State.RosettaMaxHealth);
            ActivateMode1(game, healDialog);
            skillPoints = 0;
            displaySkillPoints = 0;
        }
    }
}
