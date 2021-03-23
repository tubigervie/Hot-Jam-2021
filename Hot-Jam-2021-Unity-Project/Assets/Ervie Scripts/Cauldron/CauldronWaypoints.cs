using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronWaypoints : MonoBehaviour
{
    const float waypointGizmoRadius = .3f;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            int j = GetNextIndex(i);

            Gizmos.DrawSphere(GetWaypoint(i), waypointGizmoRadius);
            Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
        }
    }

    public int GetNextIndex(int i)
    {
        if (i + 1 == transform.childCount)
            return 0;
        return i + 1;
    }

    public Vector3 GetWaypoint(int i)
    {
        return transform.GetChild(i).position;
    }

    public Vector3 GetRandomWaypoint()
    {
        return transform.GetChild(Random.Range(0, transform.childCount)).position;
    }
}
