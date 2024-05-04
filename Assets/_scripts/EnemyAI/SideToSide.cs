using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideToSide : MonoBehaviour
{
    [SerializeField] float speed, lowX, highX, knockback;
    [SerializeField] Vector2 maxVelo;
    Rigidbody2D rb;
    public bool movingLeft;
    bool stunned;
    [SerializeField] int hp, maxHp;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > highX)
        {
            movingLeft = true;
        }
        else if (transform.position.x < lowX)
        {
            movingLeft = false;
        }

        HandleMovement();
    }

    void HandleMovement()
    {
        if (stunned)
        {
            return;
        }
        float dirMult = movingLeft ? -1 : 1;
        rb.AddForce(Vector3.right * speed * Time.deltaTime * dirMult);
        Vector2 newVelo = rb.velocity;
        newVelo.x = Mathf.Clamp(newVelo.x, -maxVelo.x, maxVelo.x);
        newVelo.y = Mathf.Clamp(newVelo.y, -maxVelo.x, maxVelo.y);
        rb.velocity = newVelo;
    }

    public void TakeHit(int damage, Transform hitter)
    {
        hp -= damage;
        if (hp < 0)
        {
            Destroy(gameObject);
        }
        rb.AddForce((transform.position - hitter.position).normalized * knockback, ForceMode2D.Impulse);
        stunned = true;
        Invoke("StopStun", 1);
    }

    void StopStun()
    {
        stunned = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerConts>() != null)
        {
            StartCoroutine(collision.gameObject.GetComponent<PlayerConts>().TakeHit(transform));
        }
    }
}
