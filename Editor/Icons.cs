using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy
{
    internal static class Icons
    {
        private struct LayerIconEntry
        {
            public Texture2D dark;
            public Texture2D light;
        }

        private static bool s_LayerIconsLoaded;

        private static readonly Dictionary<string, LayerIconEntry> s_LayerIcons = new Dictionary<string, LayerIconEntry>();

        public static bool ShouldDrawIconsNow => !EditorApplication.isPlaying || Settings.ShowIconsInPlayMode;

        private static void LoadLayerEntry(string layerName, string baseIcon)
        {
            var dark = Resources.Load<Texture2D>("Icons/" + baseIcon);
            var light = Resources.Load<Texture2D>("Icons/" + baseIcon + "_L");
            s_LayerIcons[layerName] = new LayerIconEntry
            {
                dark = dark,
                light = light != null ? light : dark,
            };
        }

        public static void EnsureLayerIconsLoaded()
        {
            if (s_LayerIconsLoaded) return;

            // Check if Unity is in Play Mode && Settings.ShowIconsInPlayMode is disabled
            if (EditorApplication.isPlaying && !Settings.ShowIconsInPlayMode) return;

            LoadLayerEntry("TransparentFX", "L_TransparentFX");
            LoadLayerEntry("Ignore Raycast", "L_IgnoreRaycast");
            LoadLayerEntry("reserved3", "L_Reserved");
            LoadLayerEntry("Item", "L_Item");
            LoadLayerEntry("Water", "L_Water");
            LoadLayerEntry("UI", "L_UI");
            LoadLayerEntry("reserved6", "L_Reserved");
            LoadLayerEntry("reserved7", "L_Reserved");
            LoadLayerEntry("Interactive", "L_Interactive");
            LoadLayerEntry("Player", "L_Player");
            LoadLayerEntry("PlayerLocal", "L_PlayerLocal");
            LoadLayerEntry("Environment", "L_Environment");
            LoadLayerEntry("UiMenu", "L_UiMenu");
            LoadLayerEntry("Pickup", "L_Pickup");
            LoadLayerEntry("PickupNoEnvironment", "L_Pickup");
            LoadLayerEntry("StereoLeft", "L_StereoL");
            LoadLayerEntry("StereoRight", "L_StereoR");
            LoadLayerEntry("Walkthrough", "L_Walkthrough");
            LoadLayerEntry("MirrorReflection", "L_MirrorReflection");
            LoadLayerEntry("InternalUI", "L_InternalUI");
            LoadLayerEntry("HardwareObjects", "L_HardwareObjects");
            LoadLayerEntry("reserved2", "L_Reserved");
            LoadLayerEntry("reserved4", "L_Reserved");
            LoadLayerEntry("reserved5", "L_Reserved");
            LoadLayerEntry("reserved8", "L_Reserved");
            LoadLayerEntry("PostProcessing", "L_PostProcessing");
            LoadLayerEntry("reserved1", "L_Reserved");

            s_LayerIconsLoaded = true;
        }

        public static bool TryGetLayerIcon(string layerName, bool isDarkTheme, out Texture2D icon)
        {
            EnsureLayerIconsLoaded();
            if (s_LayerIcons.TryGetValue(layerName, out var entry))
            {
                icon = isDarkTheme ? entry.dark : entry.light;
                return icon != null;
            }
            icon = null;
            return false;
        }
    }
}
