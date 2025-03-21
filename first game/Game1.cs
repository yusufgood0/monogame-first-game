using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Metadata;
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

        public static bool SightLine(Vector2 pointA, Vector2 pointB, int _detail)
        {
            for (int i = 0; i < _detail; i++) 
            {
                Vector2 checkPoint = Vector2.Lerp(pointA, pointB, (float)i / _detail);
                for (int index = 0; index < Tiles.numTiles; index++)
                    if (Tiles.collideRectangle[index].Contains(checkPoint) && Tiles.tileType[index] != 0)
                        return false;
            }

            return true; // No obstacles in the way
        }
        public static void Step(int _index)
        {

            Vector2 _difference = target[_index] - position[_index] ; //X and Y difference 
            float _distance = (float)Math.Sqrt(_difference.X * _difference.X + _difference.Y * _difference.Y); //hypotinuse/distance to target
            if (SightLine(Player.position, position[_index], 20))
                target[_index] = Player.position;
            if (_distance > 10)
            {
                _difference.Normalize();
                position[_index] += _difference*movementSpeed;
                collideRectangle[_index] = new Rectangle((int)position[_index].X, (int)position[_index].Y, collideRectangle[_index].Width, collideRectangle[_index].Height);
            }

        }

        public static void create(Vector2 spawn)
        {
            textureRectangle.Add(new Rectangle(0, 0, width, height));
            collideRectangle.Add(new Rectangle((int)spawn.X, (int)spawn.Y, width, height));
            position.Add(new Vector2((int)spawn.X, (int)spawn.Y));
            target.Add(Player.position);
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
        public const int height = 40;
        public const int collisionSize = 30;
        
        public static Rectangle textureRectangle = new Rectangle(0, 0, width, height);

        public static float angle;
        public static Vector2 angleVector;

        public static Vector2 position = new Vector2((Tiles.rows * Tiles.tileXY - width) / 2, (Tiles.columns * Tiles.tileXY - height) / 2);
        //public static Rectangle collideRectangle = new Rectangle((int)position.X, (int)position.Y, width, height);
        public static Vector2  speed = new Vector2(0f, 0f);
        public static float movementSpeed = 3f;


        public static void Step()
        {
            if (speed != new Vector2(0, 0))
                speed.Normalize();

            position.X += (int)(speed.X * movementSpeed);
            for (int index = 0; index < Tiles.numTiles; index++)
                if (new Rectangle((int)position.X - collisionSize / 2, (int)position.Y - collisionSize / 2, collisionSize, collisionSize).Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {
                    if (position.X > Tiles.collideRectangle[index].X)
                        position.X = Tiles.collideRectangle[index].X + Tiles.collideRectangle[index].Width + collisionSize / 2;
                    else
                        position.X = Tiles.collideRectangle[index].X - collisionSize / 2;
                }

            position.Y += (int)(speed.Y*movementSpeed);
            for (int index = 0; index < Tiles.numTiles; index++)
                if (new Rectangle((int)position.X - collisionSize / 2, (int)position.Y - collisionSize / 2, collisionSize, collisionSize).Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {

                    if (position.Y > Tiles.collideRectangle[index].Y)
                        position.Y = Tiles.collideRectangle[index].Y + Tiles.collideRectangle[index].Height + collisionSize / 2;
                    else
                        position.Y = Tiles.collideRectangle[index].Y - collisionSize / 2;
                }
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
                for (int _rows = 0; _rows < rows; _rows++)
                {
                    int _index = _columns * rows + _rows;
                    int _tileXY = textures[tileType[_index]].Height/ (int)textureArray[tileType[_index]].Y;
                    collideRectangle[_index] = new Rectangle(tileXY * _rows, tileXY * _columns, tileXY, tileXY);
                    textureRectangle[_index] = new Rectangle(Generator.Next((int)textureArray[tileType[_index]].X) * _tileXY, Generator.Next((int)textureArray[tileType[_index]].Y) * _tileXY, _tileXY, _tileXY);
                }

        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        KeyboardState movementKeyboardState;
        KeyboardState previousKeyboardState;
        KeyboardState keyboardState;
        MouseState mouseState;



        Texture2D blankTexture;

        int gametimer;
        int tps = 30;
        readonly int maxDashCharge = 3;
        readonly int dashCooldown = 1500; //how long a dash cooldown is in ms
        readonly int dashLength = 70; //how long a dash is in ms
        int dashing = 0;
        int dashTimer = 0;
        int timeElapsed;
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

            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.Red }); // Fills the texture with white

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
            timeElapsed = gameTime.ElapsedGameTime.Milliseconds;
            gametimer += timeElapsed;
            while (gametimer > 1000 / tps)
            {
                mouseState = Mouse.GetState();
                keyboardState = Keyboard.GetState();

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                Player.angleVector = new Vector2(mouseState.Y - Player.position.Y, mouseState.X - Player.position.X);
                Player.angleVector.Normalize();
                Player.angle = (float)Math.Atan2(Player.angleVector.Y, Player.angleVector.X) + 1.57f;



                if (dashing < 0)
                {
                    Player.movementSpeed = 3;
                    movementKeyboardState = Keyboard.GetState();
                    if (dashTimer < (maxDashCharge * dashCooldown))
                        dashTimer += timeElapsed;

                    if (dashTimer > dashCooldown && keyboardState.IsKeyDown(Keys.LeftShift) && !previousKeyboardState.IsKeyDown(Keys.LeftShift) && Player.speed != new Vector2(0, 0))
                    {
                        dashTimer -= dashCooldown;
                        dashing = dashLength;
                    }
                }
                else
                {
                    Player.movementSpeed = 15;
                    dashing -= timeElapsed;
                }
                Player.speed = new Vector2(0, 0);
                if (movementKeyboardState.IsKeyDown(Keys.W))
                    Player.speed.Y -= Player.movementSpeed;

                if (movementKeyboardState.IsKeyDown(Keys.S))
                    Player.speed.Y += Player.movementSpeed;

                if (movementKeyboardState.IsKeyDown(Keys.A))
                    Player.speed.X -= Player.movementSpeed;

                if (movementKeyboardState.IsKeyDown(Keys.D))
                    Player.speed.X += Player.movementSpeed;


                Player.Step();
                for (int i = 0; i < Enemy.collideRectangle.Count; i++) 
                    Enemy.Step(i);


                previousKeyboardState = Keyboard.GetState();
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
                _spriteBatch.Draw(Tiles.textures[Tiles.tileType[index]], Tiles.collideRectangle[index], Tiles.textureRectangle[index], Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
           
            for (int index = 0; index < Enemy.collideRectangle.Count; index++)
                _spriteBatch.Draw(Enemy.textures, Enemy.collideRectangle[index], Enemy.textureRectangle[index], Color.White, 0, new Vector2(Enemy.collideRectangle[index].Width/2, Enemy.collideRectangle[index].Height/2), SpriteEffects.None, 0f);
            
            _spriteBatch.Draw(Player.textures, new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), Player.textureRectangle, Color.White, Player.angle, new Vector2(Player.width / 2, Player.height / 2), SpriteEffects.None, 0f);
            
            _spriteBatch.Draw(blankTexture, new Rectangle(32, 32, (int)((float)(dashTimer/(float)(maxDashCharge * dashCooldown)) * 150), 32), null, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            for (int i = 1; i <= maxDashCharge; i++)
                _spriteBatch.Draw(blankTexture, new Rectangle(32 + i*(150 / maxDashCharge), 32, 5, 16), null, Color.Blue, 0, new Vector2(0, 0), SpriteEffects.None, 0f);
            


            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
