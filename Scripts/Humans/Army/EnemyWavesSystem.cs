using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static StaticData;
using KdTree;
using KdTree.Math;

/// <summary>
/// Управляет логикой самих волн
/// </summary>
public class EnemyWavesSystem : MonoBehaviour
{
    [SerializeField] private List<SoldierBase> enemy;
    List<GroupBase> groups = new List<GroupBase>();

    private void Start()
    {
        Invoke("StartWave", 2);
    }

    private void StartWave()
    {
        SpawnGroup(new Vector3(30, 10, 0), new GroupInfo(1));
        //SpawnGroup(new Vector3(28, 10, 0), new GroupParameters(5));
    }

    public void CreateEnemies(int amount, Vector3 pos)
    {
        SpawnGroup(pos, new GroupInfo(amount));
    }

    private void SpawnGroup(Vector2 pos, GroupInfo parameters)
    {
        var g = new EnemyGroup();

        for (int i = 0; i < parameters.counts.Count; i++)
            for (int j = 0; j < parameters.counts[i]; j++)
            {
                var e = Instantiate(enemy[i], pos, Quaternion.identity);
                //e.Damage = 0;
                g.AddSoldier(e);
            }
    }

    private Vector2 FindStartGrid()
    {
        var fc = MapManagerStatic.firstCoord;
        var lc = MapManagerStatic.lastCoord;



        return Vector2.zero;

    }

    private bool CheckGrid(Vector2 grid)
    {


        return false;
    }

    struct GroupInfo
    {
        public List<int> counts;
        
        public GroupInfo(params int[] c)
        {
            counts = new List<int>();
            for (int i = 0; i < c.Length; i++)
                counts.Add(c[i]);
        }
    }
}
