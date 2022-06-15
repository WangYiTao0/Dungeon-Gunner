using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "RoomNodeSO", menuName = "Scriptable Objects/Dungeon/RoomNodeSO", order = 0)]
public class RoomNodeSO : ScriptableObject
{
         public string ID;
         public List<string> ParentRoomNodeIDList = new List<string>();
         public List<string> ChildRoomNodeIDList = new List<string>();
        [HideInInspector] public RoomNodeGraphSO RoomNodeGraph;
        public RoomNodeTypeSO RoomNodeType;
        [HideInInspector] public RoomNodeTypeListSO RoomNodeTypeList;

        #region Editor Code
        
#if UNITY_EDITOR
        [HideInInspector] public Rect rect;
        [HideInInspector] public bool IsLeftClickDragging = false;
        [HideInInspector] public bool IsSelected = false;
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

                // if the room node has a parent node or is of type entrance then display a label else display a popup
                if (ParentRoomNodeIDList.Count > 0 || RoomNodeType.IsEntrance)
                {
                        //Display label that cant be change
                        EditorGUILayout.LabelField(RoomNodeType.RoomNodeTypeName);
                }
                else
                {
                        int selected = RoomNodeTypeList.list.FindIndex(x => x == RoomNodeType);

                        int selection = EditorGUILayout.Popup("", selected, GetRoomNodeTypesToDisplay());

                        RoomNodeType = RoomNodeTypeList.list[selection];
                }

                
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
        
        
        public void ProcessEvents(Event currentEvent)
        {
                switch (currentEvent.type)      
                {
                        case EventType.MouseDown:
                                PocessMouseDownEvent(currentEvent);
                                break;
                        case EventType.MouseUp:
                                ProcessMouseUpEvent(currentEvent);
                                break;
                        case EventType.MouseDrag:
                                ProcessMouseDragEvent(currentEvent);
                                break;
                        default:
                                break;
                }
        }
        private void PocessMouseDownEvent(Event currentEvent)
        {
                if (currentEvent.button == 0)
                {
                        ProcessLeftClickDownEvent();
                }
                
                if (currentEvent.button == 1)
                {
                        ProcessRightClickDownEvent(currentEvent);
                }
        }
        
        private void ProcessLeftClickDownEvent()
        {
               Selection.activeObject = this;

               IsSelected = !IsSelected;
        }
        
        private void ProcessRightClickDownEvent(Event currentEvent)
        {
                RoomNodeGraph.SetNodeToDrawConnectionLineFrom(this,currentEvent.mousePosition);
        }

        private void ProcessMouseUpEvent(Event currentEvent)
        {
                if (currentEvent.button == 0)
                {
                        ProcessLeftClickUpEvent();
                }
        }

        private void ProcessLeftClickUpEvent()
        {
                if (IsLeftClickDragging)
                {
                        IsLeftClickDragging = false;
                }
        }

        private void ProcessMouseDragEvent(Event currentEvent)
        {
                if (currentEvent.button == 0)
                {
                        ProcessLeftClickDragEvent(currentEvent);
                }
        }

        private void ProcessLeftClickDragEvent(Event currentEvent)
        {
                IsLeftClickDragging = true;
                DragNode(currentEvent.delta);
                GUI.changed = true;
        }

        private void DragNode(Vector2 currentEventDelta)
        {
                rect.position += currentEventDelta;
                
                EditorUtility.SetDirty(this);
        }

        /// <summary>
        /// add child ID to the Node( return true if the node has been added, false otherwise)
        /// </summary>
        /// <param name="childID"></param>
        /// <returns></returns>
        public bool AddChildRoomNodeIDToRoomNode(string childID)
        {
               ChildRoomNodeIDList.Add(childID);
               return true;
        }
        
        public bool AddParentRoomNodeIDToRoomNode(string parentID)
        {
                ParentRoomNodeIDList.Add(parentID);
                return true;
        }

#endif


        #endregion

 
}