using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : MonoBehaviour {

    [SerializeField]
    List<Vector3> ContactRays = new List<Vector3>();

    [SerializeField]
    float Distance = 0.5f;

    [SerializeField]
    Transform Origin;

    List<RaycastHit2D> ResultHit = new List<RaycastHit2D>();

    RaycastHit2D Hit;

    float resultAngle;
    float avarageX;
    float avarageY;

    public Quaternion CurrentRotation;

    [SerializeField]
    LayerMask mask;// = 0 << 0 | 1 | 16;

    private void FixedUpdate()
    {
        ResultHit.Clear();

        for(int i=0;i< ContactRays.Count; i++)
        {
            Hit = Physics2D.Raycast(Origin.position, ContactRays[i], Distance, mask);
            if (Hit)
            {
                ResultHit.Add(Hit);
                Debug.DrawRay(Origin.position, ContactRays[i] * Distance, Color.red);
            }
        }

        if (ResultHit.Count > 0)
        {
            avarageX = 0f;
            avarageY = 0f;
            for (int i = 0; i < ResultHit.Count; i++)
            {
                avarageX += ResultHit[i].point.x;
                avarageY += ResultHit[i].point.y;
            }

            avarageX /= ResultHit.Count;
            avarageY /= ResultHit.Count;

            Vector2 tempVec = (Vector2)Origin.position - new Vector2(avarageX, avarageY);
            float resultAngle = Mathf.Atan2(tempVec.y, tempVec.x) * Mathf.Rad2Deg;

            CurrentRotation = Quaternion.Slerp(Origin.rotation, Quaternion.AngleAxis(resultAngle - 90f, Vector3.forward), Time.deltaTime * 5f);
        }
        else
        {
            CurrentRotation = Quaternion.identity;
        }
    }
}
