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

            // Draw the tree lines for this GameObject
            DrawTreeLines(selectionRect, go);

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
        private static void DrawTreeLines(Rect selectionRect, GameObject go)
        {
            if (go.transform.parent == null) return;
            
            Handles.BeginGUI();
            Handles.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;

            // Calculate the depth of the GameObject in the hierarchy
            int depth = GetIndentLevel(go);

            // Constants for layout - adjust these as needed for your specific hierarchy layout
            const float lineThickness = 2f; // Thickness of the lines
            const float horizontalParentOffset = 21f; // Horizontal offset from the child to the parent's vertical line

            // Vertical line position should be constant, just to the left of the GameObject icon
            float verticalLineX = selectionRect.x - horizontalParentOffset;


            // Draw the vertical line for this GameObject
            Handles.DrawAAPolyLine(lineThickness, new Vector3(verticalLineX, selectionRect.y), new Vector3(verticalLineX, selectionRect.yMax));

            for (int i = 1; i < depth; i++)
            {
                float horizontalDistance = selectionRect.x - (33 + lineThickness);
                if (i > 1)
                {
                    horizontalDistance -= i * 10;
                }
                float lineX = selectionRect.x - (horizontalParentOffset + i * (horizontalParentOffset - 7));
                Handles.DrawAAPolyLine(lineThickness, new Vector3(lineX, selectionRect.y), new Vector3(lineX, selectionRect.yMax));
            }

            // For child GameObjects, draw the horizontal line to connect to their parent's vertical line
            if (depth > 0 && go.transform.parent != null)
            {
                // The Y position for the horizontal line is the vertical center of the GameObject's rect
                float horizontalLineY = selectionRect.y + (selectionRect.height / 2f);
                float horizontalLineStart = selectionRect.x - 4;
                if (go.transform.childCount > 0)
                {
                    horizontalLineStart -= 8;
                }

                Handles.DrawAAPolyLine(lineThickness, new Vector3(verticalLineX, horizontalLineY),
                new Vector3(horizontalLineStart, horizontalLineY));
            }


            Handles.EndGUI();
        }

        private static int GetIndentLevel(GameObject go)
        {
            int level = 0;
            Transform parent = go.transform.parent;
            while (parent != null)
            {
                level++;
                parent = parent.parent;
            }
            return level;
        }
    }
}