using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using gamelib2d;
using System.IO;

namespace Commonwealth_Guantlet
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Running : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int displaywidth;
        int displayheight;
        SpriteFont mainfont;        // Main font for drawing in-game text

        graphic2d background;       // Background image
        graphic2d CharBackground;
        Random randomiser = new Random();       // Variable to generate random numbers

        GamePadState[] pad = new GamePadState[4];       // Array to hold gamepad states
        Boolean keyboardreleased = true;
        KeyboardState keys;                             // Variable to hold keyboard state
        KeyboardState lastkeystate;
        MouseState mouse;                               // Variable to hold mouse state
        Boolean released = true;                        // Check for sticks or buttons being released

        sprite2d mousepointer1, mousepointer2;          // Sprite to hold a mouse pointer
        const int numberofoptions = 4;                    // Number of main menu options
        sprite2d[,] MenuButtons = new sprite2d[numberofoptions, 2]; // Array of sprites to hold the menu options
        int optionselected = 0;                         // Current menu option selected

        int score = 0;

        const int numberofhighscores = 10;                              // Number of high scores to store
        int[] highscores = new int[numberofhighscores];                 // Array of high scores
        string[] highscorenames = new string[numberofhighscores];       // Array of high score names
        const int maxnamelength = 30;   // Maximum name length for high score table
        int lasthighscore = numberofhighscores - 1;

        float keycounter = 0;           // Counter for delay between key strokes
        const float keystrokedelay = 200;   // Delay between key strokes in milliseconds

        enum GameState { Menu, PlayingGame, Options, HighScore, GameOver, CharacterSelection, Exit };
        GameState CurrentState = GameState.Menu;

        Dictionary<int, GameState> MenuOptions = new Dictionary<int, GameState>()
        {
            {0, GameState.CharacterSelection},
            {1, GameState.Options},
            {2, GameState.HighScore},
            {3, GameState.Exit}
        };

        sprite2d[,] heads = new sprite2d[4, 8];
        sprite2d[] Country = new sprite2d[4];
        sprite2d[] Indicators = new sprite2d[4];
        sprite2d heading;

        int p1char, p2char, p3char, p4char;
        Boolean p1released, p2released, p3released, p4released;
        Boolean p1pressed, p2pressed, p3pressed, p4pressed;


        game maingame;

        public Running()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.graphics.PreferredBackBufferWidth = 1366;
            this.graphics.PreferredBackBufferHeight = 768;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            displaywidth = graphics.GraphicsDevice.Viewport.Width;
            displayheight = graphics.GraphicsDevice.Viewport.Height;
            //graphics.ToggleFullScreen(); // Put game into full screen mode

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            mainfont = Content.Load<SpriteFont>("quartz4"); // Load font

            maingame = new game(graphics, Content, displaywidth, displayheight); // Gathers the graphics, content, width and height for the game

            background = new graphic2d(Content, "Homescreen Example", displaywidth, displayheight); // Loads the background image
            CharBackground = new graphic2d(Content, "CharacterSelectionScreen", displaywidth, displayheight); // Loads 
            mousepointer1 = new sprite2d(Content, "Cursor", 0, 0, 1f, Color.White, true); // Sets the mouse cursor in the menu
            mousepointer2 = new sprite2d(Content, "Cursor", 0, 0, 1f, Color.White, true); // Sets the mouse cursor in the menu

            MenuButtons[0, 0] = new sprite2d(Content, "PlayButton", displaywidth / 2, displayheight / 4, 1, Color.White, true); // Sets the play button in the menu.
            MenuButtons[0, 1] = new sprite2d(Content, "PlayButton(blue)", displaywidth / 2, MenuButtons[0, 0].rect.Y, 1, Color.White, true); // Sets the play button when it's been hovered.
            MenuButtons[1, 0] = new sprite2d(Content, "OptionsButton", displaywidth / 2, MenuButtons[0, 0].rect.Y + 100, 1, Color.White, true); // Sets the options button.
            MenuButtons[1, 1] = new sprite2d(Content, "OptionsButton(Blue)", displaywidth / 2, MenuButtons[0, 0].rect.Y + 100, 1, Color.White, true); // Sets the options button when it's being hovered.
            MenuButtons[2, 0] = new sprite2d(Content, "GalleryButton", displaywidth / 2, MenuButtons[0, 0].rect.Y + 200, 1, Color.White, true); // Sets the gallery button.
            MenuButtons[2, 1] = new sprite2d(Content, "GalleryButton(Blue)", displaywidth / 2, MenuButtons[0, 0].rect.Y + 200, 1, Color.White, true); // Sets the gallery button when it's being hovered.
            MenuButtons[3, 0] = new sprite2d(Content, "ExitButton", displaywidth / 2, MenuButtons[0, 0].rect.Y + 300, 1, Color.White, true); // Sets the exit button.
            MenuButtons[3, 1] = new sprite2d(Content, "ExitButton(Blue)", displaywidth / 2, MenuButtons[0, 0].rect.Y + 300, 1, Color.White, true); // Sets the exit button when it's being hovered.
            for (int i = 0; i < numberofoptions; i++)
            {
                MenuButtons[i, 0].updateobject(); // Updates menu buttons
            }

            // Sets the defaults for player characters
            p1char = 1;
            p2char = 2;
            p3char = 3;
            p4char = 7;

            float size = 0.2f; // Sets buttons sizes
            int chargap = 250; // Sets gap between the buttons
            int charpositionx = 350; // Sets the X position of the buttons
            int charpositiony = 375; // Sets the Y positions of the buttons


            for (int p = 0; p < 4; p++) // Creates a loop for each button
            // Positions all of the buttons listed in the double array
            {
                heads[p, 0] = new sprite2d(Content, "ScoButton", charpositionx + (p * chargap), charpositiony, size, Color.White, true);
                heads[p, 1] = new sprite2d(Content, "ScoButton(f)", charpositionx + (p * chargap), charpositiony, size, Color.White, true);
                heads[p, 2] = new sprite2d(Content, "EngButtton", charpositionx + (p * chargap), charpositiony, size, Color.White, true);
                heads[p, 3] = new sprite2d(Content, "EngButtton(f)", charpositionx + (p * chargap), charpositiony, size, Color.White, true);
                heads[p, 4] = new sprite2d(Content, "IreButton", charpositionx + (p * chargap), charpositiony, size, Color.White, true);
                heads[p, 5] = new sprite2d(Content, "IreButton(f)", charpositionx + (p * chargap), charpositiony, size, Color.White, true);
                heads[p, 6] = new sprite2d(Content, "WelButton", charpositionx + (p * chargap), charpositiony, size, Color.White, true);
                heads[p, 7] = new sprite2d(Content, "WelButton(f)", charpositionx + (p * chargap), charpositiony, size, Color.White, true);
            }

            //positions all of the country tags in the country array
            Country[0] = new sprite2d(Content, "Scotland", 350, charpositiony + 100, 1f, Color.White, true);
            Country[1] = new sprite2d(Content, "England", 600, charpositiony + 100, 1f, Color.White, true);
            Country[2] = new sprite2d(Content, "Ireland", 850, charpositiony + 100, 1f, Color.White, true);
            Country[3] = new sprite2d(Content, "Wales", 1100, charpositiony + 100, 1f, Color.White, true);

            //positions all of the player tags in the indicators array
            Indicators[0] = new sprite2d(Content, "P1", 350, charpositiony - 200, 0.2f, Color.White, true);
            Indicators[1] = new sprite2d(Content, "P2", 600, charpositiony - 200, 0.2f, Color.White, true);
            Indicators[2] = new sprite2d(Content, "P3", 850, charpositiony - 200, 0.2f, Color.White, true);
            Indicators[3] = new sprite2d(Content, "P4", 1100, charpositiony - 200, 0.2f, Color.White, true);

            //positions the heading for the character selection screen
            heading = new sprite2d(Content, "CharacterSelection", displaywidth / 2, 50, 1f, Color.White, true);





            // Load in high scores
            if (File.Exists(@"highscore.txt")) // This checks to see if the file exists
            {
                StreamReader sr = new StreamReader(@"highscore.txt");	// Open the file

                String line;		// Create a string variable to read each line into
                for (int i = 0; i < numberofhighscores && !sr.EndOfStream; i++)
                {
                    line = sr.ReadLine();	// Read the first line in the text file
                    highscorenames[i] = line.Trim(); // Read high score name

                    if (!sr.EndOfStream)
                    {
                        line = sr.ReadLine();	// Read the first line in the text file
                        line = line.Trim(); 	// This trims spaces from either side of the text
                        highscores[i] = Convert.ToInt32(line);	// This converts line to numeric
                    }
                }
                sr.Close();			// Close the file
            }
            // SORT HIGH SCORE TABLE
            Array.Sort(highscores, highscorenames);
            Array.Reverse(highscores);
            Array.Reverse(highscorenames);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            // Save high scores
            StreamWriter sw = new StreamWriter(@"highscore.txt");
            for (int i = 0; i < numberofhighscores; i++)
            {
                sw.WriteLine(highscorenames[i]);
                sw.WriteLine(highscores[i].ToString());
            }
            sw.Close();

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // TODO: Add your update logic here
            if (this.IsActive)
            {
                float timebetweenupdates = (float)gameTime.ElapsedGameTime.TotalMilliseconds; // Time between updates

                pad[0] = GamePad.GetState(PlayerIndex.One);     // Reads gamepad 1
                pad[1] = GamePad.GetState(PlayerIndex.Two);     // Reads gamepad 2
                pad[2] = GamePad.GetState(PlayerIndex.Three);   // Reads gamepad 3
                pad[3] = GamePad.GetState(PlayerIndex.Four);    // Reads gamepad 4
                keys = Keyboard.GetState();                     // Read keyboard
                keyboardreleased = (keys != lastkeystate);      // Has keyboard input changed
                mouse = Mouse.GetState();                       // Read Mouse

                // Read the mouse and set the mouse cursor
                mousepointer1.position.X = mouse.X;
                mousepointer1.position.Y = mouse.Y;
                mousepointer1.updateobject();
                // Set a small bounding sphere at the center of the mouse cursor
                mousepointer1.bsphere = new BoundingSphere(mousepointer1.position, 2);

                switch (CurrentState)
                {
                    case GameState.Menu: // Main menu
                        // Game is on the main menu
                        background = new graphic2d(Content, "Homescreen Example", displaywidth, displayheight - 150); // Sets the background image for the main menu
                        updatemenu(timebetweenupdates); // Updates the main menu
                        break;
                    case GameState.CharacterSelection: // Character selection screen
                        // Select character
                        UpdateSelection(pad); // Reads the controllers
                        break;
                    case GameState.PlayingGame:
                        // Game is being played
                        background = new graphic2d(Content, "blackbackground", displaywidth, displayheight); // Sets the background behind the view ports to black
                        if (maingame.update(timebetweenupdates, pad, keys, Content))
                        {
                            if (score > highscores[lasthighscore]) // Checks if the score is higher than the last highscore
                                highscorenames[lasthighscore] = ""; // Writes the name for the highscorer

                            // Switch to game over state
                            CurrentState = GameState.GameOver;
                        }

                        break;
                    case GameState.Options:
                        // Options menu
                        updateoptions();
                        break;
                    case GameState.HighScore:
                        // High Score table
                        updatehighscore();
                        break;
                    case GameState.GameOver:
                        // Game over screen
                        background = new graphic2d(Content, "Homescreen Example", displaywidth, displayheight);
                        updategameover(timebetweenupdates);
                        break;
                    default:
                        // Do something if none of the above are selected
                        this.Exit();    // Quit Game
                        break;
                }
                lastkeystate = keys;                     // Read keyboard
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            background.drawme(ref spriteBatch);
            spriteBatch.End();

            // Draw stuff depending on the game state
            switch (CurrentState)
            {
                case GameState.Menu:
                    // Game is on the main menu
                    drawmenu();
                    break;
                case GameState.CharacterSelection:
                    // Draw selection screen
                    DrawSelection();
                    break;
                case GameState.PlayingGame:
                    // Game is being played
                    maingame.draw(graphics, spriteBatch, mainfont, Content);
                    break;
                case GameState.Options:
                    // Options menu
                    drawoptions();
                    break;
                case GameState.HighScore:
                    // High Score table
                    drawhighscore();
                    break;
                case GameState.GameOver:
                    // Game over screen
                    drawgameover();
                    break;
                default:
                    break;
            }

            base.Draw(gameTime);
        }

        public void UpdateSelection(GamePadState[] pad) // Updates the players selection in the character selection screen.
        {
            if (p1released && !p1pressed) // Checks that the left stick is released and the button is not pressed
            {
                if (pad[0].ThumbSticks.Left.X > 0.5f) // Checks if the left stick is being pushed right
                    p1char++; // Adds one to the player selection counter
                if (pad[0].ThumbSticks.Left.X < -0.5f) // Checks if the left stick is being pushed left
                    p1char--; // Removes one from the player selection counter

                if (Math.Abs(pad[0].ThumbSticks.Left.X) > 0.5) // Checks if the left stick is being pushed right
                    p1released = false; // Sets released to false

                if (pad[0].Buttons.A == ButtonState.Pressed) // Checks if button A has been pressed
                {
                    heads[0, p1char].colour = Color.Red; // Sets the colour of the head to red if selected
                    p1pressed = true; // Sets pressed to true
                }
            }

            if (p2released && !p2pressed) // Checks that the left stick is released and the button is not pressed
            {
                if (pad[1].ThumbSticks.Left.X > 0.5f) // Checks if the left stick is being pushed right
                    p2char++; // Adds one to the player selection counter
                if (pad[1].ThumbSticks.Left.X < -0.5f) // Checks if the left stick is being pushed left
                    p2char--; // Removes one from the player selection counter

                if (Math.Abs(pad[1].ThumbSticks.Left.X) > 0.5) // Checks if the left stick is being pushed right
                    p2released = false; // Sets released to false

                if (pad[1].Buttons.A == ButtonState.Pressed) // Checks if button A has been pressed
                {
                    heads[1, p2char].colour = Color.Red; // Sets the colour of the head to red if selected
                    p2pressed = true; // Sets pressed to true
                }
            }

            if (p3released && !p3pressed) // Checks that the left stick is released and the button is not pressed
            {
                if (pad[2].ThumbSticks.Left.X > 0.5f) // Checks if the left stick is being pushed right
                    p3char++; // Adds one to the player selection counter
                if (pad[2].ThumbSticks.Left.X < -0.5f) // Checks if the left stick is being pushed left
                    p3char--; // Removes one from the player selection counter

                if (Math.Abs(pad[2].ThumbSticks.Left.X) > 0.5) // Checks if the left stick is being pushed right
                    p3released = false; // Sets released to false

                if (pad[2].Buttons.A == ButtonState.Pressed) // Checks if button A has been pressed
                {
                    heads[2, p3char].colour = Color.Red; // Sets the colour of the head to red if selected
                    p3pressed = true; // Sets pressed to true
                }
            }

            if (p4released && !p4pressed) // Checks that the left stick is released and the button is not pressed
            {
                if (pad[3].ThumbSticks.Left.X > 0.5f) // Checks if the left stick is being pushed right
                    p4char++; // Adds one to the player selection counter
                if (pad[3].ThumbSticks.Left.X < -0.5f) // Checks if the left stick is being pushed left
                    p4char--; // Removes one from the player selection counter

                if (Math.Abs(pad[3].ThumbSticks.Left.X) > 0.5) // Checks if the left stick is being pushed right
                    p4released = false; // Sets released to false

                if (pad[3].Buttons.A == ButtonState.Pressed) // Checks if button A has been pressed
                {
                    heads[3, p4char].colour = Color.Red; // Sets the colour of the head to red if selected
                    p4pressed = true; // Sets pressed to true
                }
            }

            if (p1char > 1) p1char = 1; // Sets it so that player one can't select a character numbered greater than one
            if (p1char < 0) p1char = 0; // Sets it so that player one can't select a character numbered less than zero
            if (p2char > 3) p2char = 3; // Sets it so that player two can't select a character numbered greater than three
            if (p2char < 2) p2char = 2; // Sets it so that player two can't select a character numbered less than two
            if (p3char > 5) p3char = 5; // Sets it so that player three can't select a character numbered greater than five
            if (p3char < 4) p3char = 4; // Sets it so that player three can't select a character numbered less than four
            if (p4char > 7) p4char = 7; // Sets it so that player four can't select a character numbered greater than seven
            if (p4char < 6) p4char = 6; // Sets it so that player four can't select a character numbered less than siz

            if (Math.Abs(pad[0].ThumbSticks.Left.X) < 0.5) // Checks if the left stick is being pushed
                p1released = true; // Sets released to true
            if (Math.Abs(pad[1].ThumbSticks.Left.X) < 0.5) // Checks if the left stick is being pushed
                p2released = true; // Sets released to true
            if (Math.Abs(pad[2].ThumbSticks.Left.X) < 0.5) // Checks if the left stick is being pushed
                p3released = true; // Sets released to true
            if (Math.Abs(pad[3].ThumbSticks.Left.X) < 0.5) // Checks if the left stick is being pushed
                p4released = true; // Sets released to true

            if (p1pressed && p2pressed && p3pressed && p4pressed) // CHecks that all players have selected a character
            {
                CurrentState = GameState.PlayingGame; // Starts playing the game
                maingame.reset(1, Content, p1char + 1, p2char + 1, p3char + 1, p4char + 1); // Sets the game to level one and the players to their correct characters.
            }


        }

        public void DrawSelection()
        {
            spriteBatch.Begin(); // Begins drawing sprite batch

            CharBackground.drawme(ref spriteBatch); // Draws the characters background

            // Draws everything stored in country array
            Country[0].drawme(ref spriteBatch);
            Country[1].drawme(ref spriteBatch);
            Country[2].drawme(ref spriteBatch);
            Country[3].drawme(ref spriteBatch);

            // Draws everything stored in inidicators array
            Indicators[0].drawme(ref spriteBatch);
            Indicators[1].drawme(ref spriteBatch);
            Indicators[2].drawme(ref spriteBatch);
            Indicators[3].drawme(ref spriteBatch);

            // Draws the heading for the character selection screen
            heading.drawme(ref spriteBatch);

            // Draws the head each player is viewing
            heads[0, p1char].drawme(ref spriteBatch);
            heads[1, p2char].drawme(ref spriteBatch);
            heads[2, p3char].drawme(ref spriteBatch);
            heads[3, p4char].drawme(ref spriteBatch);

            spriteBatch.End(); // Stops drawing
        }


        public void updatemenu(float gtime)
        {

            // Check for mousepointer being over a menu option
            for (int i = 0; i < numberofoptions; i++)
            {
                // Check for up and down on left stick of pad1 for navagating the menu options
                if (released)
                {
                    if (pad[0].ThumbSticks.Left.Y > 0.5f)
                    {
                        optionselected--;
                        released = false;
                    }
                    if (pad[0].ThumbSticks.Left.Y < -0.5f)
                    {
                        optionselected++;
                        released = false;
                    }
                }
                else
                {
                    if (Math.Abs(pad[0].ThumbSticks.Left.Y) < 0.5)
                        released = true;
                }

                // Impose limits on the selectio of menu options 
                if (optionselected < 0) optionselected = 0;
                if (optionselected >= numberofoptions) optionselected = numberofoptions - 1;

                // Check for mouse over a menu option
                if (mousepointer1.bsphere.Intersects(MenuButtons[i, 0].bbox))
                {
                    optionselected = i;
                    if (mouse.LeftButton == ButtonState.Pressed)
                        CurrentState = MenuOptions[optionselected];
                }

                if (pad[0].Buttons.A == ButtonState.Pressed)
                    CurrentState = MenuOptions[optionselected];

            }

        }

        public void drawmenu()
        {
            spriteBatch.Begin(); // Begin drawing

            // Draw menu options
            for (int i = 0; i < numberofoptions; i++)
            {
                if (optionselected == i)
                    MenuButtons[i, 1].drawme(ref spriteBatch);
                else
                    MenuButtons[i, 0].drawme(ref spriteBatch);
            }

            // Draw mouse
            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);


            spriteBatch.End(); // Stop drawing

        }



        void updategameover(float gtime)
        {
            // Game is over
            if (score > highscores[lasthighscore])
            {
                keycounter -= gtime; // Counter to delay between keys of the same value being entered
                if (keyboardreleased)
                {
                    if (keys.IsKeyDown(Keys.Back) && highscorenames[lasthighscore].Length > 0)
                    {
                        highscorenames[lasthighscore] = highscorenames[lasthighscore].Substring(0, highscorenames[lasthighscore].Length - 1);
                    }
                    else
                    {
                        char nextchar = sfunctions2d.getnextkey();
                        char lastchar = '!';
                        if (highscorenames[lasthighscore].Length > 0)
                            lastchar = Convert.ToChar(highscorenames[lasthighscore].Substring(highscorenames[lasthighscore].Length - 1, 1));
                        if (nextchar != '!' && (nextchar != lastchar || keycounter < 0))
                        {
                            keycounter = keystrokedelay;
                            highscorenames[lasthighscore] += nextchar;
                            if (highscorenames[lasthighscore].Length > maxnamelength)
                                highscorenames[lasthighscore] = highscorenames[lasthighscore].Substring(0, maxnamelength);
                        }
                    }
                }

            }

            // Allow game to return to the main menu
            if (pad[0].Buttons.B == ButtonState.Pressed || keys.IsKeyDown(Keys.Enter))
            {
                if (score > highscores[lasthighscore])
                {
                    highscores[lasthighscore] = score;
                }

                // Sort the high score table
                Array.Sort(highscores, highscorenames);
                Array.Reverse(highscores);
                Array.Reverse(highscorenames);

                CurrentState = GameState.Menu;
            }

        }

        // Draws game over
        void drawgameover()
        {
            spriteBatch.Begin(); // Begin game over


            string gameoverText = "GAME OVER"; // Draws text
            Vector2 gameovertextSize = mainfont.MeasureString(gameoverText) * 3; // Sets the size of gameover text
            spriteBatch.DrawString(mainfont, gameoverText, new Vector2(displaywidth / 2 - gameovertextSize.X / 2, gameovertextSize.Y), Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 3f, SpriteEffects.None, 0); // Sets position of gameover text and size

            if (score > highscores[numberofhighscores - 1]) // Checks if score is larger than the highscores and deletes the lowest one
            {
                spriteBatch.DrawString(mainfont, "New High Score Enter Name", new Vector2(displaywidth / 2 - (int)(mainfont.MeasureString("New High Score Enter Name").X * (1f / 2f)), 400), // Draws text in the position stated
                        Color.Blue, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0); // Sets the colour and size of text
                spriteBatch.DrawString(mainfont, highscorenames[numberofhighscores - 1], new Vector2(displaywidth / 2 - (int)(mainfont.MeasureString("New High Score Enter Name").X * (1f / 2f)), 440),
                        Color.AliceBlue, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }

            spriteBatch.End(); // Stops drawing

        }






        public void updateoptions()
        {
            // Update code for the options screen

            // Allow game to return to the main menu
            if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.B == ButtonState.Pressed)
                CurrentState = GameState.Menu;
        }

        public void drawoptions()
        {
            spriteBatch.Begin();

            // Draw graphics for OPTIONS screen
            // Draw mouse
            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);
            spriteBatch.End();

        }

        public void updatehighscore()
        {
            // Update code for the high score screen

            // Allow game to return to the main menu
            if (keys.IsKeyDown(Keys.Escape) || pad[0].Buttons.B == ButtonState.Pressed)
                CurrentState = GameState.Menu;

        }

        public void drawhighscore()
        {
            // Draw graphics for High Score table
            spriteBatch.Begin();

            // Draw top ten high scores
            for (int i = 0; i < numberofhighscores; i++)
            {
                if (highscorenames[i].Length >= 24)
                    spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". " + highscorenames[i].Substring(0, 24), new Vector2(60, 100 + (i * 30)),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
                else
                    spriteBatch.DrawString(mainfont, (i + 1).ToString("0") + ". " + highscorenames[i], new Vector2(60, 100 + (i * 30)),
                        Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);

                spriteBatch.DrawString(mainfont, highscores[i].ToString("0"), new Vector2(displaywidth - 180, 100 + (i * 30)),
                    Color.White, MathHelper.ToRadians(0), new Vector2(0, 0), 1f, SpriteEffects.None, 0);
            }


            // Draw mouse
            if (optionselected > -1)
            {
                mousepointer2.rect = mousepointer1.rect;
                mousepointer2.drawme(ref spriteBatch);
            }
            else
                mousepointer1.drawme(ref spriteBatch);

            spriteBatch.End();

        }

    }
}
