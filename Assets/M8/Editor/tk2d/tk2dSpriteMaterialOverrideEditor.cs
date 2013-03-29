using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(tk2dSpriteMaterialOverride))]
class tk2dSpriteMaterialOverrideEditor : tk2dSpriteEditor {

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        tk2dSpriteMaterialOverride sprite = (tk2dSpriteMaterialOverride)target;

        sprite.material = (Material)EditorGUILayout.ObjectField("Material", sprite.material, typeof(Material), false);

        sprite.renderer.material = sprite.material;
    }
}
