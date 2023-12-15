using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FindTools
{
    public class FindToolTypeMaterial : FindToolType
    {
        [System.Flags]
        private enum ComponentTypes
        {
            Renderer = 1 << 0,
            SkinnedRenderer  = 2 << 1,
        }
        private ComponentTypes types;
        public override string GetFindToolName() => "Material";
        protected override void ShowFieldObject() => FindToolsUtility.ShowFieldObjectByType<Material>(ref _objectToSearch, "Material");
        public override bool IsSame<T>(T objectToCompare)
        {
            Material materialToMatch = _objectToSearch as Material;
            if (objectToCompare is Renderer rendererToCompare)
                return rendererToCompare.sharedMaterial == materialToMatch;
            if (objectToCompare is SkinnedMeshRenderer skinnedMeshRendererToCompare)
                return skinnedMeshRendererToCompare.sharedMaterial == materialToMatch;
            return false;
        }

        public override bool TargetIsAsset() => false;

        protected override List<string> GetScenesWithFindToolType()
        {
            List<string> scenes = new List<string>();
            if ((types & ComponentTypes.Renderer) != 0)
                scenes.AddRange(FindToolsUtility.GetScenesWith<Renderer>(this));
            if ((types & ComponentTypes.SkinnedRenderer ) != 0)
                scenes.AddRange(FindToolsUtility.GetScenesWith<SkinnedMeshRenderer>(this));
            return scenes;
        }
        protected override List<Object> GetObjectWithFindToolType()
        {
            List<Object> objects = new List<Object>();
            if ((types & ComponentTypes.Renderer) != 0)
                objects.AddRange(FindToolsUtility.GetPrefabsWith<Renderer>(this));
            if ((types & ComponentTypes.SkinnedRenderer ) != 0)
                objects.AddRange(FindToolsUtility.GetPrefabsWith<SkinnedMeshRenderer>(this));
            return objects;
        }

        protected override void ExtraWindow()
        {
            types = (ComponentTypes)EditorGUILayout.EnumFlagsField("Component", types);
        }
    }
}
