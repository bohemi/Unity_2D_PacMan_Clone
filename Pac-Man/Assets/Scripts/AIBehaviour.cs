using System.Collections;
using UnityEngine;

public class AIBehaviour : MonoBehaviour
{
    public int testNumer = -1;
    public AudioManager AudioManager;
    public int consumedEnemyNumber = -1;
    [SerializeField] int consumedPoints;
    [SerializeField] float ghostOutSpeed;
    [SerializeField] float scatterTimeLeft;
    [SerializeField] bool[] ghostOut;
    [SerializeField] GameObject[] ghostObjs;
    [SerializeField] SpriteRenderer[] normalSprites;
    [SerializeField] SpriteRenderer[] scatterSprite;
    [SerializeField] bool[] sirenBools;

    public static bool powerMode = false;

    private void Awake()
    {

        ghostObjs = new GameObject[4] { GameObject.Find("Blinky"), GameObject.Find("Pinky"),
            GameObject.Find("Inky"), GameObject.Find("Clyde") };

        normalSprites = new SpriteRenderer[4]
        {
            GameObject.Find("BlinkyNormal").GetComponent<SpriteRenderer>(), GameObject.Find("PinkyNormal").GetComponent<SpriteRenderer>(),
            GameObject.Find("InkyNormal").GetComponent<SpriteRenderer>(), GameObject.Find("ClydeNormal").GetComponent<SpriteRenderer>()
        };

        scatterSprite = new SpriteRenderer[4]
        {
            GameObject.Find("BlinkyScatter").GetComponent<SpriteRenderer>(), GameObject.Find("PinkyScatter").GetComponent<SpriteRenderer>(),
            GameObject.Find("InkyScatter").GetComponent<SpriteRenderer>(), GameObject.Find("ClydeScatter").GetComponent<SpriteRenderer>()
        };

        ghostOut = new bool[] { false, false, false, false };
        sirenBools = new bool[] { false, false, false, false, false };

        for (int i = 0; i < 4; i++)
        {
            ghostObjs[i].GetComponent<GhostChase>().enabled = false;
            ghostObjs[i].GetComponent<ScatterBehaviour>().enabled = false;
            ghostObjs[i].GetComponent<CircleCollider2D>().isTrigger = true;

            ghostObjs[i].GetComponent<GhostChase>().currentDirection = Vector3.right;
            ghostObjs[i].GetComponent<GhostChase>().previousDirection = Vector3.left;

            ghostObjs[i].GetComponent<ScatterBehaviour>().currentDirection = Vector3.left;
            ghostObjs[i].GetComponent<ScatterBehaviour>().previousDirection = Vector3.right;
        }
    }

    private void Update()
    {
        if (!PacMan.hasControl)
            return;

        consumedPoints = Consumables.updatePoints;

        LeaveHouse(ghostOut[0], ghostOut[1], ghostOut[2], ghostOut[3]);

        if (PacMan.onPowerMode)
        {
            StartCoroutine(HandleScatterMode(scatterTimeLeft));
            PacMan.onPowerMode = false;
        }
        ConsumedEnemy();
        SirenAudio(consumedPoints);
    }

    void SirenAudio(float PointsAte)
    {
        if (PointsAte >= 0 && !sirenBools[0])
        {
            AudioManager.GetComponent<AudioManager>().play("siren_1", false);
            sirenBools[0] = true;
        }

        if (PointsAte >= 85 && !sirenBools[1])
        {
            AudioManager.GetComponent<AudioManager>().play("siren_1", true);
            AudioManager.GetComponent<AudioManager>().play("siren_2", false);
            sirenBools[1] = true;
        }

        if (PointsAte >= 85 * 2 && !sirenBools[2])
        {
            AudioManager.GetComponent<AudioManager>().play("siren_2", true);
            AudioManager.GetComponent<AudioManager>().play("siren_3", false);
            sirenBools[2] = true;
        }

        if (PointsAte >= 85 * 3 && !sirenBools[3])
        {
            AudioManager.GetComponent<AudioManager>().play("siren_3", true);
            AudioManager.GetComponent<AudioManager>().play("siren_4", false);
            sirenBools[3] = true;
        }

        if (PointsAte >= 85 * 4 && !sirenBools[4])
        {
            AudioManager.GetComponent<AudioManager>().play("siren_4", true);
            AudioManager.GetComponent<AudioManager>().play("siren_5", false);
            sirenBools[4] = true;
        }
    }

    void SetScatterDirection(int whichGhost)
    {
        if (whichGhost >= 4) { Debug.LogError("out of range " + whichGhost); return; }

        Vector3 currentChaseDirection = -ghostObjs[whichGhost].GetComponent<GhostChase>().currentDirection;

        ghostObjs[whichGhost].GetComponent<ScatterBehaviour>().currentDirection = currentChaseDirection;
        ghostObjs[whichGhost].GetComponent<ScatterBehaviour>().previousDirection = -currentChaseDirection;
        currentChaseDirection = Vector3.zero;
    }

    void SetChaseDirection(int whichGhost)
    {
        if (whichGhost >= 4) { Debug.LogError("out of range " + whichGhost); return; }

        Vector3 currentScatterDirection = -ghostObjs[whichGhost].GetComponent<ScatterBehaviour>().currentDirection;

        ghostObjs[whichGhost].GetComponent<GhostChase>().currentDirection = currentScatterDirection;
        ghostObjs[whichGhost].GetComponent<GhostChase>().previousDirection = -currentScatterDirection;
        currentScatterDirection = Vector3.zero;
    }

    IEnumerator HandleScatterMode(float time)
    {
        AudioManager.GetComponent<AudioManager>().play("power_pellet", false);
        powerMode = true;
        ScatterMode();

        yield return new WaitForSeconds(time);
        powerMode = false;
        // chase mode
        for (int i = 0; i < 4; i++)
        {
            if (ghostOut[i])
            {
                SetChaseDirection(i);
                ghostObjs[i].GetComponent<GhostChase>().openRay = true;
                ghostObjs[i].GetComponent<SpriteRenderer>().sprite = normalSprites[i].sprite;
                ghostObjs[i].GetComponent<ScatterBehaviour>().enabled = false;
                ghostObjs[i].GetComponent<GhostChase>().enabled = true;
            }
            else
                ghostObjs[i].GetComponent<SpriteRenderer>().sprite = normalSprites[i].sprite;
        }
        AudioManager.GetComponent<AudioManager>().play("power_pellet", true);
    }

    void ScatterMode()
    {
        for (int i = 0; i < 4; i++)
        {
            if (ghostOut[i])
            {
                SetScatterDirection(i);
                ghostObjs[i].GetComponent<ScatterBehaviour>().enabled = true;
                ghostObjs[i].GetComponent<ScatterBehaviour>().openRay = true;
                ghostObjs[i].GetComponent<GhostChase>().enabled = false;
                ghostObjs[i].GetComponent<SpriteRenderer>().sprite = scatterSprite[i].sprite;
            }
            else
                ghostObjs[i].GetComponent<SpriteRenderer>().sprite = scatterSprite[i].sprite;
        }
    }

    void ConsumedEnemy()
    {
        consumedEnemyNumber = PacMan.sendEnemyNumber;
        if (consumedEnemyNumber < 0 || !powerMode) { PacMan.sendEnemyNumber = -1; return; }

        AudioManager.GetComponent<AudioManager>().play("eat_ghost", false);
        if (consumedEnemyNumber == 0)
        {
            ghostOut[consumedEnemyNumber] = false;
            ghostObjs[consumedEnemyNumber].transform.position = new Vector3(0.5f, 5.5f, 0);
            ghostObjs[consumedEnemyNumber].GetComponent<CircleCollider2D>().isTrigger = true;
            PacMan.sendEnemyNumber = -1;
        }
        if (consumedEnemyNumber == 1)
        {
            ghostOut[consumedEnemyNumber] = false;
            ghostObjs[1].transform.position = new Vector3(-0.5f, 3.5f, 0);
            ghostObjs[consumedEnemyNumber].GetComponent<CircleCollider2D>().isTrigger = true;
            PacMan.sendEnemyNumber = -1;
        }
        if (consumedEnemyNumber == 2)
        {
            ghostOut[consumedEnemyNumber] = false;
            ghostObjs[2].transform.position = new Vector3(0.5f, 3.5f, 0);
            ghostObjs[consumedEnemyNumber].GetComponent<CircleCollider2D>().isTrigger = true;
            PacMan.sendEnemyNumber = -1;
        }
        if (consumedEnemyNumber == 3)
        {
            ghostOut[consumedEnemyNumber] = false;
            ghostObjs[3].transform.position = new Vector3(1.5f, 3.5f, 0);
            ghostObjs[consumedEnemyNumber].GetComponent<CircleCollider2D>().isTrigger = true;
            PacMan.sendEnemyNumber = -1;
        }
    }

    void LeaveHouse(bool blinky, bool pinky, bool inky, bool clyde)
    {
        if (consumedPoints >= 5 && !blinky)
        {
            ghostObjs[0].transform.position = new Vector3(0.5f, 5.5f, 0);
            ghostObjs[0].GetComponent<GhostChase>().enabled = true;
            ghostObjs[0].GetComponent<GhostChase>().currentDirection = Vector3.right;
            ghostObjs[0].GetComponent<GhostChase>().previousDirection = Vector3.left;
            ghostObjs[0].GetComponent<CircleCollider2D>().isTrigger = false;
            ghostOut[0] = true;
        }

        // pinky
        if (consumedPoints >= 10 && !pinky)
        {
            float distance = Vector3.Distance(ghostObjs[1].transform.position, new Vector3(ghostObjs[1].transform.position.x, 5.5f, 0));

            ghostObjs[1].transform.position = Vector3.MoveTowards(ghostObjs[1].transform.position,
                new Vector3(ghostObjs[1].transform.position.x, 5.5f, 0), ghostOutSpeed * Time.deltaTime);

            if (distance < 0.001f)
            {
                ghostObjs[1].GetComponent<GhostChase>().enabled = true;
                ghostObjs[1].GetComponent<GhostChase>().currentDirection = Vector3.left;
                ghostObjs[1].GetComponent<GhostChase>().previousDirection = Vector3.right;
                ghostObjs[1].GetComponent<CircleCollider2D>().isTrigger = false;
                ghostOut[1] = true;
            }
        }
        // Inky
        if (consumedPoints >= 30 && !inky)
        {
            float distance = Vector3.Distance(ghostObjs[2].transform.position, new Vector3(ghostObjs[2].transform.position.x, 5.5f, 0));

            ghostObjs[2].transform.position = Vector3.MoveTowards(ghostObjs[2].transform.position,
                new Vector3(ghostObjs[2].transform.position.x, 5.5f, 0), ghostOutSpeed * Time.deltaTime);

            if (distance < 0.001f)
            {
                ghostObjs[2].GetComponent<GhostChase>().enabled = true;
                ghostObjs[2].GetComponent<GhostChase>().currentDirection = Vector3.left;
                ghostObjs[2].GetComponent<GhostChase>().previousDirection = Vector3.right;
                ghostObjs[2].GetComponent<CircleCollider2D>().isTrigger = false;
                ghostOut[2] = true;
            }
        }
        // clyde
        if (consumedPoints >= 60 && !clyde)
        {
            float distance = Vector3.Distance(ghostObjs[3].transform.position, new Vector3(ghostObjs[3].transform.position.x, 5.5f, 0));

            ghostObjs[3].transform.position = Vector3.MoveTowards(ghostObjs[3].transform.position,
                new Vector3(ghostObjs[3].transform.position.x, 5.5f, 0), ghostOutSpeed * Time.deltaTime);

            if (distance < 0.001f)
            {
                ghostObjs[3].GetComponent<GhostChase>().enabled = true;
                ghostObjs[3].GetComponent<GhostChase>().currentDirection = Vector3.left;
                ghostObjs[3].GetComponent<GhostChase>().previousDirection = Vector3.right;
                ghostObjs[3].GetComponent<CircleCollider2D>().isTrigger = false;
                ghostOut[3] = true;
            }
        }
    }
}
