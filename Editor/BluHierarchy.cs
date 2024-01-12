using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Reflection;
#if VRC_SDK_VRCSDK3
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace BluWizard.Hierarchy
{
    [InitializeOnLoad]
    public static class BluHierarchy
    {

        static BluHierarchy()
        {
            // Subscribe to the Editor's hierarchy window item callback
            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyGUI;
        }

        // Inside your BluHierarchy.cs now includes a simple version of IconSystem
        private static Dictionary<string, Texture2D> loadedIcons = new Dictionary<string, Texture2D>();
        private static Dictionary<System.Type, Texture2D> customComponentIcons = new Dictionary<System.Type, Texture2D>();

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
            [MenuItem("Tools/BluWizard LABS/BluHierarchy Settings")]
            public static void ShowWindow()
            {
                GetWindow<BluHierarchySettingsWindow>("BluHierarchy Settings");
            }

            void OnGUI()
            {
                bool currentShowTransformIcon = BluHierarchySettings.ShowTransformIcon;
                bool newShowTransformIcon = EditorGUILayout.Toggle("Show Transform Icon", currentShowTransformIcon);

                if (newShowTransformIcon != currentShowTransformIcon)
                {
                    BluHierarchySettings.ShowTransformIcon = newShowTransformIcon;
                    RepaintHierarchyWindow();
                }
            }
        }

        public static class BluHierarchySettings
        {
            private const string ShowTransformIconKey = "BluHierarchy_ShowTransformIcon";

            public static bool ShowTransformIcon
            {
                get => EditorPrefs.GetBool(ShowTransformIconKey, false);
                set => EditorPrefs.SetBool(ShowTransformIconKey, value);
            }
        }

        public static void RepaintHierarchyWindow()
        {
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

            //--------- COMPONENT ICONS PROCESS ----------

            // Start drawing component icons to the left of the toggle button
            float iconSize = 16;
            float currentX = selectionRect.xMax - iconSize - 22;

            // Get all components on the GameObject
            Component[] components = go.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null) continue;

                if (component is Transform && !BluHierarchySettings.ShowTransformIcon) continue;

                Texture2D icon = null;

                // Load Custom Icons for VRC Avatar SDK Components
                if (component.GetType().Name == "VRCAvatarDescriptor") { icon = Resources.Load<Texture2D>("Icons/vrcAvatarDescriptor"); }
                else if (component.GetType().Name == "PipelineManager") { icon = Resources.Load<Texture2D>("Icons/vrcPipelineManager"); }
                else if (component.GetType().Name == "VRCPhysBone") { icon = Resources.Load<Texture2D>("Icons/vrcPhysBone"); }
                #if VRC_SDK_VRCSDK3 // Prevent borking Script if VRC SDK does not exist in the project.
                else if (component.GetType().Name == "VRCPhysBoneCollider")
                {
                    var physBoneCollider = component as VRCPhysBoneCollider;
                    switch (physBoneCollider.shapeType)
                    {
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane:
                            icon = Resources.Load<Texture2D>("Icons/vrcPhysBoneColliderPlane");
                            break;
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere:
                            icon = Resources.Load<Texture2D>("Icons/vrcPhysBoneCollider");
                            break;
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule:
                            icon = Resources.Load<Texture2D>("Icons/vrcPhysBoneCollider");
                            break;
                    }
                }
                #endif
                else if (component.GetType().Name == "VRCContactReceiver") { icon = Resources.Load<Texture2D>("Icons/vrcContactReceiver"); }
                else if (component.GetType().Name == "VRCContactSender") { icon = Resources.Load<Texture2D>("Icons/vrcContactSender"); }
                else if (component.GetType().Name == "VRCImposterSettings") { icon = Resources.Load<Texture2D>("Icons/vrcImposterSettings"); }

                // Load Custom Icons for VRC World SDK Components
                else if (component.GetType().Name == "VRCSceneDescriptor") { icon = Resources.Load<Texture2D>("Icons/vrcSceneDescriptor"); }
                else if (component.GetType().Name == "UdonBehaviour") { icon = Resources.Load<Texture2D>("Icons/vrcUdonBehaviour"); }
                else if (component.GetType().Name == "VRCPickup") { icon = Resources.Load<Texture2D>("Icons/vrcPickup"); }
                else if (component.GetType().Name == "VRCMirrorReflection") { icon = Resources.Load<Texture2D>("Icons/vrcMirrorReflection"); }
                else if (component.GetType().Name == "VRCStation") { icon = Resources.Load<Texture2D>("Icons/vrcStation"); }
                // else if (component.GetType().Name == "VRCObjectSync") { icon = Resources.Load<Texture2D>("Icons/vrcObjectSync"); }
                // else if (component.GetType().Name == "VRCObjectPool") { icon = Resources.Load<Texture2D>("Icons/vrcObjectPool"); }
                // else if (component.GetType().Name == "VRCPortalMarker") { icon = Resources.Load<Texture2D>("Icons/vrcPortalMarker"); }
                // else if (component.GetType().Name == "VRCAvatarPedestal") { icon = Resources.Load<Texture2D>("Icons/vrcAvatarPedestal"); }

                // Load Custom Icons for Third-Party Utility Components
                else if (component.GetType().Name == "VRCFury") { icon = Resources.Load<Texture2D>("Icons/VRCFury"); }

                // if no match, use Unity's Default Icons
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
                GUI.color = canToggle ? (isEnabled ? Color.white : new Color(1f, 1f, 1f, 0.5f)) : Color.white;

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
                currentX -= iconSize + 2; // icon size plus some padding

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