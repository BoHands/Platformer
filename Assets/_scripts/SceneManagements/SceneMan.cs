using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMan : MonoBehaviour
{
    GameManager gameMan;
    PlayerConts player;
    [SerializeField] Vector2[] playerSpawnPoints;
    // Start is called before the first frame update
    void Start()
    {
        gameMan = FindObjectOfType<GameManager>();
        player = FindObjectOfType<PlayerConts>();

        player.transform.position = playerSpawnPoints[gameMan.newPosition];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
