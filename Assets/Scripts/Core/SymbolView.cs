using UnityEngine;
using UnityEngine.UI;

namespace SlotGame.Core
{
    /// <summary>
    /// Visual representation of a single symbol slot on a reel.
    /// Handles sprite assignment and fallback procedural visuals.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class SymbolView : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TMPro.TextMeshProUGUI label;

        private SymbolType currentType;
        private Paytable paytable;

        private void Awake()
        {
            if (icon == null) icon = GetComponent<Image>();
            // Try to find label in children if not assigned
            if (label == null) label = GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        public void Initialize(Paytable table)
        {
            paytable = table;
        }

        /// <summary>
        /// Updates visual to show given symbol.
        /// Uses sprite if available, otherwise fallback color + text.
        /// </summary>
        public void SetSymbol(SymbolType type)
        {
            currentType = type;
            var data = paytable != null ? paytable.GetSymbol(type) : null;

            if (data != null && data.sprite != null)
            {
                icon.sprite = data.sprite;
                icon.color = Color.white;
                if (label != null) label.text = "";
            }
            else
            {
                // Procedural fallback: colored background + text label
                icon.sprite = null;
                icon.color = data != null ? data.fallbackColor : Color.gray;
                if (label != null)
                {
                    label.text = data != null ? data.displayName : type.ToString();
                    label.color = Color.white;
                }
            }
        }

        public SymbolType GetSymbol() => currentType;

        // For highlight on win
        public void SetHighlight(bool enabled)
        {
            // Simple scale + glow effect via color; anim handled externally
            icon.transform.localScale = enabled ? Vector3.one * 1.15f : Vector3.one;
            // Could add outline; keep simple for now
        }
    }
}
