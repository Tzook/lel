using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopingScenery : MonoBehaviour {

    Transform End;

    [SerializeField]
    float Speed = 1f;

    [SerializeField]
    List<string> LoopableSceneries = new List<string>();

    List<GameObject> LivingObjects = new List<GameObject>();

    private void Start()
    {
        GenerateNew();
    }

    private void Update()
    {
        if(LivingObjects.Count == 0)
        {
            return;
        }

        LoopableScenery tempLoopable;
        for(int i=0;i< LivingObjects.Count;i++)
        {
            LivingObjects[i].transform.position += -Vector3.right * Speed * Time.deltaTime;

            tempLoopable = LivingObjects[i].GetComponent<LoopableScenery>();

            if(i == LivingObjects.Count - 1 && tempLoopable.EndPoint.position.x < transform.position.x)
            {
                GenerateNew();
            }

            if (tempLoopable.EndPoint.position.x < End.position.x)
            {
                LivingObjects[i].gameObject.SetActive(false);
                LivingObjects.RemoveAt(i);
                i--;
            }
        }
    }

    void GenerateNew()
    {
        GameObject tempObj = ResourcesLoader.Instance.GetRecycledObject(LoopableSceneries[Random.Range(0, LoopableSceneries.Count)]);

        //GameObject tempObj = (GameObject)Instantiate(Resources.Load("Objects/" + LoopableSceneries[Random.Range(0, LoopableSceneries.Count)]));


        if (LivingObjects.Count == 0)
        {
            tempObj.transform.position = transform.position;
        }
        else
        {
            tempObj.transform.position = new Vector3(LivingObjects[LivingObjects.Count - 1].GetComponent<LoopableScenery>().EndPoint.position.x, LivingObjects[LivingObjects.Count - 1].transform.position.y, LivingObjects[LivingObjects.Count - 1].transform.position.z);
        }

        LivingObjects.Add(tempObj);
    }

    void OnValidate()
    {
        End = transform.Find("End");
    }




}
