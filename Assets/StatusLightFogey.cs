using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusLightFogey : MonoBehaviour {
    public GameObject[] statusLightAlternatives;
    public void ShowColor(int idx)
    {
        for (var x = 0; x < statusLightAlternatives.Length; x++)
        {
            statusLightAlternatives[x].SetActive(x == idx);
        }
    }
    public void ShowColor(string name)
    {
        switch(name)
        {
            case "None": ShowColor(0);
                break;
            case "Red": ShowColor(1);
                break;
            case "Green": ShowColor(2);
                break;
            case "Blue": ShowColor(3);
                break;
            case "Cyan": ShowColor(4);
                break;
            case "Magenta": ShowColor(5);
                break;
            case "Yellow": ShowColor(6);
                break;
            case "White": ShowColor(7);
                break;
            default: ShowColor(0);
                break;

        }
        
    }
}
