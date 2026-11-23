using UnityEngine;
using SlotGame.Core;

#if UNITY_EDITOR
using UnityEditor;

namespace SlotGame.Core
{
    /// <summary>
    /// Editor helper to auto-setup reels in inspector.
    /// Uses fully-qualified type to avoid CS0246 if assembly definitions isolate Core.
    /// </summary>
    [CustomEditor(typeof(global::SlotGame.Core.SlotMachine))]
    public class SlotMachineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var sm = (global::SlotGame.Core.SlotMachine)target;
            if (GUILayout.Button("Auto-Find Reels in Children"))
            {
                var reels = sm.GetComponentsInChildren<Reel>();
                SerializedProperty prop = serializedObject.FindProperty("reels");
                serializedObject.Update();
                prop.arraySize = reels.Length;
                for (int i = 0; i < reels.Length; i++)
                    prop.GetArrayElementAtIndex(i).objectReferenceValue = reels[i];
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif
