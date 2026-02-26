using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace SimplePlatformer
{
    enum GameState
    {
        MainMenu,
        Options,
        Gameplay,
        GameOver,
        LevelComplete
    }

    class Program
    {
        static void Main()
        {
            Raylib.InitWindow(800, 600, "Knuckles Platformer .NET");
            Raylib.InitAudioDevice(); // Inizializza il sistema audio
            Raylib.SetTargetFPS(60);

            string basePath = System.AppDomain.CurrentDomain.BaseDirectory;
            string assets = System.IO.Path.Combine(basePath, @"..\..\..\assets\");
            if (!System.IO.Directory.Exists(assets)) 
            {
                 assets = @"assets\";
            }

            GameState currentState = GameState.MainMenu;
            int selectedMenuOption = 0;

            Player player = new Player(new Vector2(400, 300), assets);
            
            Level gameLevel = new Level(assets);

            Texture2D backgroundTex = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"backgrounds\background.png"));
            Texture2D ringTexture = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"items\ring.gif"));
            Sound ringSound = Raylib.LoadSound(System.IO.Path.Combine(assets, @"sounds\ring.mp3"));
            Font customFont = Raylib.LoadFont(System.IO.Path.Combine(assets, @"fonts\PixelOperator8-Bold.ttf"));

            List<Ring> rings = new List<Ring>
            {
                new Ring(new Vector2(300, 450)),
                new Ring(new Vector2(600, 250)),
                new Ring(new Vector2(1200, 400)),
                new Ring(new Vector2(2500, 450))
            };

            int score = 0;
            float timeRemaining = 60f; // 60 secondi
            
            // Camera setup
            Camera2D camera = new Camera2D();
            camera.Target = player.Hitbox.Position;
            camera.Offset = new Vector2(800 / 2.0f, 600 / 2.0f);
            camera.Rotation = 0.0f;
            camera.Zoom = 1.0f;

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
                                player = new Player(new Vector2(400, 300), assets); // Reset pos
                                foreach(var r in rings) r.IsCollected = false;
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
                        timeRemaining -= dt;
                        if (timeRemaining <= 0)
                        {
                            currentState = GameState.GameOver;
                        }

                        player.Update(dt, gameLevel.Platforms.ToArray());
                        
                        // Game Over se cade nel vuoto
                        if (player.Hitbox.Y > 800)
                        {
                            currentState = GameState.GameOver;
                        }
                        
                        // Aggiorniamo la camera per seguire il giocatore orizzontalmente
                        if (player.Hitbox.X > 400) camera.Target.X = player.Hitbox.X;
                        camera.Target.Y = 300; // Camera fissa in verticale

                        // Traguardo (Win Condition)
                        if (Raylib.CheckCollisionRecs(player.Hitbox, gameLevel.FinishLine))
                        {
                            currentState = GameState.LevelComplete;
                        }

                        // Controlla la collisione con gli anelli
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
                }

                if (currentState == GameState.MainMenu && selectedMenuOption == 2 && Raylib.IsKeyPressed(KeyboardKey.Enter))
                {
                    break;
                }

                Raylib.BeginDrawing();
                
                if (currentState == GameState.MainMenu)
                {
                    Raylib.ClearBackground(Color.DarkBlue);
                    Raylib.DrawText("KNUCKLES PLATFORM", 200, 150, 30, Color.White);
                    
                    Color colorNuovo = (selectedMenuOption == 0) ? Color.Red : Color.LightGray;
                    Color colorOpzioni = (selectedMenuOption == 1) ? Color.Red : Color.LightGray;
                    Color colorEsci = (selectedMenuOption == 2) ? Color.Red : Color.LightGray;

                    Raylib.DrawText((selectedMenuOption == 0 ? "> " : "") + "Nuovo Gioco", 300, 300, 25, colorNuovo);
                    Raylib.DrawText((selectedMenuOption == 1 ? "> " : "") + "Opzioni", 300, 350, 25, colorOpzioni);
                    Raylib.DrawText((selectedMenuOption == 2 ? "> " : "") + "Esci", 300, 400, 25, colorEsci);
                }
                else if (currentState == GameState.Options)
                {
                    Raylib.ClearBackground(Color.DarkPurple);
                    Raylib.DrawText("- OPZIONI -", 320, 100, 30, Color.White);
                    Raylib.DrawText("[Premi BACKSPACE per tornare indietro]", 200, 500, 20, Color.Gray);
                }
                else if (currentState == GameState.Gameplay)
                {
                    Raylib.ClearBackground(Color.SkyBlue);
                    
                    // Disegna lo sfondo per primo (Fisso)
                    Raylib.DrawTexture(backgroundTex, 0, 0, Color.White);

                    Raylib.BeginMode2D(camera);

                    // Disegna i Tile del Livello (si muovono con la camera)
                    gameLevel.Draw();
                    
                    // Disegna gli anelli
                    foreach (var ring in rings)
                    {
                        ring.Draw(ringTexture);
                    }
                    
                    player.Draw(dt);
                    
                    Raylib.EndMode2D(); // Fine oggetti che seguono la camera

                    // UI STATICA (non influenzata dalla camera)
                    Raylib.DrawTextEx(customFont, $"SCORE: {score}", new Vector2(20, 20), 24, 2, Color.Gold);
                    Raylib.DrawTextEx(customFont, $"TIME: {(int)timeRemaining}", new Vector2(520, 20), 24, 2, Color.Red);
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

                Raylib.EndDrawing();
            }

            // Scarica dalla memoria texture e suoni
            Raylib.UnloadFont(customFont);
            Raylib.UnloadTexture(backgroundTex);
            Raylib.UnloadTexture(ringTexture);
            Raylib.UnloadSound(ringSound);
            gameLevel.Unload();
            player.Unload();
            
            Raylib.CloseAudioDevice(); // Chiudi il sistema audio
            Raylib.CloseWindow();
        }
    }
}
