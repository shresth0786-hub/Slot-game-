using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SlotGame.Core;
// SlotMachine lives in SlotGame.Core; explicit using ensures resolution
// regardless of assembly definition setup.

namespace SlotGame.UI
{
    /// <summary>
    /// Handles all UI updates: credits, bet, win messages, buttons, free spins indicator.
    /// Listens to SlotMachine events - no direct polling.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SlotGame.Core.SlotMachine slotMachine;
        [SerializeField] private TextMeshProUGUI creditsText;
        [SerializeField] private TextMeshProUGUI betText;
        [SerializeField] private TextMeshProUGUI winText;
        [SerializeField] private TextMeshProUGUI freeSpinsText;
        [SerializeField] private Button spinButton;
        [SerializeField] private Button betUpButton;
        [SerializeField] private Button betDownButton;
        [SerializeField] private Button autoSpinButton;
        [SerializeField] private Image winBanner;

        [Header("UI Feel")]
        [SerializeField] private float winTextDuration = 2f;

        private int currentBet;
        private Coroutine winTextRoutine;

        private void Awake()
        {
            if (slotMachine == null) slotMachine = FindObjectOfType<SlotGame.Core.SlotMachine>();
            currentBet = slotMachine != null ? slotMachine.CurrentBet : 10;
        }

        private void OnEnable()
        {
            if (slotMachine == null) return;
            slotMachine.onSpinStarted += OnSpinStarted;
            slotMachine.onSpinCompleted += OnSpinCompleted;
            slotMachine.onBalanceChanged += OnBalanceChanged;
            slotMachine.onFreeSpinsTriggered += OnFreeSpinsChanged;

            if (spinButton != null) spinButton.onClick.AddListener(() => slotMachine.Spin());
            if (betUpButton != null) betUpButton.onClick.AddListener(() => ChangeBet(5));
            if (betDownButton != null) betDownButton.onClick.AddListener(() => ChangeBet(-5));
            if (autoSpinButton != null) autoSpinButton.onClick.AddListener(ToggleAuto);
        }

        private void OnDisable()
        {
            if (slotMachine == null) return;
            slotMachine.onSpinStarted -= OnSpinStarted;
            slotMachine.onSpinCompleted -= OnSpinCompleted;
            slotMachine.onBalanceChanged -= OnBalanceChanged;
            slotMachine.onFreeSpinsTriggered -= OnFreeSpinsChanged;
        }

        private void Start()
        {
            UpdateBetDisplay();
            winText.text = "Hit SPIN to play!";
            freeSpinsText.gameObject.SetActive(false);
            if (winBanner != null) winBanner.gameObject.SetActive(false);
        }

        private void OnSpinStarted()
        {
            spinButton.interactable = false;
            winText.text = "Spinning...";
            if (winBanner != null) winBanner.gameObject.SetActive(false);
        }

        private void OnSpinCompleted(WinResult result)
        {
            spinButton.interactable = true;

            if (winTextRoutine != null) StopCoroutine(winTextRoutine);
            winTextRoutine = StartCoroutine(ShowWinResult(result));
        }

        private System.Collections.IEnumerator ShowWinResult(WinResult result)
        {
            if (result.isWin)
            {
                winText.text = result.message + $" +{result.payout}";
                winText.color = result.isJackpot ? new Color(1f, 0.84f, 0f) : Color.green;
                if (winBanner != null)
                {
                    winBanner.gameObject.SetActive(true);
                    winBanner.color = result.isJackpot ? new Color(1f, 0.84f, 0f, 0.25f) : new Color(0.2f, 1f, 0.2f, 0.2f);
                    // Punch animation
                    var rect = winBanner.rectTransform;
                    Vector3 orig = rect.localScale;
                    float t = 0;
                    while (t < 0.3f)
                    {
                        t += Time.deltaTime;
                        float s = 1f + Mathf.Sin(t / 0.3f * Mathf.PI) * 0.08f;
                        rect.localScale = orig * s;
                        yield return null;
                    }
                    rect.localScale = orig;
                }
                yield return new WaitForSeconds(winTextDuration);
                if (!slotMachine.IsSpinning) winText.text = "Spin again!";
            }
            else
            {
                winText.text = result.message;
                winText.color = Color.white;
            }
        }

        private void OnBalanceChanged(int newBalance)
        {
            creditsText.text = $"Credits: {newBalance}";
            // Flash if low
            if (newBalance < 20) creditsText.color = Color.red;
            else creditsText.color = Color.white;
        }

        private void OnFreeSpinsChanged(int remaining)
        {
            if (remaining > 0)
            {
                freeSpinsText.gameObject.SetActive(true);
                freeSpinsText.text = $"FREE SPINS: {remaining}";
            }
            else
            {
                freeSpinsText.gameObject.SetActive(false);
            }
        }

        private void ChangeBet(int delta)
        {
            if (slotMachine.IsSpinning) return;
            currentBet = Mathf.Clamp(currentBet + delta, 5, 100);
            slotMachine.SetBet(currentBet);
            UpdateBetDisplay();
        }

        private void UpdateBetDisplay() => betText.text = $"Bet: {currentBet}";

        private bool autoOn;
        private void ToggleAuto()
        {
            autoOn = !autoOn;
            slotMachine.ToggleAutoSpin(autoOn);
            var txt = autoSpinButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = autoOn ? "STOP AUTO" : "AUTO SPIN";
            autoSpinButton.image.color = autoOn ? new Color(1f, 0.6f, 0.2f) : Color.white;
        }
    }
}
