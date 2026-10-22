using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HomeDraw.Entities;

namespace HomeDraw.UI
{
    /// <summary>
    /// Draws all on-screen text: control instructions for each pet and the title label.
    /// </summary>
    public class Hud
    {
        private readonly SpriteFont _font;
        private readonly int _screenWidth;

        private const string TitleText = "My House";
        private const float ControlsStartY = 530f;
        private const float ControlsLineSpacing = 25f;
        private const float TitleY = 500f;

        public Hud(SpriteFont font, int screenWidth)
        {
            _font = font;
            _screenWidth = screenWidth;
        }

        /// <summary>
        /// Draws per-pet control instructions plus the centered title.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, IReadOnlyList<Pet> pets)
        {
            // Draw control instructions for each player
            float controlsY = ControlsStartY;
            foreach (Pet pet in pets)
            {
                string controls = BuildControlsText(pet);
                DrawCentered(spriteBatch, controls, controlsY, Color.White);
                controlsY += ControlsLineSpacing;
            }

            // Draw title text (centered horizontally, placed below the door)
            DrawCentered(spriteBatch, TitleText, TitleY, Color.White);
        }

        private static string BuildControlsText(Pet pet) =>
            $"{pet.Name}: {FormatKey(pet.MoveLeftKey)} - Move Left | " +
            $"{FormatKey(pet.MoveRightKey)} - Move Right | " +
            $"{FormatKey(pet.JumpKey)} - Jump";

        // Friendly names for keys that Keys.ToString() would print awkwardly
        private static string FormatKey(Keys key)
        {
            switch (key)
            {
                case Keys.Space: return "Space";
                case Keys.Left: return "Left";
                case Keys.Right: return "Right";
                case Keys.Up: return "Up";
                case Keys.Down: return "Down";
                default: return key.ToString();
            }
        }

        private void DrawCentered(SpriteBatch spriteBatch, string text, float y, Color color)
        {
            Vector2 textSize = _font.MeasureString(text);
            spriteBatch.DrawString(_font, text, new Vector2((_screenWidth - textSize.X) / 2, y), color);
        }
    }
}
