using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public List<Collectable> inventory;
    public int count;
    public GameObject exitMaze;

    public void Add(Collectable obj) {
        inventory.Add(obj);
        exitMaze.SetActive(true);
        count = 0;
        GetComponentInChildren<Text>().text = "INVENTORY: ";
        foreach (Collectable collectable in inventory) {
            GetComponentInChildren<Text>().text += collectable.collectableName + ", ";
            count++;

            if (count==8){
                Debug.Log("All Obj Collected");
                exitMaze.SetActive(false);
                Debug.Log("Exit is open!");
        }
        }

        

    }
}
