using System.Numerics;
using Raylib_cs;

namespace SimplePlatformer
{
    public class Player
    {
        public Rectangle Hitbox;
        public Vector2 Velocity;
        
        private Texture2D texIdle;
        private Texture2D texDown;
        private Texture2D[] texJump;
        private Texture2D[] texRun;

        private float frameTimer = 0f;
        private int currentFrame = 0;
        private bool facingRight = true;
        private bool isCrouching = false;
        public bool IsGrounded = false;

        private const float Gravity = 1000f;
        private const float JumpSpeed = -500f;
        private const float MoveSpeed = 300f;
        private const float Scale = 2.0f;

        public Player(Vector2 startPos, string assetsPath)
        {
            Hitbox = new Rectangle(startPos.X, startPos.Y, 40, 50);
            
            string spritesPath = System.IO.Path.Combine(assetsPath, @"sprites\");

            texIdle = Raylib.LoadTexture(spritesPath + "idle.png");
            texDown = Raylib.LoadTexture(spritesPath + "down.png");
            
            texJump = new Texture2D[] {
                Raylib.LoadTexture(spritesPath + "jump1.png"),
                Raylib.LoadTexture(spritesPath + "jump2.png")
            };

            texRun = new Texture2D[] {
                Raylib.LoadTexture(spritesPath + "run1.png"),
                Raylib.LoadTexture(spritesPath + "run2.png"),
                Raylib.LoadTexture(spritesPath + "run3.png"),
                Raylib.LoadTexture(spritesPath + "run4.png"),
                Raylib.LoadTexture(spritesPath + "run5.png"),
                Raylib.LoadTexture(spritesPath + "run6.png")
            };
        }

        public void Update(float dt, Rectangle[] platforms)
        {
            Velocity.X = 0;
            isCrouching = false;
            
            if (Raylib.IsKeyDown(KeyboardKey.Down) && IsGrounded)
            {
                isCrouching = true;
            }
            else
            {
                if (Raylib.IsKeyDown(KeyboardKey.Right)) 
                {
                    Velocity.X = MoveSpeed;
                    facingRight = true;
                }
                else if (Raylib.IsKeyDown(KeyboardKey.Left)) 
                {
                    Velocity.X = -MoveSpeed;
                    facingRight = false;
                }
            }

            Velocity.Y += Gravity * dt;

            if (Raylib.IsKeyPressed(KeyboardKey.Space) && IsGrounded && !isCrouching)
            {
                Velocity.Y = JumpSpeed;
                IsGrounded = false;
            }

            Hitbox.X += Velocity.X * dt;
            Hitbox.Y += Velocity.Y * dt;

            // Collisioni con i bordi dello schermo
            if (Hitbox.X < 0) 
            {
                Hitbox.X = 0;
            }
            if (Hitbox.X > 800 - Hitbox.Width) 
            {
                Hitbox.X = 800 - Hitbox.Width;
            }

            IsGrounded = false;
            foreach (var platform in platforms)
            {
                if (Raylib.CheckCollisionRecs(Hitbox, platform))
                {
                    if (Velocity.Y > 0)
                    {
                        Hitbox.Y = platform.Y - Hitbox.Height;
                        Velocity.Y = 0;
                        IsGrounded = true;
                    }
                }
            }

            if (Hitbox.Y > 600 - Hitbox.Height)
            {
                Hitbox.Y = 600 - Hitbox.Height;
                Velocity.Y = 0;
                IsGrounded = true;
            }
        }

        public void Draw(float dt)
        {
            frameTimer += dt;
            Texture2D currentTexture = texIdle;

            if (isCrouching)
            {
                currentTexture = texDown;
            }
            else if (!IsGrounded)
            {
                if (Velocity.Y < 0) 
                {
                    currentTexture = texJump[0];
                }
                else 
                {
                    currentTexture = texJump[1];
                }
            }
            else if (Velocity.X != 0)
            {
                if (frameTimer >= 0.08f) 
                {
                    frameTimer = 0f;
                    currentFrame = (currentFrame + 1) % texRun.Length;
                }
                if (currentFrame >= texRun.Length) currentFrame = 0;
                currentTexture = texRun[currentFrame];
            }
            else
            {
                currentTexture = texIdle;
                currentFrame = 0;
            }

            Rectangle sourceRect = new Rectangle(0, 0, currentTexture.Width, currentTexture.Height);
            if (!facingRight) sourceRect.Width = -sourceRect.Width;

            float drawWidth = currentTexture.Width * Scale;
            float drawHeight = currentTexture.Height * Scale;

            float drawX = Hitbox.X + (Hitbox.Width / 2) - (drawWidth / 2);
            float drawY = Hitbox.Y + Hitbox.Height - drawHeight;

            Rectangle destRect = new Rectangle(drawX, drawY, drawWidth, drawHeight);
            
            Raylib.DrawTexturePro(currentTexture, sourceRect, destRect, new Vector2(0,0), 0f, Color.White);
        }

        public void Unload()
        {
            Raylib.UnloadTexture(texIdle);
            Raylib.UnloadTexture(texDown);
            foreach(var t in texJump) Raylib.UnloadTexture(t);
            foreach(var t in texRun) Raylib.UnloadTexture(t);
        }
    }
}
