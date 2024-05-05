using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExit : MonoBehaviour
{
    public int entryPosition;
    public string nextScene;
    GameManager gameMan;
    // Start is called before the first frame update
    void Start()
    {
        gameMan = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerConts>() != null)
        {
            gameMan.newPosition = entryPosition;
            SceneManager.LoadScene(nextScene);
        }
    }
}
