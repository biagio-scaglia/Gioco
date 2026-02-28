using System.Numerics;
using Raylib_cs;
using SimplePlatformer.Models;

namespace SimplePlatformer.Engine
{
    public static class LevelGenerator
    {
        public const int TileSize = 32;

        public static MapData CreateLevel1()
        {
            var map = new MapData(19, 100);

            map.StartPosition = new Vector2(400, 300);
            
            for (int x = 0; x < 100; x++)
            {
                if (x > 30 && x < 35) continue;
                if (x > 60 && x < 68) continue;

                map.Grid[15, x].Id = 0;
                map.Grid[16, x].Id = 1;
                map.Grid[17, x].Id = 1;
                map.Grid[18, x].Id = 1;
            }

            map.Grid[12, 10].Id = 0; map.Grid[12, 11].Id = 0; map.Grid[12, 12].Id = 0;
            map.Grid[9, 17].Id = 0; map.Grid[9, 18].Id = 0;

            map.Grid[12, 25].Id = 0; map.Grid[12, 26].Id = 0;

            map.Grid[12, 32].Id = 0;
            map.Grid[11, 33].Id = 0;

            map.Grid[10, 42].Id = 0; map.Grid[10, 43].Id = 0;
            map.Grid[7, 48].Id = 0; map.Grid[7, 49].Id = 0;

            map.Grid[10, 62].Id = 0; map.Grid[8, 65].Id = 0;

            map.Grid[12, 72].Id = 0; map.Grid[12, 73].Id = 0;
            map.Grid[13, 80].Id = 0; map.Grid[13, 81].Id = 0; map.Grid[13, 82].Id = 0;

            for (int y = 0; y < map.Rows; y++)
            {
                for (int x = 0; x < map.Cols; x++)
                {
                    if (map.Grid[y, x].Id >= 0)
                    {
                        map.Platforms.Add(new Rectangle(x * TileSize, y * TileSize, TileSize, TileSize));
                    }
                }
            }

            map.Checkpoints.Add(new Rectangle(25 * TileSize, 13 * TileSize - 64, 64, 64));
            map.Checkpoints.Add(new Rectangle(75 * TileSize, 13 * TileSize - 64, 64, 64));

            map.FinishLine = new Rectangle(95 * TileSize, 0, 5 * TileSize, 600);

            map.RingSpawns.Add(new Vector2(300, 450));
            map.RingSpawns.Add(new Vector2(600, 250));
            map.RingSpawns.Add(new Vector2(850, 380));
            map.RingSpawns.Add(new Vector2(1000, 200));
            map.RingSpawns.Add(new Vector2(1200, 400));
            map.RingSpawns.Add(new Vector2(1800, 350));
            map.RingSpawns.Add(new Vector2(2500, 450));
            map.RingSpawns.Add(new Vector2(2650, 400));
            map.RingSpawns.Add(new Vector2(2800, 350));

            return map;
        }
    }
}
