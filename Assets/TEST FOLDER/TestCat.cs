using System;
using UnityEngine;

public class TestCat : MonoBehaviour
{
    public TestRat rat;
    public int num = 100000;

    void OnEnable()
    {
        rat.runUpdate = false;
    }

    void Update()
    {
        for (int i = 0; i < num; i++)
        {
            GetTest();
        }
    }

    public void GetTest()
    {
        rat.GetTest();
    }

    void OnDisable()
    {
        rat.runUpdate = true;
    }
}
