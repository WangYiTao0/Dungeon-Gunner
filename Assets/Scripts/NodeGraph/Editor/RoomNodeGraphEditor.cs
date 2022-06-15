using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle _roomNodeStyle;
    private static RoomNodeGraphSO _currentRoomNodeGraph;
    private RoomNodeSO _currentRoomNode = null;
    private RoomNodeTypeListSO _roomNodeTypeList;

    //node layout value
    private const float _nodeWidth = 160f;
    private const float _nodeHeight = 75f;
    private const int _nodePadding = 25;
    private const int _nodeBorder = 12;
    private const float _connectingLineWidth = 3f;
    private const float _connetingLineArrowSize = 6f;

    //エディターメニューの作成　pathを指定
    [MenuItem("Room Node Graph Editor", menuItem = "Dungeon/Dungeon Editor/Room Node Graph Editor")]
    private static void OpenWindow()
    {
        //エディターウィンドウの作成
         GetWindow<RoomNodeGraphEditor>("Room NodeGraph Editor");
    }

    /// <summary>
    /// RoomNodeGraphSo がダブルクリックされたら、エディターウィンドウを開く
    /// </summary>
    /// <param name="instanceID"></param>
    /// <param name="line"></param>
    /// <returns></returns>
    [OnOpenAsset(0)]
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        RoomNodeGraphSO roomNodeGraph = EditorUtility.InstanceIDToObject(instanceID) as RoomNodeGraphSO;

        if (roomNodeGraph != null)
        {
            OpenWindow();

            _currentRoomNodeGraph = roomNodeGraph;
            
            return true;
        }

        return false;
    }
    

    private void OnEnable()
    {
        //Define node Layout style レイアウト スタイルの定義
        _roomNodeStyle = new GUIStyle();
        _roomNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        _roomNodeStyle.normal.textColor = Color.white;
        _roomNodeStyle.padding = new RectOffset(_nodePadding, _nodePadding, _nodePadding, _nodePadding);
        _roomNodeStyle.border = new RectOffset(_nodeBorder, _nodeBorder, _nodeBorder, _nodeBorder);

        //Load Room Node Type
        _roomNodeTypeList = GameResources.Instance.RoomNodeTypeList;
    }

    /// <summary>
    /// エディターを描画する
    /// </summary>
    private void OnGUI()
    {
        // RoomNodeGraphSO が　選択されました
        if (_currentRoomNodeGraph != null)
        {
            //Drag Line if being dragged
            DrawDraggedLine();
            
            //process Event
            ProcessEvents(Event.current);
            
            //Draw Connects Between Room Node
            DrawRoomConnection();
            
            //Draw Room Node
            DrawRoomNode();
        }

        if (GUI.changed)
        {
            Repaint();
        }
        
    }

  
    private void DrawDraggedLine()
    {
        if (_currentRoomNodeGraph.LinePosition != Vector2.zero)
        {
            //Draw Line from node to line position
            Handles.DrawBezier(_currentRoomNodeGraph.RoomNodeToDrawLineFrom.rect.center,
                _currentRoomNodeGraph.LinePosition,
                _currentRoomNodeGraph.RoomNodeToDrawLineFrom.rect.center,
                _currentRoomNodeGraph.LinePosition,Color.white,null, _connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        //get room not that mouse is over if it is null or not currently being drag
        if (_currentRoomNode == null || _currentRoomNode.IsLeftClickDragging == false)
        {
            _currentRoomNode = IsMouseOverRoomNode(currentEvent);
        }

        //if mouse is not over a room 
        //or we are currently dragging aline from the room node then process graph event
        if (_currentRoomNode == null|| _currentRoomNodeGraph.RoomNodeToDrawLineFrom != null)
        {
            ProcessRoomNodeGraphEvents(currentEvent);
        }
        else
        {
            //process room node event
            _currentRoomNode.ProcessEvents(currentEvent);
        }
        

    }

    /// <summary>
    /// check mouse is over  a room node, 
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <returns></returns>
    private RoomNodeSO IsMouseOverRoomNode(Event currentEvent)
    {
        for (int i = _currentRoomNodeGraph.RoomNodeList.Count -1; i >= 0; i--)
        {
            if (_currentRoomNodeGraph.RoomNodeList[i].rect.Contains(currentEvent.mousePosition))
            {
                return _currentRoomNodeGraph.RoomNodeList[i];
            }
        }

        return null;
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //process mouse down event
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;
            
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;
            default:
                break;
        }
    }




    /// <summary>
    /// process mouse down events on the room node graph
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        //process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
    }
    /// <summary>
    /// processMouseDragEvent
    /// </summary>
    /// <param name="currentEvent"></param>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        //ProcessRightMouseDragEvent - draw line
        if (currentEvent.button == 1)
        {
            ProcessRightMouseDragEvent(currentEvent);
        }
    }
    /// <summary>
    /// ProcessRightMouseDragEvent - draw line
    /// </summary>
    /// <param name="currentEvent"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (_currentRoomNodeGraph.RoomNodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            
            GUI.changed = true;
        }
    }
    /// <summary>
    /// draw connecting line from room node
    /// </summary>
    /// <param name="delta"></param>
    private void DragConnectingLine(Vector2 delta)
    {
        _currentRoomNodeGraph.LinePosition += delta;
    }

    /// <summary>
    /// Show Context Menu
    /// </summary>
    /// <param name="mousePosition"></param>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Room Node"), false, CreateRoomNode, mousePosition);
        
        menu.ShowAsContext();
    }

    private void CreateRoomNode(object mousePositionObject)
    {
        CreateRoomNode(mousePositionObject,_roomNodeTypeList.list.Find(x=>x.IsNone));
    }

    /// <summary>
    /// Create Room Node at MousePosition
    /// </summary>
    /// <param name="mousePositionObject"></param>
    /// <param name="roomNodeTypeSo"></param>
    private void CreateRoomNode(object mousePositionObject,RoomNodeTypeSO roomNodeTypeSo)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        //create room node SO
        RoomNodeSO roomNodeSo = ScriptableObject.CreateInstance<RoomNodeSO>();
        
        // add room node to current room node graph
        _currentRoomNodeGraph.RoomNodeList.Add(roomNodeSo);

        //Set room node value
        roomNodeSo.Initialise(new Rect(mousePosition,new Vector2(_nodeWidth,_nodeHeight)),_currentRoomNodeGraph,roomNodeTypeSo);
        
        AssetDatabase.AddObjectToAsset(roomNodeSo,_currentRoomNodeGraph);
        AssetDatabase.SaveAssets();
        
        //refresh graph
        _currentRoomNodeGraph.OnValidate();
    }

    private void ProcessMouseUpEvent(Event currentEvent)
    {
        //if releasing the right mouse button and currently dragging a line
        if(currentEvent.button == 1 && _currentRoomNodeGraph.RoomNodeToDrawLineFrom != null)
        {
            //check if over a room node
            RoomNodeSO roomNode = IsMouseOverRoomNode(currentEvent);

            if (roomNode != null)
            {
                // if so set as a child 
                if (_currentRoomNodeGraph.RoomNodeToDrawLineFrom.AddChildRoomNodeIDToRoomNode(roomNode.ID))
                {
                    // set parent id in child node
                    roomNode.AddParentRoomNodeIDToRoomNode(_currentRoomNodeGraph.RoomNodeToDrawLineFrom.ID);
                }
            }
            ClearLineDrag();
        }
    }

    /// <summary>
    /// clear line drag from room node
    /// </summary>
    private void ClearLineDrag()
    {
        _currentRoomNodeGraph.RoomNodeToDrawLineFrom = null;
        _currentRoomNodeGraph.LinePosition = Vector2.zero;

        GUI.changed = true;
    } 
    
    private void DrawRoomConnection()
    {
        foreach (var roomNode in _currentRoomNodeGraph.RoomNodeList)
        {
            if (roomNode.ChildRoomNodeIDList.Count > 0)
            {
                foreach (var childRoomNodeID in roomNode.ChildRoomNodeIDList)
                {
                    DrawConnectionLine(roomNode, _currentRoomNodeGraph.RoomNodeDictionary[childRoomNodeID]);

                    GUI.changed = true;
                }
            }
        }
    }

    private void DrawConnectionLine(RoomNodeSO parentRoomNode, RoomNodeSO childRoomNode)
    {
        Vector2 startPosition = parentRoomNode.rect.center;
        Vector2 endPosition = childRoomNode.rect.center;

        Vector2 midPosition = (endPosition + startPosition) * 0.5f;
        Vector2 direction = endPosition - startPosition;
        
        //calculate arrow tail
        Vector2 arrowTailPoint1 =
            midPosition - new Vector2(-direction.y, direction.x).normalized * _connetingLineArrowSize;
        Vector2 arrowTailPoint2 =
            midPosition + new Vector2(-direction.y, direction.x).normalized * _connetingLineArrowSize;

        Vector2 arrowHeadPoint = midPosition + direction.normalized * _connetingLineArrowSize;
        
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1
            , arrowHeadPoint, arrowTailPoint1,
            Color.white, null, _connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2
            , arrowHeadPoint, arrowTailPoint2,
            Color.white, null, _connectingLineWidth);
        
        Handles.DrawBezier(startPosition, endPosition
            , startPosition, endPosition,
            Color.white, null, _connectingLineWidth);

        GUI.changed = true;

    }


    private void DrawRoomNode()
    {
        foreach (var roomNodeSo in _currentRoomNodeGraph.RoomNodeList)
        {
            roomNodeSo.Draw(_roomNodeStyle);
        }

        GUI.changed = true;
    }
}
