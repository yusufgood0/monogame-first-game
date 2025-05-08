using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
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
    
        public static bool OnKeyPress(Keys _key)
        {
            if (Game1.keyboardState.IsKeyDown(_key) && Game1.previousKeyboardState.IsKeyUp(_key))
            {
                return true;
            }
            return false;
        }
        public static bool OnRightButtonPress()
        {
            if (Game1.mouseState.RightButton == ButtonState.Pressed && Game1.previousMouseState.RightButton != ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        public static bool OnLeftButtonPress()
        {
            if (Game1.mouseState.LeftButton == ButtonState.Pressed && Game1.previousMouseState.LeftButton != ButtonState.Pressed)
            {
                return true;
            }
            return false;
        }
        public static float? GetSlope(Vector2 point, Vector2 point2)
        {
            if (point2.X - point.X == 0)
                return null; // vertical line
            return (point2.Y - point.Y) / (point2.X - point.X);
        }

        public static float GetYIntercept(Vector2 point, float? slope)
        {
            return point.Y - (slope.Value * point.X);
        }

        public static Vector2 lineIntercepts(Vector2 line1Point1, Vector2 line1Point2, Vector2 line2Point1, Vector2 line2Point2)
        {
            float? m1 = GetSlope(line1Point1, line1Point2);
            float? m2 = GetSlope(line2Point1, line2Point2);

            if (m1 == m2)
                return new Vector2(float.PositiveInfinity, float.PositiveInfinity);

            else if (m1 == null) // Line 1 is vertical
            {
                return new Vector2(line1Point1.X, (float)(m2 * line1Point1.X + GetYIntercept(line2Point1, m2)));
            }
            else if (m2 == null) // Line 2 is vertical
            {
                return new Vector2(line2Point1.X, (float)(m1 * line2Point1.X + GetYIntercept(line1Point1, m1)));
            }
            else
            {
                float b1 = GetYIntercept(line1Point1, m1.Value);
                float b2 = GetYIntercept(line2Point1, m2.Value);

                float x = (b2 - b1) / (m1.Value - m2.Value);
                float y = m2.Value * x + b2;
                return new Vector2(x, y);
            }
        }

        public static float PointToRectCollision(Vector2 linePoint, Vector2 linePoint2, Rectangle rect)
        {
            Vector2[] intercepts = new Vector2[4];
            intercepts[0] = lineIntercepts(linePoint, linePoint2, new Vector2(rect.X, rect.Y), new Vector2(rect.X + rect.Width, rect.Y));                               //horizontal Top
            intercepts[1] = lineIntercepts(linePoint, linePoint2, new Vector2(rect.X, rect.Y + rect.Height), new Vector2(rect.X + rect.Width, rect.Y + rect.Height));   //horizontal Bottom
            intercepts[2] = lineIntercepts(linePoint, linePoint2, new Vector2(rect.X, rect.Y), new Vector2(rect.X, rect.Y + rect.Height));                              // vertical Left
            intercepts[3] = lineIntercepts(linePoint, linePoint2, new Vector2(rect.X + rect.Width, rect.Y), new Vector2(rect.X + rect.Width, rect.Y + rect.Height));    // vertical Right

            bool DirectionIsRight = false;

            if (linePoint.X - linePoint2.X > 0)
            {
                DirectionIsRight = true;
            }
            else
            {
                DirectionIsRight = false;
            }


            float lowestDistance = float.PositiveInfinity;
            float distance;
            for (int i = 0; i < 4; i++)
            {
                if (
                    intercepts[i].X >= Math.Min(rect.Left, rect.Right) && intercepts[i].X <= Math.Max(rect.Left, rect.Right) && // x in bounds check
                    intercepts[i].Y >= Math.Min(rect.Top, rect.Bottom) && intercepts[i].Y <= Math.Max(rect.Top, rect.Bottom) && // y in bounds check
                    ((intercepts[i].X < linePoint.X && DirectionIsRight) || (intercepts[i].X > linePoint.X && !DirectionIsRight)) // correct direction check
                    )
                {
                    distance = DistanceFromPoints(linePoint, intercepts[i]);
                    if (distance < lowestDistance)
                    {
                        lowestDistance = distance;
                        switch (i)
                        {
                            case 0: // Top
                                Game1._texturePercent = (rect.Right - intercepts[i].X) / Tiles.tileXY;
                                break;
                            case 1: // Bottom
                                Game1._texturePercent = (intercepts[i].X - rect.Left) / Tiles.tileXY;
                                break;
                            case 2: // Left
                                Game1._texturePercent = (intercepts[i].Y - rect.Top) / Tiles.tileXY;
                                break;
                            case 3: // Right
                                Game1._texturePercent = (rect.Bottom - intercepts[i].Y) / Tiles.tileXY;
                                break;
                        }
                    }
                }
            }

            return lowestDistance;
        }
        public static void DrawLine(Vector2 pointa, Vector2 pointb)
        {
            float? slope = GetSlope(pointa, pointb);
            if (!slope.HasValue) { return; }
            float Yintercept = GetYIntercept(pointa, slope.Value);


            bool Direction = false;

            if (pointa.X - pointb.X > 0)
            {
                Direction = true;
            }
            else
            {
                Direction = false;
            }

            for (int x = 0; x < Game1.screenSize.X; x++)
            {
                if ((x < pointa.X && Direction) || (x > pointa.X && !Direction))
                {
                    int y = (int)(slope * x + Yintercept);
                    Game1._spriteBatch.Draw(Game1.blankTexture, new Rectangle(x, y, 1, 1), Color.White);

                }
            }
        }
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
            return new Color(_color.R * _finalLightLevel, _color.G * _finalLightLevel, _color.B * _finalLightLevel, _color.A * _finalLightLevel);
        }
        public static float InboundAngle(float inputAngle)
        {
            while (inputAngle < -Math.PI)
            {
                inputAngle += (float)Math.PI;
            }
            while (inputAngle > Math.PI)
            {
                inputAngle -= (float)Math.PI;
            }
            return inputAngle;
        }
        public static float Vector2ToAngle(Vector2 angle)
        {
            return (float)Math.Atan2(angle.Y, angle.X);
        }
        public static Vector2 AngleToVector2(double angle)
        {
            Vector2 Vector = new Vector2((float)Math.Sin(angle), (float)Math.Cos(angle));
            Vector.Normalize();
            return Vector;
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

        public enum Direction
        {
            UP, DOWN, LEFT, RIGHT
        }
        public static void MoveKeyPressed(Direction _MoveKeys)
        {
            switch (_MoveKeys)
            {
                case Direction.UP:
                    Player.speed += Player.angleVector * Player.movementSpeed;
                    Player.frame = 1;
                    break;
                case Direction.DOWN:
                    Player.speed -= Player.angleVector * Player.movementSpeed;
                    Player.frame = 0;
                    break;
                case Direction.LEFT:
                    Player.speed += AngleToVector2(Player.angle - (float)Math.PI / 2) * Player.movementSpeed;
                    Player.frame = 2;
                    Player.effect = SpriteEffects.None;
                    break;
                case Direction.RIGHT:
                    Player.speed += AngleToVector2(Player.angle + (float)Math.PI / 2) * Player.movementSpeed;
                    Player.frame = 2;
                    Player.effect = SpriteEffects.FlipHorizontally;
                    break;
            }

            if (Player.Attacks.swingSpeed == -1)
            {
                switch (_MoveKeys)
                {
                    case Direction.UP:
                        Player.Attacks.swingAngle = 20;
                        break;
                    case Direction.DOWN:
                        Player.Attacks.swingAngle = 20 + (float)Math.PI;
                        break;
                    case Direction.LEFT:
                        Player.Attacks.swingAngle = 5 + (float)Math.PI * 2.5f;
                        break;
                    case Direction.RIGHT:
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
        public static object[] CircleCollision(Vector2 circlePosition, float collisionRadius, Rectangle rect)
        {
            if (rect.Contains(circlePosition))
            {
                return new object[] { true, RectangleToVector2(rect) };
            }
            if (rect.Intersects(Vector2toRectangle(circlePosition, (int)collisionRadius * 2, (int)collisionRadius * 2)))
            {
                Vector2 closestPoint = new Vector2(
MathHelper.Clamp(circlePosition.X, rect.X, rect.X + rect.Width),
MathHelper.Clamp(circlePosition.Y, rect.Y, rect.Y + rect.Height));

                return new object[] { collisionRadius > DistanceFromPoints(circlePosition, closestPoint), closestPoint };
            };
            return new object[] { false, new Vector2() };

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
                while (Tiles.tileType[i] != 0 && (bool)CircleCollision(position, radius, Tiles.collideRectangle[i])[0])
                {
                    Vector2 difference = Difference(position, (Vector2)CircleCollision(position, radius, Tiles.collideRectangle[i])[1]);
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

        public static Vector2 screenSize = new();

        private readonly GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;
        public static KeyboardState previousKeyboardState, keyboardState;
        public static MouseState mouseState, previousMouseState;
        static SpriteFont titleFont;

        Vector2 offset = new(0, 0);

        static Texture2D swordTexture;
        public static Texture2D blankTexture;

        public static int EnemyTargetTimer = 0;

        public static float playerLightEmit = Constants.maxPlayerLightEmit;
        public static float LightLevel = Constants.maxLightLevel;


        bool previousIsActive;

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

        readonly Vector2 bowBarSize = new Vector2(1000, 30);
        Color bowChargeBarColor;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _graphics.IsFullScreen = false;
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
                new Vector2(1, 15),
                Content.Load<Texture2D>("circle"),
                new Vector2(1, 1),
                new Vector2(1, 30),
                Content.Load<Texture2D>("circle"),
                new Vector2(1, 1),
                new Vector2(1, 50),
                Content.Load<Texture2D>("square"),
                new Vector2(1, 1),
                new Vector2(.5f, 30),
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

                if (previousIsActive)
                    Player.angle += (Mouse.GetState().X - screenSize.X / 2) / 100;
                if (IsActive)
                    Mouse.SetPosition((int)screenSize.X / 2, (int)screenSize.Y / 2);
                previousIsActive = IsActive;

                Player.angleVector = General.AngleToVector2(Player.angle);

                if (Player.iFrames > 0)
                {
                    Player.iFrames -= 1;
                }

                if (OnKeyPress(Keys.Tab))
                {
                    Levels.SetLevel(Levels.Level + 1);
                    Enemy.RandomizePositions();
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                Player.speed = new Vector2(0, 0);
                if (dashLengthTimer < 0)
                {
                    if (keyboardState.IsKeyDown(Keys.W))
                    {
                        MoveKeyPressed(Direction.UP);

                    }
                    if (keyboardState.IsKeyDown(Keys.S))
                    {
                        MoveKeyPressed(Direction.DOWN);

                    }
                    if (keyboardState.IsKeyDown(Keys.A))
                    {
                        MoveKeyPressed(Direction.LEFT);

                    }
                    if (keyboardState.IsKeyDown(Keys.D))
                    {
                        MoveKeyPressed(Direction.RIGHT);
                    }
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
                //if (Player.health <= 0)
                //{
                //    state = State.Dead;
                //}
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


                    if (Player.Attacks.swingSpeed == -1)
                    {

                        if (OnRightButtonPress() && (Player.state == Player.State.Idle || Player.state == Player.State.Attacking_2) && dashCooldownTimer >= 200)
                        {
                            Player.Attacks.SwingStart(1, 0.3f, 25f, 15, 300, 1, 20);
                            Player.state = Player.State.Attacking_1;
                            dashCooldownTimer -= 350;
                        }
                        else if (OnLeftButtonPress() && (Player.state == Player.State.Idle || Player.state == Player.State.Attacking_1) && dashCooldownTimer >= 300)
                        {
                            Player.Attacks.SwingStart(-1, 0.3f, 25f, 15, 750, 1, 20);
                            Player.state = Player.State.Attacking_2;
                            dashCooldownTimer -= 350;
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
                        Player.TakeDamage(Color.Black, Constants.EnemyStats.damage[(int)Enemy.enemyType[_index]], 10, 500, 30, Player.position - Enemy.position[_index]);
                    }

                if (playerLightEmit > 300)
                {
                    playerLightEmit -= Constants.maxPlayerLightEmit / 200;
                }
                if (LightLevel > 10)
                {
                    LightLevel -= 0.2f;
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

                General.CircleMovement(true, ref Player.position, Player.width / 2, ref Player.speed, Player.movementSpeed);
                //General.CircleMovement(true, ref Player.position, Player.width/2, ref Player.speed, Player.movementSpeed);

                previousKeyboardState = Keyboard.GetState();
                previousMouseState = Mouse.GetState();
                gametimer -= Constants.tpsPerSec;
                if (gametimer > 1000)
                {
                    gametimer = 0;
                }

            }
            // TODO: Add your update logic here

            base.Update(gameTime);
        }
        public static float detail = 0.0069f;
        public static float FOV_Size = (float)Math.PI / 3;
        public static int segmentWidth;
        public static float lowestDistance = float.PositiveInfinity;
        public static float _texturePercent;
        public static float texturePercent;

        public static void CastRay(float angle)
        {
            segmentWidth = (int)(Game1.screenSize.X / (FOV_Size * 2));
            lowestDistance = float.PositiveInfinity;
            int tileIndex = 0;

            for (int _tileIndex = 0; _tileIndex < Tiles.numTiles; _tileIndex++)
            {
                if (Tiles.tileType[_tileIndex] != (int)Tiles.tileTypes.NONE)
                {
                    float Distance = .015f * PointToRectCollision(Player.position, Player.position + AngleToVector2(Player.angle + angle), Tiles.collideRectangle[_tileIndex]);
                    if (Distance < lowestDistance)
                    {
                        lowestDistance = Distance;
                        tileIndex = _tileIndex;
                        texturePercent = _texturePercent;
                    }
                }
            }
            //anti-fisheye
            //LowestDistance = LowestDistance * (float)Math.Cos(segment);

            if (lowestDistance != float.PositiveInfinity)
            {
                int columnHeight = (int)(screenSize.Y / lowestDistance);
                Color color = ColorFilter(Color.White, lowestDistance * 10);
                if (color != Color.Black)
                {
                    _spriteBatch.Draw(
                Tiles.textures[tileType[tileIndex]],
                new Rectangle(
                    (int)(screenSize.X / 2 + angle * segmentWidth),
                    (int)((screenSize.Y / 2) - columnHeight / 2),
                    (int)(segmentWidth * detail) + 1,
                    columnHeight),
                new Rectangle(
                    Tiles.textureRectangle[tileIndex].X + (int)(Tiles.textureRectangle[tileIndex].Width * Game1.texturePercent),
                    Tiles.textureRectangle[tileIndex].Y,
                    (int)((segmentWidth * detail) / Tiles.textureRectangle[tileIndex].Width),
                    Tiles.textureRectangle[tileIndex].Height),
                //new Color(colorFilter, colorFilter, colorFilter),
                color,
                0f,
                new(),
                0,
                0.1f / lowestDistance
                );
                }
                //_spriteBatch.DrawString(titleFont, LowestDistance.ToString(), new Vector2(0, 0), Color.Wheat);
                return;
            }
        }
        public static Color ColorFilter(Color _color, float distance)
        {
            return new Color((_color.R - 255f / LightLevel * distance) / 255, (_color.G - 255f / LightLevel * distance) / 255, (_color.B - 255f / LightLevel * distance) / 255);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            offset = screenSize/2 - Player.position;
            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.FrontToBack);
            //_spriteBatch.Draw(
            //    blankTexture,
            //new Rectangle(
            //    (int)screenSize.X / 2 - 50,
            //    (int)screenSize.Y / 2 - 50,
            //    100,
            //    100),
            //new Rectangle(0, 0, 1, 1),
            //Color.White,
            //0f,
            //new(),`
            //0,
            //1 / 20f
            //);

            //_spriteBatch.Draw(
            //        blankTexture,
            //        new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y),
            //        Color.Black
            //        );

            //if (Player.Attacks.swingSpeed != 0)
            //{
            //    _spriteBatch.Draw(
            //        swordTexture,
            //        offset + Player.position + new Vector2((float)Math.Cos(Player.Attacks.swingAngle),
            //        (float)Math.Sin(Player.Attacks.swingAngle)),
            //        null,
            //        Player.colorFilter,
            //        (float)(Player.Attacks.swingAngle),
            //        new Vector2(swordTexture.Width / 2, 0),
            //        0.05f,
            //        SpriteEffects.FlipVertically,
            //        1);
            //}
            //for (int index = 0; index < Tiles.numTiles; index++)
            //{
            //    _spriteBatch.Draw(Tiles.textures[Tiles.tileType[index]],
            //        new Rectangle(Tiles.collideRectangle[index].X + (int)offset.X, Tiles.collideRectangle[index].Y + (int)offset.Y,
            //        Tiles.tileXY,
            //        Tiles.tileXY),
            //        Tiles.textureRectangle[index],
            //        Darkness(Color.White, new Vector2(Tiles.collideRectangle[index].X + Tiles.collideRectangle[index].Width / 2, Tiles.collideRectangle[index].Y + +Tiles.collideRectangle[index].Height / 2)),
            //        0,
            //        new Vector2(0, 0),
            //        0f,
            //        .98f);
            //}
            //for (int index = 0; index < Enemy.health.Count; index++)
            //{
            //    _spriteBatch.Draw(Enemy.textures[(int)Enemy.enemyType[index]],
            //        new Rectangle(Enemy.collideRectangle[index].X + (int)offset.X, Enemy.collideRectangle[index].Y + (int)offset.Y, Enemy.collideRectangle[index].Width, Enemy.collideRectangle[index].Height),
            //        Enemy.textureRectangle[index],
            //        Darkness(Enemy.colorFilter[index], Enemy.position[index]),
            //        0,
            //        new Vector2(0, 0),
            //        0f,
            //        0.98f);
            //}

            //_spriteBatch.Draw(
            //    blankTexture,
            //    Vector2toRectangle(screenSize/2 + new Vector2(Player.width/2, Player.height/2), Player.width, Player.height),
            //    Player.textureRectangle,
            //    //new (0,0, 1, 1),
            //    Player.colorFilter,
            //    -Player.angle,
            //    //new(),
            //    new Vector2(Player.width / 4, Player.height / 4),
            //    Player.effect,
            //    .99f
            //    );

            for (int i = 0; i < (int)(2 * FOV_Size / detail); i++)
            {
                CastRay(-FOV_Size + i * detail);
            }
            for (int i = 0; i < Enemy.health.Count; i++)
            {
                Vector2 difference = Difference(Enemy.position[i], Player.position);
                float distance = General.DistanceFromDifference(difference);

                //int width = (int)(Enemy.collideRectangle[i].Width*10 - distance / Enemy.collideRectangle[i].Width * 10);
                //int height = (int)(Enemy.collideRectangle[i].Height*10 - distance / Enemy.collideRectangle[i].Height * 10);
                int height = (int)((Enemy.visualTextureSize[(int)Enemy.enemyType[i]].Y * screenSize.Y / (distance)));
                int width = (int)(Enemy.visualTextureSize[(int)Enemy.enemyType[i]].X * height);
                // Step 1: Get angle to enemy relative to player's forward direction
                float relativeAngle = -Vector2ToAngle(Player.angleVector) + Vector2ToAngle(difference);

                // Step 2: Normalize to range [0, 1] based on full field of view (2π radians)
                float normalizedAngle = relativeAngle;
                _spriteBatch.Draw(
                    Enemy.textures[(int)Enemy.enemyType[i]],
                    new Rectangle(
                        (int)((-relativeAngle * screenSize.X) / (FOV_Size * 2) + (screenSize.X / 2)) - width/2,
                        //(int)(enemyScreenX - width / 2),
                        //(int)(-((-screenSize.X + width) / 2) + (screenSize.X * (InboundAngle( (float)Math.PI + Player.angle - Vector2ToAngle(difference)) / Math.PI * 2))),
                        (int)(screenSize.Y/2 - height + (screenSize.Y / distance*25)),
                        width,
                        height
                        ),
                null,
                    ColorFilter(Enemy.colorFilter[i], distance/20f),
                    0f,
                    new Vector2(0, 0),
                    SpriteEffects.None,
                    0.1f/(distance / 75)
                    );
            }
            _spriteBatch.Draw(
                            blankTexture,
                            new Rectangle((int)(screenSize.X / 2 - bowBarSize.X / 2), (int)(screenSize.Y * .9f), (int)((float)(bowCharge / MaxBowCharge) * bowBarSize.X), (int)bowBarSize.Y),
                            null,
                            bowChargeBarColor,
                            0,
                            new Vector2(0, 0), 0f, 0.3f
                            );

            _spriteBatch.Draw(
                            blankTexture,
                            new Rectangle(0, 0, (int)((float)(dashCooldownTimer / (float)(maxDashCharge * dashCooldown)) * 300), 50),
                            null,
                            Color.White,
                            0,
                            new Vector2(0, 0),
                            0f,
                            1f
                            );

            for (int i = 0; i < maxDashCharge + 1; i++)
            {
                _spriteBatch.Draw(
                    blankTexture,
                    new Rectangle(i * (300 / maxDashCharge), 0, 5, 50),
                    null,
                    Color.Blue,
                    0,
                    new Vector2(0, 0),
                    0f,
                    1f
                    );
            }
            //DrawLine(Player.position + offset, Player.position + offset + Player.angleVector);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
