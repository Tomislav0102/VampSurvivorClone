using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : SectorObject
{
    SoPlayer _myData;
    [SerializeField] SpriteRenderer hpBar;
    Transform _hpBarTransform;
    float _timerHpVisible;
    const int CONST_HpBarVisibleTime = 3;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer rend;
    SpriteRenderer[] _rendsForCoroutine;
    [SerializeField] Animator anim;
    [SerializeField] float moveSpeed;
    float _hor, _ver;
    Vector2 _moveDir;
    public WeaponType[] activeWeapons;

    void Awake()
    {
        _hpBarTransform = hpBar.transform;
        _myData = (SoPlayer)myData;
    }

    void Start()
    {
        inSimulation = true;
        health = _myData.maxHealth;
        _rendsForCoroutine = new SpriteRenderer[] { rend };
        SectorCheckup();
    }

    public void GetMySectorFromGameManager(Vector2 modPosition)
    {
       // SectorCheckup(myTransform.position + (Vector3)modPosition);
    }

    void Update()
    {
        _hor = Input.GetAxis("Horizontal");
        _ver = Input.GetAxis("Vertical");
        _moveDir = new Vector2(_hor, _ver);
        _moveDir.Normalize();

        if (_hor > 0) rend.flipX = false;
        else if (_hor < 0) rend.flipX = true;
        if (_moveDir.magnitude == 0f) anim.Play("idle");
        else anim.Play("walk");

        if (Input.GetMouseButtonDown(0))
        {
        }

        if (_timerHpVisible > 0)
        {
            _timerHpVisible -= Time.deltaTime;
            Utils.Activation(hpBar, GenActivation.On);
        }
        else
        {
            Utils.Activation(hpBar, GenActivation.Off);
        }

    }

    void FixedUpdate()
    {
        rb.linearVelocity = _moveDir * moveSpeed;
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        GameManager.Instance.poolManager.GetFloatDamage(myTransform.position, damage);
        _timerHpVisible = CONST_HpBarVisibleTime;
        float hpPercent = (float)health / _myData.maxHealth;
        _hpBarTransform.localScale = new Vector3(hpPercent, _hpBarTransform.localScale.y, 1);
        StartCoroutine(Utils.HitFlash(_rendsForCoroutine));
    }
}
