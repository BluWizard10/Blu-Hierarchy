using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy
{
    internal static class AlternateRows
    {
        private const float FallbackRowHeight = 16f; // Hierarchy rows are 16px tall. Use this if Unity gives us an empty rect.

        private static readonly Color DarkTint = new Color(1f, 1f, 1f, 0.04f);
        private static readonly Color LightTint = new Color(0f, 0f, 0f, 0.045f);

        public static void Draw(Rect selectionRect)
        {
            // Purely visual: Do the work on Repaint, only when enabled.
            if (Event.current.type != EventType.Repaint) return;
            if (!Settings.ShowAlternateRows) return;

            float rowHeight = selectionRect.height > 0f ? selectionRect.height : FallbackRowHeight;

            // Rows are drawn in the scroll view's content space, so the row's own position gives us a
            // row number that stays with the row while scrolling, instead of banding the window.
            int row = Mathf.FloorToInt(selectionRect.y / rowHeight);
            if ((row & 1) == 0) return; // Every other row keeps the Editor's own background.

            // selectionRect starts after the indent, so stretch across the full width of the Hierarchy window.
            Rect rowRect = new Rect(0f, selectionRect.y, EditorGUIUtility.currentViewWidth, rowHeight);

            Color tint = EditorGUIUtility.isProSkin ? DarkTint : LightTint;
            GUI.DrawTexture(rowRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f, tint, 0f, 0f);
        }
    }
}
