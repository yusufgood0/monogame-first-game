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
using static first_game.General;
using static first_game.Player;
using static first_game.Projectile;
using static first_game.Tiles;

namespace first_game
{
    public class General
    {

        public static Color Darkness(Color _color, Vector2 _position)
        {
            //if (!Enemy.SightLine(_position, DistanceFromPoints(_position, Player.position)))
            //{
            //    return Color.Black;
            //}

            List<Vector2> _points = new();
            List<float> _luminanceLevels = new();
            float _finalLuminance = 0;
            float _finalLightLevel;

            _points.Add(Player.position);
            _luminanceLevels.Add(Game1.playerLightEmit);

            foreach (Vector2 _projectilePosition in Projectile.position)
            {
                _points.Add(_projectilePosition);
                _luminanceLevels.Add(Constants.Luminance.Projectile);
            }

            for (int i = 0; i < Enemy.position.Count; i++)
            {
                _points.Add(Enemy.position[i]);
                _luminanceLevels.Add(Constants.Luminance.Enemy * Enemy.health[i]);
            }

            for (int i = 0; i < numTiles; i++)
            {
                if (Tiles.tileType[i] == (int)Tiles.tileTypes.GATE)
                {
                    _points.Add(RectangleToVector2(Tiles.collideRectangle[i]));
                    _luminanceLevels.Add(Constants.Luminance.LevelEnd);
                }
            }

            for (int i = 0; i < _points.Count; i++)
            {
                float _distanceCheck = DistanceFromPoints(_position, _points[i]);
                if (_distanceCheck == 0)
                {
                    _distanceCheck = 1;
                }
                if (_finalLuminance < _luminanceLevels[i] / _distanceCheck)
                {
                    _finalLuminance = _luminanceLevels[i] / _distanceCheck;
                }
            }

            _finalLightLevel = _finalLuminance - Constants.LightStrength;
            return new Color(_color.R * _finalLightLevel, _color.G * _finalLightLevel, _color.B * _finalLightLevel);
        }

        public static Rectangle RectangleAddVector2(Rectangle Rect, Vector2 vector)
        {
            return new Rectangle((int)(Rect.X + vector.X), (int)(Rect.Y + vector.Y), Rect.Width, Rect.Height);
        }
        public static Rectangle Vector2toRectangle(Vector2 Position, int _width, int _height)
        {
            return new((int)(Position.X - _width / 2), (int)(Position.Y - _height / 2), _width, _height);
        }
        public static Vector2 RectangleToVector2(Rectangle _rectangle)
        {
            return new Vector2(_rectangle.X + _rectangle.Width / 2, _rectangle.Y + _rectangle.Height / 2);
        }
        public static int TileFromVector2(Vector2 _position)
        {
            for (int _tileIndex = 0; _tileIndex < Tiles.numTiles; _tileIndex++)
            {
                if (collideRectangle[_tileIndex].Contains(_position))
                    return _tileIndex;
            }
            return 0;

        }
        public static float DistanceFromPoints(Vector2 _pointA, Vector2 _pointB)
        {
            Vector2 _difference = Difference(_pointA, _pointB);
            if (_difference == new Vector2(0, 0))
                return 0;
            return (float)Math.Sqrt(_difference.X * _difference.X + _difference.Y * _difference.Y); //X and Y difference 
        }
        public static float DistanceFromDifference(Vector2 _difference)
        {
            return (float)Math.Sqrt(_difference.X * _difference.X + _difference.Y * _difference.Y);
        }
        public static Vector2 Difference(Vector2 _pointA, Vector2 _pointB)
        {
            return _pointA - _pointB; //X and Y difference 
        }

        public enum MoveKeys
        {
            UP, DOWN, LEFT, RIGHT
        }
        public static void MoveKeyPressed(MoveKeys _MoveKeys)
        {
            switch (_MoveKeys)
            {
                case MoveKeys.UP:
                    Player.speed.Y -= Player.movementSpeed;
                    Player.frame = 1;
                    break;
                case MoveKeys.DOWN:
                    Player.speed.Y += Player.movementSpeed;
                    Player.frame = 0;
                    break;
                case MoveKeys.LEFT:
                    Player.speed.X -= Player.movementSpeed;
                    Player.frame = 2;
                    Player.effect = SpriteEffects.None;
                    break;
                case MoveKeys.RIGHT:
                    Player.speed.X += Player.movementSpeed;
                    Player.frame = 2;
                    Player.effect = SpriteEffects.FlipHorizontally;
                    break;
            }

            if (Player.Attacks.swingSpeed == -1)
            {
                switch (_MoveKeys)
                {
                    case MoveKeys.UP:
                        Player.Attacks.swingAngle = 20;
                        break;
                    case MoveKeys.DOWN:
                        Player.Attacks.swingAngle = 20 + (float)Math.PI;
                        break;
                    case MoveKeys.LEFT:
                        Player.Attacks.swingAngle = 5 + (float)Math.PI * 2.5f;
                        break;
                    case MoveKeys.RIGHT:
                        Player.Attacks.swingAngle = 20 + (float)Math.PI * 2.5f;
                        break;
                }
            }
            UpdateTexture();
        }
        public static void CollisionY(ref Vector2 position, Vector2 collisionSize, Vector2 speed)
        {
            for (int index = 0; index < Tiles.numTiles; index++)
                if (new Rectangle((int)(position.X - collisionSize.X / 2), (int)(position.Y - collisionSize.Y / 2), (int)collisionSize.X, (int)collisionSize.Y).Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {
                    if (speed.Y < 0)
                    {
                        position.Y = Tiles.collideRectangle[index].Y + Tiles.collideRectangle[index].Height + collisionSize.Y / 2;
                    }
                    else
                    {
                        position.Y = Tiles.collideRectangle[index].Y - collisionSize.Y / 2;
                    }
                    return;
                }
            return;
        }
        public static void CollisionX(ref Vector2 position, Vector2 collisionSize, Vector2 speed)
        {
            for (int index = 0; index < Tiles.numTiles; index++)
                if (new Rectangle((int)(position.X - collisionSize.X / 2), (int)(position.Y - collisionSize.Y / 2), (int)collisionSize.X, (int)collisionSize.Y).Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {
                    if (speed.X < 0)
                    {
                        position.X = Tiles.collideRectangle[index].X + Tiles.collideRectangle[index].Width + collisionSize.X / 2;
                    }
                    else
                    {
                        position.X = Tiles.collideRectangle[index].X - collisionSize.X / 2;
                    }
                    return; // it has collided with smth
                }
            return;
        }
        public static bool CircleCollision(Vector2 circlePosition, float collisionRadius, Rectangle rect)
        {
            if (rect.Intersects(Vector2toRectangle(circlePosition, (int)collisionRadius * 2, (int)collisionRadius * 2)))
            {
                return collisionRadius > DistanceFromPoints(
                    circlePosition,
                    new Vector2(
                        MathHelper.Clamp(circlePosition.X, rect.X, rect.X + rect.Width),
                        MathHelper.Clamp(circlePosition.Y, rect.Y, rect.Y + rect.Height))
                    );
            }
            return false;
        }

        public static void MoveX(bool normalize, Vector2 speed, ref Vector2 position)
        {
            if (normalize && speed != new Vector2(0, 0))
                speed.Normalize();
            if (normalize)
                position.X += speed.X * movementSpeed;
            else
                position.X += speed.X;
        }
        public static void MoveY(bool normalize, Vector2 speed, ref Vector2 position)
        {
            if (normalize && speed != new Vector2(0, 0))
                speed.Normalize();
            if (normalize)
                position.Y += speed.Y * movementSpeed;
            else
                position.Y += speed.Y;
        }
        public static void CircleMovement(bool normalize, ref Vector2 position, float radius, ref Vector2 speed, float movementSpeed)
        {
            MoveX(normalize, speed, ref position);
            MoveY(normalize, speed, ref position);
            for (int i = 0; i < Tiles.numTiles; i++)
            {
                while (Tiles.tileType[i] != 0 && CircleCollision(position, radius, Tiles.collideRectangle[i]))
                {
                    Vector2 difference = Difference(position, General.RectangleToVector2(Tiles.collideRectangle[i]));
                    difference.Normalize();
                    position += difference;
                }
            }

        }
        public static void RectMovement(bool normalize, ref Vector2 position, Vector2 collisionSize, ref Vector2 speed, float movementSpeed)
        {
            MoveX(normalize, speed, ref position);
            CollisionX(ref position, collisionSize, speed);

            MoveY(normalize, speed, ref position);
            CollisionY(ref position, collisionSize, speed);
        }
    }


    public class Game1 : Game
    {

        Vector2 screenSize = new(Tiles.rows * Tiles.tileXY, Tiles.columns * Tiles.tileXY);

        private readonly GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;
        KeyboardState movementKeyboardState;
        KeyboardState previousKeyboardState;
        KeyboardState keyboardState;
        MouseState mouseState;
        static SpriteFont titleFont;

        Vector2 offset = new(0, 0);

        Texture2D swordTexture;
        Texture2D blankTexture;

        public static int EnemyTargetTimer = 0;

        public static float playerLightEmit = Constants.maxPlayerLightEmit;

        int gametimer;
        readonly static int maxDashCharge = 2;
        readonly static int dashCooldown = 500; //how long a dash cooldown is in ms
        readonly int dashLength = 100; //how long a dash is in ms
        readonly float dashSpeed = 10;
        int dashLengthTimer = 0;
        int dashCooldownTimer = dashCooldown * maxDashCharge;
        int timeElapsed;

        readonly float BowRechargeRate = 1f;
        readonly int MinBowCharge = 25;
        readonly int MaxBowCharge = 100;
        float bowCharge = 0;

        readonly int bowBarSize = 50;
        Color bowChargeBarColor;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            screenSize = new Vector2(GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            _graphics.PreferredBackBufferWidth = (int)screenSize.X; // Sets the width of the window
            _graphics.PreferredBackBufferHeight = (int)screenSize.Y; // Sets the height of the window
            _graphics.ApplyChanges(); // Applies the new dimensions

            titleFont = Content.Load<SpriteFont>("titleFont");
            Window.Title = "Adding Things";

            swordTexture = Content.Load<Texture2D>("swordNoBg");
            blankTexture = new Texture2D(GraphicsDevice, 1, 1);
            blankTexture.SetData(new[] { Color.White }); // Fills the texture with color

            Enemy.Setup(new object[] {
                Content.Load<Texture2D>("circle"),
                new Vector2(1, 1),
                Content.Load<Texture2D>("circle"),
                new Vector2(1, 1),
                Content.Load<Texture2D>("circle"),
                new Vector2(1, 1),
                Content.Load<Texture2D>("square"),
                new Vector2(1, 1),
            });
            Player.Setup(Content.Load<Texture2D>("Player"));
            Tiles.setup(new object[] {
                Content.Load<Texture2D>("dirt"),
                new Vector2(4, 4),
                Content.Load<Texture2D>("tiles"),
                new Vector2(4, 4),
                Content.Load<Texture2D>("Brickwall6_Texture"),
                new Vector2(1, 1),
                Content.Load<Texture2D>("PurpleTile"),
                new Vector2(4, 4),
            });
            Portal.Setup(Content.Load<Texture2D>("Portal"));


            Constants.EnemyStats.Setup();
            Portal.ReloadPortalPosition();

            Levels.SetLevel(Levels.Level);
            Enemy.RandomizePositions();



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
            Game1.EnemyTargetTimer -= gameTime.ElapsedGameTime.Milliseconds;


            while (gametimer > Constants.tpsPerSec)
            {
                mouseState = Mouse.GetState();
                keyboardState = Keyboard.GetState();

                Player.angleVector = new Vector2(mouseState.X - screenSize.X / 2, mouseState.Y - screenSize.Y / 2);
                Player.angle = (float)Math.Atan2(angleVector.Y, angleVector.X);
                Player.angleVector.Normalize();

                if (Player.iFrames > 0)
                {
                    Player.iFrames -= 1;
                }

                if (keyboardState.IsKeyDown(Keys.Tab) && !previousKeyboardState.IsKeyDown(Keys.Tab))
                {
                    Levels.SetLevel(Levels.Level + 1);
                    Enemy.RandomizePositions();
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                Player.speed = new Vector2(0, 0);

                if (movementKeyboardState.IsKeyDown(Keys.W))
                {
                    MoveKeyPressed(MoveKeys.UP);

                }
                if (movementKeyboardState.IsKeyDown(Keys.S))
                {
                    MoveKeyPressed(MoveKeys.DOWN);

                }
                if (movementKeyboardState.IsKeyDown(Keys.A))
                {
                    MoveKeyPressed(MoveKeys.LEFT);

                }
                if (movementKeyboardState.IsKeyDown(Keys.D))
                {
                    MoveKeyPressed(MoveKeys.RIGHT);
                }

                if (previousKeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    Player.state = Player.State.Drawing_Bow;
                    if (bowCharge < MaxBowCharge) bowCharge += BowRechargeRate;
                    if (bowCharge >= MinBowCharge)
                    {
                        bowChargeBarColor = Color.Red;
                        if (!keyboardState.IsKeyDown(Keys.LeftControl))
                            Projectile.create(
                                projectileType.PLAYER_PROJECTILE,
                                Player.position,
                                Player.angleVector,
                                5 + (int)bowCharge / 10,
                                2,
                                5,
                                1 + (int)(bowCharge / MaxBowCharge),
                                (int)bowCharge / 2);
                    }
                }
                else
                {
                    bowChargeBarColor = Color.Blue;
                    bowCharge = 0;
                }
                if (Player.health <= 0)
                {
                    state = State.Dead;
                }
                switch (Player.state)
                {
                    case State.Idle:
                    Player.colorFilter = Color.White; 
                    break;
                    case State.Drawing_Bow:
                    Player.colorFilter = Color.AliceBlue;
                    break;
                    case State.Attacking_1:
                    Player.colorFilter = new Color(200, 200, 200);
                    break;
                    case State.Attacking_2:
                    Player.colorFilter = new Color(150, 150, 150);
                    break;
                    case State.Attacking_3:
                    Player.colorFilter = new Color(100, 100, 100);
                    break;
                    case State.Stunned:
                    Player.colorFilter = Color.DarkSlateGray;
                    break;
                    case State.Dashing:
                    Player.colorFilter = Color.WhiteSmoke * 0.5f;
                    break;
                    case State.Dead:
                    Player.colorFilter = Color.Black;
                    break;

                }

                if (dashLengthTimer < 0)
                {
                    if (Player.state == Player.State.Drawing_Bow) Player.movementSpeed = 2;
                    else Player.movementSpeed = 3;

                    movementKeyboardState = Keyboard.GetState();

                    if (keyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
                    {

                        if (Player.state == Player.State.Idle && dashCooldownTimer >= 200)
                        {
                            Player.Attacks.Swing(1, 0.3f, 40f, 15, 300, 1, 20);
                            Player.state = Player.State.Attacking_1;
                            dashCooldownTimer = 200;
                        }
                        else if (Player.state == Player.State.Attacking_1 && dashCooldownTimer >= 300)
                        {
                            Player.Attacks.Swing(-1, 0.5f, 40f, 15, 750, 2, 20);
                            Player.state = Player.State.Attacking_2;
                            dashCooldownTimer = 300;
                        }
                        else if (Player.state == Player.State.Attacking_2 && dashCooldownTimer >= 400)
                        {
                            Player.Attacks.Swing(1, 0.7f, 40f, 20, 1000, 3, 20);
                            Player.state = Player.State.Attacking_3;
                            dashCooldownTimer = 400;
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
                    Player.movementSpeed = dashSpeed;
                    dashLengthTimer -= timeElapsed;
                    Player.frame += 3;
                }

                for (int _index = 0; _index < Enemy.health.Count; _index++)
                    //if (iFrames <= 0 && state != State.Dashing && Enemy.collideRectangle[_index].Intersects(new Rectangle((int)Player.position.X - Player.collisionSize / 2, (int)Player.position.Y - Player.collisionSize / 2, Player.collisionSize, Player.collisionSize)))
                    if (iFrames <= 0 && state != State.Dashing && Enemy.IsEnemyCollide(General.Vector2toRectangle(Player.position, width, height), _index))
                    {
                        Player.TakeDamage(Color.Black, Constants.EnemyStats.damage[(int)Enemy.type[_index]], 10, 500, 30, Player.position - Enemy.position[_index]);
                    }

                if (playerLightEmit > 0)
                {
                    playerLightEmit -= Constants.maxPlayerLightEmit / 2000;
                }

                if (Color.Black == Darkness(Color.White, RectangleToVector2(Tiles.collideRectangle[TileFromVector2(Player.position)])))
                {
                    Player.TakeDamage(
                        Color.White,
                        2,
                        0,
                        0,
                        0,
                        new()
                        );
                }

                Portal.update();

                Player.Attacks.SwingUpdate();

                for (int i = 0; i < Enemy.health.Count; i++)
                {
                    Enemy.Update(i);
                }
                while (Game1.EnemyTargetTimer <= 0)
                {
                    Game1.EnemyTargetTimer = 250;
                }

                for (int i = 0; i < Projectile.position.Count; i++) Projectile.update(i);

                General.RectMovement(true, ref Player.position, new Vector2(Player.width, Player.height), ref Player.speed, Player.movementSpeed);
                //General.CircleMovement(true, ref Player.position, Player.width/2, ref Player.speed, Player.movementSpeed);

                previousKeyboardState = Keyboard.GetState();
                gametimer -= Constants.tpsPerSec;
                if (gametimer > 1000)
                {
                    gametimer = 0;
                }

            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);
            // TODO: Add your drawing code here
            _spriteBatch.Begin();
            offset += (-Player.position + screenSize / 2 - offset) / (Constants.tps * Constants.cameraLag);
            for (int index = 0; index < Tiles.numTiles; index++)
            {
                _spriteBatch.Draw(Tiles.textures[Tiles.tileType[index]],
                    new Rectangle(Tiles.collideRectangle[index].X + (int)offset.X, Tiles.collideRectangle[index].Y + (int)offset.Y,
                    Tiles.tileXY,
                    Tiles.tileXY),
                    Tiles.textureRectangle[index],
                    Darkness(Color.White, new Vector2(Tiles.collideRectangle[index].X + Tiles.collideRectangle[index].Width / 2, Tiles.collideRectangle[index].Y + +Tiles.collideRectangle[index].Height / 2)),
                    0,
                    new Vector2(0, 0),
                    0f,
                    0);
            }

            _spriteBatch.Draw(
                    Portal.texture,
                    RectangleAddVector2(Portal.collideRectangle, offset),
                    new Rectangle((int)Portal.textureFrame * Portal.texture.Width / Portal.amountOfFrames, 0, Portal.texture.Width/Portal.amountOfFrames, Portal.texture.Height),
                    Darkness(Portal.Color, RectangleToVector2(Portal.collideRectangle)),
                    0,
                    new Vector2(0, 0),
                    0f,
                    0.5f);

            for (int index = 0; index < Enemy.health.Count; index++)
            {
                _spriteBatch.Draw(Enemy.textures[(int)Enemy.type[index]],
                    new Rectangle(Enemy.collideRectangle[index].X + (int)offset.X, Enemy.collideRectangle[index].Y + (int)offset.Y, Enemy.collideRectangle[index].Width, Enemy.collideRectangle[index].Height),
                    Enemy.textureRectangle[index],
                    Darkness(Enemy.colorFilter[index], Enemy.position[index]),
                    0,
                    new Vector2(0, 0),
                    0f,
                    0.1f);
            }

            for (int index = 0; index < Projectile.position.Count; index++)
            {
                _spriteBatch.Draw(
                    blankTexture,
                    Vector2toRectangle(Projectile.position[index] + offset, 10, 10),
                    null,
                    Color.White,
                    0,
                    new Vector2(0, 0),
                    0f,
                    0.5f);

            }

            _spriteBatch.Draw(
                Player.textures,
                Vector2toRectangle(Player.position + offset, Player.width, Player.height),
                Player.textureRectangle,
                Player.colorFilter,
                0,
                new Vector2(0, 0),
                Player.effect,
                0.2f
                );


            _spriteBatch.Draw(
                blankTexture,
                new Rectangle((int)offset.X + (int)Player.position.X - bowBarSize / 2, (int)offset.Y + (int)Player.position.Y + Player.height / 2, (int)((float)(bowCharge / (float)MaxBowCharge) * bowBarSize), 8),
                null,
                bowChargeBarColor,
                0,
                new Vector2(0, 0), 0f, 0.3f
                );


            _spriteBatch.Draw(
                blankTexture,
                new Rectangle(32, 32, (int)((float)(dashCooldownTimer / (float)(maxDashCharge * dashCooldown)) * 150), 32),
                null,
                Color.White,
                0,
                new Vector2(0, 0),
                0f,
                0.3f
                );
            for (int i = 0; i < maxDashCharge + 1; i++)
            {
                _spriteBatch.Draw(
                    blankTexture,
                    new Rectangle(32 + i * (150 / maxDashCharge), 32, 5, 20),
                    null,
                    Color.Blue,
                    0,
                    new Vector2(0, 0),
                    0f,
                    0.4f
                    );
            }
            if (Player.Attacks.swingSpeed != 0)
            {
                _spriteBatch.Draw(
                    swordTexture,
                    offset + Player.position + new Vector2((float)Math.Cos(Player.Attacks.swingAngle),
                    (float)Math.Sin(Player.Attacks.swingAngle)),
                    null,
                    Player.colorFilter,
                    (float)(Player.Attacks.swingAngle - Math.PI * .5f),
                    new Vector2(swordTexture.Width / 2, 0),
                    0.05f,
                    SpriteEffects.FlipVertically,
                    1);
            }

            _spriteBatch.DrawString(titleFont, (Player.health / 10).ToString(), new Vector2(10, 10), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}