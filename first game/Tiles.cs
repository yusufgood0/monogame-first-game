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
using static first_game.Levels;

namespace first_game
{
    public class Tiles
    {
        static readonly Random rnd = new();
       

        
        public static int[] tileType = Levels.levelMaps[Levels.Level];


        static Random Generator = new Random();
        public static double scale = 1f;

        public const int columns = 20;
        public const int rows = 20;
        public const int tileXY = 60;

        public const int numTiles = columns * rows;


        public static int[] health = new int[numTiles];
        public static int[] swingIFrames = new int[numTiles];
        public static Texture2D[] textures = new Texture2D[4];
        public static Vector2[] textureArray = new Vector2[4];
        public static Rectangle[] textureRectangle = new Rectangle[numTiles];
        public static Rectangle[] collideRectangle = new Rectangle[numTiles];


        public enum tileTypes
        {
            NONE = 0,
            SOLID = 1,
            BRICK = 2,
            GATE = 3,
        }

        public static void TakeDamage(Color _color, int _damage, int _iFrames, int index)
        {
            Tiles.swingIFrames[index] = _iFrames;
            Tiles.health[index] -= _damage;
            if (Tiles.health[index] <= 0)
            {
                tileType[index] = (int)tileTypes.NONE;
                loadTiles(index, index);
            }
        }

        public static int RandomOpen(tileTypes _tileType)
        {
            int _tileIndex = rnd.Next(Tiles.numTiles - 1);
            while (Tiles.tileType[_tileIndex] != (int)_tileType)
            {
                _tileIndex = rnd.Next(Tiles.numTiles - 1);
            }
            return _tileIndex;
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
            loadTiles(0, numTiles);
        }
        public static void loadTiles(int index_start, int index_end)
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
        public static void setup(object[] _textures)
        {
            for (int i = 0; i * 2 < _textures.Length; i++)
            {
                textures[i] = (Texture2D)_textures[i * 2];
                textureArray[i] = (Vector2)_textures[i * 2 + 1];
            }
            loadTiles(0, numTiles);
        }

    }
}
