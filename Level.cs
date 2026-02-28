using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace SimplePlatformer
{
    public class Level
    {
        private Texture2D tileset;
        private int tileSize = 32;
        private int tileOriginalSize = 16;
        
        private Texture2D texStart;
        private Texture2D texCheckpointIdle;
        private Texture2D texCheckpointOut;

        public int[,] MapGrid;
        public Rectangle FinishLine { get; private set; }
        public List<Rectangle> Checkpoints { get; private set; }
        public HashSet<int> ReachedCheckpoints { get; private set; }
        
        public List<Rectangle> Platforms { get; private set; }

        public Level(string assetPath)
        {
            tileset = Raylib.LoadTexture(System.IO.Path.Combine(assetPath, @"sprites\world_tileset.png"));
            texStart = Raylib.LoadTexture(System.IO.Path.Combine(assetPath, @"Start\Start (Idle).png"));
            texCheckpointIdle = Raylib.LoadTexture(System.IO.Path.Combine(assetPath, @"Checkpoint\Checkpoint (Flag Idle)(64x64).png"));
            texCheckpointOut = Raylib.LoadTexture(System.IO.Path.Combine(assetPath, @"Checkpoint\Checkpoint (Flag Out) (64x64).png"));
            
            Platforms = new List<Rectangle>();
            Checkpoints = new List<Rectangle>();
            ReachedCheckpoints = new HashSet<int>();
            
            MapGrid = new int[19, 100];
            
            for (int x = 0; x < 100; x++)
            {
                if (x > 30 && x < 35) continue;
                if (x > 60 && x < 68) continue;

                MapGrid[15, x] = 1;
                MapGrid[16, x] = 2;
                MapGrid[17, x] = 2; 
                MapGrid[18, x] = 2;
            }
            
            MapGrid[12, 10] = 1; MapGrid[12, 11] = 1; MapGrid[12, 12] = 1;
            MapGrid[9, 17] = 1; MapGrid[9, 18] = 1;
            
            MapGrid[12, 25] = 1; MapGrid[12, 26] = 1;
            
            MapGrid[12, 32] = 1;
            MapGrid[11, 33] = 1;

            MapGrid[10, 42] = 1; MapGrid[10, 43] = 1;
            MapGrid[7, 48] = 1; MapGrid[7, 49] = 1;

            MapGrid[10, 62] = 1; MapGrid[8, 65] = 1;

            MapGrid[12, 72] = 1; MapGrid[12, 73] = 1;
            MapGrid[13, 80] = 1; MapGrid[13, 81] = 1; MapGrid[13, 82] = 1;

            Checkpoints.Add(new Rectangle(25 * tileSize, 13 * tileSize, 64, 64)); 
            Checkpoints.Add(new Rectangle(75 * tileSize, 13 * tileSize, 64, 64)); 

            FinishLine = new Rectangle(95 * tileSize, 0, 5 * tileSize, 600);

            GenerateCollisionData();
        }

        private void GenerateCollisionData()
        {
            Platforms.Clear();
            int rows = MapGrid.GetLength(0);
            int cols = MapGrid.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    if (MapGrid[y, x] > 0)
                    {
                        Platforms.Add(new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize));
                    }
                }
            }
        }

        public void Draw()
        {
            int rows = MapGrid.GetLength(0);
            int cols = MapGrid.GetLength(1);

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int tileId = MapGrid[y, x];
                    if (tileId > 0)
                    {
                        int texX = 0;
                        int texY = 0;

                        if (tileId == 1)
                        {
                            texX = 0; texY = 0;
                            Rectangle source = new Rectangle(texX, texY, tileOriginalSize, tileOriginalSize);
                            Rectangle dest = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                            Raylib.DrawTexturePro(tileset, source, dest, new Vector2(0,0), 0f, Color.White);
                        }
                        else if (tileId == 2)
                        {
                            texX = 0; texY = 16;
                            Rectangle source = new Rectangle(texX, texY, tileOriginalSize, tileOriginalSize);
                            Rectangle dest = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);
                            Raylib.DrawTexturePro(tileset, source, dest, new Vector2(0,0), 0f, Color.White);
                        }
                    }
                }
            }
            
            // Draw Start flag
            Rectangle sourceStart = new Rectangle(0, 0, 64, 64);
            Rectangle destStart = new Rectangle(5 * tileSize, 13 * tileSize, 64, 64);
            Raylib.DrawTexturePro(texStart, sourceStart, destStart, new Vector2(0, 0), 0f, Color.White);

            for (int i = 0; i < Checkpoints.Count; i++)
            {
                var cp = Checkpoints[i];
                Texture2D currentTex = ReachedCheckpoints.Contains(i) ? texCheckpointOut : texCheckpointIdle;
                Rectangle sourceRect = new Rectangle(0, 0, 64, 64);
                Rectangle destRect = new Rectangle(cp.X, cp.Y, 64, 64);
                Raylib.DrawTexturePro(currentTex, sourceRect, destRect, new Vector2(0,0), 0f, Color.White);
            }

            Raylib.DrawRectangleRec(FinishLine, new Color(0, 255, 0, 100));
        }

        public void Unload()
        {
            Raylib.UnloadTexture(tileset);
            Raylib.UnloadTexture(texStart);
            Raylib.UnloadTexture(texCheckpointIdle);
            Raylib.UnloadTexture(texCheckpointOut);
        }
    }
}
