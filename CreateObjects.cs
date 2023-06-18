using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;


public class CreateObjects : MonoBehaviour
{
    public GameObject plant;
    public GameObject berry;
    public GameObject chog;
    public GameObject plants;
    public GameObject berries;

    public float size;
    public int plantMax, berryMax, chogMin;

    public Dictionary<int, FamilyInfo> families;

    public int time = 0;

    public int numChogs = 0, numPlants = 0, numBerries = 0, numMeats = 0, biggestFam = 0, biggestFamAmount = 0;

    public Canvas canvas;
    private Text text;

    private CreateGraph graph;


    
    // Start is called before the first frame update
    void Start()
    {
        graph = new CreateGraph();
        text = canvas.transform.Find("OverallStats").GetComponent<Text>();
        families = new Dictionary<int, FamilyInfo>();

        for (int i = 0; i < plantMax; i++)
        {
            CreatePlant();

            if (i % 15 == 0)
            {
                CreateChog();

            }
            if (i % 4 == 0)
            {
                CreateBerry();
            }
        }
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            graph.SaveData(0);
        }


        if ((int)Mathf.Floor(Time.fixedTime) != time)
        {
            time = (int)Mathf.Floor(Time.fixedTime);
            
            if(numPlants + numChogs * 2 < plantMax) //if there are too many plants or too many chogs, skip spawns every other second
            {
                for (int i = 0; i < 10; i++)
                {
                    CreatePlant();
                }
            }

            if (numBerries < berryMax)
            {
                for (int i = 0; i < 3; i++)
                {
                    CreateBerry();
                }
            }

            if (numChogs < chogMin || time%10 == 0)
            {
                CreateChog();
            }

            List<int> famsTORemove = new List<int>();
            foreach(KeyValuePair<int,FamilyInfo> fam in families)
            {
                if(fam.Value.GetCurrent() == 0)
                {
                    famsTORemove.Add(fam.Key);
                }
            }
            foreach(int f in famsTORemove)
            {
                families.Remove(f);
            }

            DisplayInfo();
            if(time%30 == 0)
            {
                graph.AddData(numChogs, numPlants, numBerries, numMeats, biggestFam, biggestFamAmount);
            }
            if(time % 60*60 == 0)
            {
                graph.SaveData(time);
            }


        }
    }

    public void CreatePlant()
    {
        Vector2 loc2d = Random.insideUnitCircle * size;
        Vector3 loc = new Vector3(loc2d.x, loc2d.y, 0);
        //numPlants++;
        GameObject p = Instantiate(plant, loc , Quaternion.identity);
        
        p.transform.parent = plants.transform;
    }

    public void CreateChog()
    {
        int key = Random.Range(0, 100000);
        if (families.ContainsKey(key))
        {
            return;
        }

        Vector2 loc2d = Random.insideUnitCircle * size;
        Vector3 loc = new Vector3(loc2d.x, loc2d.y, 0);

        families.Add(key, new FamilyInfo());
        GameObject c = Instantiate(chog, loc, Quaternion.identity);
        numChogs++;
        ChogBorn(key, key, c);
        c.GetComponent<ChogScript>().FirstChog(key);
    }

    private void CreateBerry()
    {
        Vector2 loc2d = Random.insideUnitCircle * size;
        Vector3 loc = new Vector3(loc2d.x, loc2d.y, 0);

        GameObject b = Instantiate(berry, loc, Quaternion.identity);

        b.transform.parent = berries.transform;
    }

    private void DisplayInfo()
    {
        var sortedDict = from entry in families orderby entry.Value.GetCurrent() descending select entry;

        string s = "";
        s+= string.Format("Time: {4}m{5}s\n\nPlants: {0}\nChogs: {1}\nBerries: {2}\nMeats: {3}\n\n", plants.transform.childCount, numChogs, berries.transform.childCount, numMeats, time / 60, time % 60);

        s+= "Biggest families:\nTag - Total - Current\n\n";

        biggestFam = sortedDict.ElementAt(0).Key;
        biggestFamAmount = sortedDict.ElementAt(0).Value.GetCurrent();


        int display = Mathf.Min(sortedDict.Count(), 15);
        for(int i = 0; i < display; i++)
        {
            KeyValuePair<int, FamilyInfo> fam = sortedDict.ElementAt(i);
            s += string.Format("{0} : {1} - {2}\n", fam.Key, fam.Value.GetTotal(), fam.Value.GetCurrent());

        }


        text.text = s;

    }


    public void ChogBorn(int dadTag, int mumTag, GameObject chog)
    {
        families[dadTag].ChogBorn(chog);
        if (mumTag != dadTag)
        {
            families[mumTag].ChogBorn(chog);
        }
    }

}
