using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Vector2 attackOffSet;
    [SerializeField] float attackRadius, attackCD, attackKnockback, aerialMod;
    [SerializeField] float pogoY, pogoX;
    public GameObject swipePrefab;
    PlayerConts playMove;
    // Start is called before the first frame update
    void Start()
    {
        playMove = GetComponent<PlayerConts>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && playMove.IsGrounded())
        {
            Attack();
        }
        else if (Input.GetMouseButtonDown(0) && !playMove.IsGrounded())
        {
            AerialAttack();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + attackOffSet, attackRadius);
    }

    bool attacked;
    void Attack()
    {
        if (attacked)
        {
            return;
        }
        attacked = true;
        Vector2 attackPos = (Vector2)transform.position + (attackOffSet * playMove.lastDirection);
        if (Input.GetAxisRaw("Vertical") > 0)
        {
            attackPos.y += aerialMod;
            if (Input.GetAxisRaw("Horizontal") < 0.2f && Input.GetAxisRaw("Horizontal") > -0.2F)  
            {
                attackPos.x = transform.position.x;
            }
        }
        Instantiate(swipePrefab, attackPos, Quaternion.identity).GetComponent<Transform>().parent = transform;
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position + (attackOffSet * playMove.lastDirection), attackRadius);

        foreach (Collider2D item in hits)
        {
            if (item.gameObject.tag == "Enemy")
            {
                print("Bop");
                playMove.KnockBackPlayer(Vector2.left * playMove.lastDirection, attackKnockback);

                if (item.gameObject.GetComponent<SideToSide>() != null)
                {
                    item.gameObject.GetComponent<SideToSide>().TakeHit(1, transform);
                }

                break;
            }
        }
        Invoke("ReloadAttack", attackCD);
    }

    void AerialAttack()
    {
        if (attacked)
        {
            return;
        }
        attacked = true;
        Vector2 attackPos = (Vector2)transform.position + (attackOffSet * playMove.lastDirection);
        if (Input.GetAxisRaw("Vertical") > 0.2F)
        {
            attackPos.y += aerialMod;
            if (Input.GetAxisRaw("Horizontal") < 0.2f && Input.GetAxisRaw("Horizontal") > -0.2F)
            {
                attackPos.x = transform.position.x;
            }
        }
        else if (Input.GetAxisRaw("Vertical") < -0.2F)
        {
            attackPos.y -= aerialMod;
            if (Input.GetAxisRaw("Horizontal") < 0.2f && Input.GetAxisRaw("Horizontal") > -0.2F)
            {
                attackPos.x = transform.position.x;
            }

        }
        Instantiate(swipePrefab, attackPos, Quaternion.identity).GetComponent<Transform>().parent = transform;
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPos, attackRadius);

        foreach (Collider2D item in hits)
        {
            if (item.gameObject.tag == "Enemy")
            {
                print("Bop");
                if (Input.GetAxisRaw("Vertical") < -0.2F)
                {
                    Vector2 pogo = Vector2.up * pogoY;
                    pogo.x = pogoX * (transform.position.x - item.transform.position.x);
                    playMove.PoGoPlayer(pogo);
                }

                if (item.gameObject.GetComponent<SideToSide>() != null)
                {
                    item.gameObject.GetComponent<SideToSide>().TakeHit(1, transform);
                }
                //playMove.KnockBackPlayer(Vector2.left * playMove.lastDirection, attackKnockback);
                break;
            }
        }
        Invoke("ReloadAttack", attackCD);
    }

    void ReloadAttack()
    {
        attacked = false;
    }
}
