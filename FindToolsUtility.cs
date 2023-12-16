using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System.Text.RegularExpressions;
namespace FindTools
{
    public static class FindToolsUtility
    {
        private static GUIStyle _styleButton;
        public static GUIStyle styleButton
        {
            get
            {
                if (_styleButton == null)
                {
                    _styleButton = new GUIStyle(GUI.skin.button);
                    _styleButton.alignment = TextAnchor.MiddleCenter;
                }
                return _styleButton;
            }
        }
        private static GUIStyle _middleSytle;
        public static GUIStyle middleSytle
        {
            get
            {
                if (_middleSytle == null)
                {
                    _middleSytle = new GUIStyle(GUI.skin.label);
                    _middleSytle.alignment = TextAnchor.MiddleCenter;
                }
                return _middleSytle;
            }
        }
        /// <summary>
        /// Gets a list of scenes containing objects of type <typeparamref name="C"/> using the provided FindToolType.
        /// </summary>
        /// <typeparam name="C">Type of object to find in scenes.</typeparam>
        /// <param name="findToolType">FindToolType instance with custom logic for matching.</param>
        /// <returns>List of scene paths with matching objects.</returns>
        /// <remarks>The matching is determined by the IsSame method in the FindToolType class.</remarks>
        public static List<string> GetScenesWith<C>(FindToolType findToolType) where C : Component
        {
            List<string> scenesWith = new List<string>();

            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", AssetDirectorySelection.FolderPaths);
            foreach (string sceneGuid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                if (string.IsNullOrEmpty(scenePath))
                    continue;

                if (IsSceneInReadOnlyPackage(scenePath))
                    continue;
                EditorSceneManager.OpenScene(scenePath);
                C[] objectsToCompare = Object.FindObjectsOfType<C>(true);
                if (objectsToCompare == null || objectsToCompare.Length == 0)
                    continue;
                foreach (C objectToCompare in objectsToCompare)
                {
                    if (!findToolType.IsSame(objectToCompare))
                        continue;
                    if (PrefabUtility.IsPartOfAnyPrefab(objectToCompare) && findToolType.avoidPrefabs)
                        continue;
                    scenesWith.Add(scenePath);
                    break;
                }
            }
            return scenesWith;
        }
        /// <summary>
        /// Gets a list of Prefabs containing objects of type <typeparamref name="T"/> using the provided FindToolType.
        /// </summary>
        /// <typeparam name="T">Type of object to find in GameObjects.</typeparam>
        /// <param name="findToolType">FindToolType instance with custom logic for matching.</param>
        /// <returns>List of Prefabs with matching objects.</returns>
        /// <remarks>The matching is determined by the IsSame method in the FindToolType class.</remarks>
        public static List<Object> GetPrefabsWith<T>(FindToolType findToolType) where T : Component
        {
            List<Object> objectsWith = new List<Object>();
            string[] guids = AssetDatabase.FindAssets("t:GameObject", AssetDirectorySelection.FolderPaths);
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                GameObject go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
                T[] objectsToCompare = go.GetComponentsInChildren<T>();
                foreach (T objectToCompare in objectsToCompare)
                {
                    if (findToolType.IsSame(objectToCompare))
                    {
                        objectsWith.Add(go);
                        Debug.Log(assetPath);
                        Debug.Log(File.ReadAllText(assetPath));
                        Debug.Log(File.ReadAllText(AssetDatabase.GetAssetPath(findToolType._objectToSearch)));
                        break;
                    }
                }
            }
            return objectsWith;
        }
        /// <summary>
        /// Gets a list of assets of type <typeparamref name="T"/> using the provided FindToolType and optional filter.
        /// </summary>
        /// <typeparam name="T">Type of asset to find.</typeparam>
        /// <param name="findToolType">FindToolType instance with custom logic for matching.</param>
        /// <param name="filter">Optional filter to refine the search.</param>
        /// <returns>List of assets with matching criteria.</returns>
        /// <remarks>The matching is determined by the IsSame method in the FindToolType class.</remarks>
        public static List<Object> GetAssetWith<T>(FindToolType findToolType, string filter = "") where T : Object
        {
            List<Object> assetsWith = new List<Object>();
            string[] guids = AssetDatabase.FindAssets(filter, AssetDirectorySelection.FolderPaths);
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (findToolType.IsSame(asset))
                    assetsWith.Add(asset);
            }
            return assetsWith;
        }
        /// <summary>
        /// Gets a list of assets that contain the specified object based on its GUID.
        /// </summary>
        /// <param name="objectToSearch">Object to find.</param>
        /// <returns>List of assets with the object.</returns>
        public static List<Object> GetPrefabsWithGUID(Object objectToSearch)
        {
            List<Object> objectsWith = new List<Object>();
            string objectPath = AssetDatabase.GetAssetPath(objectToSearch);
            string metaFile = AssetDatabase.GetTextMetaFilePathFromAssetPath(objectPath);
            string metaFileText = File.ReadAllText(metaFile);
            Regex guidRegex = new Regex("guid: (.+)");
            Match guidMatch = guidRegex.Match(metaFileText);
            if (!guidMatch.Success)
                return objectsWith;
            string objectGuid = guidMatch.Groups[1].Value;
            string[] guids = AssetDatabase.FindAssets("", AssetDirectorySelection.FolderPaths);
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                try
                {
                    string yaml = File.ReadAllText(assetPath);
                    if (yaml.Contains(objectGuid))
                        objectsWith.Add(AssetDatabase.LoadAssetAtPath<Object>(assetPath));
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error processing asset with GUID {guid}: {ex.Message}");
                }
            }
            return objectsWith;
        }
        /// <summary>
        /// Gets a list of scenes that contain the specified object based on its GUID.
        /// </summary>
        /// <param name="objectToSearch">Object to find.</param>
        /// <returns>List of scenes with the object.</returns>
        public static List<string> GetScenesWithGUID(Object objectToSearch)
        {
            List<string> scenesWith = new List<string>();
            string objectPath = AssetDatabase.GetAssetPath(objectToSearch);
            string metaFile = AssetDatabase.GetTextMetaFilePathFromAssetPath(objectPath);
            string metaFileText = File.ReadAllText(metaFile);
            Regex guidRegex = new Regex("guid: (.+)");
            Match guidMatch = guidRegex.Match(metaFileText);
            if (!guidMatch.Success)
                return scenesWith;
            string objectGuid = guidMatch.Groups[1].Value;
            string[] sceneGuids = AssetDatabase.FindAssets("t:Scene", AssetDirectorySelection.FolderPaths);
            foreach (string sceneGuid in sceneGuids)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
                if (string.IsNullOrEmpty(scenePath) || IsSceneInReadOnlyPackage(scenePath))
                    continue;
                try
                {
                    string yaml = File.ReadAllText(scenePath);
                    if (yaml.Contains(objectGuid))
                        scenesWith.Add(scenePath);
                }
                catch (System.Exception ex)
                {
                    Debug.LogError($"Error processing scene with GUID {sceneGuid}: {ex.Message}");
                }
            }
            return scenesWith;
        }
        /// <summary>
        /// Shows an ObjectField for a specified type with a given label.
        /// </summary>
        /// <typeparam name="T">Type of the ObjectField.</typeparam>
        /// <param name="objectToSearch">Reference to the Object variable.</param>
        /// <param name="name">Label displayed for the ObjectField.</param>
        public static void ShowFieldObjectByType<T>(ref Object objectToSearch, string name) where T : Object => objectToSearch = EditorGUILayout.ObjectField($"{name} To Find:", objectToSearch, typeof(T), false);
        /// <summary>
        /// Checks if a scene is part of a read-only package.
        /// </summary>
        /// <param name="path">Path of the scene.</param>
        /// <returns>True if the scene is part of a read-only package, false otherwise.</returns>
        private static bool IsSceneInReadOnlyPackage(string path) => UnityEditor.PackageManager.PackageInfo.FindForAssetPath(path) != null;

        public static void ShowWarnig(string warningText)
        {
            GUIContent warning = EditorGUIUtility.IconContent("d_console.warnicon.sml");
            warning.text = warningText;
            GUILayout.Label(warning, FindToolsUtility.middleSytle);
        }
    }
}
