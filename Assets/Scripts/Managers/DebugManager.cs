using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Sirenix.OdinInspector;

public class DebugManager : MonoBehaviour
{
    bool _showSectorNumbers;
    [SerializeField] TextMeshPro sectorNameTagPrefab;
    TextMeshPro[] _sectorNames;
    
    GameManager _gm;
    int _gridBaseCount;
    int _spawnAreaSize;
    Vector2[] _cornersSpawn;
    Vector2[] _sectorCenters;
    
    Vector2[,] _debugGridCoordinates;

    void Awake()
    {
        _gm = GameManager.Instance;
    }

    [Button]
    void SectorNumbersToggle()
    {
        _showSectorNumbers = !_showSectorNumbers;
        for (int i = 0; i < _sectorNames.Length; i++)
        {
            Utils.Activation(_sectorNames[i].gameObject, _showSectorNumbers ? GenActivation.On : GenActivation.Off);
        }

    }
    public void InitializeMe(int gridBaseCount, int spawnAreaSize)
    {
        _gridBaseCount = gridBaseCount;
        _spawnAreaSize = spawnAreaSize;
        _sectorNames = new TextMeshPro[_gridBaseCount * _gridBaseCount];
        for (int i = 0; i < _sectorNames.Length; i++)
        {
            _sectorNames[i] = Instantiate(sectorNameTagPrefab, transform);
        }
    }

    public void RefreshMe(Vector2[,] debugGridCoordinates, Vector2[] cornersSpawn,  Vector2[] sectorCenters)
    {
        _debugGridCoordinates = debugGridCoordinates;
        _cornersSpawn = cornersSpawn;
        _sectorCenters = sectorCenters;
    }

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (!_gm.debug) return;
        
        Gizmos.color = Color.green;
        
        List<Vector3> verticals = new List<Vector3>();
        for (int i = 0; i < _gridBaseCount; i++)
        {
            verticals.Add(_debugGridCoordinates[i, 0]);
            Vector3 endPoint = _debugGridCoordinates[i, _gridBaseCount - 1] + ((float)_spawnAreaSize / _gridBaseCount) * Vector2.up;
            verticals.Add(endPoint);
        }
        Gizmos.DrawLineList(verticals.ToArray());
        
        List<Vector3> horizontals = new List<Vector3>();
        for (int j = 0; j < _gridBaseCount; j++)
        {
            horizontals.Add(_debugGridCoordinates[0, j]);
            Vector3 endPoint = _debugGridCoordinates[_gridBaseCount - 1, j] + ((float)_spawnAreaSize / _gridBaseCount) * Vector2.right;
            horizontals.Add(endPoint);
        }
        Gizmos.DrawLineList(horizontals.ToArray());
        
        Gizmos.color = Color.white;
        for (int i = 0; i < 4; i++)
        {
            Gizmos.DrawSphere(_cornersSpawn[i], 1f);
        }

        if (_showSectorNumbers)
        {
            for (int i = 0; i < _sectorNames.Length; i++)
            {
                _sectorNames[i].transform.position = _sectorCenters[i];
                _sectorNames[i].text = i.ToString();
            }
        }
    }
}
