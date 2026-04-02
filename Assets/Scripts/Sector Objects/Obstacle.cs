using System;
using UnityEngine;

public class Obstacle : SectorObject
{
    Vector2 _myPos;
    void Start()
    {
        inSimulation = true;
        sector = GameManager.Instance.GetSector(myTransform.position);
        GameManager.Instance.sectors[sector].group.Add(this);
        _myPos = myTransform.position;
    }

    void Update()
    {
     //   SectorCheckup(_myPos);
        SectorCheckup();
    }

}
