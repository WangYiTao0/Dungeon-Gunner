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

                        //if room type selection has changed making child connections protentially invalid
                        if (RoomNodeTypeList.list[selected].IsCorridor && !RoomNodeTypeList.list[selection].IsCorridor
                            || !RoomNodeTypeList.list[selected].IsCorridor &&
                            RoomNodeTypeList.list[selection].IsCorridor
                            || !RoomNodeTypeList.list[selected].IsBossRoom &&
                            RoomNodeTypeList.list[selection].IsBossRoom)
                        {
                                if (ChildRoomNodeIDList.Count > 0)
                                {
                                        for (int i = ChildRoomNodeIDList.Count - 1; i >= 0; i--)
                                        {
                                                //get child room node
                                                RoomNodeSO childRoomNode = RoomNodeGraph.GetRoomNode(ChildRoomNodeIDList[i]);

                                                //if child room node is selected
                                                if (childRoomNode != null)
                                                {
                                                        //remove childId From parent room node
                                                        RemoveChildRoomNodeIDFromRoomNode(childRoomNode.ID);
                        
                                                        //remove parentID from child room node
                                                        childRoomNode.RemoveParentRoomNodeIDFromRoomNode(ID);
                                                }
                                        }
                                }
                        }
                        
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
                //Check child node can be added validly to Parent
                if (IsChildRoomValid(childID))
                {
                        ChildRoomNodeIDList.Add(childID); 
                        return true; 
                }

                return false;
        }

        /// <summary>
        /// remove child Id from node
        /// </summary>
        /// <param name="childID"></param>
        /// <returns></returns>
        public bool RemoveChildRoomNodeIDFromRoomNode(string childID)
        {
                if (ChildRoomNodeIDList.Contains(childID))
                {
                        ChildRoomNodeIDList.Remove(childID);
                        return true;
                }
                return false;
        }
        
        public bool AddParentRoomNodeIDToRoomNode(string parentID)
        {
                ParentRoomNodeIDList.Add(parentID);
                return true;
        }
        
        public bool RemoveParentRoomNodeIDFromRoomNode(string parentID)
        {
                if (ParentRoomNodeIDList.Contains(parentID))
                {
                        ParentRoomNodeIDList.Remove(parentID);
                        return true;
                }
                return false;
        }

        /// <summary>
        /// Check child node can be added validly to Parent
        /// </summary>
        /// <param name="childID"></param>
        /// <returns>return true if it can otherwise return false</returns>
        private bool IsChildRoomValid(string childID)
        {
                bool isConnectedBossNodeAlready = false;
                foreach (var roomNode in RoomNodeGraph.RoomNodeList)
                {
                        if (roomNode.RoomNodeType.IsBossRoom && roomNode.ParentRoomNodeIDList.Count > 0)
                        {
                                isConnectedBossNodeAlready = true;
                        }
                }

                //BossRoomは1つだけです。
                if (RoomNodeGraph.GetRoomNode(childID).RoomNodeType.IsBossRoom && isConnectedBossNodeAlready)
                {
                        return false;
                }
                // Can not Connect IsNone
                if (RoomNodeGraph.GetRoomNode(childID).RoomNodeType.IsNone)
                {
                        return false;
                }

                // 重複connection を　防止する
                if (ChildRoomNodeIDList.Contains(childID))
                {
                        return false;
                }

                //自分と自分のconnection を 防止する
                if (ID == childID)
                {
                        return false;
                }

                //childID 元々はParentです。逆Connection　を防止する 
                if (ParentRoomNodeIDList.Contains(childID))
                {
                        return false;
                }

                // every node should only have one parent
                if (RoomNodeGraph.GetRoomNode(childID).ParentRoomNodeIDList.Count > 0)
                {
                        return false;
                }

                // Corridor と　IsCorridor のConnection　を防止する 
                if (RoomNodeGraph.GetRoomNode(childID).RoomNodeType.IsCorridor && RoomNodeType.IsCorridor)
                {
                        return false;
                }
                
                // room と　room のConnection　を防止する 
                if (!RoomNodeGraph.GetRoomNode(childID).RoomNodeType.IsCorridor && !RoomNodeType.IsCorridor)
                {
                        return false;
                }

                // check max Corridor num
                if (RoomNodeGraph.GetRoomNode(childID).RoomNodeType.IsCorridor &&
                    ChildRoomNodeIDList.Count >= Settings.MaxChildCorridor)
                {
                        return false;
                }

                if (RoomNodeGraph.GetRoomNode(childID).RoomNodeType.IsEntrance)
                {
                        return false;
                }
                
                //
                if (!RoomNodeGraph.GetRoomNode(childID).RoomNodeType.IsCorridor && ChildRoomNodeIDList.Count > 0)
                {
                        return false;
                }
                
                return true;
        }



#endif


        #endregion

 
}