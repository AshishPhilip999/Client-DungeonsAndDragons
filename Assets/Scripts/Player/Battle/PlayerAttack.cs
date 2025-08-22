using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private void Update()
    {
       if(Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //Vector3 direction = (clickPos - transform.position).normalized;

            attack(transform.position - new Vector3(0.26f, 0, 0), Vector2.left);
        }
    }

    public GameObject attack(Vector2 targetPosition, Vector2 direction)
    {
        RaycastHit2D hitObject = Physics2D.Raycast(targetPosition, direction, 0.1f);
        Debug.DrawRay(targetPosition, Vector2.left * hitObject.distance, Color.red);
        
        if (hitObject)
        {
            Damageable damageableObject = hitObject.collider.GetComponent<TileItem>();
            damageableObject.damage(10);
            Debug.Log("[PlayereAttack] hitObject: " + hitObject.collider.name + ":" + hitObject.collider.transform.position);
        }

        return null;
    }
}
