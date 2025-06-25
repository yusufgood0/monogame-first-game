using System;
using System.Collections.Generic;

using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using static first_game.Tiles;

namespace first_game
{
    internal class Portal
    {
        public static Color Color = Color.Aquamarine;
        public static readonly int portalVisualSize = 50;
        public static Texture2D texture;
        public static readonly int amountOfFrames = 3;
        public static double textureFrame;
        public static Rectangle collideRectangle = new();
        public static SoundEffect soundEffect;
        public static void Setup(Texture2D _texture, SoundEffect _soundEffect)
        {
            texture = _texture;
            soundEffect = _soundEffect;
        }

        public static void ReloadPortalPosition()
        {
            collideRectangle = Tiles.collideRectangle[Tiles.RandomTile(tileTypes.NONE)];
            soundEffect.Play(Game1.sfxVolume * 0.3f, 0.3f, 0);
        }
        public static void update()
        {
            if (RequirementsMet())
            {
                Portal.Color = Color.BlueViolet;

                if (CollideWithPlayer())
                {
                    Levels.SetLevel(Levels.Level + 1);
                    ReloadPortalPosition();
                } 
            }
            updateTexture();
        }
        public static bool RequirementsMet()
        {
            //if (Enemy.livingEnemies().Count <= Levels.EnemySaves[Levels.Level].Sum() / 2) // for half enemies killed
            if (Enemy.livingEnemies().Count == 0) // every enemy killed 
            {
                return true;
            }
            return false;
        }
        public static bool CollideWithPlayer()
        {

            if (Portal.collideRectangle.Intersects(General.Vector2toRectangle(Player.position, Player.width, Player.height)))
            {
                return true;
            }
            return false;
        }
        public static void updateTexture()
        {
            textureFrame += 0.1;
            if (textureFrame > amountOfFrames)
            {
                textureFrame = 0;
            }
        }
    }
}
