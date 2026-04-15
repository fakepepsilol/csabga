using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace csabga
{
    public interface Enemy : Renderable
    {
        Vector2 Position { get; }
        bool CollidesWith(Bullet bullet);
        bool CollidesWith(Player player);
        void OnHit(int damage);

        float KillReward { get; }
        int Health { get; set; }
    }
}
