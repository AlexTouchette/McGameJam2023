using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplainationManager : MonoBehaviour
{
    public GameObject ExplainationUI;
    // Start is called before the first frame update

    public void OpenWindow()
    {
        ExplainationUI.SetActive(true);
    }

    public void CloseWindow()
    {
        ExplainationUI.SetActive(false);
    }
}
