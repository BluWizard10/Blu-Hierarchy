using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy
{
    internal static class GameObjectToggle
    {
        private static bool s_DragToggling;
        private static bool s_TargetActiveState;
        private static bool s_SuppressNextMouseUp;

        public static float Draw(GameObject go, Rect selectionRect)
        {
            if (!Settings.ShowGameObjectToggle) return 4f;

            Rect toggleRect = new Rect(selectionRect)
            {
                x = selectionRect.xMax - 18,
                width = 18
            };

            Event e = Event.current;

            if (Settings.EnableDragToToggle)
            {
                if (e.type == EventType.MouseDown && e.alt && e.button == 0 && toggleRect.Contains(e.mousePosition))
                {
                    Undo.RecordObject(go, "Toggle Active State");
                    bool newState = !go.activeSelf;
                    go.SetActive(newState);
                    EditorUtility.SetDirty(go);

                    s_DragToggling = true;
                    s_TargetActiveState = newState;
                    s_SuppressNextMouseUp = true;

                    e.Use();
                    EditorUtil.RepaintHierarchyWindow();
                }
                else if (s_DragToggling && (e.type == EventType.MouseDrag || e.type == EventType.MouseMove))
                {
                    if (toggleRect.Contains(e.mousePosition) && go.activeSelf != s_TargetActiveState)
                    {
                        Undo.RecordObject(go, "Toggle Active State (Drag)");
                        go.SetActive(s_TargetActiveState);
                        EditorUtility.SetDirty(go);
                        e.Use();
                        EditorUtil.RepaintHierarchyWindow();
                    }
                }
                else if (e.type == EventType.MouseUp && e.button == 0)
                {
                    if (s_SuppressNextMouseUp) e.Use();
                    s_DragToggling = false;
                    s_SuppressNextMouseUp = false;
                }
            }

            bool isActive = go.activeSelf;
            EditorGUI.BeginDisabledGroup(s_DragToggling);
            bool uiState = EditorGUI.Toggle(toggleRect, isActive);
            EditorGUI.EndDisabledGroup();

            if ((!Settings.EnableDragToToggle || !s_DragToggling) && uiState != isActive)
            {
                Undo.RecordObject(go, "Toggle Active State");
                go.SetActive(uiState);
                EditorUtility.SetDirty(go);
                EditorUtil.RepaintHierarchyWindow();
            }

            return 22f;
        }
    }
}