using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SPtankgame
{
    internal delegate void ShotHandler(Bullet bullet);

    static class ShootingControl
    {
        static List<Bullet> shoots = new List<Bullet>();
        static List<Panel> panels = new List<Panel>();
        const int delta = 6;
        static public Form1 form1;
        static public event Action BulletsMovement;
        static public event ShotHandler PewPew;
        static public event ShotHandler Boom;

        static public void Shoot(Point p, Direction d)
        {
            Mover m;
            switch (d)
                {
                    case (Direction.Down): m = MoveDown; break;
                    case (Direction.Up): m = MoveUp; break;
                    case (Direction.Left): m = MoveLeft; break;
                    case (Direction.Right): m = MoveRight; break;
                    default: m = MoveUp; break;
                }
            panels.Add(new Panel());
            Action act = () =>
                {
                    form1.Controls.Add(panels.Last());
                };
            while (!EnemyManager.GetDescriptorStatus()) { }
            form1.Invoke(act);
            Bullet b = new Bullet(p, d, panels.Last(), form1.Size, m);
            shoots.Add(b);
            if (PewPew != null) PewPew(b);
        }

        static public void BulletExplode(Bullet shot)
        {
            Panel Ptmp = panels[shoots.IndexOf(shot)];
            shoots.Remove(shot);
            panels.Remove(Ptmp);
            Action patymaker = () =>
                {
                    Ptmp.Dispose();
                };
            while (!EnemyManager.GetDescriptorStatus()) { }
            Ptmp.Invoke(patymaker);
            if (Boom != null) Boom(shot);
        }

        static public void StopPlease()
        {
            form1.stopper = true;
        }

        static public void DontStopPlease()
        {
            form1.stopper = false;
        }

        //static public void Hide()
        //{
        //    foreach (Bullet b in tohidelist)
        //    {
        //        b.HideThisBullet();
        //    }
        //    tohidelist.Clear();
        //}

        static public void MoveBullets()
        {
            if (BulletsMovement != null)
            {
                BulletsMovement();
            }
        }

        static Point MoveUp(Point oldpoint)
        {
            oldpoint.Y -= delta;
            return oldpoint;
        }

        static Point MoveDown(Point oldpoint)
        {
            oldpoint.Y += delta;
            return oldpoint;
        }

        static Point MoveLeft(Point oldpoint)
        {
            oldpoint.X -= delta;
            return oldpoint;
        }

        static Point MoveRight(Point oldpoint)
        {
            oldpoint.X += delta;
            return oldpoint;
        }

        static public List<Bullet> GetBullets()
        {
            return shoots;
        }

        static public void ExplodeAll()
        {
            int i = 0;
            if (shoots.Count != 0)
            do
            {
                shoots[i].Atpison();
                BulletExplode(shoots[i]);
            }
            while (shoots.Count > 0);
        }
    }
}
