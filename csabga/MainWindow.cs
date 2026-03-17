using csabga.Buttons;
using csabga.ShopUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csabga
{
    public partial class MainWindow : Form
    {
        public MainWindow()
        {
            InitializeComponent();
            DoubleBuffered = true;
            Instance = this;
        }
        public static MainWindow Instance;
        public const int TargetFps = 30;
        public const int TargetFrameTime = 1000 / TargetFps;
        public Random R = new Random();
        public Timer FrameTimer => frameTimer;
        public Timer EnemySpawnerTimer => enemySpawnerTimer;

        public Color playerColor;
        public SolidBrush bulletBrush;


        public Player Player;

        public Vector2 Center => new Vector2(Location.X + ClientRectangle.Width / 2, Location.Y + ClientRectangle.Height / 2);
        public Rectangle ScreenBounds;
        public Point relativePointerLocation = new Point(0, 0);
        public string SystemFontName => Font.SystemFontName;
        private void MainWindow_Load(object sender, EventArgs e)
        {
            MaximumSize = Size;

            ScreenBounds = Screen.PrimaryScreen.Bounds;
            playerColor = Color.White;
            bulletBrush = new SolidBrush(Color.White);
            Player = new Player(Location.X + ClientRectangle.Width / 2, Location.Y + ClientRectangle.Height / 2);
            renderables.Add(Player);
            Buttons.Add(new ShopButton());
            ShopUi.Add(new ExitShopButton());
            ShopUi.Add(new ShopTitle());
            ShopUi.Add(new RerollButton(R, CreateGraphics()));
            frameTimer.Interval = 10;
            frameTimer.Start();
            EnemySpawner.Start();
        }
        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MAXIMIZE = 0xF030;

            if (m.Msg == WM_SYSCOMMAND)
            {
                int command = m.WParam.ToInt32() & 0xFFF0;
                if (command == SC_MAXIMIZE)
                {
                    return;
                }
            }
            base.WndProc(ref m);
        }
        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                var enemies = renderables.OfType<Enemy>().ToList();
                foreach (Enemy enemy in enemies)
                {
                    enemy.Health = 0;
                    renderables.Add(new HitMarker(enemy.Position, Color.Red));
                    renderables.Add(new Coin(enemy.Position, (float)((R.NextDouble() + 1) * enemy.KillReward)));
                }
            }
            if (e.KeyCode == Keys.F2)
            {
                Location = new Point(Location.X - 20, Location.Y - 20);
                ResizeWindow(new Size(Size.Width + 20, Size.Height + 20));
            }
            if (e.KeyCode == Keys.F3)
            {
                Player.Coins += 67;
            }
            Player.OnKeyDown(e);
        }
        private void MainWindow_KeyUp(object sender, KeyEventArgs e)
        {
            Player.OnKeyUp(e);
        }

        private List<Renderable> renderables = new List<Renderable>();
        private object _renderablesLock = new object();
        public void AddRenderable(Renderable renderable)
        {
            lock (_renderablesLock)
            {
                renderables.Add(renderable);
            }
        }
        private void frameTimer_Tick(object sender, EventArgs e)
        {
            lock (_renderablesLock)
            {
                try
                {
                    if (ShopOpened || Player.Dead) return;
                    for (int i = 0; i < renderables.Count; i++)
                    {
                        var renderable = renderables[i];
                        renderable.Update();

                        bool _continue = false;
                        if (renderable is Bullet bullet)
                        {
                            var enemies = renderables.OfType<Enemy>().ToList();
                            for (int j = 0; j < enemies.Count; j++)
                            {
                                var enemy = enemies[j];
                                if (enemy.CollidesWith(bullet) && !bullet.PreviouslyHit(enemy))
                                {
                                    enemy.OnHit(bullet.RemainingDamage);
                                    if (enemy.ShouldBeDestroyed())
                                    {
                                        renderables.Add(new Coin(enemy.Position, (float)((R.NextDouble() + 1) * enemy.KillReward)));
                                        renderables.Add(new HitMarker(bullet.position.ToPoint(), Color.Red));
                                    }
                                    else
                                    {
                                        renderables.Add(new HitMarker(bullet.position.ToPoint(), Color.White));
                                    }

                                    bullet.OnHit(enemy);
                                    _continue = true;
                                    break;
                                }
                            }
                        }
                        if (renderable is Player player)
                        {
                            var coins = renderables.OfType<Coin>().ToList();
                            for (int j = 0; j < coins.Count; j++)
                            {
                                if (player.CollidesWith(coins[j]))
                                {
                                    coins[j].OnPickup();
                                    player.Coins += coins[j].Value;
                                    renderables.Add(new HitMarker(player.Position, coins[j].Color));
                                }
                            }
                            var enemies = renderables.OfType<Enemy>().ToList();
                            for (int j = 0; j < enemies.Count; j++)
                            {
                                var enemy = enemies[j];
                                if (enemy.CollidesWith(player))
                                {
                                    player.externalForce = (player.Position - enemy.Position);
                                    player.OnHit();
                                }
                            }
                        }
                        if (_continue) continue;
                        if (renderable.ShouldBeDestroyed())
                        {
                            renderables.RemoveAt(i);
                        }
                    }
                }
                finally
                {
                    Refresh();
                }
            }
        }

        public static DateTime lastFrame = DateTime.Now;
        public static double deltaTime;
        private void MainWindow_Paint(object sender, PaintEventArgs e)
        {
            UpdateWindowLocation();

            deltaTime = (DateTime.Now - lastFrame).TotalMilliseconds / TargetFrameTime;

            var simulatedWindowLocation = (new Vector2(relativePointerLocation) / ((ClientRectangle.Width + ClientRectangle.Height) / 2) * 10 + Location).ToPoint();


            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            try
            {
                for (int i = 0; i < renderables.Count; i++)
                {
                    renderables[i].Render(e.Graphics, simulatedWindowLocation, ClientRectangle);
                }
                if (ShopOpened)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), 0, 0, ClientRectangle.Width, ClientRectangle.Height);
                    foreach (Renderable renderable in ShopUi.Concat(ShopItems))
                    {
                        renderable.Render(e.Graphics, simulatedWindowLocation, ClientRectangle);
                    }
                }
                // coin count
                e.Graphics.DrawImage(Properties.Resources.CoinIcon, new Rectangle(20, 20, 40, 40));
                e.Graphics.DrawString(((int)Player.Coins).ToString(), new Font(Font.SystemFontName, 16, FontStyle.Bold), new SolidBrush(Color.FromArgb((int)(0.9 * 0xFF), 252, 251, 230)), new Rectangle(60, 20, 100, 40), new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });

                e.Graphics.DrawImage(Properties.Resources.HealthIcon, new Rectangle(20, 70, 40, 40));
                e.Graphics.DrawString(Player.Health.ToString(), new Font(Font.SystemFontName, 16, FontStyle.Bold), new SolidBrush(Color.FromArgb((int)(0.9 * 0xFF), 252, 251, 230)), new Rectangle(60, 70, 100, 40), new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center });


                foreach (Button button in Buttons)
                {
                    button.Render(e.Graphics, simulatedWindowLocation, ClientRectangle);
                }

                if(Player.Dead)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(0x67, 186, 4, 4)), ClientRectangle);
                    e.Graphics.DrawImage(Properties.Resources.DeathScreen, new Rectangle(ClientRectangle.Width / 2 - Properties.Resources.DeathScreen.Width * 2, ClientRectangle.Height / 2 - Properties.Resources.DeathScreen.Height * 2, Properties.Resources.DeathScreen.Width * 4, Properties.Resources.DeathScreen.Height * 4));
                }
            }
            finally
            {
                lastFrame = DateTime.Now;
            }
        }

        private bool _shopOpened = false;
        public bool ShopOpened
        {
            get => _shopOpened;
            set
            {
                enemySpawnerTimer.Enabled = !value;
                _shopOpened = value;
            }
        }

        public List<Button> Buttons = new List<Button>();
        public List<Renderable> ShopUi = new List<Renderable>();
        public List<ShopItemButton> ShopItems = new List<ShopItemButton>();
        public Button HoveredButton = null;
        private void MainWindow_MouseDown(object sender, MouseEventArgs e)
        {
            if (Player.Dead) return;
            if (HoveredButton != null) return;
            Player.OnMouseDown(e, Location);
        }
        private void MainWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if(Player.Dead) return;
            if (HoveredButton != null)
            {
                HoveredButton.OnClick(e);
            }
            Player.OnMouseUp(e, Location);
        }
        private void MainWindow_MouseMove(object sender, MouseEventArgs e)
        {
            if(Player.Dead) return;
            if (!ShopOpened)
            {
                relativePointerLocation = new Point(e.X - ClientRectangle.Width / 2, e.Y - ClientRectangle.Height / 2);
            }
            bool anyButtonsHovered = false;
            foreach (Button button in ShopOpened ? Buttons.Concat(ShopUi.OfType<Button>()).Concat(ShopItems) : Buttons)
            {
                if (button.ContainsPointer(e.Location, ClientRectangle))
                {
                    anyButtonsHovered = true;
                    HoveredButton = button;
                    if (!HoveredButton.CurrentlyHovered())
                    {
                        HoveredButton.OnHoverEnter();
                    }
                }
                else if (button.CurrentlyHovered())
                {
                    button.OnHoverExit();
                }
            }
            if (!anyButtonsHovered)
            {
                HoveredButton = null;
            }
            Player.OnMouseMove(e, Location);
        }

        private void ResizeWindow(Size size)
        {
            MaximumSize = size;
            Size = size;
            MinimumSize = size;
        }
        public void ExpandWindow(int pixels)
        {
            var newSize = Size;
            newSize.Width += pixels;
            newSize.Height += pixels;
            ResizeWindow(newSize);
        }
        private void UpdateWindowLocation()
        {
            if (MinimumSize != Size || MaximumSize != Size)
            {
                MinimumSize = Size;
                MaximumSize = Size;
            }
            Vector2 windowCenter = new Vector2(Location.X + ClientRectangle.Width / 2, Location.Y + ClientRectangle.Height / 2);
            double distanceX = Player.Position.X - windowCenter.X;
            double distanceY = Player.Position.Y - windowCenter.Y;
            double distanceFromCenter = Math.Sqrt(distanceX * distanceX + distanceY * distanceY);

            if (distanceFromCenter > 70)
            {
                double rx = cubicEaseIn(Math.Min(Math.Abs(distanceX) / ClientRectangle.Width / 2 + 0.1, 1));
                double diffX = ClientRectangle.Width * rx;
                double ry = cubicEaseIn(Math.Min(Math.Abs(distanceY) / ClientRectangle.Height / 2 + 0.1, 1));
                double diffY = ClientRectangle.Height * ry;
                if (distanceX < 0)
                {
                    diffX *= -1;
                }
                if (distanceY < 0)
                {
                    diffY *= -1;
                }

                var newLocation = new Point((int)(Location.X + diffX), (int)(Location.Y + diffY));
                bool xValid = (newLocation.X >= 0 && (newLocation.X + ClientRectangle.Width) <= ScreenBounds.Width);
                bool yValid = (newLocation.Y >= 0 && (newLocation.Y + ClientRectangle.Height) <= ScreenBounds.Height);
                Location = new Point(xValid ? ((newLocation.X + Location.X) / 2) : (newLocation.X <= 0 ? 0 : ScreenBounds.Width - ClientRectangle.Width), yValid ? ((newLocation.Y + Location.Y) / 2) : (newLocation.Y <= 0 ? 0 : ScreenBounds.Height - ClientRectangle.Height));
            }
        }
        private double cubicEaseIn(double x)
        {
            return x * x * x;
        }

        private void enemySpawnerTimer_Tick(object sender, EventArgs e)
        {
            EnemySpawner.Tick(sender, e);
        }
    }
}
