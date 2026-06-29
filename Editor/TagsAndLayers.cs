using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy
{
    internal struct ThemeColor
    {
        public Color dark;
        public Color light;

        public ThemeColor(Color dark, Color light)
        {
            this.dark = dark;
            this.light = light;
        }
    }

    internal static class Pill
    {
        public const int FontSize = 10;
        public const float Height = 14f;
        public const float PaddingX = 6f;
        public const float CornerRadius = 7f;

        private static readonly GUIContent s_Scratch = new GUIContent();
        private static GUIStyle s_Style;

        public static float Width(string text)
        {
            EnsureStyle();
            s_Scratch.text = text;
            return s_Style.CalcSize(s_Scratch).x + PaddingX * 2f;
        }

        public static void Draw(Rect pillRect, string text, Color fill)
        {
            EnsureStyle();

            Color textColor = TextContrast(fill);
            s_Style.normal.textColor = textColor;
            s_Style.hover.textColor = textColor;

            s_Scratch.text = text;

            GUI.DrawTexture(pillRect, Texture2D.whiteTexture, ScaleMode.StretchToFill, true, 0f, fill, 0f, CornerRadius);
            GUI.Label(pillRect, s_Scratch, s_Style);
        }

        private static void EnsureStyle()
        {
            if (s_Style != null) return;
            s_Style = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = FontSize,
                padding = new RectOffset(0, 0, 0, 0),
            };
        }

        private static Color TextContrast(Color background)
        {
            float luminance = 0.299f * background.r + 0.587f * background.g + 0.114f * background.b;
            return luminance > 0.6f ? Color.black : Color.white;
        }
    }

    // ====================  LAYERS  ====================
    internal static class Layers
    {
        private static Color Rgb(byte r, byte g, byte b) => new Color32(r, g, b, 255);

        private static readonly Dictionary<string, ThemeColor> s_LayerColors = new Dictionary<string, ThemeColor>
        {
            {
                "TransparentFX",
                new ThemeColor(Rgb(120, 210, 235), Rgb(22, 110, 130))
            },
            {
                "Ignore Raycast",
                new ThemeColor(Rgb(165, 165, 165), Rgb(90, 90, 90))
            },
            {
                "reserved3",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "Item",
                new ThemeColor(Rgb(104, 204, 225), Rgb(71, 138, 154))
            },
            {
                "Water",
                new ThemeColor(Rgb(27, 93, 228), Rgb(27, 93, 228))
            },
            {
                "UI",
                new ThemeColor(Rgb(41, 193, 236), Rgb(30, 146, 176))
            },
            {
                "reserved6",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "reserved7",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "Interactive",
                new ThemeColor(Rgb(41, 193, 236), Rgb(30, 146, 176))
            },
            {
                "Player",
                new ThemeColor(Rgb(0, 150, 231), Rgb(0, 150, 231))
            },
            {
                "PlayerLocal",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "Environment",
                new ThemeColor(Rgb(88, 185, 71), Rgb(63, 135, 52))
            },
            {
                "UiMenu",
                new ThemeColor(Rgb(41, 193, 236), Rgb(30, 146, 176))
            },
            {
                "Pickup",
                new ThemeColor(Rgb(255, 118, 58), Rgb(167, 80, 41))
            },
            {
                "PickupNoEnvironment",
                new ThemeColor(Rgb(255, 118, 58), Rgb(167, 80, 41))
            },
            {
                "StereoLeft",
                new ThemeColor(Rgb(170, 92, 165), Rgb(170, 92, 165))
            },
            {
                "StereoRight",
                new ThemeColor(Rgb(170, 92, 165), Rgb(170, 92, 165))
            },
            {
                "Walkthrough",
                new ThemeColor(Rgb(88, 185, 71), Rgb(63, 135, 52))
            },
            {
                "MirrorReflection",
                new ThemeColor(Rgb(0, 155, 228), Rgb(0, 155, 228))
            },
            {
                "InternalUI",
                new ThemeColor(Rgb(231, 28, 41), Rgb(231, 28, 41))
            },
            {
                "HardwareObjects",
                new ThemeColor(Rgb(170, 92, 165), Rgb(170, 92, 165))
            },
            {
                "reserved2",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "reserved4",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "reserved5",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "reserved8",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "reserved1",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
        };
        private static readonly ThemeColor s_FallbackColor = new ThemeColor(Rgb(180, 180, 180), Rgb(110, 110, 110));

        public static float Draw(GameObject go, Rect selectionRect, float currentX, float areaWidth, bool isDarkTheme)
        {
            if (!Settings.ShowLayerNames || EditorApplication.isPlaying) return currentX;

            int layer = go.layer;
            string layerName = LayerMask.LayerToName(layer);

            // Only label non-Default layers that actually carry a name.
            if (layer == 0 || string.IsNullOrEmpty(layerName)) return currentX;

            float pillWidth = Pill.Width(layerName);
            float rightEdge = currentX + areaWidth;
            float pillY = selectionRect.y + (selectionRect.height - Pill.Height) * 0.5f;
            Rect pillRect = new Rect(rightEdge - pillWidth, pillY, pillWidth, Pill.Height);

            Pill.Draw(pillRect, layerName, GetLayerColor(layerName, isDarkTheme));

            return pillRect.x - areaWidth - 2;
        }

        public static Color GetLayerColor(string layerName, bool isDarkTheme)
        {
            if (!s_LayerColors.TryGetValue(layerName, out var entry)) entry = s_FallbackColor;

            return isDarkTheme ? entry.dark : entry.light;
        }
    }

    // ====================  TAGS  ====================
    internal static class Tags
    {
        private static Color Rgb(byte r, byte g, byte b) => new Color32(r, g, b, 255);

        private static readonly Dictionary<string, ThemeColor> s_TagColors = new Dictionary<string, ThemeColor>
        {
            {
                "EditorOnly",
                new ThemeColor(Rgb(228, 31, 38), Rgb(228, 31, 38))
            },
            {
                "Respawn",
                new ThemeColor(Rgb(80, 80, 80), Rgb(175, 175, 175))
            },
            {
                "Finish",
                new ThemeColor(Rgb(80, 80, 80), Rgb(175, 175, 175))
            },
            {
                "MainCamera",
                new ThemeColor(Rgb(0, 101, 216), Rgb(0, 101, 216))
            },
            {
                "Player",
                new ThemeColor(Rgb(80, 80, 80), Rgb(175, 175, 175))
            },
            {
                "GameController",
                new ThemeColor(Rgb(80, 80, 80), Rgb(175, 175, 175))
            },
        };

        private static readonly ThemeColor s_FallbackColor = new ThemeColor(Rgb(80, 80, 80), Rgb(175, 175, 175));

        private const float ObjectIconWidth = 16f;
        private const float Gap = 6f;

        private static readonly GUIContent s_NameContent = new GUIContent();

        public static void Draw(GameObject go, Rect selectionRect)
        {
            if (!Settings.ShowTagNames) return;

            string tag = go.tag;

            // Only label objects that actually carry a tag. Skips "Untagged" objects.
            if (string.IsNullOrEmpty(tag) || tag == "Untagged") return;

            bool isDarkTheme = EditorGUIUtility.isProSkin;

            s_NameContent.text = go.name;
            float nameWidth = EditorStyles.label.CalcSize(s_NameContent).x;

            float pillWidth = Pill.Width(tag);
            float pillX = selectionRect.x + ObjectIconWidth + nameWidth + Gap;
            float pillY = selectionRect.y + (selectionRect.height - Pill.Height) * 0.5f;
            Rect pillRect = new Rect(pillX, pillY, pillWidth, Pill.Height);

            Pill.Draw(pillRect, tag, GetTagColor(tag, isDarkTheme));
        }

        public static Color GetTagColor(string tag, bool isDarkTheme)
        {
            if (!s_TagColors.TryGetValue(tag, out var entry)) entry = s_FallbackColor;

            return isDarkTheme ? entry.dark : entry.light;
        }

        public static float GetFadeBoundary(GameObject go, Rect selectionRect, float nameWidth, float fallback)
        {
            if (!Settings.ShowTagNames) return fallback;

            string tag = go.tag;
            if (string.IsNullOrEmpty(tag) || tag == "Untagged") return fallback;

            float pillRight = selectionRect.x + ObjectIconWidth + nameWidth + Gap + Pill.Width(tag);
            return Mathf.Max(fallback, pillRight + 8f);
        }
    }
}