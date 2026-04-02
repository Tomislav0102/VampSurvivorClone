using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class SimpleProjectile : Weapon
{
    public override bool Equipped
    {
        get => base.Equipped;
        set
        {
            base.Equipped = value;
        } 
    }
    
    float _moveSpeed = 10f;

    List<HitObject> _hitObjects = new List<HitObject>();
    
    [Button]
    void M()
    {
        
    }

    protected override void Start()
    {
        base.Start();
        Equipped = true;
    }

    protected override void UpdateLoop()
    {
        base.UpdateLoop();
        foreach (HitObject ho in _hitObjects)
        {
            ho.myTransform.position += _moveSpeed * Time.deltaTime * ho.myTransform.up;
        }

    }

    protected override void OnAttack()
    {
        base.OnAttack();
        Vector2 dir = ((Vector2)GameManager.Instance.mainCam.ScreenPointToRay(Input.mousePosition).origin - GameManager.Instance.playerPosition).normalized;
        HitObject ho = GameManager.Instance.poolManager.GetSectorObject<HitObject>();
        ho.myTransform.position = GameManager.Instance.playerPosition; 
        ho.myTransform.up = dir;
        ho.myWeaponParent = this;
        ho.OnActivate(lifeTime);
        _hitObjects.Add(ho);
    }

    public override void Hit(HitObject hitObject)
    {
        base.Hit(hitObject);
        EndHitObject(hitObject);
    }

    public override void Expiration(HitObject hitObject)
    {
        base.Expiration(hitObject);
        EndHitObject(hitObject);
    }

    void EndHitObject(HitObject ho)
    {
        if (_hitObjects.Contains(ho)) _hitObjects.Remove(ho);
    }
}
