using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int newPosition;
    public PlayerConts player;
    [SerializeField] GameObject playerPrefab, playerCamera;

    // Start is called before the first frame update
    void Awake()
    {
        if (FindObjectOfType<PlayerConts>() != null)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this);
        player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerConts>();
        GameObject camT = Instantiate(playerCamera, Vector3.forward * -10, Quaternion.identity);
        camT.GetComponent<CameraMan>().targetLocattion = player.transform.GetChild(3);
        DontDestroyOnLoad(player.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
