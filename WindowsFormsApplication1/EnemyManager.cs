using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SPtankgame
{
    delegate void BulletMarker(Enemy sender, Bullet bullet, ref bool nope);

    static class EnemyManager
    {
        static List<Enemy> enemies = new List<Enemy>();
        static List<Panel> panels = new List<Panel>();
        static int delta = 1;
        static public Form1 form1;

        static public int distance = 0;

        static public event Action EnemiesMovement;

        static Random rnd1 = new Random();
        static Random rnd2 = new Random();
        static Point curpoint = new Point();
        static Point nextpoint;

        static public void SpawnEnemy()
        {
            panels.Add(new Panel());
            Action act = () =>
            {
                form1.Controls.Add(panels.Last());
            };
            while (!EnemyManager.GetDescriptorStatus()) { }
            form1.Invoke(act);
            nextpoint = GenerateSpawnPoint();
            act = () =>
                {
                    form1.LocateNextSpawnPoint(nextpoint);
                };
            form1.Invoke(act);
            enemies.Add(new Enemy(curpoint, Direction.Still, panels.Last(), form1.Size));
            curpoint = nextpoint;
        }

        static Point GenerateSpawnPoint()
        {
            int line = rnd1.Next(0, 2);
            int X = 0; int Y = 0;
            if (line == 0)
            {
                int L_R = rnd2.Next(0, 2);
                if (L_R == 0) X = 0;
                else X = form1.Width - 40 - (form1.DesktopBounds.Width - form1.ClientRectangle.Width);
                Y = rnd1.Next(0, form1.Height - 40 - (form1.DesktopBounds.Height - form1.ClientRectangle.Height));
            }
            else
            {
                int T_B = rnd2.Next(0, 2);
                if (T_B == 0) Y = 0;
                else Y = form1.Height - 40 - (form1.DesktopBounds.Height - form1.ClientRectangle.Height);
                X = rnd1.Next(0, form1.Width - 40 - (form1.DesktopBounds.Width - form1.ClientRectangle.Width));
            }
            return new Point(X, Y);
        }

        static public void SetSpeed(int speed)
        {
            delta = speed;
        }

        static public void SetMover(ref Mover mover, Direction d)
        {
            switch (d)
            {
                case (Direction.Down): mover = EnemyManager.MoveDown; break;
                case (Direction.Up): mover = EnemyManager.MoveUp; break;
                case (Direction.Left): mover = EnemyManager.MoveLeft; break;
                case (Direction.Right): mover = EnemyManager.MoveRight; break;
                default: mover = EnemyManager.MoveUp; break;
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

        static public void EnemyExplode(Enemy enemy)
        {
            Panel Ptmp = panels[enemies.IndexOf(enemy)];
            enemies.Remove(enemy);
            panels.Remove(Ptmp);
            Action patymaker = () =>
            {
                Ptmp.Dispose();
            };
            while (!EnemyManager.GetDescriptorStatus()) { }
            Ptmp.Invoke(patymaker);
        }

        static public void StopPlease()
        {
            form1.stopper = true;
        }

        static public void DontStopPlease()
        {
            form1.stopper = false;
        }

        static public void MoveEnemies()
        {
            if (EnemiesMovement != null)
            EnemiesMovement();
            form1.PermitMovement();
        }

        static public bool GetDescriptorStatus()
        {
            return form1.IsHandleCreated;
        }

        static public void CheckDistance()
        {
                foreach (Enemy e in enemies) { e.OptimizeDirection(); }
        }

        static public Point GetTankLocation()
        {
            return form1.GetTankLocation();
        }

        static public void TankHit()
        {
            form1.GameOver();
        }

        static public void ExplodeAll()
        {
            int i = 0;
            if (enemies.Count != 0)
            do
            {
                enemies[i].Atpison();
                EnemyExplode(enemies[i]);
            }
            while (enemies.Count > 0);
        }
    }
}
