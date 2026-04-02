using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enemyNumText;
    int Num
    {
        get => _num;
        set
        {
            _num = value;
            if (value < CONST_Value) _num = CONST_Value;
            enemyNumText.text = $"{_num} enemies";
            PlayerPrefs.SetInt("num", _num);
        }
    }
    int _num;
    const int CONST_Value = 500;

    void Awake()
    {
        if (!PlayerPrefs.HasKey("num"))
        {
            PlayerPrefs.SetInt("num", CONST_Value);
        }
        Num = PlayerPrefs.GetInt("num");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    public void BtnMethod(bool increase)
    {
        if (increase) Num += CONST_Value;
        else
        {
            Num -= CONST_Value;
        }
    }
    
    public void BtnMethodStartGame() => SceneManager.LoadScene("Game");
}
