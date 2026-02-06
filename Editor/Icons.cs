using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy
{
    internal static class Icons
    {
        private static bool s_LayerIconsLoaded;

        private static readonly Dictionary<string, Texture2D> s_LayerIcons = new Dictionary<string, Texture2D>();

        public static bool ShouldDrawIconsNow => !EditorApplication.isPlaying || Settings.ShowIconsInPlayMode;

        public static void EnsureLayerIconsLoaded()
        {
            if (s_LayerIconsLoaded) return;

            // Check if Unity is in Play Mode && Settings.ShowIconsInPlayMode is disabled
            if (EditorApplication.isPlaying && !Settings.ShowIconsInPlayMode) return;

            s_LayerIcons["TransparentFX"] = Resources.Load<Texture2D>("Icons/L_TransparentFX");
            s_LayerIcons["Ignore Raycast"] = Resources.Load<Texture2D>("Icons/L_IgnoreRaycast");
            s_LayerIcons["reserved3"] = Resources.Load<Texture2D>("Icons/L_Reserved");
            s_LayerIcons["Item"] = Resources.Load<Texture2D>("Icons/L_Item");
            s_LayerIcons["Water"] = Resources.Load<Texture2D>("Icons/L_Water");
            s_LayerIcons["UI"] = Resources.Load<Texture2D>("Icons/L_UI");
            s_LayerIcons["reserved6"] = Resources.Load<Texture2D>("Icons/L_Reserved");
            s_LayerIcons["reserved7"] = Resources.Load<Texture2D>("Icons/L_Reserved");
            s_LayerIcons["Interactive"] = Resources.Load<Texture2D>("Icons/L_Interactive");
            s_LayerIcons["Player"] = Resources.Load<Texture2D>("Icons/L_Player");
            s_LayerIcons["PlayerLocal"] = Resources.Load<Texture2D>("Icons/L_PlayerLocal");
            s_LayerIcons["Environment"] = Resources.Load<Texture2D>("Icons/L_Environment");
            s_LayerIcons["UiMenu"] = Resources.Load<Texture2D>("Icons/L_UiMenu");
            s_LayerIcons["Pickup"] = Resources.Load<Texture2D>("Icons/L_Pickup");
            s_LayerIcons["PickupNoEnvironment"] = Resources.Load<Texture2D>("Icons/L_Pickup");
            s_LayerIcons["StereoLeft"] = Resources.Load<Texture2D>("Icons/L_Stereo");
            s_LayerIcons["StereoRight"] = Resources.Load<Texture2D>("Icons/L_Stereo");
            s_LayerIcons["Walkthrough"] = Resources.Load<Texture2D>("Icons/L_Walkthrough");
            s_LayerIcons["MirrorReflection"] = Resources.Load<Texture2D>("Icons/L_MirrorReflection");
            s_LayerIcons["InternalUI"] = Resources.Load<Texture2D>("Icons/L_InternalUI");
            s_LayerIcons["HardwareObjects"] = Resources.Load<Texture2D>("Icons/L_HardwareObjects");
            s_LayerIcons["reserved2"] = Resources.Load<Texture2D>("Icons/L_Reserved");
            s_LayerIcons["reserved4"] = Resources.Load<Texture2D>("Icons/L_Reserved");
            s_LayerIcons["reserved5"] = Resources.Load<Texture2D>("Icons/L_Reserved");
            s_LayerIcons["reserved8"] = Resources.Load<Texture2D>("Icons/L_Reserved");
            s_LayerIcons["PostProcessing"] = Resources.Load<Texture2D>("Icons/L_PostProcessing");
            s_LayerIcons["reserved1"] = Resources.Load<Texture2D>("Icons/L_Reserved");

            s_LayerIconsLoaded = true;
        }

        public static bool TryGetLayerIcon(string layerName, out Texture2D icon)
        {
            EnsureLayerIconsLoaded();
            return s_LayerIcons.TryGetValue(layerName, out icon);
        }
    }
}