using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace FindTools
{
    public abstract class FindToolTypeBaseRenderer<R, O> : FindToolType where R : Component where O : Object
    {
        private enum ComponentTypes
        {
            Renderer = 1 << 0,
            SkinnedRenderer = 2 << 1,
        }
        private ComponentTypes types;
        public abstract override string GetFindToolName();
        protected override void ShowFieldObject() => FindToolsUtility.ShowFieldObjectByType<O>(ref _objectToSearch, GetFindToolName());
        public override bool IsSame<T>(T objectToCompare)
        {
            O objectToMatch = _objectToSearch as O;
            if (objectToCompare is R rTypeToCompare)
                return CheckRenderer(objectToMatch, rTypeToCompare);
            if (objectToCompare is SkinnedMeshRenderer skinnedMeshRendererToCompare)
                return CheckSkinnedMeshRenderer(objectToMatch, skinnedMeshRendererToCompare);
            return false;
        }
        public override bool TargetIsAsset() => false;
        protected override List<string> GetScenesWithFindToolType()
        {
            List<string> scenes = new List<string>();
            if ((types & ComponentTypes.Renderer) != 0)
                scenes.AddRange(FindToolsUtility.GetScenesWith<R>(this));
            if ((types & ComponentTypes.SkinnedRenderer) != 0)
                scenes.AddRange(FindToolsUtility.GetScenesWith<SkinnedMeshRenderer>(this));
            return scenes;
        }
        protected override List<Object> GetObjectWithFindToolType()
        {
            List<Object> objects = new List<Object>();
            if ((types & ComponentTypes.Renderer) != 0)
                objects.AddRange(FindToolsUtility.GetPrefabsWith<R>(this));
            /*if ((types & ComponentTypes.SkinnedRenderer) != 0)
                objects.AddRange(FindToolsUtility.GetPrefabsWith<SkinnedMeshRenderer>(this));*/
            return objects;
        }
        protected override void ExtraWindow() => types = (ComponentTypes)EditorGUILayout.EnumFlagsField("Component", types);
        protected abstract bool CheckRenderer(O objectToMatch, R rTypeToCompare);
        protected abstract bool CheckSkinnedMeshRenderer(O objectToMatch,SkinnedMeshRenderer skinnedMeshRendererToCompare);
    }
}
