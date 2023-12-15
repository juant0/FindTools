using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
namespace FindTools
{
    /// <summary>
    /// Base class used to create custom logic for ToolType to be added in the FindToolWindow.
    /// </summary>
    public abstract class FindToolType
    {
        private bool _showingScens;
        [HideInInspector] public bool avoidPrefabs;
        private List<string> _scenesWith;
        private List<Object> _matchingObjects;
        protected Object _objectToSearch;
        private bool _userFolderRouth;
        protected string[] _folderPaths;
        /// <summary>
        /// Shows the UI for finding and displaying the custom ToolType by scenes and prefabs.
        /// </summary>
        public void Show()
        {
            EditorGUILayout.Separator();
            ShowFieldObject();
            GUILayout.Space(5);
            if (_objectToSearch == null)
                return;
            ShowFolder();
            EditorGUILayout.LabelField($"{GetFindToolName()} to find", FindToolsUtility.middleSytle);
            ExtraWindow();
            if (TargetIsAsset())
            {
                ShowAssets();
                return;
            }
            ShowPrefabsAndScenes();
        }
        private void ShowFolder()
        {
            _userFolderRouth = GUILayout.Toggle(_userFolderRouth, "Use FolderRouth");
            if (_userFolderRouth)
                AssetDirectorySelection.Show();
            else
                AssetDirectorySelection.Hide();
        }
        private void ShowAssets()
        {
            GUILayout.Space(5);
            if (GUILayout.Button($"Find {AssetName()}", FindToolsUtility.styleButton))
                _matchingObjects = GetObjectWithFindToolType();
            DisplayObjects();
        }
        private void ShowPrefabsAndScenes()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Prefabs", FindToolsUtility.styleButton))
                _showingScens = false;
            if (GUILayout.Button("Scenes", FindToolsUtility.styleButton))
                _showingScens = true;
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            if (_showingScens)
            {
                if (GUILayout.Button("Find Scene", FindToolsUtility.styleButton))
                    _scenesWith = GetScenesWithFindToolType();
                avoidPrefabs = GUILayout.Toggle(avoidPrefabs, "Avoid prefabs");
                DisplayScenes();
                return;
            }
            if (GUILayout.Button("Find Prefabs", FindToolsUtility.styleButton))
                _matchingObjects = GetObjectWithFindToolType();
            DisplayObjects();
        }
        private void DisplayObjects()
        {

            if (_matchingObjects == null)
                return;
            if (_matchingObjects.Count == 0)
            {
                GUIContent warning = EditorGUIUtility.IconContent("d_console.warnicon.sml");
                warning.text = $"NOT foud {AssetName()} with {GetFindToolName()}";
                GUILayout.Label(warning, FindToolsUtility.middleSytle);
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Label($"{AssetName()} with {GetFindToolName()}:", FindToolsUtility.middleSytle);
            foreach (Object go in _matchingObjects)
            {
                if (GUILayout.Button(go.name, FindToolsUtility.styleButton))
                    Selection.activeObject = go;
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
        private void DisplayScenes()
        {
            if (_scenesWith == null)
                return;
            if (_scenesWith.Count == 0)
            {
                GUIContent warning = EditorGUIUtility.IconContent("d_console.warnicon.sml");
                warning.text = $"NOT foud Scenes with {GetFindToolName()}";
                GUILayout.Label(warning, FindToolsUtility.middleSytle);
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Label($"Scenes with {GetFindToolName()}:", FindToolsUtility.middleSytle);
            foreach (string scenPath in _scenesWith)
            {
                string[] paths = scenPath.Split('/');
                string[] file = paths[paths.Length - 1].Split('.');
                string label = file[0];
                if (GUILayout.Button(label, FindToolsUtility.styleButton))
                    EditorSceneManager.OpenScene(scenPath);
            }
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
        /// <summary>
        /// Base class to create the field type that you want.
        /// </summary>
        protected abstract void ShowFieldObject();
        /// <summary>
        /// Base class to implement custom logic to find objects in scenes.
        /// </summary>
        /// <returns>A list of scenes with the specified ToolType.</returns>
        public abstract string GetFindToolName();
        /// <summary>
        /// Base class called in the FindToolsUtility.GetScenesWith and FindToolsUtility.GetObjectsWith.
        /// Uses to create custom logic for matching.
        /// </summary>
        /// <typeparam name="T">Type of object to compare.</typeparam>
        /// <param name="objectToCompare">The object to compare.</param>
        /// <returns>True if the objects has the FindToolType, false otherwise.</returns>
        public abstract bool IsSame<T>(T objectToCompare);
        /// <summary>
        /// Checks if the output object is an asset.
        /// </summary>
        /// <returns>True if the output object is an asset; otherwise, it could be a prefab or scene, returning false.</returns>
        public abstract bool TargetIsAsset();

        /// <summary>
        /// Base class to implement custom logic to find objects in prefabs when TargetIsAsset is false.
        /// </summary>
        /// <returns>A list of Scene path with the specified ToolType.</returns>
        protected virtual List<string> GetScenesWithFindToolType() => null;
        /// <summary>
        /// Base class to do the custom logic to find the "object/component" in the object.
        /// </summary>
        /// <returns>A list of object with the specified ToolType</returns>
        protected virtual List<Object> GetObjectWithFindToolType() => null;
        /// <summary>
        /// Base class to give the Asset type name when TargetIsAsset is true.
        /// </summary>
        /// <returns>The asset type name</returns>
        protected virtual string AssetName() => "GameObject";

        protected virtual void ExtraWindow() { }
    }
}
