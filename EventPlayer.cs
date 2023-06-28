using Microsoft.Xna.Framework.Media;
using System;

namespace ALIENgame
{
    public class EventPlayer
    {
        private string[] currentEvent;
        private bool executeNextCommand;
        private int currentLineNumber;
        public bool EventIsActive = false;
        public DialogReader dialogReader;
        private Func<bool> canExecuteNextline;
        public EventPlayer()
        {
            executeNextCommand = false;
            currentLineNumber = 0;
            canExecuteNextline = () => true;
        }

        public void Initialize(DialogReader dialogReader)
        {
            this.dialogReader = dialogReader;
            currentEvent = System.IO.File.ReadAllText("Content/Events/0.txt").Replace("\r", "").Split('\n');
        }

        public void Update(GameState state, BattleSystem battle, Game1 game)
        {
            if (!EventIsActive)
                return;
            executeNextCommand = canExecuteNextline();
            if (!executeNextCommand)
                return;
            if (currentLineNumber >= currentEvent.Length)
            {
                StopEvent();
                return;
            }
            if (currentEvent[currentLineNumber].StartsWith("dialog"))
            {
                try
                {
                    dialogReader.ActivateBox(int.Parse(currentEvent[currentLineNumber][7..]));
                }
                catch (FormatException)
                {
                    dialogReader.ActivateBox(1);
                }
                currentLineNumber++;
                canExecuteNextline = () => !dialogReader.BoxIsActive;
            }
            else if (currentEvent[currentLineNumber].StartsWith("playerNumber"))
            {
                try
                {
                    state.PlayersActive = int.Parse(currentEvent[currentLineNumber][13..]);
                }
                catch (FormatException)
                {
                    dialogReader.ActivateBox(1);
                }
                currentLineNumber++;
                canExecuteNextline = () => true;
            }
            else if (currentEvent[currentLineNumber].StartsWith("battle"))
            {
                battle.StartBattle(currentEvent[currentLineNumber][7..]);
                currentLineNumber++;
                canExecuteNextline = () => !battle.BattleActive;
            }
            else if (currentEvent[currentLineNumber].StartsWith("win"))
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = false;
                MediaPlayer.Play(game.CreditSong);
                game.GameWon = true;
                canExecuteNextline = () => false;
            }
            else if (currentEvent[currentLineNumber].StartsWith("musicOff"))
            {
                game.Map.MusicOn = false;
                currentLineNumber++;
                canExecuteNextline = () => true;
            }
            else
            {
                //this is when the command is invalid
                dialogReader.ActivateBox(1);
                currentLineNumber++;
                canExecuteNextline = () => !dialogReader.BoxIsActive;
            }
        }

        public void StartEvent(string eventName)
        {
            EventIsActive = true;
            currentEvent = System.IO.File.ReadAllText("Content/Events/" + eventName + ".txt").Replace("\r", "").Split('\n');
            currentLineNumber = 0;
            executeNextCommand = true;
        }

        public void StopEvent()
        {
            EventIsActive = false;
            dialogReader.CloseBox();
        }


    }
}
