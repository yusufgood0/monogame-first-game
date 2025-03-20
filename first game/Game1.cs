using System;
using System.Reflection;
using System.Security.Cryptography;
using first_game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace first_game
{

    public class Player
    {
        public static Texture2D textures;
        public const int width = 30;
        public const int height = 50;
        public const float movementSpeed = 2.5f;

        public static Rectangle textureRectangle = new Rectangle(0, 0, width, height);
        public static Rectangle collideRectangle = new Rectangle((Tiles.rows * Tiles.tileXY - width) / 2, (Tiles.columns * Tiles.tileXY - height) / 2, width, height);
        public static Vector2  speed = new Vector2(0f, 0f);

        public static void Step()
        {
            collideRectangle.X += (int)speed.X;
            for (int index = 0; index < Tiles.numTiles; index++)
            {
                if (Player.collideRectangle.Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] == 1)
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
                if (Player.collideRectangle.Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] == 1)
                {

                    if (collideRectangle.Y > Tiles.collideRectangle[index].Y)
                        collideRectangle.Y = Tiles.collideRectangle[index].Y + Tiles.collideRectangle[index].Height;
                    else
                    {
                        collideRectangle.Y = Tiles.collideRectangle[index].Y - collideRectangle.Height;
                    }
                }
            }

            speed.X /= 1.5f;
            speed.Y /= 1.5f;
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
        public const int tileXY = 69;
        public const int numTiles = columns * rows;

        public static Texture2D[] textures = new Texture2D[2];
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
                1, 0, 0, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1,
                1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1

        };

        public static void Setup(Texture2D _tiles, Texture2D _dirt)
        {
            textures[0] = _dirt;
            textures[1] = _tiles;

            for (int _columns = 0; _columns < columns; _columns++)
            {
                for (int _rows = 0; _rows < rows; _rows++)
                {
                    int index = _columns * rows + _rows;
                    collideRectangle[index] = new Rectangle(tileXY * _rows, tileXY * _columns, tileXY, tileXY);
                    textureRectangle[index] = new Rectangle(Generator.Next(0,4) * 128, Generator.Next(0, 4)*128, 128, 128);
                }
            }

        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        KeyboardState keyboardState;
        double gametimer ;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
                // TODO: Add your initialization logic here
            Player.Setup(Content.Load<Texture2D>("tiles"));
            Tiles.Setup(Content.Load<Texture2D>("tiles"), Content.Load<Texture2D>("dirt"));
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
            if (gametimer > 20)
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


                gametimer = 0;
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
                _spriteBatch.Draw(Tiles.textures[Tiles.tileType[index]], new Vector2(Tiles.collideRectangle[index].X, Tiles.collideRectangle[index].Y), Tiles.textureRectangle[index], Color.White, 0, new Vector2(0, 0), Tiles.tileXY/128f, SpriteEffects.None, 0f);
            }
            _spriteBatch.Draw(Player.textures, new Vector2(Player.collideRectangle.X, Player.collideRectangle.Y), Player.textureRectangle, Color.White, 0, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
