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
    public class Projectile
    {
        public static List<Vector2> speed = new List<Vector2>();
        public static List<Vector2> position = new List<Vector2>();
        public static List<projectileType> Type = new List<projectileType>();
        public static List<int> Iframes =  new List<int>();
        public static List<int> pierce = new List<int>();

        public enum projectileType
        {
            PLAYER_ARROW = 0,
            ENEMY = 1,
        }
        public static int[] damagesData =
            {
                10,
                125,
            };
        public static int[] collisionSizeData =
            {
                5,
                10,
            };
        public static int[] pierceData =
            {
                2,
                1,
            };

        public static void create(projectileType type, Vector2 spawnLocation, Vector2 angleVector, float projectileSpeed, int projectileLife, int _collisionSize)
        {
            if (angleVector != new Vector2(0, 0))
            {
                angleVector.Normalize();
            }
            speed.Add(angleVector * projectileSpeed);
            position.Add(spawnLocation);
            Type.Add(type);
            Iframes.Add(projectileLife);
            pierce.Add(pierceData[(int)type]);

        }
        public static void update(int _index)
        {
            position[_index] += speed[_index];


            for (int i = 0; i < Enemy.collideRectangle.Count; i++) {
                if (Iframes[_index] > 0)
                {
                    Iframes[_index] -= 1;
                    break;
                }

                if (new Rectangle((int)(position[_index].X - collisionSizeData[(int)Type[_index]]), (int)(position[_index].Y - collisionSizeData[(int)Type[_index]]), collisionSizeData[(int)Type[_index]] *2, collisionSizeData[(int)Type[_index]] *2).Intersects(Enemy.collideRectangle[i]))
                {
                    Enemy.TakeDamage(Color.Red, damagesData[(int)Type[_index]], 0, i);
                    Iframes[_index] = 5;
                    pierce[_index] -= 1;
                }
            }

            for (int index = 0; index < Tiles.numTiles; index++)
            {
                if (Tiles.collideRectangle[index].Contains(position[_index]) && Tiles.swingIFrames[index] <= 0)
                {
                    switch (Tiles.tileType[index])
                    {
                        case (int)tileTypes.BRICK:
                        Tiles.TakeDamage(Color.AliceBlue, damagesData[(int)Type[_index]], 15, index);
                        pierce[_index] -= 1;
                        break;

                        case (int)tileTypes.SOLID:
                        kill(_index);
                        return;

                    }

                }
            }

            if (pierce[_index] <= 0) kill(_index);
        }

        public static void kill(int _index)
        {
            pierce.RemoveAt(_index);
            speed.RemoveAt(_index);
            position.RemoveAt(_index);
            Type.RemoveAt(_index);
            Iframes.RemoveAt(_index);
        }

    }
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

    public class Enemy
    {
        public static void push(float _knockback, Vector2 _Angle, int _index)
        {
            if (_Angle != new Vector2(0, 0))
            {
                _Angle.Normalize();
                position[_index] += _Angle * _knockback;
            }
        }
        public static void TakeDamage(Color _color, int _damage, int _iFrames, int _index)
        {
            swingIFrames[_index] = _iFrames;
            health[_index] -= _damage;
            if (health[_index] <= 0)
            {
                damage.RemoveAt(_index);
                movementSpeed.RemoveAt(_index);
                textureRectangle.RemoveAt(_index);
                collideRectangle.RemoveAt(_index);
                position.RemoveAt(_index);
                target.RemoveAt(_index);
                health.RemoveAt(_index);
                swingIFrames.RemoveAt(_index);
            }
        }

        public static Texture2D textures;

        public static int width;
        public static int height;
        public static List<int> damage = new List<int>();
        public static List<int> movementSpeed = new List<int>();
        public static List<Rectangle> textureRectangle = new List<Rectangle>();
        public static List<Rectangle> collideRectangle = new List<Rectangle>();
        public static List<Vector2> position = new List<Vector2>();
        public static List<Vector2> target = new List<Vector2>();
        public static List<int> health = new List<int>();
        public static List<int> swingIFrames = new List<int>();


        public static bool SightLine(Vector2 _point, float _detail)
        {
            double _growth = 1;

            while (true)
            {
                _point = new Vector2(_point.X - (_point.X - Player.position.X) / _detail, _point.Y - (_point.Y - Player.position.Y) / _detail);
                _growth += 1;
                for (int index = 0; index < Tiles.numTiles; index++)
                    if (new Rectangle((int)(Tiles.collideRectangle[index].X - _growth / 2), (int)(Tiles.collideRectangle[index].Y - _growth / 2), (int)(Tiles.tileXY + _growth), (int)(Tiles.tileXY + _growth)).Contains(_point) && Tiles.tileType[index] != 0)
                        return false;

                if (new Rectangle((int)(Player.position.X - _growth / 2), (int)(Player.position.Y - _growth / 2), (int)(Player.width + _growth), (int)(Player.height + _growth)).Contains(_point))
                    return true; // No obstacles in the way
            }
        }
        public static void Step(int _index)
        {

            Vector2 _difference = target[_index] - position[_index]; //X and Y difference 
            float _distance = (float)Math.Sqrt(_difference.X * _difference.X + _difference.Y * _difference.Y); //hypotinuse/distance to target
            if (SightLine(position[_index], _distance))
            {
                target[_index] = Player.position;
            }
            if (_distance > 10)
            {
                Vector2 _position = position[_index];

                General.movement(ref _position, collideRectangle[_index].Width, ref _difference, Enemy.movementSpeed[_index] / 10f, Enemy.collideRectangle[_index]);

                position[_index] = _position;
                //Enemy.collideRectangle[_index] = new Rectangle((int)position[_index].X, (int)position[_index].Y, collideRectangle[_index].Width, collideRectangle[_index].Height);

                //_difference.Normalize();
                //position[_index] += _difference * movementSpeed[_index]/10;
                collideRectangle[_index] = new Rectangle((int)position[_index].X - collideRectangle[_index].Width / 2, (int)position[_index].Y - collideRectangle[_index].Height / 2, collideRectangle[_index].Width, collideRectangle[_index].Height);
            }

        }
        public enum EnemyType
        {
            SMALL,
            MEDIUM,
            LARGE,
            ARCHER,
        }
        public static void create(Vector2 _spawnLocation, EnemyType _EnemyType)
        {
            if (_EnemyType == EnemyType.SMALL)
            {
                movementSpeed.Add(25);
                damage.Add(50);
                health.Add(50);
                height = 25;
                width = 25;
            }
            if (_EnemyType == EnemyType.MEDIUM)
            {
                movementSpeed.Add(15);
                damage.Add(100);
                health.Add(200);
                height = 40;
                width = 40;
            }
            if (_EnemyType == EnemyType.LARGE)
            {
                movementSpeed.Add(7);
                damage.Add(200);
                health.Add(500);
                height = 80;
                width = 80;
            }

            Vector2 spawnLocation = new Vector2((int)_spawnLocation.X - width / 2, (int)_spawnLocation.Y - height / 2);
            textureRectangle.Add(new Rectangle(0, 0, width, height));
            position.Add(new Vector2((int)spawnLocation.X, (int)spawnLocation.Y));
            collideRectangle.Add(new Rectangle((int)spawnLocation.X, (int)spawnLocation.Y, width, height));
            target.Add(spawnLocation - new Vector2(0, 1));
            swingIFrames.Add(1);
        }
        public static void Setup(Texture2D _enemy)
        {
            textures = _enemy;
        }
    }
    public class Player
    {
        public enum State
        {
            Dead,
            Idle,
            Attacking_1,
            Attacking_2,
            Attacking_3,
            Stunned,
            Dashing
        }

        public static Texture2D textures;
        public const int width = 30;
        public const int height = 40;
        public const int collisionSize = 30;

        public static Rectangle textureRectangle = new Rectangle(0, 0, width, height);

        public static float angle;
        public static Vector2 angleVector;

        public static Vector2 position = new Vector2((Tiles.rows * Tiles.tileXY - width) / 2, (Tiles.columns * Tiles.tileXY - height) / 2);
        public static Vector2 speed = new Vector2(0f, 0f);
        public static float movementSpeed = 3f;

        public static State state = State.Idle;
        public static int recoveryTime = 0;
        public static int health = 1000;
        public static int iFrames = 0;
        public static void push(float _knockback, Vector2 _Angle)
        {
            if (_Angle != new Vector2(0, 0))
            {
                _Angle.Normalize();
                speed += _Angle * _knockback;
                movementSpeed = _knockback;
            }
        }
        public static void TakeDamage(Color _color, int _damage, int _iFrames, int _stunnTime, float _knockback, Vector2 _enemyPlayerAngle)
        {
            if (iFrames <= 0 && state != State.Dashing)
            {
                push(_knockback, _enemyPlayerAngle);
                state = State.Stunned;
                iFrames = _iFrames;
                health -= _damage;
                recoveryTime = _stunnTime;
                if (health <= 0)
                {
                    state = State.Dead;
                }
            }
        }
        public class Attacks
        {
            new public class Swing
            {
                public static Vector2 checkpoint = new Vector2();
                private static float swingWidth;
                private static float swingRange;
                private static int swingDamage;
                public static float attackAngle;
                public static float endAngle;
                private static float swingSpeed;
                private static int pierce;
                public static void swing(float _swingWidth, float _swingRange, int _damage, int _recoveryTime, int _swingSpeed, int _pierce)
                {
                    push(20, new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)));
                    recoveryTime = _recoveryTime;
                    swingWidth = (float)Math.PI * _swingWidth;
                    swingRange = _swingRange;
                    swingDamage = _damage;
                    attackAngle = angle + swingWidth;
                    endAngle = angle - swingWidth;
                    swingSpeed = _swingSpeed;
                    pierce = _pierce;
                    for (int _index = 0; _index < Enemy.swingIFrames.Count; _index++)
                        Enemy.swingIFrames[_index] = 0;
                    iFrames = 10;

                }
                public static void swingUpdate()
                {
                    for (int _index = 0; _index < Enemy.swingIFrames.Count; _index++)
                        if (Enemy.swingIFrames[_index] > 0) { Enemy.swingIFrames[_index] -= 1; }
                    for (int _index = 0; _index < Tiles.numTiles; _index++)
                        if (Tiles.swingIFrames[_index] > 0) { Tiles.swingIFrames[_index] -= 1; }

                    for (int i = 0; i <= swingSpeed; i++)
                        if (attackAngle >= endAngle)
                        {
                            checkpoint = position + new Vector2((float)Math.Cos(attackAngle), (float)Math.Sin(attackAngle)) * swingRange;
                            for (int index = 0; index < Enemy.collideRectangle.Count; index++)
                                if (Enemy.collideRectangle[index].Contains(checkpoint) && Enemy.swingIFrames[index] <= 0 && pierce > 0)
                                {
                                    Enemy.push(15, Enemy.position[index] - position, index);
                                    Enemy.TakeDamage(Color.Red, swingDamage, swingDamage, index);
                                    pierce -= 1;

                                }
                            attackAngle -= 0.1f;
                            for (int index = 0; index < Tiles.numTiles; index++)
                            {
                                if (Tiles.collideRectangle[index].Contains(checkpoint) && Tiles.swingIFrames[index] <= 0 && Tiles.tileType[index] == (int)Tiles.tileTypes.BRICK)
                                {
                                    Tiles.TakeDamage(Color.AliceBlue, swingDamage, 15, index);
                                }
                            }

                        }
                }
            }



        }
        public static void Step()
        {
            if (speed != new Vector2(0, 0))
                speed.Normalize();

            position.X += (int)(speed.X * movementSpeed);
            for (int index = 0; index < Tiles.numTiles; index++)
                if (new Rectangle((int)position.X - collisionSize / 2, (int)position.Y - collisionSize / 2, collisionSize, collisionSize).Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {
                    if (speed.X < 0)
                        position.X = Tiles.collideRectangle[index].X + Tiles.collideRectangle[index].Width + collisionSize / 2;
                    else
                        position.X = Tiles.collideRectangle[index].X - collisionSize / 2;
                }

            position.Y += (int)(speed.Y * movementSpeed);
            for (int index = 0; index < Tiles.numTiles; index++)
                if (new Rectangle((int)position.X - collisionSize / 2, (int)position.Y - collisionSize / 2, collisionSize, collisionSize).Intersects(Tiles.collideRectangle[index]) && Tiles.tileType[index] != 0)
                {

                    if (speed.Y < 0)
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
                        health[_index] = 1;
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

            Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2, 200), Enemy.EnemyType.SMALL);
            Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2 - 50, 300), Enemy.EnemyType.MEDIUM);
            Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2 + 50, 300), Enemy.EnemyType.MEDIUM);
            Enemy.create(new Vector2((Tiles.rows * Tiles.tileXY - Enemy.width) / 2, 400), Enemy.EnemyType.LARGE);

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
                    Tiles.regenerateTilemap();

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

                if (keyboardState.IsKeyDown(Keys.LeftControl) && !(previousKeyboardState.IsKeyDown(Keys.LeftControl)))
                {
                    Projectile.create(projectileType.PLAYER_ARROW, Player.position, Player.angleVector, 10, 2, 5);
                }

                if (dashLengthTimer < 0)
                {
                    Player.movementSpeed = 3;
                    movementKeyboardState = Keyboard.GetState();

                    if (keyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
                    {
                        if (Player.state == Player.State.Idle)
                        {
                            Player.Attacks.Swing.swing(0.4f, 30f, 10, 300, 2, 2);
                            Player.state = Player.State.Attacking_1;
                        }
                        else if (Player.state == Player.State.Attacking_1)
                        {
                            Player.Attacks.Swing.swing(0.2f, 35f, 12, 750, 2, 2);
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
                        Player.TakeDamage(Color.BlueViolet, Enemy.damage[_index], 10, 500, 30, Player.position - Enemy.position[_index]);


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
            for (int index = 0; index < Enemy.collideRectangle.Count; index++)
            {
                _spriteBatch.Draw(Enemy.textures, Enemy.collideRectangle[index], Enemy.textureRectangle[index], Color.White, 0, new Vector2(0, 0), 0f, 0.1f);
                _spriteBatch.DrawString(titleFont, Enemy.health[index].ToString(), Enemy.position[index], Color.Red);
            }
            for (int index = 0; index < Projectile.position.Count; index++)
            {
                _spriteBatch.Draw(blankTexture, new Rectangle((int)Projectile.position[index].X - 5, (int)Projectile.position[index].Y - 5, 10, 10), null, Color.White, 0, new Vector2(0, 0), 0f, 0.5f);
                //_spriteBatch.DrawString(titleFont, Enemy.health[index].ToString(), Enemy.position[index], Color.Red);
            }

            _spriteBatch.Draw(Player.textures, new Rectangle((int)Player.position.X, (int)Player.position.Y, Player.width, Player.height), Player.textureRectangle, Color.White, Player.angle + (float)Math.PI / 2, new Vector2(Player.width / 2, Player.height / 2), 0f, 0.2f);
            _spriteBatch.Draw(blankTexture, new Rectangle(32, 32, (int)((float)(dashCooldownTimer / (float)(maxDashCharge * dashCooldown)) * 150), 32), null, Color.White, 0, new Vector2(0, 0), 0f, 0.3f);
            for (int i = 0; i < maxDashCharge + 1; i++) { _spriteBatch.Draw(blankTexture, new Rectangle(32 + i * (150 / maxDashCharge), 32, 5, 20), null, Color.Blue, 0, new Vector2(0, 0), 0f, 0.4f); }
            if (Player.Attacks.Swing.attackAngle >= Player.Attacks.Swing.endAngle) { _spriteBatch.Draw(blankTexture, new Rectangle((int)Player.Attacks.Swing.checkpoint.X - 5, (int)Player.Attacks.Swing.checkpoint.Y - 5, 10, 10), null, Color.White, 0, new Vector2(0, 0), 0f, 0.5f); }

            _spriteBatch.DrawString(titleFont, (Player.health / 10).ToString(), new Vector2(10, 10), Color.White);

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
