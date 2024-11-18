using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp8
{
    public partial class Form1 : Form
    {
        private List<Brick> bricks = new List<Brick>();
        private System.Windows.Forms.Timer gameTimer;
        private Point ballPosition;
        private Point ballVelocity;
        private int ballRadius = 10;

        public Form1()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                DefaultExt = "txt",
                AddExtension = true
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (Brick brick in bricks)
                    {
                        writer.WriteLine($"{brick.Location.X},{brick.Location.Y},{brick.Color.ToArgb()},{brick.Hardness}");
                    }
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                DefaultExt = "txt",
                AddExtension = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader reader = new StreamReader(openFileDialog.FileName))
                {
                    bricks.Clear();
                    string? line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        string[] parts = line.Split(',');
                        if (parts.Length == 4)
                        {
                            int x = int.Parse(parts[0]);
                            int y = int.Parse(parts[1]);
                            Color color = Color.FromArgb(int.Parse(parts[2]));
                            int hardness = int.Parse(parts[3]);
                            bricks.Add(new Brick(new Point(x, y), color, hardness));
                        }
                    }
                }
                Invalidate();
                Update();
                Refresh();
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            foreach (Brick brick in bricks)
            {
                if (brick.Contains(e.Location))
                {
                    MessageBox.Show($"Brick clicked at {brick.Location} with hardness {brick.Hardness}");
                    break;
                }
            }
        }

        private void btnChangeColor_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                foreach (Brick brick in bricks)
                {
                    brick.Color = colorDialog.Color;
                }
                Invalidate();
                Update();
                Refresh();
            }
        }

        private void InitializeGame()
        {
            ballPosition = new Point(ClientSize.Width / 2, ClientSize.Height / 2);
            ballVelocity = new Point(5, 5);

            gameTimer = new System.Windows.Forms.Timer();
            gameTimer.Interval = 16; // oko³o 60 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            MoveBall();
            CheckCollisions();
            Invalidate();
        }

        private void MoveBall()
        {
            ballPosition.X += ballVelocity.X;
            ballPosition.Y += ballVelocity.Y;

            if (ballPosition.X <= 0 || ballPosition.X >= ClientSize.Width - ballRadius)
            {
                ballVelocity.X = -ballVelocity.X;
            }

            if (ballPosition.Y <= 0 || ballPosition.Y >= ClientSize.Height - ballRadius)
            {
                ballVelocity.Y = -ballVelocity.Y;
            }
        }

        private void CheckCollisions()
        {
            Rectangle ballRect = new Rectangle(ballPosition.X, ballPosition.Y, ballRadius, ballRadius);

            for (int i = bricks.Count - 1; i >= 0; i--)
            {
                if (bricks[i].Contains(ballPosition))
                {
                    bricks.RemoveAt(i);
                    ballVelocity.Y = -ballVelocity.Y;
                    break;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            foreach (Brick brick in bricks)
            {
                brick.Draw(e.Graphics);
            }

            e.Graphics.FillEllipse(Brushes.Red, new Rectangle(ballPosition.X, ballPosition.Y, ballRadius, ballRadius));
        }
    }

    public class Brick
    {
        public Point Location { get; }
        public Color Color { get; set; }
        public int Hardness { get; }

        private static readonly Size BrickSize = new Size(50, 20);

        public Brick(Point location, Color color, int hardness)
        {
            Location = location;
            Color = color;
            Hardness = hardness;
        }

        public void Draw(Graphics graphics)
        {
            using (Brush brush = new SolidBrush(Color))
            {
                graphics.FillRectangle(brush, new Rectangle(Location, BrickSize));
            }
        }

        public bool Contains(Point point)
        {
            return new Rectangle(Location, BrickSize).Contains(point);
        }
    }

        public void Draw(Graphics graphics)
        {
            using (Brush brush = new SolidBrush(Color))
            {
                graphics.FillRectangle(brush, new Rectangle(Location, Size));
            }
        }

        public bool Contains(Point point)
        {
            return new Rectangle(Location, Size).Contains(point);
        }
    }
}