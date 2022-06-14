using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class RoomNodeGraphEditor : EditorWindow
{
    private GUIStyle _roomNodeStyle;
    private static RoomNodeGraphSO _currentRoomNodeGraph;
    private RoomNodeTypeListSO _roomNodeTypeList;

    //node layout value
    private const float _nodeWidth = 160;
    private const float _nodeHeight = 75;
    private const int _nodePadding = 25;
    private const int _nodeBorder = 12;

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
            //process Event
            ProcessEvents(Event.current);
            
            //Draw Room Node
            DrawRoomNode();
        }

        if (GUI.changed)
        {
            Repaint();
        }
        
    }
    
    

    private void ProcessEvents(Event currentEvent)
    {
        ProcessRoomNodeGraphEvents(currentEvent);
    }

    private void ProcessRoomNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            //process mouse down event
            case EventType.MouseDown:
                processMouseDownEvent(currentEvent);
                break;
            
            default:
                break;
        }
    }

    /// <summary>
    /// process mouse down events on the room node graph
    /// </summary>
    /// <param name="currentEvent"></param>
    private void processMouseDownEvent(Event currentEvent)
    {
        //process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1)
        {
            ShowContextMenu(currentEvent.mousePosition);
        }
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
