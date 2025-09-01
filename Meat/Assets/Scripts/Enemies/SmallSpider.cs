using UnityEngine;

public class SmallSpider : MonoBehaviour
{
    public Vector3 center = new Vector3(-30f, 0f, 22f);
    public float speed = 1f;
    public float maxDistance = 10f;
    public float wanderStrength = 0.5f; // 0 = no wander, 1 = all wander
    private float directionChangeInterval;

    private Vector3 currentDirection;
    private float directionTimer;

    void Start()
    {
        UpdateDirection();
    }

    void Update()
    {
        directionTimer -= Time.deltaTime;

        if (directionTimer <= 0f)
        {
            UpdateDirection();
            directionChangeInterval = Random.Range(0.5f, 2f);
            directionTimer = directionChangeInterval * Random.Range(2, 3);
        }
        else if (directionTimer >= directionChangeInterval)
        {
            GetComponent<Animator>().SetBool("Walk", true);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            GetComponent<Animator>().SetBool("Walk", false);
        }

        Vector3 targetDirection = new Vector3(currentDirection.x, 0, currentDirection.z).normalized;
        if (targetDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 3f);
        }
    }

    void UpdateDirection()
    {
        Vector3 toCenter = (center - transform.position);
        toCenter.y = 0f;
        float distance = toCenter.magnitude;

        Vector3 centerDir = toCenter.normalized;

        Vector2 randCircle = Random.insideUnitCircle.normalized;
        Vector3 randomDir = new Vector3(randCircle.x, 0, randCircle.y);

        float bias = Mathf.Clamp01(distance / maxDistance);

        currentDirection = Vector3.Slerp(randomDir, centerDir, bias * (1f - wanderStrength)).normalized;
    }
}
