using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    private float speed = 8;

    public GameObject chogToFollow = null;

    public Canvas canvas;

    private Text input, output, brain,stats;

    private int counter = 0;
    private Dictionary<int, FamilyInfo> fam;


    private void Start()
    {
        input = canvas.transform.GetChild(0).GetComponent<Text>();
        //skip 1 for overallStats
        stats = canvas.transform.GetChild(2).GetComponent<Text>();
        brain = canvas.transform.GetChild(3).GetComponent<Text>();
        output = canvas.transform.GetChild(4).GetComponent<Text>();

        fam = GameObject.Find("CreateObjects").GetComponent<CreateObjects>().families;
    }

    void Update()
    {
        counter++;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            chogToFollow = null;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int eaten = 0;
            GameObject[] chogs = GameObject.FindGameObjectsWithTag("Chog");
            foreach (GameObject c in chogs)
            {

                if (c.GetComponent<ChogScript>().stats["plantsEaten"] > eaten)
                {
                    eaten = (int)c.GetComponent<ChogScript>().stats["plantsEaten"];
                    chogToFollow = c;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            int timeAlive = 0;
            GameObject[] chogs = GameObject.FindGameObjectsWithTag("Chog");
            foreach (GameObject c in chogs)
            {

                if (c.GetComponent<ChogScript>().stats["timeAlive"] > timeAlive)
                {
                    timeAlive = (int)c.GetComponent<ChogScript>().stats["timeAlive"];
                    chogToFollow = c;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            CreateGraph cg = new CreateGraph();
            int eaten = 0;
            GameObject[] chogs = GameObject.FindGameObjectsWithTag("Chog");
            foreach (GameObject c in chogs)
            {

                if (c.GetComponent<ChogScript>().stats["meatEaten"] > eaten)
                {
                    eaten = (int)c.GetComponent<ChogScript>().stats["meatEaten"];
                    chogToFollow = c;
                }
            }
        }

        //Get newest decendant of biggest family
        if (Input.GetKeyDown(KeyCode.F))
        {
            int key = 0;
            int size = 0;
            foreach(KeyValuePair<int,FamilyInfo> f in fam)
            {
                if (f.Value.GetCurrent() > size)
                {
                    size = f.Value.GetCurrent();
                    key = f.Key;
                }
            }            
            chogToFollow = fam[key].livingChogs[(int)Random.Range(0, fam[key].GetCurrent())];
        }

        if (chogToFollow != null)
        {
            transform.position = new Vector3(chogToFollow.transform.position.x , chogToFollow.transform.position.y ,-10);
            if(counter%40 == 0)
            {
                input.text = chogToFollow.GetComponent<ChogScript>().PrintInputInfo();
                brain.text = chogToFollow.GetComponent<ChogScript>().PrintBrainInfo();
                output.text = chogToFollow.GetComponent<ChogScript>().PrintOutputInfo();
                stats.text = chogToFollow.GetComponent<ChogScript>().PrintStatInfo();
            }
            return;
        }
        else
        {
            input.text = "M: Chog which has consumes the most meat\nSpace: Chog which has consumes the most plants\nF: Most recent child of largest family\n";
            output.text = "";
            brain.text = "";
            stats.text = "";
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += Vector3.left * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += Vector3.right * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += Vector3.up * Time.deltaTime * speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += Vector3.down * Time.deltaTime * speed;
        }
    }


}
