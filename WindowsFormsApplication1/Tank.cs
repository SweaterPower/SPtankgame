using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace SPtankgame
{
    public enum Direction { Up, Down, Left, Right, Still }

    class UnexpectedDirectionException : Exception
    {

    }

    class OutOfBoundsException : Exception
    {

    }

    class Tank
    {
        public event Action StartMovement;
        public event Action FinishMovement;

        public Tank(Panel p, Size s)
        {
            PlayerPanel = p;
            size = s;
            sw = new Stopwatch();
            sw.Start();
        }


        public Panel PlayerPanel;
        Size size;

        int speed = 3;

        RotateFlipType ttt = RotateFlipType.Rotate180FlipY;
        void RotatePlayer(Keys k)
        {
            switch (ttt)
            {
                case (RotateFlipType.Rotate180FlipNone): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipNone); break;
                case (RotateFlipType.Rotate270FlipNone): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                case (RotateFlipType.Rotate90FlipNone): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                default: break;
            }

            switch (k)
            {
                case (Keys.Up): ttt = RotateFlipType.Rotate180FlipY; break;
                case (Keys.W): ttt = RotateFlipType.Rotate180FlipY; break;
                case (Keys.Down): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipNone); ttt = RotateFlipType.Rotate180FlipNone; break;
                case (Keys.S): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipNone); ttt = RotateFlipType.Rotate180FlipNone; break;
                case (Keys.Left): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone); ttt = RotateFlipType.Rotate270FlipNone; break;
                case (Keys.A): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone); ttt = RotateFlipType.Rotate270FlipNone; break;
                case (Keys.Right): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate90FlipNone); ttt = RotateFlipType.Rotate90FlipNone; break;
                case (Keys.D): PlayerPanel.BackgroundImage.RotateFlip(RotateFlipType.Rotate90FlipNone); ttt = RotateFlipType.Rotate90FlipNone; break;
                default: ttt = RotateFlipType.Rotate180FlipY; break;
            }

            Action refre = () => { PlayerPanel.Refresh(); };
            while (!EnemyManager.GetDescriptorStatus()) { }
            PlayerPanel.Invoke(refre);
        }

        void PlayerControl(Keys k)
        {
            Direction dir = new Direction();
            switch (k)
            {
                case (Keys.W): dir = Direction.Up; break;
                case (Keys.S): dir = Direction.Down; break;
                case (Keys.A): dir = Direction.Left; break;
                case (Keys.D): dir = Direction.Right; break;
                default: dir = Direction.Still; break;
            }
            try
            {
                Move(PlayerPanel.Location, dir, this.size);//PlayerPanel.Location = 
            }
            catch (UnexpectedDirectionException e)
            {
            }
            catch (OutOfBoundsException e)
            {
            }
        }

        void Move(Point start, Direction dir, Size s)
        {
            Point old = new Point(start.X, start.Y);
            int xmoder = speed; int ymoder = speed;
            switch (dir)
            {
                case (Direction.Down): xmoder = 0; ymoder = speed; break; //start.Y += 40; break;
                case (Direction.Up): xmoder = 0; ymoder = -speed; break;//start.Y -= 40; break;
                case (Direction.Left): xmoder = -speed; ymoder = 0; break; //start.X -= 40; break;
                case (Direction.Right): xmoder = speed; ymoder = 0; break; //start.X += 40; break;
                case (Direction.Still): break;
                default: throw new UnexpectedDirectionException();
            }

            if ( xmoder != ymoder)
            {
                    start.X += xmoder;
                    start.Y += ymoder;
                    Thread.Sleep(10);
                    if (start.X < 0 || start.Y < 0 || start.Y > s.Height - 40 || start.X > s.Width - 40)
                        {
                            start = old;
                            throw new OutOfBoundsException();
                        }
                Action fcknthreads = () => 
                {
                    PlayerPanel.Location = start;
                };
                while (!EnemyManager.GetDescriptorStatus()) { }
                PlayerPanel.Invoke(fcknthreads);
            }
        }

        Stopwatch sw;
        void FireControl(Keys k)
        {
            if (sw.ElapsedMilliseconds >= 1000)
            {
                Direction dir = new Direction();
                switch (k)
                {
                    case (Keys.Up): dir = Direction.Up; break;
                    case (Keys.Down): dir = Direction.Down; break;
                    case (Keys.Left): dir = Direction.Left; break;
                    case (Keys.Right): dir = Direction.Right; break;
                    default: dir = Direction.Still; break;
                }
                if (dir == Direction.Still) return;
                ShootingControl.Shoot(PlayerPanel.Location, dir);
                sw.Restart();
            }
        }

        public void PlayerMoving(Keys k)
        {
            if (StartMovement != null) StartMovement();

            RotatePlayer(k);
            PlayerControl(k);

            if (FinishMovement != null) FinishMovement();
        }

        public void PlayerShooting(Keys k)
        {
            RotatePlayer(k);
            FireControl(k);
        }

        public void Resizing(Size newsize)
        {
            this.size = newsize;
        }

        public void Boost()
        {
            int oldspeed = speed;
            Action boostender = () =>
                {
                    speed += 10;
                    Thread.Sleep(5000);
                    speed = oldspeed;
                };
            Task t = new Task(boostender);
            t.Start();
        }

        public void Teleport(Point p)
        {
            Action aaa = () => {
                PlayerPanel.Location = p;
            };
            PlayerPanel.Invoke(aaa);
        }
    }
}
