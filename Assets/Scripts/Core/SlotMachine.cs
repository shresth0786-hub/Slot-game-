using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace SlotGame.Core
{
    /// <summary>
    /// Main controller: orchestrates reels, RNG, win evaluation, balance, and UI events.
    /// Central hub - delegates to WinEvaluator, RNGService, BalanceManager.
    /// 
    /// Flow:
    /// 1. Player presses Spin -> deduct bet, disable input
    /// 2. Generate RNG result (SymbolType[] for each reel)
    /// 3. Start staggered reel spins (0.2s apart for anticipation)
    /// 4. On all reels stopped -> evaluate win -> credit payout -> fire events -> re-enable
    /// 5. Handle bonus: free spins auto-trigger
    /// </summary>
    public class SlotMachine : MonoBehaviour
    {
        [Header("Config")]
        [SerializeField] private Paytable paytable;
        [SerializeField] private int reelCount = 3;
        [SerializeField] private int defaultBet = 10;
        [SerializeField] private float reelStaggerDelay = 0.25f;

        [Header("Reels")]
        [SerializeField] private Reel[] reels;

        [Header("Events")]
        public UnityEvent onSpinStarted;
        public UnityEvent<WinResult> onSpinCompleted;
        public UnityEvent<int> onBalanceChanged;
        public UnityEvent<int> onFreeSpinsTriggered;

        // Dependencies (injected or created)
        private RNGService rng;
        private WinEvaluator evaluator;
        private BalanceManager balance;

        // State
        private bool isSpinning;
        private int currentBet;
        private int freeSpinsRemaining;
        private bool isAutoSpin;

        public bool IsSpinning => isSpinning;
        public int CurrentBet => currentBet;
        public int FreeSpinsRemaining => freeSpinsRemaining;

        private void Awake()
        {
            // Dependency setup - allow inspector override, fallback to runtime defaults
            if (paytable == null)
            {
                paytable = ScriptableObject.CreateInstance<Paytable>();
            }

            rng = new RNGService(paytable);
            evaluator = new WinEvaluator(paytable);

            // Balance: try to find or create
            balance = FindObjectOfType<BalanceManager>();
            if (balance == null)
            {
                var go = new GameObject("BalanceManager");
                balance = go.AddComponent<BalanceManager>();
            }

            currentBet = defaultBet;

            // Validate reels array length
            if (reels == null || reels.Length != reelCount)
            {
                Debug.LogWarning($"[SlotMachine] Reels array length {reels?.Length} != reelCount {reelCount}. Check inspector.");
            }

            foreach (var r in reels) r.Initialize(paytable);
        }

        private void Start()
        {
            onBalanceChanged?.Invoke(balance.Credits);
        }

        /// <summary>
        /// Player-initiated spin. Validates balance and state before starting.
        /// </summary>
        public void Spin()
        {
            if (isSpinning) return;
            if (freeSpinsRemaining <= 0 && !balance.CanAfford(currentBet))
            {
                Debug.Log("[SlotMachine] Insufficient credits.");
                onSpinCompleted?.Invoke(new WinResult { isWin = false, message = "Insufficient credits!" });
                return;
            }
            StartCoroutine(SpinRoutine());
        }

        public void SetBet(int amount)
        {
            currentBet = Mathf.Max(1, amount);
        }

        public void ToggleAutoSpin(bool enabled)
        {
            isAutoSpin = enabled;
            if (enabled && !isSpinning) Spin();
        }

        private IEnumerator SpinRoutine()
        {
            isSpinning = true;
            onSpinStarted?.Invoke();

            // Deduct bet unless free spin
            bool isFreeSpin = freeSpinsRemaining > 0;
            if (isFreeSpin)
            {
                freeSpinsRemaining--;
                onFreeSpinsTriggered?.Invoke(freeSpinsRemaining);
            }
            else
            {
                balance.Deduct(currentBet);
                onBalanceChanged?.Invoke(balance.Credits);
            }

            // Generate RNG result
            SymbolType[] result = rng.NextResult(reelCount);

            // Optional: debug force win for testing (comment out in production)
            // if (Random.value < 0.05f) result = new[] { SymbolType.Seven, SymbolType.Seven, SymbolType.Seven };

            // Staggered spin
            for (int i = 0; i < reels.Length; i++)
            {
                StartCoroutine(reels[i].Spin(result[i], i * reelStaggerDelay));
            }

            // Wait until all reels stopped
            yield return new WaitUntil(() => AllReelsStopped());

            // Evaluate
            WinResult win = evaluator.Evaluate(result, currentBet);

            if (win.isWin)
            {
                balance.Add(win.payout);
                onBalanceChanged?.Invoke(balance.Credits);

                // Highlight winning reels
                foreach (var r in reels) r.SetWinHighlight(true);

                // Free spins bonus
                if (win.isFreeSpinTrigger)
                {
                    freeSpinsRemaining += 3; // award 3 free spins
                    onFreeSpinsTriggered?.Invoke(freeSpinsRemaining);
                }

                // Brief delay to show highlight before clearing
                yield return new WaitForSeconds(1.5f);
                foreach (var r in reels) r.SetWinHighlight(false);
            }

            isSpinning = false;
            onSpinCompleted?.Invoke(win);

            // Auto-spin chaining
            if (isAutoSpin && (freeSpinsRemaining > 0 || balance.CanAfford(currentBet)))
            {
                yield return new WaitForSeconds(win.isWin ? 1.0f : 0.5f);
                Spin();
            }
            else if (freeSpinsRemaining > 0)
            {
                // Auto-trigger next free spin
                yield return new WaitForSeconds(1.0f);
                Spin();
            }
        }

        private bool AllReelsStopped()
        {
            foreach (var r in reels) if (r.IsSpinning) return false;
            return true;
        }

        // For testing / editor
        public void AddCredits(int amount)
        {
            balance.Add(amount);
            onBalanceChanged?.Invoke(balance.Credits);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (defaultBet < 1) defaultBet = 1;
            if (reelCount < 3) reelCount = 3;
        }
#endif
    }
}
