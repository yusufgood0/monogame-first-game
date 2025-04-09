using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace first_game
{
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
}
