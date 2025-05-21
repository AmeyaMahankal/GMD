using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField] private Transform target;      // Assign the Player here
    [SerializeField] private Vector3 offset = new Vector3(0f, 15f, -5f);
    [SerializeField] private float followSpeed = 10f;

    private void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

        transform.LookAt(target);
    }
}