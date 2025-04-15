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

namespace first_game
{
    public class Player
    {
        public enum State
        {
            Dead,
            Idle,
            Drawing_Bow,
            Attacking_1,
            Attacking_2,
            Attacking_3,
            Stunned,
            Dashing
        }

        public static Texture2D textures;
        public static Color colorFilter;
        public const int width = 35;
        public const int height = (int)(width * 1.25f);
        public const int collisionSize = 30;

        public static Rectangle textureRectangle = new(0, 0, width, height);
        public static int frame = 0;
        public static double frameState = 0;
        public static SpriteEffects effect;

        public static float angle;
        public static Vector2 angleVector;

        public static Vector2 position = new((Tiles.rows * Tiles.tileXY - width) / 2, (Tiles.columns * Tiles.tileXY - height) / 2);
        public static Vector2 speed = new(0f, 0f);
        public static float movementSpeed = 2.5f;

        public static State state = State.Idle;
        public static int recoveryTime = 0;
        public static int health = 1000;
        public static int iFrames = 0;
        public static void Push(float _knockback, Vector2 _Angle)
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
            colorFilter = _color;
            Push(_knockback, _enemyPlayerAngle);
            state = State.Stunned;
            iFrames = _iFrames;
            health -= _damage;
            recoveryTime = _stunnTime;
            if (health <= 0)
            {
                state = State.Dead;
            }
        }
        public class Attacks
        {
            private static Vector2 checkpoint = new();
            private static float swingWidth;
            private static float swingRange;
            private static int swingDamage;
            public static float attackAngle;
            public static Vector2 attackAngleVector;
            private static float startAngle;
            private static float endAngle;
            public static float swingSpeed;
            private static int pierce;
            private static readonly int swingHitboxSize = 10;
            private static readonly int projectileHitboxSize = 30;
            private static int flipped;
            public static void Swing(int _flipped, float _swingWidth, float _swingRange, int _damage, int _recoveryTime, int _swingSpeed, int _pierce)
            {
                Push(5, angleVector);
                flipped = _flipped;
                recoveryTime = _recoveryTime;
                swingWidth = (float)Math.PI * _swingWidth;
                swingRange = _swingRange;
                swingDamage = _damage;
                attackAngle = angle + swingWidth * flipped; //flipped shouldo only be 1 or -1
                startAngle = angle + swingWidth;
                endAngle = angle - swingWidth;
                swingSpeed = _swingSpeed;
                pierce = _pierce;
                for (int _index = 0; _index < Enemy.iFrames.Count; _index++)
                    Enemy.iFrames[_index] = 0;
                for (int _index = 0; _index < Tiles.swingIFrames.Length; _index++)
                    Tiles.swingIFrames[_index] = 0;
                Player.iFrames = 10;
            }
            public static void SwingUpdate()
            {
                for (int _index = 0; _index < Enemy.iFrames.Count; _index++)
                    if (Enemy.iFrames[_index] > 0) { Enemy.iFrames[_index] -= 1; }
                for (int _index = 0; _index < Tiles.numTiles; _index++)
                    if (Tiles.swingIFrames[_index] > 0) { Tiles.swingIFrames[_index] -= 1; }

                for (int i = 0; i <= swingSpeed; i++)
                {
                    if (!(attackAngle >= endAngle && attackAngle <= startAngle))
                    {
                        swingSpeed = -1;
                    }
                    checkpoint = position + attackAngleVector * swingRange;
                    attackAngle -= 0.1f * flipped;
                    attackAngleVector = new((float)Math.Cos(attackAngle), (float)Math.Sin(attackAngle));

                    for (int index = 0; index < Enemy.collideRectangle.Count; index++)
                        if (Enemy.collideRectangle[index].Intersects(new Rectangle((int)(checkpoint.X - swingHitboxSize / 2), (int)(checkpoint.Y - swingHitboxSize / 2), swingHitboxSize, swingHitboxSize)) && Enemy.iFrames[index] <= 0 && pierce > 0)
                        {
                            Enemy.Push(25, Enemy.position[index] - position, index);
                            Enemy.TakeDamage(Color.Red, swingDamage, swingDamage, index);
                            pierce -= 1;
                        }
                    for (int index = 0; index < Tiles.numTiles; index++)
                    {
                        if (Tiles.collideRectangle[index].Contains(checkpoint) && Tiles.swingIFrames[index] <= 0 && Tiles.tileType[index] == (int)Tiles.tileTypes.BRICK)
                        {
                            Tiles.TakeDamage(Color.AliceBlue, swingDamage, 10, index);
                        }
                    }
                    for (int index = 0; index < Projectile.position.Count; index++)
                    {
                        if (!Projectile.IframesEnemyIndex[index].Contains("swing") && new Rectangle((int)(checkpoint.X - projectileHitboxSize / 2), (int)(checkpoint.Y - projectileHitboxSize / 2), projectileHitboxSize, projectileHitboxSize).Contains(Projectile.position[index]))
                        {
                            Projectile.pierce[index] += 1;
                            Projectile.speed[index] = Player.Attacks.attackAngleVector * 20;
                            Projectile.damage[index] = (int)(Projectile.damage[index] * 2.5f);
                            Projectile.Type[index] = Projectile.projectileType.PLAYER_PROJECTILE;
                            Projectile.Iframes[index].Add(10);
                            Projectile.IframesEnemyIndex[index].Add("swing");
                        }
                    }
                }
            }

        }
        public static void Setup(Texture2D _player)
        {
            textures = _player;
            UpdateTexture();
        }
        public static void UpdateTexture()
        {

            if (frameState < 2.9) { frameState += 0.1; }
            else { Player.frameState = 0; }
            Player.textureRectangle = new Rectangle(16 * (int)frameState, Player.frame * 32, 16, 20);
        }
    }
}
