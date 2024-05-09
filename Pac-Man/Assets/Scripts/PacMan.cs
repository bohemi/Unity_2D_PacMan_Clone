using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PacMan : MonoBehaviour
{
    // if movement is causing undefined behaviour then check if box is correct sized
    [Header("BoxCast Settings")]
    [SerializeField] Vector2 boxSize; // 0.83, 0.83
    [SerializeField] LayerMask mask;
    [SerializeField] float boxDistance; // 1
    [SerializeField] float speed;

    public static bool onPowerMode;
    public static int sendEnemyNumber;
    public static bool hasControl;

    Vector3 currentDirection, queueDirection;
    bool leftQueue, rightQueue, upQueue, downQueue;
    Rigidbody2D rb;
    public AudioManager audioManager;

    private void Start()
    {
        sendEnemyNumber = -1;
        onPowerMode = false;
        hasControl = false;
        Invoke("PlayPacmanSound", 4.2f);
        rb = GetComponent<Rigidbody2D>();
        currentDirection = Vector3.zero;
    }

    void PlayPacmanSound()
    {
        hasControl = true;
    }

    private void Update()
    {
        if (!hasControl)
            return;

        RegularMovement();
        MakeQueueDirection();
    }

    private void FixedUpdate()
    {
        Vector3 translation = currentDirection * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.transform.position + translation);
    }

    void RegularMovement()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && MovableDirection(Vector3.left))
        {
            PacManRotation(0, false, false);
            currentDirection = Vector3.left;
            upQueue = false; downQueue = false; rightQueue = false;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !MovableDirection(Vector3.left))
        {
            upQueue = false; downQueue = false; rightQueue = false;
            leftQueue = true;
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow) && MovableDirection(Vector3.right))
        {
            PacManRotation(0, true, false);
            currentDirection = Vector3.right;
            leftQueue = false; upQueue = false; downQueue = false;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !MovableDirection(Vector3.right))
        {
            leftQueue = false; upQueue = false; downQueue = false;
            rightQueue = true;
        }

        else if (Input.GetKeyDown(KeyCode.UpArrow) && MovableDirection(Vector3.up))
        {
            PacManRotation(90, false, true);
            leftQueue = false; rightQueue = false; downQueue = false;
            currentDirection = Vector3.up;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) && !MovableDirection(Vector3.up))
        {
            leftQueue = false; rightQueue = false; downQueue = false;
            upQueue = true;
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow) && MovableDirection(Vector3.down))
        {
            PacManRotation(-90, false, true);
            leftQueue = false; rightQueue = false; upQueue = false;
            currentDirection = Vector3.down;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !MovableDirection(Vector3.down))
        {
            leftQueue = false; rightQueue = false; upQueue = false;
            downQueue = true;
        }
    }

    void MakeQueueDirection()
    {
        if (!upQueue && !downQueue && !leftQueue && !rightQueue) // nothing to check below then
            return;

        if (leftQueue && MovableDirection(Vector3.left))
        {
            leftQueue = false;
            PacManRotation(0, false, false);
            queueDirection = Vector3.left;
            currentDirection = queueDirection;
            queueDirection = Vector3.zero;
        }
        else if (rightQueue && MovableDirection(Vector3.right))
        {
            rightQueue = false;
            PacManRotation(0, true, false);
            queueDirection = Vector3.right;
            currentDirection = queueDirection;
            queueDirection = Vector3.zero;
        }
        else if (upQueue && MovableDirection(Vector3.up))
        {
            upQueue = false;
            PacManRotation(90, false, true);
            queueDirection = Vector3.up;
            currentDirection = queueDirection;
            queueDirection = Vector3.zero;
        }
        else if (downQueue && MovableDirection(Vector3.down))
        {
            downQueue = false;
            PacManRotation(-90, false, true);
            queueDirection = Vector3.down;
            currentDirection = queueDirection;
            queueDirection = Vector3.zero;
        }
    }

    void PacManRotation(float angle, bool isDirectionRight, bool isRotate)
    {
        Vector3 scale = transform.localScale;

        if (isRotate && !isDirectionRight)
        {
            if (scale == Vector3.one) { transform.rotation = Quaternion.Euler(Vector3.forward * angle); }
            else { transform.rotation = Quaternion.Euler(Vector3.forward * -angle); }
        }

        else if (!isRotate && isDirectionRight)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
            scale.Set(1, 1, 1);
            transform.localScale = scale;
        }
        else if(!isRotate && !isDirectionRight)
        {
            transform.rotation = Quaternion.Euler(Vector3.forward * angle);
            scale.Set(-1, 1, 1);
            transform.localScale = scale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Power"))
        {
            onPowerMode = true;
            Destroy(collision.gameObject);
        }

        if (collision.name == "Blinky")
        {
            sendEnemyNumber = 0;
        }
        if (collision.name == "Pinky")
        {
            sendEnemyNumber = 1;
        }
        if (collision.name == "Inky")
        {
            sendEnemyNumber = 2;
        }
        if (collision.name == "Clyde")
        {
            sendEnemyNumber = 3;
        }
    }

    bool MovableDirection(Vector3 direction)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxSize, 0, direction, boxDistance, mask);

        if (hit)
            return false;
        else
            return true;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawCube(transform.position + currentDirection * boxDistance, boxSize);
    //}
}