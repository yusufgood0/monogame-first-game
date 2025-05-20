using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace first_game
{
    internal class Gems
    {
        public static Texture2D gemTexture;
        public static Vector2 gemTextureArray = new Vector2(4, 3);
        public static Rectangle[] TextureRect = new Rectangle[12];
        public static void setup()
        {
            Vector2 vector2 = new Vector2((int)(gemTexture.Width / gemTextureArray.X), (int)(gemTexture.Height / gemTextureArray.Y));
            int a = 0;
            for (int rows = 0; rows < 4; rows++)
            {
                for (int colums = 0; colums < 3; colums++)
                {
                    TextureRect[a] = new Rectangle((int)vector2.X * rows, (int)vector2.Y * colums, (int)vector2.X, (int)vector2.Y);
                    a += 1;
                }
            }
        }
        public static void Draw(SpriteBatch _spriteBatch, Vector2 position, int gemstone, Color color, float size, float layer)
        {
            int scale = (int)(size / (gemTexture.Width / gemTextureArray.X));
            _spriteBatch.Draw(
                    gemTexture,
                    new (position.X - size/2, position.Y - size / 2),
                    TextureRect[gemstone],
                    color,
                    0,
                    new(),
                    //new((int)(gemTexture.Width / gemTextureArray.X) / 2, (int)(gemTexture.Height / gemTextureArray.Y) / 2),
                    size / (gemTexture.Width / gemTextureArray.X),
                    SpriteEffects.None,
                    layer
                    );
        }
    }
}
