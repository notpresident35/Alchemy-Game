using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltConnector : MonoBehaviour
{
	public float Connection;

    public Transform[] Path;
    public float TargetSegmentLength;
	public AnimationCurve ConnectionToWidthCompression;
	public AnimationCurve ConnectionToGapSize;
	public AnimationCurve ConnectionToMidWidth;
	public AnimationCurve ConnectionToEndWidth;

    private LineRenderer LightningRenderer;

    private void Start() {
        LightningRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
		LightningRenderer.positionCount = 0;
		int posCounter = 0;

		// Connect each point with the line renderer
		for (int i = 0; i < Path.Length - 1; i++)
		{
			Vector3 StartPos = Path[i].position;
			Vector3 ToEndPosVector = Path[i + 1].position - StartPos;

			int segmentCount = Mathf.FloorToInt(ToEndPosVector.magnitude / TargetSegmentLength);
			float segmentLengthModifier = ToEndPosVector.magnitude / (TargetSegmentLength * segmentCount);
			LightningRenderer.positionCount += segmentCount + 1;
			for (int j = 0; j < segmentCount + 1; j++)
			{
				LightningRenderer.SetPosition(j + posCounter, StartPos + (ToEndPosVector.normalized * j * TargetSegmentLength * segmentLengthModifier));
			}
			posCounter += segmentCount;
		}
		LightningRenderer.SetPosition(posCounter + 1, Path[Path.Length - 1].position);
		LightningRenderer.positionCount--;

		// Set line renderer width
		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(new Keyframe (Mathf.Clamp01(ConnectionToWidthCompression.Evaluate(Connection) / 2 - 0.0002f), ConnectionToEndWidth.Evaluate(Connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(0.5f - (ConnectionToGapSize.Evaluate(Connection) / 2) - 0.0001f), ConnectionToMidWidth.Evaluate(Connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(0.5f + (ConnectionToGapSize.Evaluate(Connection) / 2) + 0.0001f), ConnectionToMidWidth.Evaluate(Connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(1 - ConnectionToWidthCompression.Evaluate(Connection) / 2 + 0.0002f), ConnectionToEndWidth.Evaluate(Connection), 0, 0));
		LightningRenderer.widthCurve = curve;
	}
}
