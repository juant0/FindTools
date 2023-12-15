using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace FindTools
{
    public class FindToolsEditorWindow : EditorWindow
    {
        private static FindToolType[] _findToolTypes;
        public FindToolType currentFindToolType;
        private int _selectedOption = 0;
        private bool _useFindGeneric;
        private static FindToolGeneric findToolGeneric;
        [MenuItem("Tools/FindWindow")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindToolsEditorWindow), false, "Find tool window");
            findToolGeneric = new FindToolGeneric();
            GetFindToolTypes();
        }
        private static void GetFindToolTypes()
        {
            List<Type> findToolTypesList = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                IEnumerable<Type> filteredTypes = types
                .Where(type => typeof(FindToolType).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract && type != typeof(FindToolType));
                findToolTypesList.AddRange(filteredTypes);
            }
            _findToolTypes = new FindToolType[findToolTypesList.Count];
            for (int i = 0; i < findToolTypesList.Count; i++)
                _findToolTypes[i] = (FindToolType)Activator.CreateInstance(findToolTypesList[i]);
        }
        private void OnGUI()
        {
            _useFindGeneric = GUILayout.Toggle(_useFindGeneric, "Use generic find");
            EditorGUILayout.Separator();
            if (_useFindGeneric)
            {
                FindToolsUtility.ShowWarnig("Using generic find may take more time depending on project size.");
                EditorGUILayout.Separator();
                findToolGeneric.ShowUI();
                return;
            }
            ShowFindToolTypes();
            if (currentFindToolType == null)
                return;
            EditorGUILayout.Separator();
            currentFindToolType.Show();
        }
        private void ShowFindToolTypes()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select object to find:");
            try
            {
                string[] options = new string[_findToolTypes.Length];
                for (int i = 0; i < _findToolTypes.Length; i++)
                    options[i] = _findToolTypes[i].GetFindToolName();
                _selectedOption = EditorGUILayout.Popup(_selectedOption, options);
            }
            catch
            {
                Close();
            }
            currentFindToolType = _findToolTypes[_selectedOption];
            GUILayout.EndHorizontal();
        }
    }
}
