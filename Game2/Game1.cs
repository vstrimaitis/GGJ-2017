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
        const float WorldRotationSpeed = 1 / 120f;
        const float Gravity = 100f;
        const float PlayerHorizontalSpeed = 0.9f;
        const float PlayerJumpSpeed = 1.5f;
        float _worldAngle = 0;

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
            _world.Player = new Entity(Vector2.UnitX * 25, Vector2.Zero, _world);
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
            var earth = Content.Load<Texture2D>("earth");
            var colors1D = new Color[earth.Width * earth.Height];
            earth.GetData(colors1D);
            for (int x = 0; x < earth.Width; x++)
            {
                for (int y = 0; y < earth.Height; y++)
                {
                    var c = colors1D[x + y * earth.Width]; ;
                    if(c.A != 0)
                        _world.Blocks.Add(new Block(x - earth.Width/2, y - earth.Height/2, BlockType.Ground, c));
                }
            }
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
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

            // TODO: Add your drawing code here
            spriteBatch.Begin(transformMatrix: Matrix.CreateRotationZ(_worldAngle) *Matrix.CreateTranslation(_width/2, _height/2, 0));
            
            foreach (var b in _world.Blocks)
            {
                b.Draw(spriteBatch);
            }
            _world.Player.Draw(spriteBatch);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
