using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltConnector : MonoBehaviour
{
    public Transform StartPoint;
    public Transform EndPoint;
    public float TargetSegmentLength;

    private LineRenderer LightningRenderer;

    private void Start() {
        LightningRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        Vector3 dir = (EndPoint.position - StartPoint.position).normalized;
        float dist = (EndPoint.position - StartPoint.position).magnitude;
        int segmentCount = Mathf.FloorToInt(dist / TargetSegmentLength);
        float segmentLengthModifier = dist / (TargetSegmentLength * segmentCount);
        LightningRenderer.positionCount = segmentCount + 1;
        for (int i = 0; i < segmentCount + 1; i++) {
            LightningRenderer.SetPosition(i, StartPoint.position + (dir * i * TargetSegmentLength * segmentLengthModifier));
        }
    }
}
