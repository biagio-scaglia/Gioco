using System.Numerics;
using Raylib_cs;
using SimplePlatformer.Models;

namespace SimplePlatformer.Engine
{
    public class MapRenderer
    {
        private Texture2D _tileset;
        private Texture2D _texStart;
        private Texture2D _texCheckpointIdle;
        private Texture2D _texCheckpointOut;
        private int _tileOriginalSize = 16;
        private int _tileSize = LevelGenerator.TileSize;

        public MapRenderer(string assetPath)
        {
            _tileset = Raylib.LoadTexture(System.IO.Path.Combine(assetPath, @"sprites\world_tileset.png"));
            _texStart = Raylib.LoadTexture(System.IO.Path.Combine(assetPath, @"Start\Start (Idle).png"));
            _texCheckpointIdle = Raylib.LoadTexture(System.IO.Path.Combine(assetPath, @"Checkpoint\Checkpoint (Flag Idle)(64x64).png"));
            _texCheckpointOut = Raylib.LoadTexture(System.IO.Path.Combine(assetPath, @"Checkpoint\Checkpoint (Flag Out) (64x64).png"));
        }

        public void Draw(MapData map)
        {
            for (int y = 0; y < map.Rows; y++)
            {
                for (int x = 0; x < map.Cols; x++)
                {
                    int tileId = map.Grid[y, x].Id;
                    if (tileId >= 0)
                    {
                        int tilesPerRow = _tileset.Width / _tileOriginalSize;
                        if (tilesPerRow <= 0) tilesPerRow = 1;

                        int texX = (tileId % tilesPerRow) * _tileOriginalSize;
                        int texY = (tileId / tilesPerRow) * _tileOriginalSize;

                        Rectangle source = new Rectangle(texX, texY, _tileOriginalSize, _tileOriginalSize);
                        Rectangle dest = new Rectangle(x * _tileSize, y * _tileSize, _tileSize, _tileSize);
                        Raylib.DrawTexturePro(_tileset, source, dest, Vector2.Zero, 0f, Color.White);
                    }
                }
            }

            Rectangle sourceStart = new Rectangle(0, 0, 64, 64);
            Rectangle destStart = new Rectangle(map.StartPosition.X, map.StartPosition.Y - 64, 64, 64);
            Raylib.DrawTexturePro(_texStart, sourceStart, destStart, Vector2.Zero, 0f, Color.White);

            for (int i = 0; i < map.Checkpoints.Count; i++)
            {
                var cp = map.Checkpoints[i];
                Texture2D currentTex = map.ReachedCheckpoints.Contains(i) ? _texCheckpointOut : _texCheckpointIdle;
                Rectangle sourceRect = new Rectangle(0, 0, 64, 64);
                Rectangle destRect = new Rectangle(cp.X, cp.Y, 64, 64);
                Raylib.DrawTexturePro(currentTex, sourceRect, destRect, Vector2.Zero, 0f, Color.White);
            }

            Raylib.DrawRectangleRec(map.FinishLine, new Color(0, 255, 0, 100));
        }

        public void Unload()
        {
            Raylib.UnloadTexture(_tileset);
            Raylib.UnloadTexture(_texStart);
            Raylib.UnloadTexture(_texCheckpointIdle);
            Raylib.UnloadTexture(_texCheckpointOut);
        }
    }
}
