using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButton : MonoBehaviour
{
    public GameObject ThingToToggle;
   public void Toggle()
    {
        ThingToToggle.SetActive(!ThingToToggle.activeInHierarchy);
    }
}
