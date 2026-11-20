using UnityEngine;

namespace SlotGame.Core
{
    /// <summary>
    /// All possible symbols on the reels.
    /// Regular symbols: Cherry, Lemon, Orange, Plum, Bell, Bar, Seven
    /// Bonus symbols: Wild (substitutes), Scatter (free spins)
    /// </summary>
    public enum SymbolType
    {
        Cherry = 0,
        Lemon = 1,
        Orange = 2,
        Plum = 3,
        Bell = 4,
        Bar = 5,
        Seven = 6,
        Wild = 7,     // Bonus: substitutes any regular symbol
        Scatter = 8   // Bonus: triggers free spins
    }

    /// <summary>
    /// Extension helpers for SymbolType.
    /// </summary>
    public static class SymbolTypeExtensions
    {
        public static bool IsRegular(this SymbolType type)
        {
            return type >= SymbolType.Cherry && type <= SymbolType.Seven;
        }

        public static bool IsWild(this SymbolType type) => type == SymbolType.Wild;
        public static bool IsScatter(this SymbolType type) => type == SymbolType.Scatter;
        public static bool IsBonus(this SymbolType type) => type == SymbolType.Wild || type == SymbolType.Scatter;
    }
}
