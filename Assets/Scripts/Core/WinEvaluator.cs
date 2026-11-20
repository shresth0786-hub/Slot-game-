using UnityEngine;

namespace SlotGame.Core
{
    /// <summary>
    /// Pure logic class: evaluates if a set of symbols is a winning combination.
    /// No Unity dependencies except Paytable - fully testable.
    /// 
    /// Rules (assignment spec):
    /// - Player wins when ALL slots have the SAME symbol (3-of-a-kind on 3 reels).
    /// - Wild substitutes for any regular symbol.
    /// - Scatter: 3 Scatters = free spins bonus (does not count as regular win, but pays scatter bonus).
    /// - Payout = betAmount * multiplier from Paytable.
    /// </summary>
    public class WinEvaluator
    {
        private readonly Paytable paytable;

        public WinEvaluator(Paytable paytable)
        {
            this.paytable = paytable;
        }

        /// <summary>
        /// Evaluate the current reel symbols for win conditions.
        /// </summary>
        /// <param name="symbols">Symbols currently displayed (one per reel)</param>
        /// <param name="betAmount">Current bet</param>
        public WinResult Evaluate(SymbolType[] symbols, int betAmount)
        {
            if (symbols == null || symbols.Length == 0)
                return WinResult.Loss;

            // Bonus check: Scatter triggers free spins (3 scatters)
            int scatterCount = Count(symbols, SymbolType.Scatter);
            if (scatterCount == symbols.Length)
            {
                // 3 Scatters = bonus, not regular win; give fixed free spin payout
                return new WinResult
                {
                    isWin = true,
                    isFreeSpinTrigger = true,
                    winningSymbol = SymbolType.Scatter,
                    multiplier = 10,
                    payout = betAmount * 10,
                    message = "SCATTER BONUS! +3 FREE SPINS!"
                };
            }

            // Wild logic: Wild substitutes any regular symbol.
            // Determine the "target" symbol: first non-wild, or Cherry if all wilds.
            SymbolType target = GetWildResolvedTarget(symbols);

            // Check if all symbols match target or are Wild (wild cards)
            bool isWin = true;
            foreach (var s in symbols)
            {
                if (s != target && !s.IsWild())
                {
                    isWin = false;
                    break;
                }
            }

            // Edge: if target is Scatter, we already handled; but wild+scatter mix is not a win
            if (target.IsScatter())
                isWin = false;

            if (!isWin)
                return WinResult.Loss;

            // It's a win - compute payout
            int multiplier = paytable.GetPayout(target);
            // Wild win edge: if all wilds, give highest payout (Seven jackpot)
            if (IsAllWild(symbols))
            {
                target = SymbolType.Seven;
                multiplier = paytable.jackpotMultiplier;
            }

            bool isJackpot = target == SymbolType.Seven && IsAllSame(symbols, SymbolType.Seven) || IsAllWild(symbols);

            return new WinResult
            {
                isWin = true,
                isJackpot = isJackpot,
                winningSymbol = target,
                multiplier = multiplier,
                payout = betAmount * multiplier,
                message = isJackpot ? "JACKPOT!!!" : $"WIN! {target} x{multiplier}"
            };
        }

        private SymbolType GetWildResolvedTarget(SymbolType[] symbols)
        {
            foreach (var s in symbols)
            {
                if (!s.IsWild()) return s;
            }
            return SymbolType.Wild; // all wilds
        }

        private bool IsAllWild(SymbolType[] symbols)
        {
            foreach (var s in symbols) if (!s.IsWild()) return false;
            return true;
        }

        private bool IsAllSame(SymbolType[] symbols, SymbolType type)
        {
            foreach (var s in symbols) if (s != type) return false;
            return true;
        }

        private int Count(SymbolType[] symbols, SymbolType type)
        {
            int c = 0;
            foreach (var s in symbols) if (s == type) c++;
            return c;
        }
    }
}
