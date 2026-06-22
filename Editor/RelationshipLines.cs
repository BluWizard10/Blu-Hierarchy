using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy
{
    internal static class RelationshipLines
    {
        private const float IndentWidth = 14f; // Space between each nested level's vertical line.
        private const float FirstSpineOffset = 22f; // Horizontal position of vertical lines.
        private const float VerticalConnect = 0.46f; // Vertical height of the horizontal connectors.
        private const float IconGap = 5f; // Leave a gap before the cube icon.
        private const float FoldoutGap = 16f; // Leave a gap before the arrow (objects with children).
        private const float LineThickness = 1f; // Width of the line. Should be thin. (increasing this makes it bolder)

        public static void Draw(GameObject go, Rect selectionRect)
        {
            // Lines are purely visual: Do the work on Repaint, only when enabled.
            if (Event.current.type != EventType.Repaint) return;
            if (!Settings.ShowRelationshipLines) return;

            Transform parent = go.transform.parent;
            if (parent == null) return; // Scene-root objects should have no parent to connect to.

            // Color of the Lines in RGBA. Should be set to match Editor Theme. 0.4f for Dark Theme, 0.5f for Light Theme. Keep it Opaque.
            Color lineColor = EditorGUIUtility.isProSkin ? new Color(0.4f, 0.4f, 0.4f, 1f) : new Color(0.5f, 0.5f, 0.5f, 1f);

            float top = selectionRect.y;
            float bottom = selectionRect.yMax;
            float midY = Mathf.Round(top + selectionRect.height * VerticalConnect);

            // When the object has it's own additional foldout, leave some room for it.
            float connectorEndX = selectionRect.x - (go.transform.childCount > 0 ? FoldoutGap : IconGap);

            Transform child = go.transform;
            int level = 1;

            // At each level, `child` is the node on our path whose last-child status decides what to draw at `parent` spine columns.
            while (parent != null)
            {
                float colX = selectionRect.x - FirstSpineOffset - IndentWidth * (level - 1);
                bool childIsLast = child.GetSiblingIndex() == parent.childCount - 1;

                if (level == 1)
                {
                    // Connect to the immediate parent, vertical down to the elbow when more siblings follow.
                    // Half height when this is the last child.
                    VLine(colX, top, childIsLast ? midY : bottom, lineColor);
                    HLine(colX, connectorEndX, midY, lineColor);
                }
                else if (!childIsLast)
                {
                    // Pass through the spine for an ancestor that still has siblings below.
                    VLine(colX, top, bottom, lineColor);
                }

                child = parent;
                parent = parent.parent;
                level++;
            }
        }

        private static void VLine(float x, float y0, float y1, Color color)
        {
            float xi = Mathf.Round(x);
            float a = Mathf.Round(Mathf.Min(y0, y1));
            float b = Mathf.Round(Mathf.Max(y0, y1));
            EditorGUI.DrawRect(new Rect(xi, a, LineThickness, b - a), color);
        }

        private static void HLine(float x0, float x1, float y, Color color)
        {
            float yi = Mathf.Round(y);
            float a = Mathf.Round(Mathf.Min(x0, x1));
            float b = Mathf.Round(Mathf.Max(x0, x1));
            EditorGUI.DrawRect(new Rect(a, yi, b - a, LineThickness), color);
        }
    }
}
