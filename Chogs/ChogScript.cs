using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class ChogScript : MonoBehaviour
{
    public Dictionary<int, float> ancestorList;
    private int totalAncestors;

    public Dictionary<string, float> stats;
    public Dictionary<string, float> geneList;

    //brain
    List<string> inputKeys;
    List<string> outputKeys;

    public Dictionary<string, Node> inputNodes;
    public Dictionary<string, Node> outputNodes;

    public List<Node> hidden1;
    public List<Node> hidden2;

    private int numInternal = 8;


    //values

    private Vector3 me;
    const int reproduceConstant = 30;
    const int maxSoloKids = 4;

    const bool allowSoloKids = false;
    const bool helpChogs = true;

    public bool custom = false;

    private Renderer rend;
    public CreateObjects god;

    private GameObject plantTouch;
    private GameObject chogTouch;
    private GameObject meatTouch;
    private GameObject berryTouch;

    private GameObject localChog;

    //private List<int> chogsCanFreelyBreedWith;

    public List<GameObject> plantSee;
    public List<GameObject> meatSee;
    public List<GameObject> chogSee;
    public List<GameObject> berrySee;
    public List<GameObject> pheromoneSmell;

    public GameObject chog;
    public GameObject meat;
    public GameObject pheromone;

    private GameObject pheromones;


    private bool mouseOver;
    private int time = 0;
    int brainSkip = 0;
    private bool willingToBreed;
    private GameObject friendlyChog;
    private Renderer vision;


    // Start is called before the first frame update
    void Start()
    {

        vision = transform.GetChild(0).GetChild(0).GetComponent<Renderer>();
        rend = GetComponent<Renderer>();
        plantSee = new List<GameObject>();
        meatSee = new List<GameObject>();
        chogSee = new List<GameObject>();
        pheromoneSmell = new List<GameObject>();
        pheromones = GameObject.FindGameObjectWithTag("Pheromones");

        god = GameObject.Find("CreateObjects").GetComponent<CreateObjects>();
        //chogsCanFreelyBreedWith = new List<int>();

        time = 0;

    }

    public void FirstChog(int familyKey)
    {
        rend = GetComponent<Renderer>();
        plantSee = new List<GameObject>();
        meatSee = new List<GameObject>();
        chogSee = new List<GameObject>();
        ancestorList = new Dictionary<int, float>();
        ancestorList.Add(familyKey, 1);
        totalAncestors = 1;

        god = GameObject.Find("CreateObjects").GetComponent<CreateObjects>();

        geneList = new Dictionary<string, float>
        {
            { "maxSpeed", Random.Range(0.6f,0.8f) },
            { "size", Random.Range(0.8f,1.2f) }, //bigger size = more
            { "metabolism", Random.Range(0.8f,1.2f) }, //energy consumed per update just to stay alive
            { "maxEnergy", Random.Range(40f,60f) },
            { "maxHealth", Random.Range(70f,110f) },
            { "viewAngle", Random.Range(160f,240f) },
            { "viewDistance", Random.Range(1.2f,2f) },
            { "mutationChance", Random.Range(0.4f,0.6f) },
            { "mutationSize", 0.35f },
            { "eggRate",Random.Range(0.8f,1.2f) },
            { "aggression", Random.Range(0.4f,1.2f) },
            { "strength", Random.Range(0.6f,1.4f) },
            { "diet", Random.Range(0.2f,0.8f) }, // < 0.3 = herbivor, 0.3 - 0.7 omnivor,  > 0.7 carnivor
            { "pheromoneStrength", Random.Range(10,30) },
        };



        buildInOutBrain();

        hidden1 = new List<Node>();
        hidden2 = new List<Node>();

        for (int i = 0; i < numInternal; i++)
        {
            hidden1.Add(new Node(Random.Range(-1f, 1f), true));
            hidden2.Add(new Node(Random.Range(-1f, 1f), true));
        }


        inputKeys = new List<string>(inputNodes.Keys);
        outputKeys = new List<string>(outputNodes.Keys);


        stats = new Dictionary<string, float>
        {
            { "speed", geneList["maxSpeed"] },
            { "energy", geneList["maxEnergy"] / 2f }, //bigger size = more
            { "health", geneList["maxHealth"] }, //energy consumed per update just to stay alive
            { "plantsEaten", 0 },
            { "meatEaten", 0 },
            { "chogsAttacked", 0},
            { "children", 0 },
            { "mutations", 0 },
            { "timeSinceLastMeal",0},
            { "timeAlive",0 },
            { "reproduceTimer", reproduceConstant * geneList["eggRate"] },
            { "berryTimer", 0 },
            { "angle", Random.Range(-Mathf.PI, Mathf.PI)},
            { "familyTag", familyKey },
            { "mumTag", familyKey },
            { "familyNum", god.families[familyKey].GetTotal() },
            
            { "generation", 1 },
            { "age", 1 },
        };

        //copy input -> 1st from parents
        foreach (string key in inputKeys)
        {
            for (int i = 0; i < numInternal; i++)
            {
                float weight = Random.Range(-0.1f, 0.1f);
                if (Random.Range(0, 8) == 0)
                {
                    weight = Random.Range(-2f, 2f);
                }
                inputNodes[key].addConnection(hidden1[i], weight);
            }
        }

        //GIVE DIRECT LINK BETWEEN FOOD AND TURN
        if (helpChogs)
        {
            inputNodes["plantAngle"].addConnection(outputNodes["turn"], 1.5f);
        }

        //copy 1st->2nd from parents
        for (int i = 0; i < numInternal; i++)
        {
            for (int j = 0; j < numInternal; j++)
            {
                float weight = Random.Range(-0.1f, 0.1f);
                if (Random.Range(0,4) == 0)
                {
                    weight = Random.Range(-2f, 2f);
                }
                hidden1[i].addConnection(hidden2[j], weight);
            }
        }

        //copy 2nd-> output from parents
        for (int i = 0; i < numInternal; i++)
        {
            int j = 0;
            foreach (string key in outputKeys)
            {
                float weight = Random.Range(-0.1f, 0.1f);
                if (Random.Range(0, 6) == 0)
                {
                    weight = Random.Range(-2f, 2f);
                }
                hidden2[i].addConnection(outputNodes[key], weight);
                j++;
            }
        }


        if (custom == false)
        {
            int range = Random.Range(10, 18);
            for (int i = 0; i < range; i++) { Mutate(); }
        }

        rend.material.color = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.6f, 1f);

        me = new Vector3(); me.x = Mathf.Cos(0); me.y = Mathf.Sin(0); me.z = 0;

        transform.name = string.Format("Chog {0}/{1}", stats["familyTag"], stats["familyNum"]);
        //Now its time to use these genes to edit the chogs collider values and such

        transform.GetChild(0).GetComponent<CircleCollider2D>().radius = geneList["viewDistance"];
        transform.GetChild(0).GetChild(0).localScale = new Vector3(12, 12, 1) * geneList["viewDistance"];

        transform.GetChild(1).GetComponent<CircleCollider2D>().radius = geneList["viewDistance"] * 3;
        transform.GetChild(1).GetChild(0).localScale = new Vector3(12, 12, 1) * geneList["viewDistance"] * 3;

    }

    // Update is called once per frame
    void Update()
    {


        if(transform.position.x > 50 || transform.position.x < -50 || transform.position.y > 50 || transform.position.y < -50)
        {
            Die();
        }

        brainSkip++;
        if(brainSkip == 50)
        {
            //run the inputs
            CalculateInputs();
            RunNet();
            brainSkip = 0;
        }

        
        //evaluate the outputs
        RunOutputs();

        if(mouseOver && Input.GetMouseButtonDown(0))
        {
            GameObject.Find("Main Camera").GetComponent<CameraScript>().chogToFollow = this.gameObject;
        }

        if ((int)Mathf.Floor(Time.fixedTime) != time)
        {
            time = (int)Mathf.Floor(Time.fixedTime);
            UpdateEachSecond();
        }

        //transform.position = Vector3.MoveTowards(transform.position, transform.position + me, speed /1000f);

    }

    private void CalculateInputs()
    {
        localChog = null;
        friendlyChog = null;

        //runs 1 time per second.

        inputNodes["berryAngle"].reset();
        inputNodes["berryProximity"].reset();

        inputNodes["plantAngle"].reset();
        inputNodes["plantProximity"].reset();

        inputNodes["meatAngle"].reset();
        inputNodes["meatProximity"].reset();

        inputNodes["chogAngle"].reset();
        inputNodes["chogProximity"].reset();
        inputNodes["chogDirection"].reset();

        inputNodes["pheromoneAngle"].reset();


        ViewWorldObjects();

        inputNodes["healthLevel"].setValue((stats["health"] / geneList["maxHealth"]) * 4 - 2);
        inputNodes["energyLevel"].setValue((stats["energy"] / geneList["maxEnergy"]) * 4 - 2);
    }

    private void RunNet()
    {
        //reset hidden and ouput
        foreach (Node node in hidden1)
        {
            node.reset();
        }
        foreach (Node node in hidden2)
        {
            node.reset();
        }
        foreach (string key in outputKeys)
        {
            outputNodes[key].reset();
        }


        //and run
        foreach (string key in inputKeys)
        {
            inputNodes[key].contributeToNextLayer();
        }
        foreach (Node node in hidden1)
        {
            node.contributeToNextLayer();
        }
        foreach (Node node in hidden2)
        {
            node.contributeToNextLayer();
        }

    }

    private void RunOutputs()
    {
        HandleMovement();

        HandleReproduction();

        InteractWithWorldObjects();

        //Drain health if energy is 0
        if (stats["energy"] <= 0)
        {
            stats["energy"] = 0;
            stats["health"] -= geneList["maxHealth"] * Time.deltaTime * 0.2f;
        }

        //die if health is 0
        if (stats["health"] < 0)
        {
            Die();
        }


    }

    public void ViewWorldObjects()
    {
        if (plantSee.Count != 0) //if chog is within proximity of a plant
        {
            float biggestPlant = 0;

            //Work out which plant is closest and run inputs on that plant
            try
            {
                foreach (GameObject plant in plantSee)
                {
                    float angleToFood = Vector2.SignedAngle(new Vector2(me.x, me.y), new Vector2(plant.transform.position.x - transform.position.x, plant.transform.position.y - transform.position.y));

                    //IF IT IS IN ITS FIELD OF VIEW
                    if (angleToFood < geneList["viewAngle"] / 2f && angleToFood > -geneList["viewAngle"] / 2f)
                    {

                        if (plant.GetComponent<plantScript>().size > biggestPlant)
                        {
                            biggestPlant = plant.GetComponent<plantScript>().size;
                            inputNodes["plantAngle"].setValue(Mathf.Deg2Rad * angleToFood);
                            inputNodes["plantProximity"].setValue(2 - Vector3.Distance(transform.position, plant.transform.position) / geneList["viewDistance"]);
                        }
                    }
                }
            }
            catch
            {
                plantSee.Clear();
            }

        }

        if (chogSee.Count != 0) //if chog is within proximity of another chog
        {

            //handle overpopulation
            if(chogSee.Count > 6) 
            {
                Die();
            }

            float distToClosestChog = 100;

            //Work out which chog is closest and run inputs on that plant
            try
            {
                foreach (GameObject chog in chogSee)
                {
                    ////If chog is in the same family
                    //if ((int)stats["familyTag"] == (int)chog.GetComponent<ChogScript>().stats["familyTag"])
                    //{
                    //    friendlyChog = chog;

                    //}//or if they share a tag with a previously bred with species ("Ive bred with your family member before so can freely breed with you too")
                    //else if (chogsCanFreelyBreedWith.Contains((int)chog.GetComponent<ChogScript>().stats["familyTag"]))
                    //{
                    //    friendlyChog = chog;
                    //}

                    float angleToChog = Vector2.SignedAngle(new Vector2(me.x, me.y), new Vector2(chog.transform.position.x - transform.position.x, chog.transform.position.y - transform.position.y));

                    //IF IT IS IN ITS FIELD OF VIEW
                    if (Vector3.Distance(transform.position, chog.transform.position) < distToClosestChog)
                    {
                        localChog = chog;
                        if (angleToChog < geneList["viewAngle"] / 2f && angleToChog > -geneList["viewAngle"] / 2f)
                        {
                            distToClosestChog = Vector3.Distance(transform.position, chog.transform.position);
                            inputNodes["chogAngle"].setValue(Mathf.Deg2Rad * angleToChog);
                            inputNodes["chogProximity"].setValue(2 - Vector3.Distance(transform.position, chog.transform.position) / geneList["viewDistance"]);

                            Vector3 chogDir = chog.transform.GetComponent<ChogScript>().me;
                            inputNodes["chogDirection"].setValue(Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(me.x, me.y), new Vector2(chogDir.x, chogDir.y)));


                        }
                    }
                }
            }
            catch
            {
                chogSee.Clear();
            }



        }

        if (meatSee.Count != 0) //if chog is within proximity of a plant
        {
            float distToClosestMeat = 100;

            //Work out which plant is closest and run inputs on that plant
            try
            {
                foreach (GameObject meat in meatSee)
                {
                    float angleToFood = Vector2.SignedAngle(new Vector2(me.x, me.y), new Vector2(meat.transform.position.x - transform.position.x, meat.transform.position.y - transform.position.y));

                    //IF IT IS IN ITS FIELD OF VIEW
                    if (angleToFood < geneList["viewAngle"] / 2f && angleToFood > -geneList["viewAngle"] / 2f)
                    {
                        if (Vector3.Distance(transform.position, meat.transform.position) < distToClosestMeat)
                        {
                            distToClosestMeat = Vector3.Distance(transform.position, meat.transform.position);
                            inputNodes["meatAngle"].setValue(Mathf.Deg2Rad * angleToFood);
                            inputNodes["meatProximity"].setValue(2 - Vector3.Distance(transform.position, meat.transform.position) / geneList["viewDistance"]);
                        }
                    }
                }
            }
            catch
            {
                meatSee.Clear();
            }

        }

        if (berrySee.Count != 0) //if chog is within proximity of a plant
        {
            float distToClosestBerry = 100;

            //Work out which plant is closest and run inputs on that plant
            try
            {
                foreach (GameObject berry in berrySee)
                {
                    float angleToBerry = Vector2.SignedAngle(new Vector2(me.x, me.y), new Vector2(berry.transform.position.x - transform.position.x, berry.transform.position.y - transform.position.y));

                    //IF IT IS IN ITS FIELD OF VIEW
                    if (angleToBerry < geneList["viewAngle"] / 2f && angleToBerry > -geneList["viewAngle"] / 2f)
                    {
                        if (Vector3.Distance(transform.position, berry.transform.position) < distToClosestBerry)
                        {
                            distToClosestBerry = Vector3.Distance(transform.position, meat.transform.position);
                            inputNodes["berryAngle"].setValue(Mathf.Deg2Rad * angleToBerry);
                            inputNodes["berryProximity"].setValue(2 - Vector3.Distance(transform.position, berry.transform.position) / geneList["viewDistance"]);
                        }
                    }
                }
            }
            catch
            {
                berrySee.Clear();
            }

        }

        if (pheromoneSmell.Count != 0) //if chog is within proximity of a plant
        {
            float pheromoneStrength = 1;

            //Work out which plant is closest and run inputs on that plant
            try
            {
                foreach (GameObject phero in pheromoneSmell)
                {
                    if (phero.GetComponent<PheromoneScript>().GetStrength(gameObject) > pheromoneStrength)
                    {
                        pheromoneStrength = phero.GetComponent<PheromoneScript>().GetStrength(gameObject);
                        inputNodes["pheromoneAngle"].setValue(Mathf.Deg2Rad * Vector2.SignedAngle(new Vector2(me.x, me.y), new Vector2(phero.transform.position.x - transform.position.x, phero.transform.position.y - transform.position.y)));
                    }
                }
            }
            catch
            {
                pheromoneSmell.Clear();
            }

        }
    }

    public void HandleMovement()
    {
        //speed - clamp to 0.5 max speed, max speed
        float newSpeed = stats["speed"] + outputNodes["acceleration"].getRawValue() * Time.deltaTime;
        stats["speed"] = Mathf.Max(geneList["maxSpeed"] / 2f, Mathf.Min(geneList["maxSpeed"], newSpeed));


        if (float.IsNaN(stats["speed"]))
        {
            stats["speed"] = geneList["maxSpeed"];
            Debug.Log("Had to fix a NaN speed :(");
        }

        //angle - clamp to -pi, pi
        float newAngle = stats["angle"] + outputNodes["turn"].getRawValue() * Time.deltaTime;

        if (newAngle > Mathf.PI) { newAngle -= 2 * Mathf.PI; }
        if (newAngle < -Mathf.PI) { newAngle += 2 * Mathf.PI; }
        stats["angle"] = newAngle;


        //rotate chog to match angle of movement, and move forward
        me.x = Mathf.Cos(stats["angle"]); me.y = Mathf.Sin(stats["angle"]);

        //metabolism
        float berryModifier = 1;

        if (stats["berryTimer"] >= 0)
        {
            berryModifier = 0.33f;
        }

        // rotation
        var rotation = Quaternion.AngleAxis(stats["angle"] * Mathf.Rad2Deg, Vector3.forward);
        transform.rotation = rotation;

        if (outputNodes["sprint"].getActivationValue() > 0)
        {
            transform.Translate(Vector3.right * stats["speed"] * Time.deltaTime * (1 + outputNodes["sprint"].getActivationValue()));
            stats["energy"] -= geneList["metabolism"] * Time.deltaTime * ((stats["speed"] + geneList["maxSpeed"]) / 2f) * Mathf.Pow(0.5f + outputNodes["sprint"].getActivationValue(), 2) * berryModifier;
        }
        else
        {
            transform.Translate(Vector3.right * stats["speed"] * Time.deltaTime);
            stats["energy"] -= geneList["metabolism"] * Time.deltaTime * ((stats["speed"] + geneList["maxSpeed"]) / 2f) * berryModifier;
        }
    }

    public void InteractWithWorldObjects()
    {
        //interactions with food
        if (plantTouch != null && outputNodes["eat"].getRawValue() > 0)
        {
            stats["plantsEaten"]++;
            stats["timeSinceLastMeal"] = 0;

            //Take damage if diet is not compatable
            if (geneList["diet"] > 0.7f)
            {
                stats["health"] -= plantTouch.gameObject.GetComponent<plantScript>().consume();
            }
            else
            {
                float newEnergy = stats["energy"] + plantTouch.gameObject.GetComponent<plantScript>().consume() * (1 - geneList["diet"]);//maximum when diet is 0
                stats["energy"] = Mathf.Min(newEnergy, geneList["maxEnergy"]);
            }
            plantSee.Remove(plantTouch);
            plantTouch = null;

        }

        if (meatTouch != null && outputNodes["eat"].getRawValue() > 0)
        {
            stats["timeSinceLastMeal"] = 0;
            stats["meatEaten"]++;

            //Take damage if diet is not compatable
            if (geneList["diet"] < 0.3f)
            {
                stats["health"] -= meatTouch.gameObject.GetComponent<MeatScript>().consume();
            }
            else
            {
                float newEnergy = stats["energy"] + meatTouch.gameObject.GetComponent<MeatScript>().consume() * geneList["diet"];//maximum when diet is 0
                stats["energy"] = Mathf.Min(newEnergy, geneList["maxEnergy"]);
            }
            meatSee.Remove(meatTouch);
            meatTouch = null;

        }

        if (berryTouch != null && outputNodes["eat"].getRawValue() > 0)
        {
            stats["berryTimer"] += berryTouch.gameObject.GetComponent<BerryScript>().consume();

            berrySee.Remove(berryTouch);
            berryTouch = null;

        }

        //decide whether to attack a chog
        if (chogTouch != null && outputNodes["attack"].getActivationValue() * geneList["aggression"] > 1f)
        {
            stats["chogsAttacked"]++;
            stats["energy"] += chogTouch.GetComponent<ChogScript>().Attack(outputNodes["attack"].getActivationValue() * geneList["strength"] * geneList["aggression"]);
            stats["energy"] = Mathf.Min(stats["energy"], geneList["maxEnergy"]); //dont let energy go over max

        }

        //decide whether to breed with a chog
        if (localChog != null)
        {
            //breed with a nearby and consenting chog
            if (localChog.GetComponent<ChogScript>().IsBreedable() && willingToBreed)
            {
                //Debug.Log("Hooking up with a stranger....");
                Reproduce(localChog);
                //chogsCanFreelyBreedWith.Add((int)localChog.GetComponent<ChogScript>().stats["familyTag"]);
            }
        }

        //if (friendlyChog != null && willingToBreed)
        //{
        //    //Debug.Log("Familiar face....");
        //    Reproduce(friendlyChog);
        //}

        if (willingToBreed && stats["familyNum"] + stats["children"] < maxSoloKids && allowSoloKids)
        {
            Reproduce(gameObject);
        }
    }

    public void HandleReproduction()
    {


        //Check if available to reproduce
        if (stats["energy"] / geneList["maxEnergy"] > 0.4f && outputNodes["reproduce"].getRawValue() > 0 && stats["reproduceTimer"] <= 0)
        {
            willingToBreed = true;
            vision.material.color = Color.green;
        }
        else
        {
            willingToBreed = false;
            vision.material.color = Color.white;
        }
    }

    public float Attack(float damage)
    {
        stats["health"] -= damage;
        return damage * geneList["metabolism"];
    }

    private void Reproduce(GameObject chogToBreedWith)
    {
        //called on dad chog

        stats["reproduceTimer"] = reproduceConstant * geneList["eggRate"];
        stats["energy"] *= 0.6f;
        stats["children"]++;
        GameObject baby = Instantiate(chog, transform.position, Quaternion.identity);
        god.numChogs++;

        baby.GetComponent<ChogScript>().SexualReproduction(transform.gameObject, chogToBreedWith);
        willingToBreed = false;
        

    }

    private void SexualReproduction(GameObject d, GameObject m)
    {
        //dad chog calls this on child chog

        ChogScript dad = d.GetComponent<ChogScript>();
        ChogScript mum = m.GetComponent<ChogScript>();

        //Assin some stuff for the other chog
        mum.stats["reproduceTimer"] = reproduceConstant * mum.geneList["eggRate"];
        //mum.chogsCanFreelyBreedWith.Add((int)dad.stats["familyTag"]);
        mum.stats["energy"] *= 0.6f;
        mum.stats["children"]++;


        Debug.Log(string.Format("A BABY IS BORN FROM {0} and {1}", dad.stats["familyTag"], mum.stats["familyTag"]));
        rend = GetComponent<Renderer>();
        god = GameObject.Find("CreateObjects").GetComponent<CreateObjects>();

        geneList = new Dictionary<string, float>();

        hidden1 = new List<Node>();
        hidden2 = new List<Node>();

        //Add all genes, average of mum and dads values
        foreach (KeyValuePair<string, float> gene in dad.geneList)
        {
            geneList.Add(gene.Key, Random.Range(gene.Value, mum.geneList[gene.Key]));
        }

        //REBUILD BRAIN FROM PARENTS

        buildInOutBrain();

        for (int i = 0; i < numInternal; i++)
        {
            hidden1.Add(new Node(Random.Range(dad.hidden1[i].getOffset(), mum.hidden1[i].getOffset()), true ));
            hidden2.Add(new Node(Random.Range(dad.hidden2[i].getOffset(), mum.hidden2[i].getOffset()), true ));
        }



        god.ChogBorn((int)(dad.stats["familyTag"]), (int)(mum.stats["familyTag"]), gameObject);


        transform.localScale = new Vector3(1.5f, 1, 1) * geneList["size"];
        transform.GetChild(0).localScale = new Vector3(0.66f, 1, 1) / geneList["size"];

        stats = new Dictionary<string, float>
        {
            { "speed", geneList["maxSpeed"] },
            { "energy", dad.stats["energy"] }, //bigger size = more
            { "health", geneList["maxHealth"] }, //energy consumed per update just to stay alive
            { "plantsEaten", 0 },
            { "meatEaten", 0 },
            { "chogsAttacked", 0},
            { "children", 0 },
            { "mutations", dad.stats["mutations"] },
            { "timeSinceLastMeal",0},
            { "timeAlive",0 },
            { "reproduceTimer", reproduceConstant * geneList["eggRate"] },
            { "berryTimer", 0 },
            { "angle", Random.Range(-Mathf.PI, Mathf.PI)},
            { "familyTag", dad.stats["familyTag"] },
            { "mumTag", mum.stats["familyTag"] },
            { "familyNum", god.families[(int)dad.stats["familyTag"]].GetTotal() },
            { "generation", dad.stats["generation"]+1 },
            { "age", dad.stats["age"] },
        };

        inputKeys = new List<string>(inputNodes.Keys);
        outputKeys = new List<string>(outputNodes.Keys);


        //copy input -> 1st from parents
        foreach (string key in inputKeys)
        {
            for (int i = 0; i < numInternal; i++)
            {
                float weight = Random.Range(dad.inputNodes[key].outputWeights[i], mum.inputNodes[key].outputWeights[i]);
                inputNodes[key].addConnection(hidden1[i], weight);
            }
        }

        //Copy turn to plant gene
        if (helpChogs)
        {
            inputNodes["plantAngle"].addConnection(outputNodes["turn"], dad.inputNodes["plantAngle"].outputWeights[dad.inputNodes["plantAngle"].outputWeights.Count-1]);
        }
        

        //copy 1st->2nd from parents
        for (int i = 0; i < numInternal; i++)
        {
            for (int j = 0; j < numInternal; j++)
            {
                float weight = Random.Range(dad.hidden1[i].outputWeights[j], mum.hidden1[i].outputWeights[j]);
                hidden1[i].addConnection(hidden2[j], weight);
            }
        }

        //copy 2nd-> output from parents
        for (int i = 0; i < numInternal; i++)
        {
            int j = 0;
            foreach (string key in outputKeys)
            {
                float weight = Random.Range(dad.hidden2[i].outputWeights[j], mum.hidden2[i].outputWeights[j]);
                hidden2[i].addConnection(outputNodes[key], weight);
                j++;
            }
        }


        float dice = (int)Random.Range(1, 100) / 100f;
        if (dice < geneList["mutationChance"])
        {
            Mutate(); Mutate();
        }

        me = new Vector3(); me.x = Mathf.Cos(0); me.y = Mathf.Sin(0); me.z = 0;

        transform.name = string.Format("Chog {0}/{1}", stats["familyTag"], stats["familyNum"]);

        //Now its time to use these genes to edit the chogs collider values and such

        GetComponentInChildren<CircleCollider2D>().radius = geneList["viewDistance"];
        transform.GetChild(0).GetChild(0).localScale = new Vector3(12, 12, 1) * geneList["viewDistance"];

        WorkOutAncestory(dad, mum);
        
    }

    private void buildInOutBrain()
    {

        inputNodes = new Dictionary<string, Node>
        {
            { "random", new Node(0, true) },
            { "plantAngle", new Node(0, false) },
            { "plantProximity", new Node(0, false) },
            { "meatAngle", new Node(0, false) },
            { "meatProximity", new Node(0, false) },
            { "chogAngle", new Node(0, false) },
            { "chogProximity", new Node(0, false) },
            { "chogDirection", new Node(0, false) },
            { "berryProximity", new Node(0, false) },
            { "berryAngle", new Node(0, false) },
            { "pheromoneAngle", new Node(0, false) },
            { "healthLevel", new Node(0, false) },
            { "energyLevel", new Node(0, false) },
        };

        outputNodes = new Dictionary<string, Node>
        {
            { "turn", new Node(0, false) },
            { "eat", new Node(0, false) },
            { "acceleration", new Node(0, false) },
            { "attack", new Node(-1f, false) },
            { "sprint", new Node(-1f, true) },
            { "reproduce", new Node(0, false) },
            { "pheromone", new Node(-1f, true) },
        };
    }

    private void WorkOutAncestory(ChogScript dad, ChogScript mum)
    {
        ancestorList = new Dictionary<int, float>();

        Dictionary<int, float> temp = new Dictionary<int, float>();

        foreach (KeyValuePair<int, float> dadFam in dad.ancestorList)
        {
            temp.Add(dadFam.Key, dadFam.Value/2f);
        }
        foreach (KeyValuePair<int, float> mumFam in mum.ancestorList)
        {
            if (temp.ContainsKey(mumFam.Key))
            {
                temp[mumFam.Key] += mumFam.Value/2f;
            }
            else
            {
                temp.Add(mumFam.Key, mumFam.Value/2f);

            }
        }

        foreach(KeyValuePair<int,float> f in from entry in temp where entry.Value > 0.002f orderby entry.Value descending select entry)
        {
            ancestorList.Add(f.Key,f.Value);
        }
        totalAncestors = dad.totalAncestors + mum.totalAncestors;
    }

    //public void GiveGenes(GameObject d)
    //{
    //    //dad chog calls this on child chog

    //    ChogScript dad = d.GetComponent<ChogScript>();

    //    rend = GetComponent<Renderer>();
    //    god = GameObject.Find("CreateObjects").GetComponent<CreateObjects>();

    //    geneList = new Dictionary<string, float>();

    //    hidden1 = new List<Node>();
    //    hidden2 = new List<Node>();

    //    //Add all genes, average of mum and dads values
    //    foreach (KeyValuePair<string, float> gene in dad.geneList)
    //    {
    //        geneList.Add(gene.Key, gene.Value);
    //    }

    //    //REBUILD BRAIN FROM PARENTS

    //    buildInOutBrain();

    //    for (int i = 0; i < numInternal; i++)
    //    {
    //        hidden1.Add(new Node(dad.hidden1[i].getOffset(), true));
    //        hidden2.Add(new Node(dad.hidden2[i].getOffset(), true));
    //    }



    //    god.families[(int)(dad.stats["familyTag"])].ChogBorn(gameObject);

    //    transform.localScale = new Vector3(1.5f, 1, 1) * geneList["size"];
    //    transform.GetChild(0).localScale = new Vector3(0.66f, 1, 1) / geneList["size"];

    //    stats = new Dictionary<string, float>
    //    {
    //        { "speed", geneList["maxSpeed"] },
    //        { "energy", dad.stats["energy"] }, //bigger size = more
    //        { "health", geneList["maxHealth"] }, //energy consumed per update just to stay alive
    //        { "plantsEaten", 0 },
    //        { "meatEaten", 0 },
    //        { "chogsAttacked", 0},
    //        { "children", 0 },
    //        { "mutations", dad.stats["mutations"] },
    //        { "timeSinceLastMeal",0},
    //        { "timeAlive",0 },
    //        { "reproduceTimer", reproduceConstant * geneList["eggRate"] },
    //        { "berryTimer", 0 },
    //        { "angle", Random.Range(-Mathf.PI, Mathf.PI)},
    //        { "familyTag", dad.stats["familyTag"] },
    //        { "familyNum", god.families[(int)dad.stats["familyTag"]].GetTotal() },
    //        { "generation", dad.stats["generation"]+1 },
    //    };

    //    inputKeys = new List<string>(inputNodes.Keys);
    //    outputKeys = new List<string>(outputNodes.Keys);


    //    //copy input -> 1st from parents
    //    foreach (string key in inputKeys)
    //    {
    //        for (int i = 0; i < numInternal; i++)
    //        {
    //            float weight = dad.inputNodes[key].outputWeights[i];
    //            inputNodes[key].addConnection(hidden1[i], weight);
    //        }
    //    }

    //    //copy 1st->2nd from parents
    //    for (int i = 0; i < numInternal; i++)
    //    {
    //        for (int j = 0; j < numInternal; j++)
    //        {
    //            float weight = dad.hidden1[i].outputWeights[j];
    //            hidden1[i].addConnection(hidden2[j], weight);
    //        }
    //    }

    //    //copy 2nd-> output from parents
    //    for (int i = 0; i < numInternal; i++)
    //    {
    //        int j = 0;
    //        foreach (string key in outputKeys)
    //        {
    //            float weight = dad.hidden2[i].outputWeights[j];
    //            hidden2[i].addConnection(outputNodes[key], weight);
    //            j++;
    //        }
    //    }


    //    float dice = (int)Random.Range(1, 100) / 100f;
    //    if (dice < geneList["mutationChance"])
    //    {
    //        Mutate(); Mutate();
    //        if (dice < geneList["mutationChance"]/2)
    //        {
    //            Mutate(); Mutate();
    //        }
    //    }

    //    me = new Vector3(); me.x = Mathf.Cos(0); me.y = Mathf.Sin(0); me.z = 0;

    //    transform.name = string.Format("Chog {0}/{1}", stats["familyTag"], stats["familyNum"]);

    //    //Now its time to use these genes to edit the chogs collider values and such

    //    GetComponentInChildren<CircleCollider2D>().radius = geneList["viewDistance"];
    //    transform.GetChild(0).GetChild(0).localScale = new Vector3(12, 12, 1) * geneList["viewDistance"];

    //    WorkOutAncestory(dad, dad);

    //}
    

    private void Mutate()
    {
        int dice;

        int geneMutateChance = 4, weightChangeChance = 2, offsetChance = 6, colourChance = 20;


        dice = Random.Range(0, geneMutateChance);

        //MUTATE GENE
        if (dice == 0)
        {
            stats["mutations"]++;
            string key = geneList.ElementAt(Random.Range(0, geneList.Count - 1)).Key;
            geneList[key] *= 1 + Random.Range(-geneList["mutationSize"], geneList["mutationSize"]);

            if (geneList["diet"] > 1) { geneList["diet"] = 1; }
            if (geneList["diet"] < 0) { geneList["diet"] = 0; }
        }

        dice = Random.Range(0, weightChangeChance);

        //CHANGE WEIGHT OF EXISTING connection
        if (dice == 0)
        {
            stats["mutations"]++;
            switch (Random.Range(0, 3))
            {
                case 0:
                    inputNodes[inputKeys[Random.Range(0, inputKeys.Count)]].mutateWeight();
                    break;

                case 1:
                    hidden1[Random.Range(0, numInternal)].mutateWeight();
                    break;

                case 2:
                    hidden2[Random.Range(0, numInternal)].mutateWeight();
                    break;

                default:
                    break;
            }

        }


        dice = Random.Range(0, colourChance);

        //Change colour slight;y
        if (dice == 0)
        {
            stats["mutations"]++;
            float r = rend.material.color.r + Random.Range(-0.1f, 0.1f);
            float g = rend.material.color.g + Random.Range(-0.1f, 0.1f);
            float b = rend.material.color.b + Random.Range(-0.1f, 0.1f);
            rend.material.color = new Color(r, g, b);
        }

        dice = Random.Range(0, offsetChance);

        //Change offset of output node slight;y
        if (dice == 0)
        {
            stats["mutations"]++;
            switch (Random.Range(0, 3))
            {
                case 0:
                    inputNodes[inputKeys[Random.Range(0, inputKeys.Count)]].mutateOffset();
                    break;

                case 1:
                    hidden1[Random.Range(0, numInternal)].mutateOffset();
                    break;

                case 2:
                    hidden2[Random.Range(0, numInternal)].mutateOffset();
                    break;

                default:
                    break;
            }

        }

    }


    public bool IsBreedable()
    {
        return willingToBreed;
    }



    private void Die()
    {
        GameObject m = Instantiate(meat, transform.position, Quaternion.identity);
        m.GetComponent<MeatScript>().SetEnergy((geneList["maxEnergy"] * (stats["plantsEaten"] + stats["meatEaten"]))/ 40);
        m.transform.parent = GameObject.FindGameObjectWithTag("Meats").transform;
        god.numChogs--;
        god.families[(int)stats["familyTag"]].ChogDied(gameObject);

        if ((int)stats["familyTag"] != (int)stats["mumTag"])
        {
            god.families[(int)stats["mumTag"]].ChogDied(gameObject);
        }

        Destroy(gameObject);
    }


    private void UpdateEachSecond()
    {
        stats["timeAlive"]++;
        stats["reproduceTimer"]--;
        stats["timeSinceLastMeal"]++;
        inputNodes["random"].setValue( Random.Range(-2f,2f));

        if (stats["berryTimer"] >= 0)
        {
            stats["berryTimer"]--;
        }

        if (outputNodes["pheromone"].getActivationValue() > 0)
        {
            EmitPheromone();
        }
        
    }

    private void EmitPheromone()
    {
        GameObject p = Instantiate(pheromone, transform.position - me / 3f, Quaternion.identity);
        p.GetComponent<PheromoneScript>().SetStrength(geneList["pheromoneStrength"] * geneList["size"], gameObject);
        p.transform.parent = pheromones.transform;
        p.name = "Phero-" + name;

    }

    public string PrintInputInfo()
    {
        string s = "";

        foreach (string key in inputKeys.AsEnumerable().Reverse())
        {
            s += string.Format("{1} : {0}\n", key, inputNodes[key].getRawValue().ToString("F"));
        }

        return s;
    }

    public string PrintBrainInfo()
    {
        string graph = "";
        //draw ascii diagram
        foreach (string key in inputKeys)
        {
            if (inputNodes[key].getRawValue() > 0) { graph += "<color=#00FF00>0</color>"; }
            if (inputNodes[key].getRawValue() == 0) { graph += "<color=#777777>0</color>"; }
            if (inputNodes[key].getRawValue() < 0) { graph += "<color=#FF0000>0</color>"; }
        }
        graph += "\n  ";
        for (int i = 0; i < numInternal; i++)
        {
            if (hidden1[i].getActivationValue() > 0) { graph += "<color=#00FF00>0</color>"; }
            if (hidden1[i].getActivationValue() == 0) { graph += "<color=#777777>0</color>"; }
            if (hidden1[i].getActivationValue() < 0) { graph += "<color=#FF0000>0</color>"; }
        }
        graph += "\n  ";
        for (int i = 0; i < numInternal; i++)
        {
            if (hidden2[i].getActivationValue() > 0) { graph += "<color=#00FF00>0</color>"; }
            if (hidden2[i].getActivationValue() == 0) { graph += "<color=#777777>0</color>"; }
            if (hidden2[i].getActivationValue() < 0) { graph += "<color=#FF0000>0</color>"; }
        }
        graph += "\n   ";
        foreach (string key in outputKeys)
        {
            if (outputNodes[key].getRawValue() > 0) { graph += "<color=#00FF00>0</color>"; }
            if (outputNodes[key].getRawValue() == 0) { graph += "<color=#777777>0</color>"; }
            if (outputNodes[key].getRawValue() < 0) { graph += "<color=#FF0000>0</color>"; }
        }

        return graph;
    }

    public string PrintOutputInfo()
    {
        string s = "";

        foreach (string key in outputKeys.AsEnumerable().Reverse())
        {
            s += string.Format("{0} : {1}\n", key, outputNodes[key].getRawValue().ToString("F"));
        }

        return s;
    }

    public string PrintStatInfo()
    {
        string s = "";

        foreach (KeyValuePair<string, float> gene in geneList)
        {
            s += string.Format("{0,15} : {1,8:F6} \n", gene.Key, gene.Value.ToString("F"));
        }
        s += "\nStats:\n";
        foreach (KeyValuePair<string, float> stat in stats)
        {
            s += string.Format("{0,-15}: {1,-8}\n", stat.Key, stat.Value.ToString("F"));
        }
        s += "\nAncestry:\n";
        foreach (KeyValuePair<int, float> fam in ancestorList)
        {
            s += string.Format("{0,-15}: {1,5:P1}\n", fam.Key,fam.Value);
        }
        return s;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {

        if (coll.gameObject.tag == "Plant")
        {
            plantTouch = coll.gameObject;
        }
        if (coll.gameObject.tag == "Chog")
        {
            chogTouch = coll.gameObject;
        }
        if (coll.gameObject.tag == "Meat")
        {
            meatTouch = coll.gameObject;
        }
        if (coll.gameObject.tag == "Berry")
        {
            berryTouch = coll.gameObject;
        }
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        if (coll.gameObject == plantTouch)
        {
            plantTouch = null;
        }
        if (coll.gameObject == chogTouch)
        {
            chogTouch = null;
        }
        if (coll.gameObject == meatTouch)
        {
            meatTouch = null;
        }
        if (coll.gameObject == berryTouch)
        {
            berryTouch = null;
        }
    }

    void OnMouseOver()
    {
        mouseOver = true;
    }

    void OnMouseExit()
    {
        mouseOver = false;
    }
}
