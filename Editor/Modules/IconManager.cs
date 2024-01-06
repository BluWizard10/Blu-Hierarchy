using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy.IconManager
{
    public class IconManager : MonoBehaviour
    {
        // Inside your IconManager.cs
        public static class IconSystem
        {
            // A cache for loaded icons
            private static Dictionary<string, Texture2D> loadedIcons = new Dictionary<string, Texture2D>();

            // New dictionary for custom component icons
            private static Dictionary<System.Type, Texture2D> customComponentIcons = new Dictionary<System.Type, Texture2D>();

            // Method to get an icon by name
            public static Texture2D GetIcon(string name)
            {
                if (!loadedIcons.TryGetValue(name, out Texture2D icon))
                {
                    icon = EditorGUIUtility.IconContent(name).image as Texture2D;
                    loadedIcons[name] = icon;
                }
                return icon;
            }

            // Method to set a custom icon for a component type
            public static void SetCustomIcon(System.Type componentType, Texture2D icon)
            {
                customComponentIcons[componentType] = icon;
            }

            // Method to get a custom icon for a component type
            public static Texture2D GetCustomIcon(System.Type componentType)
            {
                customComponentIcons.TryGetValue(componentType, out Texture2D icon);
                return icon;
            }
        }

    }
}
