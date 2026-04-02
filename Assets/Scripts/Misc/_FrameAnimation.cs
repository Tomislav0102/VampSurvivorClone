using System;
using UnityEngine;

public class FrameAnimation 
{
    SpriteRenderer[] _renderers;
    int _length;
    float _timer;
    int _counter;
    System.Action _callbackSequenceEnd;
    bool _isPlaying;

    public FrameAnimation(SpriteRenderer[] renderers, Sprite[] spritesToInsert = null, System.Action callbackSequenceEnd = null)
    {
        _renderers = renderers;
        if (_renderers == null || _renderers.Length == 0)
        {
            _isPlaying = false;
            return;
        }
        
        Sprite[] currentSprites = spritesToInsert;
        if (currentSprites == null)
        {
            currentSprites = new Sprite[_renderers.Length];
            for (int i = 0; i < currentSprites.Length; i++)
            {
                currentSprites[i] = _renderers[i].sprite;
            }
        }
        else
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                if (i < currentSprites.Length)
                {
                    _renderers[i].sprite = currentSprites[i];
                    _renderers[i].enabled = i == 0;
                }
                else
                {
                    _renderers[i].enabled = false;
                }
            }
        }
        
        _callbackSequenceEnd = callbackSequenceEnd;
        _length = currentSprites.Length;
        _counter = 0;
        _timer = 0f;
        _isPlaying = true;
    }


    public void UpdateLoop(bool flipX = false)
    {
        if (!_isPlaying) return;
        
        _timer += Time.deltaTime;
        if (_timer > 0.1f)
        {
            _timer = 0f;
            _renderers[_counter].enabled = false;
            _counter = (1 + _counter) % _length;
            _renderers[_counter].enabled = true;
            _renderers[_counter].flipX = flipX;

            if (_counter == _length - 1 && _callbackSequenceEnd != null)
            {
                _isPlaying = false;
                _callbackSequenceEnd.Invoke();
            }
        }
    }

}
