using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    [SerializeField] Transform player;
    public SpriteRenderer playerRenderer;
    public int numOfIterations = 1000;
    [HideInInspector] public Vector2 plPosition;
    [SerializeField] Transform parEnemies;
    [SerializeField] TestRat ratPrefab;
    [SerializeField] int numOfRats;
    [HideInInspector] public TestRat[] rats;

    public Vector2[] vs;

    [Title("")]
    public Material mat;
    public bool matEnable;
    public string key;

    

    [Button]
    void Normalize()
    {
    }

    // public struct MyStruct
    // {
    //     public GenMove move;
    //
    //     public MyStruct(GenMove move)
    //     {
    //         this.move = move;
    //     }
    // }
    // void Metoda(params object[] args)
    // {
    //     for (int i = 0; i < args.Length; i++)
    //     {
    //         print(args[i]);
    //     }
    // }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawRay(player.position, vs[0]);
    //     Gizmos.color = Color.green;
    //     Gizmos.DrawRay(player.position, vs[1]);
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawRay(player.position, vs[2]);
    //     Gizmos.color = Color.black;
    //     Gizmos.DrawRay(player.position, vs[3]);
    // }
}
