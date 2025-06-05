using UnityEngine;

public class PlayerDetection : MonoBehaviour
{
    public Transform player;
    public float detectionRange = 15f;
    public float fovAngle = 45f;

    private PlayerStealth stealth;

    private void Awake()
    {
        if (player == null)
        {
            PlayerScript ps = FindObjectOfType<PlayerScript>();
            if (ps != null)
            {
                player = ps.transform;
                stealth = player.GetComponent<PlayerStealth>();
            }
        }
        else
        {
            stealth = player.GetComponent<PlayerStealth>();
        }
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > detectionRange) return false;

        if (stealth != null && stealth.IsStealthed)
        {
            return InFOV(transform, player, fovAngle, detectionRange);
        }
        else
        {
            return HasLineOfSight(transform.position, player.position, detectionRange);
        }
    }

    public static bool InFOV(Transform origin, Transform target, float maxAngle, float maxRadius)
    {
        Collider[] overlaps = new Collider[10];
        int count = Physics.OverlapSphereNonAlloc(origin.position, maxRadius, overlaps);

        for (int i = 0; i < count; i++)
        {
            if (overlaps[i] && overlaps[i].transform == target)
            {
                Vector3 dir = (target.position - origin.position).normalized;
                dir.y = 0;

                if (Vector3.Angle(origin.forward, dir) > maxAngle) continue;

                if (Physics.Raycast(origin.position, target.position - origin.position,
                                    out RaycastHit hit, maxRadius) &&
                    hit.transform == target)
                    return true;
            }
        }
        return false;
    }

    private bool HasLineOfSight(Vector3 origin, Vector3 targetPos, float range)
    {
        Vector3 dir = (targetPos - origin).normalized;
        if (Physics.Raycast(origin, dir, out RaycastHit hit, range))
        {
            return hit.transform == player;
        }
        return false;
    }
}
