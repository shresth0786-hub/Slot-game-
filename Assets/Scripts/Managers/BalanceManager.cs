using UnityEngine;

namespace SlotGame.Core
{
    /// <summary>
    /// Manages player credits. Single responsibility: balance tracking.
    /// Persists via PlayerPrefs (optional).
    /// </summary>
    public class BalanceManager : MonoBehaviour
    {
        [Header("Starting Balance")]
        [SerializeField] private int startingCredits = 1000;

        private int credits;
        public int Credits => credits;

        private const string PREFS_KEY = "SlotCredits";

        private void Awake()
        {
            // Load or init
            if (PlayerPrefs.HasKey(PREFS_KEY))
                credits = PlayerPrefs.GetInt(PREFS_KEY, startingCredits);
            else
                credits = startingCredits;

            DontDestroyOnLoad(gameObject);
        }

        public bool CanAfford(int amount) => credits >= amount;

        public void Deduct(int amount)
        {
            credits = Mathf.Max(0, credits - amount);
            Save();
        }

        public void Add(int amount)
        {
            credits += amount;
            Save();
        }

        public void ResetBalance()
        {
            credits = startingCredits;
            Save();
        }

        private void Save()
        {
            PlayerPrefs.SetInt(PREFS_KEY, credits);
            PlayerPrefs.Save();
        }
    }
}
