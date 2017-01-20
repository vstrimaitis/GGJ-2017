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
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public static int PixelSize { get; private set; } = 5;
        int _width;
        int _height;
        Texture2D _earth;
        World _world = new World();

        float time;

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
            _earth = Content.Load<Texture2D>("earth");
            var colors = new Color[_earth.Width, _earth.Height];
            var colors1D = new Color[_earth.Width * _earth.Height];
            _earth.GetData(colors1D);
            
            for (int x = 0; x < _earth.Width; x++)
            {
                for (int y = 0; y < _earth.Height; y++)
                {
                    var c = colors1D[x + y * _earth.Width]; ;
                    if(c.A != 0)
                    {
                        _world.Blocks.Add(new Block(x - _earth.Width/2, y - _earth.Height/2, BlockType.Ground, c));
                    }
                    //colors[x, y] = colors1D[x + y * _earth.Width];
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
            time += 1 / 60f;
            var state = Keyboard.GetState();
            var speed = 1f;

            if (state.IsKeyDown(Keys.Left))
            {
                var v = _world.Player.Position;
                var vv = new Vector2(v.Y, -v.X);
                _world.Player.Position += vv / vv.Length() * speed;
            }
            if (state.IsKeyDown(Keys.Right))
            {
                var v = _world.Player.Position;
                var vv = new Vector2(-v.Y, v.X);
                _world.Player.Position += vv / vv.Length() * speed;
            }

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
            spriteBatch.Begin(transformMatrix: Matrix.CreateRotationZ(time)*Matrix.CreateTranslation(_width/2, _height/2, 0));
            
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
