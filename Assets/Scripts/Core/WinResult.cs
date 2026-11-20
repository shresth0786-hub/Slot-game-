using UnityEngine;

namespace SlotGame.Core
{
    /// <summary>
    /// Result of evaluating a spin.
    /// Immutable data transfer object.
    /// </summary>
    public struct WinResult
    {
        public bool isWin;
        public bool isJackpot;
        public bool isFreeSpinTrigger; // Scatter bonus
        public SymbolType winningSymbol;
        public int payout; // coins won
        public int multiplier;
        public string message;

        public static WinResult Loss => new WinResult
        {
            isWin = false,
            payout = 0,
            message = "Try again!"
        };

        public override string ToString()
        {
            if (!isWin) return "Loss";
            return $"Win: {winningSymbol} x{multiplier} = {payout} {(isJackpot ? "[JACKPOT]" : "")} {(isFreeSpinTrigger ? "[FREE SPINS]" : "")}";
        }
    }
}
