using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace FindTools
{
    /// <summary>
    /// A utility class for selecting directories in the Editor Window.
    /// Use FolderPaths to get the folder list.
    /// </summary>
    public static class AssetDirectorySelection
    {
        private class CheckDirectory
        {
            public bool showChildDirectories;
            public bool isSelected;
            public bool parentSelected;

            public CheckDirectory() { }
            public CheckDirectory(bool selected) => this.isSelected = selected;
        }
        private static string _rootPath;
        private static Dictionary<string, CheckDirectory> _directories;
        private static int _indentationSpace = 15;
        private static GUIStyle _directoryLabelStyle;
        private static string[] _defaultFolderPaths = new string[1] { "Assets" };
        private static string[] _folderPaths;
        /// <summary>
        /// Return \Assets path by default if folder paths array is not set.
        /// </summary>
        public static string[] FolderPaths
        {
            get
            {
                if (_folderPaths == null)
                    _folderPaths = new string[1] { "Assets" };
                return _usedefaulPath ? _defaultFolderPaths : _folderPaths;
            }
        }
        private static bool _initialized = false;
        private static bool _usedefaulPath = true;
        private static void Initialize()
        {
            if (_initialized)
                return;
            _rootPath = @$"{Directory.GetParent(Application.dataPath)}\Assets";
            _directories = new Dictionary<string, CheckDirectory>();
            if (!_directories.ContainsKey(_rootPath))
                _directories.Add(_rootPath, new CheckDirectory(true));
            _initialized = true;
        }
        /// <summary>
        /// Displays the AssetDirectorySelection utility in the Editor Window to set FolderPaths.
        /// </summary>
        public static void Show()
        {
            Initialize();
            _directoryLabelStyle = new GUIStyle(GUI.skin.label) { fixedWidth = 250 };
            DrawButton(_rootPath, true, false);
            if (_directories[_rootPath].showChildDirectories)
                ShowChildDirectories(_rootPath, _indentationSpace );
            List<string> folderPaths = new List<string>();
            foreach (KeyValuePair<string, CheckDirectory> kvp in _directories)
            {
                if (kvp.Key == _rootPath)
                    continue;
                if (kvp.Value.parentSelected)
                    continue;
                if (!kvp.Value.isSelected)
                    continue;
                string[] path = kvp.Key.Split(@"Assets\");
                folderPaths.Add(@$"Assets\{path[1]}");
            }
            _folderPaths = folderPaths.ToArray();
            _usedefaulPath = false;
        }
        /// <summary>
        /// Hides the AssetDirectorySelection utility setting FolderPaths to the default \Assets path.
        /// </summary>
        public static void Hide()
        {
            _usedefaulPath = true;
        }

        private static void ShowChildDirectories(string folderPath, int acumulativeSpace)
        {
            string[] childFolderPaths = GetNonHiddenDirectories(folderPath);
            foreach (string childFolderPath in childFolderPaths)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(acumulativeSpace);
                if (!_directories.ContainsKey(childFolderPath))
                    _directories.Add(childFolderPath, new CheckDirectory());
                bool hasDirestories = GetNonHiddenDirectories(childFolderPath).Length > 0;
                DrawButton(childFolderPath, hasDirestories, _directories[folderPath].isSelected);
                GUILayout.EndHorizontal();
                if (!hasDirestories)
                    continue;
                if (_directories[childFolderPath].showChildDirectories)
                    ShowChildDirectories(childFolderPath, acumulativeSpace + _indentationSpace );
            }

        }
        private static string[] GetNonHiddenDirectories(string path)
        {

            string[] allDirectories = Directory.GetDirectories(path);
            List<string> nonHiddenDirectories = new List<string>();
            foreach (string directory in allDirectories)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(directory);
                if ((directoryInfo.Attributes & FileAttributes.Hidden) == 0)
                    nonHiddenDirectories.Add(directory);
            }
            return nonHiddenDirectories.ToArray();
        }
        private static void DrawButton(string folderPath, bool showExpand, bool parentSelected)
        {
            GUILayout.BeginHorizontal();
            if (showExpand)
            {
                GUIContent expandIcon = EditorGUIUtility.IconContent(_directories[folderPath].showChildDirectories ? "d_icon dropdown" : "d_forward");
                expandIcon.text = $" {Path.GetFileName(folderPath)}";
                if (GUILayout.Button(expandIcon, _directoryLabelStyle))
                    _directories[folderPath].showChildDirectories = !_directories[folderPath].showChildDirectories;
            }
            else
                GUILayout.Label($" {Path.GetFileName(folderPath)}", _directoryLabelStyle);

            if (parentSelected)
            {
                _directories[folderPath].isSelected = true;
                GUI.enabled = false;
            }
            else if (_directories[folderPath].parentSelected)
                _directories[folderPath].isSelected = false;
            GUIContent toggleIcon = EditorGUIUtility.IconContent(_directories[folderPath].isSelected ? "TestPassed" : "TestNormal");
            if (GUILayout.Button(toggleIcon, GUI.skin.label) && !parentSelected)
                _directories[folderPath].isSelected = !_directories[folderPath].isSelected;
            GUI.enabled = true;
            _directories[folderPath].parentSelected = parentSelected;
            GUILayout.EndHorizontal();
        }
    }
}
