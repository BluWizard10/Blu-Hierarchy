using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BluWizard.Hierarchy
{
    internal static class SceneHeader
    {
        public static void Draw(int instanceID, Rect selectionRect)
        {
            if (!ShouldShowSceneHeaderButtons()) return;
            
            // Scene Headers can come through as a Scene handle (instanceID) -> GetSceneByHandle
            // OR as a SceneAsset object -> InstanceIDToObject to return SceneAsset.
            // We support both and gracefully no-op if neither matches.
            Scene scene;
            bool hasScene = TryGetSceneFromHierarchyItem(instanceID, out scene);

            string scenePath = hasScene ? scene.path : null;
            SceneAsset sceneAsset = null;

            if (!hasScene)
            {
                // If no scene handle, try SceneAsset
                Object obj = EditorUtility.InstanceIDToObject(instanceID);
                sceneAsset = obj as SceneAsset;
                if (sceneAsset == null) return;

                scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                if (string.IsNullOrEmpty(scenePath)) return;

                if (TryGetLoadedSceneByPath(scenePath, out scene)) hasScene = true;
            }

            bool isLoaded = hasScene && scene.IsValid() && scene.isLoaded;
            bool isActive = isLoaded && scene == EditorSceneManager.GetActiveScene();

            // Layout
            GUI.BeginGroup(selectionRect);
            try
            {
                Rect row = new Rect(0f, 0f, selectionRect.width, selectionRect.height);

                // Size Buttons to Row Height
                float h = Mathf.Min(14f, row.height - 2f);
                const float yNudge = -3.5f;
                float y = (row.height - h) * 0.5f + yNudge;

                // Reserve for context menu at far right
                const float contextMenuReserved = 70f;
                const float gap = 2f;

                GUIStyle smallBtn = new GUIStyle(EditorStyles.miniButton)
                {
                    fontSize = 9,
                    padding = new RectOffset(4, 4, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0),
                };

                float right = selectionRect.xMax - contextMenuReserved;

                // Button Widths (and order of placement)
                float activeW = 60f;
                float loadW = 75f;

                // Load/Unload Button Rect
                Rect loadRect = new Rect(right - loadW, y, loadW, h);
                right = loadRect.xMin - gap;

                // Active Button Rect
                Rect activeRect = new Rect(right - activeW, y, activeW, h);

                string loadLabel = isLoaded ? "Unload Scene" : "Load Scene";

                // Load/Unload Boolean
                bool canToggleLoad = (isLoaded && EditorSceneManager.sceneCount > 1) || (!isLoaded);

                using (new EditorGUI.DisabledScope(!canToggleLoad))
                {
                    if (GUI.Button(loadRect, loadLabel, smallBtn))
                    {
                        if (isLoaded)
                        {
                            if (EditorSceneManager.SaveModifiedScenesIfUserWantsTo(new[] { scene }))
                            {
                                // IMPORTANT: Match same behavior as Unloading a Scene by keeping the scene entry in the Hierarchy
                                EditorSceneManager.CloseScene(scene, false);
                            }
                        }
                        else EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

                        GUIUtility.ExitGUI();
                    }
                }

                if (isLoaded && !isActive)
                {
                    if (GUI.Button(activeRect, "Set Active", smallBtn))
                    {
                        EditorSceneManager.SetActiveScene(scene);
                        GUIUtility.ExitGUI();
                    }
                }
            }
            finally
            {
                GUI.EndGroup();
            }
        }

        private static bool ShouldShowSceneHeaderButtons()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return false;
            
            return EditorSceneManager.GetSceneManagerSetup().Length > 1;
        }

        private static bool TryGetLoadedSceneByPath(string scenePath, out Scene scene)
        {
            int count = EditorSceneManager.sceneCount;
            for (int i = 0; i < count; i++)
            {
                Scene s = EditorSceneManager.GetSceneAt(i);
                if (s.IsValid() && string.Equals(s.path, scenePath, System.StringComparison.OrdinalIgnoreCase))
                {
                    scene = s;
                    return true;
                }
            }

            scene = default;
            return false;
        }

        private static bool TryGetSceneFromHierarchyItem(int instanceID, out Scene scene)
        {
            int count = EditorSceneManager.sceneCount;
            for (int i = 0; i < count; i++)
            {
                Scene s = EditorSceneManager.GetSceneAt(i);
                if (s.IsValid() && s.handle == instanceID)
                {
                    scene = s;
                    return true;
                }
            }

            scene = default;
            return false;
        }
    }
}
