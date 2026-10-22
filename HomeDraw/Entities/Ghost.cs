using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HomeDraw.Entities
{
    /// <summary>
    /// A single ghost. Owns all of its own state: position, direction, speed,
    /// and texture tint. Ghosts wander the play area in straight lines and
    /// bounce off the world bounds; the GhostManager handles ghost-vs-ghost
    /// collisions (direction changes) and pet-vs-ghost grabbing.
    /// </summary>
    public class Ghost
    {
        public const int Width = 40;
        public const int Height = 40;
        private const float Speed = 120f;

        public Vector2 Position { get; private set; }
        public Vector2 Direction { get; private set; }
        public Color Tint { get; }

        private readonly Texture2D _texture;

        // World bounds the ghost bounces off of (set by GhostManager).
        private Rectangle _bounds;

        public Ghost(Texture2D texture, Vector2 startPosition, Color tint, Rectangle bounds)
        {
            _texture = texture;
            Tint = tint;
            _bounds = bounds;

            // Snap the spawn position into the bounds so we never start outside.
            Position = new Vector2(
                MathHelper.Clamp(startPosition.X, _bounds.Left, _bounds.Right - Width),
                MathHelper.Clamp(startPosition.Y, _bounds.Top, _bounds.Bottom - Height));

            Direction = RandomDirection();
        }

        /// <summary>
        /// Moves the ghost along its current direction and bounces it off the
        /// world bounds.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Position += Direction * Speed * deltaTime;

            bool bounced = false;

            if (Position.X <= _bounds.Left)
            {
                Position = new Vector2(_bounds.Left, Position.Y);
                Direction = new Vector2(Math.Abs(Direction.X), Direction.Y);
                bounced = true;
            }
            else if (Position.X >= _bounds.Right - Width)
            {
                Position = new Vector2(_bounds.Right - Width, Position.Y);
                Direction = new Vector2(-Math.Abs(Direction.X), Direction.Y);
                bounced = true;
            }

            if (Position.Y <= _bounds.Top)
            {
                Position = new Vector2(Position.X, _bounds.Top);
                Direction = new Vector2(Direction.X, Math.Abs(Direction.Y));
                bounced = true;
            }
            else if (Position.Y >= _bounds.Bottom - Height)
            {
                Position = new Vector2(Position.X, _bounds.Bottom - Height);
                Direction = new Vector2(Direction.X, -Math.Abs(Direction.Y));
                bounced = true;
            }

            // If a bounce cancelled both axes (corner hit), keep moving diagonally.
            if (bounced && Direction == Vector2.Zero)
                Direction = RandomDirection();
        }

        /// <summary>
        /// Reverses this ghost's direction (used when two ghosts collide).
        /// </summary>
        public void ReverseDirection()
        {
            Direction = -Direction;
        }

        /// <summary>
        /// Relocates the ghost to a new position, keeping it inside the bounds.
        /// Called by GhostManager when a pet grabs this ghost.
        /// </summary>
        public void Respawn(Vector2 newPosition)
        {
            Position = new Vector2(
                MathHelper.Clamp(newPosition.X, _bounds.Left, _bounds.Right - Width),
                MathHelper.Clamp(newPosition.Y, _bounds.Top, _bounds.Bottom - Height));
            Direction = RandomDirection();
        }

        /// <summary>The ghost's bounding box for collision detection.</summary>
        public Rectangle GetBoundingBox() =>
            new Rectangle((int)Position.X, (int)Position.Y, Width, Height);

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, Position, null, Tint, 0f, Vector2.Zero,
                             Width / (float)_texture.Width, SpriteEffects.None, 0f);
        }

        private static Vector2 RandomDirection()
        {
            // Random unit vector on the circle, but avoid near-axis-aligned
            // directions so ghosts always travel diagonally-ish and separate well.
            double angle;
            do
            {
                angle = Random.Shared.NextDouble() * MathHelper.TwoPi;
            }
            while (Math.Abs(Math.Cos(angle)) > 0.95 || Math.Abs(Math.Sin(angle)) > 0.95);

            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }
}
