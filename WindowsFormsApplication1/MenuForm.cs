using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SPtankgame
{
    public partial class MenuForm : Form
    {
        Form1 f;
        bool gamestate = true;
        int score = 0;
        double moder = 1;
        int streak = 0;
        int timebonus = 0;

        string playername = "player";

        public double Moder
        {
            get { return moder; }
            set { if (value >= 1) moder = value; }
        }

        public MenuForm(Form1 f)
        {
            InitializeComponent();
            this.f = f;
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {
            ShootingControl.Boom += ScoreAdd;
            this.Location = new Point(f.Location.X + f.Width, f.Location.Y);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            f.RestartGame();
            gamestate = true;
            Action exortexortexort = () =>
            {
                PlusLabel.Text = "+0";
                ScoreLabel.Text = "0";
            };
            score = 0;
            streak = 0;
            timebonus = 0;
            this.Invoke(exortexortexort);
        }

        void ScoreAdd(Bullet b)
        {
            if (gamestate)
            {
                if (b.EnemyHit)
                {
                    streak++;

                    int gift = (int)Math.Round(100 * Moder) + 10 * streak + timebonus;
                    score += gift;
                    Action exortexortexort = () =>
                    {
                        PlusLabel.Text = "100x" + Moder.ToString() + " + 10x" + streak + " + " + timebonus + " = " + gift;
                        ScoreLabel.Text = score.ToString();
                    };
                    this.Invoke(exortexortexort);
                }
                else streak = 0;
            }
        }

        public void ChangeModer(double moder)
        {
            Moder = moder;
            Action opa = () =>
                {
                    ModerLabel.Text = Moder.ToString();
                };
            this.Invoke(opa);
        }

        public void IncTimeBonus()
        {
            timebonus++;
        }

        public void GameOver()
        {
            if (gamestate)
            {
                gamestate = false;
                RefreshHighscoreFile(playername, score);
                RefreshTableBox();
            }
        }

        public void GameStart()
        {
            gamestate = true;
            playername = textBox1.Text;
            RefreshTableBox();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            f.PauseResumeGame();
        }

        void RefreshTableBox()
        {
            Action aadsad = () =>
            {
                richTextBox1.Text = "";
                foreach (string[] s in GetHighscoreTable())
                {
                    richTextBox1.Text += s[0] + " " + s[1] + Environment.NewLine;
                }
            };
            richTextBox1.Invoke(aadsad);
        }

        List<string[]> GetHighscoreTable()
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\TGhighscoretable.sptgf", FileMode.OpenOrCreate);
            List<string[]> data = new List<string[]>();
            StreamReader sw = new StreamReader(fs);
            while (sw.Peek() != -1)
            {
                data.Add(new string[2]);
                string[] tmp = new string[2];
                data[data.Count - 1] = sw.ReadLine().Split(' ');
            }
            sw.Close();
            fs.Close();
            return data;
        }

        void RefreshHighscoreFile(string playername, int score)
        {
            List<string[]> lines = GetHighscoreTable();
            lines.Add(new string[2]);
            lines[lines.Count - 1][0] = playername;
            lines[lines.Count - 1][1] = score.ToString();
            List<string[]> toremove = new List<string[]>();
            lines.Sort((l1, l2) =>
            {
                if (l1 == null || l2 == null)
                    return 0;
                int a = Convert.ToInt32(l1[1]);
                int b = Convert.ToInt32(l2[1]);
                if (l1[0] == l2[0])
                {
                    if (a >= b) toremove.Add(l2);
                    else toremove.Add(l1);
                }
                if (a < b)
                    return 1;
                else if (a == b) return 0;
                else return -1;
            });
            foreach (string[] s in toremove)
            {
                lines.Remove(s);
            }
            ReCreateHighscoreTable(lines);
        }

        void ReCreateHighscoreTable(List<string[]> lines)
        {
            FileStream fs = new FileStream(Application.StartupPath + "\\TGhighscoretable.sptgf", FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            foreach (string[] s in lines)
            {
                sw.WriteLine(s[0] + " " + s[1]);
            }
            sw.Close();
            fs.Close();
        }

        private void MenuForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
