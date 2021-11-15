using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    public List<Collectable> inventory;

    public void Add(Collectable obj) {
        inventory.Add(obj);

        GetComponentInChildren<Text>().text = "Inventory: ";
        foreach (Collectable collectable in inventory) {
            GetComponentInChildren<Text>().text += collectable.collectableName + ", ";
        }
    }
}
