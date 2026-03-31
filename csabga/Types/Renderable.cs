using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csabga
{
    public interface Renderable
    {
        void Update();
        void Render(Graphics g, Point windowLocation, Rectangle clientRectangle);

        bool ShouldBeDestroyed();
    }
}
