using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static first_game.Constants;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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
        public static readonly float homingStrength = .7f;

        //slider Positions
        public static Rectangle healthSliderRect;
        public static Rectangle staminaSliderRect;

        public static Rectangle sensitivitySliderRect;
        public static Rectangle FOVSliderRect;
        public static Rectangle detailSliderRect;

        public static readonly float jumpHeight = 400;
        public static readonly float jumpWidth = 6;
        public static readonly float defaultPlayerHeight = 350;
        public static readonly float floorLevel = -500;

        public static readonly int maxHealth = 1000;
        public static readonly int maxStamina = 1500;

        public static readonly float maxDetail = 0.01f;
        public static readonly float maxSensitivity = 1f / 100f;
        public static readonly float maxFOV = (float)Math.PI * .67f;

        public static readonly float minDetail = 0.0030f;
        public static readonly float minSensitivity = .25f / 100f;
        public static readonly float minFOV = (float)Math.PI * .33f;

        public static readonly int tps = 45; 
        //public static readonly int tpsPerSec = 1000 / tps;

        public static float cameraLag = 0.25f; // how much the camera sould lag behind the player

        public class Luminance
        {
            public static float Player = 0.25f;
            public static float Projectile = .1f;
            public static float Enemy = .00f;
            public static float LevelEnd = .1f;
        }

        public static readonly float maxPlayerLightEmit = (float)Luminance.Player;
        public static readonly float LightStrength = 0.0003f;

        public static int maxLightLevel = 300; // the light level where everything is completely lit
        public static int minLightLevel = 30; // the lowest possible light level
        public static float lightlevelLoss = .3f; // the light lost speed

        public class EnemyStats
        {
            public static List<float> movementSpeed = new();
            public static List<int> damage = new();
            public static List<int> health = new();
            public static List<int> height = new();
            public static List<int> width = new();
            public static List<bool> circle = new();
            public static void Setup()
            {
                //stats for Small enemies
                movementSpeed.Add(5f);
                damage.Add(80);
                health.Add(50);
                height.Add(24);
                width.Add(24);
                circle.Add(true);

                //stats for Medium enemies
                movementSpeed.Add(2.8f);
                damage.Add(150);
                health.Add(150);
                height.Add(40);
                width.Add(40);
                circle.Add(true);

                //stats for Large enemies
                movementSpeed.Add(2.1f);
                damage.Add(450);
                health.Add(300);
                height.Add(60);
                width.Add(60);
                circle.Add(true);

                //stats for Archer enemies
                movementSpeed.Add(1.5f);
                damage.Add(100);
                health.Add(100);
                height.Add(30);
                width.Add(30);
                circle.Add(false);

                //stats for BOSS enemy
                movementSpeed.Add(9f);
                damage.Add(10);
                health.Add(1500);
                height.Add(80);
                width.Add(80);
                circle.Add(false);
            }
        }
        public class Archer
        {
            public static readonly int archerDamage = 250;
            public static readonly int attackDelay = tps * 3;
            public static readonly int archerStopRange = Tiles.tileXY * 5; // how close the archer can get before stopping
            public static readonly float archerBackupRange = archerStopRange * 0.8f; // at what range should the archer starts backing up
            public static readonly float archerBackupSpeed = 2.5f; // how fast to the archers movement speed should he back up
        }
    }
}
