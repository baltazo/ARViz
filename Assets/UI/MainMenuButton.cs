using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour {

    public MainController mainController;

    public Button button;
    //public Image iconImage;

    private ItemList item;
    private MainMenuScrollList menuScrollList;
    private GameObject mainControllerObject;

    private void Start()
    {
        mainControllerObject = GameObject.Find("MainController");
        mainController = mainControllerObject.GetComponent<MainController>();
    }

    public void Setup(ItemList currentItem, MainMenuScrollList scrollList)
    {
        item = currentItem;
        scrollList = menuScrollList;
        this.GetComponent<Image>().sprite = item.icon;
    }

    public void OnClick()
    {
        mainController.ItemChosen(item.prefab);
    }

}
