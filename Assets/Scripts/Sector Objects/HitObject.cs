using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class HitObject : SectorObject
{
    [ReadOnly] public Weapon myWeaponParent;
    bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_isActive)
            {
                
            }
            else
            {
                myWeaponParent = null;
                _lifeTime = -1;
            }
        }
    }
    bool _isActive;
    float _lifeTime = -1;
    
    void Update()
    {
        if (!IsActive) return;
        SectorCheckup();

        if (_lifeTime == -1) return;
        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0)
        {
            myWeaponParent.Expiration(this);
            IsActive = false;
        }
    }

    public void OnActivate(float lifeTime = -1)
    {
        IsActive = true;
        _lifeTime = lifeTime;
    }
    public void OnDeActivate()
    {
        IsActive = false;
    }
    public void HitSomething()
    {
        myWeaponParent.Hit(this);
    }

}
