using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HomeDraw.Entities
{
    /// <summary>
    /// Manages an arbitrary number of ghosts: spawning, adding/removing at any
    /// time, movement, drawing, ghost-vs-ghost collisions (direction reversal),
    /// and pet-vs-ghost grabbing. Each grab scores a point for the pet and
    /// refreshes the ghost to a new random location away from all pets.
    /// </summary>
    public class GhostManager
    {
        // Minimum distance between two spawn points so ghosts never overlap on spawn.
        private const float MinSpawnDistance = 80f;
        private const int InitialGhostCount = 4;
        private const int MaxSpawnAttempts = 200;

        // Keep newly refreshed ghosts this far away from every pet's bounding box.
        private const int PetSafetyMargin = 60;

        private readonly List<Ghost> _ghosts = new List<Ghost>();
        private readonly Texture2D _texture;
        private readonly Rectangle _bounds;

        public IReadOnlyList<Ghost> Ghosts => _ghosts;

        public GhostManager(Texture2D texture, Rectangle bounds)
        {
            _texture = texture;
            _bounds = bounds;

            // Create 4 ghosts in random locations that don't spawn on top of each other.
            for (int i = 0; i < InitialGhostCount; i++)
            {
                AddGhost();
            }
        }

        /// <summary>
        /// Adds a ghost at a random location guaranteed not to spawn on top of
        /// any existing ghost. Can be called at any time.
        /// </summary>
        public void AddGhost()
        {
            Vector2 spawn = FindSpawnPosition(_ghosts, avoidPets: null);
            _ghosts.Add(new Ghost(_texture, spawn, RandomTint(), _bounds));
        }

        /// <summary>
        /// Removes the given ghost from the manager. Can be called at any time.
        /// Returns true if the ghost was present and removed.
        /// </summary>
        public bool RemoveGhost(Ghost ghost) => _ghosts.Remove(ghost);

        /// <summary>
        /// Moves every ghost, resolves ghost-vs-ghost collisions by reversing
        /// directions, then checks pet-vs-ghost collisions. Each grab gives the
        /// pet one point and refreshes the grabbed ghost to a new random spot.
        /// </summary>
        public void Update(GameTime gameTime, IEnumerable<Pet> pets)
        {
            // Move all ghosts (each ghost manages its own state).
            foreach (Ghost ghost in _ghosts)
            {
                ghost.Update(gameTime);
            }

            ResolveGhostCollisions();
            HandleGrabbing(pets);
        }

        /// <summary>Draws every ghost.</summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Ghost ghost in _ghosts)
            {
                ghost.Draw(spriteBatch);
            }
        }

        // When two ghosts collide, change both of their directions.
        private void ResolveGhostCollisions()
        {
            for (int i = 0; i < _ghosts.Count; i++)
            {
                for (int j = i + 1; j < _ghosts.Count; j++)
                {
                    Ghost a = _ghosts[i];
                    Ghost b = _ghosts[j];

                    if (a.GetBoundingBox().Intersects(b.GetBoundingBox()))
                    {
                        a.ReverseDirection();
                        b.ReverseDirection();
                    }
                }
            }
        }

        // Pets "grab" ghosts by touching them: award a point and refresh the ghost.
        private void HandleGrabbing(IEnumerable<Pet> pets)
        {
            var petList = new List<Pet>(pets);

            foreach (Pet pet in petList)
            {
                if (Pet.IsGameOver)
                    continue;

                Rectangle petBox = pet.GetBoundingBox();

                foreach (Ghost ghost in _ghosts)
                {
                    if (petBox.Intersects(ghost.GetBoundingBox()))
                    {
                        pet.ScoreGhost();
                        RefreshGhost(ghost, petList);
                    }
                }
            }
        }

        // Re-spawn a grabbed ghost at a random spot clear of the other ghosts
        // and far enough from every pet so it isn't instantly re-grabbed.
        private void RefreshGhost(Ghost ghost, List<Pet> pets)
        {
            // Exclude the ghost being refreshed from the "avoid" list.
            var others = new List<Ghost>(_ghosts);
            others.Remove(ghost);

            Vector2 spawn = FindSpawnPosition(others, pets);
            ghost.Respawn(spawn);
        }

        /// <summary>
        /// Picks a random position inside the bounds that is at least
        /// MinSpawnDistance away from every ghost in <paramref name="avoidGhosts"/>
        /// and outside the inflated boxes of every pet in
        /// <paramref name="avoidPets"/> (if provided).
        /// </summary>
        private Vector2 FindSpawnPosition(List<Ghost> avoidGhosts, List<Pet> avoidPets)
        {
            for (int attempt = 0; attempt < MaxSpawnAttempts; attempt++)
            {
                Vector2 candidate = RandomPosition();

                if (IsClearOfGhosts(candidate, avoidGhosts) && IsClearOfPets(candidate, avoidPets))
                    return candidate;
            }

            // Fallback for extremely crowded boards: take a random spot anyway.
            return RandomPosition();
        }

        private static bool IsClearOfGhosts(Vector2 candidate, List<Ghost> ghosts)
        {
            foreach (Ghost ghost in ghosts)
            {
                if (Vector2.Distance(candidate, ghost.Position) < MinSpawnDistance)
                    return false;
            }
            return true;
        }

        private bool IsClearOfPets(Vector2 candidate, List<Pet> pets)
        {
            if (pets == null)
                return true;

            var probe = new Rectangle((int)candidate.X, (int)candidate.Y,
                                      Ghost.Width, Ghost.Height);

            foreach (Pet pet in pets)
            {
                // Note: Microsoft.Xna.Framework.Rectangle has no static Inflate
                // overload in .NET 8 (the instance method was only added in .NET 9),
                // so grow the box manually for cross-framework compatibility.
                Rectangle petBox = pet.GetBoundingBox();
                Rectangle inflated = new Rectangle(
                    petBox.X - PetSafetyMargin,
                    petBox.Y - PetSafetyMargin,
                    petBox.Width + PetSafetyMargin * 2,
                    petBox.Height + PetSafetyMargin * 2);
                if (inflated.Intersects(probe))
                    return false;
            }
            return true;
        }

        private Vector2 RandomPosition()
        {
            // Random spot inside the play area, keeping the full ghost on screen.
            int maxX = Math.Max(_bounds.Left + 1, _bounds.Right - Ghost.Width);
            int maxY = Math.Max(_bounds.Top + 1, _bounds.Bottom - Ghost.Height);

            return new Vector2(
                Random.Shared.Next(_bounds.Left, maxX),
                Random.Shared.Next(_bounds.Top, maxY));
        }

        private static Color RandomTint()
        {
            Color[] palette =
            {
                Color.White,
                Color.SkyBlue,
                Color.Pink,
                Color.LightGreen,
                Color.MediumPurple,
                Color.Khaki,
            };
            return palette[Random.Shared.Next(palette.Length)];
        }
    }
}
