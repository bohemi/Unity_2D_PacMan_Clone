using UnityEngine;

public class InkyTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] Transform pacmanTarget;
    [SerializeField] Vector3 pos;

    private void Update()
    {
        target.position = pacmanTarget.position + pos;
    }
}