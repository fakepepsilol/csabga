using csabga.ShopUI;
using csabga.UI.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga.UI.DeathScreen
{
    internal class DeathScreen : Button
    {
        private Point lastMouseLocation;
        private Rectangle lastClientRectangle;
        private List<Renderable> renderables = new List<Renderable>();
        public bool ContainsPointer(Point mouseLocation, Rectangle clientRectangle)
        {
            lastMouseLocation = mouseLocation;
            lastClientRectangle = clientRectangle;
            return true;
        }

        public bool CurrentlyHovered() => true;

        public void OnClick(System.Windows.Forms.MouseEventArgs e)
        {
            foreach (var button in renderables.OfType<Button>())
            {
                if (button.ContainsPointer(lastMouseLocation, lastClientRectangle))
                {
                    button.OnClick(e);
                }
            }
        }

        public void OnHoverEnter()
        {
        }

        public void OnHoverExit()
        {
        }

        public void Render(Graphics g, Point windowLocation, Rectangle clientRectangle)
        {
            foreach(var renderable in renderables)
            {
                renderable.Render(g, windowLocation, clientRectangle);
            }
            g.DrawImage(Properties.Resources.DeathScreen, new Rectangle(clientRectangle.Width / 2 - Properties.Resources.DeathScreen.Width * 2, clientRectangle.Height / 2 - Properties.Resources.DeathScreen.Height * 2 - 40, Properties.Resources.DeathScreen.Width * 4, Properties.Resources.DeathScreen.Height * 4));
        }

        public bool ShouldBeDestroyed() => false;

        public void Update()
        {
            foreach (var button in renderables.OfType<Button>())
            {
                if(button.ContainsPointer(lastMouseLocation, lastClientRectangle))
                {
                    if(!button.CurrentlyHovered())
                    {
                        button.OnHoverEnter();
                    }
                } else
                {
                    if(button.CurrentlyHovered())
                    {
                        button.OnHoverExit();
                    }
                }
            }
        }
        public DeathScreen()
        {
            renderables.Add(new Backdrop(Color.FromArgb(0x67, 186, 4, 4)));
            renderables.Add(new ExitButton());
            renderables.Add(new RestartButton());
        }
    }
}
