using System.Numerics;
using System.Collections.Generic;
using Raylib_cs;

namespace SimplePlatformer
{
    enum GameState
    {
        MainMenu,
        Options,
        Gameplay
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
            
            Rectangle[] platforms = new Rectangle[] {
                new Rectangle(100, 500, 600, 40)
            };

            Texture2D backgroundTex = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"backgrounds\background.png"));
            Texture2D ringTexture = Raylib.LoadTexture(System.IO.Path.Combine(assets, @"items\ring.gif"));
            Sound ringSound = Raylib.LoadSound(System.IO.Path.Combine(assets, @"sounds\ring.mp3"));

            List<Ring> rings = new List<Ring>
            {
                new Ring(new Vector2(200, 450)),
                new Ring(new Vector2(300, 450)),
                new Ring(new Vector2(350, 400)),
                new Ring(new Vector2(500, 450))
            };

            int score = 0;

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
                            if (selectedMenuOption == 0)      currentState = GameState.Gameplay;
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
                        player.Update(dt, platforms);

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
                    
                    // Disegna lo sfondo per primo (dietro a tutto)
                    Raylib.DrawTexture(backgroundTex, 0, 0, Color.White);

                    foreach(var plat in platforms)
                    {
                        Raylib.DrawRectangleRec(plat, Color.DarkGray);
                    }
                    
                    // Disegna gli anelli
                    foreach (var ring in rings)
                    {
                        ring.Draw(ringTexture);
                    }
                    
                    player.Draw(dt);

                    // Disegna il Punteggio (Score)
                    Raylib.DrawText($"SCORE: {score}", 620, 20, 30, Color.Gold);
                }

                Raylib.EndDrawing();
            }

            // Scarica dalla memoria texture e suoni
            Raylib.UnloadTexture(backgroundTex);
            Raylib.UnloadTexture(ringTexture);
            Raylib.UnloadSound(ringSound);
            player.Unload();
            
            Raylib.CloseAudioDevice(); // Chiudi il sistema audio
            Raylib.CloseWindow();
        }
    }
}
