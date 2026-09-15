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
        
        // Font
        private SpriteFont _font;
        
        // House positions
        private Vector2 _housePosition;
        private Vector2 _roofPosition;
        private Vector2 _doorPosition;

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

            // Stack them perfectly:
            _roofPosition = new Vector2(centerX, 30);  // Moved UP so it sits on top of the wall
            _housePosition = new Vector2(centerX, 180); // Wall sits directly below roof
            _doorPosition = new Vector2(centerX, 200);  // Door sits directly below wall
    
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            // Load textures
            _houseTexture = Content.Load<Texture2D>("house");
            _roofTexture = Content.Load<Texture2D>("roof");
            _doorTexture = Content.Load<Texture2D>("door");
            
            // Load font
            _font = Content.Load<SpriteFont>("spritefont");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            _spriteBatch.Begin();
            
            // Draw the house components
            _spriteBatch.Draw(_houseTexture, _housePosition, Color.White);
            _spriteBatch.Draw(_roofTexture, _roofPosition, Color.White);
            _spriteBatch.Draw(_doorTexture, _doorPosition, Color.White);
            
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