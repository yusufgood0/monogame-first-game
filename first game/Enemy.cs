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
using static first_game.Enemy;
using static first_game.Projectile;
using static first_game.Tiles;

namespace first_game
{
    public class Enemy
    {
        public static bool IsEnemyCollide(Rectangle _rect, int _index)
        {
            if (Constants.EnemyStats.circle[(int)Enemy.enemyType[_index]])
            {
                if (General.CircleCollisionBool(Enemy.position[_index], Enemy.collideRectangle[_index].Width / 2, _rect))
                {
                    return true;
                }
            }
            else if (Enemy.collideRectangle[_index].Intersects(_rect))
            {
                return true;
            }
            return false;
        }

        static readonly Random rnd = new();
        public static void RandomizePositions()
        {
            for (int _enemyIndex = Enemy.collideRectangle.Count - 1; _enemyIndex >= 0; _enemyIndex--)
            {
                Enemy.position[_enemyIndex] = General.RectangleToVector2(Tiles.collideRectangle[Tiles.RandomOpen(tileTypes.NONE)]);
                while (SightLine(position[_enemyIndex]))
                {
                    Enemy.position[_enemyIndex] = General.RectangleToVector2(Tiles.collideRectangle[Tiles.RandomOpen(tileTypes.NONE)]);
                }
            }

        }

        public static bool CheckEnemyTileCollision(int _index)
        {
            for (int tileIndex = Tiles.numTiles - 1; tileIndex >= 0; tileIndex--)
                if (Tiles.tileType[tileIndex] != (int)Tiles.tileTypes.NONE && Enemy.collideRectangle[_index].Intersects(Tiles.collideRectangle[tileIndex]))
                {
                    switch (Tiles.tileType[tileIndex])
                    {
                        case (int)Tiles.tileTypes.SOLID:
                            return true;

                        case (int)Tiles.tileTypes.BRICK:
                            Tiles.tileType[tileIndex] = (int)tileTypes.NONE;
                            Tiles.loadTiles(tileIndex, tileIndex);
                            return true;

                    }

                }
            return false;
        }

        public static void Push(float _knockback, Vector2 _Angle, int _index)
        {
            if (_Angle != new Vector2(0, 0))
            {
                _Angle.Normalize();
                speed[_index] += _Angle * _knockback;
            }
        }
        public static bool TakeDamage(Color _color, int _damage, int _iFrames, int _index)
        {
            Game1.LightLevel += _damage;
            colorFilter[_index] = _color;
            iFrames[_index] = _iFrames;
            health[_index] -= _damage;
            if (health[_index] <= 0)
            {
                Kill(_index);
                return true;
            }
            else return false;
        }
        public static void CreateEnemies(int[] _enemyTypes)
        {
            for (int i = 0; i < _enemyTypes.Length; i++)
            {
                for (int i2 = 0; i2 < _enemyTypes[i]; i2++)
                    Create(new(), (EnemyType)i);

            }
        }
        public static void Create(Vector2 _spawnLocation, EnemyType _EnemyType)
        {
            abilityTimer.Add(0);
            health.Add(Constants.EnemyStats.health[(int)_EnemyType]);
            textureRectangle.Add(new Rectangle(0, 0, (int)(Enemy.textures[(int)_EnemyType].Width / Enemy.textureArray[(int)_EnemyType].X), (int)(Enemy.textures[(int)_EnemyType].Height / Enemy.textureArray[(int)_EnemyType].X)));
            collideRectangle.Add(new Rectangle((int)_spawnLocation.X - Constants.EnemyStats.width[(int)_EnemyType] / 2, (int)_spawnLocation.Y - Constants.EnemyStats.height[(int)_EnemyType] / 2, Constants.EnemyStats.width[(int)_EnemyType], Constants.EnemyStats.height[(int)_EnemyType]));
            position.Add(new Vector2((int)_spawnLocation.X, (int)_spawnLocation.Y));
            target.Add(_spawnLocation - new Vector2(0, 10));
            iFrames.Add(1);
            speed.Add(new());
            enemyType.Add(_EnemyType);
            colorFilter.Add(Color.DarkSalmon);
        }
        public static void KillAll()
        {
            for (int _index = health.Count - 1; _index >= 0; _index--)
            {
                Kill(_index);
            }
        }
        public static void Kill(int _index)
        {
            Game1.playerLightEmit -= Constants.maxPlayerLightEmit / 200 * Constants.EnemyStats.health[(int)Enemy.enemyType[_index]];
            if (Game1.playerLightEmit >= Constants.maxPlayerLightEmit)
                Game1.playerLightEmit = Constants.maxPlayerLightEmit;

            for (int ProjectileIndex = 0; ProjectileIndex < Projectile.Iframes.Count; ProjectileIndex++)
            {
                Projectile.Iframes[ProjectileIndex].Remove(_index);
                Projectile.IframesEnemyIndex[ProjectileIndex].Remove(_index);
            }
            abilityTimer.RemoveAt(_index);
            health.RemoveAt(_index);
            textureRectangle.RemoveAt(_index);
            collideRectangle.RemoveAt(_index);
            position.RemoveAt(_index);
            target.RemoveAt(_index);
            iFrames.RemoveAt(_index);
            speed.RemoveAt(_index);
            enemyType.RemoveAt(_index);
            colorFilter.RemoveAt(_index);
        }

        public static Texture2D[] textures = new Texture2D[4];
        public static Vector2[] textureArray = new Vector2[4];
        public static Vector2[] visualTextureSize = new Vector2[4];

        public static List<int> damage = new();
        public static List<int> abilityTimer = new();
        public static List<float> movementSpeed = new();
        public static List<Rectangle> textureRectangle = new();
        public static List<Rectangle> collideRectangle = new();
        public static List<Vector2> speed = new();
        public static List<Vector2> position = new();
        public static List<Vector2> target = new();
        public static List<int> health = new();
        public static List<int> iFrames = new();
        public static List<EnemyType> enemyType = new();
        public static List<Color> colorFilter = new();


        public static bool SightLine(Vector2 _point)
        {
            float _detail = General.DistanceFromPoints(_point, Player.position)/10;
            Vector2 _difference = (Player.position - _point) / _detail;
            for (int i = 0; i < _detail; i++)
            {
                _point += _difference;
                for (int index = 0; index < Tiles.numTiles; index++)
                    if (Tiles.collideRectangle[index].Contains(_point) && Tiles.tileType[index] != 0)
                        return false;
            }
            return true; // No obstacles in the way
        }
        public static void TextureRectSetup()
        {

        }
        public static void Update(int _index)
        {
            Vector2 _difference = General.Difference(target[_index], position[_index]); //X and Y difference 
            if (_difference == new Vector2(0, 0))
            {
                return;
            }
            float _distance = General.DistanceFromDifference(_difference); //hypotinuse/distance to target
            _difference.Normalize();

            Vector2 _speed = speed[_index] / ((textureRectangle[_index].Height + textureRectangle[_index].Width) / 7);
            bool _sightline = SightLine(position[_index]);

            Vector2 _newTargetAngle = new(rnd.Next(-50, 50), rnd.Next(-50, 50));
            while (_newTargetAngle == new Vector2(0, 0)) _newTargetAngle = new Vector2(rnd.Next(-50, 50), rnd.Next(-50, 50));

            _newTargetAngle.Normalize();

            if (Game1.EnemyTargetTimer <= 0)
            {

                switch (enemyType[_index])
                {
                    case EnemyType.ARCHER:
                        if (rnd.Next(4) == 1)
                        {
                            target[_index] = position[_index] + _newTargetAngle * rnd.Next(10, 30) * Constants.EnemyStats.movementSpeed[(int)Enemy.enemyType[_index]] * Constants.Archer.archerStopRange;
                        }
                        break;
                    default:
                        if (rnd.Next(20) == 1)
                            target[_index] = position[_index] + _newTargetAngle * rnd.Next(10, 30) * Constants.EnemyStats.movementSpeed[(int)Enemy.enemyType[_index]];
                        break;
                }
                if (_sightline)
                {
                    colorFilter[_index] = Color.Coral;
                    target[_index] = Player.position;
                }

            }

            

            if (_distance > 10)
            {
                if (iFrames[_index] == 0)
                {
                    if (_sightline)
                    {
                        colorFilter[_index] = Color.Red;
                    }

                    _speed += _difference * Constants.EnemyStats.movementSpeed[(int)Enemy.enemyType[_index]];
                    switch (enemyType[_index])
                    {
                        case EnemyType.ARCHER:
                            if (_sightline)
                            {
                                if (abilityTimer[_index] < 0)
                                {
                                    abilityTimer[_index] = Constants.Archer.attackDelay;
                                    Projectile.create(Projectile.projectileType.ENEMY_PROJECTILE, 
                                        position[_index], 
                                        General.Difference(Player.position, Enemy.position[_index]), 
                                        7f, 
                                        1000, 
                                        10, 
                                        1,
                                        Constants.Archer.archerDamage);
                                }
                                else
                                {
                                    abilityTimer[_index] -= 1;
                                }
                                if (_distance < Constants.Archer.archerStopRange)
                                {
                                    _speed -= _difference * Constants.EnemyStats.movementSpeed[(int)EnemyType.ARCHER];
                                    if (_distance < Constants.Archer.archerBackupRange)
                                    {
                                        _speed -= _difference * Constants.Archer.archerBackupSpeed;
                                    }
                                }
                            }
                            break;
                        case EnemyType.LARGE:
                            break;
                    }
                }
                Vector2 _position = position[_index];
                if (Constants.EnemyStats.circle[(int)enemyType[_index]])
                {
                    General.CircleMovement(false, ref _position, Enemy.collideRectangle[_index].Width / 2, ref _speed, Constants.EnemyStats.movementSpeed[(int)Enemy.enemyType[_index]]);
                }
                else
                {
                    General.RectMovement(false, ref _position, new Vector2(collideRectangle[_index].Width, collideRectangle[_index].Height), ref _speed, Constants.EnemyStats.movementSpeed[(int)Enemy.enemyType[_index]]);
                }
                position[_index] = _position;
                speed[_index] = speed[_index] * 0.6f;
                collideRectangle[_index] = new Rectangle((int)position[_index].X - collideRectangle[_index].Width / 2, (int)position[_index].Y - collideRectangle[_index].Height / 2, collideRectangle[_index].Width, collideRectangle[_index].Height);
                collideRectangle[_index] = General.Vector2toRectangle(position[_index], collideRectangle[_index].Width, collideRectangle[_index].Height);
            }
            
        }
        public enum EnemyType
        {
            SMALL = 0,
            MEDIUM = 1,
            LARGE = 2,
            ARCHER = 3,
        }
        public static void Setup(object[] _textures)
        {
            for (int i = 0; i * 3 < _textures.Length; i++)
            {
                Enemy.textures[i] = (Texture2D)_textures[i * 3];
                Enemy.textureArray[i] = (Vector2)_textures[i * 3 + 1];
                Enemy.visualTextureSize[i] = (Vector2)_textures[i * 3 + 2];
            }
        }
    }
}
