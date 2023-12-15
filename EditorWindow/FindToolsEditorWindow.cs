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
        [MenuItem("Tools/FindWindow")]
        public static void ShowWindow()
        {
            GetWindow(typeof(FindToolsEditorWindow), false, "Find tool window");
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
            ShowFindToolTypes();
            if (currentFindToolType == null)
                return;
            EditorGUILayout.Separator();
            currentFindToolType.Show();
        }
        private void ShowFindToolTypes()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Select objecto to find:");
            string[] options = new string[_findToolTypes.Length];
            for (int i = 0; i < _findToolTypes.Length; i++)
                options[i] = _findToolTypes[i].GetFindToolName();
            _selectedOption = EditorGUILayout.Popup(_selectedOption, options);
            currentFindToolType = _findToolTypes[_selectedOption];
            GUILayout.EndHorizontal();
        }
    }
}
