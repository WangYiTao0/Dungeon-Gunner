using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RoomNodeTypeListSO", menuName = "Scriptable Objects/Dungeon/RoomNodeTypeListSO", order = 0)]
public class RoomNodeTypeListSO : ScriptableObject
{
        [Space(10)]
        [Header("Room Node Type List")]
        [Tooltip("このListはゲームの中にすべてのRoomNodeTypeSO,  enum の代わりです" )]
        public List<RoomNodeTypeSO> RoomNodeTypeSoList;
        
        
        #if UNITY_EDITOR

        private void OnValidate()
        {
                HelpUtility.ValidateCheckEnumerableValue(this, nameof(RoomNodeTypeSoList), RoomNodeTypeSoList);
        }
#endif
}