using UnityEngine;
using UnityEditor;

namespace BluWizard.Hierarchy
{
    [InitializeOnLoad]
    public static class BluHierarchy
    {
        static BluHierarchy()
        {
            // Subscribe to the Editor's hierarchy window item callback
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            bool drawIconsNow = ComponentIcons.ShouldDrawIconsNow;

            // Convert the instance ID to a GameObject
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
            {
                // Scene Headers are not GameObjects. If the Hierarchy item is a Scene header, draw Scene buttons.
                SceneHeader.Draw(instanceID, selectionRect);
                return;
            }

            // ---------- RELATIONSHIP LINES ----------
            RelationshipLines.Draw(go, selectionRect);

            // ---------- GAME OBJECT TOGGLE ----------
            float toggleOffset = GameObjectToggle.Draw(go, selectionRect);

            // ---------- ICONS (Layer + Components) ----------
            ComponentIcons.Draw(go, selectionRect, toggleOffset, drawIconsNow);

            // ---------- TAG NAME ----------
            Tags.Draw(go, selectionRect);
        }
    }
}