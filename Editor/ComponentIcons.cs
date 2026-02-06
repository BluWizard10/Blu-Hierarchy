using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Dynamics.PhysBone.Components;

namespace BluWizard.Hierarchy
{
    internal static class ComponentIcons
    {
        private static readonly Dictionary<Type, Texture2D> s_CustomComponentIcons = new Dictionary<Type, Texture2D>();
        private static Texture2D s_SeparatorTexture;

        public static void SetCustomIcon(Type componentType, Texture2D icon)
        {
            s_CustomComponentIcons[componentType] = icon;
        }

        public static Texture2D GetCustomIcon(Type componentType)
        {
            s_CustomComponentIcons.TryGetValue(componentType, out Texture2D icon);
            return icon;
        }

        private static Texture2D ResolveIconForComponent(Component component, bool isDarkTheme, ref string tooltip)
        {
            var t = component.GetType();
            string typeName = t.Name;

            // UdonSharp derived classes need special handling for the script icon and tooltip
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

                        if (!EditorUtil.IsDefaultCSharpScriptIcon(candidate)) scriptIcon = candidate;

                        if (Settings.ShowTooltips)
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

        private static Texture2D GetSeparatorTexture()
        {
            if (s_SeparatorTexture != null) return s_SeparatorTexture;

            s_SeparatorTexture = new Texture2D(1, 1);
            s_SeparatorTexture.SetPixel(0, 0, Color.gray);
            s_SeparatorTexture.Apply();
            s_SeparatorTexture.hideFlags = HideFlags.HideAndDontSave;
            return s_SeparatorTexture;
        }

        public static void Draw(GameObject go, Rect selectionRect, float toggleOffset, bool drawIconsNow)
        {
            float iconSize = 16f;
            float currentX = selectionRect.xMax - iconSize - toggleOffset;

            // ---------- LAYER ICON PROCESS ----------
            if (Settings.ShowLayerIcon && drawIconsNow)
            {
                if (!EditorApplication.isPlaying)
                {
                    string layerName = LayerMask.LayerToName(go.layer);

                    if (Icons.TryGetLayerIcon(layerName, out Texture2D layerIcon) && layerIcon != null)
                    {
                        Rect layerIconRect = new Rect(currentX, selectionRect.y, iconSize, iconSize);
                        GUI.DrawTexture(layerIconRect, layerIcon);

                        currentX -= iconSize + 2;

                        float separatorWidth = 1f;
                        Rect separatorRect = new Rect(currentX, selectionRect.y, separatorWidth, selectionRect.height);
                        GUI.DrawTexture(separatorRect, GetSeparatorTexture());

                        currentX -= separatorWidth + 2;
                    }
                }
            }

            // ---------- COMPONENT ICON PROCESS ----------
            Component[] components = go.GetComponents<Component>();

            GUIStyle labelStyle = EditorStyles.label;
            float nameWidth = labelStyle.CalcSize(new GUIContent(go.name)).x;
            float minX = selectionRect.x + nameWidth + 8f;
            float fadeThreshold = minX + 16f;

            // Control Missing Scripts Process
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
                    if (currentX < fadeThreshold) alpha = Mathf.InverseLerp(minX, fadeThreshold, currentX);

                    GUIContent content = new GUIContent(warn.image, $"Missing Script(s): {missingCount}");

                    Color prev = GUI.color;
                    GUI.color = new Color(prev.r, prev.g, prev.b, alpha);
                    GUI.Label(missRect, content);
                    GUI.color = prev;
                    currentX -= iconSize + 2;
                }
            }

            // If we're not drawing icons now, skip component icons but keep missing script icon above.
            if (!drawIconsNow) return;

            foreach (Component component in components)
            {
                // Early exit the entire draw in play mode when icons are disabled
                if (EditorApplication.isPlaying && !Settings.ShowIconsInPlayMode) return;

                if (component == null) continue;

                if (!Settings.ShowHiddenComponents && (component.hideFlags & HideFlags.HideInInspector) != 0) continue;

                if (component is Transform && !Settings.ShowTransformIcon) continue;

                // Fade Out & Skip Icon if overlapping name
                if (currentX < minX) break;

                float alpha = 1f;
                if (currentX < fadeThreshold) alpha = Mathf.InverseLerp(minX, fadeThreshold, currentX);

                bool isDarkTheme = EditorGUIUtility.isProSkin;
                string tooltip = Settings.ShowTooltips ? ObjectNames.NicifyVariableName(component.GetType().Name) : null;

                // Fetch Helper and resolve Component Icons
                Texture2D icon = ResolveIconForComponent(component, isDarkTheme, ref tooltip);

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
                else
                {
                    var enabledProp = component.GetType().GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
                    if (enabledProp != null && enabledProp.PropertyType == typeof(bool))
                    {
                        isEnabled = (bool)enabledProp.GetValue(component, null);
                        canToggle = true;
                    }
                }

                // Collider Component Toggle functionality
                if (component is Collider colliderComponent)
                {
                    isEnabled = colliderComponent.enabled;
                    canToggle = true;

                    if (GUI.Button(iconRect, GUIContent.none, GUIStyle.none))
                    {
                        Undo.RecordObject(colliderComponent, "Toggle Collider");
                        colliderComponent.enabled = !isEnabled;
                        EditorUtility.SetDirty(colliderComponent);
                    }
                }

                float disabledAlpha = (canToggle && !isEnabled) ? 0.5f : 1f;
                float finalAlpha = disabledAlpha * alpha;

                // Draw icon with a tooltip; use Label when not toggleable, Button when toggleable
                Color prevColor = GUI.color;
                GUI.color = new Color(prevColor.r, prevColor.g, prevColor.b, finalAlpha);

                GUIContent iconContentWithTooltip = new GUIContent(icon, tooltip);

                if (canToggle)
                {
                    if (GUI.Button(iconRect, iconContentWithTooltip, GUIStyle.none))
                    {
                        Undo.RecordObject(component, "Toggle Component");
                        bool newEnabled = !isEnabled;

                        if (component is Behaviour) ((Behaviour)component).enabled = newEnabled;
                        else if (component is Renderer) ((Renderer)component).enabled = newEnabled;
                        else
                        {
                            var enabledProp = component.GetType().GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
                            if (enabledProp != null && enabledProp.PropertyType == typeof(bool)) enabledProp.SetValue(component, newEnabled, null);
                        }

                        EditorUtility.SetDirty(component);
                    }
                }
                else GUI.Label(iconRect, new GUIContent(icon, tooltip), new GUIStyle()
                {
                    imagePosition = ImagePosition.ImageOnly
                });

                GUI.color = prevColor;

                // Adjust currentX for next icon
                currentX -= iconSize + 2;
            }
        }
    }
}