using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FamilyInfo
{
    int total;
    int current;

    public List<GameObject> livingChogs;

    public FamilyInfo()
    {
        total = 0;
        current = 0;
        livingChogs = new List<GameObject>();
    }

    public void ChogBorn(GameObject chog)
    {
        total++;
        current++;
        livingChogs.Add(chog);
    }

    public void ChogDied(GameObject chog)
    {
        livingChogs.Remove(chog);
        current--;
    }

    public int GetTotal()
    {
        return total;
    }

    public int GetCurrent()
    {
        return current;
    }
}
