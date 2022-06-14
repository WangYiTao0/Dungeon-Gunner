using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "RoomNodeSO", menuName = "Scriptable Objects/Dungeon/RoomNodeSO", order = 0)]
public class RoomNodeSO : ScriptableObject
{
        [HideInInspector] public string ID;
        [HideInInspector] public List<string> ParentRoomNodeIDList = new List<string>();
        [HideInInspector] public List<string> ChildRoomNodeIDList = new List<string>();
        [HideInInspector] public RoomNodeGraphSO RoomNodeGraph;
        public RoomNodeTypeSO RoomNodeType;
        [HideInInspector] public RoomNodeTypeListSO RoomNodeTypeList;

        #region Editor Code
        
#if UNITY_EDITOR
        [HideInInspector] public Rect rect;
        
        public void Initialise(Rect rect, RoomNodeGraphSO currentRoomNodeGraph, RoomNodeTypeSO roomNodeTypeSo)
        {
                this.rect =   rect;
                this.ID = Guid.NewGuid().ToString();
                this.name = "RoomNode";
                this.RoomNodeGraph = currentRoomNodeGraph;
                this.RoomNodeType = roomNodeTypeSo;

                RoomNodeTypeList = GameResources.Instance.RoomNodeTypeList;
        }

      
        public void Draw(GUIStyle roomNodeStyle)
        {
                GUILayout.BeginArea(rect,roomNodeStyle);
                // start region to Detect popup selection changes
                EditorGUI.BeginChangeCheck();

                int selected = RoomNodeTypeList.list.FindIndex(x => x == RoomNodeType);

                int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

                RoomNodeType = RoomNodeTypeList.list[selection];
                
                if(EditorGUI.EndChangeCheck())
                        EditorUtility.SetDirty(this);
                
                GUILayout.EndArea();
        }

        private string[] GetRoomNodeTypesToDisplay()
        {
                string[] roomArray = new string[RoomNodeTypeList.list.Count];

                for (int i = 0; i < RoomNodeTypeList.list.Count; i++)
                {
                        if (RoomNodeTypeList.list[i].DisplayerInNodeGraphEditor)
                        {
                                roomArray[i] = RoomNodeTypeList.list[i].RoomNodeTypeName;
                        }
                        
                }

                return roomArray;
        }
#endif


        #endregion

}