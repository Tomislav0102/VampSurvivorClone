using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public class ObstacleGenerator : MonoBehaviour
{
    [SerializeField] BoxCollider2D boxCollider;
    [SerializeField] GameObject obstaclePrefab;
    [Range(1, 100)] [SerializeField] int width = 1;
    [Range(1, 100)] [SerializeField] int height = 1;

    [Button]
    void Generate()
    {
        Destroy();
#if (UNITY_EDITOR)

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(obstaclePrefab, transform);
                go.transform.localPosition = new Vector3(j, i, 0);

            }
        }
        
        boxCollider.size = new Vector2(width, height);
        boxCollider.offset = new Vector2((float)(width -1) / 2, (float)(height - 1) / 2);
        boxCollider.enabled = true;
#endif

    }

    [Button]
    void Destroy()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(transform.childCount - 1).gameObject);
        }
        boxCollider.size = Vector2.one;
        boxCollider.offset = Vector2.zero;
        boxCollider.enabled = false;
    }

    void Awake()
    {
        if (transform.childCount == 0) Utils.DestroyGo(gameObject);
    }
}
