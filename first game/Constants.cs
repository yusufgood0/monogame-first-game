using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static first_game.Constants;
/*
 * DARK WIZARD
 * EPIC BOSS
 * DECREASING LIGHT
 * 
 * 
 */


namespace first_game
{
    internal class Constants
    {
        public static readonly int tps = 45; 
        public static readonly int tpsPerSec = 1000 / tps;

        public static float cameraLag = 0.25f; // how much the camera sould lag behind the player

        public class Luminance
        {
            public static float Player = 0.25f;
            public static float Projectile = .1f;
            public static float Enemy = .2f;
            public static float LevelEnd = .1f;
        }

        public static readonly float maxPlayerLightEmit = (float)Luminance.Player ;
        public static readonly float LightStrength = 0.0003f;
        public class EnemyStats
        {
            public static List<float> movementSpeed = new();
            public static List<int> damage = new();
            public static List<int> health = new();
            public static List<int> height = new();
            public static List<int> width = new();
            public static void Setup()
            {
                //stats for Small enemies
                movementSpeed.Add(5f);
                damage.Add(40);
                health.Add(50);
                height.Add(24);
                width.Add(24);

                //stats for Medium enemies
                movementSpeed.Add(2.8f);
                damage.Add(100);
                health.Add(150);
                height.Add(40);
                width.Add(40);

                //stats for Large enemies
                movementSpeed.Add(2.1f);
                damage.Add(200);
                health.Add(300);
                height.Add(60);
                width.Add(60);

                //stats for Archer enemies
                movementSpeed.Add(2.3f);
                damage.Add(200);
                health.Add(100);
                height.Add(40);
                width.Add(25);
            }
        }
        public class Archer
        {
            public static readonly int attackDelay = tps * 3;
            public static readonly int archerStopRange = Tiles.tileXY * 3; // how close the archer can get before stopping
            public static readonly float archerBackupRange = archerStopRange * 0.6f; // at what range should the archer starts backing up
            public static readonly float archerBackupSpeed = 1f; // how fast to the archers movement speed should he back up
        }
    }
}
