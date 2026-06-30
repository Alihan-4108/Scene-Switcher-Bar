#if UNITY_EDITOR

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Toolbars;
using UnityEngine;

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
            var icon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image as Texture2D;

            if (SceneSwitcherSettings.DisplayMode == SceneSwitcherDisplayMode.Dropdown)
            {
                var content = new MainToolbarContent("Scenes", icon, "Open a build scene");
                yield return new MainToolbarDropdown(content, ShowSceneMenu);
                yield break;
            }

            foreach (var guid in SceneSwitcherSettings.Selected)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path)) continue;

                var label = Path.GetFileNameWithoutExtension(path);
                var content = new MainToolbarContent(label, icon, $"Open: {path}");

                yield return new MainToolbarButton(content, () => OpenScene(path));
            }
        }

        static void ShowSceneMenu(Rect rect)
        {
            var menu = new GenericMenu();

            foreach (var guid in SceneSwitcherSettings.Selected)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                if (string.IsNullOrEmpty(path)) continue;

                var label = Path.GetFileNameWithoutExtension(path);
                var scenePath = path;
                menu.AddItem(new GUIContent(label), false, () => OpenScene(scenePath));
            }

            if (menu.GetItemCount() == 0)
                menu.AddDisabledItem(new GUIContent("No scenes selected"), false);

            menu.DropDown(rect);
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

#endif
