using Game2.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Game2
{
    enum GameState
    {
        Menu,
        Playing,
        Gameover,
        Exiting
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Dictionary<GameState, IWindow> stateWindows;
        GameState _currentState = GameState.Menu;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            int h, w;
            graphics.PreferredBackBufferHeight = h = 850;
            graphics.PreferredBackBufferWidth = w = 850;
            Content.RootDirectory = "Content";

            var menuWindow = new MenuWindow(w, h);
            var gameWindow = new GameplayWindow(w, h);
            var gameoverWindow = new GameoverWindow(w, h);

            menuWindow.StateChanged += OnStateChanged;
            gameWindow.StateChanged += OnStateChanged;
            gameoverWindow.StateChanged += OnStateChanged;

            stateWindows = new Dictionary<GameState, IWindow>()
            {
                {GameState.Menu, menuWindow },
                {GameState.Playing, gameWindow },
                {GameState.Gameover, gameoverWindow },
                {GameState.Exiting, gameoverWindow },
            };
        }

        private void OnStateChanged(object sender, GameState state)
        {
            _currentState = state;
            stateWindows[_currentState].Initialize();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
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
            Resources.Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Resources.Pixel.SetData(new Color[] { Color.White });
            Resources.Planet = Content.Load<Texture2D>("planet");
            Resources.Sheriff = Content.Load<Texture2D>("player");
            Resources.SheriffHat = Content.Load<Texture2D>("player_hat_overlay");
            Resources.Thief = Content.Load<Texture2D>("thief");
            Resources.ThiefHat = Content.Load<Texture2D>("thief_overlay");
            Resources.Stars = new Texture2D[] { Content.Load<Texture2D>("star_1"), Content.Load<Texture2D>("star_2") };
            Resources.BatteryOutline = Content.Load<Texture2D>("battery");
            Resources.BatteryFill = Content.Load<Texture2D>("battery_overlay");
            Resources.Sun = Content.Load<Texture2D>("sun");
            Resources.MenuBackground = Content.Load<Texture2D>("menu");
            Resources.GameOverBackground = Content.Load<Texture2D>("you_tried");
            Resources.Background = new Texture2D(GraphicsDevice, 1, 1000);

            Resources.BackgroundMusic = Content.Load<Song>("background_music_2");
            Resources.StarCollectSound = Content.Load<SoundEffect>("star_capture");
            Resources.DeathSound = Content.Load<SoundEffect>("death_sound");
            Resources.JumpSound = Content.Load<SoundEffect>("jump");
            Resources.LowBatterySound = Content.Load<SoundEffect>("warning");

            Resources.Font = Content.Load<SpriteFont>("font");

            foreach(var state in (IEnumerable<GameState>)Enum.GetValues(typeof(GameState)))
                stateWindows[state].Initialize();
        }
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Resources.Planet.Dispose();
            Resources.Pixel.Dispose();
        }

        protected override void Update(GameTime gameTime)
        {
            if (_currentState == GameState.Exiting)
                Exit();
            stateWindows[_currentState].Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (_currentState == GameState.Exiting)
                return;
            stateWindows[_currentState].Draw(spriteBatch);
            base.Draw(gameTime);
        }


    }
}
