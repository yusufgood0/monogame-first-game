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
using static first_game.Player;
using static first_game.Projectile;
using static first_game.Tiles;

namespace first_game
{
    public class General
    {
        public static void collisionY(ref Vector2 position, int collisionSize, Vector2 speed, Rectangle colliderRectangle, float movementSpeed)
        {
            for (int index = 0; index < Tiles.numTiles; index++)
                if (new Rectangle((int)position.X - collisionSize / 2, (int)position.Y - collisionSize / 2, collisionSize, collisionSize).Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {
                    if (speed.Y < 0)
                    {
                        position.Y = Tiles.collideRectangle[index].Y + Tiles.collideRectangle[index].Height + collisionSize / 2;
                    }
                    else
                    {
                        position.Y = Tiles.collideRectangle[index].Y - collisionSize / 2;
                    }
                    //position.X += speed.Y * movementSpeed;
                }
        }
        public static void collisionX(ref Vector2 position, int collisionSize, Vector2 speed, Rectangle colliderRectangle, float movementSpeed)
        {
            for (int index = 0; index < Tiles.numTiles; index++)
                if (new Rectangle((int)position.X - collisionSize / 2, (int)position.Y - collisionSize / 2, collisionSize, collisionSize).Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {
                    if (speed.X < 0)
                    {
                        position.X = Tiles.collideRectangle[index].X + Tiles.collideRectangle[index].Width + collisionSize / 2;
                    }
                    else
                    {
                        position.X = Tiles.collideRectangle[index].X - collisionSize / 2;
                    }

                    //position.Y += speed.X * movementSpeed;
                }
        }



        public static void movement(ref Vector2 position, int collisionSize, ref Vector2 speed, float movementSpeed, Rectangle colliderRectangle)
        {
            if (speed != new Vector2(0, 0))
                speed.Normalize();

            position.X += speed.X * movementSpeed;
            collisionX(ref position, collisionSize, speed, colliderRectangle, movementSpeed);

            position.Y += speed.Y * movementSpeed;
            collisionY(ref position, collisionSize, speed, colliderRectangle, movementSpeed);
        }
    }


    public class Game1 : Game
    {


        private GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;
        KeyboardState movementKeyboardState;
        KeyboardState previousKeyboardState;
        KeyboardState keyboardState;
        MouseState mouseState;
        SpriteFont titleFont;


        Texture2D blankTexture;

        int gametimer;
        int tps = 30;
        readonly int maxDashCharge = 3;
        readonly int dashCooldown = 1500; //how long a dash cooldown is in ms
        readonly int dashLength = 100; //how long a dash is in ms
        int dashLengthTimer = 0;
        int dashCooldownTimer = 0;
        int timeElapsed; 

        readonly float BowRechargeRate = 1f;
        readonly int MinBowCharge = 25;
        readonly static int MaxBowCharge = 100;
        float bowCharge = MaxBowCharge;

        int bowBarSize = 50;
        Color bowChargeBar;

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
            Tiles.setup(Content.Load<Texture2D>("tiles"), Content.Load<Texture2D>("dirt"), Content.Load<Texture2D>("Brickwall6_Texture"));

            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.Red }); // Fills the texture with white
            for (int i = 0; i < 5; i++) Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2, 200), Enemy.EnemyType.SMALL);
            for (int i = 0; i < 3; i++) Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2 + 50, 300), Enemy.EnemyType.MEDIUM);
            for (int i = 0; i < 2; i++) Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2, 400), Enemy.EnemyType.LARGE);
            Tiles.regenerateTilemap();
            Enemy.respawn_enemies();

            _graphics.PreferredBackBufferWidth = Tiles.rows * Tiles.tileXY; // Sets the width of the window
            _graphics.PreferredBackBufferHeight = Tiles.columns * Tiles.tileXY; // Sets the height of the window
            _graphics.ApplyChanges(); // Applies the new dimensions

            titleFont = Content.Load<SpriteFont>("titleFont");
            Window.Title = "Adding Things";

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

                Player.angleVector = new Vector2(mouseState.X - Player.position.X, mouseState.Y - Player.position.Y);
                Player.angle = (float)Math.Atan2(angleVector.Y, angleVector.X);

                if (Player.iFrames > 0)
                {
                    Player.iFrames -= 1;
                }

                if (keyboardState.IsKeyDown(Keys.Tab) && !previousKeyboardState.IsKeyDown(Keys.Tab))
                {
                    Tiles.regenerateTilemap();
                    Enemy.respawn_enemies();
                }


                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                Player.speed = new Vector2(0, 0);

                if (movementKeyboardState.IsKeyDown(Keys.W))
                    Player.speed.Y -= Player.movementSpeed;
                if (movementKeyboardState.IsKeyDown(Keys.S))
                    Player.speed.Y += Player.movementSpeed;
                if (movementKeyboardState.IsKeyDown(Keys.A))
                    Player.speed.X -= Player.movementSpeed;
                if (movementKeyboardState.IsKeyDown(Keys.D))
                    Player.speed.X += Player.movementSpeed;




                if (previousKeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    Player.state = Player.State.Drawing_Bow;
                    if (bowCharge < MaxBowCharge) bowCharge += BowRechargeRate;
                    if (bowCharge >= MinBowCharge)
                    {
                        bowChargeBar = Color.White;
                        if (!keyboardState.IsKeyDown(Keys.LeftControl)) Projectile.create(projectileType.PLAYER_ARROW, Player.position, Player.angleVector, 10, 2, 5, 1 + (int)(bowCharge / MaxBowCharge), (int)bowCharge / 2);
                    }
                }
                else
                {
                    bowChargeBar = Color.Blue;
                    bowCharge = 0;
                }



                if (dashLengthTimer < 0)
                {
                    if (Player.state == Player.State.Drawing_Bow) Player.movementSpeed = 2;
                    else Player.movementSpeed = 3;

                    movementKeyboardState = Keyboard.GetState();

                    if (keyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
                    {
                        if (Player.state == Player.State.Idle)
                        {
                            Player.Attacks.Swing.swing(0.4f, 30f, 10, 300, 2, 20);
                            Player.state = Player.State.Attacking_1;
                        }
                        else if (Player.state == Player.State.Attacking_1)
                        {
                            Player.Attacks.Swing.swing(0.2f, 35f, 12, 750, 2, 20);
                            Player.state = Player.State.Attacking_2;
                        }
                        else if (Player.state == Player.State.Attacking_2)
                        {
                            Player.Attacks.Swing.swing(0.05f, 40f, 18, 1000, 2, 10);
                            Player.state = Player.State.Attacking_3;
                        }
                    }

                    if (Player.recoveryTime > 0)
                        Player.recoveryTime -= timeElapsed;
                    else
                        Player.state = Player.State.Idle;

                    if (dashCooldownTimer < (maxDashCharge * dashCooldown))
                        dashCooldownTimer += timeElapsed;

                    if (dashCooldownTimer > dashCooldown && keyboardState.IsKeyDown(Keys.LeftShift) && !previousKeyboardState.IsKeyDown(Keys.LeftShift) && Player.speed != new Vector2(0, 0))
                    {
                        dashCooldownTimer -= dashCooldown;
                        dashLengthTimer = dashLength;
                    }
                }
                else
                {
                    Player.state = Player.State.Dashing;
                    Player.movementSpeed = 15;
                    dashLengthTimer -= timeElapsed;
                }

                for (int _index = 0; _index < Enemy.collideRectangle.Count; _index++)
                    if (Enemy.collideRectangle[_index].Intersects(new Rectangle((int)Player.position.X - Player.collisionSize / 2, (int)Player.position.Y - Player.collisionSize / 2, Player.collisionSize, Player.collisionSize)))
                    {
                        Player.TakeDamage(Color.BlueViolet, Enemy.damage[_index], 10, 500, 30, Player.position - Enemy.position[_index]);
                    }




                Player.Attacks.Swing.swingUpdate();

                General.movement(ref Player.position, Player.collisionSize, ref Player.speed, Player.movementSpeed, new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height));
                for (int i = 0; i < Enemy.collideRectangle.Count; i++) Enemy.Step(i);
                for (int i = 0; i < Projectile.position.Count; i++) Projectile.update(i);


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

            for (int index = 0; index < Tiles.numTiles; index++) { _spriteBatch.Draw(Tiles.textures[Tiles.tileType[index]], Tiles.collideRectangle[index], Tiles.textureRectangle[index], Color.White, 0, new Vector2(0, 0), 0f, 0); }

            for (int index = 0; index < Projectile.position.Count; index++)
            {
                _spriteBatch.Draw(blankTexture, new Rectangle((int)Projectile.position[index].X - 5, (int)Projectile.position[index].Y - 5, 10, 10), null, Color.White, 0, new Vector2(0, 0), 0f, 0.5f);
                //_spriteBatch.DrawString(titleFont, Enemy.health[index].ToString(), Enemy.position[index], Color.Red);
            }

            for (int index = 0; index < Enemy.collideRectangle.Count; index++)
            {
                _spriteBatch.Draw(Enemy.textures, Enemy.collideRectangle[index], Enemy.textureRectangle[index], Color.White, 0, new Vector2(0, 0), 0f, 0.1f);
                _spriteBatch.DrawString(titleFont, Enemy.health[index].ToString(), Enemy.position[index], Color.Red);
            }


            _spriteBatch.Draw(Player.textures, new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), Player.textureRectangle, Color.White, Player.angle + (float)Math.PI / 2, new Vector2(Player.width / 2, Player.height / 2), 0f, 0.2f);


            _spriteBatch.Draw(blankTexture, new Rectangle((int)Player.position.X - bowBarSize / 2, (int)Player.position.Y + Player.height / 2, (int)((float)(bowCharge / (float)MaxBowCharge) * bowBarSize), 8), null, bowChargeBar, 0, new Vector2(0, 0), 0f, 0.3f);


            _spriteBatch.Draw(blankTexture, new Rectangle(32, 32, (int)((float)(dashCooldownTimer / (float)(maxDashCharge * dashCooldown)) * 150), 32), null, Color.White, 0, new Vector2(0, 0), 0f, 0.3f);
            for (int i = 0; i < maxDashCharge + 1; i++) { _spriteBatch.Draw(blankTexture, new Rectangle(32 + i * (150 / maxDashCharge), 32, 5, 20), null, Color.Blue, 0, new Vector2(0, 0), 0f, 0.4f); }
            if (Player.Attacks.Swing.attackAngle >= Player.Attacks.Swing.endAngle) { _spriteBatch.Draw(blankTexture, new Rectangle((int)Player.Attacks.Swing.checkpoint.X - 5, (int)Player.Attacks.Swing.checkpoint.Y - 5, 10, 10), null, Color.White, 0, new Vector2(0, 0), 0f, 0.5f); }

            _spriteBatch.DrawString(titleFont, (Player.health / 10).ToString(), new Vector2(10, 10), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
