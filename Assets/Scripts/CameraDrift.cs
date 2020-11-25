using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDrift : MonoBehaviour
{
    public Vector3 originPosition;
    public Vector3 randomTargetPosition;
    public float speed = 0.01f;
    public float delay = 0.01f;

    public float returnSpeed = 0.05f;

    private Coroutine driftRoutine = null;
    private Coroutine returnRoutine = null;

    // Start is called before the first frame update
    void Start()
    {
        originPosition = transform.position;
        randomTargetPosition = GenerateRandomPosition(originPosition, 2f);
    }

    public void StartDrifting()
    {
        if(returnRoutine != null)
        {
            StopCoroutine(returnRoutine);
            returnRoutine = null;
        }
        driftRoutine = StartCoroutine(Drift());
    }

    public void StopDrifting()
    {
        if(driftRoutine != null)
        {
            StopCoroutine(driftRoutine);
            driftRoutine = null;
        }

        returnRoutine = StartCoroutine(ReturnToOrigin());
    }

    private IEnumerator Drift()
    {
        // while not at target, move towards target
        while (Vector3.Distance(transform.position, randomTargetPosition) > 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, randomTargetPosition, speed);
            yield return new WaitForSeconds(delay);
        }

        // choose new target
        randomTargetPosition = GenerateRandomPosition(originPosition, 2f);

        driftRoutine = StartCoroutine(Drift());
    }

    private IEnumerator ReturnToOrigin()
    {
        while (Vector3.Distance(transform.position, originPosition) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, originPosition, returnSpeed);
            yield return new WaitForSeconds(delay);
        }
    }

    private Vector3 GenerateRandomPosition(Vector3 origin, float distance)
    {
        float randomX = Random.Range(origin.x - distance, origin.x + distance);
        float randomZ = Random.Range(origin.z - distance, origin.z + distance);

        return new Vector3(randomX, origin.y, randomZ);
    }
}
