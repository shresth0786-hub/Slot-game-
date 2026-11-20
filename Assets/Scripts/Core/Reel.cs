using System.Collections;
using UnityEngine;

namespace SlotGame.Core
{
    /// <summary>
    /// Controls a single reel: smooth spinning animation, symbol display, and stopping with easing.
    /// Uses physics-inspired easing (ease-out cubic + slight bounce) for realistic feel.
    /// Each reel spins independently with staggered stop - key to slot game feel.
    /// </summary>
    public class Reel : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform strip; // container that moves vertically
        [SerializeField] private SymbolView[] symbolViews; // visible symbols (typically 3 visible, but we animate strip)

        [Header("Animation Settings")]
        [Tooltip("Duration of fast spin before slowdown")]
        [SerializeField] private float spinDuration = 1.0f;
        [Tooltip("Extra cycles for blur effect")]
        [SerializeField] private float spinSpeed = 1800f; // pixels per second
        [Tooltip("Bounce overshoot distance")]
        [SerializeField] private float bounceAmount = 20f;

        private Paytable paytable;
        private SymbolType finalSymbol;
        private bool isSpinning;

        // Layout constants - strip moves vertically; each symbol height = 140px (configurable)
        private const float SYMBOL_HEIGHT = 140f;

        public bool IsSpinning => isSpinning;

        public void Initialize(Paytable table)
        {
            paytable = table;
            foreach (var v in symbolViews) v.Initialize(paytable);
            // Set initial random symbol so reel not empty on start
            var init = (SymbolType)Random.Range(0, 7);
            SetImmediate(init);
        }

        public void SetImmediate(SymbolType type)
        {
            finalSymbol = type;
            // Show same symbol in center, random above/below for preview
            foreach (var v in symbolViews) v.SetSymbol(type);
            if (strip != null) strip.anchoredPosition = Vector2.zero;
        }

        /// <summary>
        /// Starts spinning and stops on target symbol with smooth easing.
        /// Called by SlotMachine with staggered delay per reel.
        /// </summary>
        public IEnumerator Spin(SymbolType target, float delayBeforeStop)
        {
            isSpinning = true;
            finalSymbol = target;

            // Phase 1: fast spin - rapidly randomize symbols for visual blur
            float elapsed = 0f;
            float totalFast = spinDuration + delayBeforeStop;

            // Store original strip pos
            Vector2 startPos = strip != null ? strip.anchoredPosition : Vector2.zero;

            while (elapsed < totalFast)
            {
                // Rapid symbol flicker - conveys motion
                if (elapsed % 0.06f < 0.02f) // flicker every ~60ms
                {
                    var flicker = (SymbolType)Random.Range(0, 9);
                    foreach (var v in symbolViews) v.SetSymbol(flicker);
                }

                // Move strip down to simulate spinning (loop)
                if (strip != null)
                {
                    float offset = (spinSpeed * Time.deltaTime);
                    strip.anchoredPosition += Vector2.down * offset;
                    // Wrap to keep within bounds - fake infinite scroll
                    if (strip.anchoredPosition.y < -SYMBOL_HEIGHT * 3)
                        strip.anchoredPosition += Vector2.up * SYMBOL_HEIGHT;
                }

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Phase 2: deceleration & snap to target with easing
            yield return StartCoroutine(StopWithEasing(target, startPos));

            isSpinning = false;
        }

        private IEnumerator StopWithEasing(SymbolType target, Vector2 startPos)
        {
            // Set final symbol visuals now (but keep animating position)
            foreach (var v in symbolViews) v.SetSymbol(target);

            float duration = 0.45f;
            float elapsed = 0f;

            // Target position is startPos (aligned). We'll overshoot then bounce back
            Vector2 overshoot = startPos + Vector2.down * bounceAmount;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                // Ease-out cubic
                float eased = 1f - Mathf.Pow(1f - t, 3);

                // Add bounce at end (last 25%)
                float bounce = 0f;
                if (t > 0.75f)
                {
                    float bt = (t - 0.75f) / 0.25f;
                    bounce = Mathf.Sin(bt * Mathf.PI) * bounceAmount * (1f - bt) * 0.5f;
                }

                if (strip != null)
                {
                    // Lerp from current to overshoot then back to start with bounce
                    if (t < 0.85f)
                        strip.anchoredPosition = Vector2.Lerp(strip.anchoredPosition, overshoot, eased * 0.3f);
                    else
                        strip.anchoredPosition = Vector2.Lerp(strip.anchoredPosition, startPos + Vector2.up * bounce, 0.4f);
                }
                yield return null;
            }

            if (strip != null) strip.anchoredPosition = startPos;

            // Subtle punch scale on arrival
            yield return StartCoroutine(PunchScale());
        }

        private IEnumerator PunchScale()
        {
            Transform t = symbolViews.Length > 0 ? symbolViews[0].transform : transform;
            Vector3 original = t.localScale;
            float duration = 0.2f;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float p = elapsed / duration;
                float scale = 1f + Mathf.Sin(p * Mathf.PI) * 0.12f;
                t.localScale = original * scale;
                yield return null;
            }
            t.localScale = original;
        }

        /// <summary>
        /// Highlight win state (pulsing).
        /// </summary>
        public void SetWinHighlight(bool enabled)
        {
            foreach (var v in symbolViews) v.SetHighlight(enabled);
            // Start/stop pulsing coroutine if needed - keep simple: just scale
        }
    }
}
