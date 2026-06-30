#if UNITY_EDITOR

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Alihan4108.SceneSwitcherBar
{
    public enum SceneSwitcherDisplayMode
    {
        Buttons,
        Dropdown
    }

    public static class SceneSwitcherSettings
    {
        public static HashSet<string> Selected { get; private set; } = Load();

        static string Key => $"SceneToolbar.Selected:{Application.dataPath}";
        static string ModeKey => $"SceneToolbar.DisplayMode:{Application.dataPath}";

        public static SceneSwitcherDisplayMode DisplayMode
        {
            get => (SceneSwitcherDisplayMode)EditorPrefs.GetInt(ModeKey, (int)SceneSwitcherDisplayMode.Buttons);
            set
            {
                if (DisplayMode == value) return;

                EditorPrefs.SetInt(ModeKey, (int)value);
                SceneSwitcherBar.Refresh();
            }
        }

        static HashSet<string> Load()
        {
            var raw = EditorPrefs.GetString(Key, "");

            return new HashSet<string>(raw.Split(new[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries));
        }

        public static bool IsSelected(string guid) => Selected.Contains(guid);

        public static void Set(string guid, bool on)
        {
            if (on)
                Selected.Add(guid);
            else
                Selected.Remove(guid);

            Save();
        }

        static void Save()
        {
            EditorPrefs.SetString(Key, string.Join(";", Selected));
            SceneSwitcherBar.Refresh();
        }

        public static void EnableAll(IEnumerable<string> guids)
        {
            foreach (var guid in guids)
                Selected.Add(guid);

            Save();
        }

        public static void DisableAll()
        {
            Selected.Clear();
            Save();
        }
    }
}

#endif
