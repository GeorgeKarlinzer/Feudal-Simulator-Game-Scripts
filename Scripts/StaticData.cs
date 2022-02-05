using UnityEngine;
using UnityEngine.UI;

public class StaticData : MonoBehaviour
{
    private static StaticData thisObject;

    [SerializeField] private AstarPath astarPath;
    [SerializeField] private BuildMenu buildMenu;
    [SerializeField] private RedactingGrid redactingGrid;
    [SerializeField] private SideMenu sideMenu;
    [SerializeField] private MapManager mapManager;
    [SerializeField] private PopUpManager popUp;
    [SerializeField] private AutoClosingSystem autoClosingSystem;
    [SerializeField] private UIManager uIManager;
    [SerializeField] private InfoPanel infoPanel;
    [SerializeField] private SoldiersSystem soldiersSystem;
    [SerializeField] private Forest forest;
    [SerializeField] private Button buildButton;
    [SerializeField] private Button rotateButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private SpritesLoader spritesLoader;

    public static AstarPath AstarPathStatic => thisObject.astarPath;
    public static BuildMenu BuildMenuStatic => thisObject.buildMenu;
    public static RedactingGrid RedactingGridStatic => thisObject.redactingGrid;
    public static SideMenu SideMenuStatic => thisObject.sideMenu;
    public static MapManager MapManagerStatic => thisObject.mapManager;
    public static PopUpManager PopUpStatic => thisObject.popUp;
    public static AutoClosingSystem AutoClosingSystemStatic => thisObject.autoClosingSystem;
    public static UIManager UIManagerStatic => thisObject.uIManager;
    public static InfoPanel InfoPanelStatic => thisObject.infoPanel;
    public static SoldiersSystem SoldiersSystemStatic => thisObject.soldiersSystem;
    public static Forest ForestStatic => thisObject.forest;
    public static Button BuildButtonStatic => thisObject.buildButton;
    public static Button RotateButtonStatic => thisObject.rotateButton;
    public static Button CancelButtonStatic => thisObject.cancelButton;
    public static Button MoveButtonStatic => thisObject.moveButton;
    public static SpritesLoader SpritesLoaderStatic => thisObject.spritesLoader;

    private void Awake()
        => thisObject = this;
}
