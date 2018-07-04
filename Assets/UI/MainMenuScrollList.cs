using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemList
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public int type;
    /* Type is an integer to fit the dropdown choice
     * 0 == Chairs
     * 1 == Tables
     * 2 == Beds */
}

public class MainMenuScrollList : MonoBehaviour {

    public List<ItemList> itemList;
    public Transform contentPanel;
    public ObjectPool objectPool;
    public Dropdown dropdownChoice;

	// Use this for initialization
	void Start () {
        RefreshDisplay();
	}

    public void RefreshDisplay()
    {
        AddButtons();
    }

    void AddButtons()
    {
        for(int i = 0; i < itemList.Count; i++)
        {
            ItemList item = itemList[i];
            if(dropdownChoice.value == item.type)
            {
                GameObject newButton = objectPool.GetObject();
                newButton.transform.SetParent(contentPanel);
                MainMenuButton menuButton = newButton.GetComponent<MainMenuButton>();
                menuButton.Setup(item, this);
            }
        }
    }

    public void SwitchButtons()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            objectPool.ReturnObject(toRemove);

        }
        AddButtons();
    }
	
}
