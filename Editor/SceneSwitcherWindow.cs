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

        const float RowHeight = 26f;

        GUIStyle countBadge;
        GUIStyle rowLabel;
        GUIStyle rowPath;
        Texture2D sceneIcon;
        bool stylesReady;

        [MenuItem("Tools/Scene Toolbar Settings")]
        static void Open()
        {
            var window = GetWindow<SceneSwitcherWindow>(false, "Scene Toolbar");
            window.minSize = new Vector2(420, 320);
            window.wantsMouseMove = true;
            window.Show();
        }

        void OnEnable() => Refresh();
        void OnFocus() => Refresh();
        void Refresh() => guids = AssetDatabase.FindAssets("t:SceneAsset");

        void BuildStyles()
        {
            if (stylesReady) return;

            sceneIcon = EditorGUIUtility.IconContent("d_SceneAsset Icon").image as Texture2D;

            countBadge = new GUIStyle(EditorStyles.miniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = EditorGUIUtility.isProSkin ? new Color(0.7f, 0.85f, 1f) : new Color(0.1f, 0.25f, 0.5f) }
            };

            rowLabel = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft
            };

            rowPath = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                normal = { textColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.5f) : new Color(0f, 0f, 0f, 0.5f) }
            };

            stylesReady = true;
        }

        void OnGUI()
        {
            BuildStyles();

            if (Event.current.type == EventType.MouseMove)
                Repaint();

            DrawModeSelector();
            DrawSearchBar();

            EditorGUILayout.Space(4);
            DrawSceneList();

            DrawFooter();
        }

        void DrawModeSelector()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Display", GUILayout.Width(60));

            var current = (int)SceneSwitcherSettings.DisplayMode;
            var next = GUILayout.Toolbar(current, new[] { "  Buttons", "  Dropdown" }, GUILayout.Height(22));
            if (next != current)
                SceneSwitcherSettings.DisplayMode = (SceneSwitcherDisplayMode)next;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField(
                SceneSwitcherSettings.DisplayMode == SceneSwitcherDisplayMode.Buttons
                    ? "Scenes appear as separate buttons side by side."
                    : "Scenes are grouped into one dropdown menu.",
                EditorStyles.centeredGreyMiniLabel);

            EditorGUILayout.Space(2);
        }

        void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal();

            var searchStyle = GUI.skin.FindStyle("ToolbarSearchTextField") ?? EditorStyles.toolbarTextField;
            var cancelStyle = GUI.skin.FindStyle("ToolbarSearchCancelButton") ?? GUI.skin.button;

            search = GUILayout.TextField(search, searchStyle);
            if (GUILayout.Button(GUIContent.none, cancelStyle))
            {
                search = "";
                GUI.FocusControl(null);
            }

            GUILayout.Space(6);
            showPath = GUILayout.Toggle(showPath, "Path", EditorStyles.miniButton, GUILayout.Width(48));

            EditorGUILayout.EndHorizontal();
        }

        void DrawSceneList()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandHeight(true));

            var hoverColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.05f) : new Color(0f, 0f, 0f, 0.04f);
            var altColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.02f) : new Color(0f, 0f, 0f, 0.02f);

            int drawn = 0;

            for (int i = 0; i < guids.Length; i++)
            {
                var guid = guids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var name = Path.GetFileNameWithoutExtension(path);

                if (!string.IsNullOrEmpty(search) && name.IndexOf(search, System.StringComparison.OrdinalIgnoreCase) < 0)
                    continue;

                bool on = SceneSwitcherSettings.IsSelected(guid);

                float rowHeight = showPath ? 40f : RowHeight;
                var row = GUILayoutUtility.GetRect(0, rowHeight, GUILayout.ExpandWidth(true));
                bool hover = row.Contains(Event.current.mousePosition);

                if (hover) EditorGUI.DrawRect(row, hoverColor);
                else if (drawn % 2 == 1) EditorGUI.DrawRect(row, altColor);

                var checkRect = new Rect(row.x + 8, row.y + (rowHeight - 16) / 2, 16, 16);
                bool now = GUI.Toggle(checkRect, on, GUIContent.none);

                var iconRect = new Rect(checkRect.xMax + 6, row.y + (rowHeight - 16) / 2, 16, 16);
                if (sceneIcon != null) GUI.DrawTexture(iconRect, sceneIcon, ScaleMode.ScaleToFit);

                var labelRect = new Rect(iconRect.xMax + 6, row.y, row.width - iconRect.xMax - 10, rowHeight);
                if (showPath)
                {
                    var nameRect = new Rect(labelRect.x, row.y + 5, labelRect.width, 17);
                    var pathRect = new Rect(labelRect.x, row.y + 21, labelRect.width, 15);
                    GUI.Label(nameRect, name, rowLabel);
                    GUI.Label(pathRect, path, rowPath);
                }
                else
                {
                    GUI.Label(labelRect, name, rowLabel);
                }

                if (Event.current.type == EventType.MouseDown && hover && !checkRect.Contains(Event.current.mousePosition))
                {
                    now = !on;
                    Event.current.Use();
                }

                if (now != on)
                    SceneSwitcherSettings.Set(guid, now);

                drawn++;
            }

            if (drawn == 0)
            {
                EditorGUILayout.Space(20);
                EditorGUILayout.LabelField(
                    guids.Length == 0 ? "No scenes found in the project." : "No scenes match your search.",
                    EditorStyles.centeredGreyMiniLabel);
            }

            EditorGUILayout.EndScrollView();
        }

        void DrawFooter()
        {
            var line = EditorGUILayout.GetControlRect(false, 1f);
            EditorGUI.DrawRect(line, EditorGUIUtility.isProSkin ? new Color(0f, 0f, 0f, 0.3f) : new Color(0f, 0f, 0f, 0.12f));

            EditorGUILayout.Space(4);
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            var prevBg = GUI.backgroundColor;
            var btnStyle = new GUIStyle(GUI.skin.button) { fontStyle = FontStyle.Bold };

            GUI.backgroundColor = new Color(0.40f, 0.80f, 0.40f);
            if (GUILayout.Button("Open All", btnStyle, GUILayout.Width(110), GUILayout.Height(32)))
                SceneSwitcherSettings.EnableAll(guids);

            GUILayout.Space(6);

            GUI.backgroundColor = new Color(0.85f, 0.40f, 0.40f);
            if (GUILayout.Button("Close All", btnStyle, GUILayout.Width(110), GUILayout.Height(32)))
                SceneSwitcherSettings.DisableAll();

            GUI.backgroundColor = prevBg;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(4);
        }
    }
}

#endif
