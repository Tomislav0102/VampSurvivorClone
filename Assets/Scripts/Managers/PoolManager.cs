using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using UnityEditor;

public class PoolManager : SerializedMonoBehaviour
{
    [SerializeField] Transform parFloatDamage;
    TextMeshPro[] _floatingDamageTexts;
    int _floatingDamageCounter;
    [SerializeField] Transform parEnemies;
    Enemy[] _enemies;
    int _enemiesCounter;
    [SerializeField] Transform parHits;
    HitObject[] _hits;
    int _hitsCounter;
    [SerializeField] Transform parW_Simple;
    Dictionary<WeaponType, WeaponClass> _dicWeapons = new Dictionary<WeaponType, WeaponClass>();

    [Title("Editor operations")]
    public Transform parPool;
    public GameObject prefabPoolItem;
    public int numOfItemsInPool;

    [Button][HorizontalGroup]
    void GeneratePool()
    {
#if (UNITY_EDITOR)
        
        for (int i = 0; i < numOfItemsInPool; i++)
        {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefabPoolItem, parPool);
            go.name += $" {i}";
        }
#endif

    }

    [Button][HorizontalGroup]
    void ClearPool()
    {
        while (parPool.childCount > 0)
        {
            for (int i = 0; i < parPool.childCount; i++)
            {
                DestroyImmediate(parPool.GetChild(i).gameObject);
            }
        }
    }

    void Awake()
    {
        _floatingDamageTexts = Utils.AllChildren<TextMeshPro>(parFloatDamage);
        _enemies = Utils.AllChildren<Enemy>(parEnemies);
        _hits = Utils.AllChildren<HitObject>(parHits);
    }

    #region FLOATS
    public void GetFloatDamage(Vector2 pos, int val)
    {
         TextMeshPro textMeshPro = _floatingDamageTexts[_floatingDamageCounter];
         Transform targetTransform = textMeshPro.transform;
         textMeshPro.text = val.ToString();
         targetTransform.position = pos;
         textMeshPro.enabled = true;
         StartCoroutine(EndFloatDamage(textMeshPro, targetTransform));
         _floatingDamageCounter = (1 + _floatingDamageCounter) % _floatingDamageTexts.Length;
    }

    IEnumerator EndFloatDamage(TextMeshPro target, Transform targetTransform)
    {
        float timer = 0;
        while (timer < 2f)
        {
            timer += Time.deltaTime;
            targetTransform.position +=  2f * Time.deltaTime * Vector3.up;
            yield return null;
        }
        target.enabled = false;
    }
    #endregion

    public T GetSectorObject<T>() where T : SectorObject
    {
        if (typeof(T) == typeof(Enemy))
        {
            _enemiesCounter = (1 + _enemiesCounter) % _enemies.Length;
            return (T)(object)_enemies[_enemiesCounter];
        }
        if (typeof(T) == typeof(HitObject))
        {
            _hitsCounter = (1 + _hitsCounter) % _hits.Length;
            return (T)(object)_hits[_hitsCounter];
        }

        return null;
    }

    public Weapon GetWeapon(WeaponType weaponType)
    {
        WeaponClass wc = _dicWeapons[weaponType];
        wc.counter = (1 + wc.counter) % wc.weapons.Length;
        return wc.weapons[wc.counter];
    }

    class WeaponClass
    {
        public Weapon[] weapons;
        public int counter;

        public WeaponClass(Transform parent)
        {
            weapons = Utils.AllChildren<Weapon>(parent);
        }
    }
}
