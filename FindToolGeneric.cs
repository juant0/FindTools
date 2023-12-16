using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FindTools
{
    public class FindToolGeneric
    {
        private Object _objectToSearch;
        private List<Object> _matchingObjects;
        private List<string> _scenesWith;
        private bool _showingScenes;
        private bool _userFolderRouth;
        /// <summary>
        /// Shows the UI for finding by scenes and prefabs.
        /// </summary>
        public void ShowUI()
        {
            _objectToSearch = EditorGUILayout.ObjectField($"Asset To Find:", _objectToSearch, typeof(Object), false);
            GUILayout.Space(5);
            if (_objectToSearch == null)
                return;
            ShowFolder();
            _showingScenes = GUILayout.Toggle(_showingScenes, "Include scenes");
            if (GUILayout.Button("Find", FindToolsUtility.styleButton))
            {
                _matchingObjects = FindToolsUtility.GetPrefabsWithGUID(_objectToSearch);
                if (_showingScenes)
                    _scenesWith = FindToolsUtility.GetScenesWithGUID(_objectToSearch);
            }
            DisplayObjects();
            if (_showingScenes)
                DisplayScenes();
        }
        private void ShowFolder()
        {
            _userFolderRouth = GUILayout.Toggle(_userFolderRouth, "Use FolderRouth");
            if (_userFolderRouth)
                AssetDirectorySelection.Show();
            else
                AssetDirectorySelection.Hide();
        }
        private void DisplayObjects()
        {
            if (_matchingObjects == null)
                return;
            if (_matchingObjects.Count == 0)
            {
                FindToolsUtility.ShowWarnig($"NOT foud Assets with {_objectToSearch.name}");
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Label($"Assets with {_objectToSearch.name}:", FindToolsUtility.middleSytle);
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
                FindToolsUtility.ShowWarnig($"NOT foud Scenes with {_objectToSearch.name}");
                return;
            }
            GUILayout.BeginVertical();
            GUILayout.Label($"Scenes with {_objectToSearch.name}:", FindToolsUtility.middleSytle);
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
    }
}
