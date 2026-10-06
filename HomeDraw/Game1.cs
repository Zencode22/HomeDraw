using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HomeDraw
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        
        // Textures
        private Texture2D _houseTexture;
        private Texture2D _roofTexture;
        private Texture2D _doorTexture;
        private Texture2D _grassTexture;
        private Texture2D _petTexture;
        
        // Font
        private SpriteFont _font;
        
        // House positions
        private Vector2 _housePosition;
        private Vector2 _roofPosition;
        private Vector2 _doorPosition;
        
        // Grass position
        private Vector2 _grassPosition;
        
        // Pet properties (player 1: A/D move, Space jump)
        private Vector2 _pet1Position;
        private float _pet1VelocityY;
        private float _pet1VelocityX;
        private bool _isPet1Jumping;

        // Pet properties (player 2: Left/Right move, Up jump)
        private Vector2 _pet2Position;
        private float _pet2VelocityY;
        private float _pet2VelocityX;
        private bool _isPet2Jumping;

        private const float JumpForce = -400f;
        private const float Gravity = 800f;
        private const float GroundY = 450f;
        private const float MoveSpeed = 200f;
        private const int PetWidth = 50;
        private const int PetHeight = 50;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            // Set window size
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
        }

        protected override void Initialize()
        {
            // Window is 800x600. Center X is 400.
            // The images are 200 wide. So, starting X is 400 - 100 = 300.
            int centerX = 300; 

            // Stack them properly with house sitting on grass:
            // Grass top is at Y=450, house (200px tall) should sit on top of it
            // House bottom at Y=450, so house top at Y=250 (450 - 200)
            _housePosition = new Vector2(centerX, 300); 
            // Roof (200px tall) sits on top of house, so roof bottom at Y=250, roof top at Y=50
            _roofPosition = new Vector2(centerX, 150);  
            // Door sits on the house wall
            _doorPosition = new Vector2(centerX, 320);  
            
            // Grass position - spans the bottom of the screen
            _grassPosition = new Vector2(0, GroundY);
            
            // Pet 1 initial position - starts on the ground near the house
            _pet1Position = new Vector2(500, GroundY);
            _pet1VelocityY = 0f;
            _isPet1Jumping = false;

            // Pet 2 initial position - starts on the ground on the other side
            _pet2Position = new Vector2(600, GroundY);
            _pet2VelocityY = 0f;
            _isPet2Jumping = false;
    
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load textures
            _houseTexture = Content.Load<Texture2D>("house");
            _roofTexture = Content.Load<Texture2D>("roof");
            _doorTexture = Content.Load<Texture2D>("door");
            _grassTexture = Content.Load<Texture2D>("grass");
            _petTexture = Content.Load<Texture2D>("pet");
            
            // Load font
            _font = Content.Load<SpriteFont>("spritefont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboard = Keyboard.GetState();

            // Pet 1: A/D move, Space jump
            _pet1VelocityX = 0f;
            if (keyboard.IsKeyDown(Keys.A))
                _pet1VelocityX = -MoveSpeed;
            else if (keyboard.IsKeyDown(Keys.D))
                _pet1VelocityX = MoveSpeed;

            if (keyboard.IsKeyDown(Keys.Space) && !_isPet1Jumping)
            {
                _pet1VelocityY = JumpForce;
                _isPet1Jumping = true;
            }

            // Pet 2: Left/Right move, Up jump
            _pet2VelocityX = 0f;
            if (keyboard.IsKeyDown(Keys.Left))
                _pet2VelocityX = -MoveSpeed;
            else if (keyboard.IsKeyDown(Keys.Right))
                _pet2VelocityX = MoveSpeed;

            if (keyboard.IsKeyDown(Keys.Up) && !_isPet2Jumping)
            {
                _pet2VelocityY = JumpForce;
                _isPet2Jumping = true;
            }

            // Apply horizontal movement for both pets
            _pet1Position.X += _pet1VelocityX * deltaTime;
            _pet2Position.X += _pet2VelocityX * deltaTime;

            // Keep pets within screen bounds
            _pet1Position.X = MathHelper.Clamp(_pet1Position.X, 0, 800 - PetWidth);
            _pet2Position.X = MathHelper.Clamp(_pet2Position.X, 0, 800 - PetWidth);

            // Apply gravity / vertical movement for pet 1
            if (_isPet1Jumping)
            {
                _pet1VelocityY += Gravity * deltaTime;
                _pet1Position.Y += _pet1VelocityY * deltaTime;

                if (_pet1Position.Y >= GroundY)
                {
                    _pet1Position.Y = GroundY;
                    _pet1VelocityY = 0f;
                    _isPet1Jumping = false;
                }
            }

            // Apply gravity / vertical movement for pet 2
            if (_isPet2Jumping)
            {
                _pet2VelocityY += Gravity * deltaTime;
                _pet2Position.Y += _pet2VelocityY * deltaTime;

                if (_pet2Position.Y >= GroundY)
                {
                    _pet2Position.Y = GroundY;
                    _pet2VelocityY = 0f;
                    _isPet2Jumping = false;
                }
            }

            ResolvePetCollision();

            base.Update(gameTime);
        }

        // Push the two pets apart when their bounding boxes overlap
        private void ResolvePetCollision()
        {
            Rectangle rect1 = new Rectangle((int)_pet1Position.X, (int)_pet1Position.Y, PetWidth, PetHeight);
            Rectangle rect2 = new Rectangle((int)_pet2Position.X, (int)_pet2Position.Y, PetWidth, PetHeight);

            if (!rect1.Intersects(rect2))
                return;

            // Compute penetration depth on each axis
            int overlapLeft = rect1.Right - rect2.Left;
            int overlapRight = rect2.Right - rect1.Left;
            int overlapTop = rect1.Bottom - rect2.Top;
            int overlapBottom = rect2.Bottom - rect1.Top;

            int minOverlapX = Math.Min(overlapLeft, overlapRight);
            int minOverlapY = Math.Min(overlapTop, overlapBottom);

            if (minOverlapX <= minOverlapY)
            {
                // Separate horizontally, splitting the push between both pets
                float push = minOverlapX / 2f;
                if (overlapLeft < overlapRight)
                {
                    _pet1Position.X -= push;
                    _pet2Position.X += push;
                }
                else
                {
                    _pet1Position.X += push;
                    _pet2Position.X -= push;
                }
            }
            else
            {
                // Separate vertically
                float push = minOverlapY / 2f;
                if (overlapTop < overlapBottom)
                {
                    _pet1Position.Y -= push;
                    _pet2Position.Y += push;
                }
                else
                {
                    _pet1Position.Y += push;
                    _pet2Position.Y -= push;
                }
            }

            // Re-clamp after separation so pets stay on screen
            _pet1Position.X = MathHelper.Clamp(_pet1Position.X, 0, 800 - PetWidth);
            _pet2Position.X = MathHelper.Clamp(_pet2Position.X, 0, 800 - PetWidth);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();
            
            // Draw the grass first (background layer - behind the house)
            _spriteBatch.Draw(_grassTexture, _grassPosition, Color.White);
            
            // Draw the house components (in front of grass)
            _spriteBatch.Draw(_houseTexture, _housePosition, Color.White);
            _spriteBatch.Draw(_roofTexture, _roofPosition, Color.White);
            _spriteBatch.Draw(_doorTexture, _doorPosition, Color.White);
            
            // Draw the pets (in front of everything), tinted to tell them apart
            _spriteBatch.Draw(_petTexture, _pet1Position, Color.White);
            _spriteBatch.Draw(_petTexture, _pet2Position, Color.Orange);
            
            // Draw control instructions for both players
            string controls1 = "Pet 1 (Brown): A - Move Left | D - Move Right | Space - Jump";
            Vector2 controls1Size = _font.MeasureString(controls1);
            _spriteBatch.DrawString(_font, controls1, new Vector2((800 - controls1Size.X) / 2, 530), Color.White);

            string controls2 = "Pet 2 (Orange): Left - Move Left | Right - Move Right | Up - Jump";
            Vector2 controls2Size = _font.MeasureString(controls2);
            _spriteBatch.DrawString(_font, controls2, new Vector2((800 - controls2Size.X) / 2, 555), Color.White);
            
            // Draw text label (centered)
            string text = "My House";
            Vector2 textSize = _font.MeasureString(text);
            // Center text horizontally, place it below the door
            _spriteBatch.DrawString(_font, text, new Vector2((800 - textSize.X) / 2, 500), Color.White);
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}