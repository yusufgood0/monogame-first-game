using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using first_game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace first_game
{
    public class Enemy
    {
        public static Texture2D textures;

        public const int width = 30;
        public const int height = 50;
        public const float movementSpeed = 2f;

        public static List<Rectangle> textureRectangle = new List<Rectangle>();
        public static List<Rectangle> collideRectangle = new List<Rectangle>();
        public static List<Vector2> position = new List<Vector2>();
        public static List<Vector2> target = new List<Vector2>();
        static List<int> health = new List<int>();

        public static bool SightLine(Vector2 pointA, Vector2 pointB)
        {
            int _detail = 20;
            for (int i = 0; i < _detail; i++) 
            {
                Vector2 checkPoint = Vector2.Lerp(pointA, pointB, (float)i / _detail);

                for (int index = 0; index < Tiles.numTiles; index++)
                {
                    if (Tiles.collideRectangle[index].Contains(checkPoint) && Tiles.tileType[index] != 0)
                    {
                        return false;
                    }
                }
            }

            return true; // No obstacles in the way
        }
        public static void move(int _index)
        {
            Vector2 _speed = (target[_index] - position[_index])/20;
            _speed.Normalize();
            position[_index] += _speed;

        }
        public static void Step(int _index)
        {
            move(_index);

            Vector2 _difference = target[_index] - position[_index]; //X and Y difference 
            float _distance = (float)Math.Sqrt(_difference.X * _difference.X + _difference.Y * _difference.Y); //hypotinuse/distance to target
            if (SightLine(new Vector2(Player.collideRectangle.X + (width / 2), Player.collideRectangle.Y + (height / 2)), position[_index]))
            {
                target[_index] = new Vector2(Player.collideRectangle.X + (width / 2), Player.collideRectangle.Y + (height / 2));
            }
            if (_distance > 10)
            {
                float _ratio = movementSpeed / _distance;
                position[_index] += _difference * _ratio;
                collideRectangle[_index] = new Rectangle((int)position[_index].X, (int)position[_index].Y, collideRectangle[_index].Width, collideRectangle[_index].Height);
            }

        }

        public static void create(Vector2 spawn)
        {
            textureRectangle.Add(new Rectangle(0, 0, width, height));
            collideRectangle.Add(new Rectangle((int)spawn.X, (int)spawn.Y, width, height));
            position.Add(new Vector2((int)spawn.X, (int)spawn.Y));
            target.Add(new Vector2(Player.collideRectangle.X + (width/2), Player.collideRectangle.Y + (height / 2)));
            health.Add(50);
        }
        public static void Setup(Texture2D _enemy)
        {
            textures = _enemy;
        }
    }
    public class Player
    {
        public static Texture2D textures;
        public const int width = 30;
        public const int height = 50;
        public const float movementSpeed = 3f;

        public static Rectangle textureRectangle = new Rectangle(0, 0, width, height);
        public static Rectangle collideRectangle = new Rectangle((Tiles.rows * Tiles.tileXY - width) / 2, (Tiles.columns * Tiles.tileXY - height) / 2, width, height);
        public static Vector2  speed = new Vector2(0f, 0f);

        public static void Step()
        {
            collideRectangle.X += (int)speed.X;
            for (int index = 0; index < Tiles.numTiles; index++)
            {
                if (Player.collideRectangle.Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {
                    if (collideRectangle.X > Tiles.collideRectangle[index].X)
                        collideRectangle.X = Tiles.collideRectangle[index].X + Tiles.collideRectangle[index].Width;
                    else
                    {
                        collideRectangle.X = Tiles.collideRectangle[index].X - collideRectangle.Width;
                    }
                }
            }

            collideRectangle.Y += (int)speed.Y;
            for (int index = 0; index < Tiles.numTiles; index++)
            {
                if (Player.collideRectangle.Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {

                    if (collideRectangle.Y > Tiles.collideRectangle[index].Y)
                        collideRectangle.Y = Tiles.collideRectangle[index].Y + Tiles.collideRectangle[index].Height;
                    else
                    {
                        collideRectangle.Y = Tiles.collideRectangle[index].Y - collideRectangle.Height;
                    }
                }
            }

            speed.X /= 2.5f;
            speed.Y /= 2.5f;
        }
        public static void Setup(Texture2D _player)
        {
            textures = _player;
        }
    }
    public class Tiles
    {
        static Random Generator = new Random();
        public static double scale = 1f;

        public const int columns = 15;
        public const int rows = 15;
        public const int tileXY = 60;
        public const int numTiles = columns * rows;

        public static Texture2D[] textures = new Texture2D[3];
        public static Vector2[] textureArray = new Vector2[3];
        public static Rectangle[] textureRectangle = new Rectangle[numTiles];
        public static Rectangle[] collideRectangle = new Rectangle[numTiles];
        public static int[] tileType = {
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 2, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1

        };

        public static void Setup(Texture2D _tiles, Texture2D _dirt, Texture2D _bricks)
        {
            textures[0] = _dirt;
            textureArray[0] = new Vector2(4, 4);
            textures[1] = _tiles;
            textureArray[1] = new Vector2(4, 4);
            textures[2] = _bricks;
            textureArray[2] = new Vector2(1, 1);

            for (int _columns = 0; _columns < columns; _columns++)
            {
                for (int _rows = 0; _rows < rows; _rows++)
                {
                    int _index = _columns * rows + _rows;
                    int _tileXY = textures[tileType[_index]].Height/ (int)textureArray[tileType[_index]].Y;
                    collideRectangle[_index] = new Rectangle(tileXY * _rows, tileXY * _columns, tileXY, tileXY);
                    textureRectangle[_index] = new Rectangle(Generator.Next((int)textureArray[tileType[_index]].X) * _tileXY, Generator.Next((int)textureArray[tileType[_index]].Y) * _tileXY, _tileXY, _tileXY);
                }
            }

        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        KeyboardState keyboardState;
        double gametimer;
        double tps = 30;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Enemy.Setup(Content.Load<Texture2D>("tiles"));
            Player.Setup(Content.Load<Texture2D>("tiles"));
            Tiles.Setup(Content.Load<Texture2D>("tiles"), Content.Load<Texture2D>("dirt"), Content.Load<Texture2D>("Brickwall6_Texture"));
            Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2, 200));
            Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2, 100));
            Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2, 50));
            Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2, 25));

            _graphics.PreferredBackBufferWidth = Tiles.rows * Tiles.tileXY; // Sets the width of the window
            _graphics.PreferredBackBufferHeight = Tiles.columns * Tiles.tileXY; // Sets the height of the window
            _graphics.ApplyChanges(); // Applies the new dimensions

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            
            gametimer += gameTime.ElapsedGameTime.Milliseconds;
            while (gametimer > 1000 / tps)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.Up))
                {
                    Player.speed.Y -= Player.movementSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.Down))
                {
                    Player.speed.Y += Player.movementSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    Player.speed.X -= Player.movementSpeed;
                }
                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    Player.speed.X += Player.movementSpeed;
                }

                Player.Step();
                for (int i = 0; i < Enemy.collideRectangle.Count; i++) {
                    Enemy.Step(i);
                }


                gametimer -= 1000 / tps;
            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // TODO: Add your drawing code here
            _spriteBatch.Begin();


            for (int index = 0; index < Tiles.numTiles; index++)
            {
                _spriteBatch.Draw(Tiles.textures[Tiles.tileType[index]], Tiles.collideRectangle[index], Tiles.textureRectangle[index], Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            }
            for (int index = 0; index < Enemy.collideRectangle.Count; index++)
            {
                _spriteBatch.Draw(Enemy.textures, Enemy.collideRectangle[index], Enemy.textureRectangle[index], Color.White, 0, new Vector2(Enemy.collideRectangle[index].Width/2, Enemy.collideRectangle[index].Height/2), SpriteEffects.None, 0f);
            }

            _spriteBatch.Draw(Player.textures, Player.collideRectangle, Player.textureRectangle, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
