using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomNodeGraph", menuName = "Scriptable Objects/Dungeon/Room Node Graph")]
public class RoomNodeGraphSO : ScriptableObject
{
    [HideInInspector] public RoomNodeTypeListSO RoomNodeTypeList;
    [HideInInspector] public List<RoomNodeSO> RoomNodeList = new List<RoomNodeSO>();
    [HideInInspector] public Dictionary<string, RoomNodeSO> RoomNodeDictionary = new Dictionary<string, RoomNodeSO>();

    private void Awake()
    {
        LoadRoomNodeDictionary();
    }

    /// <summary>
    /// Load the room node dictionary from the room node list
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    private void LoadRoomNodeDictionary()
    {
       RoomNodeDictionary.Clear();

       foreach (var node in RoomNodeList)
       {
           RoomNodeDictionary[node.ID] = node;
       }
    }

    /// <summary>
    /// GetRoomNode
    /// </summary>
    /// <param name="roomNodeID"></param>
    /// <returns> IDは存在だったら、return RoomNodeSO else return null </returns>
    public RoomNodeSO GetRoomNode(string roomNodeID)
    {
        if (RoomNodeDictionary.TryGetValue(roomNodeID, out RoomNodeSO roomNode))
        {
            return roomNode;
        }

        return null;
    }

#if UNITY_EDITOR

    [HideInInspector] public RoomNodeSO RoomNodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 LinePosition;

    // リジェネレート node dictionary
    public void OnValidate()
    {
        LoadRoomNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(RoomNodeSO node, Vector2 position)
    {
        RoomNodeToDrawLineFrom = node;
        LinePosition = position;
    }
    
#endif
}