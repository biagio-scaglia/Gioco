using System.Numerics;
using Raylib_cs;

namespace SimplePlatformer
{
    public class Ring
    {
        public Rectangle Hitbox;
        public bool IsCollected = false;
        
        public Ring(Vector2 position)
        {
            // Dimensione indicativa dell'anello
            Hitbox = new Rectangle(position.X, position.Y, 30, 30);
        }

        public void Draw(Texture2D texture)
        {
            if (!IsCollected)
            {
                Rectangle sourceRect = new Rectangle(0, 0, texture.Width, texture.Height);
                Rectangle destRect = new Rectangle(Hitbox.X, Hitbox.Y, Hitbox.Width, Hitbox.Height);
                Raylib.DrawTexturePro(texture, sourceRect, destRect, new Vector2(0, 0), 0f, Color.White);
            }
        }
    }
}
