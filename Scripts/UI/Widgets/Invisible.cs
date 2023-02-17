using UnityEngine;
using UnityEngine.UI;

namespace M8.UI.Widgets {
    [RequireComponent(typeof(CanvasRenderer))]
    [AddComponentMenu("M8/UI/Widgets/Invisible")]
    public class Invisible : MaskableGraphic {
        public override void SetMaterialDirty() {}
        public override void SetVerticesDirty() {}

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
        }
    }
}