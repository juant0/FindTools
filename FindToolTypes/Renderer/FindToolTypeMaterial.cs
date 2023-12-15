using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace FindTools
{
    public class FindToolTypeMaterial : FindToolTypeBaseRenderer<Renderer, Material>
    {
        public override string GetFindToolName() => "Material";
        protected override bool CheckRenderer(Material objectToMatch, Renderer rTypeToCompare) => rTypeToCompare.sharedMaterial == objectToMatch;
        protected override bool CheckSkinnedMeshRenderer(Material objectToMatch, SkinnedMeshRenderer skinnedMeshRendererToCompare) => false;

    }
}
