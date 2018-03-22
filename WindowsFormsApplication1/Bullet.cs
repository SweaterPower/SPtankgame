using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace SPtankgame
{
    internal delegate Point Mover(Point oldpoint);

    class Bullet
    {

        public Point position;
        public Direction direction;
        public Panel label;
        public bool EnemyHit = false;
        Size s;
        Mover mover;
        Action movement;

        public Bullet(Point pos, Direction dir, Panel l, Size s, Mover m)
        {
            position = pos;
            direction = dir;
            label = l;
            this.s = s;
            Action lblact = () =>
            {
                label.BackgroundImage = new Bitmap(Application.StartupPath + "\\Bullet_not_bad.png");
                switch (direction)
                {
                    case (Direction.Down): label.BackgroundImage.RotateFlip(RotateFlipType.Rotate180FlipNone); position.X += 10; position.Y += 40; break;
                    case (Direction.Up): position.X += 10; position.Y -= 20; break;
                    case (Direction.Left): label.BackgroundImage.RotateFlip(RotateFlipType.Rotate270FlipNone); position.Y += 10; position.X -= 20; break;
                    case (Direction.Right): label.BackgroundImage.RotateFlip(RotateFlipType.Rotate90FlipNone); position.X += 40; position.Y += 10; break;
                }
                label.Width = 20;
                label.Height = 20;
                label.BackColor = Color.Transparent;
                label.Location = position;
            };
            while (!EnemyManager.GetDescriptorStatus()) { }
            label.Invoke(lblact);
            mover = m;
            movement = () =>
            {
                label.Location = mover(label.Location);
            };
            ShootingControl.BulletsMovement += this.MoveForward;
        }

        void MoveForward()
        {
            while (!EnemyManager.GetDescriptorStatus()) { }
            label.Invoke(movement);
            if (label.Location.X < 0 || label.Location.Y < 0 || label.Location.Y > s.Height || label.Location.X > s.Width)
            {
                ShootingControl.StopPlease();
                ShootingControl.BulletsMovement -= this.MoveForward;
                ShootingControl.BulletExplode(this);
                ShootingControl.DontStopPlease();
            }
        }

        public void Atpison()
        {
            ShootingControl.BulletsMovement -= this.MoveForward;
        }

        //всем привет, я вижуал студио и я люблю тормозить работу приложения вместо того чтобы выдать ексепшон
        //хлоп-хлоп-хлоп
        //upd на самом деле это операции в другом потоке или все нормально

        //мне кажется, или на втором курсе я уже мог знать C# и получше
        //кажется

        //public void HideThisBullet()
        //{
        //    //PROBLEM!!!! при любом обращении к панели они перестают свигаться
        //    //THIS THING SMELLS LIKE HYPERTHREADING PROBLEM => label.Visible = false;
        //}

        ////ПОЧЕМУ ИНВОУК ЗАМОРАЖИВАЕТ МОЮ ПРОГУ
        ////НУ ПОДУМАЕШЬ БЕСКОНЕЧНЫЙ ЦИКЛ НУ С КЕМ НЕ БЫВАЕТ
        //void GO()
        //{
        //    Action a;
        //    a = () =>
        //        {
        //            bool block = true;
        //            while (block)
        //            {
        //                label.Location = mover(label.Location);
        //                if (label.Location.X < 0 || label.Location.Y < 0 || label.Location.Y > s.Height || label.Location.X > s.Width)
        //                {
        //                    block = false;
        //                    throw new OutOfBoundsException();
        //                }
        //                Thread.Sleep(50);
        //            }
        //        };
        //    //Thread t = new Thread(new ThreadStart(a));
        //    try
        //    {
        //        label.Invoke(a);
        //        //t.Start();
        //    }
        //    catch
        //    {
        //        label.Dispose();
        //        ShootingControl.BulletExplode(this);
        //    }
        //}
    }
}
