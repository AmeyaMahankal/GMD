using UnityEngine;

public class StealthKillGizmos : MonoBehaviour
{
    public float killDistance = 2f;   
    public float killAngle = 45f;     

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killDistance); 

         Vector3 backward = transform.position - transform.forward * killDistance;

        Quaternion leftRotation = Quaternion.Euler(0, killAngle+90, 0);   
        Quaternion rightRotation = Quaternion.Euler(0, -killAngle-90, 0); 

        Vector3 leftDirection = leftRotation * -transform.forward * killDistance;
        Vector3 rightDirection = rightRotation * -transform.forward * killDistance;
                
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDirection);
        Gizmos.DrawLine(transform.position, transform.position + rightDirection);
    }
}

