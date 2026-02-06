using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy
{
    internal static class EditorUtil
    {
        private static float s_NextRepaintTime;

        public static void RepaintHierarchyWindow()
        {
            if (Time.realtimeSinceStartup < s_NextRepaintTime) return;

            // Throttle: Avoid spamming repaint while dragging/toggling.
            s_NextRepaintTime = Time.realtimeSinceStartup + 0.5f;
            EditorApplication.RepaintHierarchyWindow();
        }

        // Helper for UdonSharpBehaviour Icons
        public static bool IsDefaultCSharpScriptIcon(Texture2D tex)
        {
            if (tex == null) return true;
            var cs = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            return ReferenceEquals(tex, cs);
        }
    }
}