using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapManager : SerializedMonoBehaviour
{
    GameManager _gm;
    [SerializeField] Transform[] terrains;
    [SerializeField] Vector2Int terrainChunkSize = new Vector2Int(8, 4);
    Vector2 _distVector;
    Dictionary<GenDirection, Transform> _distributedTerrains = new Dictionary<GenDirection, Transform>();
    void Awake()
    {
        _gm = GameManager.Instance;
    }

    void Start()
    {
        UpdateDictionary();
    }
    
    void CalculateDistance()
    {
        _distVector = _gm.pc.myTransform.position - _distributedTerrains[GenDirection.Center].position;

        if (Mathf.Abs(_distVector.x)  > terrainChunkSize.x * 0.6f)
        {
            switch (_distVector.x)
            {
                case > 0f:
                    _distributedTerrains[GenDirection.LeftUp].position = _distributedTerrains[GenDirection.RightUp].position + terrainChunkSize.x * Vector3.right;
                    _distributedTerrains[GenDirection.Left].position = _distributedTerrains[GenDirection.Right].position + terrainChunkSize.x * Vector3.right;
                    _distributedTerrains[GenDirection.LeftDown].position = _distributedTerrains[GenDirection.RightDown].position + terrainChunkSize.x * Vector3.right;
                    UpdateDictionary();
                    print("moved right");
                    break;
                case < 0f:
                    _distributedTerrains[GenDirection.RightUp].position = _distributedTerrains[GenDirection.LeftUp].position + terrainChunkSize.x * Vector3.left;
                    _distributedTerrains[GenDirection.Right].position = _distributedTerrains[GenDirection.Left].position + terrainChunkSize.x * Vector3.left;
                    _distributedTerrains[GenDirection.RightDown].position = _distributedTerrains[GenDirection.LeftDown].position + terrainChunkSize.x * Vector3.left;
                    UpdateDictionary();
                    print("moved left");
                    break;
            }
        }
        if (Mathf.Abs(_distVector.y)  > terrainChunkSize.y * 0.6f)
        {
            switch (_distVector.y)
            {
                case > 0f:
                    _distributedTerrains[GenDirection.LeftDown].position = _distributedTerrains[GenDirection.LeftUp].position + terrainChunkSize.y * Vector3.up;
                    _distributedTerrains[GenDirection.Down].position = _distributedTerrains[GenDirection.Up].position + terrainChunkSize.y * Vector3.up;
                    _distributedTerrains[GenDirection.RightDown].position = _distributedTerrains[GenDirection.RightUp].position + terrainChunkSize.y * Vector3.up;
                    UpdateDictionary();
                    print("moved up");
                    break;
                case < 0f:
                    _distributedTerrains[GenDirection.LeftUp].position = _distributedTerrains[GenDirection.LeftDown].position + terrainChunkSize.y * Vector3.down;
                    _distributedTerrains[GenDirection.Up].position = _distributedTerrains[GenDirection.Down].position + terrainChunkSize.y * Vector3.down;
                    _distributedTerrains[GenDirection.RightUp].position = _distributedTerrains[GenDirection.RightDown].position + terrainChunkSize.y * Vector3.down;
                    UpdateDictionary();
                    print("moved down");
                    break;
            }
        }
    }

    void UpdateDictionary()
    {
        _distributedTerrains.Clear();
        List<Transform> list = terrains.OrderBy(n => n.position.x).ThenBy(n => n.position.y).ToList();
        for (int i = 0; i < list.Count; i++)
        {
            _distributedTerrains.Add((GenDirection)i, list[i]);
        }
    }

    void Update()
    {
        CalculateDistance();
    }

    
    
}
