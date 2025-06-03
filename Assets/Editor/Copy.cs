// Assets/Editor/ComponentCopyUtility.cs
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public static class Copy
{
    static Component[] clipboard;

    [MenuItem("GameObject/Copy All Components", false, 0)]
    static void CopyAllComponents()
    {
        if (Selection.activeGameObject == null) return;
        clipboard = Selection.activeGameObject.GetComponents<Component>();
        Debug.Log($"Copied {clipboard.Length} components from {Selection.activeGameObject.name}");
    }

    [MenuItem("GameObject/Paste All Components", false, 0)]
    static void PasteAllComponents()
    {
        if (Selection.activeGameObject == null || clipboard == null) return;
        foreach (var comp in clipboard)
        {
            // 컴포넌트 복사
            ComponentUtility.CopyComponent(comp);
            ComponentUtility.PasteComponentAsNew(Selection.activeGameObject);
        }
        Debug.Log($"Pasted {clipboard.Length} components to {Selection.activeGameObject.name}");
    }
}
