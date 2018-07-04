using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSwitch : MonoBehaviour {

	public void ToggleMainMenu(bool toggle)
    {
        gameObject.SetActive(toggle);
    }
}
