using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
namespace FindTools
{
    public class FindToolTypeCustomScript : FindToolType
    {
        public override string GetFindToolName() => "ScriptAsset";
        protected override void ShowFieldObject() => FindToolsUtility.ShowFieldObjectByType<MonoScript>(ref _objectToSearch, "Script"); 
        public override bool IsSame<T>(T objectToCompare) => true;
        public override bool TargetIsAsset() => false;

        protected override List<string> GetScenesWithFindToolType()
        {
            Type scriptType = (_objectToSearch as MonoScript).GetClass();
            MethodInfo methodInfo = typeof(FindToolsUtility).GetMethod("GetScenesWith");
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(scriptType);
            return (List<string>)genericMethod.Invoke(null, new object[] { this }); ;
        }
        protected override List<UnityEngine.Object> GetObjectWithFindToolType()
        {
            Type scriptType = (_objectToSearch as MonoScript).GetClass();
            MethodInfo methodInfo = typeof(FindToolsUtility).GetMethod("GetPrefabsWith");
            MethodInfo genericMethod = methodInfo.MakeGenericMethod(scriptType);
            return (List<UnityEngine.Object>)genericMethod.Invoke(null, new object[] { this });
        }
    }
}
