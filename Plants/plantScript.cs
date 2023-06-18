using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plantScript : MonoBehaviour
{
    public float energyStored;
    public int surviveTime;
    private int time;
    private Renderer rend;
    public int lifeSpan, growthRate;
    public int size;

    private CreateObjects god;

    // Start is called before the first frame update
    void Start()
    {
        size = 1;
        surviveTime = 0;
        lifeSpan = Random.Range(60, 120);
        growthRate = Random.Range(7,11);
        energyStored = 6f;
        rend = GetComponent<Renderer>();
        god = GameObject.Find("CreateObjects").GetComponent<CreateObjects>();
        god.numPlants++;
        rend.material.color = new Color(0, 0.3f, 0);
        transform.localScale = new Vector3(0.5f,0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        if ((int)Mathf.Floor(Time.fixedTime) != time)
        {
            time = (int)Mathf.Floor(Time.fixedTime);
            surviveTime++;
            //grow once every "growthRate" seconds
            if(surviveTime%growthRate == 0)
            {
                Grow();
            }


            if(surviveTime > lifeSpan)
            {
                consume();
                

            }

        }
    }

    public float consume()
    {
        //Debug.Log("Consmued plant");
        god.numPlants--;
        Destroy(this.gameObject, 0.01f);
        
        return energyStored;
    }

    public void SetEnergy(float e)
    {
        energyStored = e;
    }

    private void Grow()
    {
        size++;

        energyStored++;
        transform.localScale = new Vector3(transform.localScale.x + Random.Range(0.06f, 0.1f), transform.localScale.y + Random.Range(0.06f, 0.1f));
        rend.material.color = new Color(rend.material.color.r, rend.material.color.g + 0.04f, rend.material.color.b);
    }


}
