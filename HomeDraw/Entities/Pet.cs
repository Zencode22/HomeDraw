using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HomeDraw.Entities
{
    /// <summary>
    /// A player-controlled pet with simple platformer physics.
    /// Each player controls one pet via its own set of keys.
    /// </summary>
    public class Pet
    {
        // Physics constants (shared by all pets)
        private const float JumpForce = -400f;
        private const float Gravity = 800f;
        private const float MoveSpeed = 200f;
        private const int Width = 50;
        private const int Height = 50;

        public Vector2 Position { get; private set; }
        public Color Tint { get; }
        public string Name { get; }

        // Screen bounds used to keep the pet on-screen
        private float _minX = float.MinValue;
        private float _maxX = float.MaxValue;

        /// <summary>Sets the horizontal bounds the pet is clamped to.</summary>
        public void SetBounds(float minX, float maxX)
        {
            _minX = minX;
            _maxX = maxX;
            ClampToScreen();
        }

        // Controls for this pet (one per player) — exposed so the HUD can describe them
        private readonly Keys _moveLeftKey;
        private readonly Keys _moveRightKey;
        private readonly Keys _jumpKey;

        public Keys MoveLeftKey => _moveLeftKey;
        public Keys MoveRightKey => _moveRightKey;
        public Keys JumpKey => _jumpKey;

        private float _velocityX;
        private float _velocityY;
        private bool _isJumping;

        private readonly Texture2D _texture;
        private readonly float _groundY;

        public Pet(Texture2D texture, Vector2 startPosition, Color tint, string name,
                   Keys moveLeftKey, Keys moveRightKey, Keys jumpKey,
                   float groundY, int screenWidth)
        {
            _texture = texture;
            _groundY = groundY;

            Position = startPosition;
            Tint = tint;
            Name = name;

            _moveLeftKey = moveLeftKey;
            _moveRightKey = moveRightKey;
            _jumpKey = jumpKey;

            SetBounds(0, screenWidth - Width);
        }

        /// <summary>
        /// Reads input and applies movement, gravity, and screen-bound clamping.
        /// </summary>
        public void Update(GameTime gameTime, KeyboardState keyboard)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Horizontal movement
            _velocityX = 0f;
            if (keyboard.IsKeyDown(_moveLeftKey))
                _velocityX = -MoveSpeed;
            else if (keyboard.IsKeyDown(_moveRightKey))
                _velocityX = MoveSpeed;

            // Jumping
            if (keyboard.IsKeyDown(_jumpKey) && !_isJumping)
            {
                _velocityY = JumpForce;
                _isJumping = true;
            }

            // Apply movement
            Position += new Vector2(_velocityX * deltaTime, 0f);

            // Apply gravity / vertical movement while airborne
            if (_isJumping)
            {
                _velocityY += Gravity * deltaTime;
                Position += new Vector2(0f, _velocityY * deltaTime);

                if (Position.Y >= _groundY)
                {
                    Position = new Vector2(Position.X, _groundY);
                    _velocityY = 0f;
                    _isJumping = false;
                }
            }

            // Keep within screen bounds
            ClampToScreen();
        }

        /// <summary>
        /// Moves this pet by the given offset (used by collision resolution).
        /// Keeps the pet inside the horizontal screen bounds.
        /// </summary>
        public void Push(Vector2 offset)
        {
            Position += offset;
            ClampToScreen();
        }

        private void ClampToScreen()
        {
            Position = new Vector2(MathHelper.Clamp(Position.X, _minX, _maxX), Position.Y);
        }

        /// <summary>
        /// The pet's current bounding box, used for collision detection.
        /// </summary>
        public Rectangle GetBoundingBox() =>
            new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, Tint);
        }
    }
}
