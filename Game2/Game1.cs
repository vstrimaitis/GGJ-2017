using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Game2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        const int MinBlockSize = 4;
        const int MaxBlockSize = 8;
        const float PulseSpeed = 1 / 60f;
        const float WorldRotationSpeed = 1 / 120f;
        const float BackgroundRotationSpeed = -1 / 500f;
        const float Gravity = 100f;
        const float PlayerHorizontalSpeed = 0.4f;//0.9f;
        const float PlayerJumpSpeed = 1.5f;
        float _worldAngle = 0;
        float _backgroundAngle = 0;
        float _pulseTime = 0;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int _width;
        int _height;
        World _world = new World();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = _height = 700;
            graphics.PreferredBackBufferWidth = _width= 1500;
            Content.RootDirectory = "Content";
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
            Graphics.Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Graphics.Pixel.SetData(new Color[] { Color.White });

            //_world.Entities.Add(new Entity(Vector2.Zero, new Vector2(1, 1), Vector2.Zero, _world));
            _world.Player = new Entity(-Vector2.UnitY * 25, Vector2.Zero, _world);
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
            Graphics.Planet = Content.Load<Texture2D>("planet");
            Graphics.Player = Content.Load<Texture2D>("player");
            Graphics.Background = new Texture2D(GraphicsDevice, 1, 1000);// Content.Load<Texture2D>("background");
            GenerateSkyGradient();
            LoadPlanetBlocks();
        }

        private void GenerateSkyGradient()
        {
            var colors = new Color[Graphics.Background.Width * Graphics.Background.Height];
            for(int y = 0; y < Graphics.Background.Height; y++)
            {
                float c = (float)y / (Graphics.Background.Height-1);
                for (int x = 0; x < Graphics.Background.Width; x++)
                    colors[x + y * Graphics.Background.Width] = Graphics.Interpolate(Graphics.LightSky, Graphics.DarkSky, c);
            }
            Graphics.Background.SetData(colors);
        }
        
        private void LoadPlanetBlocks()
        {
            var colors1D = new Color[Graphics.Planet.Width * Graphics.Planet.Height];
            Graphics.Planet.GetData(colors1D);
            for (int x = 0; x < Graphics.Planet.Width; x++)
            {
                for (int y = 0; y < Graphics.Planet.Height; y++)
                {
                    var c = colors1D[x + y * Graphics.Planet.Width]; ;
                    if (c.A != 0)
                        _world.Blocks.Add(new Block(x - Graphics.Planet.Width / 2, y - Graphics.Planet.Height / 2, BlockType.Ground, c));
                }
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Graphics.Planet.Dispose();
            Graphics.Pixel.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            _worldAngle += WorldRotationSpeed;
            _backgroundAngle += BackgroundRotationSpeed - WorldRotationSpeed;
            _pulseTime += PulseSpeed;
            _world.SunPosition = new Vector2((float)Math.Cos(_backgroundAngle), (float)Math.Sin(_backgroundAngle));

            var state = Keyboard.GetState();
            
            if (state.IsKeyDown(Keys.Left) && _world.Player.IsOnGround)
            {
                var v = _world.Player.Position;
                var vv = new Vector2(v.Y, -v.X);
                _world.Player.Velocity += vv / v.Length() * PlayerHorizontalSpeed;
            }
            else if (state.IsKeyDown(Keys.Right) && _world.Player.IsOnGround)
            {
                var v = _world.Player.Position;
                var vv = new Vector2(-v.Y, v.X);
                _world.Player.Velocity += vv / v.Length() * PlayerHorizontalSpeed;
            }
            if (state.IsKeyDown(Keys.Up) && _world.Player.IsOnGround)
            {
                _world.Player.Velocity += _world.Player.Position / _world.Player.Position.Length() * PlayerJumpSpeed;
            }

            //_world.Player.Velocity -= _world.Player.Position * Gravity;
            var g = -_world.Player.Position;
            g.Normalize();
            g *= (Gravity / _world.Player.Position.LengthSquared());
            _world.Player.Velocity += g;
            _world.Player.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            spriteBatch.Begin(transformMatrix: Matrix.CreateRotationZ(_backgroundAngle+_worldAngle+(float)Math.PI/2) * Matrix.CreateTranslation(_width / 2, _height / 2, 0));
            spriteBatch.Draw(Graphics.Pixel, new Rectangle(-5000, -5000, 10000, 5000), Graphics.LightSky);
            spriteBatch.Draw(Graphics.Pixel, new Rectangle(-5000, 0, 10000, 5000), Graphics.DarkSky);
            spriteBatch.Draw(Graphics.Background, new Rectangle(-5000, -50*3, 10000, 100*3), Color.White);
            spriteBatch.End();

            // TODO: Add your drawing code here
            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(1 + (float)Math.Sin(_pulseTime) / 5) * Matrix.CreateRotationZ(_worldAngle) * Matrix.CreateTranslation(_width/2, _height/2, 0));
            
            foreach (var b in _world.Blocks)
            {
                b.Draw(spriteBatch, b.Position.Dot(_world.SunPosition) * 0.15f);
            }
            spriteBatch.Draw(Graphics.Pixel, new Rectangle((int)(_world.SunPosition.X * 200), (int)(_world.SunPosition.Y * 200), 10, 10), Color.Yellow);
            _world.Player.Draw(spriteBatch);
            spriteBatch.End();


            
            base.Draw(gameTime);
        }

        public static bool IsLit(Vector2 point, Vector2 sunPosition)
        {
            return point.Dot(sunPosition) > 0;
        }

        
    }
}
