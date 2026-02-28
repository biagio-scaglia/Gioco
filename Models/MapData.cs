using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace SimplePlatformer.Models
{
    public class MapData
    {
        public Tile[,] Grid { get; set; }
        public int Rows { get; set; }
        public int Cols { get; set; }
        
        public Vector2 StartPosition { get; set; }
        public Rectangle FinishLine { get; set; }
        
        public List<Rectangle> Platforms { get; set; }
        public List<Rectangle> Checkpoints { get; set; }
        public HashSet<int> ReachedCheckpoints { get; set; }
        public List<Vector2> RingSpawns { get; set; }

        public MapData(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Grid = new Tile[rows, cols];
            Platforms = new List<Rectangle>();
            Checkpoints = new List<Rectangle>();
            ReachedCheckpoints = new HashSet<int>();
            RingSpawns = new List<Vector2>();
            
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    Grid[y, x] = new Tile(-1);
                }
            }
        }
    }
}
