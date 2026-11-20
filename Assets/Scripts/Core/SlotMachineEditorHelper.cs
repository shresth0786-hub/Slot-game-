using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace SlotGame.Core
{
    /// <summary>
    /// Editor helper to auto-setup reels in inspector.
    /// </summary>
    [CustomEditor(typeof(SlotMachine))]
    public class SlotMachineEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var sm = (SlotMachine)target;
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
