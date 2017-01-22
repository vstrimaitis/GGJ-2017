using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace Game2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Vector2 Battery1Position;
        Vector2 Battery2Position;
        Random Random = new Random();
        const int JumpDrainAmount = 10;
        public const int PlanetBlockSize = 5;
        public const int StarBlockSize = 1;
        public const int CollisionDistance = 30;
        public const float StarVisibilityThreshold = 0.5f;
        const int MaxStarHeight = 200;
        const float PulseSpeed = 1 / 60f;
        const float WorldRotationSpeed = 1 / 120f;
        const float BackgroundRotationSpeed = -1 / 500f;
        const float Gravity = 100f;
        const float PlayerHorizontalSpeed = 0.4f;//0.9f;
        const float PlayerJumpSpeed = 2f;
        float _worldAngle = 0;
        float _backgroundAngle = 0;
        float _pulseTime = 0;


        Matrix _backgroundMatrix;
        Matrix _worldMatrix;
        Matrix _starMatrix;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        int _width;
        int _height;
        World _world = new World();

        HashSet<Star> _starsToDelete = new HashSet<Star>();

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
            Resources.Pixel = new Texture2D(GraphicsDevice, 1, 1);
            Resources.Pixel.SetData(new Color[] { Color.White });

            //_world.Entities.Add(new Entity(Vector2.Zero, new Vector2(1, 1), Vector2.Zero, _world));
            
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
            Resources.Planet = Content.Load<Texture2D>("planet");
            Resources.Sheriff = Content.Load<Texture2D>("player");
            Resources.SheriffHat = Content.Load<Texture2D>("player_hat_overlay");
            Resources.Thief = Content.Load<Texture2D>("thief");
            Resources.ThiefHat = Content.Load<Texture2D>("thief_overlay");
            Resources.Stars = new Texture2D[] { Content.Load<Texture2D>("star_1"), Content.Load<Texture2D>("star_2") };
            Resources.BatteryOutline = Content.Load<Texture2D>("battery");
            Resources.BatteryFill = Content.Load<Texture2D>("battery_overlay");
            Resources.Sun = Content.Load<Texture2D>("sun");
            Resources.Background = new Texture2D(GraphicsDevice, 1, 1000);

            Resources.BackgroundMusic = Content.Load<Song>("background_music_2");
            Resources.StarCollectSound = Content.Load<SoundEffect>("star_capture");
            Resources.DeathSound = Content.Load<SoundEffect>("death_sound");

            Resources.Font = Content.Load<SpriteFont>("font");
         
            GenerateSkyGradient();
            LoadPlanetBlocks();

            for (int i = 0; i < 20; i++)
            {
                if (!GenerateStar())
                    i--;
            }


            //BatteryPosition = new Vector2(10, _height - Graphics.BatteryOutline.Height - 500);
            Battery1Position = new Vector2(10, _height - Resources.BatteryOutline.Height * 2 - 10);
            Battery2Position = new Vector2(_width - Resources.BatteryOutline.Width * 2 - 10, _height - Resources.BatteryOutline.Height * 2 - 10);

            _world.Sheriff = new PlayerEntity(-Vector2.UnitY * 25, Vector2.Zero, _world, 1/15f, 1/10f, Resources.Sheriff, Resources.SheriffHat);

            _world.Thief = new PlayerEntity(Vector2.UnitY * 25, Vector2.Zero, _world, 1/20f, 1/5f, Resources.Thief, Resources.ThiefHat);

            _world.Sheriff.OnDeath += (sender, args) =>
            {
                MediaPlayer.Stop();
                Resources.DeathSound.Play();
                System.Windows.Forms.MessageBox.Show("Thief wins!");
                this.Exit();
            };

            _world.Thief.OnDeath += (sender, args) =>
            {
                MediaPlayer.Stop();
                Resources.DeathSound.Play();
                System.Windows.Forms.MessageBox.Show("Sheriff wins!");
                this.Exit();
            };

            Star.OnCollision += (sender, args) =>
            {
                var hitStar = sender as Star;
                var player = args as PlayerEntity;
                _starsToDelete.Add(hitStar);
                player.Score++;
                Resources.StarCollectSound.Play();
            };
            LoadPlayerBlocks();

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(Resources.BackgroundMusic);
        }

        private void LoadPlayerBlocks()
        {
            LoadSheriffBlocks();
            LoadThiefBlocks();   
        }

        private void LoadThiefBlocks()
        {
            var colors1D = new Color[Resources.Thief.Width * Resources.Thief.Height];
            Resources.Thief.GetData(colors1D);
            int startX = Resources.Thief.Width, endX = 0, startY = Resources.Thief.Height, endY = 0;
            for (int xx = 0; xx < Resources.Thief.Width; xx++)
            {
                for (int yy = 0; yy < Resources.Thief.Height; yy++)
                {
                    var c = colors1D[xx + yy * Resources.Thief.Width];
                    if (c.A != 0)
                    {
                        startX = Math.Min(startX, xx);
                        endX = Math.Max(endX, xx);
                        startY = Math.Min(startY, yy);
                        endY = Math.Max(endY, yy);
                    }
                }
            }
            _world.Sheriff.Bounds = new Vector2(endX - startX + 1, endY - startY + 1);
            for (int xx = 0; xx < Resources.Thief.Width; xx++)
            {
                for (int yy = 0; yy < Resources.Thief.Height; yy++)
                {
                    var c = colors1D[xx + yy * Resources.Thief.Width];
                    if (c.A != 0)
                    {
                        var block = new Block((int)(xx + _world.Sheriff.Position.X), (int)(yy + _world.Sheriff.Position.Y), PlayerEntity.BlockSize, BlockType.Player, c);
                        block.AbsolutePosition = block.Position;
                        block.Coordinates = new Vector2(xx - startX, yy - startY);
                        _world.Sheriff.Blocks.Add(block);
                        if (xx == Resources.Thief.Width / 2 && yy == Resources.Thief.Height - 1)
                            _world.Sheriff.ReferenceBlock = block;
                    }
                }
            }
        }

        private void LoadSheriffBlocks()
        {
            var colors1D = new Color[Resources.Sheriff.Width * Resources.Sheriff.Height];
            Resources.Sheriff.GetData(colors1D);
            int startX = Resources.Sheriff.Width, endX = 0, startY = Resources.Sheriff.Height, endY = 0;
            for (int xx = 0; xx < Resources.Sheriff.Width; xx++)
            {
                for (int yy = 0; yy < Resources.Sheriff.Height; yy++)
                {
                    var c = colors1D[xx + yy * Resources.Sheriff.Width];
                    if (c.A != 0)
                    {
                        startX = Math.Min(startX, xx);
                        endX = Math.Max(endX, xx);
                        startY = Math.Min(startY, yy);
                        endY = Math.Max(endY, yy);
                    }
                }
            }
            _world.Sheriff.Bounds = new Vector2(endX - startX + 1, endY - startY + 1);
            for (int xx = 0; xx < Resources.Sheriff.Width; xx++)
            {
                for (int yy = 0; yy < Resources.Sheriff.Height; yy++)
                {
                    var c = colors1D[xx + yy * Resources.Sheriff.Width];
                    if (c.A != 0)
                    {
                        var block = new Block((int)(xx + _world.Sheriff.Position.X), (int)(yy + _world.Sheriff.Position.Y), PlayerEntity.BlockSize, BlockType.Player, c);
                        block.AbsolutePosition = block.Position;
                        block.Coordinates = new Vector2(xx - startX, yy - startY);
                        _world.Sheriff.Blocks.Add(block);
                        if (xx == Resources.Sheriff.Width / 2 && yy == Resources.Sheriff.Height - 1)
                            _world.Sheriff.ReferenceBlock = block;
                    }
                }
            }
        }

        private bool GenerateStar()
        {
            double angle = Random.NextDouble() * 2 * Math.PI;
            double r = Random.NextDouble() * MaxStarHeight + _world.PlanetRadius;
            int x = (int)(r * Math.Cos(angle));
            int y = (int)(r * Math.Sin(angle));
            var star = new Star(x, y, Resources.Stars[Random.Next(0, 2)], _world);
            foreach (var s in _world.Stars)
                if (star.Intersects(s))
                    return false;
            _world.Stars.Add(star);
            return true;
        }

        private void GenerateSkyGradient()
        {
            var colors = new Color[Resources.Background.Width * Resources.Background.Height];
            for(int y = 0; y < Resources.Background.Height; y++)
            {
                float c = (float)y / (Resources.Background.Height-1);
                for (int x = 0; x < Resources.Background.Width; x++)
                    colors[x + y * Resources.Background.Width] = GraphicsHelper.Interpolate(Resources.LightSky, Resources.DarkSky, c);
            }
            Resources.Background.SetData(colors);
        }

        private void LoadPlanetBlocks()
        {
            var colors1D = new Color[Resources.Planet.Width * Resources.Planet.Height];
            Resources.Planet.GetData(colors1D);
            int startX = 0, startY = 0, endX = 0, endY = 0;
            for (int x = 0; x < Resources.Planet.Width; x++)
            {
                for (int y = 0; y < Resources.Planet.Height; y++)
                {
                    var c = colors1D[x + y * Resources.Planet.Width]; ;
                    if (c.A != 0)
                    {
                        _world.PlanetBlocks.Add(new Block(x - Resources.Planet.Width / 2, y - Resources.Planet.Height / 2, PlanetBlockSize, BlockType.Ground, c));
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
            Resources.Planet.Dispose();
            Resources.Pixel.Dispose();
        }

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
            UpdateVariables();
            UpdateKeyboard();
            UpdateAbsolutePositions();
            UpdateStars();
            AddGravity();
            base.Update(gameTime);
        }

        private void AddGravity()
        {
            var g = -_world.Sheriff.Position;
            g.Normalize();
            g *= (Gravity / _world.Sheriff.Position.LengthSquared());
            _world.Sheriff.Velocity += g;
            _world.Sheriff.Update();

            g = -_world.Thief.Position;
            g.Normalize();
            g *= (Gravity / _world.Thief.Position.LengthSquared());
            _world.Thief.Velocity += g;
            _world.Thief.Update();
        }

        private void UpdateStars()
        {
            var prevSun = _world.SunPosition;
            _world.SunPosition = new Vector2((float)Math.Cos(_backgroundAngle + _worldAngle), (float)Math.Sin(_backgroundAngle + _worldAngle));

            foreach (var s in _world.Stars)
                s.Update();

            if (_starsToDelete.Count > 0)
            {
                foreach (var s in _starsToDelete)
                    _world.Stars.Remove(s);
                _starsToDelete.Clear();
            }

            _world.SunPosition = prevSun;
        }

        private void UpdateAbsolutePositions()
        {
            foreach (var b in _world.Sheriff.Blocks)
            {
                float angle = (float)Math.Atan2(_world.Sheriff.Position.Y, _world.Sheriff.Position.X) + MathHelper.PiOver2;
                var translationMatrix = Matrix.CreateTranslation(new Vector3(-_world.Sheriff.Bounds.X / 2, -_world.Sheriff.Bounds.Y, 0));
                var rotationMatrix = Matrix.CreateRotationZ(angle);

                var t = Vector2.Transform(b.Position, rotationMatrix * translationMatrix);
                var p = Vector2.Transform(_world.Sheriff.ReferenceBlock.Position, rotationMatrix * translationMatrix);
                var diff = t - p;
                var tmp = _world.Sheriff.Position * PlanetBlockSize + diff;
                b.AbsolutePosition = Vector2.Transform(tmp, _worldMatrix);
            }

            foreach (var b in _world.Thief.Blocks)
            {
                float angle = (float)Math.Atan2(_world.Thief.Position.Y, _world.Thief.Position.X) + MathHelper.PiOver2;
                var translationMatrix = Matrix.CreateTranslation(new Vector3(-_world.Thief.Bounds.X / 2, -_world.Thief.Bounds.Y, 0));
                var rotationMatrix = Matrix.CreateRotationZ(angle);

                var t = Vector2.Transform(b.Position, rotationMatrix * translationMatrix);
                var p = Vector2.Transform(_world.Thief.ReferenceBlock.Position, rotationMatrix * translationMatrix);
                var diff = t - p;
                var tmp = _world.Thief.Position * PlanetBlockSize + diff;
                b.AbsolutePosition = Vector2.Transform(tmp, _worldMatrix);
            }
            _world.Sheriff.AbsolutePosition = Vector2.Transform(_world.Sheriff.Position * PlayerEntity.BlockSize, _worldMatrix); // transform to absolute coordinates
            _world.Thief.AbsolutePosition = Vector2.Transform(_world.Thief.Position * PlayerEntity.BlockSize, _worldMatrix); // transform to absolute coordinates
            foreach (var b in _world.PlanetBlocks)
                b.AbsolutePosition = Vector2.Transform(b.Position * PlanetBlockSize, _worldMatrix);
            foreach (var s in _world.Stars)
                foreach (var b in s.Blocks)
                    b.AbsolutePosition = Vector2.Transform(b.Position * StarBlockSize, _starMatrix);

        }

        private void UpdateKeyboard()
        {
            var state = Keyboard.GetState();

            if (state.IsKeyDown(Keys.Left) && _world.Sheriff.IsOnGround)
            {
                var v = _world.Sheriff.Position;
                var vv = new Vector2(v.Y, -v.X);
                _world.Sheriff.Velocity += vv / v.Length() * PlayerHorizontalSpeed;
            }
            else if (state.IsKeyDown(Keys.Right) && _world.Sheriff.IsOnGround)
            {
                var v = _world.Sheriff.Position;
                var vv = new Vector2(-v.Y, v.X);
                _world.Sheriff.Velocity += vv / v.Length() * PlayerHorizontalSpeed;
            }
            if (state.IsKeyDown(Keys.Up) && _world.Sheriff.IsOnGround)
            {
                _world.Sheriff.Velocity += _world.Sheriff.Position / _world.Sheriff.Position.Length() * PlayerJumpSpeed;
                _world.Sheriff.DrainPower(JumpDrainAmount);
            }


            if (state.IsKeyDown(Keys.A) && _world.Thief.IsOnGround)
            {
                var v = _world.Thief.Position;
                var vv = new Vector2(v.Y, -v.X);
                _world.Thief.Velocity += vv / v.Length() * PlayerHorizontalSpeed;
            }
            else if (state.IsKeyDown(Keys.D) && _world.Thief.IsOnGround)
            {
                var v = _world.Thief.Position;
                var vv = new Vector2(-v.Y, v.X);
                _world.Thief.Velocity += vv / v.Length() * PlayerHorizontalSpeed;
            }
            if (state.IsKeyDown(Keys.W) && _world.Thief.IsOnGround)
            {
                _world.Thief.Velocity += _world.Thief.Position / _world.Thief.Position.Length() * PlayerJumpSpeed;
                _world.Thief.DrainPower(JumpDrainAmount);
            }
        }

        private void UpdateVariables()
        {
            _worldAngle += WorldRotationSpeed;
            _backgroundAngle += BackgroundRotationSpeed - WorldRotationSpeed;
            _pulseTime += PulseSpeed;
            _world.SunPosition = new Vector2((float)Math.Cos(_backgroundAngle), (float)Math.Sin(_backgroundAngle));

            _backgroundMatrix = Matrix.CreateRotationZ(_backgroundAngle + _worldAngle + (float)Math.PI / 2) * Matrix.CreateTranslation(_width / 2, _height / 2, 0);
            float scale = 1 + (float)(Math.Sin(_pulseTime * 2) + Math.Cos(_pulseTime * 3)) / 10;
            _worldMatrix = Matrix.CreateScale(scale) * Matrix.CreateRotationZ(_worldAngle) * Matrix.CreateTranslation(_width / 2, _height / 2, 0);
            _starMatrix = Matrix.CreateScale(0.6f) * Matrix.CreateTranslation(_width / 2, _height / 2, 0);
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            DrawBackground();
            DrawStars();
            DrawWorld();
            DrawBatteries();
            DrawScore();
            //DrawDebugBlocks();

            base.Draw(gameTime);
        }

        private void DrawDebugBlocks()
        {
            spriteBatch.Begin();
            //spriteBatch.Draw(Resources.Pixel, new Rectangle((int)_world.Player.AbsolutePosition.X, (int)_world.Player.AbsolutePosition.Y, 10, 10), Color.Red);
            foreach (var b in _world.PlanetBlocks)
                spriteBatch.Draw(Resources.Pixel, new Rectangle((int)b.AbsolutePosition.X, (int)b.AbsolutePosition.Y, 10, 10), Color.Yellow);
            foreach(var s in _world.Stars)
                foreach (var b in s.Blocks)
                    spriteBatch.Draw(Resources.Pixel, new Rectangle((int)b.AbsolutePosition.X, (int)b.AbsolutePosition.Y, 10, 10), Color.Pink);

            foreach (var b in _world.Sheriff.Blocks)
                spriteBatch.Draw(Resources.Pixel, new Rectangle((int)b.AbsolutePosition.X, (int)b.AbsolutePosition.Y, 5,5), Color.Black);
            spriteBatch.Draw(Resources.Pixel, new Rectangle((int)_world.Sheriff.ReferenceBlock.AbsolutePosition.X, (int)_world.Sheriff.ReferenceBlock.AbsolutePosition.Y, 5,5), Color.Red);
            var abs = Vector2.Transform(_world.Sheriff.Position, _worldMatrix);
            spriteBatch.Draw(Resources.Pixel, new Rectangle((int)abs.X, (int)abs.Y, 5, 5), Color.Blue);


            foreach (var b in _world.Sheriff.Blocks)
                spriteBatch.Draw(Resources.Pixel, new Rectangle((int)b.Position.X, (int)b.Position.Y, 5, 5), Color.Black);
            spriteBatch.Draw(Resources.Pixel, new Rectangle((int)_world.Sheriff.ReferenceBlock.Position.X, (int)_world.Sheriff.ReferenceBlock.Position.Y, 5, 5), Color.Red);
            spriteBatch.Draw(Resources.Pixel, new Rectangle((int)_world.Sheriff.Position.X, (int)_world.Sheriff.Position.Y, 5, 5), Color.Blue);

            spriteBatch.Draw(Resources.Pixel, new Rectangle(0, 0, 10, 10), Color.Pink);
            var worldAbs = Vector2.Transform(Vector2.Zero, _worldMatrix);
            spriteBatch.Draw(Resources.Pixel, new Rectangle((int)worldAbs.X, (int)worldAbs.Y, 10, 10), Color.Pink);


            spriteBatch.End();
        }

        private void DrawStars()
        {
            spriteBatch.Begin(transformMatrix: _starMatrix);
            var prevSun = _world.SunPosition;
            _world.SunPosition = new Vector2((float)Math.Cos(_backgroundAngle + _worldAngle), (float)Math.Sin(_backgroundAngle + _worldAngle));
            foreach (var s in _world.Stars)
                s.Draw(spriteBatch);
            _world.SunPosition = prevSun;
            spriteBatch.End();
        }

        private void DrawWorld()
        {
            spriteBatch.Begin(transformMatrix: _worldMatrix);
            foreach (var b in _world.PlanetBlocks)
                b.Draw(spriteBatch, b.Position.Dot(_world.SunPosition) * 0.15f);
            int sunX = (int)(_world.SunPosition.X * (_world.PlanetRadius));
            int sunY = (int)(_world.SunPosition.Y * (_world.PlanetRadius));
            spriteBatch.Draw(Resources.Sun, new Rectangle(sunX, sunY, Resources.Sun.Width * 2, Resources.Sun.Height * 2), Color.White);
            _world.Sheriff.Draw(spriteBatch);
            _world.Thief.Draw(spriteBatch);
            spriteBatch.End();
        }

        private void DrawBackground()
        {
            spriteBatch.Begin(transformMatrix: _backgroundMatrix);
            spriteBatch.Draw(Resources.Pixel, new Rectangle(-5000, -5000, 10000, 5000), Resources.LightSky);
            spriteBatch.Draw(Resources.Pixel, new Rectangle(-5000, 0, 10000, 5000), Resources.DarkSky);
            spriteBatch.Draw(Resources.Background, new Rectangle(-5000, -50 * 3, 10000, 100 * 3), Color.White);
            spriteBatch.End();
        }

        private void DrawScore()
        {
            Rectangle sheriffScoreBoundaries = new Rectangle(10 + Resources.BatteryOutline.Width * 2 + 20,
                                                             _height - Resources.BatteryOutline.Height * 2 - 10,
                                                             50,
                                                             Resources.BatteryOutline.Height * 2);

            Rectangle sheriffTitleBoundaries = new Rectangle(10,
                                                             _height - Resources.BatteryOutline.Height * 2 - 10 - 50,
                                                             150,
                                                             50);

            Rectangle thiefScoreBoundaries = new Rectangle(_width - 10 - Resources.BatteryOutline.Width * 2 - 20 - 50,
                                                           _height - Resources.BatteryOutline.Height * 2 - 10,
                                                           50,
                                                           Resources.BatteryOutline.Height * 2);

            Rectangle thiefTitleBoundaries = new Rectangle(_width-160,
                                                           _height - Resources.BatteryOutline.Height * 2 - 10 - 50,
                                                           150,
                                                           50);
            spriteBatch.Begin();

            DrawString(Resources.Font, _world.Sheriff.Score.ToString(), sheriffScoreBoundaries, Resources.FontColor);
            DrawString(Resources.Font, _world.Thief.Score.ToString(), thiefScoreBoundaries, Resources.FontColor);
            DrawString(Resources.Font, "Sheriff".ToUpper(), sheriffTitleBoundaries, Resources.FontColor);
            DrawString(Resources.Font, "Thief".ToUpper(), thiefTitleBoundaries, Resources.FontColor);
            spriteBatch.End();
        }

        private void DrawString(SpriteFont font, string strToDraw, Rectangle boundaries, Color color)
        {
            Vector2 size = font.MeasureString(strToDraw);

            float xScale = (boundaries.Width / size.X);
            float yScale = (boundaries.Height / size.Y);

            float scale = Math.Min(xScale, yScale);

            int strWidth = (int)Math.Round(size.X * scale);
            int strHeight = (int)Math.Round(size.Y * scale);
            Vector2 position = new Vector2();
            position.X = (((boundaries.Width - strWidth) / 2) + boundaries.X);
            position.Y = (((boundaries.Height - strHeight) / 2) + boundaries.Y);

            float rotation = 0.0f;
            Vector2 spriteOrigin = new Vector2(0, 0);
            float spriteLayer = 0.0f;
            SpriteEffects spriteEffects = new SpriteEffects();
            
            spriteBatch.DrawString(font, strToDraw, position, color, rotation, spriteOrigin, scale, spriteEffects, spriteLayer);
        }

        private void DrawBatteries()
        {
            var percentage1 = _world.Sheriff.Power / 100;
            var color1 = GraphicsHelper.Interpolate(Color.Red, new Color(0, 255, 0), percentage1);
            var length1 = Resources.BatteryFill.Width * percentage1;

            var percentage2 = _world.Thief.Power / 100;
            var color2 = GraphicsHelper.Interpolate(Color.Red, new Color(0, 255, 0), percentage2);
            var length2 = Resources.BatteryFill.Width * percentage2;
            spriteBatch.Begin(transformMatrix: Matrix.CreateScale(2));
            spriteBatch.Draw(Resources.BatteryOutline, new Rectangle((int)Battery1Position.X / 2, (int)Battery1Position.Y / 2, Resources.BatteryOutline.Width, Resources.BatteryOutline.Height), Color.White);
            spriteBatch.Draw(Resources.BatteryFill, new Rectangle((int)Battery1Position.X / 2, (int)Battery1Position.Y / 2, (int)(Resources.BatteryFill.Width * percentage1), Resources.BatteryFill.Height), color1);

            spriteBatch.Draw(Resources.BatteryOutline, new Rectangle((int)Battery2Position.X / 2, (int)Battery2Position.Y / 2, Resources.BatteryOutline.Width, Resources.BatteryOutline.Height), Color.White);
            spriteBatch.Draw(Resources.BatteryFill, new Rectangle((int)Battery2Position.X / 2, (int)Battery2Position.Y / 2, (int)(Resources.BatteryFill.Width * percentage2), Resources.BatteryFill.Height), color2);
            spriteBatch.End();
        }
        #endregion

        public static bool IsLit(Vector2 point, Vector2 sunPosition)
        {
            return point.Dot(sunPosition) > 0;
        }

        
    }
}
