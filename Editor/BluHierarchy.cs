using UnityEngine;
using UnityEditor;
#if UNITY_6000_4_OR_NEWER
using HierarchyItemId = UnityEngine.EntityId;
#else
using HierarchyItemId = System.Int32;
#endif

namespace BluWizard.Hierarchy
{
    [InitializeOnLoad]
    public static class BluHierarchy
    {
        static BluHierarchy()
        {
            // Subscribe to the Editor's hierarchy window item callback
#if UNITY_6000_4_OR_NEWER
            EditorApplication.hierarchyWindowItemByEntityIdOnGUI += OnHierarchyGUI;
#else
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
#endif
        }

        private static void OnHierarchyGUI(HierarchyItemId itemID, Rect selectionRect)
        {
            bool drawIconsNow = ComponentIcons.ShouldDrawIconsNow;

            // ---------- ALTERNATING ROW COLORING ----------
            // Drawn first so every row (GameObjects and Scene headers alike) get banded.
            AlternateRows.Draw(selectionRect);

            // Convert the Hierarchy itemID to a GameObject.
            // EntityIdToObject exists from 6.3 onward.
#if UNITY_6000_3_OR_NEWER
            GameObject go = EditorUtility.EntityIdToObject(itemID) as GameObject;
#else
            GameObject go = EditorUtility.InstanceIDToObject(itemID) as GameObject;
#endif
            if (go == null)
            {
                // Scene Headers are not GameObjects. If the Hierarchy item is a Scene header, draw Scene buttons.
                SceneHeader.Draw(itemID, selectionRect);
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