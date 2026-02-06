using System;
using UnityEditor;
using UnityEngine;

namespace BluWizard.Hierarchy
{
    public static class Menus
    {
        private const string Root = "Tools/BluWizard LABS/Enhanced Hierarchy Settings/";

        // Explicit priorities on dropdown menu
        private const int PRI_GameObjectToggle = 10;
        private const int PRI_TransformIcon = 20;
        private const int PRI_LayerIcons = 30;
        private const int PRI_HiddenComponents = 40;
        private const int PRI_Tooltips = 50;
        private const int PRI_DragToToggle = 60;
        private const int PRI_PlayModeIcons = 70;
        private const int PRI_Reset = 100;

        private static void Toggle(Func<bool> get, Action<bool> set, string path)
        {
            set(!get());
            Menu.SetChecked(path, get());
            EditorUtil.RepaintHierarchyWindow();
        }

        // GameObject Toggle
        [MenuItem(Root + "Show GameObject Toggle", false, PRI_GameObjectToggle)]
        private static void M_ShowGameObjectToggle() => Toggle(() => Settings.ShowGameObjectToggle, v => Settings.ShowGameObjectToggle = v, Root + "Show GameObject Toggle");
        [MenuItem(Root + "Show GameObject Toggle", true)]
        private static bool V_ShowGameObjectToggle()
        {
            Menu.SetChecked(Root + "Show GameObject Toggle", Settings.ShowGameObjectToggle);
            return true;
        }

        // Transform Icon Toggle
        [MenuItem(Root + "Show Transform Icon", false, PRI_TransformIcon)]
        private static void M_ShowTransformIcon() => Toggle(() => Settings.ShowTransformIcon, v => Settings.ShowTransformIcon = v, Root + "Show Transform Icon");
        [MenuItem(Root + "Show Transform Icon", true)]
        private static bool V_ShowTransformIcon()
        {
            Menu.SetChecked(Root + "Show Transform Icon", Settings.ShowTransformIcon);
            return true;
        }

        // Show Layer Icons
        [MenuItem(Root + "Show Layer Icons", false, PRI_LayerIcons)]
        private static void M_ShowLayerIcon() => Toggle(() => Settings.ShowLayerIcon, v => Settings.ShowLayerIcon = v, Root + "Show Layer Icons");
        [MenuItem(Root + "Show Layer Icons", true)]
        private static bool V_ShowLayerIcon()
        {
            Menu.SetChecked(Root + "Show Layer Icons", Settings.ShowLayerIcon);
            return true;
        }

        // Hidden Components Toggle
        [MenuItem(Root + "Show Hidden Components", false, PRI_HiddenComponents)]
        private static void M_ShowHiddenComponents() => Toggle(() => Settings.ShowHiddenComponents, v => Settings.ShowHiddenComponents = v, Root + "Show Hidden Components");
        [MenuItem(Root + "Show Hidden Components", true)]
        private static bool V_ShowHiddenComponents()
        {
            Menu.SetChecked(Root + "Show Hidden Components", Settings.ShowHiddenComponents);
            return true;
        }

        // Tooltips Toggle
        [MenuItem(Root + "Show Tooltips", false, PRI_Tooltips)]
        private static void M_ShowTooltips() => Toggle(() => Settings.ShowTooltips, v => Settings.ShowTooltips = v, Root + "Show Tooltips");
        [MenuItem(Root + "Show Tooltips", true)]
        private static bool V_ShowTooltips()
        {
            Menu.SetChecked(Root + "Show Tooltips", Settings.ShowTooltips);
            return true;
        }

        // Drag-To-Toggle Function setting
        [MenuItem(Root + "Enable Drag-To-Toggle (Hold ALT Key)", false, PRI_DragToToggle)]
        private static void M_EnableDragToToggle() => Toggle(() => Settings.EnableDragToToggle, v => Settings.EnableDragToToggle = v, Root + "Enable Drag-To-Toggle (Hold ALT Key)");
        [MenuItem(Root + "Enable Drag-To-Toggle (Hold ALT Key)", true)]
        private static bool V_EnableDragToToggle()
        {
            Menu.SetChecked(Root + "Enable Drag-To-Toggle (Hold ALT Key)", Settings.EnableDragToToggle);
            return true;
        }

        // Icons in Play Mode Toggle
        [MenuItem(Root + "Show Icons During Play Mode", false, PRI_PlayModeIcons)]
        private static void M_ShowIconsInPlayMode() => Toggle(() => Settings.ShowIconsInPlayMode, v => Settings.ShowIconsInPlayMode = v, Root + "Show Icons During Play Mode");
        [MenuItem(Root + "Show Icons During Play Mode", true)]
        private static bool V_ShowIconsInPlayMode()
        {
            Menu.SetChecked(Root + "Show Icons During Play Mode", Settings.ShowIconsInPlayMode);
            return true;
        }

        // Reset to Defaults
        [MenuItem(Root + "Reset to Defaults", false, PRI_Reset)]
        private static void M_ResetToDefaults()
        {
            if (EditorUtility.DisplayDialog("Reset Enhanced Hierarchy Settings", "This will restore all Enhanced Hierarchy Settings to their factory defaults.\n\nAre you sure?", "Reset", "Cancel"))
            {
                Settings.ResetToDefaults();
                // Refresh Checkmarks
                V_ShowGameObjectToggle();
                V_ShowTransformIcon();
                V_ShowLayerIcon();
                V_ShowHiddenComponents();
                V_ShowTooltips();
                V_EnableDragToToggle();
                V_ShowIconsInPlayMode();
                Debug.Log("[<color=#0092d9>BluWizard LABS</color>] Enhanced Hierarchy Settings have been reset to defaults!");
            }
        }
    }

    public static class CopyPathMenuItem
    {
        [MenuItem("GameObject/Copy Path")]
        private static void CopyPath()
        {
            var go = Selection.activeGameObject;

            if (go == null) return;

            var path = go.name;

            while (go.transform.parent != null)
            {
                go = go.transform.parent.gameObject;
                path = string.Format("{0}/{1}", go.name, path);
            }

            EditorGUIUtility.systemCopyBuffer = path;
        }

        [MenuItem("GameObject/Copy Path", true)]
        private static bool CopyPathValidation()
        {
            // We can only copy the path in case 1 object is selected.
            return Selection.gameObjects.Length == 1;
        }
    }
}