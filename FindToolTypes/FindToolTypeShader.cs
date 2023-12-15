using System.Collections.Generic;
using UnityEngine;
namespace FindTools
{
    public class FindToolTypeShader : FindToolType
    {
        public override string GetFindToolName() => "Shader";
        protected override void ShowFieldObject() => FindToolsUtility.ShowFieldObjectByType<Shader>(ref _objectToSearch, "Shader");

        public override bool IsSame<T>(T objectToCompare)
        {
            Material materialToCompare = objectToCompare as Material;
            Shader shaderToMatch = _objectToSearch as Shader;
            return materialToCompare.shader == shaderToMatch;
        }
        public override bool TargetIsAsset() => true;
        protected override List<Object> GetObjectWithFindToolType() => FindToolsUtility.GetAssetWith<Material>(this, "t:Material");
        protected override string AssetName() => "Materials";

    }
}
