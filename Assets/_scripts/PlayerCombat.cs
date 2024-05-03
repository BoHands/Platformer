using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Vector2 attackOffSet;
    [SerializeField] float attackRadius, attackCD, attackKnockback;
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
        if (Input.GetMouseButtonDown(0))
        {
            Attack();
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
        Instantiate(swipePrefab, attackPos, Quaternion.identity);
        Collider2D[] hits = Physics2D.OverlapCircleAll((Vector2)transform.position + (attackOffSet * playMove.lastDirection), attackRadius);

        foreach (Collider2D item in hits)
        {
            if (item.gameObject.tag == "Enemy")
            {
                print("Bop");
                playMove.KnockBackPlayer(Vector2.left * playMove.lastDirection, attackKnockback);
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
