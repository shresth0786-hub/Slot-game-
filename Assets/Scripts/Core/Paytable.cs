using System.Collections.Generic;
using UnityEngine;

namespace SlotGame.Core
{
    /// <summary>
    /// ScriptableObject defining payouts and symbol weights.
    /// Follows Single Responsibility: only holds data and queries.
    /// Create via Assets > Create > Slot Game > Paytable
    /// </summary>
    [CreateAssetMenu(fileName = "Paytable", menuName = "Slot Game/Paytable")]
    public class Paytable : ScriptableObject
    {
        [Header("Symbol Definitions")]
        [Tooltip("Ordered list of symbols with payouts and weights")]
        public List<SymbolData> symbols = new List<SymbolData>()
        {
            new SymbolData(SymbolType.Cherry,  "Cherry",  new Color(0.9f,0.2f,0.2f), 5,  20),
            new SymbolData(SymbolType.Lemon,   "Lemon",   new Color(0.9f,0.9f,0.2f), 5,  20),
            new SymbolData(SymbolType.Orange,  "Orange",  new Color(1f,0.5f,0f),    10, 18),
            new SymbolData(SymbolType.Plum,    "Plum",    new Color(0.6f,0.2f,0.8f),10, 18),
            new SymbolData(SymbolType.Bell,    "Bell",    new Color(0.9f,0.8f,0.2f),15, 12),
            new SymbolData(SymbolType.Bar,     "Bar",     new Color(0.3f,0.3f,0.3f),20, 8),
            new SymbolData(SymbolType.Seven,   "Seven",   new Color(0.9f,0.1f,0.1f),50, 4),
            new SymbolData(SymbolType.Wild,    "Wild",    new Color(0.2f,0.9f,0.4f),0,  3),
            new SymbolData(SymbolType.Scatter, "Scatter", new Color(0.2f,0.6f,1f),  0,  3),
        };

        [Header("Global Multipliers")]
        [Tooltip("Jackpot multiplier when all 3 are Seven")]
        public int jackpotMultiplier = 50;

        private Dictionary<SymbolType, SymbolData> lookup;

        private void OnEnable() => BuildLookup();

        private void BuildLookup()
        {
            lookup = new Dictionary<SymbolType, SymbolData>();
            foreach (var s in symbols)
            {
                if (!lookup.ContainsKey(s.type))
                    lookup.Add(s.type, s);
            }
        }

        /// <summary>
        /// Get SymbolData by type. Builds lookup lazily if needed (for runtime ScriptableObject clone).
        /// </summary>
        public SymbolData GetSymbol(SymbolType type)
        {
            if (lookup == null || lookup.Count == 0) BuildLookup();
            return lookup.TryGetValue(type, out var data) ? data : null;
        }

        /// <summary>
        /// Returns payout multiplier for a given symbol (0 for Wild/Scatter which have special rules).
        /// </summary>
        public int GetPayout(SymbolType type)
        {
            var data = GetSymbol(type);
            return data != null ? data.payoutMultiplier : 0;
        }

        /// <summary>
        /// Weight used for weighted RNG. Higher weight => more likely.
        /// </summary>
        public int GetWeight(SymbolType type) => GetSymbol(type)?.weight ?? 1;

        /// <summary>
        /// All weighted entries for RNG service.
        /// </summary>
        public IEnumerable<SymbolData> GetWeightedSymbols() => symbols;
    }
}
