using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private float value;
    private List<Node> outputs;
    public List<float> outputWeights;

    private bool useActivationFunction;

    private float offset; //range -2, +2

    public Node(float off, bool useA)
    {
        outputs = new List<Node>();
        outputWeights = new List<float>();
        value = offset;
        offset = off;
        useActivationFunction = useA;
    }

    public void addConnection(Node n, float value)
    {
        outputs.Add(n);
        outputWeights.Add(value);
    }



    public void reset()
    {
        value = offset;
    }

    public float getOffset()
    {
        return offset;
    }

    public void setValue(float val)
    {
        value = val;
    }

    public float getActivationValue()
    {
        if(Mathf.Abs(value) < 1) { return 0; }
        return Mathf.Max(-2 , Mathf.Min(value, 2));
    }

    public float getRawValue()
    {
        return value;
    }

    public void contributeToNextLayer()
    {
        float valueToUse = getRawValue(); ;
        if (useActivationFunction)
        {
            valueToUse = getActivationValue();
        }

        for (int i = 0; i < outputs.Count; i++)
        {
            outputs[i].receiveContribution(valueToUse * outputWeights[i]);
        }
    }

    public void receiveContribution(float val)
    {
        value += val;
    }

    public void mutateWeight()
    {
        int index = Random.Range(0, outputWeights.Count);
        outputWeights[index] = Random.Range(-1.5f, 1.5f);
    }

    public void mutateOffset()
    {
        offset = Random.Range(-1.5f,1.5f);
    }

}
