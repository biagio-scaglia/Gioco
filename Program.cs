using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;
using SimplePlatformer.Models;
using SimplePlatformer.Engine;

namespace SimplePlatformer
{
    enum GameState
    {
        MainMenu,
        Options,
        Gameplay,
        GameOver,
        LevelComplete,
        PauseMenu
    }

    class Program
    {
        static void Main()
        {
            Raylib.InitWindow(800, 600, "Knuckles Platformer .NET");
            Raylib.SetExitKey(KeyboardKey.Null);
            Raylib.InitAudioDevice();
            Raylib.SetTargetFPS(60);

            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string assets = System.IO.Path.Combine(basePath, @"..\..\..\assets\");
            if (!System.IO.Directory.Exists(assets)) 
            {
                 assets = @"assets\";
            }

            GameState currentState = GameState.MainMenu;
            int selectedMenuOption = 0;

            MapData mapData = LevelGenerator.CreateLevel1();
            MapRenderer mapRenderer = new MapRenderer(assets);
            Player player = new Player(mapData.StartPosition, assets);

            Texture2D backgroundTex = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"backgrounds\background.png"));
            Texture2D ringTexture = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"items\ring.gif"));
            Sound ringSound = Raylib.LoadSound(System.IO.Path.Combine(assets, @"sounds\ring.mp3"));
            Sound checkpointSound = Raylib.LoadSound(System.IO.Path.Combine(assets, @"sounds\sonic-checkpoint.mp3"));
            Font customFont = Raylib.LoadFont(System.IO.Path.Combine(assets, @"fonts\PixelOperator8-Bold.ttf"));

            Texture2D texBtnPlay = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"buttons\Play.png"));
            Texture2D texBtnSettings = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"buttons\Settings.png"));
            Texture2D texBtnClose = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"buttons\Close.png"));

            List<Ring> rings = new List<Ring>();
            foreach (var pos in mapData.RingSpawns)
            {
                rings.Add(new Ring(pos));
            }

            int score = 0;
            float timeRemaining = 60f;
            
            Camera2D camera = new Camera2D();
            camera.Target = player.Hitbox.Position;
            camera.Offset = new Vector2(800 / 2.0f, 600 / 2.0f);
            camera.Rotation = 0.0f;
            camera.Zoom = 1.0f;

            Vector2 lastCheckpoint = mapData.StartPosition;

            while (!Raylib.WindowShouldClose())
            {
                float dt = Raylib.GetFrameTime();

                switch (currentState)
                {
                    case GameState.MainMenu:
                        if (Raylib.IsKeyPressed(KeyboardKey.Down)) selectedMenuOption++;
                        if (Raylib.IsKeyPressed(KeyboardKey.Up)) selectedMenuOption--;

                        if (selectedMenuOption < 0) selectedMenuOption = 2;
                        if (selectedMenuOption > 2) selectedMenuOption = 0;

                        if (Raylib.IsKeyPressed(KeyboardKey.Enter))
                        {
                            if (selectedMenuOption == 0)      
                            {
                                score = 0;
                                timeRemaining = 60f;
                                player = new Player(lastCheckpoint, assets);
                                camera.Target = player.Hitbox.Position;
                                foreach(var r in rings) r.IsCollected = false;
                                mapData.ReachedCheckpoints.Clear();
                                currentState = GameState.Gameplay;
                            }
                            else if (selectedMenuOption == 1) currentState = GameState.Options;
                        }
                        break;

                    case GameState.Options:
                        if (Raylib.IsKeyPressed(KeyboardKey.Backspace))
                        {
                            currentState = GameState.MainMenu;
                        }
                        break;

                    case GameState.Gameplay:
                        if (Raylib.IsKeyPressed(KeyboardKey.Escape))
                        {
                            currentState = GameState.PauseMenu;
                            break;
                        }

                        timeRemaining -= dt;
                        if (timeRemaining <= 0)
                        {
                            currentState = GameState.GameOver;
                        }

                        player.Update(dt, mapData.Platforms.ToArray());
                        
                        if (player.Hitbox.Y > 800)
                        {
                            currentState = GameState.GameOver;
                        }
                        
                        if (player.Hitbox.X > 400) camera.Target.X = player.Hitbox.X;
                        camera.Target.Y = 300;

                        if (Raylib.CheckCollisionRecs(player.Hitbox, mapData.FinishLine))
                        {
                            currentState = GameState.LevelComplete;
                            lastCheckpoint = mapData.StartPosition;
                        }

                        for (int cpIdx = 0; cpIdx < mapData.Checkpoints.Count; cpIdx++)
                        {
                            var cp = mapData.Checkpoints[cpIdx];
                            if (Raylib.CheckCollisionRecs(player.Hitbox, cp))
                            {
                                if (mapData.ReachedCheckpoints.Add(cpIdx))
                                {
                                    Raylib.PlaySound(checkpointSound);
                                    lastCheckpoint = new Vector2(cp.X, cp.Y - 50);
                                }
                            }
                        }

                        foreach (var ring in rings)
                        {
                            if (!ring.IsCollected && Raylib.CheckCollisionRecs(player.Hitbox, ring.Hitbox))
                            {
                                ring.IsCollected = true;
                                Raylib.PlaySound(ringSound);
                                score += 10;
                            }
                        }
                        break;
                        
                    case GameState.GameOver:
                    case GameState.LevelComplete:
                        if (Raylib.IsKeyPressed(KeyboardKey.Enter)) currentState = GameState.MainMenu;
                        break;
                        
                    case GameState.PauseMenu:
                        if (Raylib.IsKeyPressed(KeyboardKey.Escape)) currentState = GameState.Gameplay;
                        if (Raylib.IsKeyPressed(KeyboardKey.Q)) currentState = GameState.MainMenu;
                        break;
                }

                if (currentState == GameState.MainMenu && selectedMenuOption == 2 && Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    break;
                }

                Raylib.BeginDrawing();
                
                if (currentState == GameState.MainMenu)
                {
                    Raylib.ClearBackground(new Color(20, 30, 60, 255));
                    
                    int titleWidth = Raylib.MeasureText("KNUCKLES PLATFORM", 40);
                    Raylib.DrawText("KNUCKLES PLATFORM", 400 - (titleWidth / 2), 100, 40, Color.Gold);
                    
                    string[] menuItems = { "Nuovo Gioco / Riprendi", "Opzioni", "Esci" };
                    Texture2D[] btnIcons = { texBtnPlay, texBtnSettings, texBtnClose };
                    int startY = 250;
                    int spacing = 80;

                    for (int i = 0; i < menuItems.Length; i++)
                    {
                        Rectangle btnRect = new Rectangle(250, startY + (i * spacing), 350, 50);
                        
                        if (selectedMenuOption == i)
                        {
                            Raylib.DrawRectangleRounded(btnRect, 0.5f, 10, Color.Red);
                            Raylib.DrawRectangleLinesEx(btnRect, 3, Color.White);
                            Raylib.DrawTexture(btnIcons[i], (int)btnRect.X + 15, (int)btnRect.Y + 9, Color.White);
                            Raylib.DrawText(menuItems[i], (int)btnRect.X + 60, (int)btnRect.Y + 15, 20, Color.White);
                        }
                        else
                        {
                            Raylib.DrawRectangleRounded(btnRect, 0.5f, 10, Color.DarkBlue);
                            Raylib.DrawTexture(btnIcons[i], (int)btnRect.X + 15, (int)btnRect.Y + 9, Color.White);
                            Raylib.DrawText(menuItems[i], (int)btnRect.X + 60, (int)btnRect.Y + 15, 20, Color.LightGray);
                        }
                    }
                }
                else if (currentState == GameState.Options)
                {
                    Raylib.ClearBackground(Color.DarkPurple);
                    Raylib.DrawText("- OPZIONI -", 320, 100, 30, Color.White);
                    
                    Raylib.DrawText("Volume: 100%", 300, 200, 25, Color.LightGray);
                    Raylib.DrawText("Schermo Intero: OFF", 300, 260, 25, Color.LightGray);
                    Raylib.DrawText("Difficoltà: Normale", 300, 320, 25, Color.LightGray);

                    Raylib.DrawText("[Premi BACKSPACE per tornare indietro]", 200, 500, 20, Color.Gray);
                }
                else if (currentState == GameState.Gameplay)
                {
                    Raylib.ClearBackground(Color.SkyBlue);
                    
                    Raylib.DrawTexture(backgroundTex, 0, 0, Color.White);

                    Raylib.BeginMode2D(camera);

                    mapRenderer.Draw(mapData);
                    
                    foreach (var ring in rings)
                    {
                        ring.Draw(ringTexture);
                    }
                    
                    player.Draw(dt);
                    
                    Raylib.EndMode2D();

                    Raylib.DrawRectangleRounded(new Rectangle(10, 10, 220, 40), 0.5f, 10, new Color(0, 0, 0, 150));
                    Raylib.DrawRectangleRounded(new Rectangle(510, 10, 240, 40), 0.5f, 10, new Color(0, 0, 0, 150));

                    Raylib.DrawTextEx(customFont, $"SCORE: {score}", new Vector2(20, 20), 24, 2, Color.Gold);
                    Raylib.DrawTextEx(customFont, $"TIME: {(int)timeRemaining}", new Vector2(530, 20), 24, 2, Color.Red);
                }
                else if (currentState == GameState.GameOver)
                {
                    Raylib.ClearBackground(Color.Black);
                    Raylib.DrawTextEx(customFont, "GAME OVER", new Vector2(250, 250), 40, 2, Color.Red);
                    Raylib.DrawTextEx(customFont, "Premi INVIO", new Vector2(300, 350), 20, 2, Color.White);
                }
                else if (currentState == GameState.LevelComplete)
                {
                    Raylib.ClearBackground(Color.DarkGreen);
                    Raylib.DrawTextEx(customFont, "LIVELLO COMPLETATO!", new Vector2(50, 250), 35, 2, Color.Gold);
                    Raylib.DrawTextEx(customFont, $"Punti totali: {score}", new Vector2(250, 320), 24, 2, Color.White);
                    Raylib.DrawTextEx(customFont, "Premi INVIO", new Vector2(300, 400), 20, 2, Color.LightGray);
                }
                else if (currentState == GameState.PauseMenu)
                {
                    Raylib.ClearBackground(new Color(0, 0, 0, 200));
                    Raylib.DrawText("- PAUSA -", 340, 150, 30, Color.White);
                    
                    Raylib.DrawText("Volume: 100%", 320, 250, 25, Color.LightGray);
                    Raylib.DrawText("Comandi:", 200, 320, 20, Color.Red);
                    Raylib.DrawText("- Frecce Destra/Sinistra per muoversi", 200, 360, 20, Color.Gold);
                    Raylib.DrawText("- Spazio per saltare", 200, 400, 20, Color.Gold);
                    
                    Raylib.DrawText("[Premi ESC per Riprendere]", 250, 500, 20, Color.White);
                    Raylib.DrawText("[Premi Q per Uscire al Menu]", 250, 540, 20, Color.Gray);
                }

                Raylib.EndDrawing();
            }

            Raylib.UnloadFont(customFont);
            Raylib.UnloadTexture(backgroundTex);
            Raylib.UnloadTexture(ringTexture);
            Raylib.UnloadTexture(texBtnPlay);
            Raylib.UnloadTexture(texBtnSettings);
            Raylib.UnloadTexture(texBtnClose);
            Raylib.UnloadSound(ringSound);
            Raylib.UnloadSound(checkpointSound);
            mapRenderer.Unload();
            player.Unload();
            
            Raylib.CloseAudioDevice();
            Raylib.CloseWindow();
        }
    }
}
