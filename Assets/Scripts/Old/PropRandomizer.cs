using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropRandomizer : MonoBehaviour
{
    [SerializeField] Transform[] propSpawnPos;
    [SerializeField] Sprite[] propSprites;
    [SerializeField] GameObject propPrefab;
    
    void Start()
    {
        SpawnProps();
    }
    
    void SpawnProps()
    {
        for (int i = 0; i < propSpawnPos.Length; i++)
        {
            GameObject go = Instantiate(propPrefab, propSpawnPos[i].position, Quaternion.identity, propSpawnPos[i]);
            go.GetComponent<SpriteRenderer>().sprite = propSprites[Random.Range(0, propSprites.Length)];
        }
    }


}
