using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ALIENgame
{
    public class ItemManager
    {
        public bool InventoryOpen = false;
        private bool characterSelectionActive = false;
        public const int InventorySpace = 9;
        public List<Item> Items = new List<Item>(InventorySpace);
        private Game1 gameObject;
        private bool saveSelected = false;
        private int selectedItem = 0;
        private int selectedCharacter = 0;
        private Texture2D backgroundTexture;
        private Texture2D arrow;
        private Vector2 healthDrawPosition = new Vector2(0, 0);
        private Vector2 origin = new Vector2(0, 0);
        private Vector2 saveDrawPosition = new Vector2(2 * (Game1.BaseWindowWidth / 3), 50);
        private Vector2 itemsDrawPosition = new Vector2(30, 50);
        private Vector2 arrowOffset = new Vector2(-20, 20);
        private int characterSelectionArrowYPosition = 20;
        private int[] characterSelectionArrowXPositions = new int[3]
        {
            20,
            195,
            370
        };

        private const string saveButtonString = "Save game";
        public ItemManager(Game1 game)
        {
            gameObject = game;
            backgroundTexture = game.Content.Load<Texture2D>("Content/Colors/Black");
            arrow = game.Content.Load<Texture2D>("Content/UIAssets/SelectionArrow");
        }
        public void Update(Game1 game)
        {
            if (!InventoryOpen && game.Controls.MenuPressed && !game.Events.EventIsActive)
            {
                OpenInventory();
            }
            else if (InventoryOpen && characterSelectionActive)
            {
                UpdateCharacterSelection(game);
            }
            else if (InventoryOpen)
            {
                UpdateInventoryScreen(game);
            }

        }

        private void UpdateCharacterSelection(Game1 game)
        {
            if (game.Controls.LeftPressed)
            {
                MoveCharacterSelectionLeft(game);
            }
            else if (game.Controls.RightPressed)
            {
                MoveCharacterSelectionRight(game);
            }
            else if (game.Controls.CancelPressed)
            {
                characterSelectionActive = false;
            }
            else if (game.Controls.ConfirmPressed)
            {
                characterSelectionActive = false;
                SelectItem(selectedItem);
            }
        }

        private void ActivateCharacterSelection()
        {
            characterSelectionActive = true;
            selectedCharacter = 0;
        }

        private void UpdateInventoryScreen(Game1 game)
        {
            if (game.Controls.UpPressed)
            {
                MoveSelectionUp();
            }
            else if (game.Controls.DownPressed)
            {
                MoveSelectionDown();
            }
            else if (game.Controls.ConfirmPressed)
            {
                if (saveSelected)
                {
                    CloseInventory();
                    game.SaveGame();
                }
                else
                    ActivateCharacterSelection();
            }
            else if (game.Controls.CancelPressed)
            {
                CloseInventory();
            }
            else if (game.Controls.RightPressed)
            {
                saveSelected = true;
            }
            else if (game.Controls.LeftPressed && Items.Count != 0)
            {
                saveSelected = false;
            }
        }

        private void OpenInventory()
        {
            if (gameObject.Map.Xoffset != 0 || gameObject.Map.Yoffset != 0)
                return;
            InventoryOpen = true;
            saveSelected = Items.Count <= 0;
            selectedItem = 0;
        }
        private void CloseInventory()
        {
            InventoryOpen = false;
        }


        private void SelectItem(int index)
        {
            UseItem(index, selectedCharacter);
            selectedItem = Math.Min(selectedItem, Items.Count - 1);
            if (selectedItem == -1)
                saveSelected = true;
        }

        private void MoveSelectionUp()
        {
            selectedItem--;
            if (selectedItem < 0)
                selectedItem = Items.Count - 1;
        }
        private void MoveSelectionDown()
        {
            selectedItem++;
            if (selectedItem >= Items.Count)
                selectedItem = 0;
        }

        private void MoveCharacterSelectionLeft(Game1 game)
        {
            selectedCharacter--;
            if (selectedCharacter < 0)
                selectedCharacter = game.State.PlayersActive - 1;
        }

        private void MoveCharacterSelectionRight(Game1 game)
        {
            selectedCharacter++;
            if (selectedCharacter >= game.State.PlayersActive)
            {
                selectedCharacter = 0;
            }
        }

        public void UseItem(int index, int playerToUseOn)
        {
            Items[index].Use(gameObject.State, playerToUseOn);
            Items.RemoveAt(index);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw health
            if (!InventoryOpen)
                return;
            spriteBatch.Draw(backgroundTexture, gameObject.MakeScaledRectangle(0, 0, Game1.BaseWindowWidth, Game1.BaseWindowHeight), Color.White);
            string healthStats = "";
            if (gameObject.State.PlayersActive >= 1)
                healthStats += "G:" + gameObject.State.GaryHealth + "/" + gameObject.State.GaryMaxHealth + "  ";
            if (gameObject.State.PlayersActive >= 2)
                healthStats += "S:" + gameObject.State.SteveHealth + "/" + gameObject.State.SteveMaxHealth + "  ";
            if (gameObject.State.PlayersActive >= 3)
                healthStats += "R:" + gameObject.State.RosettaHealth + "/" + gameObject.State.RosettaMaxHealth;
            spriteBatch.DrawString(gameObject.DeterminationMono, healthStats, gameObject.GetScaledVector(healthDrawPosition), Color.White, 0, origin, (float)gameObject.ScalingFactor, SpriteEffects.None, 1);

            spriteBatch.DrawString(gameObject.DeterminationMono, saveButtonString, gameObject.GetScaledVector(saveDrawPosition), Color.White, 0, origin, (float)gameObject.ScalingFactor, SpriteEffects.None, 1);

            string itemList = "";
            for (int i = 0; i < Items.Count; i++)
            {
                itemList += Items[i].DisplayName + "\n";
            }
            itemList = itemList.TrimEnd('\n');
            spriteBatch.DrawString(gameObject.DeterminationMono, itemList, gameObject.GetScaledVector(itemsDrawPosition), Color.White, 0, origin, (float)gameObject.ScalingFactor, SpriteEffects.None, 1);
            float heigthOfAnItem = gameObject.DeterminationMono.MeasureString(itemList).Y / Items.Count;
            Vector2 arrowVector;
            if (characterSelectionActive)
            {
                arrowVector = new Vector2(characterSelectionArrowXPositions[selectedCharacter], characterSelectionArrowYPosition);
            }
            else if (saveSelected)
                arrowVector = saveDrawPosition + arrowOffset;
            else
            {
                arrowVector = itemsDrawPosition + arrowOffset;
                arrowVector.Y += selectedItem * heigthOfAnItem;
            }
            spriteBatch.Draw(arrow, gameObject.MakeScaledRectangle((int)Math.Round(arrowVector.X), (int)Math.Round(arrowVector.Y), arrow.Width, arrow.Height), Color.White);
        }
        public static List<Item> GetItemList(List<int> itemNumberList)
        {
            List<Item> itemList = new List<Item>();
            foreach (int itemNumber in itemNumberList)
            {
                switch (itemNumber)
                {
                    case 1:
                        itemList.Add(new LavaBerry());
                        break;
                    case 2:
                        itemList.Add(new CakeSlice());
                        break;
                    case 3:
                        itemList.Add(new IceCream());
                        break;
                }
            }
            return itemList;
        }
        public string GetBattleItemString(int firstItem, int numberOfItems = 4)
        {
            string output = "";
            for (int i = firstItem; i < Math.Min(Items.Count, numberOfItems + firstItem); i++)
            {
                output += Items[i].DisplayName + "\n";
            }
            return output;
        }
    }
    public abstract class Item
    {
        public string DisplayName;
        public int Number;
        public abstract void Use(GameState state, int playerNumber);
    }
    public class LavaBerry : Item
    {
        private const int healingAmount = 20;
        public LavaBerry()
        {
            Number = 1;
            DisplayName = "Lava berry";
        }
        public override void Use(GameState state, int playerNumber)
        {
            state.DamagePlayer(playerNumber, -healingAmount);
        }
    }
    public class CakeSlice : Item
    {
        private const int healingAmount = 40;
        public CakeSlice()
        {
            Number = 2;
            DisplayName = "Cake slice";
        }
        public override void Use(GameState state, int playerNumber)
        {
            state.DamagePlayer(playerNumber, -healingAmount);
        }
    }
    public class IceCream : Item
    {
        private const int healingAmount = 80;
        public IceCream()
        {
            Number = 3;
            DisplayName = "Ice cream";
        }
        public override void Use(GameState state, int playerNumber)
        {
            state.DamagePlayer(playerNumber, -healingAmount);
        }
    }
}
