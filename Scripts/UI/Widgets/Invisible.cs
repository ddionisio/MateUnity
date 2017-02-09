using UnityEngine;
using UnityEngine.UI;

namespace M8.UI.Widgets {
    [AddComponentMenu("M8/UI/Widgets/Invisible")]
    public class Invisible : Graphic {
        public override bool Raycast(Vector2 sp, Camera eventCamera) {
            return true;
        }

        protected override void OnPopulateMesh(VertexHelper vh) {
            vh.Clear();
        }
    }
}