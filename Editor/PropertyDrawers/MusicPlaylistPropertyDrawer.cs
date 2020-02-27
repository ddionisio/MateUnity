using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace M8 {
    [CustomPropertyDrawer(typeof(MusicPlaylistAttribute))]
    public class MusicPlaylistPropertyDrawer : PropertyDrawer {
        private static MusicPlaylist mPlaylist;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            if(!mPlaylist) {
                //manually grab
                mPlaylist = AssetDatabase.LoadAssetAtPath<MusicPlaylist>(MusicPlaylist.assetPath);
                if(!mPlaylist) {
                    //grab first instance of type from assets (there should only be one anyhow)                    
                    var guids = AssetDatabase.FindAssets("t:" + typeof(MusicPlaylist).Name);
                    if(guids.Length > 0)
                        mPlaylist = AssetDatabase.LoadAssetAtPath<MusicPlaylist>(AssetDatabase.GUIDToAssetPath(guids[0]));
                }
            }

            if(mPlaylist) {
                EditorGUI.BeginProperty(position, label, property);

                //generate names
                var musicNameList = new List<string>();
                musicNameList.Add("<None>");
                for(int i = 0; i < mPlaylist.music.Length; i++)
                    musicNameList.Add(mPlaylist.music[i].name);

                var curMusicName = property.stringValue;

                //get current take name list index
                int index = -1;
                if(string.IsNullOrEmpty(curMusicName)) {
                    index = 0;
                }
                else {
                    for(int i = 1; i < musicNameList.Count; i++) {
                        if(musicNameList[i] == curMusicName) {
                            index = i;
                            break;
                        }
                    }
                }

                //select
                index = EditorGUI.Popup(position, label.text, index, musicNameList.ToArray());
                if(index >= 1 && index < musicNameList.Count)
                    property.stringValue = musicNameList[index];
                else
                    property.stringValue = "";

                EditorGUI.EndProperty();
            }
            else
                EditorGUI.PropertyField(position, property, label);
        }
    }
}