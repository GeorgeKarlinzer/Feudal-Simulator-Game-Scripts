using System.Collections;
using UnityEngine;
using static StaticData;

public class MapGenerator : MonoBehaviour
{
    public AstarPath astarPath;
    public bool scan;

    public FieldGenerator fieldGenerator;
    public bool field;
    public DirtGeneration dirtGeneration;
    public bool dirt;
    public RiverGeneration riverGeneration;
    public bool river;
    public MountainGenerator mountainGenerator;
    public bool mountain;
    public HillGenerator hillGenerator;
    public bool hill;
    public MineMountainGenerator mineMountainGenerator;
    public bool mine;
    public ForestGenerator forestGenerator;
    public bool forest;
    public BorderGenerator borderGenerator;
    public bool border;

    public bool obstacles;

    private void Start()
    {
        Generate();
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            ReGenerate();
    }
#endif
    private void ReGenerate()
    {
        fieldGenerator.Clear();
        dirtGeneration.Clear();
        riverGeneration.Clear();
        mountainGenerator.Clear();
        hillGenerator.Clear();
        forestGenerator.Clear();
        mineMountainGenerator.Clear();
        borderGenerator.Clear();
        MapManagerStatic.ClearMap();

        Generate();
    }

    private void Generate()
    {
        if (field) fieldGenerator.Generate();
        if (river && obstacles) riverGeneration.Generate();
        if (mountain && obstacles) mountainGenerator.Generate();
        if (hill && obstacles) hillGenerator.Generate();
        if (mountain && obstacles) mountainGenerator.GenerateSingle();
        if (dirt) dirtGeneration.Generate();
        if (forest && obstacles) forestGenerator.Generate();
        if (mine && obstacles) mineMountainGenerator.Generate();
        if (border) borderGenerator.Generate();

        if (scan) StartCoroutine(Scan());
    }

    IEnumerator Scan()
    {
        yield return new WaitForEndOfFrame();

        MapManager mapManager = MapManagerStatic;
        Vector2Int size = mapManager.Size;

        float nodeSize = 0.1f;
        int nodesPerUnit = 10;

        AstarPath.active.data.gridGraph.SetDimensions(size.x * nodesPerUnit, size.y * nodesPerUnit, nodeSize);

        Vector3 pos = new Vector3(mapManager.lastCoord.x - mapManager.firstCoord.x, mapManager.lastCoord.y - mapManager.firstCoord.y, 0) / 2;
        pos += new Vector3(mapManager.firstCoord.x - 0.5f, mapManager.firstCoord.y - 0.5f, 0);
        AstarPath.active.data.gridGraph.center = pos;

        astarPath.Scan();
    }
}
