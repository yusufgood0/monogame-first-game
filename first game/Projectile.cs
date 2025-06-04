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
using static first_game.Tiles;

namespace first_game
{
    public class Projectile
    {
        public static List<Vector2> speed = new();
        public static List<Vector2> position = new();
        public static List<float> height = new();
        public static List<projectileType> Type = new();
        public static Texture2D[] textures = new Texture2D[3];
        public static Rectangle[] textureRects = new Rectangle[3];
        public static List<List<int>> Iframes = new();
        public static List<List<int>> IframesEnemyIndex = new();
        public static List<int> pierce = new();
        public static List<int> damage = new();
        public static List<int> timeLimit = new();
        public enum projectileType
        {
            PLAYER_PROJECTILE = 0,
            ENEMY_PROJECTILE = 1,
            HOMING_PROJECTILE = 2
        }
        public static void Setup(object[] _textures)
        {
            for (int i = 0; i < _textures.Length; i++)
            {
                Projectile.textureRects[i] = (Rectangle)_textures[i];
            }
        }


        public static int[] collisionSizeData =
            {
                5,
                10,
                5
            };
        public static Color GetProjectileColor(int index)
        {
            switch (Type[index])
            {
                case projectileType.PLAYER_PROJECTILE:
                return Color.DarkOrchid;

                case projectileType.ENEMY_PROJECTILE:
                return Color.DarkViolet;

                case projectileType.HOMING_PROJECTILE:
                return Color.WhiteSmoke;
            }
            return Color.White;

        }
        public static void create(projectileType type, Vector2 spawnLocation, Vector2 angleVector, float projectileSpeed, int projectileLife, int _collisionSize, int _pierce, int _damage, int _timeLimit)
        {
            if (angleVector != new Vector2(0, 0))
            {
                angleVector.Normalize();
            }
            height.Add(Game1.PlayerHeight - Constants.floorLevel - 40);
            speed.Add(angleVector * projectileSpeed);
            position.Add(spawnLocation);
            Type.Add(type);
            Iframes.Add(new List<int>());
            IframesEnemyIndex.Add(new List<int>());
            pierce.Add(_pierce);
            damage.Add(_damage);
            timeLimit.Add(_timeLimit);
        }
        public static void update(int _index)
        {
            position[_index] += speed[_index];

            if (projectileType.HOMING_PROJECTILE == Projectile.Type[_index])
            {
                Vector2 difference = General.Difference(Player.position, Projectile.position[_index]);
                difference.Normalize();
                speed[_index] += difference;
            }


                for (int iFramesIndex = 0; iFramesIndex < Iframes[_index].Count; iFramesIndex++)
            {
                if (Iframes[_index][iFramesIndex] > 0)
                {
                    Iframes[_index][iFramesIndex] -= 1;
                }
                else
                {
                    Iframes[_index].RemoveAt(iFramesIndex);
                    IframesEnemyIndex[_index].RemoveAt(iFramesIndex);
                }
            }
            if (projectileType.PLAYER_PROJECTILE == Projectile.Type[_index])
                for (int EnemyIndex = 0; EnemyIndex < Enemy.health.Count; EnemyIndex++)
                {
                    if (!IframesEnemyIndex[_index].Contains(EnemyIndex) && new Rectangle((int)(position[_index].X - collisionSizeData[(int)Type[_index]]), (int)(position[_index].Y - collisionSizeData[(int)Type[_index]]), collisionSizeData[(int)Type[_index]] * 2, collisionSizeData[(int)Type[_index]] * 2).Intersects(Enemy.collideRectangle[EnemyIndex]))
                    {
                        Enemy.Push(damage[_index] * 2, speed[_index], EnemyIndex);
                        pierce[_index] -= 1;
                        if (!Enemy.TakeDamage(Color.Purple, damage[_index], 10, EnemyIndex)
                            || pierce[_index] >= 0
                            )
                        {
                            Iframes[_index].Add(15);
                            IframesEnemyIndex[_index].Add(EnemyIndex);
                        }

                    }
                }

            for (int TileIndex = 0; TileIndex < Tiles.numTiles; TileIndex++)
            {
                if (Tiles.collideRectangle[TileIndex].Contains(position[_index]))
                {
                    switch (Tiles.tileType[TileIndex])
                    {
                        case (int)tileTypes.BRICK:
                        Tiles.TakeDamage(Color.AliceBlue, damage[_index], 0, TileIndex);
                        pierce[_index] -= 1;
                        break;

                        case (int)tileTypes.SOLID:
                        kill(_index);
                        return;

                    }

                }
            }

            if (Player.iFrames <= 0 && Player.state != Player.State.Dashing && 
                projectileType.PLAYER_PROJECTILE != Projectile.Type[_index] && 
                !IframesEnemyIndex[_index].Contains(-1) && 
                new Rectangle((int)(position[_index].X - collisionSizeData[(int)Type[_index]]), (int)(position[_index].Y - collisionSizeData[(int)Type[_index]]), collisionSizeData[(int)Type[_index]] * 2, collisionSizeData[(int)Type[_index]] * 2).Intersects(new Rectangle((int)(Player.position.X - Player.width / 2), (int)(Player.position.Y - Player.height / 2), Player.width, Player.height)))
            {
                pierce[_index] -= 1;
                Player.TakeDamage(Color.Red, damage[_index], 10, 10, damage[_index] / 10, speed[_index]);
                Iframes[_index].Add(15);
                IframesEnemyIndex[_index].Add(-1);
            }
            timeLimit[_index] -= 1;
            if (pierce[_index] <= 0 || timeLimit[_index] <= 0) kill(_index);
        }
        public static void kill(int _index)
        {
            timeLimit.RemoveAt(_index);
            height.RemoveAt(_index);
            pierce.RemoveAt(_index);
            speed.RemoveAt(_index);
            position.RemoveAt(_index);
            Type.RemoveAt(_index);
            Iframes.RemoveAt(_index);
            IframesEnemyIndex.RemoveAt(_index);
            damage.RemoveAt(_index);
        }

    }
}
