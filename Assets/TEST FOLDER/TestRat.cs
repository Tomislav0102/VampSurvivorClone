using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class TestRat : MonoBehaviour
{
    public Test test;
    public int num = 100000;
    public bool runUpdate;

    void Update()
    {
        if (!runUpdate) return;
        for (int i = 0; i < num; i++)
        {
            GetTest();
        }
    }

    public void GetTest()
    {
        test.matEnable = !test.matEnable;
      // test.key = "asdfdsf";
    }
}
