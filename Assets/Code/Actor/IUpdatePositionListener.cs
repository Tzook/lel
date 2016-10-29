using UnityEngine;
using System.Collections;

public interface IUpdatePositionListener
{
    void UpdateMovement(Vector3 pos, float angle);
}
