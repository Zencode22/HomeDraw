using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HomeDraw.World
{
    /// <summary>
    /// The static house scene: grass, house body, roof, and door.
    /// Owns its textures, positions, and drawing order.
    /// </summary>
    public class House
    {
        // Textures
        private readonly Texture2D _houseTexture;
        private readonly Texture2D _roofTexture;
        private readonly Texture2D _doorTexture;
        private readonly Texture2D _grassTexture;

        // Positions
        private readonly Vector2 _housePosition;
        private readonly Vector2 _roofPosition;
        private readonly Vector2 _doorPosition;
        private readonly Vector2 _grassPosition;

        /// <summary>Y coordinate where the ground surface starts.</summary>
        public const float GroundY = 450f;

        public House(Texture2D houseTexture, Texture2D roofTexture,
                     Texture2D doorTexture, Texture2D grassTexture)
        {
            _houseTexture = houseTexture;
            _roofTexture = roofTexture;
            _doorTexture = doorTexture;
            _grassTexture = grassTexture;

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
        }

        /// <summary>
        /// Draws the background layers (grass first, then the house parts).
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the grass first (background layer - behind the house)
            spriteBatch.Draw(_grassTexture, _grassPosition, Color.White);

            // Draw the house components (in front of grass)
            spriteBatch.Draw(_houseTexture, _housePosition, Color.White);
            spriteBatch.Draw(_roofTexture, _roofPosition, Color.White);
            spriteBatch.Draw(_doorTexture, _doorPosition, Color.White);
        }
    }
}
