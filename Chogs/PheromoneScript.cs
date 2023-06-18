using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PheromoneScript : MonoBehaviour
{
    private GameObject creatorChog;
    private float strength;
    private int surviveTime = 0;
    private int timer = 0;
    private Renderer rend;


    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)Mathf.Floor(Time.fixedTime) != surviveTime)
        {
            surviveTime = (int)Mathf.Floor(Time.fixedTime);
            timer++;

            //runs 1 time per second
            if (timer % 2 == 0)
            {
                strength /= 2f;
                transform.localScale = new Vector3(transform.localScale.x * 0.95f, transform.localScale.y * 0.95f);
                rend.material.color = new Color(rend.material.color.r * 0.9f, rend.material.color.g, rend.material.color.b * 0.9f);
            }

            if (strength < 1)
            {
                Destroy(gameObject);

            }

        }
    }


    public GameObject GetCreator()
    {
        return creatorChog;
    }


    public float GetStrength(GameObject smeller)
    {
        if(smeller == creatorChog)
        {
            return 0;
        }
        return strength;
    }


    public void SetStrength(float e, GameObject creator)
    {
        creatorChog = creator;
        strength = e;
    }
}
