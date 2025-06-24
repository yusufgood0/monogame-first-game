using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static first_game.Projectile;
using static first_game.Tiles;

namespace first_game
{
    public class Enemy
    {
        public static List<int> livingEnemies()
        {
            List<int> list = new();
            for (int i = 0; i < Enemy.health.Count; i++)
            {
                if (!isDead[i])
                    list.Add(i);
            }
            return list;
        }
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
        public static bool IsEnemyCollide(Vector2 point, float radius, int _index)
        {
            if (Constants.EnemyStats.circle[(int)Enemy.enemyType[_index]])
            {
                if (Vector2.Distance(point, Enemy.position[_index]) < radius)
                {
                    return true;
                }
            }
            else if (General.CircleCollisionBool(point, radius, Enemy.collideRectangle[_index]))
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
                Enemy.position[_enemyIndex] = General.RectangleToVector2(Tiles.collideRectangle[Tiles.RandomTile(tileTypes.NONE)]);
                while (SightLine(position[_enemyIndex]))
                {
                    Enemy.position[_enemyIndex] = General.RectangleToVector2(Tiles.collideRectangle[Tiles.RandomTile(tileTypes.NONE)]);
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
                            Tiles.TakeDamage(Color.Red, Constants.EnemyStats.damage[(int)enemyType[_index]], 10, tileIndex);
                            Tiles.loadTiles(tileIndex, tileIndex);
                            return true;

                        case (int)Tiles.tileTypes.GATE:
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
            if (!isDead[_index])
            {
                colorFilter[_index] = _color;
                iFrames[_index] = _iFrames;
                health[_index] -= _damage;
                Game1.punchHit.Play(.5f * Game1.sfxVolume, 0, 0);

                if (health[_index] <= 0)
                {
                    Game1.EnemyDeath.Play(.6f * Game1.sfxVolume, 0, 0);
                    Enemy.Kill(_index);
                    return true;
                }
                else return false;
            }
            return true;
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
            isDead.Add(false);
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
        public static void DeleteAll()
        {
            for (int _index = health.Count - 1; _index >= 0; _index--)
            {
                Delete(_index);
            }
        }
        public static void Kill(int _index)
        {
            isDead[_index] = true;
            colorFilter[_index] = Color.White;
            switch (enemyType[_index])
            {
                case EnemyType.SMALL:
                    textureRectangle[_index] = new(0, 480, 75, 79);
                    break;
                case EnemyType.MEDIUM:
                    textureRectangle[_index] = new(0, 635, 85, 75);
                    break;
                case EnemyType.LARGE:
                    textureRectangle[_index] = new(10, 632, 132, 102);
                    break;
                case EnemyType.ARCHER:
                    textureRectangle[_index] = new(0, 640, 160, 160);
                    break;
            }

        }
        public static void Delete(int _index)
        {
            //Game1.playerLightEmit -= Constants.maxPlayerLightEmit / 200 * Constants.EnemyStats.health[(int)Enemy.enemyType[_index]];
            //if (Game1.playerLightEmit >= Constants.maxPlayerLightEmit)
            //    Game1.playerLightEmit = Constants.maxPlayerLightEmit;

            for (int ProjectileIndex = 0; ProjectileIndex < Projectile.Iframes.Count; ProjectileIndex++)
            {
                Projectile.Iframes[ProjectileIndex].Remove(_index);
                Projectile.IframesEnemyIndex[ProjectileIndex].Remove(_index);
            }
            isDead.RemoveAt(_index);
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

        public static Texture2D[] textures = new Texture2D[5];
        public static Vector2[] textureArray = new Vector2[5];
        public static Vector2[] visualTextureSize = new Vector2[5];

        public static List<bool> isDead = new();
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
            float _detail = General.DistanceFromPoints(_point, Player.position) / 10;
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
        public static void Update(int _index, SpriteBatch spriteBatch)
        {
            //if (Vector2.Distance(speed[_index], Vector2.Zero) > 300)
            //{
            //    speed[_index].Normalize();
            //    speed[_index] *= 300;
            //}

            if (Enemy.position[_index].X < 0 || Enemy.position[_index].X > Tiles.tileXY * Tiles.columns || Enemy.position[_index].Y < 0 || Enemy.position[_index].Y > Tiles.tileXY * Tiles.rows)
            {
                Enemy.position[_index] = General.RectangleToVector2(Tiles.collideRectangle[Tiles.RandomTile(tileTypes.NONE)]);
            }

            if (!isDead[_index])
            {
                Vector2 _difference = General.Difference(target[_index], position[_index]); //X and Y difference 
                if (_difference == new Vector2(0, 0))
                {
                    return;
                }
                float _distance = General.DistanceFromDifference(_difference); //hypotinuse/distance to target
                _difference.Normalize();


                Vector2 _speed = speed[_index] / 7;
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
                        case EnemyType.BOSS:
                            if (_distance <= 10)
                            {
                                Vector2 differenceToPlayer = General.Difference(Player.position, position[_index]);
                                float angleToPlayer = General.Vector2ToAngle(differenceToPlayer);
                                angleToPlayer += .3f;
                                target[_index] = Player.position + (General.DistanceFromDifference(differenceToPlayer) + 10) * new Vector2((float)Math.Cos(angleToPlayer), (float)Math.Sin(angleToPlayer));

                                //differenceToPlayer.Normalize();
                                //target[_index] = differenceToPlayer * 20;
                            }
                            if (Vector2.Distance(target[_index], Player.position) < Tiles.tileXY * 7.5f || rnd.Next(20) == 1)
                            {
                                Point position = Tiles.collideRectangle[Tiles.RandomTile(tileTypes.NONE)].Center;
                                target[_index] = new(position.X, position.Y);
                            }
                            if ((Game1.LightLevel > Constants.maxLightLevel/3 && rnd.Next(100) == 1) || (rnd.Next(60+80 * health[_index]/Constants.EnemyStats.health[(int)enemyType[_index]]) == 1))
                            {
                                for (int x = -1; x < 1; x++)
                                    for (int y = -1; y < 1; y++)
                                    {
                                        Create(position[_index] + new Vector2(Tiles.tileXY * x, Tiles.tileXY * y), (EnemyType)(rnd.Next(0, 4)));
                                        foreach (Rectangle rect in Tiles.collideRectangle)
                                        {
                                            if (Enemy.CheckEnemyTileCollision(Enemy.health.Count - 1))
                                            {
                                                Enemy.Delete(Enemy.health.Count - 1);
                                            }
                                        }
                                    }

                            }
                            break;
                        default:
                            if (rnd.Next(20) == 1)
                                target[_index] = position[_index] + _newTargetAngle * rnd.Next(10, 30) * Constants.EnemyStats.movementSpeed[(int)Enemy.enemyType[_index]];
                            break;
                    }
                    if (_sightline && EnemyType.BOSS != enemyType[_index])
                    {
                        colorFilter[_index] = Color.Coral;
                        target[_index] = Player.position;
                    }

                }
                if (_distance > 10)
                {
                    if (iFrames[_index] <= 0)
                    {
                        if (_sightline)
                        {
                            colorFilter[_index] = Color.White;
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
                                            10,
                                            General.Difference(Player.position, Enemy.position[_index]),
                                            7f,
                                            1000,
                                            10,
                                            1,
                                            Constants.Archer.archerDamage,
                                            3000);
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
                            case EnemyType.BOSS:

                                if (abilityTimer[_index] <= 0)
                                {
                                    Projectile.create(projectileType.HOMING_PROJECTILE,
                                        position[_index],
                                        rnd.Next(-1000, 500),
                                        new((float)rnd.NextDouble() - .5f, (float)rnd.NextDouble() - .5f),
                                        (float)rnd.Next(10, 40),
                                        4,
                                        0,
                                        1,
                                        50,
                                        100
                                        );
                                    abilityTimer[_index] += rnd.Next(1, 8);
                                }
                                else
                                {
                                    abilityTimer[_index] -= 1;
                                }

                                break;
                        }
                    }
                    Vector2 _position = position[_index];
                    if (Constants.EnemyStats.circle[(int)enemyType[_index]])
                    {
                        General.CircleMovement(false, ref _position, Enemy.collideRectangle[_index].Width / 2, ref _speed, Constants.EnemyStats.movementSpeed[(int)Enemy.enemyType[_index]]);
                        position[_index] = _position;
                        for (int i = 0; i < Enemy.health.Count; i++)
                        {
                            if (!isDead[i] && IsEnemyCollide(position[_index], Enemy.collideRectangle[_index].Width, i) && i != _index)
                            {
                                Enemy.speed[_index] += General.Difference(position[_index], Enemy.position[i]);
                            }
                        }
                    }
                    else
                    {
                        General.RectMovement(false, ref _position, new Vector2(collideRectangle[_index].Width, collideRectangle[_index].Height), ref _speed, Constants.EnemyStats.movementSpeed[(int)Enemy.enemyType[_index]]);
                        position[_index] = _position;
                        for (int i = 0; i < Enemy.health.Count; i++)
                        {
                            if (!isDead[i] && IsEnemyCollide(General.Vector2toRectangle(Enemy.position[_index], Enemy.collideRectangle[_index].Width, Enemy.collideRectangle[_index].Height), i) && i != _index)
                            {
                                Enemy.speed[_index] += General.Difference(position[_index], Enemy.position[i]);
                            }
                        }
                    }
                    speed[_index] *= 0.6f;
                    collideRectangle[_index] = new Rectangle((int)position[_index].X - collideRectangle[_index].Width / 2, (int)position[_index].Y - collideRectangle[_index].Height / 2, collideRectangle[_index].Width, collideRectangle[_index].Height);
                    collideRectangle[_index] = General.Vector2toRectangle(position[_index], collideRectangle[_index].Width, collideRectangle[_index].Height);
                }
            }
        }
        public static void UpdateTexture(int _index)
        {
            switch (enemyType[_index])
            {
                case EnemyType.SMALL:
                    if (isDead[_index])
                    {
                        textureRectangle[_index] = new(Math.Min(textureRectangle[_index].X + textureRectangle[_index].Width, 375), textureRectangle[_index].Y, textureRectangle[_index].Width, textureRectangle[_index].Height);
                    }
                    else
                    {
                        textureRectangle[_index] = new(43, textureRectangle[_index].Y + textureRectangle[_index].Height, 75, 79);
                        if (textureRectangle[_index].Y >= textureRectangle[_index].Height * 6)
                        {
                            textureRectangle[_index] = new(textureRectangle[_index].X, 0, textureRectangle[_index].Width, textureRectangle[_index].Height);
                        }
                    }
                    break;
                case EnemyType.MEDIUM:
                    if (isDead[_index])
                    {
                        textureRectangle[_index] = new(Math.Min(textureRectangle[_index].X + textureRectangle[_index].Width, 425 - 85), textureRectangle[_index].Y, textureRectangle[_index].Width, textureRectangle[_index].Height);
                    }
                    else
                    {
                        textureRectangle[_index] = new(40, textureRectangle[_index].Y + textureRectangle[_index].Height, 64, 76);
                        if (textureRectangle[_index].Y >= textureRectangle[_index].Height * 5)
                        {
                            textureRectangle[_index] = new(textureRectangle[_index].X, 20, textureRectangle[_index].Width, textureRectangle[_index].Height);
                        }
                    }
                    break;
                case EnemyType.LARGE:
                    if (isDead[_index])
                    {
                        textureRectangle[_index] = new(Math.Min(textureRectangle[_index].X + textureRectangle[_index].Width, 519), textureRectangle[_index].Y, textureRectangle[_index].Width, textureRectangle[_index].Height);
                    }
                    else
                    {
                        textureRectangle[_index] = new(10, textureRectangle[_index].Y + textureRectangle[_index].Height, 130, 102);
                        if (textureRectangle[_index].Y >= textureRectangle[_index].Height * 5)
                        {
                            textureRectangle[_index] = new(textureRectangle[_index].X, 20, textureRectangle[_index].Width, textureRectangle[_index].Height);
                        }
                    }
                    break;
                case EnemyType.ARCHER:

                    textureRectangle[_index] = new(textureRectangle[_index].X + textureRectangle[_index].Width, textureRectangle[_index].Y, 160, 160);
                    if (textureRectangle[_index].X >= textureRectangle[_index].Width * 4)
                    {
                        if (!isDead[_index] && textureRectangle[_index].Y >= textureRectangle[_index].Height * 3)
                        {
                            textureRectangle[_index] = new(0, 0, textureRectangle[_index].Width, textureRectangle[_index].Height);
                        }
                        textureRectangle[_index] = new(0, textureRectangle[_index].Y + textureRectangle[_index].Height, textureRectangle[_index].Width, textureRectangle[_index].Height);
                    }

                    break;
            }
        }
        public enum EnemyType
        {
            SMALL = 0,
            MEDIUM = 1,
            LARGE = 2,
            ARCHER = 3,
            BOSS = 4,
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
