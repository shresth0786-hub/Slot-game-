using UnityEngine;

namespace SlotGame.Core
{
    /// <summary>
    /// Data container for a single symbol. Used by Paytable and SymbolView.
    /// Create assets via CreateAssetMenu, or rely on runtime fallback if sprites missing.
    /// </summary>
    [System.Serializable]
    public class SymbolData
    {
        [Tooltip("Symbol identifier")]
        public SymbolType type;

        [Tooltip("Display name for UI")]
        public string displayName;

        [Tooltip("Sprite for this symbol (optional - fallback color generated if null)")]
        public Sprite sprite;

        [Tooltip("Color tint used if sprite is null (procedural fallback)")]
        public Color fallbackColor = Color.white;

        [Tooltip("Payout multiplier when 3 of this symbol align (bet * multiplier)")]
        public int payoutMultiplier = 5;

        [Tooltip("Weight for RNG - higher weight = more frequent")]
        [Range(1, 100)]
        public int weight = 10;

        public SymbolData(SymbolType type, string name, Color color, int payout, int weight)
        {
            this.type = type;
            this.displayName = name;
            this.fallbackColor = color;
            this.payoutMultiplier = payout;
            this.weight = weight;
        }
    }
}
