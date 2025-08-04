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

        // Inside your BluHierarchy.cs now includes a simple version of IconSystem
        private static Dictionary<string, Texture2D> loadedIcons = new Dictionary<string, Texture2D>();
        private static Dictionary<string, Texture2D> layerIcons = new Dictionary<string, Texture2D>();
        private static Dictionary<System.Type, Texture2D> customComponentIcons = new Dictionary<System.Type, Texture2D>();

        public static void LoadLayerIcons()
        {
            // Check if Unity is in Play Mode
            if (EditorApplication.isPlaying)
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


        //--------- SETTINGS ----------
        public class BluHierarchySettingsWindow : EditorWindow
        {
            [MenuItem("Tools/BluWizard LABS/Enhanced Hierarchy Settings")]
            public static void ShowWindow()
            {
                GetWindow<BluHierarchySettingsWindow>("Enhanced Hierarchy Settings");
            }

            void OnGUI()
            {
                //---------- GUI LAYOUT APPEARANCE ----------
                GUIStyle headerStyle = new GUIStyle(EditorStyles.label);
                headerStyle.fontSize = 18;
                headerStyle.fontStyle = FontStyle.Bold;
                headerStyle.normal.textColor = new Color(0f, 0.573f, 0.851f);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Enhanced Hierarchy", headerStyle, GUILayout.Height(28f));
                GUILayout.Space(8);
                GUIStyle authorStyle = new GUIStyle(EditorStyles.miniLabel);
                authorStyle.fontSize = 12;
                authorStyle.normal.textColor = Color.gray;
                GUILayout.Label("BluWizard LABS", authorStyle, GUILayout.Height(22f));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                //---------- SETTINGS ----------
                EditorGUI.BeginChangeCheck();

                Rect toggleRect = EditorGUILayout.GetControlRect();
                bool newShowGameObjectToggle = EditorGUI.Toggle(toggleRect,
                    new GUIContent("GameObject Toggle", "Checkbox that directly activates or deactivates the GameObject locally (GameObject.SetActive)."),
                    BluHierarchySettings.ShowGameObjectToggle);

                Rect toggleRect1 = EditorGUILayout.GetControlRect();
                bool newShowTransformIcon = EditorGUI.Toggle(toggleRect1,
                    new GUIContent("Transform Icon", "Toggles visibility of the Transform component on the GameObject."),
                    BluHierarchySettings.ShowTransformIcon);

                Rect toggleRect2 = EditorGUILayout.GetControlRect();
                bool newShowLayerIcon = EditorGUI.Toggle(toggleRect2,
                    new GUIContent("Layer Icons", "Toggles icons for Unity Layers. Common for World Devs."),
                    BluHierarchySettings.ShowLayerIcon);

                Rect toggleRect3 = EditorGUILayout.GetControlRect();
                bool newShowHiddenComponents = EditorGUI.Toggle(toggleRect3,
                    new GUIContent("Hidden Components", "Toggles visibility of hidden scripts and components. This may show isolated UdonSharp Scripts."),
                    BluHierarchySettings.ShowHiddenComponents);

                if (EditorGUI.EndChangeCheck())
                {
                    BluHierarchySettings.ShowGameObjectToggle = newShowGameObjectToggle;
                    BluHierarchySettings.ShowTransformIcon = newShowTransformIcon;
                    BluHierarchySettings.ShowLayerIcon = newShowLayerIcon;
                    BluHierarchySettings.ShowHiddenComponents = newShowHiddenComponents;
                    RepaintHierarchyWindow();
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


        private static void OnHierarchyGUI(int instanceID, Rect selectionRect)
        {
            // Convert the instance ID to a GameObject
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null) return;

            //--------- GAME OBJECT TOGGLE ----------

            if (BluHierarchySettings.ShowGameObjectToggle)
            {
                Rect toggleRect = new Rect(selectionRect);
                toggleRect.x = selectionRect.xMax - 18;
                toggleRect.width = 18;

                bool isActive = EditorGUI.Toggle(toggleRect, go.activeSelf);
                if (isActive != go.activeSelf)
                {
                    Undo.RecordObject(go, "Toggle Active State");
                    go.SetActive(isActive);
                    EditorUtility.SetDirty(go);
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

            if (BluHierarchySettings.ShowLayerIcon)
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

            //--------- SEPARATOR LINE ---------


            //--------- COMPONENT ICONS PROCESS ----------

            // Get all components on the GameObject
            Component[] components = go.GetComponents<Component>();

            // Calculate name width to prevent overlap
            GUIStyle labelStyle = EditorStyles.label;
            float nameWidth = labelStyle.CalcSize(new GUIContent(go.name)).x;
            float minX = selectionRect.x + nameWidth + 8f; // Prevent drawing over the name + some spacing
            float fadeThreshold = minX + 16f; // Start fading out slightly before the cutoff

            foreach (Component component in components)
            {
                // Check if Unity is in Play Mode
                if (EditorApplication.isPlaying)
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

                // Load Custom Icons for VRC Avatar SDK Components
                if (component.GetType().Name == "VRCAvatarDescriptor") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcAvatarDescriptor" : "Icons/vrcAvatarDescriptor_L"); }
                else if (component.GetType().Name == "PipelineManager") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPipelineManager" : "Icons/vrcPipelineManager_L"); }
                else if (component.GetType().Name == "VRCPhysBone") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBone" : "Icons/vrcPhysBone_L"); }
#if VRC_SDK_VRCSDK3 // Prevent borking Script if VRC SDK does not exist in the project, as it switches the Icon depending on type of Phys Bone Collider selected.
                else if (component.GetType().Name == "VRCPhysBoneCollider")
                {
                    var physBoneCollider = component as VRCPhysBoneCollider;
                    switch (physBoneCollider.shapeType)
                    {
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane:
                            icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBoneColliderPlane" : "Icons/vrcPhysBoneColliderPlane_L");
                            break;
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere:
                            icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBoneCollider" : "Icons/vrcPhysBoneCollider_L");
                            break;
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule:
                            icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPhysBoneCollider" : "Icons/vrcPhysBoneCollider_L");
                            break;
                    }
                }
#endif
                else if (component.GetType().Name == "VRCContactReceiver") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcContactReceiver" : "Icons/vrcContactReceiver_L"); }
                else if (component.GetType().Name == "VRCContactSender") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcContactSender" : "Icons/vrcContactSender_L"); }
                else if (component.GetType().Name == "VRCImpostorSettings") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcImpostorSettings" : "Icons/vrcImpostorSettings_L"); }
                else if (component.GetType().Name == "VRCImpostorEnvironment") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcImpostorSettings" : "Icons/vrcImpostorSettings_L"); }
                else if (component.GetType().Name == "VRCHeadChop") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcHeadChop" : "Icons/vrcHeadChop_L"); }
                else if (component.GetType().Name == "VRCParentConstraint") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcParentConstraint" : "Icons/vrcParentConstraint_L"); }
                else if (component.GetType().Name == "VRCPositionConstraint") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPositionConstraint" : "Icons/vrcPositionConstraint_L"); }
                else if (component.GetType().Name == "VRCRotationConstraint") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcRotationConstraint" : "Icons/vrcRotationConstraint_L"); }
                else if (component.GetType().Name == "VRCScaleConstraint") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcScaleConstraint" : "Icons/vrcScaleConstraint_L"); }
                else if (component.GetType().Name == "VRCAimConstraint") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcAimConstraint" : "Icons/vrcAimConstraint_L"); }
                else if (component.GetType().Name == "VRCLookAtConstraint") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcLookAtConstraint" : "Icons/vrcLookAtConstraint_L"); }
                else if (component.GetType().Name == "VRCPerPlatformOverrides") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcPerPlatformOverrides" : "Icons/vrcPerPlatformOverrides"); }

                // Load Custom Icons for VRC World SDK Components
                else if (component.GetType().Name == "VRCSceneDescriptor") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcSceneDescriptor" : "Icons/vrcSceneDescriptor_L"); }
                else if (component.GetType().Name == "UdonBehaviour") { icon = Resources.Load<Texture2D>("Icons/vrcUdonBehaviour"); }
                else if (component.GetType().Name == "VRCPickup") { icon = Resources.Load<Texture2D>("Icons/vrcPickup"); }
                else if (component.GetType().Name == "VRCMirrorReflection") { icon = Resources.Load<Texture2D>("Icons/vrcMirrorReflection"); }
                else if (component.GetType().Name == "VRCStation") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/vrcStation" : "Icons/vrcStation_L"); }
                else if (component.GetType().Name == "VRCObjectSync") { icon = Resources.Load<Texture2D>("Icons/vrcObjectSync"); }
                else if (component.GetType().Name == "VRCObjectPool") { icon = Resources.Load<Texture2D>("Icons/vrcObjectPool"); }
                else if (component.GetType().Name == "VRCPortalMarker") { icon = Resources.Load<Texture2D>("Icons/vrcPortalMarker"); }
                else if (component.GetType().Name == "VRCAvatarPedestal") { icon = Resources.Load<Texture2D>("Icons/vrcAvatarPedestal"); }
                else if (component.GetType().Name == "VRCAVProVideoPlayer") { icon = Resources.Load<Texture2D>("Icons/vrcAVProVideoPlayer"); }
                else if (component.GetType().Name == "VRCAVProVideoScreen") { icon = Resources.Load<Texture2D>("Icons/vrcAVProVideoScreen"); }
                else if (component.GetType().Name == "VRCAVProVideoSpeaker") { icon = Resources.Load<Texture2D>("Icons/vrcAVProVideoSpeaker"); }
                else if (component.GetType().Name == "VRCSpatialAudioSource") { icon = Resources.Load<Texture2D>("Icons/vrcSpatialAudioSource"); }
                else if (component.GetType().Name == "VRCUiShape") { icon = Resources.Load<Texture2D>("Icons/vrcUiShape"); }
                else if (component.GetType().Name == "VRCUnityVideoPlayer") { icon = Resources.Load<Texture2D>("Icons/vrcUnityVideoPlayer"); }
                else if (component.GetType().Name == "VRCUrlInputField") { icon = Resources.Load<Texture2D>("Icons/vrcURLInputField"); }

                // Load Custom Icons for Third-Party Utility Components
                else if (component.GetType().Name == "VRCFury") { icon = Resources.Load<Texture2D>("Icons/VRCFury"); }
                else if (component.GetType().Name == "VRCFuryComponent") { icon = Resources.Load<Texture2D>("Icons/VRCFury"); }
                else if (component.GetType().Name == "VRCFuryGlobalCollider") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/VRCFuryGlobalCollider" : "Icons/VRCFuryGlobalCollider_L"); }
                else if (component.GetType().Name == "VRCFuryHapticPlug") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/VRCFurySPSPlug" : "Icons/VRCFurySPSPlug_L"); }
                else if (component.GetType().Name == "VRCFuryHapticSocket") { icon = Resources.Load<Texture2D>("Icons/VRCFurySPSSocket"); }
                else if (component.GetType().Name == "VRCFuryHapticTouchReceiver") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/VRCFuryHapticReceiver" : "Icons/VRCFuryHapticReceiver_L"); }
                else if (component.GetType().Name == "VRCFuryHapticTouchSender") { icon = Resources.Load<Texture2D>(isDarkTheme ? "Icons/VRCFuryHapticSender" : "Icons/VRCFuryHapticSender_L"); }

                else if (component.GetType().Name == "BakeryPointLight") { icon = Resources.Load<Texture2D>("Icons/bakeryPointLight"); }
                else if (component.GetType().Name == "BakeryLightMesh") { icon = Resources.Load<Texture2D>("Icons/bakeryLightMesh"); }
                else if (component.GetType().Name == "BakeryDirectLight") { icon = Resources.Load<Texture2D>("Icons/bakeryDirectLight"); }
                else if (component.GetType().Name == "BakerySkyLight") { icon = Resources.Load<Texture2D>("Icons/bakeryDirectLight"); }
                else if (component.GetType().Name == "BakeryLightmapGroupSelector") { icon = Resources.Load<Texture2D>("Icons/bakeryGeneric"); }
                else if (component.GetType().Name == "BakeryLightmappedPrefab") { icon = Resources.Load<Texture2D>("Icons/bakeryGeneric"); }
                else if (component.GetType().Name == "BakeryPackAsSingleSquare") { icon = Resources.Load<Texture2D>("Icons/bakeryGeneric"); }
                else if (component.GetType().Name == "BakerySector") { icon = Resources.Load<Texture2D>("Icons/bakeryGeneric"); }
                else if (component.GetType().Name == "BakeryVolume") { icon = Resources.Load<Texture2D>("Icons/bakeryVolume"); }
                else if (component.GetType().Name == "ftLightmapsStorage") { icon = Resources.Load<Texture2D>("Icons/bakeryGeneric"); }

                else if (component.GetType().Name == "d4rkAvatarOptimizer") { icon = Resources.Load<Texture2D>("Icons/d4rkAvatarOptimizer"); }

                // if no match, use Unity's Default Icons (or if the component provides one) for everything else
                else
                {
                    icon = GetCustomIcon(component.GetType());
                }

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

                // Set the color for the icon based on the enabled state and if it's toggleable
                Color prevColor = GUI.color;
                GUI.color = canToggle ? (isEnabled ? new Color(1f, 1f, 1f, alpha) : new Color(1f, 1f, 1f, 0.5f * alpha)) : new Color(1f, 1f, 1f, alpha);

                // Draw the texture with the modified color
                GUI.DrawTexture(iconRect, icon);

                // Reset color to previous state
                GUI.color = prevColor;

                // Only add button functionality if the component can be toggled
                if (canToggle && GUI.Button(iconRect, GUIContent.none, GUIStyle.none))
                {
                    Undo.RecordObject(component, "Toggle Component");
                    isEnabled = !isEnabled;

                    // Apply the toggled state back to the component
                    if (component is Behaviour)
                    {
                        (component as Behaviour).enabled = isEnabled;
                    }
                    else if (component is Renderer)
                    {
                        (component as Renderer).enabled = isEnabled;
                    }

                    EditorUtility.SetDirty(component);
                }

                // Adjust currentX for the next icon
                currentX -= iconSize + 2; // Move to the next icon

            }
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