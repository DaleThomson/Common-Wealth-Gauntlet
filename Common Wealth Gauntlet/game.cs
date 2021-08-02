using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using gamelib2d;

namespace Commonwealth_Guantlet
{
    public class game
    {
        //Declarations
        int displaywidth;
        int displayheight;
        float gameruntime;      // Time since game started
        graphic2d[] background = new graphic2d[4];
        graphic2d[,] sbackground = new graphic2d[4, 6];
        graphic2d CharBackground;
        List<sprite2d> obstacles = new List<sprite2d>();
        Random randomiser = new Random();       // Variable to generate random numbers
        Texture2D GoldTrophy;
        Texture2D SilverTrophy;
        Texture2D BronzeTrophy;
        SoundEffect Footsteps;
        SoundEffect Splash;
        SoundEffect Gunshot;
        Song Crowd;

        private Camera2D[] cam = new Camera2D[4];
        private const int camspeed = 5;

        private Viewport[] viewports = new Viewport[5];

        private animatedsprite[] player = new animatedsprite[4];
        private animation Clyde;

        int currentPosition = 0;
        public int LevelCounter;
        const int maxlevels = 2;
        Boolean endrace = false;
        Boolean gameover = false;

        int glevel = 525;
        int gap = 50;
        int startx = 450;

        public game(GraphicsDeviceManager graphics, ContentManager Content, int dwidth, int dheight)
        {
            //Setting the display width, displayheight, Resetting the LevelCounter and a gap between the viewports
            displaywidth = dwidth;
            displayheight = dheight;
            LevelCounter = 1;
            const int margin = 10;

            // Content loaded in for the game
            CharBackground = new graphic2d(Content, "CharacterSelectionScreen", displaywidth, displayheight);
            CharBackground.stretch2fit(displaywidth, displayheight);
            Footsteps = Content.Load<SoundEffect>("Footsteps");
            Splash = Content.Load<SoundEffect>("Splashing");
            Gunshot = Content.Load<SoundEffect>("Gunshot");
            Crowd = Content.Load<Song>("Crowd");
            GoldTrophy = Content.Load<Texture2D>("GoldTrophy");
            SilverTrophy = Content.Load<Texture2D>("SilverTrophy");
            BronzeTrophy = Content.Load<Texture2D>("BronzeTrophy");
            Clyde = new animation(Content, "Clyde", 500, 600, 0.20f, Color.White, true, 24, 5, 5, true, false, false);

            // Viewport Settings
            viewports[0] = graphics.GraphicsDevice.Viewport;

            viewports[1] = viewports[0];
            viewports[1].Width = (viewports[0].Width - margin);
            viewports[1].Height = (viewports[0].Height - margin) / 4;
            viewports[1].X = 0;
            viewports[1].Y = 0;

            viewports[2] = viewports[1];
            viewports[2].Y += viewports[1].Height + margin;

            viewports[3] = viewports[2];
            viewports[3].Y += viewports[2].Height + margin;

            viewports[4] = viewports[3];
            viewports[4].Y += viewports[3].Height + margin;

            // Sets Positions for players correctly when they spawn
            for (int f = 0; f < player.Count(); f++)
            {
                player[f] = new animatedsprite(new Vector3(startx, glevel + (f * 100), 0), 0.95f, 2f, 5f, 1, 5f);
            }
        }

        // Reset values for the start of a new game
        public void reset(int Level, ContentManager Content, int p1char, int p2char, int p3char, int p4char)
        {
            Clyde.start(); //Starts Animation for Clyde.

            player[0].CharacterSelection = p1char; // Sets the players character based on what they chose
            player[1].CharacterSelection = p2char; // Sets the players character based on what they chose
            player[2].CharacterSelection = p3char; // Sets the players character based on what they chose
            player[3].CharacterSelection = p4char; // Sets the players character based on what they chose


            currentPosition = 0; // Sets all racers position to 0


            if (Level <= maxlevels) // Checks that the level number is equal to or less than the maximum amount of levels.
            {
                gameover = false; // Sets game over to false.
                endrace = false; // Sets end race to false
                LevelCounter = Level; // Sets the LevelCounter to the current Level
                gameruntime = 0; // Sets gameruntime to 0

                // Draw stuff depending on the game state
                switch (LevelCounter)
                {
                    case 1:
                        // Running Race

                        //Sets the gap and the ground level for the sprites.
                        gap = 50;
                        glevel = 525;

                        //Draws the four background images
                        background[0] = new graphic2d(Content, "Running Screen 1", (int)(displaywidth * 1f), (int)(displayheight * 1f));
                        background[1] = new graphic2d(Content, "Running Screen 2", (int)(displaywidth * 1f), (int)(displayheight * 1f));
                        background[2] = new graphic2d(Content, "Running Screen 2", (int)(displaywidth * 1f), (int)(displayheight * 1f));
                        background[3] = new graphic2d(Content, "Running Screen 4", (int)(displaywidth * 1f), (int)(displayheight * 1f));



                        //Sets the four images positons
                        for (int i = 1; i < background.Count(); i++)
                        {
                            background[i].rect.Y = background[0].rect.Y;
                            background[i].rect.X = background[i - 1].rect.X + background[i - 1].rect.Width;
                        }

                        // Numbers all of the characters for the character selection screen.
                        for (int f = 0; f < player.Count(); f++)
                        {
                            player[f].position.X = startx;

                            if (player[f].CharacterSelection == 1)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "ScottishSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }
                            if (player[f].CharacterSelection == 2)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "FemaleScottishSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }
                            if (player[f].CharacterSelection == 3)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "EnglishSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }
                            if (player[f].CharacterSelection == 4)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "FemaleEnglishSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }
                            if (player[f].CharacterSelection == 5)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "IrishSpriteSheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }
                            if (player[f].CharacterSelection == 6)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "FemaleIrishSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }
                            if (player[f].CharacterSelection == 7)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "WelshSpriteSheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }
                            if (player[f].CharacterSelection == 8)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "FemaleWelshSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }

                        }

                        MediaPlayer.Play(Crowd); // Plays the crowd in the background


                        break;
                    case 2:
                        // Swimming Race
                        gap = 100; // Sets the gap between players
                        glevel = 485; // Sets the ground level

                        // Draws the four background images
                        background[0] = new graphic2d(Content, "PoolScreen1", (int)(displaywidth * 1f), (int)(displayheight * 1f));
                        background[1] = new graphic2d(Content, "PoolScreen2", (int)(displaywidth * 1f), (int)(displayheight * 1f));
                        background[2] = new graphic2d(Content, "PoolScreen2", (int)(displaywidth * 1f), (int)(displayheight * 1f));
                        background[3] = new graphic2d(Content, "PoolScreen3", (int)(displaywidth * 1f), (int)(displayheight * 1f));



                        // Positions the background images correctly
                        for (int i = 1; i < background.Count(); i++)
                        {
                            background[i].rect.Y = background[0].rect.Y;
                            background[i].rect.X = background[i - 1].rect.X + background[i - 1].rect.Width;
                        }

                        // Double Array for drawing waves and barriers
                        sbackground[0, 0] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[0, 1] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[0, 2] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[0, 3] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[0, 4] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[0, 5] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[1, 0] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[1, 1] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[1, 2] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[1, 3] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[1, 4] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[1, 5] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[2, 0] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[2, 1] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[2, 2] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[2, 3] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[2, 4] = new graphic2d(Content, "BackWave", 100, 100);
                        sbackground[2, 5] = new graphic2d(Content, "Barrier1", 100, 100);
                        sbackground[3, 0] = new graphic2d(Content, "BackWave2", 100, 100);
                        sbackground[3, 1] = new graphic2d(Content, "barrier2", 100, 100);
                        sbackground[3, 2] = new graphic2d(Content, "BackWave2", 100, 100);
                        sbackground[3, 3] = new graphic2d(Content, "barrier2", 100, 100);
                        sbackground[3, 4] = new graphic2d(Content, "BackWave2", 100, 100);
                        sbackground[3, 5] = new graphic2d(Content, "barrier2", 100, 100);


                        // Positions the player on the start line and numbers the characters for character selection.
                        for (int f = 0; f < player.Count(); f++)
                        {
                            player[f].position.X = startx;

                            if (player[f].CharacterSelection == 1)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "ScottishSwimmerSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }

                            if (player[f].CharacterSelection == 2)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "ScottishFemaleSwimmerSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }

                            if (player[f].CharacterSelection == 3)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "EnglishSwimmerSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }

                            if (player[f].CharacterSelection == 4)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "EnglishFemaleSwimmerSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }

                            if (player[f].CharacterSelection == 5)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "WelshSwimmerSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }

                            if (player[f].CharacterSelection == 6)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "WelshFemaleSwimmerSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }

                            if (player[f].CharacterSelection == 7)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "IrishSwimmerSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }

                            if (player[f].CharacterSelection == 8)
                            {
                                player[f].spriteanimation[0] = new animation(Content, "IrishFemaleSwimmerSpritesheet", 0, 0, 0.2f, Color.White, true, 24, 5, 5, true, false, false);
                            }
                        }

                        // Swimming pool waves and barriers

                        for (int i = 0; i < 6; i++)
                        {
                            for (int section = 0; section < 4; section++)
                            {
                                // Is I an even number
                                if (i % 2 == 0)
                                {
                                    // waves
                                    sbackground[section, i].rect.X = background[section].rect.X + 480;
                                    sbackground[section, i].rect.Y = background[section].rect.Y + 480 + (i * 60);
                                    sbackground[section, i].rect.Width = background[section].rect.Width + 100;
                                    sbackground[section, i].rect.Height = 200;
                                    sbackground[3, 0].rect.X = background[section].rect.X + 480;
                                    sbackground[3, 2].rect.X = background[section].rect.X + 480;
                                    sbackground[3, 4].rect.X = background[section].rect.X + 480;
                                    sbackground[3, 0].rect.Y = background[section].rect.Y + 480 + (i * 60);
                                    sbackground[3, 2].rect.Y = background[section].rect.Y + 400 + (i * 60);
                                    sbackground[3, 4].rect.Y = background[section].rect.Y + 220 + (i * 60);
                                    sbackground[3, 0].rect.Width = 400;
                                    sbackground[3, 2].rect.Width = 400;
                                    sbackground[3, 4].rect.Width = 400;
                                    sbackground[3, 0].rect.Height = 210;
                                    sbackground[3, 2].rect.Height = 210;
                                    sbackground[3, 4].rect.Height = 210;

                                }
                                else
                                {
                                    // Barriers
                                    sbackground[section, i].rect.X = background[section].rect.X + 480;
                                    sbackground[section, i].rect.Y = background[section].rect.Y + 550 + (i * 60);
                                    sbackground[section, i].rect.Width = background[section].rect.Width + 100;
                                    sbackground[section, i].rect.Height = 30;
                                    sbackground[3, 1].rect.Width = 400;
                                    sbackground[3, 3].rect.Width = 400;
                                    sbackground[3, 5].rect.Width = 400;
                                }
                            }
                        }
                        // Plays the crowd in the background.
                        MediaPlayer.Play(Crowd);
                        break;
                }




                for (int f = 0; f < player.Count(); f++)
                {
                    player[f].RunningTime = 0;
                }

                //Setup a camera for each viewport
                for (int i = 0; i < cam.Count(); i++)
                {
                    cam[i] = new Camera2D(viewports[i + 1], background[0].rect.Width * 4, background[0].rect.Height);
                }

            }
            else
            {
                gameover = true;
            }
        }

        public Boolean update(float gtime, GamePadState[] pad, KeyboardState keys, ContentManager Content)
        {

            Clyde.update(gtime); // Update animated sprite Clyde.

            if (!endrace)
            {
                // Main game code
                gameruntime += gtime;  // Count how long the game has been running for

                // Players are racing.
                for (int f = 0; f < player.Count(); f++)
                {
                    if (player[f].position.X < 4995 && gameruntime > 3000)
                    {
                        if (LevelCounter == 1)
                        {
                            player[f].dalemove(pad[f], gtime, 10000, glevel + (f * gap), Footsteps);
                        }
                        else
                            player[f].dalemove(pad[f], gtime, 10000, glevel + (f * gap), Splash);
                    }
                    else
                        player[f].updatesprite(gtime, 10000, glevel + (f * gap));

                    cam[f].setcam(new Vector2(player[f].position.X, player[f].position.Y));

                    if (player[f].position.X > 4995 && player[f].RunningTime == 0)
                    {
                        player[f].RunningTime = (gameruntime / 1000 - 3);
                        currentPosition++;
                        player[f].FinishingPosition = currentPosition;
                    }
                }
                    // Check to see if all players have crossed the finish line
                    endrace = true;
                for (int f = 0; f < player.Count(); f++)
                {
                    if (player[f].RunningTime == 0)
                    {
                        endrace = false;
                    }
                }
            }
            else
            {
                Boolean ready = true;
                // The rcae is over.
                for (int f = 0; f < player.Count(); f++)
                {
                    if (pad[f].Buttons.Start == ButtonState.Released) // Checks if the start button is not being pressed
                    {
                        ready = false; // False if start button is not pressed
                    }
                }
                if (ready) // Checks if all players are ready
                {
                    if (LevelCounter == 1) // If start button is pressed by all players go to next level
                    {
                        reset(2, Content, player[0].CharacterSelection, player[1].CharacterSelection, player[2].CharacterSelection, player[3].CharacterSelection);
                    }
                    else
                        gameover = true; // If the game is on its second level and the race finishes then game over is triggered.
                }
            }
    
 
            if (keys.IsKeyDown(Keys.Escape))
            {
                // END GAME
                gameover = true;
            }

            return gameover;
        }

        public void draw(GraphicsDeviceManager graphics, SpriteBatch spriteBatch, SpriteFont mainfont, ContentManager Content)
        {
            // Draw the game graphics inside each viewport using the correct camera
            for (int i = 0; i < cam.Count(); i++)
            {
                // Draw the in-game graphics in viewport 1
                graphics.GraphicsDevice.Viewport = viewports[i + 1];
                var viewMatrix = cam[i].GetViewMatrix();
                spriteBatch.Begin(transformMatrix: viewMatrix);

                // Draw backgrounds
                for (int r = 0; r < background.Count(); r++)
                {
                    background[r].drawme(ref spriteBatch);
                }

                
                if (LevelCounter == 1) // Checks that the game is on level one.
                {
                    for (int c = 0; c < 23; c++)
                    {
                        Clyde.drawme(ref spriteBatch, new Vector3(500 + (c * 200), 425, 0)); // If the game is on level one then draw 23 clydes.
                    }
                }

                if (LevelCounter == 2) // Checks that the game is on level two.
                {
                    for (int c = 0; c < 4; c++)
                    {
                        Clyde.drawme(ref spriteBatch, new Vector3(350 , 425 + (c * 100), 0)); // If teh game is on level two then draw 4 clydes.
                    }
                }
                

                for (int f = 0; f < player.Count(); f++)
                {
                    if (LevelCounter == 2) // Check that the game is on level two
                    {
                        for (int s = 0; s < 6; s += 2)
                        {
                            sbackground[f, s].drawme(ref spriteBatch); // Draw waves for each player
                        }
                    }
                  
                }

                for (int f = 0; f < player.Count(); f++)
                {

                    if (LevelCounter == 2) // Check that the game is on level two
                    {
                        for (int s = 1; s < 6; s += 2)
                        {
                            sbackground[f, s].drawme(ref spriteBatch); // Draw barriers for each player
                        }
                    }
                }

                for (int f = 0; f < player.Count(); f++) // For each player loop
                {

                    player[f].drawme(ref spriteBatch); // For each player draw a character


                    if (player[f].RunningTime > 0) // Checks that the running time is greater than 0
                    {
                        spriteBatch.DrawString(mainfont, player[f].FinishingPosition.ToString("0"), new Vector2(player[f].position.X, player[f].position.Y - 100), // Draw the players finishing position above them
                        Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0); //Sets the text size and colour.

                        if (player[f].FinishingPosition == 1) // Checks if player finishes first.
                        {
                            spriteBatch.Draw(GoldTrophy, new Vector2(player[f].position.X - 25, player[f].position.Y - 120), null, Color.White,0,Vector2.Zero,0.05f,SpriteEffects.None,0); // If player finished first draw a gold trophy above their head.
                        }

                        if (player[f].FinishingPosition == 2) // Checks if the player finishes 2nd
                        {
                            spriteBatch.Draw(SilverTrophy, new Vector2(player[f].position.X - 25, player[f].position.Y - 120), null, Color.White, 0, Vector2.Zero, 0.05f, SpriteEffects.None, 0); // If the player finished second draw a silver trophy above their head.
                        }


                        if (player[f].FinishingPosition == 3) // Checks if the player finishes 3rd
                        {
                            spriteBatch.Draw(BronzeTrophy, new Vector2(player[f].position.X - 25, player[f].position.Y - 120), null, Color.White, 0, Vector2.Zero, 0.05f, SpriteEffects.None, 0); // If the player finished third draw a bronze trophy above their head.
                        }

                        if (player[f].FinishingPosition == 4) // Checks if the player finishes 4th
                        {
                            spriteBatch.DrawString(mainfont, player[f].FinishingPosition.ToString("4th"), new Vector2(player[f].position.X, player[f].position.Y - 100), // Writes 4th above the players head if they place 4th
                            Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0); // Sets the size and colour of the position text
                        }

                        spriteBatch.DrawString(mainfont, player[f].RunningTime.ToString("0.00"), new Vector2(player[f].position.X - 50, player[f].position.Y + 50), // Writes the players finishing time below them
                        Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0); // Sets the size abd colour of the finishing time text.
                    }


                }

                spriteBatch.End(); // End drawing

            }

            // Draw stuff on top of all the viewports onto the main game window
            graphics.GraphicsDevice.Viewport = viewports[0];

            spriteBatch.Begin();

            
            if (gameruntime <= 3000) // Checks if the game has been running for less than or equal to three seconds.
            {
                
                if (gameruntime >= 2900) // Checks if the game has been running for 2.9 or more seconds.
                {
                    spriteBatch.DrawString(mainfont, "GO!", new Vector2(displaywidth / 2 - 200, displayheight / 2 - 200), // If the game has been running for more than or equal to 2.9 seconds then display GO!
                    Color.Green, MathHelper.ToRadians(0), new Vector2(0, 0), 10f, SpriteEffects.None, 0); // Sets the size and colour of the GO! text
                    Gunshot.Play(); // Plays a Gunshot Sound

                }
            else
            {
                spriteBatch.DrawString(mainfont, (3 - gameruntime / 1200).ToString("0"), new Vector2(displaywidth / 2, displayheight / 2 - 200), // If the game has been running for less than 2.9 seconds then countdown from 3
                Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 10f, SpriteEffects.None, 0); // Sets the size and colour of the countdown text
            }
            }
            else
                spriteBatch.DrawString(mainfont, "Time " + (gameruntime / 1000 - 3).ToString("0.00"), new Vector2(displaywidth / 2 - 100, 20), // Draws the total time running after 2.9 seconds have passed.
                Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0); // Sets the total time spent running text size amd colour.

            spriteBatch.End(); // End drawing.

            if (endrace)
            {
                graphics.GraphicsDevice.Viewport = viewports[0]; // Remove viewports for a unified screen
                spriteBatch.Begin(); // Begin drawing sprites.


                CharBackground.drawme(ref spriteBatch); // Draw the character selection background
                spriteBatch.DrawString(mainfont, "RACE ENDED", new Vector2(50, 50), // Draw text that states the race has ended.
                  Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 4f, SpriteEffects.None, 0); // Set the size and colour of this text.
                MediaPlayer.Stop(); //Stops the crowd cheering.

                for (int f = 0; f < player.Count(); f++) // Creates a loop for all players
                {
                    player[f].spriteanimation[0].size = 0.5f; // Sets players size to half
                    player[f].spriteanimation[0].updatesize(); // Updates the players size.
                    player[f].position.X = 100 + (f * 250); // Sets the players position and puts a 250 pixel gap between them on the X co-ordinate
                    player[f].position.Y = 600; // Sets the players position on the Y co-ordinate
                    player[f].updatesprite(0, displaywidth, displayheight); // Update the sprites.
                }


                for (int f = 0; f < player.Count(); f++) // Creates a loop for all players
                {
                    player[f].drawme(ref spriteBatch); // Draws the players.


                    if (player[f].RunningTime > 0) // Checks if running time is greater than 0
                    {
                        spriteBatch.DrawString(mainfont, player[f].FinishingPosition.ToString("0"), new Vector2(player[f].position.X, player[f].position.Y - 100), // Draw the players finishing position above them
                        Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0); //Sets the text size and colour.

                        if (player[f].FinishingPosition == 1)  // Checks if player finishes first.
                        {
                            spriteBatch.Draw(GoldTrophy, new Vector2(player[f].position.X, player[f].position.Y - 240), null, Color.White, 0, Vector2.Zero, 0.05f, SpriteEffects.None, 0); // If player finished first draw a gold trophy above their head.
                        }

                        if (player[f].FinishingPosition == 2) // Checks if the player finishes 2nd
                        {
                            spriteBatch.Draw(SilverTrophy, new Vector2(player[f].position.X, player[f].position.Y - 240), null, Color.White, 0, Vector2.Zero, 0.05f, SpriteEffects.None, 0); // If the player finished second draw a silver trophy above their head.
                        }


                        if (player[f].FinishingPosition == 3) // Checks if the player finishes 3rd
                        {
                            spriteBatch.Draw(BronzeTrophy, new Vector2(player[f].position.X, player[f].position.Y - 240), null, Color.White, 0, Vector2.Zero, 0.05f, SpriteEffects.None, 0); // If the player finished third draw a bronze trophy above their head.
                        }

                        if (player[f].FinishingPosition == 4) // Checks if the player finishes 4th
                        {
                            spriteBatch.DrawString(mainfont, player[f].FinishingPosition.ToString("4th"), new Vector2(player[f].position.X, player[f].position.Y - 240), // Writes 4th above the players head if they place 4th
                            Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0); // Sets the size and colour of the position text
                        }

                        spriteBatch.DrawString(mainfont, player[f].RunningTime.ToString("0.00"), new Vector2(player[f].position.X - 50, player[f].position.Y + 50), // Writes the players finishing time below them
                        Color.Black, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);  // Sets the size abd colour of the finishing time text.
                    }


                }


                spriteBatch.End(); // Stop drawing sprites.

            }





        }
    }
}
