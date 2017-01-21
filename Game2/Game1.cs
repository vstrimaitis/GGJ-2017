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
        Vector2 BatteryPosition;
        Random Random = new Random();
        const int JumpDrainAmount = 10;
        public const int PlanetBlockSize = 5;
        const int MaxStarHeight = 200;
        public const int StarBlockSize = 1;
        const float PulseSpeed = 1 / 60f;
        const float WorldRotationSpeed = 1 / 120f;
        const float BackgroundRotationSpeed = -1 / 500f;
        const float Gravity = 100f;
        const float PlayerHorizontalSpeed = 0.4f;//0.9f;
        const float PlayerJumpSpeed = 2f;
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
            graphics.PreferredBackBufferHeight = _height = 850;
            graphics.PreferredBackBufferWidth = _width = 850;
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
            _world.Player = new PlayerEntity(-Vector2.UnitY * 25, Vector2.Zero, _world);
            _world.Player.OnDeath += (sender, args) =>
            {
                System.Windows.Forms.MessageBox.Show("You ded man ;(");
                this.Exit();
            };
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
            Graphics.PlayerHat = Content.Load<Texture2D>("player_hat_overlay");
            Graphics.Stars = new Texture2D[] { Content.Load<Texture2D>("star_1"), Content.Load<Texture2D>("star_2") };
            Graphics.BatteryOutline = Content.Load<Texture2D>("battery");
            Graphics.BatteryFill = Content.Load<Texture2D>("battery_overlay");
            Graphics.Background = new Texture2D(GraphicsDevice, 1, 1000);// Content.Load<Texture2D>("background");
         
            GenerateSkyGradient();
            LoadPlanetBlocks();

            for (int i = 0; i < 20; i++)
            {
                double angle = Random.NextDouble() * 2 * Math.PI;
                double r = Random.NextDouble() * MaxStarHeight + _world.PlanetRadius;
                int x = (int)(r * Math.Cos(angle));
                int y = (int)(r * Math.Sin(angle));
                _world.Stars.Add(new Star(x, y, Graphics.Stars[Random.Next(0, 2)], _world));
            }


            //BatteryPosition = new Vector2(10, _height - Graphics.BatteryOutline.Height - 500);
            BatteryPosition = new Vector2(10, _height-Graphics.BatteryOutline.Height*2-10);
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
            int startX = 0, startY = 0, endX = 0, endY = 0;
            for (int x = 0; x < Graphics.Planet.Width; x++)
            {
                for (int y = 0; y < Graphics.Planet.Height; y++)
                {
                    var c = colors1D[x + y * Graphics.Planet.Width]; ;
                    if (c.A != 0)
                    {
                        _world.PlanetBlocks.Add(new Block(x - Graphics.Planet.Width / 2, y - Graphics.Planet.Height / 2, PlanetBlockSize, BlockType.Ground, c));
                        endX = Math.Max(endX, x);
                        endY = Math.Max(endY, y);
                        startX = Math.Min(startX, x);
                        startY = Math.Min(startY, y);

                    }
                }
            }

            //_world.PlanetRadius = (int)(Math.Max(endX - startX + 1, endY - startY + 1) * 2.4f * PlanetBlockSize / 2) ;
            _world.PlanetRadius = (int)(Math.Max(endX - startX + 1, endY - startY + 1) / 2 * PlanetBlockSize*2.5f) ;

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
                _world.Player.DrainPower(JumpDrainAmount);
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
            float scale = 1 + (float)(Math.Sin(_pulseTime * 2) + Math.Cos(_pulseTime * 3)) / 10 ;
            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(scale) * Matrix.CreateRotationZ(_worldAngle) * Matrix.CreateTranslation(_width/2, _height/2, 0));
            
            foreach (var b in _world.PlanetBlocks)
            {
                b.Draw(spriteBatch, b.Position.Dot(_world.SunPosition) * 0.15f);
            }
            int sunX = (int)(_world.SunPosition.X * (_world.PlanetRadius));
            int sunY = (int)(_world.SunPosition.Y * ( _world.PlanetRadius));
            spriteBatch.Draw(Graphics.Pixel, new Rectangle(sunX, sunY, 10, 10), Color.Yellow);
            _world.Player.Draw(spriteBatch);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(0.6f) * Matrix.CreateTranslation(_width / 2, _height / 2, 0));
            var prevSun = _world.SunPosition;
            _world.SunPosition = new Vector2((float)Math.Cos(_backgroundAngle + _worldAngle), (float)Math.Sin(_backgroundAngle + _worldAngle));
            foreach (var s in _world.Stars)
            {
                s.Draw(spriteBatch);
            }
            _world.SunPosition = prevSun;
            spriteBatch.End();

            DrawBattery();
            
            base.Draw(gameTime);
        }

        private void DrawBattery()
        {
            var percentage = _world.Player.Power / 100;
            var color = Graphics.Interpolate(Color.Red, new Color(0, 255, 0), percentage);
            var length = Graphics.BatteryFill.Width * percentage;
            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(2));
            spriteBatch.Draw(Graphics.BatteryOutline, new Rectangle((int)BatteryPosition.X / 2, (int)BatteryPosition.Y / 2, Graphics.BatteryOutline.Width, Graphics.BatteryOutline.Height), Color.White);
            spriteBatch.Draw(Graphics.BatteryFill, new Rectangle((int)BatteryPosition.X / 2, (int)BatteryPosition.Y / 2, (int)(Graphics.BatteryFill.Width * percentage), Graphics.BatteryFill.Height), color);
            spriteBatch.End();
        }

        public static bool IsLit(Vector2 point, Vector2 sunPosition)
        {
            return point.Dot(sunPosition) > 0;
        }

        
    }
}
