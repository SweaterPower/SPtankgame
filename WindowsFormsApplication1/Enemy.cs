using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace SPtankgame
{
    class Enemy
    {
        public Point position;
        public Direction direction;
        public Panel label;
        Size s;
        Mover mover;
        Action movement;
        public int distance = 0;
        List<Bullet> DangerBullets;

        public Enemy(Point pos, Direction dir, Panel l, Size s)
        {
            DangerBullets = new List<Bullet>();
            position = pos;
            direction = dir;
            label = l;
            Action lblact = () =>
            {
                label.Width = 40;
                label.Height = 40;
                label.BackColor = Color.Transparent;
                label.Location = pos;
                label.BackgroundImage = new Bitmap(Application.StartupPath + "\\enemy_not_bad.png");
            };
            while (!EnemyManager.GetDescriptorStatus()) { }
            label.Invoke(lblact);
            this.s = s;
            movement = () =>
            {
                label.Location = mover(label.Location);
            };
            EnemyManager.EnemiesMovement += this.MoveAhead;
            ShootingControl.PewPew += this.AddPotentialKiller;
            ShootingControl.Boom += this.RemovePotentialKiller;
            DangerBullets.AddRange(ShootingControl.GetBullets());
            OptimizeDirection();
        }

        void MoveAhead()
        {
            while (!EnemyManager.GetDescriptorStatus()) { }
            label.Invoke(movement);
            distance++;
            if (distance >= 40)
            {
                OptimizeDirection();
                distance = 0;
            }
            Bullet b = null;
            bool t = false;
            if (DangerBullets.Count != 0)
                t = this.HitCheck(out b);
            if (t || label.Location.X < 0 || label.Location.Y < 0 || label.Location.Y > s.Height || label.Location.X > s.Width)
            {
                while (!EnemyManager.GetDescriptorStatus()) { }
                ShootingControl.StopPlease();
                EnemyManager.EnemyExplode(this);
                if (t)
                {
                    b.EnemyHit = true;
                    ShootingControl.BulletExplode(b);
                    b.Atpison();
                }
                EnemyManager.EnemiesMovement -= this.MoveAhead;
                ShootingControl.PewPew -= this.AddPotentialKiller;
                ShootingControl.Boom -= this.RemovePotentialKiller;
                ShootingControl.DontStopPlease();
            }
        }

        List<Bullet> AnalyzeBullets(List<Bullet> bullets)
        {
            return bullets;
        }

        bool HitCheck(out Bullet b)
        {
            foreach (Bullet bullet in this.DangerBullets)
            {
                Point position = bullet.label.Location;
                if ((position.X >= label.Location.X || position.X + bullet.label.Width >= label.Location.X)
                    && (position.X <= label.Location.X + label.Width)
                    && (position.Y >= label.Location.Y || position.Y + bullet.label.Height >= label.Location.Y)
                    && (position.Y <= label.Location.Y + label.Height))
                {
                    b = bullet;
                    return true;
                }
            }
            b = DangerBullets.First();
            return false;
        }

        public void OptimizeDirection()
        {
            Point target = EnemyManager.GetTankLocation();
            List<Direction> possible_directions = new List<Direction>();
            int dx = label.Location.X - target.X;
            int absdx = Math.Abs(dx);
            if (absdx >= 35)
            if (dx > 0) possible_directions.Add(Direction.Left);
            else if (dx < 0) possible_directions.Add(Direction.Right);
            int dy = label.Location.Y - target.Y;
            int absdy = Math.Abs(dy);
            if (absdy >= 35)
            if (dy > 0) possible_directions.Add(Direction.Up);
            else if (dy < 0) possible_directions.Add(Direction.Down);

            if (possible_directions.Count == 0) 
            { EnemyManager.TankHit(); return; } //GOTCHA

            if (possible_directions.Count == 1)
            {
                Random rnd = new Random();
                if (rnd.Next(0, 5) == 0)
                {
                    switch (possible_directions[0])
                    {
                        case Direction.Down: possible_directions[0] = Direction.Left; break;
                        case Direction.Left: possible_directions[0] = Direction.Up; break;
                        case Direction.Up: possible_directions[0] = Direction.Right; break;
                        case Direction.Right: possible_directions[0] = Direction.Down; break;
                    }
                }
            }

            Random rand = new Random();
            EnemyManager.SetMover(ref this.mover, this.direction = possible_directions[rand.Next(0, possible_directions.Count)]);

            DangerBullets = AnalyzeBullets(DangerBullets);
        }

        void AddPotentialKiller(Bullet b)
        {
            this.DangerBullets.Add(b);
            //OptimizeDirection();
        }

        void RemovePotentialKiller(Bullet b)
        {
            this.DangerBullets.Remove(b);
            //OptimizeDirection();
        }

        public void Atpison()
        {
            EnemyManager.EnemiesMovement -= this.MoveAhead;
            ShootingControl.PewPew -= this.AddPotentialKiller;
            ShootingControl.Boom -= this.RemovePotentialKiller;
        }
    }
}
