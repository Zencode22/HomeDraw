using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace HomeDraw.Entities
{
    /// <summary>
    /// Detects and resolves collisions between pets so they push each other apart.
    /// </summary>
    public static class CollisionManager
    {
        /// <summary>
        /// Checks every pair of pets and separates overlapping ones.
        /// </summary>
        public static void ResolvePetCollisions(IEnumerable<Pet> pets)
        {
            var petList = new List<Pet>(pets);

            for (int i = 0; i < petList.Count; i++)
            {
                for (int j = i + 1; j < petList.Count; j++)
                {
                    ResolvePairCollision(petList[i], petList[j]);
                }
            }
        }

        // Push the two pets apart when their bounding boxes overlap
        private static void ResolvePairCollision(Pet petA, Pet petB)
        {
            Rectangle rectA = petA.GetBoundingBox();
            Rectangle rectB = petB.GetBoundingBox();

            if (!rectA.Intersects(rectB))
                return;

            // Compute penetration depth on each axis
            int overlapLeft = rectA.Right - rectB.Left;
            int overlapRight = rectB.Right - rectA.Left;
            int overlapTop = rectA.Bottom - rectB.Top;
            int overlapBottom = rectB.Bottom - rectA.Top;

            int minOverlapX = System.Math.Min(overlapLeft, overlapRight);
            int minOverlapY = System.Math.Min(overlapTop, overlapBottom);

            if (minOverlapX <= minOverlapY)
            {
                // Separate horizontally, splitting the push between both pets
                float push = minOverlapX / 2f;
                Vector2 direction = overlapLeft < overlapRight
                    ? new Vector2(-push, 0)  // A moves left, B moves right
                    : new Vector2(push, 0);   // A moves right, B moves left

                petA.Push(direction);
                petB.Push(-direction);
            }
            else
            {
                // Separate vertically
                float push = minOverlapY / 2f;
                Vector2 direction = overlapTop < overlapBottom
                    ? new Vector2(0, -push)  // A moves up, B moves down
                    : new Vector2(0, push);   // A moves down, B moves up

                petA.Push(direction);
                petB.Push(-direction);
            }
        }
    }
}
