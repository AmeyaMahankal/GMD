#if UNITY_EDITOR
using UnityEngine;

[ExecuteInEditMode]
public class EnemyGizmos : MonoBehaviour
{
    [Header("Detection")]
    public float detectionRange = 15f;
    public float fovAngle = 45f;

    [Header("Stealth-Kill")]
    public float killDistance = 2f;
    public float killAngle = 45f;

    public Transform player;

    private void OnDrawGizmos()
    {
        // 360° detection range
        Gizmos.color = new Color(1f, 1f, 0f, 0.2f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        // 45° stealth cone
        Vector3 left = Quaternion.AngleAxis(fovAngle, transform.up) * transform.forward * detectionRange;
        Vector3 right = Quaternion.AngleAxis(-fovAngle, transform.up) * transform.forward * detectionRange;
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, left);
        Gizmos.DrawRay(transform.position, right);

        // Player line-of-sight ray
        if (player != null)
        {
            Vector3 toPlayer = player.position - transform.position;
            float distance = toPlayer.magnitude;

            bool hasClearLine = Physics.Raycast(transform.position, toPlayer.normalized, out RaycastHit hit, detectionRange)
                                && hit.transform == player;

            bool inFOV = Vector3.Angle(transform.forward, toPlayer.normalized) <= fovAngle;

            Gizmos.color = (hasClearLine && inFOV) ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, toPlayer.normalized * Mathf.Min(distance, detectionRange));
        }

        // Kill range and angle
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killDistance);

        Vector3 killL = Quaternion.AngleAxis(killAngle, transform.up) * -transform.forward * killDistance;
        Vector3 killR = Quaternion.AngleAxis(-killAngle, transform.up) * -transform.forward * killDistance;
        Gizmos.color = new Color(1f, 0f, 1f);
        Gizmos.DrawRay(transform.position, killL);
        Gizmos.DrawRay(transform.position, killR);
    }
}
#endif
