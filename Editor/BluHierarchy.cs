using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Reflection;
using System;

#if VRC_SDK_VRCSDK3 // Prevent borking Script if VRC SDK does not exist in the project.
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace BluWizard.Hierarchy
{
    [InitializeOnLoad]
    public static class BluHierarchy
    {
        private static bool iconsLoaded = false;
        static BluHierarchy()
        {
            // Subscribe to the Editor's hierarchy window item callback
            EditorApplication.update += OnEditorUpdate;
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        private static void OnEditorUpdate()
        {
            if (!iconsLoaded)
            {
                LoadLayerIcons();
                iconsLoaded = true;

                // Remove the update callback to prevent it from running repeatedly
                EditorApplication.update -= OnEditorUpdate;
            }
        }

        private static bool ShouldDrawIconsNow => !EditorApplication.isPlaying || BluHierarchySettings.ShowIconsInPlayMode;

        private static Dictionary<string, Texture2D> loadedIcons = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Texture2D> layerIcons = new Dictionary<string, Texture2D>();
        private static Dictionary<System.Type, Texture2D> customComponentIcons = new Dictionary<System.Type, Texture2D>();

        private static bool s_DragToggling = false;
        private static bool s_TargetActiveState = false;
        private static bool s_SuppressNextMouseUp = false;

        public static void LoadLayerIcons()
        {
            // Check if Unity is in Play Mode
            if (EditorApplication.isPlaying && !BluHierarchySettings.ShowIconsInPlayMode)
            {
                // Exit the method to optimize performance
                return;
            }
            layerIcons.Add("TransparentFX", Resources.Load<Texture2D>("Icons/L_TransparentFX"));
            layerIcons.Add("Ignore Raycast", Resources.Load<Texture2D>("Icons/L_IgnoreRaycast"));
            layerIcons.Add("reserved3", Resources.Load<Texture2D>("Icons/L_Reserved"));
            layerIcons.Add("Item", Resources.Load<Texture2D>("Icons/L_Item"));
            layerIcons.Add("Water", Resources.Load<Texture2D>("Icons/L_Water"));
            layerIcons.Add("UI", Resources.Load<Texture2D>("Icons/L_UI"));
            layerIcons.Add("reserved6", Resources.Load<Texture2D>("Icons/L_Reserved"));
            layerIcons.Add("reserved7", Resources.Load<Texture2D>("Icons/L_Reserved"));
            layerIcons.Add("Interactive", Resources.Load<Texture2D>("Icons/L_Interactive"));
            layerIcons.Add("Player", Resources.Load<Texture2D>("Icons/L_Player"));
            layerIcons.Add("PlayerLocal", Resources.Load<Texture2D>("Icons/L_PlayerLocal"));
            layerIcons.Add("Environment", Resources.Load<Texture2D>("Icons/L_Environment"));
            layerIcons.Add("UiMenu", Resources.Load<Texture2D>("Icons/L_UiMenu"));
            layerIcons.Add("Pickup", Resources.Load<Texture2D>("Icons/L_Pickup"));
            layerIcons.Add("PickupNoEnvironment", Resources.Load<Texture2D>("Icons/L_Pickup"));
            layerIcons.Add("StereoLeft", Resources.Load<Texture2D>("Icons/L_Stereo"));
            layerIcons.Add("StereoRight", Resources.Load<Texture2D>("Icons/L_Stereo"));
            layerIcons.Add("Walkthrough", Resources.Load<Texture2D>("Icons/L_Walkthrough"));
            layerIcons.Add("MirrorReflection", Resources.Load<Texture2D>("Icons/L_MirrorReflection"));
            layerIcons.Add("InternalUI", Resources.Load<Texture2D>("Icons/L_InternalUI"));
            layerIcons.Add("HardwareObjects", Resources.Load<Texture2D>("Icons/L_HardwareObjects"));
            layerIcons.Add("reserved2", Resources.Load<Texture2D>("Icons/L_Reserved"));
            layerIcons.Add("reserved4", Resources.Load<Texture2D>("Icons/L_Reserved"));
        }

        public static void SetCustomIcon(System.Type componentType, Texture2D icon)
        {
            customComponentIcons[componentType] = icon;
        }

        public static Texture2D GetCustomIcon(System.Type componentType)
        {
            customComponentIcons.TryGetValue(componentType, out Texture2D icon);
            return icon;
        }

        // HELPER TO RETRIEVE ICONS FOR COMPONENTS, CALLED UPON LATER IN THIS SCRIPT
        private static Texture2D ResolveIconForComponent(Component component, bool isDarkTheme, ref string tooltip)
        {
            var t = component.GetType();
            string typeName = t.Name;

            // UdonSharp derived classes need special handling for the script icon and tooltip.
            // So, handle UdonSharp (base-type match) first
            if (typeName == "UdonSharpBehaviour" || (t.BaseType != null && t.BaseType.Name == "UdonSharpBehaviour"))
            {
                Texture2D scriptIcon = null;
#if UNITY_EDITOR
                var mb = component as MonoBehaviour;
                if (mb != null)
                {
                    var monoScript = UnityEditor.MonoScript.FromMonoBehaviour(mb);
                    if (monoScript != null)
                    {
                        var gc = EditorGUIUtility.ObjectContent(monoScript, typeof(UnityEditor.MonoScript));
                        var candidate = gc.image as Texture2D;

                        if (!IsDefaultCSharpScriptIcon(candidate))
                        {
                            scriptIcon = candidate;
                        }

                        if (BluHierarchySettings.ShowTooltips)
                        {
                            var cls = monoScript.GetClass();
                            string className = cls != null ? cls.Name : monoScript.name;
                            tooltip = $"Udon Sharp Behaviour ({className})";
                        }
                    }
                }
#endif
                return scriptIcon != null ? scriptIcon : Resources.Load<Texture2D>("Icons/vrcUdonSharpBehaviour");
            }

            switch (typeName)
            {
                // VRC Avatars SDK
                case "VRCAvatarDescriptor": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcAvatarDescriptor" : "Icons/vrcAvatarDescriptor_L");
                case "VRCPerPlatformOverrides": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPerPlatformOverrides" : "Icons/vrcPerPlatformOverrides");
                case "VRCHeadChop": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcHeadChop" : "Icons/vrcHeadChop_L");
                case "VRCImpostorSettings": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcImpostorSettings" : "Icons/vrcImpostorSettings_L");
                case "VRCImpostorEnvironment": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcImpostorSettings" : "Icons/vrcImpostorSettings_L");

                // VRC Worlds SDK
                case "VRCSceneDescriptor": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcSceneDescriptor" : "Icons/vrcSceneDescriptor_L");
                case "UdonBehaviour": return Resources.Load<Texture2D>("Icons/vrcUdonBehaviour");
                case "VRCPickup": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPickup" : "Icons/vrcPickup_L");
                case "VRCMirrorReflection": return Resources.Load<Texture2D>("Icons/vrcMirrorReflection");
                case "VRCStation": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcStation" : "Icons/vrcStation_L");
                case "VRCObjectSync": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcObjectSync" : "Icons/vrcObjectSync_L");
                case "VRCObjectPool": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcObjectPool" : "Icons/vrcObjectPool_L");
                case "VRCPortalMarker": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPortalMarker" : "Icons/vrcPortalMarker_L");
                case "VRCAvatarPedestal": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcAvatarPedestal" : "Icons/vrcAvatarPedestal_L");
                case "VRCAVProVideoPlayer": return Resources.Load<Texture2D>("Icons/vrcAVProVideoPlayer");
                case "VRCAVProVideoScreen": return Resources.Load<Texture2D>("Icons/vrcAVProVideoScreen");
                case "VRCAVProVideoSpeaker": return Resources.Load<Texture2D>("Icons/vrcAVProVideoSpeaker");
                case "VRCUiShape": return Resources.Load<Texture2D>("Icons/vrcUiShape");
                case "VRCUnityVideoPlayer": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcUnityVideoPlayer" : "Icons/vrcUnityVideoPlayer_L");
                case "VRCUrlInputField": return Resources.Load<Texture2D>("Icons/vrcURLInputField");
                case "VRCCameraDollyAnimation": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcCameraDollyAnimation" : "Icons/vrcCameraDollyAnimation_L");
                case "VRCCameraDollyPath": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcCameraDollyPath" : "Icons/vrcCameraDollyAnimation_L");
                case "VRCCameraDollyPathPoint": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcCameraDollyPoint" : "Icons/vrcCameraDollyPoint_L");

                // VRC Base SDK
                case "PipelineManager": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPipelineManager" : "Icons/vrcPipelineManager_L");
                case "VRCPhysBone": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBone" : "Icons/vrcPhysBone_L");
                case "VRCPhysBoneRoot": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBoneRoot" : "Icons/vrcPhysBoneRoot_L");
#if VRC_SDK_VRCSDK3
                case "VRCPhysBoneCollider":
                {
                    var physBoneCollider = component as VRCPhysBoneCollider;
                        if (physBoneCollider != null)
                        {
                            switch (physBoneCollider.shapeType)
                            {
                                case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane: return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBoneColliderPlane" : "Icons/vrcPhysBoneColliderPlane_L");
                                case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere: return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBoneCollider" : "Icons/vrcPhysBoneCollider_L");
                                case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule: return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBoneCollider" : "Icons/vrcPhysBoneCollider_L");
                            }
                        }
                    break;
                }
#endif
                case "VRCContactReceiver": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcContactReceiver" : "Icons/vrcContactReceiver_L");
                case "VRCContactSender": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcContactSender" : "Icons/vrcContactSender_L");
                case "VRCParentConstraint": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcParentConstraint" : "Icons/vrcParentConstraint_L");
                case "VRCPositionConstraint": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPositionConstraint" : "Icons/vrcPositionConstraint_L");
                case "VRCRotationConstraint": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcRotationConstraint" : "Icons/vrcRotationConstraint_L");
                case "VRCScaleConstraint": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcScaleConstraint" : "Icons/vrcScaleConstraint_L");
                case "VRCAimConstraint": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcAimConstraint" : "Icons/vrcAimConstraint_L");
                case "VRCLookAtConstraint": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcLookAtConstraint" : "Icons/vrcLookAtConstraint_L");
                case "VRCSpatialAudioSource": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcSpatialAudioSource" : "Icons/vrcSpatialAudioSource_L");

                // Third-Party Utilities
                case "VRCFury": return Resources.Load<Texture2D>("Icons/VRCFury");
                case "VRCFuryComponent": return Resources.Load<Texture2D>("Icons/VRCFury");
                case "VRCFuryGlobalCollider": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/VRCFuryGlobalCollider" : "Icons/VRCFuryGlobalCollider_L");
                case "VRCFuryHapticPlug": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/VRCFurySPSPlug" : "Icons/VRCFurySPSPlug_L");
                case "VRCFuryHapticSocket": return Resources.Load<Texture2D>("Icons/VRCFurySPSSocket");
                case "VRCFuryHapticTouchReceiver": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/VRCFuryHapticReceiver" : "Icons/VRCFuryHapticReceiver_L");
                case "VRCFuryHapticTouchSender": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/VRCFuryHapticSender" : "Icons/VRCFuryHapticSender_L");
                case "VRCFuryDebugInfo": return Resources.Load<Texture2D>("Icons/VRCFuryDebugInfo");
                case "VRCFuryTest": return Resources.Load<Texture2D>("Icons/VRCFuryDebugInfo");

                case "BakeryPointLight": return Resources.Load<Texture2D>("Icons/bakeryPointLight");
                case "BakeryLightMesh": return Resources.Load<Texture2D>("Icons/bakeryLightMesh");
                case "BakeryDirectLight": return Resources.Load<Texture2D>("Icons/bakeryDirectLight");
                case "BakerySkyLight": return Resources.Load<Texture2D>("Icons/bakeryDirectLight");
                case "BakeryLightmapGroupSelector": return Resources.Load<Texture2D>("Icons/bakeryGeneric");
                case "BakeryLightmappedPrefab": return Resources.Load<Texture2D>("Icons/bakeryGeneric");
                case "BakeryPackAsSingleSquare": return Resources.Load<Texture2D>("Icons/bakeryGeneric");
                case "BakerySector": return Resources.Load<Texture2D>("Icons/bakeryGeneric");
                case "BakeryVolume": return Resources.Load<Texture2D>("Icons/bakeryVolume");
                case "ftLightmapsStorage": return Resources.Load<Texture2D>("Icons/bakeryGeneric");

                case "d4rkAvatarOptimizer": return Resources.Load<Texture2D>("Icons/d4rkAvatarOptimizer");

                case "GestureManager": return Resources.Load<Texture2D>(isDarkTheme ? "Icons/gestureManager" : "Icons/gestureManager_L");
            }

            // Custom-per-type override, then Unity fallback
            var custom = GetCustomIcon(t);
            if (custom != null) return custom;

            var iconContent = EditorGUIUtility.ObjectContent(component, t);
            return iconContent.image as Texture2D;
        }


        //--------- SETTINGS ----------
        public static class EnhancedHierarchyMenu
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
                RepaintHierarchyWindow();
            }

            // GameObject Toggle
            [MenuItem(Root + "Show GameObject Toggle", false, PRI_GameObjectToggle)]
            private static void M_ShowGameObjectToggle() => Toggle(() => BluHierarchySettings.ShowGameObjectToggle, v => BluHierarchySettings.ShowGameObjectToggle = v, Root + "Show GameObject Toggle");
            [MenuItem(Root + "Show GameObject Toggle", true)]
            private static bool V_ShowGameObjectToggle()
            {
                Menu.SetChecked(Root + "Show GameObject Toggle", BluHierarchySettings.ShowGameObjectToggle);
                return true;
            }

            // Transform Icon Toggle
            [MenuItem(Root + "Show Transform Icon", false, PRI_TransformIcon)]
            private static void M_ShowTransformIcon() => Toggle(() => BluHierarchySettings.ShowTransformIcon, v => BluHierarchySettings.ShowTransformIcon = v, Root + "Show Transform Icon");
            [MenuItem(Root + "Show Transform Icon", true)]
            private static bool V_ShowTransformIcon()
            {
                Menu.SetChecked(Root + "Show Transform Icon", BluHierarchySettings.ShowTransformIcon);
                return true;
            }

            // Show Layer Icons
            [MenuItem(Root + "Show Layer Icons", false, PRI_LayerIcons)]
            private static void M_ShowLayerIcon() => Toggle(() => BluHierarchySettings.ShowLayerIcon, v => BluHierarchySettings.ShowLayerIcon = v, Root + "Show Layer Icons");
            [MenuItem(Root + "Show Layer Icons", true)]
            private static bool V_ShowLayerIcon()
            {
                Menu.SetChecked(Root + "Show Layer Icons", BluHierarchySettings.ShowLayerIcon);
                return true;
            }

            // Hidden Components Toggle
            [MenuItem(Root + "Show Hidden Components", false, PRI_HiddenComponents)]
            private static void M_ShowHiddenComponents() => Toggle(() => BluHierarchySettings.ShowHiddenComponents, v => BluHierarchySettings.ShowHiddenComponents = v, Root + "Show HIdden Components");
            [MenuItem(Root + "Show Hidden Components", true)]
            private static bool V_ShowHiddenComponents()
            {
                Menu.SetChecked(Root + "Show Hidden Components", BluHierarchySettings.ShowHiddenComponents);
                return true;
            }

            // Tooltips Toggle
            [MenuItem(Root + "Show Tooltips", false, PRI_Tooltips)]
            private static void M_ShowTooltips() => Toggle(() => BluHierarchySettings.ShowTooltips, v => BluHierarchySettings.ShowTooltips = v, Root + "Show Tooltips");
            [MenuItem(Root + "Show Tooltips", true)]
            private static bool V_ShowTooltips()
            {
                Menu.SetChecked(Root + "Show Tooltips", BluHierarchySettings.ShowTooltips);
                return true;
            }

            // Drag-To-Toggle Function Setting
            [MenuItem(Root + "Enable Drag-To-Toggle (Hold ALT Key)", false, PRI_DragToToggle)]
            private static void M_EnableDragToToggle() => Toggle(() => BluHierarchySettings.EnableDragToToggle, v => BluHierarchySettings.EnableDragToToggle = v, Root + "Enable Drag-To-Toggle (Hold ALT Key)");
            [MenuItem(Root + "Enable Drag-To-Toggle (Hold ALT Key)", true)]
            private static bool V_EnableDragToToggle()
            {
                Menu.SetChecked(Root + "Enable Drag-To-Toggle (Hold ALT Key)", BluHierarchySettings.EnableDragToToggle);
                return true;
            }

            // Icons in Play Mode Toggle
            [MenuItem(Root + "Show Icons During Play Mode", false, PRI_PlayModeIcons)]
            private static void M_ShowIconsInPlayMode() => Toggle(() => BluHierarchySettings.ShowIconsInPlayMode, v => BluHierarchySettings.ShowIconsInPlayMode = v, Root + "Show Icons During Play Mode");
            [MenuItem(Root + "Show Icons During Play Mode", true)]
            private static bool V_ShowIconsInPlayMode()
            {
                Menu.SetChecked(Root + "Show Icons During Play Mode", BluHierarchySettings.ShowIconsInPlayMode);
                return true;
            }

            // Reset to Defaults
            [MenuItem(Root + "Reset to Defaults", false, PRI_Reset)]
            private static void M_ResetToDefaults()
            {
                if (EditorUtility.DisplayDialog("Reset Enhanced Hierarchy Settings", "This will restore all Enhanced Hierarchy Settings to their factory defaults.\n\nAre you sure?", "Reset", "Cancel"))
                {
                    BluHierarchySettings.ResetToDefaults();
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

                if (go == null)
                {
                    return;
                }

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
                // We can only copy the path in case 1 object is selected
                return Selection.gameObjects.Length == 1;
            }
        }

        public static class BluHierarchySettings
        {
            private const string ShowTransformIconKey = "BluHierarchy_ShowTransformIcon";
            private const string ShowLayerIconKey = "BluHierarchy_ShowLayerIcon";
            private const string ShowHiddenComponentsKey = "BluHierarchy_ShowHiddenComponents";
            private const string ShowGameObjectToggleKey = "BluHierarchy_ShowGameObjectToggle";
            private const string ShowTooltipsKey = "BluHierarchy_ShowTooltips";
            private const string EnableDragToToggleKey = "BluHierarchy_EnableDragToToggle";
            private const string ShowIconsInPlayModeKey = "BluHierarchy_ShowIconsInPlayMode";

            public static bool ShowTransformIcon
            {
                get => EditorPrefs.GetBool(ShowTransformIconKey, false);
                set => EditorPrefs.SetBool(ShowTransformIconKey, value);
            }

            public static bool ShowLayerIcon
            {
                get => EditorPrefs.GetBool(ShowLayerIconKey, false);
                set => EditorPrefs.SetBool(ShowLayerIconKey, value);
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

            public static void ResetToDefaults(bool repaint = true)
            {
                string[] keys =
                {
                    ShowGameObjectToggleKey,
                    ShowTransformIconKey,
                    ShowLayerIconKey,
                    ShowHiddenComponentsKey,
                    ShowTooltipsKey,
                    EnableDragToToggleKey,
                    ShowIconsInPlayModeKey,
                };

                foreach (var k in keys)
                    EditorPrefs.DeleteKey(k);

                if (repaint)
                    RepaintHierarchyWindow();
            }
        }

        public static float nextRepaintTime;
        public static void RepaintHierarchyWindow()
        {
            if (Time.realtimeSinceStartup < nextRepaintTime) return;

            // Optimize draw calls on repaint - fixed CPU performance on theme change?
            nextRepaintTime = Time.realtimeSinceStartup + 0.5f;
            // This will get all open hierarchy windows and repaint them
            EditorApplication.RepaintHierarchyWindow();
        }
        //--------- END SETTINGS ----------

        // Helper for UdonSharpBehaviour Icons
        private static bool IsDefaultCSharpScriptIcon(Texture2D tex)
        {
            if (tex == null) return true;
            var cs = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            return ReferenceEquals(tex, cs);
        }

        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            bool drawIconsNow = ShouldDrawIconsNow;
            
            // Convert the instance ID to a GameObject
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            //--------- GAME OBJECT TOGGLE ----------

            if (BluHierarchySettings.ShowGameObjectToggle)
            {
                Rect toggleRect = new Rect(selectionRect);
                toggleRect.x = selectionRect.xMax - 18;
                toggleRect.width = 18;

                Event e = Event.current;

                if (BluHierarchySettings.EnableDragToToggle)
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
                        RepaintHierarchyWindow();
                    }
                    else if (s_DragToggling && (e.type == EventType.MouseDrag || e.type == EventType.MouseMove))
                    {
                        if (toggleRect.Contains(e.mousePosition) && go.activeSelf != s_TargetActiveState)
                        {
                            Undo.RecordObject(go, "Toggle Active State (Drag)");
                            go.SetActive(s_TargetActiveState);
                            EditorUtility.SetDirty(go);
                            e.Use();
                            RepaintHierarchyWindow();
                        }
                    }
                    else if (e.type == EventType.MouseUp && e.button == 0)
                    {
                        if (s_SuppressNextMouseUp)
                        {
                            e.Use();
                        }
                        s_DragToggling = false;
                        s_SuppressNextMouseUp = false;
                    }
                }

                bool isActive = go.activeSelf;
                EditorGUI.BeginDisabledGroup(s_DragToggling);
                bool uiState = EditorGUI.Toggle(toggleRect, isActive);
                EditorGUI.EndDisabledGroup();

                if ((!BluHierarchySettings.EnableDragToToggle || !s_DragToggling) && uiState != isActive)
                {
                    Undo.RecordObject(go, "Toggle Active State");
                    go.SetActive(uiState);
                    EditorUtility.SetDirty(go);
                    RepaintHierarchyWindow();
                }
            }

            //--------- ICONS ---------

            // Start drawing component icons to the left of the toggle button
            float iconSize = 16;
            float toggleOffset = BluHierarchySettings.ShowGameObjectToggle ? 22 : 4; // Use toggleOffset to control the appearance of the rest of the icons when the Game Object Toggle appearance is changed.
            float currentX = selectionRect.xMax - iconSize - toggleOffset;


            //--------- LAYER ICON PROCESS ---------

            float separatorWidth = 1;
            float separatorHeight = selectionRect.height;
            float separatorX = selectionRect.xMax - iconSize - (toggleOffset + 2) - separatorWidth;
            float separatorY = selectionRect.y;

            if (BluHierarchySettings.ShowLayerIcon && drawIconsNow)
            {
                // Check if Unity is in Play Mode
                if (EditorApplication.isPlaying)
                {
                    // Exit the method during Play Mode to optimize performance.
                    return;
                }

                string layerName = LayerMask.LayerToName(go.layer);

                if (layerIcons.ContainsKey(layerName))
                {
                    Texture2D layerIcon = layerIcons[layerName];
                    if (layerIcon != null)
                    {
                        Rect layerIconRect = new Rect(currentX, selectionRect.y, iconSize, iconSize);
                        GUI.DrawTexture(layerIconRect, layerIcon);
                    }

                    currentX -= iconSize + 2;

                    Rect separatorRect = new Rect(separatorX, separatorY, separatorWidth, separatorHeight);
                    Texture2D separatorTexture = new Texture2D(1, 1);
                    separatorTexture.SetPixel(0, 0, Color.gray);
                    separatorTexture.Apply();

                    GUI.DrawTexture(separatorRect, separatorTexture);

                    currentX -= separatorWidth + 2;
                }
            }

            //--------- COMPONENT ICONS PROCESS ----------

            // Get all components on the GameObject
            Component[] components = go.GetComponents<Component>();

            // Calculate name width to prevent overlap
            GUIStyle labelStyle = EditorStyles.label;
            float nameWidth = labelStyle.CalcSize(new GUIContent(go.name)).x;
            float minX = selectionRect.x + nameWidth + 8f; // Prevent drawing over the name + some spacing
            float fadeThreshold = minX + 16f; // Start fading out slightly before the cutoff

            // CONTROL MISSING SCRIPTS PROCESS

            int missingCount = 0;
#if UNITY_2019_2_OR_NEWER
            missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
#else
            var mbs = go.GetComponents<MonoBehaviour>();
            for (int i = 0; i < mbs.Length; i++)
            {
                if (mbs[i] == null) missingCount++;
            }
#endif

            if (missingCount > 0)
            {
                GUIContent warn = EditorGUIUtility.IconContent("console.warnicon");
                Rect missRect = new Rect(currentX, selectionRect.y, iconSize, iconSize);

                if (currentX >= minX)
                {
                    float alpha = 1f;
                    if (currentX < fadeThreshold)
                        alpha = Mathf.InverseLerp(minX, fadeThreshold, currentX);

                    GUIContent content = new GUIContent(warn.image, $"Missing Script(s): {missingCount}");

                    Color prev = GUI.color;
                    GUI.color = new Color(prev.r, prev.g, prev.b, alpha);
                    GUI.Label(missRect, content);
                    GUI.color = prev;

                    currentX -= iconSize + 2;
                }
            }

            if (!drawIconsNow)
            {
                goto AFTER_ICON_DRAWING;
            }

            foreach (Component component in components)
            {
                // Check if Unity is in Play Mode
                if (EditorApplication.isPlaying && !BluHierarchySettings.ShowIconsInPlayMode)
                {
                    // Exit the method during Play Mode to optimize performance.
                    return;
                }

                if (component == null) continue;

                if (!BluHierarchySettings.ShowHiddenComponents && (component.hideFlags & HideFlags.HideInInspector) != 0) continue;

                if (component is Transform && !BluHierarchySettings.ShowTransformIcon) continue;

                Texture2D icon = null;

                // Check for Pro Theme
                bool isDarkTheme = EditorGUIUtility.isProSkin;

                string tooltip = BluHierarchySettings.ShowTooltips ? ObjectNames.NicifyVariableName(component.GetType().Name) : null;

                // FETCH HELPER AND RESOLVE COMPONENT ICONS
                icon = ResolveIconForComponent(component, isDarkTheme, ref tooltip);

                if (icon == null)
                {
                    GUIContent iconContent = EditorGUIUtility.ObjectContent(component, component.GetType());
                    icon = iconContent.image as Texture2D;
                }

                Rect iconRect = new Rect(currentX, selectionRect.y, iconSize, iconSize);

                // --- FADE OUT / SKIP ICON IF OVERLAPPING NAME ---
                if (currentX < minX)
                    break;

                float alpha = 1f;
                if (currentX < fadeThreshold)
                    alpha = Mathf.InverseLerp(minX, fadeThreshold, currentX);

                bool isEnabled = false;
                bool canToggle = false;

                // Check for components that can be toggled and set the isEnabled flag
                if (component is Behaviour behaviourComponent)
                {
                    isEnabled = behaviourComponent.enabled;
                    canToggle = true;
                }
                else if (component is Renderer rendererComponent)
                {
                    isEnabled = rendererComponent.enabled;
                    canToggle = true;
                }
                else
                {
                    var enabledProp = component.GetType().GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
                    if (enabledProp != null && enabledProp.PropertyType == typeof(bool))
                    {
                        isEnabled = (bool)enabledProp.GetValue(component, null);
                        canToggle = true;
                    }
                }

                // Component Toggle functionality
                if (!canToggle && component is Component)
                {
                    PropertyInfo enabledProperty = component.GetType().GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
                    if (enabledProperty != null && enabledProperty.PropertyType == typeof(bool))
                    {
                        isEnabled = (bool)enabledProperty.GetValue(component, null);
                        canToggle = true;
                    }
                }

                // Collider Component Toggle functionality
                if (component is Collider colliderComponent)
                {
                    isEnabled = colliderComponent.enabled;
                    canToggle = true;

                    if (canToggle && GUI.Button(iconRect, GUIContent.none, GUIStyle.none))
                    {
                        Undo.RecordObject(colliderComponent, "Toggle Collider");
                        colliderComponent.enabled = !isEnabled;
                        EditorUtility.SetDirty(colliderComponent);
                    }
                }

                float disabledAlpha = (canToggle && !isEnabled) ? 0.5f : 1f;
                float finalAlpha = disabledAlpha * alpha;

                // Draw the icon WITH a tooltip; use Label when not toggleable, Button when toggleable
                Color prevColor = GUI.color;
                GUI.color = new Color(prevColor.r, prevColor.g, prevColor.b, finalAlpha);

                GUIContent iconContentWithTooltip = new GUIContent(icon, tooltip);

                if (canToggle)
                {
                    if (GUI.Button(iconRect, iconContentWithTooltip, GUIStyle.none))
                    {
                        Undo.RecordObject(component, "Toggle Component");
                        bool newEnabled = !isEnabled;

                        if (component is Behaviour)
                            ((Behaviour)component).enabled = newEnabled;
                        else if (component is Renderer)
                            ((Renderer)component).enabled = newEnabled;
                        else
                        {
                            var enabledProp = component.GetType().GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
                            if (enabledProp != null && enabledProp.PropertyType == typeof(bool))
                                enabledProp.SetValue(component, newEnabled, null);
                        }

                        EditorUtility.SetDirty(component);
                    }
                }
                else
                {
                    GUI.Label(iconRect, new GUIContent(icon, tooltip), new GUIStyle() { imagePosition = ImagePosition.ImageOnly });
                }

                GUI.color = prevColor;

                // Adjust currentX for the next icon
                currentX -= iconSize + 2; // Move to the next icon

            }
            AFTER_ICON_DRAWING:
            // Draw EditorOnly Text
            DrawEditorOnlyText(go, selectionRect);
        }

        //--------- SHOW LAYERS AS TEXT NEXT TO GAMEOBJECT ----------
        private static void DrawEditorOnlyText(GameObject go, Rect selectionRect)
        {
            if (go.tag == "EditorOnly")
            {
                // Create a new GUIStyle for the hierarchy layer text to avoid affecting other EditorStyles
                GUIStyle style = new GUIStyle(EditorStyles.label)
                {
                    fontStyle = FontStyle.Italic,
                    normal = { textColor = Color.grey }, // Default Text Color
                    hover = { textColor = Color.grey } // Prevent color change on hover by setting the hover state color to the same as normal
                };

                string textToShow = "EditorOnly";

                // Calculate the width of the GameObject's name using the style
                GUIContent content = new GUIContent(go.name);
                float nameWidth = style.CalcSize(content).x;

                // Calculate an offset to add some space between the GameObject's name and the text
                float offset = 20; // You can adjust this value for more or less spacing

                // Calculate the position for the layer text to appear right after the GameObject's name
                Rect tagTextRect = new Rect(selectionRect.x + nameWidth + offset, selectionRect.y, selectionRect.width - nameWidth - offset, selectionRect.height);

                // Draw the layer name using the new GUIStyle
                GUI.Label(tagTextRect, textToShow, style);
            }

        }
    }
}