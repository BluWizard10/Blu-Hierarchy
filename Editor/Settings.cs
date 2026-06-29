using UnityEditor;

namespace BluWizard.Hierarchy
{
    public static class Settings
    {
        private const string ShowTransformIconKey = "BluHierarchy_ShowTransformIcon";
        private const string ShowLayerNamesKey = "BluHierarchy_ShowLayerNames";
        private const string ShowTagNamesKey = "BluHierarchy_ShowTagNames";
        private const string ShowHiddenComponentsKey = "BluHierarchy_ShowHiddenComponents";
        private const string ShowGameObjectToggleKey = "BluHierarchy_ShowGameObjectToggle";
        private const string ShowTooltipsKey = "BluHierarchy_ShowTooltips";
        private const string EnableDragToToggleKey = "BluHierarchy_EnableDragToToggle";
        private const string ShowIconsInPlayModeKey = "BluHierarchy_ShowIconsInPlayMode";
        private const string ShowRelationshipLinesKey = "BluHierarchy_ShowRelationshipLines";

        public static bool ShowTransformIcon
        {
            get => EditorPrefs.GetBool(ShowTransformIconKey, false);
            set => EditorPrefs.SetBool(ShowTransformIconKey, value);
        }

        public static bool ShowLayerNames
        {
            get => EditorPrefs.GetBool(ShowLayerNamesKey, false);
            set => EditorPrefs.SetBool(ShowLayerNamesKey, value);
        }

        public static bool ShowTagNames
        {
            get => EditorPrefs.GetBool(ShowTagNamesKey, true);
            set => EditorPrefs.SetBool(ShowTagNamesKey, value);
        }

        public static bool ShowGameObjectToggle
        {
            get => EditorPrefs.GetBool(ShowGameObjectToggleKey, true);
            set => EditorPrefs.SetBool(ShowGameObjectToggleKey, value);
        }

        public static bool ShowHiddenComponents
        {
            get => EditorPrefs.GetBool(ShowHiddenComponentsKey, false);
            set => EditorPrefs.SetBool(ShowHiddenComponentsKey, value);
        }

        public static bool ShowTooltips
        {
            get => EditorPrefs.GetBool(ShowTooltipsKey, true);
            set => EditorPrefs.SetBool(ShowTooltipsKey, value);
        }

        public static bool EnableDragToToggle
        {
            get => EditorPrefs.GetBool(EnableDragToToggleKey, true);
            set => EditorPrefs.SetBool(EnableDragToToggleKey, value);
        }

        public static bool ShowIconsInPlayMode
        {
            get => EditorPrefs.GetBool(ShowIconsInPlayModeKey, false);
            set => EditorPrefs.SetBool(ShowIconsInPlayModeKey, value);
        }

        public static bool ShowRelationshipLines
        {
            get => EditorPrefs.GetBool(ShowRelationshipLinesKey, true);
            set => EditorPrefs.SetBool(ShowRelationshipLinesKey, value);
        }

        public static void ResetToDefaults(bool repaint = true)
        {
            string[] keys =
            {
                ShowGameObjectToggleKey,
                ShowTransformIconKey,
                ShowLayerNamesKey,
                ShowTagNamesKey,
                ShowHiddenComponentsKey,
                ShowTooltipsKey,
                EnableDragToToggleKey,
                ShowIconsInPlayModeKey,
                ShowRelationshipLinesKey,
            };

            foreach (var k in keys) EditorPrefs.DeleteKey(k);

            if (repaint) EditorUtil.RepaintHierarchyWindow();
        }
    }
}