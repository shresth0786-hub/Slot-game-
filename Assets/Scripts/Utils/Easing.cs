using UnityEngine;

namespace SlotGame.Utils
{
    /// <summary>
    /// Common easing functions for reel animations.
    /// </summary>
    public static class Easing
    {
        public static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3);
        public static float EaseInCubic(float t) => t * t * t;
        public static float EaseOutBack(float t)
        {
            const float c1 = 1.70158f;
            const float c3 = c1 + 1f;
            return 1f + c3 * Mathf.Pow(t - 1f, 3) + c1 * Mathf.Pow(t - 1f, 2);
        }
        public static float EaseOutElastic(float t)
        {
            if (t == 0) return 0;
            if (t == 1) return 1;
            return Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * 10f - 0.75f) * (2f * Mathf.PI / 3f)) + 1f;
        }
    }
}
