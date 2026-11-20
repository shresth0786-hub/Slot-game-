using System;
using System.Collections.Generic;
using UnityEngine;

namespace SlotGame.Core
{
    /// <summary>
    /// Provides fair, unpredictable random outcomes using System.Random seeded by Unity's Random + Time.
    /// Encapsulates RNG so it can be mocked/tested and ensures single responsibility.
    /// Uses weighted random selection for balanced game feel.
    /// </summary>
    public class RNGService
    {
        private readonly System.Random random;
        private readonly Paytable paytable;

        public RNGService(Paytable paytable, int? seed = null)
        {
            this.paytable = paytable;
            // Seed with high entropy: use Unity Random + Environment.TickCount if no seed provided
            int finalSeed = seed ?? Environment.TickCount ^ UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            random = new System.Random(finalSeed);
        }

        /// <summary>
        /// Returns a random SymbolType using weighted distribution from Paytable.
        /// Lower payout symbols appear more often (fairness).
        /// </summary>
        public SymbolType NextSymbol()
        {
            var symbols = paytable.GetWeightedSymbols();
            int totalWeight = 0;
            foreach (var s in symbols) totalWeight += s.weight;

            int roll = random.Next(0, totalWeight);
            int cumulative = 0;
            foreach (var s in symbols)
            {
                cumulative += s.weight;
                if (roll < cumulative)
                    return s.type;
            }
            return SymbolType.Cherry; // fallback
        }

        /// <summary>
        /// Generate full result for N reels. Used by SlotMachine.
        /// Internally calls NextSymbol N times.
        /// </summary>
        public SymbolType[] NextResult(int reelCount)
        {
            var result = new SymbolType[reelCount];
            for (int i = 0; i < reelCount; i++)
                result[i] = NextSymbol();
            return result;
        }

        /// <summary>
        /// For testing: allow deterministic seed.
        /// </summary>
        public static RNGService WithSeed(Paytable paytable, int seed) => new RNGService(paytable, seed);

        /// <summary>
        /// Uniform random int [min, max)
        /// </summary>
        public int NextInt(int min, int max) => random.Next(min, max);

        /// <summary>
        /// Uniform float [0,1)
        /// </summary>
        public float NextFloat() => (float)random.NextDouble();
    }
}
