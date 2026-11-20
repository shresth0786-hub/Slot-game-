using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SlotGame.Utils
{
    /// <summary>
    /// Editor helper to quickly scaffold the scene when opening project first time.
    /// Tools > Slot Game > Auto Setup Scene
    /// </summary>
    public static class SetupHelper
    {
        [MenuItem("Tools/Slot Game/Auto Setup Scene")]
        public static void AutoSetup()
        {
            Debug.Log("[SetupHelper] Creating Slot Game scene scaffolding...");

            // Create main objects if missing
            var slot = Object.FindObjectOfType<Core.SlotMachine>();
            if (slot == null)
            {
                var go = new GameObject("SlotMachine");
                go.AddComponent<Core.SlotMachine>();
                go.AddComponent<Core.BonusFeature>();
                Undo.RegisterCreatedObjectUndo(go, "Create SlotMachine");
            }

            var ui = Object.FindObjectOfType<UI.UIManager>();
            if (ui == null)
            {
                var go = new GameObject("UIManager");
                go.AddComponent<UI.UIManager>();
                Undo.RegisterCreatedObjectUndo(go, "Create UIManager");
            }

            var balance = Object.FindObjectOfType<Core.BalanceManager>();
            if (balance == null)
            {
                var go = new GameObject("BalanceManager");
                go.AddComponent<Core.BalanceManager>();
                Undo.RegisterCreatedObjectUndo(go, "Create BalanceManager");
            }

            var sound = Object.FindObjectOfType<Managers.SoundManager>();
            if (sound == null)
            {
                var go = new GameObject("SoundManager");
                go.AddComponent<Managers.SoundManager>();
                Undo.RegisterCreatedObjectUndo(go, "Create SoundManager");
            }

            Debug.Log("[SetupHelper] Done. Now assign Paytable and Reels in SlotMachine inspector. See Assets/Prefabs/README_Prefabs.txt");
        }

        [MenuItem("Tools/Slot Game/Create Paytable Asset")]
        public static void CreatePaytable()
        {
            var paytable = ScriptableObject.CreateInstance<Core.Paytable>();
            AssetDatabase.CreateAsset(paytable, "Assets/ScriptableObjects/Paytable.asset");
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = paytable;
            Debug.Log("[SetupHelper] Paytable asset created at Assets/ScriptableObjects/Paytable.asset");
        }
    }
}
#endif
