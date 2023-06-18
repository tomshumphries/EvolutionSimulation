using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerryScript : MonoBehaviour
{
    public int surviveTime;
    private int time;
    private Renderer rend;
    public int lifeSpan, growthRate;
    public int size;

    private CreateObjects god;

    // Start is called before the first frame update
    void Start()
    {
        size = 5;
        surviveTime = 0;
        lifeSpan = Random.Range(40, 120);
        growthRate = Random.Range(7, 11);
        rend = GetComponent<Renderer>();
        god = GameObject.Find("CreateObjects").GetComponent<CreateObjects>();
        god.numBerries++;
        rend.material.color = new Color(0.9f, 0.6f, 0.9f);
        transform.localScale = new Vector3(0.5f, 0.5f);

    }

    // Update is called once per frame
    void Update()
    {
        if ((int)Mathf.Floor(Time.fixedTime) != time)
        {
            time = (int)Mathf.Floor(Time.fixedTime);
            surviveTime++;
            //grow once every "growthRate" seconds
            if (surviveTime % growthRate == 0)
            {
                Grow();
            }


            if (surviveTime > lifeSpan)
            {
                consume();


            }

        }
    }

    public int consume()
    {
        //Debug.Log("Consmued plant");
        god.numBerries--;
        Destroy(this.gameObject, 0.01f);

        return size;
    }

    private void Grow()
    {
        size++;
        transform.localScale = new Vector3(transform.localScale.x + Random.Range(0.06f, 0.1f), transform.localScale.y + Random.Range(0.06f, 0.1f));
        rend.material.color = new Color(rend.material.color.r + 0.03f, rend.material.color.g + 0.04f, rend.material.color.b + 0.02f);
    }
}
