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
        static readonly Random rnd = new();
        public static void RespawnEnemies()
        {
            int _tileIndex;

            for (int _enemyIndex = Enemy.collideRectangle.Count - 1; _enemyIndex >= 0; _enemyIndex--)
            {

                _tileIndex = RandomTileIndex();
                Enemy.Create(new Vector2(Tiles.collideRectangle[_tileIndex].X + Tiles.tileXY / 2, Tiles.collideRectangle[_tileIndex].Y + Tiles.tileXY / 2), Enemy.type[_enemyIndex]);
                while (CheckEnemyTileCollision(Enemy.collideRectangle.Count - 1))
                {
                    _tileIndex = RandomTileIndex();
                    Kill(Enemy.collideRectangle.Count - 1);
                    Enemy.Create(new Vector2(Tiles.collideRectangle[_tileIndex].X + Tiles.tileXY / 2, Tiles.collideRectangle[_tileIndex].Y + Tiles.tileXY / 2), Enemy.type[_enemyIndex]);
                }
                Kill(_enemyIndex);

            }   
            _tileIndex = RandomTileIndex();
            Player.position = new Vector2(Tiles.collideRectangle[_tileIndex].X + Tiles.tileXY / 2, Tiles.collideRectangle[_tileIndex].Y + Tiles.tileXY / 2);

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
                            Tiles.load(tileIndex, tileIndex);
                            return true;

                    }
                    
                }
            return false;
        }
        public static int RandomTileIndex()
        {
            int _tileIndex = rnd.Next(Tiles.numTiles - 1);
            while (Tiles.tileType[_tileIndex] != (int)Tiles.tileTypes.NONE)
            {
                _tileIndex = rnd.Next(Tiles.numTiles - 1);
            }
            return _tileIndex;
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
            colorFilter[_index] = _color;
            iFrames[_index] = _iFrames;
            health[_index] -= _damage;
            if (type[_index] == EnemyType.ARCHER)
            {
                abilityTimer[_index] = Constants.Archer.attackDelay;
            }
            if (health[_index] <= 0)
            {
                Kill(_index);
                return true;
            }
            else return false;
        }
        public static void Create(Vector2 _spawnLocation, EnemyType _EnemyType)
        {
            abilityTimer.Add(0);
            health.Add(Constants.EnemyStats.health[(int)_EnemyType]);
            textureRectangle.Add(new Rectangle(0, 0, Constants.EnemyStats.width[(int)_EnemyType], Constants.EnemyStats.height[(int)_EnemyType]));
            collideRectangle.Add(new Rectangle((int)_spawnLocation.X - Constants.EnemyStats.width[(int)_EnemyType] / 2, (int)_spawnLocation.Y - Constants.EnemyStats.height[(int)_EnemyType] / 2, Constants.EnemyStats.width[(int)_EnemyType], Constants.EnemyStats.height[(int)_EnemyType]));
            position.Add(new Vector2((int)_spawnLocation.X, (int)_spawnLocation.Y));
            target.Add(_spawnLocation - new Vector2(0, 10));
            iFrames.Add(1);
            speed.Add(new Vector2(0, 0));
            type.Add(_EnemyType);
            colorFilter.Add(Color.DarkSalmon);
        }
        public static void Kill(int _index)
        {
            for (int ProjectileIndex = 0; ProjectileIndex < Projectile.Iframes.Count; ProjectileIndex++)
            {
                Projectile.Iframes[ProjectileIndex].Remove(_index);
                Projectile.IframesEnemyIndex[ProjectileIndex].Remove(_index.ToString());
            }
            abilityTimer.RemoveAt(_index);
            health.RemoveAt(_index);
            textureRectangle.RemoveAt(_index);
            collideRectangle.RemoveAt(_index);
            position.RemoveAt(_index);
            target.RemoveAt(_index);
            iFrames.RemoveAt(_index);
            speed.RemoveAt(_index);
            type.RemoveAt(_index);
            colorFilter.RemoveAt(_index);

        }

        public static Texture2D textures;

        public static int width;
        public static int height;
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
        public static List<EnemyType> type = new();
        public static List<Color> colorFilter = new();


        public static bool SightLine(Vector2 _point, float _detail)
        {
            while (true)
            {
                _point = new Vector2(_point.X - (_point.X - Player.position.X) / _detail, _point.Y - (_point.Y - Player.position.Y) / _detail);
                for (int index = 0; index < Tiles.numTiles; index++)
                    if (new Rectangle((int)(Tiles.collideRectangle[index].X), (int)(Tiles.collideRectangle[index].Y), (int)(Tiles.tileXY), (int)(Tiles.tileXY)).Contains(_point) && Tiles.tileType[index] != 0)
                        return false;

                if (new Rectangle((int)(Player.position.X - Player.width/2), (int)(Player.position.Y - Player.height/2), (int)(Player.width), (int)(Player.height)).Contains(_point))
                    return true; // No obstacles in the way
            }
        }
        public static void Update(int _index)
        {
            Vector2 _difference = target[_index] - position[_index]; //X and Y difference 
            if (_difference == new Vector2(0, 0))
            {
                return;
            }
            float _distance = (float)Math.Sqrt(_difference.X * _difference.X + _difference.Y * _difference.Y); //hypotinuse/distance to target
            Vector2 _speed = speed[_index] / ((textureRectangle[_index].Height + textureRectangle[_index].Width)/7);
            Vector2 _newTargetAngle = new(rnd.Next(-50, 50), rnd.Next(-50, 50));
            //while (_newTargetAngle == new Vector2(0, 0)) _newTargetAngle = new Vector2(rnd.Next(-50, 50), rnd.Next(-50, 50));

            
            _newTargetAngle.Normalize();

            if (SightLine(position[_index], _distance))
            {
                target[_index] = Player.position;
            }
            else if (rnd.Next(100) == 1 && (type[_index] == EnemyType.SMALL || type[_index] == EnemyType.MEDIUM || type[_index] == EnemyType.LARGE))
            {
                target[_index] = position[_index] + _newTargetAngle * rnd.Next(10, 30) * Constants.EnemyStats.movementSpeed[(int)Enemy.type[_index]];
            }
            else if (rnd.Next(50) == 1 && (type[_index] == EnemyType.ARCHER))
            {
                target[_index] = position[_index] + _newTargetAngle * rnd.Next(10, 30) * Constants.EnemyStats.movementSpeed[(int)Enemy.type[_index]] * Constants.Archer.archerStopRange;
            }

            if (_distance > 10)
            {
                Vector2 _position = position[_index];
                _difference.Normalize();
                if (iFrames[_index] == 0)
                {
                    _speed += _difference * Constants.EnemyStats.movementSpeed[(int)Enemy.type[_index]];
                    if (type[_index] == EnemyType.ARCHER && SightLine(position[_index], _distance))
                    {
                        if (abilityTimer[_index] < 0)
                        {
                            abilityTimer[_index] = Constants.Archer.attackDelay;
                            Projectile.create(Projectile.projectileType.ENEMY_PROJECTILE, position[_index], _difference, 7f, 1000, 10, 1, 10);
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
                    colorFilter[_index] = Color.DarkSalmon;
                }
                General.Movement(false, ref _position, new Vector2(collideRectangle[_index].Width, collideRectangle[_index].Height), ref _speed, Constants.EnemyStats.movementSpeed[(int)Enemy.type[_index]]);
                position[_index] = _position;
                speed[_index] = speed[_index] * 0.6f;


                collideRectangle[_index] = new Rectangle((int)position[_index].X - collideRectangle[_index].Width / 2, (int)position[_index].Y - collideRectangle[_index].Height / 2, collideRectangle[_index].Width, collideRectangle[_index].Height);
                
            }
        }
        public enum EnemyType
        {
            SMALL = 0,
            MEDIUM = 1,
            LARGE = 2,
            ARCHER = 3,
        }
        public static void Setup(Texture2D _texture)
        {
            textures = _texture;
        }
    }
}
