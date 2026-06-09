#if UNITY_EDITOR

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Alihan4108.SceneSwitcherBar
{
    public class SceneSwitcherWindow : EditorWindow
    {
        string[] guids = new string[0];
        string search = "";
        Vector2 scroll;
        bool showPath;

        [MenuItem("Tools/Scene Toolbar Settings")]
        static void Open()
        {
            var window = GetWindow<SceneSwitcherWindow>(false, "Scene Toolbar");
            window.minSize = new Vector2(400, 300);
            window.Show();
        }

        void OnEnable() => Refresh();
        void OnFocus() => Refresh();
        void Refresh() => guids = AssetDatabase.FindAssets("t:SceneAsset");

        void OnGUI()
        {
            search = EditorGUILayout.TextField("Search", search);
            showPath = EditorGUILayout.ToggleLeft("Show Path", showPath, EditorStyles.boldLabel);

            EditorGUILayout.Space();

            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var name = Path.GetFileNameWithoutExtension(path);

                if (!string.IsNullOrEmpty(search) && name.IndexOf(search, System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                bool on = SceneSwitcherSettings.IsSelected(guid);

                string label = showPath ? $"{name}    ({path})" : name;
                bool now = EditorGUILayout.ToggleLeft(label, on);

                if (now != on)
                    SceneSwitcherSettings.Set(guid, now);
            }

            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var prevBg = GUI.backgroundColor;
            var btnStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold };

            GUI.backgroundColor = new Color(0.40f, 0.80f, 0.40f);
            if (GUILayout.Button("Open All", btnStyle, GUILayout.Width(100), GUILayout.Height(35)))
                SceneSwitcherSettings.EnableAll(guids);

            GUI.backgroundColor = new Color(0.85f, 0.40f, 0.40f);
            if (GUILayout.Button("Close All", btnStyle, GUILayout.Width(100), GUILayout.Height(35)))
                SceneSwitcherSettings.DisableAll();

            GUI.backgroundColor = prevBg;
            EditorGUILayout.EndHorizontal();
        }
    }
}

#endif
