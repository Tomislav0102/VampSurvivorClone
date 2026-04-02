
using System;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] SpriteRenderer[] renders;
    FrameAnimation _frameAnimation;

    void Start()
    {
        _frameAnimation = new FrameAnimation(renders);
    }

    void Update()
    {
        _frameAnimation.UpdateLoop();
    }
}
