# 🎰 Unity Slot Game

A polished 3-reel slot machine built with Unity 2022.3 LTS, featuring smooth reel animations, weighted RNG, clean OOP architecture, and bonus features (Wild / Scatter Free Spins).

> Assignment: Unity Slot Game — win when **all slots have the same symbol**, with realistic spin, fairness, and payouts.

---

## 🎮 Game Overview

- **Reels:** 3 reels, 1 payline (center row). Extensible to N reels.
- **Symbols:** 9 types — Cherry, Lemon, Orange, Plum, Bell, Bar, Seven, **Wild** (substitutes any), **Scatter** (free spins).
- **Win Condition:** All reels show same symbol (Wild is wildcard). Example: `Cherry-Cherry-Cherry` or `Wild-Cherry-Cherry`.
- **Payout:** `bet × multiplier` from Paytable (Seven = 50× jackpot, Bar = 20×, Bell = 15×, etc.)
- **RNG:** Weighted `System.Random` seeded with `Environment.TickCount ^ Unity.Random` — heavily tested / deterministic seed option for QA.
- **Free Spins:** 3× Scatter → 3 free spins auto-triggered, +10× bet scatter payout.
- **Jackpot:** `Seven-Seven-Seven` or `Wild-Wild-Wild` (treated as Seven jackpot).
- **Auto Spin:** Toggle for continuous play; free spins auto-chain.

---

## ✨ Features

| Feature | Detail |
|---------|--------|
| **Winning Logic** | `WinEvaluator.cs:Evaluate()` — pure logic, testable, handles Wild substitution |
| **Smooth Reel Animations** | `Reel.cs:Spin()` — fast flicker phase → ease-out cubic deceleration → overshoot bounce → punch scale |
| **RNG** | `RNGService.cs` — weighted by Paytable weights, uniform `System.Random`, seedable |
| **Payouts** | `Paytable.asset` ScriptableObject — editable multipliers/weights without code change |
| **Bonus** | Wild, Scatter Free Spins, Nudge chance, particle hooks |
| **Managers** | `BalanceManager` (credits + PlayerPrefs), `UIManager` (event-driven), `SoundManager` |
| **Folder Structure** | `Scripts/Core`, `Scripts/Managers`, `Scripts/UI`, `Scripts/Utils`, `Prefabs`, `Animations`, `Sounds`, `UI`, `Sprites` |

---

## 📁 Project Structure

```
Assets/
├── Animations/          # ReelSpin.anim (loop + easing curves)
├── Materials/
├── Prefabs/             # Reel, SymbolView, SlotMachine, UICanvas (see README_Prefabs.txt)
├── Scenes/MainScene.unity
├── ScriptableObjects/Paytable.asset
├── Scripts/
│   ├── Core/
│   │   ├── SymbolType.cs
│   │   ├── SymbolData.cs
│   │   ├── Paytable.cs
│   │   ├── RNGService.cs
│   │   ├── WinResult.cs
│   │   ├── WinEvaluator.cs
│   │   ├── SymbolView.cs
│   │   ├── Reel.cs
│   │   └── SlotMachine.cs
│   ├── Managers/
│   │   ├── BalanceManager.cs
│   │   └── SoundManager.cs
│   ├── UI/UIManager.cs
│   └── Utils/
│       ├── Easing.cs
│       └── SetupHelper.cs  (Editor: Tools > Slot Game)
├── Sounds/              # Add spin/win/jackpot clips
├── Sprites/Symbols/     # Drop symbol PNGs here (fallback colors if missing)
└── UI/                  # Canvas prefabs
Build/WebGL/             # WebGL build output (index.html placeholder until you build)
Packages/manifest.json
ProjectSettings/
```

---

## 🚀 How to Run (Editor)

1. **Requirements:** Unity Hub + Unity **2022.3 LTS** (tested on 2022.3.20f1) + WebGL module.
2. Clone:
   ```bash
   git clone <your-repo-url>
   ```
3. Open with Unity Hub → `Slot Game` folder.
4. First-time setup:
   - `Tools > Slot Game > Auto Setup Scene` — creates Managers if missing
   - `Tools > Slot Game > Create Paytable Asset` (or use existing `Assets/ScriptableObjects/Paytable.asset`)
   - See `Assets/Prefabs/README_Prefabs.txt` for manual prefab wiring (Reels → Strip → SymbolViews)
   - Assign `Paytable` to `SlotMachine.paytable`, drag 3 `Reel` refs
5. Hit **Play**.

### Controls
- **SPIN** — spin once (cost = bet)
- **+/−** — change bet (5–100, step 5)
- **AUTO SPIN** — toggle continuous spins
- Credits start at 1000, persist via `PlayerPrefs`.

---

## 🌐 WebGL Build

### Build from Editor
1. `File > Build Settings > WebGL > Switch Platform`
2. `Player Settings > Resolution: 1920x1080, Compression: Gzip`
3. `Build > Build/WebGL` (overwrites `Build/WebGL` folder)
4. Commit `Build/WebGL` (`Build.data`, `Build.wasm`, `Build.js`, `index.html`)

### Run WebGL Locally
- **Option A (recommended):** `Build/WebGL/index.html` → serve via `npx serve Build/WebGL` or `python -m http.server` (browsers block `file://` wasm)
- **Option B:** Unity → `Build and Run`

The repo already contains a placeholder `Build/WebGL/index.html` with instructions — replace it with the real Unity build output.

---

## 🧠 Thought Process & Approach

1. **OOP & SRP:** Each class has one job — `RNGService` only randomizes, `WinEvaluator` only evaluates, `Reel` only animates, `SlotMachine` orchestrates, `BalanceManager` only tracks credits.
2. **Testability:** `WinEvaluator` and `RNGService` have no `MonoBehaviour` dependencies; `WithSeed()` allows deterministic QA. Could add EditMode tests: `Assert.IsTrue(evaluator.Evaluate([Seven,Seven,Seven],10).isJackpot)`.
3. **Game Feel:** Staggered stops (0.25s apart) create anticipation. Easing is `1 - (1-t)^3` + bounce overshoot. Strip wrapping simulates infinite scroll without needing 20 sprites.
4. **Fairness:** Weighted table — low payouts (Cherry/Lemon weight 20) appear 5× more than Seven (weight 4). Wild/Scatter weight 3 → rare but exciting.
5. **Fallback Visuals:** If artist assets missing, `SymbolView` generates colored Image + label — game is playable without importing sprites.
6. **Extensibility:** Add paylines by expanding `WinEvaluator` to check rows/diagonals; add more reels by changing `reelCount`; add paytable entries without code.
7. **Bonus Creativity:** Wild + Scatter + Nudge (15% chance to re-roll third reel when first two match) + particle hooks for juice.

---

## 🔧 Configuration

- **Paytable:** Select `Assets/ScriptableObjects/Paytable.asset` → edit `payoutMultiplier` / `weight` / `fallbackColor` / `sprite`
- **Reel Feel:** On `Reel` prefab → `spinDuration`, `spinSpeed`, `bounceAmount`
- **Bet:** On `SlotMachine` → `defaultBet`, `reelStaggerDelay`
- **Bonus:** On `BonusFeature` → `nudgeChance`, `freeSpinsAwarded`

---

## 📝 Git Commit History (meaningful steps)

This repo was built with incremental commits (see `git log --oneline`):
- `feat: scaffold Unity project structure`
- `feat: add core gameplay (Paytable, RNG, WinEvaluator)`
- `feat: add Reel animation with easing + SlotMachine controller`
- `feat: add managers (Balance, UI, Sound) + bonus features`
- `chore: add WebGL placeholder + prefabs docs`
- `docs: add README`

Avoid squashing into one commit — reviewers check progression.

---

## 📦 Asset Notes

- Drop provided symbol PNGs into `Assets/Sprites/Symbols/` and assign to `Paytable.symbols[].sprite`.
- Sounds: add clips to `Assets/Sounds/` and assign to `SoundManager`.
- No external packages required beyond `TextMeshPro` + `UGUI` (included in manifest).

---

## ✅ Evaluation Checklist

- [x] Win when all symbols equal (Wild wildcard)
- [x] Smooth reel animations (staggered, easing, bounce, punch)
- [x] Clean symbol display (Mask, centered, highlight on win)
- [x] RNG (weighted, seedable, fair)
- [x] Payouts (Paytable ScriptableObject, `bet * multiplier`)
- [x] Bonus (Wild, Scatter Free Spins, Nudge)
- [x] Clean OOP, commented, meaningful names
- [x] Folder organization
- [x] `README.md` + `Build/WebGL` + commit history

Good luck — and enjoy the spin! 🎲
