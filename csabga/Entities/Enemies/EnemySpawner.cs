using csabga.Enemies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace csabga
{
    internal static class EnemySpawner
    {
        private static Timer timer => MainWindow.Instance.EnemySpawnerTimer;
        private static int spawnCount = 0;
        public static void Start()
        {
            timer.Interval = 3000;
            timer.Start();
        }
        static Random r = new Random();
        public static void Tick(object sender, EventArgs e)
        {
            double angle = r.NextDouble() * 2 * Math.PI;
            Vector2 direction = new Vector2(Math.Sin(angle), Math.Cos(angle));
            Vector2 position = MainWindow.Instance.Center;
            while (position.IsInside(MainWindow.Instance.Bounds))
            {
                position += direction * 20;
            }
            position += direction * 20;
            if (spawnCount != 0 && spawnCount % 75 == 0)
            {
                MainWindow.Instance.AddRenderable(new Boss(position));
                spawnCount++;
                return;
            }
            if (r.Next(0, 20) > 15)
            {
                MainWindow.Instance.AddRenderable(new Hexagon(position));
            }
            else
            {
                MainWindow.Instance.AddRenderable(new Triangle(position));
            }
            spawnCount++;
            timer.Interval = Math.Max(200, timer.Interval - 50);
        }
        public static void Reset()
        {
            spawnCount = 0;
            MainWindow.Instance.EnemySpawnerTimer.Start();
        }
    }
}
