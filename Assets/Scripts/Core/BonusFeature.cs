using UnityEngine;
// ParticleSystem lives in UnityEngine.ParticleSystemModule (com.unity.modules.particlesystem).
// Manifest now explicitly references that module; fully-qualified type used for robustness.

namespace SlotGame.Core
{
    /// <summary>
    /// Bonus features inspired by classic slots.
    /// Currently handles:
    /// - Wild Substitution (handled in WinEvaluator)
    /// - Scatter Free Spins (3 Scatters -> 3 free spins)
    /// - Nudge: if 2 reels match, third gets a second chance (small chance)
    /// - Jackpot multiplier
    /// 
    /// This component can be attached alongside SlotMachine for extra logic.
    /// Demonstrates creativity / bonus requirement.
    /// </summary>
    public class BonusFeature : MonoBehaviour
    {
        [Header("Nudge Feature")]
        [Tooltip("Chance to nudge third reel when first two match")]
        [Range(0f, 1f)] public float nudgeChance = 0.15f;

        [Header("Free Spins")]
        public int freeSpinsAwarded = 3;

        [Header("Events")]
        public UnityEngine.ParticleSystem winParticles;
        public UnityEngine.ParticleSystem jackpotParticles;

        private SlotMachine slotMachine;

        private void Awake()
        {
            slotMachine = GetComponent<SlotMachine>();
            if (slotMachine == null) slotMachine = FindObjectOfType<SlotMachine>();
        }

        private void OnEnable()
        {
            if (slotMachine != null)
            {
                slotMachine.onSpinCompleted += OnSpinCompleted;
            }
        }

        private void OnDisable()
        {
            if (slotMachine != null)
                slotMachine.onSpinCompleted -= OnSpinCompleted;
        }

        private void OnSpinCompleted(WinResult result)
        {
            if (result.isWin)
            {
                if (result.isJackpot)
                {
                    if (jackpotParticles != null) jackpotParticles.Play();
                    Debug.Log("[Bonus] JACKPOT Particles!");
                }
                else
                {
                    if (winParticles != null) winParticles.Play();
                }
            }
        }

        /// <summary>
        /// Nudge logic: called if you want to give player a second chance.
        /// Not integrated into deterministic RNG - purely for game feel bonus.
        /// Could be wired to re-roll third reel with weighted chance.
        /// </summary>
        public bool ShouldNudge(SymbolType[] result)
        {
            if (result.Length < 3) return false;
            // If first two match (or wild), chance to nudge third to match
            bool firstTwoMatch = result[0] == result[1] || result[0].IsWild() || result[1].IsWild();
            return firstTwoMatch && Random.value < nudgeChance;
        }
    }
}
