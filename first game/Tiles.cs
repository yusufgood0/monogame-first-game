using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Threading;
using first_game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace first_game
{
    public class Tiles
    {
        public static void TakeDamage(Color _color, int _damage, int _iFrames, int index)
        {
            Tiles.swingIFrames[index] = _iFrames;
            Tiles.health[index] -= _damage;
            if (Tiles.health[index] <= 0)
            {
                tileType[index] = (int)tileTypes.NONE;
                load(index, index);
            }
        }
        static Random Generator = new Random();
        public static double scale = 1f;

        public const int columns = 15;
        public const int rows = 15;
        public const int tileXY = 60;
        public const int numTiles = columns * rows;


        public static int[] health = new int[numTiles];
        public static int[] swingIFrames = new int[numTiles];
        public static Texture2D[] textures = new Texture2D[3];
        public static Vector2[] textureArray = new Vector2[3];
        public static Rectangle[] textureRectangle = new Rectangle[numTiles];
        public static Rectangle[] collideRectangle = new Rectangle[numTiles];
        public static int[] tileType = {
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 1,
                1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 2, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 1, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1
        };

        //public static readonly int NONE = 0;
        //public static readonly int SOLID = 1;
        //public static readonly int BRICK = 2;
        public enum tileTypes
        {
            NONE = 0,
            SOLID = 1,
            BRICK = 2,
        }
        public static void regenerateTilemap()
        {
            for (int index = 0; index < numTiles; index++)
            {
                if (tileType[index] == (int)tileTypes.BRICK)
                {
                    tileType[index] = (int)tileTypes.NONE;
                }
                if (Generator.Next(3) == 1 && tileType[index] == (int)tileTypes.NONE)
                {
                    tileType[index] = (int)tileTypes.BRICK;
                }
            }
            load(0, numTiles);
        }
        public static void load(int index_start, int index_end)
        {
            for (int _columns = 0; _columns < columns; _columns++)
                for (int _rows = 0; _rows < rows; _rows++)
                {
                    int _index = _columns * rows + _rows;
                    if (_index <= index_end && _index >= index_start)
                    {
                        int _tileXY = textures[tileType[_index]].Height / (int)textureArray[tileType[_index]].Y;
                        health[_index] = 25;
                        collideRectangle[_index] = new Rectangle(tileXY * _rows, tileXY * _columns, tileXY, tileXY);
                        textureRectangle[_index] = new Rectangle(Generator.Next((int)textureArray[tileType[_index]].X) * _tileXY, Generator.Next((int)textureArray[tileType[_index]].Y) * _tileXY, _tileXY, _tileXY);

                    }
                }
        }
        public static void setup(Texture2D _tiles, Texture2D _dirt, Texture2D _bricks)
        {
            textures[0] = _dirt;
            textureArray[0] = new Vector2(4, 4);
            textures[1] = _tiles;
            textureArray[1] = new Vector2(4, 4);
            textures[2] = _bricks;
            textureArray[2] = new Vector2(1, 1);
            load(0, numTiles);
        }

    }
}
