using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class Enemy : SectorObject
{
    SoEnemy _myData;
    FrameAnimation _frameAnimation;
    float _moveSpeed;
    const float CONST_RateOfFire = 0.5f;
    [SerializeField] SpriteRenderer[] currentRenderers;
    Vector2 _moveDir;
    bool _canHit = true;
    float _timerCanHit;

    [Button]
    void Check()
    {
       
    }

    

    void OnEnable()
    {
        _myData = (SoEnemy)myData;
        _moveSpeed = _myData.moveSpeed;
        #if(UNITY_EDITOR)
        {
            if (GameManager.Instance.useCustomMoveSpeed) _moveSpeed = GameManager.Instance.customMoveSpeed;
        }
        #endif
        
        inSimulation = true;
        _canHit = true;
        _timerCanHit = 0f;
        if (myData != null) health = _myData.maxHealth;
        
        _frameAnimation = new FrameAnimation(currentRenderers, myData.spritesMove);
    }

    void OnDisable()
    {
        int childCounter = currentRenderers.Length;
        for (int i = 0; i < childCounter; i++)
        {
            Destroy(currentRenderers[i].gameObject);
        }
    }


    public void UpdateLoop()
    {
        _frameAnimation.UpdateLoop(myTransform.position.x < GameManager.Instance.playerPosition.x);
        if (!inSimulation) return;
        myTransform.position += _moveSpeed  * Time.deltaTime * (Vector3)_moveDir;

        if (!_canHit)
        {
            _timerCanHit += Time.deltaTime;
            if (_timerCanHit >= CONST_RateOfFire)
            {
                _timerCanHit = 0f;
                _canHit = true;
            }
        }
    }

    public void SegmentedUpdateLoop()
    {
        if (!inSimulation) return;
        
        Vector2 myPos = myTransform.position;
        SectorCheckup(myPos);
        Collisions();
        _moveDir += (GameManager.Instance.playerPosition - myPos).normalized;
        _moveDir.Normalize();

        void Collisions()
        {
            if (!GameManager.Instance.sectorsOnScreens.Contains(sector)) return;

            HashSet<SectorObject> areaEnemies = GameManager.Instance.NeighbouringObjects(sector);
            foreach (SectorObject item in areaEnemies)
            {
                if (item == this || !item.inSimulation) continue;
                Vector2 itemPos = item.myTransform.position;
                float dist = Mathf.Abs(myPos.x - itemPos.x) + Mathf.Abs(myPos.y - itemPos.y);
                if (dist > 1f) continue;

                if (item is HitObject bullet)
                {
                    if (!bullet.myWeaponParent.enemiesHit.Contains(this))
                    {
                        bullet.myWeaponParent.enemiesHit.Add(this);
                        StartCoroutine(DelayedTakeDamage(bullet.myWeaponParent.damage));
                        bullet.HitSomething();
                    }
                }
                if (item is PlayerControl)
                {
                    if (_canHit) item.TakeDamage(_myData.damage);
                    _canHit = false;
                }
                if (item is Enemy) Avoid();
                if (item is Obstacle) Avoid(10);

                void Avoid(int magnitude = 1)
                {
                    Vector2 direction = myPos - itemPos;
                    direction.Normalize();
                    _moveDir += magnitude * direction;
                }
            }
            _moveDir *= 10f;
        }
    }

    IEnumerator DelayedTakeDamage(int dam)
    {
        yield return new WaitForEndOfFrame();
        TakeDamage(dam);
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        GameManager.Instance.poolManager.GetFloatDamage(myTransform.position, damage);
        StopAllCoroutines();
        StartCoroutine(Utils.HitFlash(currentRenderers));
        if (health <= 0)
        {
            inSimulation = false;
            _frameAnimation = new FrameAnimation(currentRenderers, myData.spritesDeath, () => { StartCoroutine(DeathSequence()); });
        }
    }

    IEnumerator DeathSequence()
    {
        yield return Utils.GetWait(3f);
        GameManager.Instance.SectorObjectsCommand(this, Commands.RemoveFromGroup);
        Utils.Activation(myGameObject, GenActivation.Off);
    }

}


