using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
#if VRC_SDK_VRCSDK3 // Prevent breaking script if VRC SDK does not exist in the project.
using VRC.SDK3.Dynamics.PhysBone.Components;
#endif

namespace BluWizard.Hierarchy
{
    internal static class ComponentIcons
    {
        private static readonly Dictionary<Type, Texture2D> s_CustomComponentIcons = new Dictionary<Type, Texture2D>();
        private static Texture2D s_SeparatorTexture;

        private struct IconEntry
        {
            public Texture2D dark;
            public Texture2D light;
            public bool isUdonSharp;
            public bool isPhysBoneCollider;
        }

        private static readonly Dictionary<Type, IconEntry> s_IconByType = new Dictionary<Type, IconEntry>();
        private static readonly Dictionary<MonoScript, Texture2D> s_UdonSharpIconByScript = new Dictionary<MonoScript, Texture2D>();
        private static readonly Dictionary<MonoScript, string> s_UdonSharpTooltipByScript = new Dictionary<MonoScript, string>();
        private static readonly Dictionary<Type, PropertyInfo> s_EnabledPropByType = new Dictionary<Type, PropertyInfo>();
        private static readonly Dictionary<Type, string> s_NicifiedNameByType = new Dictionary<Type, string>();

        private static bool s_PbcIconsLoaded;
        private static Texture2D s_PbcPlaneDark, s_PbcPlaneLight, s_PbcSphereDark, s_PbcSphereLight;

        private static readonly List<Component> s_ComponentBuffer = new List<Component>(16);
        private static readonly GUIContent s_ScratchIconContent = new GUIContent();
        private static readonly GUIContent s_ScratchNameContent = new GUIContent();
        private static readonly GUIStyle s_IconOnlyStyle = new GUIStyle { imagePosition = ImagePosition.ImageOnly };

        public static void SetCustomIcon(Type componentType, Texture2D icon)
        {
            s_CustomComponentIcons[componentType] = icon;
        }

        public static Texture2D GetCustomIcon(Type componentType)
        {
            s_CustomComponentIcons.TryGetValue(componentType, out Texture2D icon);
            return icon;
        }

        private static IconEntry BuildIconEntry(Type t)
        {
            var entry = new IconEntry();
            string typeName = t.Name;

            // UdonSharp (base-type match) must be detected first so subclasses route to the MonoScript path.
            if (typeName == "UdonSharpBehaviour" || (t.BaseType != null && t.BaseType.Name == "UdonSharpBehaviour"))
            {
                entry.isUdonSharp = true;
                return entry;
            }

            switch (typeName)
            {
                // VRC Avatars SDK
                case "VRCAvatarDescriptor":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcAvatarDescriptor");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcAvatarDescriptor_L");
                    return entry;
                case "VRCPerPlatformOverrides":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcPerPlatformOverrides");
                    entry.light = entry.dark;
                    return entry;
                case "VRCHeadChop":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcHeadChop");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcHeadChop_L");
                    return entry;
                case "VRCImpostorSettings":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcImpostorSettings");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcImpostorSettings_L");
                    return entry;
                case "VRCImpostorEnvironment":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcImpostorSettings");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcImpostorSettings_L");
                    return entry;
                case "VRCRaycast":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcRaycast");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcRaycast_L");
                    return entry;

                // VRC Worlds SDK
                case "VRCSceneDescriptor":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcSceneDescriptor");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcSceneDescriptor_L");
                    return entry;
                case "UdonBehaviour":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcUdonBehaviour");
                    entry.light = entry.dark;
                    return entry;
                case "VRCPickup":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcPickup");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcPickup_L");
                    return entry;
                case "VRCMirrorReflection":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcMirrorReflection");
                    entry.light = entry.dark;
                    return entry;
                case "VRCStation":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcStation");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcStation_L");
                    return entry;
                case "VRCObjectSync":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcObjectSync");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcObjectSync_L");
                    return entry;
                case "VRCObjectPool":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcObjectPool");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcObjectPool_L");
                    return entry;
                case "VRCPortalMarker":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcPortalMarker");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcPortalMarker_L");
                    return entry;
                case "VRCAvatarPedestal":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcAvatarPedestal");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcAvatarPedestal_L");
                    return entry;
                case "VRCAVProVideoPlayer":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcAVProVideoPlayer");
                    entry.light = entry.dark;
                    return entry;
                case "VRCAVProVideoScreen":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcAVProVideoScreen");
                    entry.light = entry.dark;
                    return entry;
                case "VRCAVProVideoSpeaker":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcAVProVideoSpeaker");
                    entry.light = entry.dark;
                    return entry;
                case "VRCUiShape":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcUiShape");
                    entry.light = entry.dark;
                    return entry;
                case "VRCUnityVideoPlayer":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcUnityVideoPlayer");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcUnityVideoPlayer_L");
                    return entry;
                case "VRCUrlInputField":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcURLInputField");
                    entry.light = entry.dark;
                    return entry;
                case "VRCCameraDollyAnimation":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcCameraDollyAnimation");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcCameraDollyAnimation_L");
                    return entry;
                case "VRCCameraDollyPath":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcCameraDollyPath");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcCameraDollyAnimation_L");
                    return entry;
                case "VRCCameraDollyPathPoint":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcCameraDollyPoint");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcCameraDollyPoint_L");
                    return entry;

                // VRC Base SDK
                case "PipelineManager":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcPipelineManager");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcPipelineManager_L");
                    return entry;
                case "VRCPhysBone":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcPhysBone");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcPhysBone_L");
                    return entry;
                case "VRCPhysBoneRoot":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcPhysBoneRoot");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcPhysBoneRoot_L");
                    return entry;
                #if VRC_SDK_VRCSDK3
                case "VRCPhysBoneCollider":
                    entry.isPhysBoneCollider = true;
                    return entry;
                #else
                case "VRCPhysBoneCollider":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcPhysBoneCollider");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcPhysBoneCollider_L");
                    return entry;
                #endif
                case "VRCContactReceiver":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcContactReceiver");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcContactReceiver_L");
                    return entry;
                case "VRCContactSender":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcContactSender");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcContactSender_L");
                    return entry;
                case "VRCParentConstraint":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcParentConstraint");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcParentConstraint_L");
                    return entry;
                case "VRCPositionConstraint":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcPositionConstraint");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcPositionConstraint_L");
                    return entry;
                case "VRCRotationConstraint":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcRotationConstraint");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcRotationConstraint_L");
                    return entry;
                case "VRCScaleConstraint":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcScaleConstraint");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcScaleConstraint_L");
                    return entry;
                case "VRCAimConstraint":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcAimConstraint");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcAimConstraint_L");
                    return entry;
                case "VRCLookAtConstraint":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcLookAtConstraint");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcLookAtConstraint_L");
                    return entry;
                case "VRCSpatialAudioSource":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrcSpatialAudioSource");
                    entry.light = Resources.Load<Texture2D>("Icons/vrcSpatialAudioSource_L");
                    return entry;

                // Third-Party Utilities
                case "VRCFury":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFury");
                    entry.light = entry.dark;
                    return entry;
                case "VRCFuryComponent":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFury");
                    entry.light = entry.dark;
                    return entry;
                case "VRCFuryGlobalCollider":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFuryGlobalCollider");
                    entry.light = Resources.Load<Texture2D>("Icons/VRCFuryGlobalCollider_L");
                    return entry;
                case "VRCFuryHapticPlug":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFurySPSPlug");
                    entry.light = Resources.Load<Texture2D>("Icons/VRCFurySPSPlug_L");
                    return entry;
                case "VRCFuryHapticSocket":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFurySPSSocket");
                    entry.light = entry.dark;
                    return entry;
                case "VRCFuryHapticTouchReceiver":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFuryHapticReceiver");
                    entry.light = Resources.Load<Texture2D>("Icons/VRCFuryHapticReceiver_L");
                    return entry;
                case "VRCFuryHapticTouchSender":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFuryHapticSender");
                    entry.light = Resources.Load<Texture2D>("Icons/VRCFuryHapticSender_L");
                    return entry;
                case "VRCFuryDebugInfo":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFuryDebugInfo");
                    entry.light = entry.dark;
                    return entry;
                case "VRCFuryTest":
                    entry.dark = Resources.Load<Texture2D>("Icons/VRCFuryDebugInfo");
                    entry.light = entry.dark;
                    return entry;

                case "BakeryPointLight":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryPointLight");
                    entry.light = entry.dark;
                    return entry;
                case "BakeryLightMesh":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryLightMesh");
                    entry.light = entry.dark;
                    return entry;
                case "BakeryDirectLight":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryDirectLight");
                    entry.light = entry.dark;
                    return entry;
                case "BakerySkyLight":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryDirectLight");
                    entry.light = entry.dark;
                    return entry;
                case "BakeryLightmapGroupSelector":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryGeneric");
                    entry.light = entry.dark;
                    return entry;
                case "BakeryLightmappedPrefab":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryGeneric");
                    entry.light = entry.dark;
                    return entry;
                case "BakeryPackAsSingleSquare":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryGeneric");
                    entry.light = entry.dark;
                    return entry;
                case "BakerySector":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryGeneric");
                    entry.light = entry.dark;
                    return entry;
                case "BakeryVolume":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryVolume");
                    entry.light = entry.dark;
                    return entry;
                case "ftLightmapsStorage":
                    entry.dark = Resources.Load<Texture2D>("Icons/bakeryGeneric");
                    entry.light = entry.dark;
                    return entry;

                case "d4rkAvatarOptimizer":
                    entry.dark = Resources.Load<Texture2D>("Icons/d4rkAvatarOptimizer");
                    entry.light = entry.dark;
                    return entry;

                case "GestureManager":
                    entry.dark = Resources.Load<Texture2D>("Icons/gestureManager");
                    entry.light = Resources.Load<Texture2D>("Icons/gestureManager_L");
                    return entry;
                
                case "FaceEmoLauncherComponent":
                    entry.dark = Resources.Load<Texture2D>("Icons/FaceEmo");
                    entry.light = Resources.Load<Texture2D>("Icons/FaceEmo_L");
                    return entry;
                case "BlinkDisabler":
                    entry.dark = Resources.Load<Texture2D>("Icons/FaceEmo");
                    entry.light = Resources.Load<Texture2D>("Icons/FaceEmo_L");
                    return entry;
                case "TrackingControlDisabler":
                    entry.dark = Resources.Load<Texture2D>("Icons/FaceEmo");
                    entry.light = Resources.Load<Texture2D>("Icons/FaceEmo_L");
                    return entry;
                case "MenuRepositoryComponent":
                    entry.dark = Resources.Load<Texture2D>("Icons/FaceEmo");
                    entry.light = Resources.Load<Texture2D>("Icons/FaceEmo_L");
                    return entry;
                case "MenuRepositoryTestComponent":
                    entry.dark = Resources.Load<Texture2D>("Icons/FaceEmo");
                    entry.light = Resources.Load<Texture2D>("Icons/FaceEmo_L");
                    return entry;
                case "RestorationCheckpoint":
                    entry.dark = Resources.Load<Texture2D>("Icons/FaceEmo");
                    entry.light = Resources.Load<Texture2D>("Icons/FaceEmo_L");
                    return entry;

                // Other Component Types
                case "VRMMeta":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrmMeta");
                    entry.light = Resources.Load<Texture2D>("Icons/vrmMeta_L");
                    return entry;
                case "VRMBlendShapeProxy":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrmBlendShapeProxy");
                    entry.light = Resources.Load<Texture2D>("Icons/vrmBlendShapeProxy_L");
                    return entry;
                case "VRMSpringBone":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrmSpringBone");
                    entry.light = Resources.Load<Texture2D>("Icons/vrmSpringBone_L");
                    return entry;
                case "VRMSpringBoneColliderGroup":
                    entry.dark = Resources.Load<Texture2D>("Icons/vrmSpringBoneColliderGroup");
                    entry.light = Resources.Load<Texture2D>("Icons/vrmSpringBoneColliderGroup_L");
                    return entry;

                case "DynamicBone":
                    entry.dark = Resources.Load<Texture2D>("Icons/dynamicBone");
                    entry.light = Resources.Load<Texture2D>("Icons/dynamicBone_L");
                    return entry;
                case "DynamicBoneCollider":
                    entry.dark = Resources.Load<Texture2D>("Icons/dynamicBoneCollider");
                    entry.light = Resources.Load<Texture2D>("Icons/dynamicBoneCollider_L");
                    return entry;
                case "DynamicBoneColliderBase":
                    entry.dark = Resources.Load<Texture2D>("Icons/dynamicBoneCollider");
                    entry.light = Resources.Load<Texture2D>("Icons/dynamicBoneCollider_L");
                    return entry;
                case "DynamicBonePlaneCollider":
                    entry.dark = Resources.Load<Texture2D>("Icons/dynamicBonePlaneCollider");
                    entry.light = Resources.Load<Texture2D>("Icons/dynamicBonePlaneCollider_L");
                    return entry;
            }

            return entry;
        }

        private static IconEntry GetIconEntry(Type t)
        {
            if (s_IconByType.TryGetValue(t, out var entry)) return entry;
            entry = BuildIconEntry(t);
            s_IconByType[t] = entry;
            return entry;
        }

        private static void EnsurePhysBoneColliderIcons()
        {
            if (s_PbcIconsLoaded) return;
            s_PbcPlaneDark = Resources.Load<Texture2D>("Icons/vrcPhysBoneColliderPlane");
            s_PbcPlaneLight = Resources.Load<Texture2D>("Icons/vrcPhysBoneColliderPlane_L");
            s_PbcSphereDark = Resources.Load<Texture2D>("Icons/vrcPhysBoneCollider");
            s_PbcSphereLight = Resources.Load<Texture2D>("Icons/vrcPhysBoneCollider_L");
            s_PbcIconsLoaded = true;
        }

        private static Texture2D ResolveUdonSharpIcon(MonoBehaviour mb, bool showTooltip, ref string tooltip)
        {
            var monoScript = MonoScript.FromMonoBehaviour(mb);
            if (monoScript == null) return Resources.Load<Texture2D>("Icons/vrcUdonSharpBehaviour");

            if (!s_UdonSharpIconByScript.TryGetValue(monoScript, out var icon))
            {
                var gc = EditorGUIUtility.ObjectContent(monoScript, typeof(MonoScript));
                var candidate = gc.image as Texture2D;
                icon = EditorUtil.IsDefaultCSharpScriptIcon(candidate) ? null : candidate;
                s_UdonSharpIconByScript[monoScript] = icon;
            }

            if (showTooltip)
            {
                if (!s_UdonSharpTooltipByScript.TryGetValue(monoScript, out var tip))
                {
                    var cls = monoScript.GetClass();
                    string className = cls != null ? cls.Name : monoScript.name;
                    tip = $"Udon Sharp Behaviour ({className})";
                    s_UdonSharpTooltipByScript[monoScript] = tip;
                }
                tooltip = tip;
            }

            return icon != null ? icon : Resources.Load<Texture2D>("Icons/vrcUdonSharpBehaviour");
        }

        private static Texture2D ResolveIconForComponent(Component component, Type t, bool isDarkTheme, ref string tooltip)
        {
            var entry = GetIconEntry(t);

            if (entry.isUdonSharp)
            {
                var mb = component as MonoBehaviour;
                if (mb != null) return ResolveUdonSharpIcon(mb, Settings.ShowTooltips, ref tooltip);
                return Resources.Load<Texture2D>("Icons/vrcUdonSharpBehaviour");
            }

            if (entry.isPhysBoneCollider)
            {
            #if VRC_SDK_VRCSDK3
                EnsurePhysBoneColliderIcons();
                var physBoneCollider = component as VRCPhysBoneCollider;
                if (physBoneCollider != null)
                {
                    switch (physBoneCollider.shapeType)
                    {
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Plane:
                            return isDarkTheme ? s_PbcPlaneDark : s_PbcPlaneLight;
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Sphere:
                        case VRC.Dynamics.VRCPhysBoneColliderBase.ShapeType.Capsule:
                            return isDarkTheme ? s_PbcSphereDark : s_PbcSphereLight;
                    }
                }
            #endif
            }

            if (entry.dark != null || entry.light != null)
            {
                return isDarkTheme ? entry.dark : entry.light;
            }

            var custom = GetCustomIcon(t);
            if (custom != null) return custom;

            var iconContent = EditorGUIUtility.ObjectContent(component, t);
            return iconContent.image as Texture2D;
        }

        private static PropertyInfo GetEnabledProperty(Type t)
        {
            if (s_EnabledPropByType.TryGetValue(t, out var prop)) return prop;
            prop = t.GetProperty("enabled", BindingFlags.Instance | BindingFlags.Public);
            if (prop != null && prop.PropertyType != typeof(bool)) prop = null;
            s_EnabledPropByType[t] = prop;
            return prop;
        }

        private static string GetNicifiedTypeName(Type t)
        {
            if (s_NicifiedNameByType.TryGetValue(t, out var name)) return name;
            name = ObjectNames.NicifyVariableName(t.Name);
            s_NicifiedNameByType[t] = name;
            return name;
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
            GUIStyle labelStyle = EditorStyles.label;
            s_ScratchNameContent.text = go.name;
            float nameWidth = labelStyle.CalcSize(s_ScratchNameContent).x;
            float minX = selectionRect.x + nameWidth + 8f;
            float fadeThreshold = minX + 16f;

            // Control Missing Scripts Process
            int missingCount;
            #if UNITY_2019_2_OR_NEWER
            missingCount = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
            #else
            missingCount = 0;
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

                    s_ScratchIconContent.image = warn.image;
                    s_ScratchIconContent.tooltip = $"Missing Script(s): {missingCount}";

                    Color prev = GUI.color;
                    GUI.color = new Color(prev.r, prev.g, prev.b, alpha);
                    GUI.Label(missRect, s_ScratchIconContent);
                    GUI.color = prev;
                    currentX -= iconSize + 2;
                }
            }

            // If we're not drawing icons now, skip component icons but keep missing script icon above.
            if (!drawIconsNow) return;

            // Early exit the entire draw in play mode when icons are disabled
            if (EditorApplication.isPlaying && !Settings.ShowIconsInPlayMode) return;

            bool isDarkTheme = EditorGUIUtility.isProSkin;
            bool showTooltips = Settings.ShowTooltips;
            bool showHidden = Settings.ShowHiddenComponents;
            bool showTransform = Settings.ShowTransformIcon;

            go.GetComponents(s_ComponentBuffer);
            int count = s_ComponentBuffer.Count;

            for (int ci = 0; ci < count; ci++)
            {
                Component component = s_ComponentBuffer[ci];

                if (component == null) continue;

                if (!showHidden && (component.hideFlags & HideFlags.HideInInspector) != 0) continue;

                if (component is Transform && !showTransform) continue;

                // Fade Out & Skip Icon if overlapping name
                if (currentX < minX) break;

                float alpha = 1f;
                if (currentX < fadeThreshold) alpha = Mathf.InverseLerp(minX, fadeThreshold, currentX);

                Type componentType = component.GetType();
                string tooltip = showTooltips ? GetNicifiedTypeName(componentType) : null;

                // Fetch Helper and resolve Component Icons
                Texture2D icon = ResolveIconForComponent(component, componentType, isDarkTheme, ref tooltip);

                if (icon == null)
                {
                    GUIContent iconContent = EditorGUIUtility.ObjectContent(component, componentType);
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
                    var enabledProp = GetEnabledProperty(componentType);
                    if (enabledProp != null)
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

                s_ScratchIconContent.image = icon;
                s_ScratchIconContent.tooltip = tooltip;

                if (canToggle)
                {
                    if (GUI.Button(iconRect, s_ScratchIconContent, GUIStyle.none))
                    {
                        Undo.RecordObject(component, "Toggle Component");
                        bool newEnabled = !isEnabled;

                        if (component is Behaviour bh) bh.enabled = newEnabled;
                        else if (component is Renderer rn) rn.enabled = newEnabled;
                        else
                        {
                            var enabledProp = GetEnabledProperty(componentType);
                            if (enabledProp != null) enabledProp.SetValue(component, newEnabled, null);
                        }

                        EditorUtility.SetDirty(component);
                    }
                }
                else
                {
                    GUI.Label(iconRect, s_ScratchIconContent, s_IconOnlyStyle);
                }

                GUI.color = prevColor;

                // Adjust currentX for next icon
                currentX -= iconSize + 2;
            }

            s_ComponentBuffer.Clear();
        }
    }
}
