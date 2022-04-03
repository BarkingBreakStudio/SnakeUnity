using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitSystem : MonoBehaviour
{

    public Transform FruitPrefab;

    private Transform fruit;

    public static event System.Action FruitConsumed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!fruit)
        {
           
        }
    }

    public Vector3 GetFruitPosition()
    {
        return fruit.position;
    }

    public void DestroyFruit()
    {
        if (fruit)
        {
            Destroy(fruit.gameObject);
            fruit = null;
        }
    }


    public void ConsumeFruit(List<Vector2Int> possibleLocations)
    {
        if (fruit)
        {
            Destroy(fruit.gameObject);
            FruitConsumed?.Invoke();
        }


        Vector2Int spawnLocation = possibleLocations[Random.Range(0, possibleLocations.Count)];
        fruit = Instantiate<Transform>(FruitPrefab, this.transform);
        fruit.position = (Vector3.right * spawnLocation.x + Vector3.forward * spawnLocation.y);
    }


}
