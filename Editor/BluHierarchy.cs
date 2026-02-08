using UnityEngine;
using UnityEditor;

namespace BluWizard.Hierarchy
{
    [InitializeOnLoad]
    public static class BluHierarchy
    {
        private static bool s_Initialized;

        static BluHierarchy()
        {
            // Subscribe to the Editor's hierarchy window item callback
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnEditorUpdate()
        {
            if (s_Initialized) return;

            Icons.EnsureLayerIconsLoaded();
            s_Initialized = true;

            // Remove the update callback to prevent it from running repeatedly
            EditorApplication.update -= OnEditorUpdate;
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            bool drawIconsNow = Icons.ShouldDrawIconsNow;

            // Convert the instance ID to a GameObject
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
            {
                // Scene Headers are not GameObjects. If the Hierarchy item is a Scene header, draw Scene buttons.
                SceneHeader.Draw(instanceID, selectionRect);
                return;
            }

            // ---------- GAME OBJECT TOGGLE ----------
            float toggleOffset = GameObjectToggle.Draw(go, selectionRect);

            // ---------- ICONS (Layer + Components) ----------
            ComponentIcons.Draw(go, selectionRect, toggleOffset, drawIconsNow);

            // Draw EditorOnly Text
            DrawEditorOnlyText(go, selectionRect);
        }

        private static void DrawEditorOnlyText(GameObject go, Rect selectionRect)
        {
            if (go.tag == "EditorOnly")
            {
                // Create a new GUIStyle for the hierarchy layer text to avoid affecting other EditorStyles
                GUIStyle style = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Italic,
                    normal = { textColor = Color.grey }, // Default Text Color
                    hover = { textColor = Color.grey }   // Prevent color change on hover
                };

                const string textToShow = "EditorOnly";

                // Calculate the width of the GameObject's name using the style
                GUIContent content = new GUIContent(go.name);
                float nameWidth = style.CalcSize(content).x;

                // Calculate an offset to add some space between the GameObject's name and the text
                float offset = 20;

                // Calculate the position for the layer text to appear right after the GameObject's name
                Rect tagTextRect = new Rect(selectionRect.x + nameWidth + offset, selectionRect.y, selectionRect.width - nameWidth - offset, selectionRect.height);

                // Draw the tag text
                GUI.Label(tagTextRect, textToShow, style);
            }
        }
    }
}