using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatScript : MonoBehaviour
{
    private float energyStored;
    private int surviveTime = 0;
    private Renderer rend;
    private int rotRate;

    private CreateObjects god;

    // Start is called before the first frame update
    void Start()
    {
        god = GameObject.Find("CreateObjects").GetComponent<CreateObjects>();
        god.numMeats++;

        rotRate = Random.Range(16, 25);
        rend = GetComponent<Renderer>();
        rend.material.color = new Color(0.6f, 0.2f, 0.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)Mathf.Floor(Time.fixedTime) != surviveTime)
        {
            surviveTime = (int)Mathf.Floor(Time.fixedTime);
            //runs 1 time per second
            if (surviveTime % rotRate == 0)
            {
                energyStored--;
                transform.localScale = new Vector3(transform.localScale.x * 0.95f, transform.localScale.y * 0.95f);
                rend.material.color = new Color(rend.material.color.r * 0.95f, rend.material.color.g, rend.material.color.b);
            }

            if (energyStored < 3)
            {
                consume();

            }

        }
    }

    public float consume()
    {
        god.numMeats--;
        //Debug.Log("Consmued plant");
        Destroy(this.gameObject, 0.01f);
        return energyStored;
    }

    public void SetEnergy(float e)
    {
        energyStored = e;
    }
}
