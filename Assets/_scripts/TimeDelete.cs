using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeDelete : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        print("Hi");
        Destroy(gameObject, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}