#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

#endif

namespace Alihan4108.SceneSwitcherBar
{
    public static class SceneSwitcherBar
    {
        const string Id = "Scenes/Build Scenes";

        public static void Refresh()
        {
            MainToolbar.Refresh(Id);
        }

        [MainToolbarElement(Id, defaultDockPosition = MainToolbarDockPosition.Right)]
        public static IEnumerable<MainToolbarElement> SceneButtons()
        {
            foreach (var guid in SceneSwitcherSettings.Selected)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var icon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image as Texture2D;

                if (string.IsNullOrEmpty(path)) continue;

                var label = Path.GetFileNameWithoutExtension(path);
                var content = new MainToolbarContent(label, icon, $"Aç: {path}");

                yield return new MainToolbarButton(content, () => OpenScene(path));
            }
        }

        static void OpenScene(string path)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(path);
            }
        }
    }
}
