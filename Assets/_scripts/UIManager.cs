using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] Image[] hpPoints;
    [SerializeField] Color red, green;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HandleHPDisplay(int toDis)
    {
        for (int i = 0; i < hpPoints.Length; i++)
        {
            if (i >= toDis)
            {

            }
        }
    }
}
