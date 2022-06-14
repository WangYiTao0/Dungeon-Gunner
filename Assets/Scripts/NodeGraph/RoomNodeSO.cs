using System;
using System.Collections.Generic;
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

 
}