using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScatterBehaviour : MonoBehaviour
{
    [Header("AI Behaviour Settings")]
    [SerializeField] Transform target;
    [SerializeField] public float delayPerDirection = 0.6f;
    [SerializeField] float dotRayDistance, wallRayDistance; // 1.5, 0.7
    [SerializeField] LayerMask DotLayer, wallLayer;
    [SerializeField] float Ghostspeed;
    public Vector3 currentDirection, nextDirection, previousDirection;
    public bool openRay = true; // close the dotLayer if we found the dotObj for delayDirection time

    Transform currentDot; // represents the ray hit
    RaycastHit2D dotRay, wallRay;
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentDot = null;
        openRay = true;
    }

    private void Update()
    {
        if (DidRayHitDot(transform.position, currentDirection) && openRay)
        {
            currentDot = dotRay.transform;
            StartCoroutine(SetNewDirection());
            openRay = false;
        }
    }

    private void FixedUpdate()
    {
        // ghost movement
        Vector3 translation = currentDirection * Ghostspeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.transform.position + translation);
    }

    IEnumerator SetNewDirection()
    {
        float dotDistance = Vector3.Distance(transform.position, currentDot.position);
        float dotReachTime = dotDistance / Ghostspeed;

        // for when to proceed below at exact time for changing direction
        yield return new WaitForSeconds(dotReachTime - 0.01f);

        float tempDistance = 0f;
        // we have reached the dotObject now change to the available direction by casting wallRay
        if (GetOpenDirection(transform.position, Vector3.up) && previousDirection != Vector3.up)
        {
            tempDistance = Vector3.Distance(new Vector3(currentDot.position.x, currentDot.position.y + 1.3f, 0), target.position);
            nextDirection = Vector3.up;
        }

        if (GetOpenDirection(transform.position, Vector3.left) && previousDirection != Vector3.left)
        {
            float leftCompare = Vector3.Distance(new Vector3(currentDot.position.x - 1.3f, currentDot.position.y, 0), target.position);
            if (tempDistance != 0f && leftCompare <= tempDistance)
            {
                tempDistance = leftCompare;
                nextDirection = Vector3.left;
            }
            else if (tempDistance == 0f)
            {
                tempDistance = leftCompare;
                nextDirection = Vector3.left;
            }
        }

        if (GetOpenDirection(transform.position, Vector3.down) && previousDirection != Vector3.down)
        {
            float downCompare = Vector3.Distance(new Vector3(currentDot.position.x, currentDot.position.y - 1.3f, 0), target.position);
            if (tempDistance != 0f && downCompare <= tempDistance)
            {
                tempDistance = downCompare;
                nextDirection = Vector3.down;
            }
            else if (tempDistance == 0f)
            {
                tempDistance = downCompare;
                nextDirection = Vector3.down;
            }
        }

        if (GetOpenDirection(transform.position, Vector3.right) && previousDirection != Vector3.right)
        {
            float rightCompare = Vector3.Distance(new Vector3(currentDot.position.x + 1.3f, currentDot.position.y, 0), target.position);
            if (tempDistance != 0f && rightCompare <= tempDistance)
            {
                tempDistance = rightCompare;
                nextDirection = Vector3.right;
            }
            else if (tempDistance == 0f)
            {
                tempDistance = rightCompare;
                nextDirection = Vector3.right;
            }
        }

        currentDirection = nextDirection;
        previousDirection = -currentDirection;
        nextDirection = Vector3.zero;

        yield return new WaitForSeconds(delayPerDirection);
        openRay = true;
    }

    bool DidRayHitDot(Vector3 origin, Vector3 direction)
    {
        dotRay = Physics2D.Raycast(origin, direction, dotRayDistance, DotLayer);

        if (dotRay)
            return true;
        else
            return false;
    }

    bool GetOpenDirection(Vector3 origin, Vector3 direction)
    {
        wallRay = Physics2D.Raycast(origin, direction, wallRayDistance, wallLayer);

        if (wallRay) // since it hits wall. we wont go there
            return false;
        else
            return true;
    }
}
