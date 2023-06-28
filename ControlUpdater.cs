using Microsoft.Xna.Framework.Input;
using System.Collections;
namespace ALIENgame
{
    public class ControlUpdater
    {
        public bool ConfirmHeld;
        public bool ConfirmPressed;

        public bool CancelHeld;
        public bool CancelPressed;

        public bool MenuHeld;
        public bool MenuPressed;

        public bool UpHeld;
        public bool UpPressed;

        public bool LeftHeld;
        public bool LeftPressed;

        public bool DownHeld;
        public bool DownPressed;
        public bool DownPressedLastFrame;

        public bool RightHeld;
        public bool RightPressed;

        public bool FullscreenHeld;
        public bool FullscreenPressed;

        public bool ExitHeld;
        public Stack DirectionalButtonOrder;
        public ControlUpdater()
        {
            DirectionalButtonOrder = new Stack();
            DirectionalButtonOrder.Push("none");
            ConfirmHeld = false;
            ConfirmPressed = false;
            CancelHeld = false;
            CancelPressed = false;
            MenuHeld = false;
            MenuPressed = false;
            UpHeld = false;
            UpPressed = false;
            LeftHeld = false;
            LeftPressed = false;
            DownHeld = false;
            DownPressed = false;
            RightHeld = false;
            RightPressed = false;
            FullscreenHeld = false;
            FullscreenPressed = false;
            ExitHeld = false;
        }
        public void Update()
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Z) || Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                if (!ConfirmHeld)
                {
                    ConfirmPressed = true;
                }
                else
                {
                    ConfirmPressed = false;
                }
                ConfirmHeld = true;
            }
            else
            {
                ConfirmHeld = false;
                ConfirmPressed = false;
            }


            if (Keyboard.GetState().IsKeyDown(Keys.X) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
            {
                if (!CancelHeld)
                {
                    CancelPressed = true;
                }
                else
                {
                    CancelPressed = false;
                }
                CancelHeld = true;
            }
            else
            {
                CancelHeld = false;
                CancelPressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C) || Keyboard.GetState().IsKeyDown(Keys.RightControl))
            {
                if (!MenuHeld)
                {
                    MenuPressed = true;
                }
                else
                {
                    MenuPressed = false;
                }
                MenuHeld = true;
            }
            else
            {
                MenuHeld = false;
                MenuPressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) || Keyboard.GetState().IsKeyDown(Keys.W))
            {
                if (!UpHeld)
                {
                    UpPressed = true;
                    DirectionalButtonOrder.Push("up");
                }
                else
                {
                    UpPressed = false;
                }
                UpHeld = true;
            }
            else
            {
                UpHeld = false;
                UpPressed = false;
                if (DirectionalButtonOrder.Peek().ToString() == "up")
                    DirectionalButtonOrder.Pop();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left) || Keyboard.GetState().IsKeyDown(Keys.A))
            {
                if (!LeftHeld)
                {
                    LeftPressed = true;
                    DirectionalButtonOrder.Push("left");
                }
                else
                {
                    LeftPressed = false;
                }
                LeftHeld = true;
            }
            else
            {
                LeftHeld = false;
                LeftPressed = false;
                if (DirectionalButtonOrder.Peek().ToString() == "left")
                    DirectionalButtonOrder.Pop();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down) || Keyboard.GetState().IsKeyDown(Keys.S))
            {
                if (!DownHeld)
                {
                    DownPressed = true;
                    DirectionalButtonOrder.Push("down");
                }
                else
                {
                    DownPressed = false;
                }
                DownHeld = true;
            }
            else
            {
                DownHeld = false;
                DownPressed = false;
                if (DirectionalButtonOrder.Peek().ToString() == "down")
                    DirectionalButtonOrder.Pop();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right) || Keyboard.GetState().IsKeyDown(Keys.D))
            {
                if (!RightHeld)
                {
                    RightPressed = true;
                    DirectionalButtonOrder.Push("right");
                }
                else
                {
                    RightPressed = false;
                }
                RightHeld = true;
            }
            else
            {
                RightHeld = false;
                RightPressed = false;
                if (DirectionalButtonOrder.Peek().ToString() == "right")
                    DirectionalButtonOrder.Pop();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F11))
            {
                if (!FullscreenHeld)
                {
                    FullscreenPressed = true;
                }
                else
                {
                    FullscreenPressed = false;
                }
                FullscreenHeld = true;
            }
            else
            {
                FullscreenHeld = false;
                FullscreenPressed = false;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                ExitHeld = true;
            }
            else
            {
                ExitHeld = false;
            }
        }
    }
}
