using System.Collections.Generic;
using UnityEngine;
namespace FindTools
{
    public class FindToolTypeTerrainData : FindToolType
    {
        public override string GetFindToolName() => "TerrainData";
        protected override void ShowFieldObject() => FindToolsUtility.ShowFieldObjectByType<TerrainData>(ref _objectToSearch, "TerrainData");      
        public override bool IsSame<T>(T objectToCompare)
        {
            Terrain terrainToCompare = objectToCompare as Terrain;
            TerrainData terrainDataToMatch = _objectToSearch as TerrainData;
            return terrainToCompare.terrainData == terrainDataToMatch;
        }
        public override bool TargetIsAsset() => false;

        protected override List<string> GetScenesWithFindToolType() => FindToolsUtility.GetScenesWith<Terrain>(this);
        protected override List<Object> GetObjectWithFindToolType() => FindToolsUtility.GetPrefabsWith<Terrain>(this);
    }
}
