using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace SPtankgame
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Tank tank;
        bool endless = true;
        public bool stopper = false;
        Action ak;
        Keys k;
        bool prevention = false;
        MenuForm mf;
        bool permit = false;

        public void PermitMovement()
        {
            permit = true;
        }

        public Point GetTankLocation()
        {
            return tank.PlayerPanel.Location;
        }

        public void RestartGame()
        {
            domework = false;
            //while (working) { }
            tank.Teleport(new Point(this.Width / 2 - 20, this.Height / 2 - 20));
            ShootingControl.ExplodeAll();
            EnemyManager.ExplodeAll();
            control_block = true;
            this.BackColor = Color.WhiteSmoke;
            basespeed = 1;
            mf.GameStart();
            label1.Visible = false;
            domework = true;
        }

        public void GameOver()
        {
            try
            {
                Action dalol = () =>
                    {
                        BackColor = Color.DarkOrange;
                        label1.Visible = true;
                        //InvokeRequired? Of course!
                    };
                this.Invoke(dalol);
                mf.GameOver();
            }
            catch { }
        }

        public void PauseResumeGame()
        {
            BlockControls();
        }

        void HandleDeHandler(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
        }

        void HandleHandler(object sender, EventArgs e)
        {
            backgroundWorker1.CancelAsync();
            backgroundWorker1.RunWorkerAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ShootingControl.form1 = this;
            EnemyManager.form1 = this;
            tank = new Tank(TankPanel, this.Size);
            backgroundWorker1.RunWorkerAsync();
            //backgroundWorker2.RunWorkerAsync();
            tank.StartMovement += BlockControls;
            tank.FinishMovement += BlockControls;
            ak = () =>
            {
                while (prevention)
                {
                    if (control_block)
                    {
                        if (permit)
                        {
                            tank.PlayerMoving(k);
                            permit = false;
                        }
                    }
                }
            };
            t = new Task(ak);
            this.HandleDestroyed += HandleDeHandler;
            this.HandleCreated += HandleHandler;
            CalculateSpeed();
            mf = new MenuForm(this);
            mf.Show();
            LocateNextSpawnPoint(new Point());
            ShootingControl.PewPew += ReloadVis;
            mf.ChangeModer(CalculateModer());
            PauseResumeGame();
        }

        public void LocateNextSpawnPoint(Point location)
        {
            SpawnPanel.Location = location;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            tank.Resizing(this.Size);
            CalculateSpeed();
            mf.ChangeModer(CalculateModer());
            if (tank.PlayerPanel.Location.X > this.Width || tank.PlayerPanel.Location.Y > this.Height) GameOver();
            tank.Teleport(new Point(this.Width / 2 - 20, this.Height / 2 - 20));
        }

        int basespeed = 1;
        void CalculateSpeed()
        {
            EnemyManager.SetSpeed((int)Math.Round((double)(this.Height * this.Width) / (Screen.PrimaryScreen.Bounds.Width * Screen.PrimaryScreen.Bounds.Height) * 2) + basespeed);
        }

        double CalculateModer()
        {
            return 10 * (this.Height * this.Width) / (Screen.PrimaryScreen.Bounds.Width * Screen.PrimaryScreen.Bounds.Height);
        }

        bool control_block = true;
        Task t;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            k = e.KeyCode;
            if (k == Keys.P) this.PauseResumeGame();
            else if (k == Keys.Y) this.RestartGame();
            if (control_block)
                if (!prevention)
                {
                    prevention = true;

                    if (k == Keys.W || k == Keys.S || k == Keys.D || k == Keys.A)
                    {
                        if (t.Status != TaskStatus.Running)
                        {
                            t = new Task(ak);
                            t.Start();
                        }
                    }
                    else tank.PlayerShooting(k);
                }
        }

        void ReloadVis(Bullet useless)
        {
            backgroundWorker2.RunWorkerAsync();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (prevention)
                prevention = false;
        }

        void BlockControls()
        {
            control_block = !control_block;
        }

        bool working = false;
        bool domework = true;
        int spawnrate = 850;
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                int elapsed = 0;
                int elapsedtotal = 0;
                while (endless)
                {
                    if (control_block && domework)
                    {
                        working = true;
                        ShootingControl.MoveBullets();
                        EnemyManager.MoveEnemies();
                        while (stopper) { }
                        Thread.Sleep(10);
                        elapsed += 10;
                        elapsedtotal += 10;
                        if (elapsedtotal >= 1000)
                        {
                            mf.IncTimeBonus();
                            spawnrate--;
                            elapsedtotal = 0;
                        }
                        if (elapsed >= spawnrate)
                        {
                            EnemyManager.SpawnEnemy();
                            elapsed = 0;
                            mf.IncTimeBonus();
                        }
                    }
                    working = false;
                }
            }
            catch
            {
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            endless = false;
            backgroundWorker1.Dispose();
            backgroundWorker2.Dispose();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            tank.Boost();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            EnemyManager.SpawnEnemy();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Action nulling = () =>
                {
                    progressBar1.Value = 0;
                };
                progressBar1.Invoke(nulling);
                Action plusing = () =>
                {
                    //progressBar1.Value += progressBar1.Step;
                    //progressBar1.Refresh();
                    //Thread.Sleep(progressBar1.Maximum / progressBar1.Step);
                    progressBar1.Value += progressBar1.Maximum / 2;
                };
                //while (progressBar1.Value < progressBar1.Maximum)
                //{
                progressBar1.Invoke(plusing);
                //}
                Thread.Sleep(500);
                progressBar1.Invoke(plusing);
            }
            catch
            {

            }
        }


    }
}
