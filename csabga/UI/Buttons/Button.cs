using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csabga
{
    public interface Button : Renderable
    {
        bool ContainsPointer(Point mouseLocation, Rectangle clientRectangle);
        void OnHoverEnter();
        bool CurrentlyHovered();
        void OnHoverExit();
        void OnClick(MouseEventArgs e);
    }
}
