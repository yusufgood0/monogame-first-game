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
using static first_game.Gems;
using System.Reflection.PortableExecutable;

namespace first_game
{
    public class General
    {
        public static Random rnd = new Random();
        public static Color ColorMultiply(Color color1, float multiplier)
        {
            return new Color((int)(color1.R * multiplier), (int)(color1.G * multiplier), (int)(color1.B * multiplier));
        }
        public static Color ColorFilter(Color _color, float distance)
        {
            return new Color((_color.R - 255f / Game1.LightLevel * distance) / 255, (_color.G - 255f / Game1.LightLevel * distance) / 255, (_color.B - 255f / Game1.LightLevel * distance) / 255);
        }
        public static float AngleDifference(float a, float b)
        {
            float diff = a - b;
            while (diff > MathF.PI) diff -= MathF.Tau;
            while (diff < -MathF.PI) diff += MathF.Tau;
            return diff;
        }
        public static void drawObject(SpriteBatch _spriteBatch, Texture2D texture, Rectangle? textureRect, Color color, Vector2 objectPosition, float objectY, float objectHeight, float objectwidthRatio)
        {
            Vector2 difference = Difference(objectPosition, Player.position);
            float distance = General.DistanceFromDifference(difference) / 75;
            int Height = (int)(objectHeight * Game1.screenSize.Y / (distance * 75));
            int width = (int)(Height / objectwidthRatio);
            _spriteBatch.Draw(
                texture,
                new Rectangle(
                    (int)((-AngleDifference(Vector2ToAngle(Player.angleVector), Vector2ToAngle(difference)) * Game1.screenSize.X) / (Game1.FOV * 2) + (Game1.screenSize.X / 2)) - width / 2,
                    (int)(Game1.screenSize.Y / 2 - Height + (Game1.screenSize.Y / distance) / 2 + (Game1.PlayerHeight - objectY) / distance),
                    width,
                    Height
                    ),
                textureRect,
                ColorFilter(color, distance / 25f),
                //color,
                0f,
                new Vector2(0, 0),
                SpriteEffects.None,
                0.1f / distance
                );
        }
        public static void Slider(MouseState _mouseState, MouseState _previousMouseState, ref float sliderValue, float minValue, float maxValue, Rectangle sliderSize)
        {
            if (sliderSize.Contains(_previousMouseState.Position) && _mouseState.LeftButton == ButtonState.Pressed)
            {
                sliderValue = Math.Max(Math.Min(((float)(_mouseState.X - sliderSize.X) / sliderSize.Width) * (maxValue - minValue) + minValue, maxValue), minValue);
            }
        }
        public static void SliderDraw(SpriteBatch _spritebatch, float sliderValue, float minValue, float maxValue, Rectangle sliderSize, Color backgroundColor, Color sliderColor, string text, float textScale, float visualSliderValueMultiplier)
        {
            string visualSliderValue = ((int)(sliderValue * visualSliderValueMultiplier)).ToString();
            _spritebatch.DrawString(
            Game1.titleFont,
            visualSliderValue,
            new(sliderSize.Right - Game1.titleFont.MeasureString(visualSliderValue).X * textScale, sliderSize.Y - Game1.titleFont.LineSpacing * textScale),
            backgroundColor,
            0,
            new(),
            textScale,
            0,
            .99f
            );

            _spritebatch.DrawString(
                Game1.titleFont,
                text,
                new(sliderSize.X, sliderSize.Y - Game1.titleFont.LineSpacing * textScale),
                backgroundColor,
                0,
                new(),
                textScale,
                0,
                .99f
                );
            _spritebatch.Draw(
                Game1.blankTexture,
                sliderSize,
                null,
                backgroundColor,
                0,
                new(),
                0,
                .98f
                );
            _spritebatch.Draw(
                Game1.blankTexture,
                new(sliderSize.X, sliderSize.Y, (int)(sliderSize.Width / (maxValue - minValue) * (sliderValue - minValue)), sliderSize.Height),
                null,
                sliderColor,
                0,
                new(),
                0,
                .99f
                );
        }
        public static void SliderDraw(SpriteBatch _spritebatch, float sliderValue, float minValue, float maxValue, Rectangle sliderSize, Color backgroundColor, Color sliderColor, string text, float textScale)
        {
            _spritebatch.DrawString(
                Game1.titleFont,
                text,
                new(sliderSize.X, sliderSize.Y - Game1.titleFont.LineSpacing * textScale),
                backgroundColor,
                0,
                new(),
                textScale,
                0,
                .91f
                );
            _spritebatch.Draw(
                Game1.blankTexture,
                sliderSize,
                null,
                backgroundColor,
                0,
                new(),
                0,
                .90f
                );
            _spritebatch.Draw(
                Game1.blankTexture,
                new(sliderSize.X, sliderSize.Y, (int)(sliderSize.Width / (maxValue - minValue) * (sliderValue - minValue)), sliderSize.Height),
                null,
                sliderColor,
                0,
                new(),
                0,
                .91f
                );
        }
        public static int NegativeOrPositive(float num)
        {
            if (num < 0)
            {
                return -1;
            }
            return 1;
        }
        public static bool OnKeyPress(Keys _key)
        {
            if (Game1.keyboardState.IsKeyDown(_key) && Game1.previousKeyboardState.IsKeyUp(_key))
            {
                return true;
            }
            return false;
        }
        public static bool OnKeyRelease(Keys _key)
        {
            if (Game1.keyboardState.IsKeyUp(_key) && Game1.previousKeyboardState.IsKeyDown(_key))
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
                //float y = (float)m2 * line1Point1.X + GetYIntercept(line2Point2, m2);
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

        public static float LineToRectCollision(Vector2 linePoint, Vector2 linePoint2, Rectangle rect)
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

            //bool DirectionIsDown = false;
            //if (linePoint.Y - linePoint2.Y > 0)
            //{
            //    DirectionIsDown = true;
            //}
            //else
            //{
            //    DirectionIsDown = false;
            //}

            float lowestDistance = float.PositiveInfinity;
            float distance;
            for (int i = 0; i < 4; i++)
            {
                if (
                    intercepts[i].X >= Math.Min(rect.Left, rect.Right) && intercepts[i].X <= Math.Max(rect.Left, rect.Right) // x in bounds check
                    && intercepts[i].Y >= Math.Min(rect.Top, rect.Bottom) && intercepts[i].Y <= Math.Max(rect.Top, rect.Bottom) // y in bounds check
                    && ((intercepts[i].X < linePoint.X && DirectionIsRight) || (intercepts[i].X > linePoint.X && !DirectionIsRight)) // correct direction check
                                                                                                                                     //&& ((intercepts[i].Y < linePoint.Y && DirectionIsDown) || (intercepts[i].Y > linePoint.Y && !DirectionIsDown))
                    )
                {
                    distance = DistanceFromPoints(linePoint, intercepts[i]);
                    if (distance <= lowestDistance)
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


            bool Direction;

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
            _luminanceLevels.Add(Constants.Luminance.Player);

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
            Vector2 Vector = new((float)Math.Cos(angle), (float)Math.Sin(angle));
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
            if (_pointA == _pointB)
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
            UP = Keys.W,
            DOWN = Keys.S,
            LEFT = Keys.A,
            RIGHT = Keys.D
        }
        public static void MoveKeyPressed(KeyboardState keyboardState)
        {
            Vector2 speedChange = new();

            if (keyboardState.IsKeyDown((Keys)Direction.UP))
            {
                speedChange += Player.angleVector;
                Player.frame = 1;
            }
            if (keyboardState.IsKeyDown((Keys)Direction.DOWN))
            {
                speedChange -= Player.angleVector;
                Player.frame = 0;
            }
            if (keyboardState.IsKeyDown((Keys)Direction.LEFT))
            {
                speedChange += AngleToVector2(Player.angle - (float)Math.PI / 2);
                Player.frame = 2;
                Player.effect = SpriteEffects.None;
            }
            if (keyboardState.IsKeyDown((Keys)Direction.RIGHT))
            {
                speedChange += AngleToVector2(Player.angle + (float)Math.PI / 2);
                Player.frame = 2;
                Player.effect = SpriteEffects.FlipHorizontally;
            }

            if (speedChange != new Vector2(0, 0))
            {
                speedChange.Normalize();
                Player.speed += speedChange * Player.movementSpeed;

            }

            //if (Player.Attacks.swingSpeed == -1)
            //{
            //    switch (_MoveKeys)
            //    {
            //        case Direction.UP:
            //            break;
            //        case Direction.DOWN:
            //            Player.Attacks.swingAngle = 20 + (float)Math.PI;
            //            break;
            //        case Direction.LEFT:
            //            Player.Attacks.swingAngle = 5 + (float)Math.PI * 2.5f;
            //            break;
            //        case Direction.RIGHT:
            //            Player.Attacks.swingAngle = 20 + (float)Math.PI * 2.5f;
            //            break;
            //    }
            //}
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
        }
        public static Vector2 nearestPoint(Vector2 position, Rectangle rect)
        {
            return new Vector2(
MathHelper.Clamp(position.X, rect.Left, rect.Right),
MathHelper.Clamp(position.Y, rect.Top, rect.Bottom));
        }
        public static bool CircleCollisionBool(Vector2 circlePosition, float collisionRadius, Rectangle rect)
        {
            if (rect.Contains(circlePosition))
            {
                return true;
            }
            if (rect.Intersects(Vector2toRectangle(circlePosition, (int)collisionRadius * 2, (int)collisionRadius * 2)))
            {
                Vector2 closestPoint = nearestPoint(circlePosition, rect);

                return collisionRadius > DistanceFromPoints(circlePosition, closestPoint);
            };
            return false;

        }
        public static Vector2 CircleCollisionPoint(Vector2 circlePosition, float collisionRadius, Rectangle rect)
        {
            if (rect.Contains(circlePosition))
            {
                return RectangleToVector2(rect); // returns centre of rect
            }
            if (rect.Intersects(Vector2toRectangle(circlePosition, (int)collisionRadius * 2, (int)collisionRadius * 2)))
            {
                return nearestPoint(circlePosition, rect); // returns the closest point


            };
            return new Vector2();

        }

        public static void MoveX(bool normalize, Vector2 speed, ref Vector2 position, float movementSpeed)
        {
            if (normalize && speed != new Vector2(0, 0))
            {
                speed.Normalize();
                position.X += speed.X * movementSpeed;
            }
            else
            {
                position.X += speed.X;
            }
        }
        public static void MoveY(bool normalize, Vector2 speed, ref Vector2 position, float movementSpeed)
        {
            if (normalize && speed != new Vector2(0, 0))
            {
                speed.Normalize();
                position.Y += speed.Y * movementSpeed;
            }
            else
            {
                position.Y += speed.Y;
            }
        }
        public static void CircleMovement(bool normalize, ref Vector2 position, float radius, ref Vector2 speed, float movementSpeed)
        {
            Vector2 direction = new Vector2(NegativeOrPositive(speed.X), NegativeOrPositive(speed.Y)) * radius;
            MoveX(normalize, speed, ref position, movementSpeed);
            for (int i = 0; i < Tiles.numTiles; i++)
            {
                if (Tiles.tileType[i] != 0 && Tiles.collideRectangle[i].Contains(position))
                {
                    position.X -= direction.X;
                }
            }
            MoveY(normalize, speed, ref position, movementSpeed);
            for (int i = 0; i < Tiles.numTiles; i++)
            {
                if (Tiles.tileType[i] != 0 && Tiles.collideRectangle[i].Contains(position))
                {
                    position.Y -= direction.Y;
                }
            }
            for (int i = 0; i < Tiles.numTiles; i++)
            {
                if (Tiles.tileType[i] != 0 && CircleCollisionBool(position, radius, Tiles.collideRectangle[i]))
                {
                    Vector2 difference = Difference(position, CircleCollisionPoint(position, radius, Tiles.collideRectangle[i]));
                    position -= difference - difference * (radius / MathF.Sqrt(difference.X * difference.X + difference.Y * difference.Y));
                }
            }
        }
        public static void RectMovement(bool normalize, ref Vector2 position, Vector2 collisionSize, ref Vector2 speed, float movementSpeed)
        {
            MoveX(normalize, speed, ref position, movementSpeed);
            CollisionX(ref position, collisionSize, speed);

            MoveY(normalize, speed, ref position, movementSpeed);
            CollisionY(ref position, collisionSize, speed);
        }


    }
    public class Game1 : Game
    {
        public static float jumpTime = Constants.jumpWidth;

        public static float detail = Constants.maxDetail * .7f;
        public static float FOV = Constants.maxFOV * .6f;
        public static int segmentWidth;
        public static float lowestDistance = float.PositiveInfinity;
        public static float _texturePercent;
        public static float texturePercent;
        public static float PlayerHeight = Constants.floorLevel;

        private enum GameState
        {
            Playing,
            paused
        }
        private static GameState gameState = GameState.Playing;

        private static float sensitivity = Constants.maxSensitivity;

        private static bool cheats = false;

        public static Vector2 screenSize = new();

        private readonly GraphicsDeviceManager _graphics;
        public static SpriteBatch _spriteBatch;
        public static KeyboardState previousKeyboardState, keyboardState;
        public static MouseState mouseState, previousMouseState;
        public static SpriteFont titleFont;

        Vector2 offset = new(0, 0);

        static Texture2D swordTexture;
        static readonly Texture2D[] punchTextures = new Texture2D[4];
        public static float punchFrame = 5;
        static Vector2 handRectSize;
        bool punchSideLeft;

        public static Texture2D crosshairTexture;
        public static Texture2D blankTexture;

        public static int EnemyTargetTimer = 0;
        public static int enemyAnimationTimer = 0;


        public static Color healthBarColor;
        public static Color screenFilter = new Color(0, 0, 0, 0);

        public static float playerLightEmit = Constants.maxPlayerLightEmit;
        public static float LightLevel = Constants.maxLightLevel;

        static int combo = 0;

        bool previousIsActive;

        int gametimer;
        readonly static float staminaRechargeMultiplier = .8f;
        readonly static int maxDashCharge = 5;
        readonly static int dashCost = 300; //how long a how much stamina a dash takes
        readonly int dashLength = 100; //how long a dash is in ms
        readonly float dashSpeed = 10;
        int dashLengthTimer = 0;
        float stamina = Constants.maxStamina;
        int timeElapsed;

        readonly float BowRechargeRate = 1f;
        readonly int MinBowCharge = 5;
        readonly int MaxBowCharge = 100;
        float bowCharge = 0;


        readonly Vector2 bowBarSize = new Vector2(1000, 30);
        Color bowChargeBarColor;



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = false;
            _graphics.IsFullScreen = true;
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

            for (int i = 1; i < 5; i++)
            {
                punchTextures[i - 1] = Content.Load<Texture2D>("PunchTexture/punchFrame" + i.ToString());
            }

            Enemy.Setup(new object[] {
                Content.Load<Texture2D>("smallEnemySpritesheet"),
                new Vector2(1, 1),
                new Vector2(1f, 40),
                Content.Load<Texture2D>("mediumEnemySpritesheet"),
                new Vector2(1, 1),
                new Vector2(1.25f, 55),
                Content.Load<Texture2D>("circle"),
                new Vector2(1, 1),
                new Vector2(1, 70),
                Content.Load<Texture2D>("square"),
                new Vector2(1, 1),
                new Vector2(2f, 60),
                Content.Load<Texture2D>("square"),
                new Vector2(1, 1),
                new Vector2(1.75f, 90),
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
            Portal.Setup(Content.Load<Texture2D>("transparentPortal"));


            Constants.EnemyStats.Setup();
            Portal.ReloadPortalPosition();

            Levels.SetLevel(Levels.Level);
            Enemy.RandomizePositions();

            handRectSize = new Vector2(punchTextures[0].Width * 2.5f, punchTextures[0].Height * 2.5f);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            Gems.gemTexture = Content.Load<Texture2D>("Gems");
            Gems.setup();

            Projectile.Setup(new object[] {
                Gems.TextureRect[11],
                Gems.TextureRect[0],
                Gems.TextureRect[9],
            });
            crosshairTexture = Content.Load<Texture2D>("Crosshair");

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Constants.healthSliderRect = new Rectangle((int)(Game1.screenSize.X * .04f), (int)(Game1.screenSize.Y * .05f), (int)(Game1.screenSize.X * .4f), (int)(Game1.screenSize.Y * .05f));
            Constants.staminaSliderRect = new Rectangle((int)(Game1.screenSize.X * .04f), (int)(Game1.screenSize.Y * .1f), (int)(Game1.screenSize.X * .3f), (int)(Game1.screenSize.Y * .05f));
            Constants.sensitivitySliderRect = new Rectangle((int)(Game1.screenSize.X * .1f), (int)(Game1.screenSize.Y * .4f), (int)(Game1.screenSize.X * .3f), (int)(Game1.screenSize.Y * .05f));
            Constants.FOVSliderRect = new Rectangle((int)(Game1.screenSize.X * .6f), (int)(Game1.screenSize.Y * .4f), (int)(Game1.screenSize.X * .3f), (int)(Game1.screenSize.Y * .05f));
            Constants.detailSliderRect = new Rectangle((int)(Game1.screenSize.X * .6f), (int)(Game1.screenSize.Y * .6f), (int)(Game1.screenSize.X * .3f), (int)(Game1.screenSize.Y * .05f));
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {

            timeElapsed = gameTime.ElapsedGameTime.Milliseconds;

            Game1.EnemyTargetTimer -= gameTime.ElapsedGameTime.Milliseconds;
            Game1.enemyAnimationTimer += gameTime.ElapsedGameTime.Milliseconds;

            while (enemyAnimationTimer > 100)
            {
                for (int i = 0; i < Enemy.health.Count; i++)
                {
                    if (rnd.Next(1, 5) != 1)
                    Enemy.UpdateTexture(i);
                }
                enemyAnimationTimer -= rnd.Next(75, 100);
            }


            if (Player.Attacks.flipped == -1)
            {
                punchSideLeft = true;
            }
            else
            {
                punchSideLeft = false;
            }
            if (punchFrame < 3 || combo == 0)
            {
                Game1.punchFrame += timeElapsed / 30f * (combo * .65f + 0.1f) * .5f;
            }
            if (punchFrame >= 3)
            {
                punchSideLeft = !punchSideLeft;
            }

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            if (OnKeyPress(Keys.V))
            {
                switch (gameState)
                {
                    case GameState.Playing:
                        gameState = GameState.paused;
                        IsMouseVisible = true;
                        break;
                    case GameState.paused:
                        gameState = GameState.Playing;
                        IsMouseVisible = false;
                        Mouse.SetPosition((int)screenSize.X / 2, (int)screenSize.Y / 2);
                        break;
                }
            }
            if (OnKeyPress(Keys.LeftAlt))
            {
                cheats = !cheats;
            }
            if (gameState == GameState.paused)
            {
                Slider(mouseState, previousMouseState, ref sensitivity, Constants.minSensitivity, Constants.maxSensitivity, Constants.sensitivitySliderRect);
                Slider(mouseState, previousMouseState, ref FOV, Constants.minFOV, Constants.maxFOV, Constants.FOVSliderRect);
                Slider(mouseState, previousMouseState, ref detail, Constants.minDetail, Constants.maxDetail, Constants.detailSliderRect);
            }

            if (gameState == GameState.Playing)
            {


                gametimer += timeElapsed;


                PlayerHeight = Constants.floorLevel + Constants.defaultPlayerHeight + (-4 * Constants.jumpHeight / (Constants.jumpWidth * Constants.jumpWidth) * jumpTime * (jumpTime - Constants.jumpWidth));
                jumpTime = Math.Min(jumpTime + 0.1f, Constants.jumpWidth);
                if (OnKeyPress(Keys.Space) && jumpTime == Constants.jumpWidth)
                {
                    jumpTime = 0;
                }

                if (Player.Attacks.swingSpeed == -1 && dashLengthTimer < 0)
                {

                    if (OnRightButtonPress() && (Player.state == Player.State.Idle || Player.state == Player.State.Attacking_2) && stamina >= 200)
                    {
                        Player.Attacks.SwingStart(1, 0.2f, 20f, 10, 200, combo / 40f + 0.1f, 20);
                        Player.state = Player.State.Attacking_1;
                        stamina -= 200;
                        combo += 1;
                        Player.Push(4f / combo, Player.angleVector);


                    }
                    else if (OnLeftButtonPress() && (Player.state == Player.State.Idle || Player.state == Player.State.Attacking_1) && stamina >= 200)
                    {
                        Player.Attacks.SwingStart(-1, 0.2f, 20f, 10, 200, combo / 40f + 0.1f, 20);
                        Player.state = Player.State.Attacking_2;
                        stamina -= 200;
                        combo += 1;
                        Player.Push(4f / combo, Player.angleVector);
                    }
                }
                if (stamina > dashCost && General.OnKeyPress(Keys.LeftShift) && Player.speed != new Vector2(0, 0))
                {
                    Player.iFrames = (int)(dashLength * .7f);
                    stamina -= dashCost;
                    dashLengthTimer = dashLength;
                }
                if (OnKeyRelease(Keys.LeftControl) && bowCharge >= MinBowCharge)
                {
                    if (!keyboardState.IsKeyDown(Keys.LeftControl))
                        Projectile.create(projectileType.PLAYER_PROJECTILE,
                            Player.position,
                            Player.height,
                            Player.angleVector,
                            25 + (int)bowCharge / 5,
                            2,
                            5,
                            1 + (int)(bowCharge / MaxBowCharge),
                            5 + (int)bowCharge / 5,
                            100);
                }
            }
            while (gametimer > 1000 / Constants.tps)
            {

                if (previousIsActive)
                    Player.angle += (Mouse.GetState().X - screenSize.X / 2) * sensitivity;
                if (IsActive)
                    Mouse.SetPosition((int)screenSize.X / 2, (int)screenSize.Y / 2);
                previousIsActive = IsActive;

                Player.angleVector = General.AngleToVector2(Player.angle);

                Player.iFrames = Math.Max(Player.iFrames - 1, 0);

                if (OnKeyPress(Keys.Tab))
                {
                    Levels.SetLevel(Levels.Level + 1);
                    Enemy.RandomizePositions();
                }

                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                    Exit();

                Player.speed /= 2;

                MoveKeyPressed(keyboardState);

                if (keyboardState.IsKeyDown(Keys.H) && Player.health < Constants.maxHealth && LightLevel > Constants.minLightLevel)
                {
                    healthBarColor = Color.MediumVioletRed;
                    LightLevel -= Constants.lightlevelLoss;
                    Player.health += 2;
                }
                else
                {
                    healthBarColor = ColorMultiply(Color.DeepPink, .9f);
                }
                if (previousKeyboardState.IsKeyDown(Keys.LeftControl))
                {
                    Player.state = Player.State.Drawing_Bow;
                    bowCharge = Math.Min(bowCharge + BowRechargeRate, MaxBowCharge);
                    if (bowCharge >= MinBowCharge)
                    {
                        bowChargeBarColor = Color.Red;
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
                    //case State.Drawing_Bow:
                    //Player.colorFilter = Color.AliceBlue;
                    //break;
                    case State.Attacking_1:
                        Player.colorFilter = new Color(0, combo * 8, combo * 8);
                        break;
                    case State.Attacking_2:
                        Player.colorFilter = new Color(0, combo * 8, combo * 8);
                        break;
                    case State.Dashing:
                        Player.colorFilter = Color.Purple;
                        break;
                    case State.Dead:
                        Player.colorFilter = Color.Black;
                        break;

                }

                if (dashLengthTimer < 0)
                {

                    if (Player.state == Player.State.Drawing_Bow) Player.movementSpeed = 2;
                    else Player.movementSpeed = 2;

                    if (Player.recoveryTime > 0)
                        Player.recoveryTime -= timeElapsed;
                    else
                    {
                        Player.state = Player.State.Idle;
                        combo = 0;

                    }

                    if (stamina < Constants.maxStamina)
                        stamina += timeElapsed * staminaRechargeMultiplier;
                }
                else
                {
                    Player.state = Player.State.Dashing;
                    Player.movementSpeed = dashSpeed;
                    dashLengthTimer -= timeElapsed;
                    Player.frame += 3;
                }

                for (int _index = 0; _index < Enemy.health.Count; _index++)
                    if (!Enemy.isDead[_index] && iFrames <= 0 && state != State.Dashing && Enemy.IsEnemyCollide(General.Vector2toRectangle(Player.position, width, Player.height), _index))
                    {
                        Player.TakeDamage(Color.Red, Constants.EnemyStats.damage[(int)Enemy.enemyType[_index]], 10, 500, 10, Difference(Player.position, Enemy.position[_index]));
                        combo = 0;

                    }
                LightLevel = Math.Max(Math.Min(LightLevel - Constants.lightlevelLoss, Constants.maxLightLevel), Constants.minLightLevel);


                if (Color.Black == Darkness(Color.White, RectangleToVector2(Tiles.collideRectangle[TileFromVector2(Player.position)])))
                {
                    Player.TakeDamage(
                        Color.Red,
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

                General.CircleMovement(false, ref Player.position, Player.width / 2, ref Player.speed, Player.movementSpeed * (combo / 2));
                //General.CircleMovement(true, ref Player.position, Player.width/2, ref Player.speed, Player.movementSpeed);

                gametimer -= 1000 / Constants.tps;
                if (gametimer > 1000)
                {
                    gametimer = 0;
                }

            }
            // TODO: Add your update logic here

            previousKeyboardState = Keyboard.GetState();
            previousMouseState = Mouse.GetState();

            base.Update(gameTime);
        }



        public static void CastRay(float angle)
        {
            segmentWidth = (int)(Game1.screenSize.X / (FOV * 2));
            lowestDistance = float.PositiveInfinity;
            int tileIndex = 0;

            for (int _tileIndex = 0; _tileIndex < Tiles.numTiles; _tileIndex++)
            {
                if (Tiles.tileType[_tileIndex] != (int)Tiles.tileTypes.NONE)
                {
                    float distance = .015f * LineToRectCollision(Player.position, Player.position + AngleToVector2(Player.angle + angle), Tiles.collideRectangle[_tileIndex]);
                    if (distance < lowestDistance)
                    {
                        lowestDistance = distance;
                        tileIndex = _tileIndex;
                        texturePercent = _texturePercent;
                    }
                }
            }
            //anti-fisheye
            //LowestDistance = LowestDistance * (float)Math.Cos(segment);

            if (lowestDistance != float.PositiveInfinity)
            {
                //lowestDistance *= (float)Math.Cos(angle);

                int columnHeight = (int)(screenSize.Y / lowestDistance);
                Color color = ColorFilter(Color.White, lowestDistance * 10);
                if (cheats) { color = Color.White; }
                //if (color != Color.Black)
                //{
                _spriteBatch.Draw(
            Tiles.textures[tileType[tileIndex]],
            new Rectangle(
                (int)(screenSize.X / 2 + angle * segmentWidth),
                (int)((screenSize.Y / 2) - columnHeight / 2 + PlayerHeight / lowestDistance),
                (int)(segmentWidth * detail) + 1,
                columnHeight),
            new Rectangle(
                Tiles.textureRectangle[tileIndex].X + (int)(Tiles.textureRectangle[tileIndex].Width * Game1.texturePercent),
                Tiles.textureRectangle[tileIndex].Y,
                (int)((segmentWidth * detail) / Tiles.textureRectangle[tileIndex].Width),
                Tiles.textureRectangle[tileIndex].Height),
            color,
            0f,
            new Vector2(),
            0,
            //lowestDistance / 100000
            0.1f / lowestDistance
            );
                //}
                //_spriteBatch.DrawString(titleFont, LowestDistance.ToString(), new Vector2(0, 0), Color.Wheat);
                return;
            }
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            offset = screenSize / 2 - Player.position;
            // TODO: Add your drawing code here
            _spriteBatch.Begin(SpriteSortMode.FrontToBack);
            if (cheats)
            {
                _spriteBatch.Draw(
                    Portal.texture,
                    new(Portal.collideRectangle.X + (int)offset.X, Portal.collideRectangle.Y + (int)offset.Y, Portal.collideRectangle.Width, Portal.collideRectangle.Height),
                    new Rectangle((int)Portal.textureFrame * Portal.texture.Width / Portal.amountOfFrames, 0, Portal.texture.Width / Portal.amountOfFrames, Portal.texture.Height),
                    Darkness(Portal.Color, RectangleToVector2(Portal.collideRectangle)),
                    0,
                    new Vector2(0, 0),
                    0f,
                    0.5f);

                _spriteBatch.Draw(
                        blankTexture,
                        new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y),
                        Color.Black
                        );

                if (Player.Attacks.swingSpeed != 0)
                {
                    _spriteBatch.Draw(
                        swordTexture,
                        offset + Player.position + new Vector2((float)Math.Cos(Player.Attacks.swingAngle),
                        (float)Math.Sin(Player.Attacks.swingAngle)),
                        null,
                        Player.colorFilter,
                        (float)(Player.Attacks.swingAngle),
                        new Vector2(swordTexture.Width / 2, 0),
                        0.05f,
                        SpriteEffects.FlipVertically,
                        1);
                }
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
                        .98f);
                }
                for (int index = 0; index < Enemy.health.Count; index++)
                {
                    _spriteBatch.Draw(Enemy.textures[(int)Enemy.enemyType[index]],
                        new Rectangle(Enemy.collideRectangle[index].X + (int)offset.X, Enemy.collideRectangle[index].Y + (int)offset.Y, Enemy.collideRectangle[index].Width, Enemy.collideRectangle[index].Height),
                        Enemy.textureRectangle[index],
                        Darkness(Enemy.colorFilter[index], Enemy.position[index]),
                        0,
                        new Vector2(0, 0),
                        0f,
                        0.98f);
                }
                _spriteBatch.Draw(
                    blankTexture,
                    Vector2toRectangle(screenSize / 2 + new Vector2(Player.width / 2, Player.height / 2), Player.width, Player.height),
                    Player.textureRectangle,
                    //new (0,0, 1, 1),
                    Player.colorFilter,
                    -Player.angle,
                    //new(),
                    new Vector2(Player.width / 4, Player.height / 4),
                    Player.effect,
                    .99f
                    );
            }

            _spriteBatch.Draw(crosshairTexture, new(), null, Color.White, 0, new(), 0, 0, 1.99f);

            for (int i = 0; i < (int)(2 * FOV / detail) + 1; i++)
            {
                CastRay(-FOV + i * detail);
            }
            for (int i = 0; i < Enemy.health.Count; i++)
            {
                drawObject(_spriteBatch,
                    Enemy.textures[(int)Enemy.enemyType[i]],
                    Enemy.textureRectangle[i],
                    Enemy.colorFilter[i],
                    Enemy.position[i],
                    -100,
                    Enemy.visualTextureSize[(int)Enemy.enemyType[i]].Y,
                    Enemy.visualTextureSize[(int)Enemy.enemyType[i]].X);
        }

            for (int projectileIndex = 0; projectileIndex<Projectile.position.Count; projectileIndex++)
            {
                drawObject(_spriteBatch,
                    Gems.gemTexture,
                    Projectile.textureRects[(int)Projectile.Type[projectileIndex]],
                    GetProjectileColor(projectileIndex),
                    Projectile.position[projectileIndex],
                    Projectile.height[projectileIndex],
                    Projectile.collisionSizeData[(int)Projectile.Type[projectileIndex]]* 2,
                    1);
                for (int afterImageIndex = 0; afterImageIndex<Projectile.afterImages[projectileIndex].Count; afterImageIndex++)
                {
                    drawObject(_spriteBatch,
                    Gems.gemTexture,
                    Projectile.textureRects[(int)Projectile.Type[projectileIndex]],
                    (GetProjectileColor(projectileIndex)* afterImages[projectileIndex][afterImageIndex].W),
                    new (afterImages[projectileIndex][afterImageIndex].X, afterImages[projectileIndex][afterImageIndex].Y),
                    afterImages[projectileIndex][afterImageIndex].Z,
                    Projectile.collisionSizeData[(int)Projectile.Type[projectileIndex]]* 2,
                    1);
                }
}

drawObject(_spriteBatch,
    Portal.texture,
    new Rectangle((int)((Portal.texture.Width / Portal.amountOfFrames) * (int)Portal.textureFrame), 0, Portal.texture.Width / Portal.amountOfFrames, Portal.texture.Height),
    Portal.Color,
    RectangleToVector2(Portal.collideRectangle),
    10,
    Portal.portalVisualSize,
    1);

_spriteBatch.Draw(
                blankTexture,
                new Rectangle((int)(screenSize.X / 2 - bowBarSize.X / 2), (int)(screenSize.Y * .9f), (int)((float)(bowCharge / MaxBowCharge) * bowBarSize.X), (int)bowBarSize.Y),
                null,
                bowChargeBarColor,
                0,
                new Vector2(0, 0),
                0f,
                0.99f
                );

//draws the health and stamina bars
SliderDraw(_spriteBatch, Player.health, 0, Constants.maxHealth, Constants.healthSliderRect, ColorMultiply(healthBarColor, 0.55f), healthBarColor, "", 0);
SliderDraw(_spriteBatch, stamina, 0, Constants.maxStamina, Constants.staminaSliderRect, new Color(5, 15, 50), Color.MediumPurple, "", 0);

Gems.Draw(_spriteBatch,
        new Vector2(Constants.healthSliderRect.Left - 30, Constants.healthSliderRect.Bottom),
        1,
        Color.DarkSlateBlue,
        Constants.healthSliderRect.Height * 4,
        .97f);
//draws the heart gem
Gems.Draw(_spriteBatch,
        new Vector2(Constants.healthSliderRect.Left + Constants.healthSliderRect.Width * ((float)Player.health / Constants.maxHealth), Constants.healthSliderRect.Center.Y - 10),
        6,
        Color.Pink,
        Constants.healthSliderRect.Height * 2,
        .96f);
Gems.Draw(_spriteBatch,
        new Vector2(Constants.healthSliderRect.Right, Constants.healthSliderRect.Center.Y - 10),
        6,
        Color.DeepPink,
        Constants.healthSliderRect.Height * 2,
        .95f);
//draws the stamina gems
for (int i = 1; i < maxDashCharge + 1; i++)
{
    Color gemColor;
    if (stamina >= i * (Constants.maxStamina / maxDashCharge))
    {
        gemColor = Color.AliceBlue;
    }
    else
    {
        gemColor = Color.DarkSlateGray;
    }
    Gems.Draw(_spriteBatch,
        new Vector2((i * (Constants.staminaSliderRect.Width / maxDashCharge) + Constants.staminaSliderRect.X), Constants.staminaSliderRect.Center.Y + 12),
        2,
        gemColor,
        Constants.staminaSliderRect.Height * 2,
        .96f);
}
colorFilter = new Color(colorFilter.R - 5, colorFilter.G - 5, colorFilter.B - 5);

if (punchFrame < 4)
{
    if (punchSideLeft)
    {
        _spriteBatch.Draw(punchTextures[(int)punchFrame], new(0, (int)(screenSize.Y - handRectSize.Y), (int)handRectSize.X, (int)handRectSize.Y), null, Color.LightGray, 0, new(), SpriteEffects.FlipHorizontally, 1);

    }
    else
    {
        _spriteBatch.Draw(punchTextures[(int)punchFrame], new((int)(screenSize.X - handRectSize.X), (int)(screenSize.Y - handRectSize.Y), (int)handRectSize.X, (int)handRectSize.Y), null, Color.LightGray, 0, new(), 0, 1);
    }
}

for (int i = 0; i < 12; i++)
{
    Gems.Draw(_spriteBatch, new Vector2(0, i * 50), i, Color.Wheat, 50, 1);
}

if (gameState == GameState.paused)
{
    _spriteBatch.DrawString(titleFont, "PAUSED", new Vector2(screenSize.X / 2 - titleFont.MeasureString("PAUSED").X * 2 / 2, screenSize.Y * 0.1f), Color.White, 0, new(), 2, 0, .99f);
    _spriteBatch.Draw(blankTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), null, Color.Black * 0.5f, 0, new(), 0, .97f);

    SliderDraw(_spriteBatch, sensitivity, Constants.minSensitivity, Constants.maxSensitivity, Constants.sensitivitySliderRect, Color.AliceBlue, Color.DarkBlue, "Sensitivity", 1, 10000);
    SliderDraw(_spriteBatch, FOV, Constants.minFOV, Constants.maxFOV, Constants.FOVSliderRect, Color.AliceBlue, Color.DarkBlue, "FOV", 1, 100f / Constants.maxFOV);
    SliderDraw(_spriteBatch, detail, Constants.minDetail, Constants.maxDetail, Constants.detailSliderRect, Color.AliceBlue, Color.DarkBlue, "Strip Size", 1, 100f / Constants.maxDetail);
}

_spriteBatch.Draw(blankTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), null, colorFilter * 0.2f, 0, new(), 0, .96f);


_spriteBatch.End();
base.Draw(gameTime);
        }
    }
}
