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
using static first_game.Projectile;
using static first_game.Tiles;

namespace first_game
{
    public class Enemy
    {
        static Random rnd = new Random();
        public static void respawn_enemies()
        {
            int _tileIndex;
            int numTargets = Enemy.collideRectangle.Count;

            for (int _enemyIndex = Enemy.collideRectangle.Count - 1; _enemyIndex >= 0; _enemyIndex--)
            {

                _tileIndex = randomTileIndex();
                Enemy.create(new Vector2(Tiles.collideRectangle[_tileIndex].X + Tiles.tileXY / 2, Tiles.collideRectangle[_tileIndex].Y + Tiles.tileXY / 2), Enemy.type[_enemyIndex]);
                while (checkEnemyTileCollision(Enemy.collideRectangle.Count - 1))
                {
                    _tileIndex = randomTileIndex();
                    kill(Enemy.collideRectangle.Count - 1);
                    Enemy.create(new Vector2(Tiles.collideRectangle[_tileIndex].X + Tiles.tileXY / 2, Tiles.collideRectangle[_tileIndex].Y + Tiles.tileXY / 2), Enemy.type[_enemyIndex]);
                }
                kill(_enemyIndex);

            }   
            _tileIndex = randomTileIndex();
            Player.position = new Vector2(Tiles.collideRectangle[_tileIndex].X + Tiles.tileXY / 2, Tiles.collideRectangle[_tileIndex].Y + Tiles.tileXY / 2);

        }
        public static bool checkEnemyTileCollision(int _index)
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
        public static int randomTileIndex()
        {
            int _tileIndex = rnd.Next(Tiles.numTiles - 1);
            while (Tiles.tileType[_tileIndex] != (int)Tiles.tileTypes.NONE)
            {
                _tileIndex = rnd.Next(Tiles.numTiles - 1);
            }
            return _tileIndex;
        }
        public static void push(float _knockback, Vector2 _Angle, int _index)
        {
            if (_Angle != new Vector2(0, 0))
            {
                _Angle.Normalize();
                position[_index] += _Angle * _knockback;
            }
        }
        public static bool TakeDamage(Color _color, int _damage, int _iFrames, int _index)
        {
            swingIFrames[_index] = _iFrames;
            health[_index] -= _damage;
            if (health[_index] <= 0)
            {
                kill(_index);
                return true;
            }
            else return false;
        }
        public static void kill(int _index)
        {
            for (int ProjectileIndex = 0; ProjectileIndex < Projectile.Iframes.Count; ProjectileIndex++)
            {
                Projectile.Iframes[ProjectileIndex].Remove(_index);
                Projectile.IframesEnemyIndex[ProjectileIndex].Remove(_index);
            }

            damage.RemoveAt(_index);
            movementSpeed.RemoveAt(_index);
            textureRectangle.RemoveAt(_index);
            collideRectangle.RemoveAt(_index);
            position.RemoveAt(_index);
            target.RemoveAt(_index);
            health.RemoveAt(_index);
            swingIFrames.RemoveAt(_index);
            type.RemoveAt(_index);

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
        public static List<EnemyType> type = new List<EnemyType>();


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
            Vector2 _newTargetAngle = new Vector2(0, 0);
            if (SightLine(position[_index], _distance))
            {
                target[_index] = Player.position;
            }
            else if (rnd.Next(100) == 1)
            {
                
                while (_newTargetAngle == new Vector2(0, 0)) _newTargetAngle = new Vector2(rnd.Next(-50, 50), rnd.Next(-50, 50));
                _newTargetAngle.Normalize();

                target[_index] = position[_index] + _newTargetAngle * rnd.Next(1, 3) * movementSpeed[_index];
            }
            //else if (rnd.Next(50) == 1)
            //{
            //    break;
            //}
            

            if (_distance > 10)
            {
                Vector2 _position = position[_index];
                General.movement(ref _position, collideRectangle[_index].Width, ref _difference, Enemy.movementSpeed[_index] / 10f, Enemy.collideRectangle[_index]);
                position[_index] = _position;

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
                movementSpeed.Add(35);
                damage.Add(40);
                health.Add(50);
                height = 24;
                width = 24;
            }
            if (_EnemyType == EnemyType.MEDIUM)
            {
                movementSpeed.Add(27);
                damage.Add(100);
                health.Add(150);
                height = 40;
                width = 40;
            }
            if (_EnemyType == EnemyType.LARGE)
            {
                movementSpeed.Add(20);
                damage.Add(200);
                health.Add(300);
                height = 80;
                width = 80;
            }

            Vector2 spawnLocation = new Vector2((int)_spawnLocation.X - width / 2, (int)_spawnLocation.Y - height / 2);
            textureRectangle.Add(new Rectangle(0, 0, width, height));
            position.Add(new Vector2((int)spawnLocation.X, (int)spawnLocation.Y));
            collideRectangle.Add(new Rectangle((int)spawnLocation.X, (int)spawnLocation.Y, width, height));
            target.Add(spawnLocation - new Vector2(0, 1));
            swingIFrames.Add(1);
            type.Add(_EnemyType);
        }
        public static void Setup(Texture2D _enemy)
        {
            textures = _enemy;
        }
    }
}
