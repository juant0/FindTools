using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FindTools
{
    public class FindToolTypeMesh : FindToolTypeBaseRenderer<MeshFilter, Mesh>
    {
        public override string GetFindToolName() => "Mesh";
        protected override bool CheckRenderer(Mesh objectToMatch, MeshFilter rTypeToCompare) => rTypeToCompare.sharedMesh == objectToMatch;
        protected override bool CheckSkinnedMeshRenderer(Mesh objectToMatch, SkinnedMeshRenderer skinnedMeshRendererToCompare) => skinnedMeshRendererToCompare.sharedMesh == objectToMatch;
        
    }
}

