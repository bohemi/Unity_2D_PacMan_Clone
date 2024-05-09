using System.Collections;
using UnityEngine;

public class Consumables : MonoBehaviour
{
    [SerializeField] GameObject pacManObj;
    public static int updatePoints;

    private void Start()
    {
        updatePoints = 0;
        pacManObj = GameObject.Find("Pac-Man");
        if (pacManObj == null)
            print("Consumable Error");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            updatePoints++;
            Destroy(gameObject);
        }
    }
}
