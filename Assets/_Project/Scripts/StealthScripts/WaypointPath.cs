using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    public enum PathType { Loop, ReverseWhenComplete }

    public Transform[] waypoints;
    public PathType    pathType = PathType.Loop;

    private int index;
    private int direction = 1;

    // ─────────────────────────────────────────────────────────
    // NEW ❶  —  returns position of nearest waypoint
    public Vector3 GetClosestWaypoint(Vector3 fromPos)
    {
        index = GetClosestWaypointIndex(fromPos);   // sets internal index
        return waypoints[index].position;
    }

    // NEW ❷  —  returns index of nearest waypoint without changing 'index'
    public int GetClosestWaypointIndex(Vector3 fromPos)
    {
        if (waypoints == null || waypoints.Length == 0)
            return 0;

        float bestDistSqr = Mathf.Infinity;
        int   best        = 0;

        for (int i = 0; i < waypoints.Length; i++)
        {
            float d = (fromPos - waypoints[i].position).sqrMagnitude;
            if (d < bestDistSqr)
            {
                bestDistSqr = d;
                best        = i;
            }
        }
        return best;
    }
    // ─────────────────────────────────────────────────────────

    public Vector3 GetCurrentWayPoint() => waypoints[index].position;

    public Vector3 GetNextWaypoint()
    {
        if (waypoints.Length == 0) return transform.position;

        index = GetNextWaypointIndex();
        return waypoints[index].position;
    }

    private int GetNextWaypointIndex()
    {
        index += direction;

        if (pathType == PathType.Loop)
        {
            index = (index + waypoints.Length) % waypoints.Length;
        }
        else if (pathType == PathType.ReverseWhenComplete)
        {
            if (index >= waypoints.Length || index < 0)
            {
                direction *= -1;
                index     += direction * 2;
            }
        }
        return index;
    }
    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            return;
        }

        for (int i = 0; i < waypoints.Length - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        if (pathType == PathType.Loop || pathType == PathType.ReverseWhenComplete)
        {
            if (waypoints[0] != null && waypoints[waypoints.Length - 1] != null)
            {
                Gizmos.DrawLine(waypoints[waypoints.Length - 1].position, waypoints[0].position);
            }
        }
    }


}
