Prefab Setup Instructions (do once in Unity Editor):

1. Reel Prefab:
   - Create Canvas (Screen Space Overlay, 1920x1080 Reference)
   - Create Empty "ReelsContainer" with HorizontalLayoutGroup (spacing 20)
   - For each Reel (x3):
     * Create Panel "Reel_0" (size 180x420, Mask component)
     * Inside: Empty "Strip" (RectTransform)
     * Inside Strip: 3 x SymbolView (Image + TextMeshProUGUI child)
       - SymbolView script attached
       - Image size 160x130
     * Attach Reel.cs to Reel_0, assign strip + symbolViews

2. SymbolView Prefab:
   - Image + TextMeshProUGUI label centered
   - SymbolView.cs

3. SlotMachine Prefab:
   - Empty GameObject "SlotMachine"
   - SlotMachine.cs + BonusFeature.cs
   - Assign Paytable, Reels array (3)
   - Reels auto-find via helper button in inspector

4. UI Canvas:
   - TopBar: CreditsText, BetText, FreeSpinsText
   - Center: ReelsContainer + WinBanner (Image)
   - BottomBar: BetDown, BetUp, Spin, AutoSpin buttons
   - UIManager.cs on Canvas, assign all refs
   - SoundManager prefab with AudioSources

5. After setup, save as prefabs in Assets/Prefabs/
   - Reel.prefab
   - SymbolView.prefab
   - SlotMachine.prefab
   - UICanvas.prefab

Place MainScene with these in hierarchy and save.


Auto-Setup Script (optional):
Add a menu item via Tools > Slot Game > Auto Setup (see Editor/SetupHelper.cs if present)
