using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : SerializedMonoBehaviour
{
    [SerializeField] TextMeshProUGUI enemyCountText;
    [SerializeField] int numOfEnemies = 100;
    public static GameManager Instance;
    public SoGameData gameData;
    public PoolManager poolManager;
    public Camera mainCam;
    public PlayerControl pc;
    [HideInInspector] public Vector2 playerPosition;
    public DebugManager debugManager;
    
    [SerializeField] int gridBaseCount;
    int _spawnAreaSize;
    float _screenWidth;
    float _screenHeight;
    float _cellSize;
    Vector2[] _cornersScreen = new Vector2[4];
    Vector2[] _cornersSpawn = new Vector2[4];
    Vector2[,] _debugGridCoordinates;

    public Material spriteDefault, spriteFlash;
    [HideInInspector] public SoEnemy[] enemyScriptables;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] Transform parEnemies;
    List<Enemy> _allEnemies = new List<Enemy>();
    int _batch;
    Vector2Int _currentSpan;
    
    [HideInInspector] public SectorGroup[] sectors;
    Vector2 _mainCenter;
    Vector2[] _sectorCenters;
    [HideInInspector]  public HashSet<int> sectorsOnScreens = new HashSet<int>();

    #region EDITOR

    [Title("Editor operations")]
    public bool debug;
    public bool useCustomMoveSpeed;
    [ShowIf(nameof(useCustomMoveSpeed))]
    public float customMoveSpeed;
    #endregion

    #region MONO
    
    void Awake()
    {
        Instance = this;
        enemyScriptables = Resources.LoadAll<SoEnemy>("Enemies");
    }
    void Start()
    {
        _screenHeight = mainCam.orthographicSize * 2;
        _screenWidth = _screenHeight * mainCam.aspect;
        float spawnArea = _screenWidth * 2;
        _spawnAreaSize = (int)spawnArea;
        _cellSize = spawnArea / gridBaseCount;
        
        sectors = new SectorGroup[gridBaseCount * gridBaseCount];
        for (int i = 0; i < sectors.Length; i++)
        {
            sectors[i] = new SectorGroup();
        }

        pc.GetMySectorFromGameManager(_cellSize * 0.5f * Vector2.one);
        GridGeneration(pc.myTransform.position);
        
        GetScreenSectors();
        SpawnEnemies();
        
        _batch = _allEnemies.Count / 50;
        _currentSpan = new Vector2Int(0, _batch);
        
        debugManager.InitializeMe(gridBaseCount, _spawnAreaSize);
        
        void GetScreenSectors()
        {
            _cornersScreen = ScreenCorners(pc.myTransform.position);
            for (int i = 0; i < _sectorCenters.Length; i++)
            {
                if (IsOnScreen(_sectorCenters[i].x, _sectorCenters[i].y, (int)_cellSize)) sectorsOnScreens.Add(i);
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) SceneManager.LoadScene("MainMenu");

        if (pc.myTransform.hasChanged)
        {
            pc.myTransform.hasChanged = false;
            GridGeneration(pc.myTransform.position);
        }
        playerPosition = pc.myTransform.position;
        
        foreach (Enemy en in _allEnemies)
        {
            en.SegmentedUpdateLoop();
            en.UpdateLoop();
        }

    }




    #endregion

    #region SECTORS
    
    void GridGeneration(Vector2 centerPoint)
    {
        _mainCenter = centerPoint;
        _sectorCenters = new Vector2[gridBaseCount * gridBaseCount];
        int centerIndex = 0;
        
        _debugGridCoordinates = new Vector2[gridBaseCount, gridBaseCount];
        
        _cornersSpawn[0] = new Vector2(_mainCenter.x - _spawnAreaSize * 0.5f, _mainCenter.y - _spawnAreaSize * 0.5f);
        _cornersSpawn[1] = new Vector2(_mainCenter.x - _spawnAreaSize * 0.5f, _mainCenter.y + _spawnAreaSize * 0.5f);
        _cornersSpawn[2] = new Vector2(_mainCenter.x + _spawnAreaSize * 0.5f, _mainCenter.y + _spawnAreaSize * 0.5f);
        _cornersSpawn[3] = new Vector2(_mainCenter.x + _spawnAreaSize * 0.5f, _mainCenter.y - _spawnAreaSize * 0.5f);
        float startX = _cornersSpawn[0].x;
        float startY = _cornersSpawn[0].y;
        for (int j = 0; j < gridBaseCount; j++)
        {
            for (int i = 0; i < gridBaseCount; i++)
            {
                float x = startX + i * _cellSize;
                float y = startY + j * _cellSize;
                _sectorCenters[centerIndex] = new Vector2(x + _cellSize * 0.5f, y + _cellSize * 0.5f);
                centerIndex++;
                
                _debugGridCoordinates[i, j] = new Vector2(x, y);
            }
        }
        if (debug) debugManager.RefreshMe(_debugGridCoordinates, _cornersSpawn, _sectorCenters);

    }
    public int GetSector(Vector2 enPos)
    {
        enPos -= _mainCenter;
        float adjustedX = enPos.x + _spawnAreaSize * 0.5f;
        float adjustedY = enPos.y + _spawnAreaSize * 0.5f;
        
        int x = (int)(adjustedX / _cellSize);
        int y = (int)(adjustedY / _cellSize);
        
        return x + y * gridBaseCount;
    }
    public HashSet<SectorObject> NeighbouringObjects(int sectorIndex, string message = "")
    {
        HashSet<int> indexes = NeighbouringSectors();
        HashSet<SectorObject> hs = new HashSet<SectorObject>();
        foreach (int itemInt in indexes)
        {
            foreach (SectorObject item in sectors[itemInt].group)
            {
                hs.Add(item);
            }
        }
        return hs;

        HashSet<int> NeighbouringSectors()
        {
            HashSet<int> hsIndex = new HashSet<int>();
            int right = sectorIndex + 1;
            int left = sectorIndex - 1;
            int up = sectorIndex + gridBaseCount;
            int down = sectorIndex - gridBaseCount;
            int leftDown = sectorIndex - (gridBaseCount + 1);
            int leftUp = sectorIndex + (gridBaseCount - 1);
            int rightUp = sectorIndex + (gridBaseCount + 1);
            int rightDown = sectorIndex - (gridBaseCount - 1);

            if (sectorIndex % gridBaseCount == gridBaseCount - 1) //right edge
            {
                //  print("right");
                right = rightDown = rightUp = -1;
            }
            if (sectorIndex % gridBaseCount == 0) //left edge
            {
                //  print("left");
                left = leftDown = leftUp = -1;
            }
            if (sectorIndex < gridBaseCount) //down edge
            {
                // print("down");
                down = leftDown = rightDown = -1;
            }
            if (sectorIndex > gridBaseCount * gridBaseCount - (gridBaseCount + 1)) //up edge
            {
                //  print("up");
                up = leftUp = rightUp = -1;
            }

            AddValue(sectorIndex);
            AddValue(right);
            AddValue(left);
            AddValue(up);
            AddValue(down);
            AddValue(leftDown);
            AddValue(leftUp);
            AddValue(rightUp);
            AddValue(rightDown);

            return hsIndex;

            void AddValue(int neighbourIndex)
            {
                if (neighbourIndex >= 0 && sectorsOnScreens.Contains(neighbourIndex)) hsIndex.Add(neighbourIndex);
            }
        }
    }
    #endregion

    #region ENEMIES
    
    void SpawnEnemies()
    {
        if (!Application.isEditor) numOfEnemies = PlayerPrefs.GetInt("num");

        for (int i = 0; i < numOfEnemies; i++)
        {
            Vector3 enPos = RdnSpawnPosition();
            Enemy enemy = poolManager.GetSectorObject<Enemy>();
            enemy.myTransform.position = enPos;
            enemy.myData = enemyScriptables[Random.Range(0, enemyScriptables.Length)];
            enemy.sector = GetSector(enPos);
            Utils.Activation(enemy.myGameObject, GenActivation.On);
            SectorObjectsCommand(enemy, Commands.AddToGroup);
        }

        Vector3 RdnSpawnPosition()
        {
            Vector2 direction = Utils.RotateV2(Vector2.up, Random.Range(0, 360));
            float dist = Random.Range(_screenWidth * 0.6f, _screenWidth);
            return  dist * direction;
          //  return Random.insideUnitCircle * 10f;
        }

    }

    public void SectorObjectsCommand(SectorObject sectorObj, Commands commands)
    {
        switch (commands)
        {
            case Commands.AddToGroup:
                if (sectorObj is Enemy en)
                {
                    if (!_allEnemies.Contains(en)) _allEnemies.Add(en);
                }
                sectors[sectorObj.sector].group.Add(sectorObj);
                break;
            case Commands.RemoveFromGroup:
                Remove();
                break;
        }

        _batch = _allEnemies.Count / 50;
        enemyCountText.text = $"Enemies: {_allEnemies.Count}";
        
        void Remove()
        {
            if (sectorObj is Enemy en)
            {
                if (_allEnemies.Contains(en)) _allEnemies.Remove(en);
            }
            if (sectors[sectorObj.sector].group.Contains(sectorObj)) sectors[sectorObj.sector].group.Remove(sectorObj);
        }
    }
    #endregion

    #region HELPERS
    
    bool IsOnScreen(float posX, float posY, int offset = 0)
    {
        return posX >= _cornersScreen[0].x - offset &&
               posX <= _cornersScreen[3].x + offset &&
               posY >= _cornersScreen[0].y - offset &&
               posY <= _cornersScreen[1].y + offset;
    }

    Vector2[] ScreenCorners(Vector3 screenCenter)
    {
        Vector2[] v2 = new Vector2[4];

        float left = screenCenter.x - _screenWidth / 2;
        float right = screenCenter.x + _screenWidth / 2;
        float top = screenCenter.y + _screenHeight / 2;
        float bottom = screenCenter.y - _screenHeight / 2;

        v2[0] = new Vector2(left, bottom);
        v2[1] = new Vector2(left, top);
        v2[2] = new Vector2(right, top);
        v2[3] = new Vector2(right, bottom);

        return v2;
    }
    #endregion


}




// void FixedUpdate()
// {
//     bool hasGoneOver = false;
//     _currentSpan.x += _batch;
//     _currentSpan.y += _batch;
//     if (_currentSpan.x > _allEnemies.Count - 1)
//     {
//         _currentSpan = new Vector2Int(0, _batch);
//     }
//     else if (_currentSpan.y > _allEnemies.Count)
//     {
//         hasGoneOver = true;
//         _currentSpan = new Vector2Int(_currentSpan.x, _allEnemies.Count);
//     }
//     for (int i = 0; i < _allEnemies.Count; i++)
//     {
//         Enemy en = _allEnemies[i];
//         if (i >= _currentSpan.x && i < _currentSpan.y)
//         {
//             en.SegmentedUpdateLoop();
//         }
//         en.UpdateLoop();
//     }
//     if (hasGoneOver) _currentSpan = new Vector2Int(0, _batch);
// }

