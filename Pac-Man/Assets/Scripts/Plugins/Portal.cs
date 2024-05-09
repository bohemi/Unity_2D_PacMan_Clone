using UnityEngine;

public class Portal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.position.x <= -1f)
            collision.transform.position = new Vector3(9f, 3.5f, 0);

        else if (collision.transform.position.x >= -1f)
            collision.transform.position = new Vector3(-8.5f, 3.5f, 0);
    }
}
