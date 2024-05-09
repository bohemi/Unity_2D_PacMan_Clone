using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClydeTarget : MonoBehaviour
{
    [SerializeField] Transform pacmanTransform;
    [SerializeField] Transform clydeScatterLocation;
    [SerializeField] Transform target;

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, pacmanTransform.position);
        if (distance <= 8.0f)
        {
            target.position = clydeScatterLocation.position;
        }
        else
            target.position = pacmanTransform.position;
    }
}
