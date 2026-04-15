using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace csabga.UI.Components
{
    public class BossBar : Renderable
    {
        public float Max { get; set; } = 100f;
        public float Value { get; set; } = 100f;
        public Color fillColor = Color.Orange;

        private bool isVisible = false;
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (isVisible == value) return;
                if (value)
                {
                    MainWindow.Instance.EnemySpawnerTimer.Stop();
                }
                else
                {
                    MainWindow.Instance.EnemySpawnerTimer.Interval += 1000;
                    MainWindow.Instance.EnemySpawnerTimer.Start();
                }
                isVisible = value;
            }
        }
        public bool TextVisible { get; set; } = true;
        public string Text { get; set; } = "Boss";

        private const int margin = 30;
        private int width = 0;
        private int height = 5;


        private Rectangle GetBounds(Rectangle clientRectangle) => new Rectangle(clientRectangle.Width / 2 - width / 2, clientRectangle.Height - 15, width, height);
        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            width = clientRectangle.Width - margin * 2;
            if (!IsVisible) return;
            var bounds = GetBounds(clientRectangle);
            var fillBounds = new Rectangle(clientRectangle.Width / 2 - width / 2, clientRectangle.Height - 15, (int)(width * (Value / Max)), height);
            g.FillRectangle(new SolidBrush(fillColor), fillBounds);
            g.DrawRectangle(new Pen(Color.White), bounds);

            var textBounds = new Rectangle(clientRectangle.Width / 2 - width / 2, clientRectangle.Height - 35, width, 15);
            g.DrawString(Text, new Font(MainWindow.Instance.SystemFontName, 10f), new SolidBrush(Color.White), textBounds, new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            });
        }

        public bool ShouldBeDestroyed() => false;

        public void Update() { }
    }
}
