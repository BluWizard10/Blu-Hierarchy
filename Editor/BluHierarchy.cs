using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using System.Reflection;

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

        // Ensure this method is called to initialize your custom icons
        // public static void InitializeIcons()
        // {
        //    Texture2D testIcon = Resources.Load<Texture2D>("Icons/Test-Icon");
        //    SetCustomIcon(typeof(Camera), testIcon); // Replace MyComponent with your specific component class
        //}

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
                if (component == null || component is Transform) continue;

                Texture2D icon = GetCustomIcon(component.GetType());

                if (icon == null)
                {
                    GUIContent iconContent = EditorGUIUtility.ObjectContent(component, component.GetType());
                    icon = iconContent.image as Texture2D;

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
            }
        }
    }
}