using System;
using System.Collections.Generic;
using UnityEngine;

namespace YagizEraslan.Mistral.Unity
{
    /// <summary>
    /// Utility class for trimming collections when they exceed a maximum size.
    /// Follows the DRY principle - consolidates duplicate trimming logic.
    /// Follows Single Responsibility Principle - only handles list trimming.
    /// </summary>
    public static class HistoryTrimmer
    {
        /// <summary>
        /// Trims a list to the specified target count if it exceeds the maximum count.
        /// Removes items from the beginning of the list (oldest items first - FIFO).
        /// </summary>
        /// <typeparam name="T">The type of items in the list.</typeparam>
        /// <param name="list">The list to trim.</param>
        /// <param name="maxCount">The maximum allowed count before trimming occurs.</param>
        /// <param name="trimToCount">The target count after trimming.</param>
        /// <returns>True if trimming occurred, false otherwise.</returns>
        public static bool TrimIfNeeded<T>(List<T> list, int maxCount, int trimToCount)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (maxCount <= 0)
                return false;

            if (trimToCount < 0)
                trimToCount = 0;

            if (trimToCount > maxCount)
                trimToCount = maxCount;

            if (list.Count <= maxCount)
                return false;

            int itemsToRemove = list.Count - trimToCount;
            if (itemsToRemove > 0)
            {
                list.RemoveRange(0, itemsToRemove);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Trims a list of GameObjects, destroying the removed objects.
        /// Removes items from the beginning of the list (oldest items first - FIFO).
        /// </summary>
        /// <param name="gameObjects">The list of GameObjects to trim.</param>
        /// <param name="maxCount">The maximum allowed count before trimming occurs.</param>
        /// <param name="trimToCount">The target count after trimming.</param>
        /// <returns>True if trimming occurred, false otherwise.</returns>
        public static bool TrimGameObjectsIfNeeded(List<GameObject> gameObjects, int maxCount, int trimToCount)
        {
            if (gameObjects == null)
                throw new ArgumentNullException(nameof(gameObjects));

            if (maxCount <= 0)
                return false;

            if (trimToCount < 0)
                trimToCount = 0;

            if (trimToCount > maxCount)
                trimToCount = maxCount;

            if (gameObjects.Count <= maxCount)
                return false;

            int itemsToRemove = gameObjects.Count - trimToCount;
            if (itemsToRemove > 0)
            {
                for (int i = 0; i < itemsToRemove; i++)
                {
                    if (gameObjects[i] != null)
                    {
                        UnityEngine.Object.Destroy(gameObjects[i]);
                    }
                }
                gameObjects.RemoveRange(0, itemsToRemove);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Validates that the trim count is less than or equal to the max count.
        /// Useful for configuration validation.
        /// </summary>
        /// <param name="maxCount">The maximum count setting.</param>
        /// <param name="trimToCount">The trim-to count setting.</param>
        /// <returns>True if the configuration is valid, false otherwise.</returns>
        public static bool ValidateConfiguration(int maxCount, int trimToCount)
        {
            if (maxCount <= 0)
                return true; // Unlimited is valid

            return trimToCount >= 0 && trimToCount <= maxCount;
        }
    }
}
