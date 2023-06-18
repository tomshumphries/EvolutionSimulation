using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChogVisionScript : MonoBehaviour
{
    ChogScript cho = null;

    private void Start()
    {
        cho = GetComponentInParent<ChogScript>();
    }
    void OnTriggerEnter2D(Collider2D coll)
    {
        if(cho == null)
        {
            cho = GetComponentInParent<ChogScript>();
        }

        if (coll.gameObject.CompareTag("Plant"))
        {
            if (!cho.plantSee.Contains(coll.gameObject))
            {
                cho.plantSee.Add(coll.gameObject);
            }

        }

        if (coll.gameObject.CompareTag("Chog"))
        {
            if (!cho.chogSee.Contains(coll.gameObject) && coll.gameObject != transform.parent.gameObject)
            {
                cho.chogSee.Add(coll.gameObject);
            }

        }

        if (coll.gameObject.CompareTag("Meat"))
        {
            if (!cho.meatSee.Contains(coll.gameObject))
            {
                cho.meatSee.Add(coll.gameObject);
            }

        }

        if (coll.gameObject.CompareTag("Berry"))
        {
            if (!cho.berrySee.Contains(coll.gameObject))
            {
                cho.berrySee.Add(coll.gameObject);
            }

        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (cho.plantSee.Contains(coll.gameObject))
        {
            cho.plantSee.Remove(coll.gameObject);
        }
        if (cho.chogSee.Contains(coll.gameObject))
        {
            cho.chogSee.Remove(coll.gameObject);
        }
        if (cho.meatSee.Contains(coll.gameObject))
        {
            cho.meatSee.Remove(coll.gameObject);
        }

        if (cho.berrySee.Contains(coll.gameObject))
        {
            cho.berrySee.Remove(coll.gameObject);
        }
    }

}
