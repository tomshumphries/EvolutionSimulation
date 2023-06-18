using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChogScent : MonoBehaviour
{
    ChogScript cho = null;

    private void Start()
    {
        cho = GetComponentInParent<ChogScript>();
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.CompareTag("Pheromone"))
        {
            if(coll.GetComponent<PheromoneScript>().GetCreator() == transform.parent.gameObject)
            {
                return;
            }
            if (!cho.pheromoneSmell.Contains(coll.gameObject))
            {
                cho.pheromoneSmell.Add(coll.gameObject);
            }

        }
    }

    private void OnTriggerExit2D(Collider2D coll)
    {
        if (cho.pheromoneSmell.Contains(coll.gameObject))
        {
            cho.pheromoneSmell.Remove(coll.gameObject);
        }
    }
}


