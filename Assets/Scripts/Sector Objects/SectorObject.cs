using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

public class SectorObject : MonoBehaviour
{
    [ReadOnly] public int sector;
    public Transform myTransform;
    public GameObject myGameObject;
    public SoAgent myData;
    [ReadOnly] public bool inSimulation;
    protected int health;


    public virtual void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
    }

    protected void SectorCheckup()
    {
        Vector2 myPos = myTransform.position;
        SectorCheckup(myPos);
    }

    protected void SectorCheckup(Vector2 myPos)
    {
        int currentSector = GameManager.Instance.GetSector(myPos);
        if (currentSector == sector) return;
        
        if (GameManager.Instance.sectors[sector].group.Contains(this)) GameManager.Instance.sectors[sector].group.Remove(this);
        if (currentSector < GameManager.Instance.sectors.Length && currentSector >= 0)
        {
            GameManager.Instance.sectors[currentSector].group.Add(this);
            sector = currentSector;
            return;
        }
        
        //sectorObject is outside sector grid, needs to be handled (teleport to other side?)
    }
    


}

public class SectorGroup
{
    public HashSet<SectorObject> group;

    public SectorGroup()
    {
        group = new HashSet<SectorObject>();
    }
}

