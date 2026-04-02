using System;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] protected SpriteRenderer[] renderers;
    protected FrameAnimation frameAnimation;
    public HashSet<SectorObject> enemiesHit = new HashSet<SectorObject>();
    public int damage;

    public virtual bool Equipped
    {
        get => _equipped;
        set
        {
            _equipped = value;
        }
    }
    bool _equipped;
    
    float _timerRof;
    [SerializeField]  protected float rof, lifeTime;

    protected virtual void Start()
    {
        frameAnimation = new FrameAnimation(renderers);
    }

    void Update()
    {
        if (!Equipped)
        {
            return;
        }
        
        if (_timerRof <= rof) _timerRof += Time.deltaTime;
        else
        {
            _timerRof = 0f;
            OnAttack();
        }
        
        UpdateLoop();
    }

    protected virtual void UpdateLoop(){}

    protected virtual void OnAttack(){}
    
    public virtual void Hit(HitObject hitObject){}
    public virtual void Expiration(HitObject hitObject){}

}
