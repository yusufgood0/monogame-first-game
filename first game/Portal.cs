using System;
using System.Collections.Generic;

using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace first_game
{
    internal class Portal
    {
        public static Texture2D texture;
        public static Rectangle collideRectangle;
        public static void Setup(Texture2D _texture)
        {
            texture = _texture;
        }
        
        public static void ReloadPortalPosition()
        {
            collideRectangle = Tiles.collideRectangle[Tiles.RandomOpen()];
        }
    }
}
