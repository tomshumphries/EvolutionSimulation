using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class CreateGraph
{
    public List<int> time;
    List<int> chogPop;
    List<int> plantPop;
    List<int> berryPop;
    List<int> meatPop;
    List<int> family;
    List<int> familyAmount;

    public CreateGraph()
    {
        chogPop = new List<int>();
        plantPop = new List<int>();
        berryPop = new List<int>();
        meatPop = new List<int>();
        family = new List<int>();
        familyAmount = new List<int>();
    }


    public void AddData(int chog, int plant, int berry, int meat, int fam, int famAmount)
    {
        chogPop.Add(chog);
        plantPop.Add(plant);
        berryPop.Add(berry);
        meatPop.Add(meat);
        family.Add(fam);
        familyAmount.Add(famAmount);
    }

    public void SaveData(int id)
    {
        var csv = new System.Text.StringBuilder();

        csv.AppendLine("time,chogPop,plantPop,berryPop,meatPop,family,familyAmount");

        for (int i = 0; i < chogPop.Count; i++)
        {
            var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6}", i * .25f, chogPop[i].ToString(),plantPop[i].ToString(), berryPop[i].ToString(), meatPop[i].ToString(), (family[i]/100).ToString(), familyAmount[i].ToString());
            csv.AppendLine(newLine);
        }
        

        //after your loop
        File.WriteAllText("Assets/Charts/chogData" + id.ToString(), csv.ToString());
    }


}
