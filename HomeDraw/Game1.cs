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
        
        // Pet properties
        private Vector2 _petPosition;
        private float _petVelocityY;
        private float _petVelocityX;
        private bool _isPetJumping;
        private const float JumpForce = -400f;
        private const float Gravity = 800f;
        private const float GroundY = 450f;
        private const float MoveSpeed = 200f;
        
        // Keyboard state for pet jumping
        private KeyboardState _previousKeyboardState;

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
            
            // Pet initial position - starts on the ground near the house
            _petPosition = new Vector2(500, GroundY);
            _petVelocityY = 0f;
            _isPetJumping = false;
    
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

            // Handle pet jumping with spacebar and movement with A/D keys
            KeyboardState currentKeyboardState = Keyboard.GetState();
            
            // Horizontal movement
            _petVelocityX = 0f;
            if (currentKeyboardState.IsKeyDown(Keys.A))
            {
                _petVelocityX = -MoveSpeed;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.D))
            {
                _petVelocityX = MoveSpeed;
            }
            
            // Apply horizontal movement
            _petPosition.X += _petVelocityX * (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Keep pet within screen bounds
            if (_petPosition.X < 0)
                _petPosition.X = 0;
            if (_petPosition.X > 750) // 800 - 50 (pet width)
                _petPosition.X = 750;
            
            // Jumping
            if (currentKeyboardState.IsKeyDown(Keys.Space) && !_isPetJumping)
            {
                _petVelocityY = JumpForce;
                _isPetJumping = true;
            }
            
            // Apply gravity to pet
            if (_isPetJumping)
            {
                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _petVelocityY += Gravity * deltaTime;
                _petPosition.Y += _petVelocityY * deltaTime;
                
                // Check if pet has landed back on the ground
                if (_petPosition.Y >= GroundY)
                {
                    _petPosition.Y = GroundY;
                    _petVelocityY = 0f;
                    _isPetJumping = false;
                }
            }
            
            _previousKeyboardState = currentKeyboardState;

            base.Update(gameTime);
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
            
            // Draw the pet (in front of everything)
            _spriteBatch.Draw(_petTexture, _petPosition, Color.White);
            
            // Draw control instructions
            string controls = "Controls: A - Move Left | D - Move Right | Space - Jump";
            Vector2 controlsSize = _font.MeasureString(controls);
            _spriteBatch.DrawString(_font, controls, new Vector2((800 - controlsSize.X) / 2, 550), Color.White);
            
            // Draw text label (centered)
            string text = "My House";
            Vector2 textSize = _font.MeasureString(text);
            // Center text horizontally, place it below the door
            _spriteBatch.DrawString(_font, text, new Vector2((800 - textSize.X) / 2, 520), Color.White);
            
            _spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}