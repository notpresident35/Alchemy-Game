using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltConnector : MonoBehaviour
{
	public class BoltPoint {
		public Transform Point;
		public Vector3 Velocity;
		public Vector3 OffsetPos;
	}

	public float StrikeLength = 1;
	public bool UseFlash = true;
		
	public float StrikeFlashHesitation = 0.1f;
	public float FlashSpeed = 100;

	[Tooltip("Distance in units to scatter each point by")]
	[Range(0, 2)]
	public float ScatterDist;
	public int PointsPerUnit;

    public float TargetSegmentLength;
	public AnimationCurve ConnectionToWidthCompression;
	public AnimationCurve ConnectionToGapSize;
	public AnimationCurve ConnectionToMidWidth;
	public AnimationCurve ConnectionToEndWidth;

	private bool isStriking = false;
	[SerializeField] private float connection;
    private LineRenderer LightningRenderer;
    private Material mat;
	private List<BoltPoint> Path = new List<BoltPoint>();
	private Transform startPoint;
	private Transform endPoint;

    private void Start() {
        LightningRenderer = GetComponent<LineRenderer>();
        mat = LightningRenderer.material;
    }

	// Spawn and scatter points along path from start to end position
	public void InitializeStrike (Transform from, Transform to) {
		// Reset
		foreach (BoltPoint point in Path) {
			Destroy(point.Point.gameObject);
		}
		Path = new List<BoltPoint>();
		startPoint = from;
		endPoint = to;

		// Spawn number of scattered points based on the distance between the start and end points
		int pointCount = Mathf.CeilToInt((startPoint.position - endPoint.position).magnitude * PointsPerUnit);
		float distPerPoint = (startPoint.position - endPoint.position).magnitude / pointCount;
		for	(int i = 0; i < pointCount; i++) {
			BoltPoint point = new BoltPoint();
			point.Point = new GameObject ("Bolt Point").transform;
			point.OffsetPos = Statics.RandVectorPosNeg(ScatterDist);
			point.Point.position = startPoint.position + Vector3.one * distPerPoint * i + point.OffsetPos;
			Path.Add(point);
		}

		// Start animating points
		StartCoroutine(AnimateStrike());
	}

	IEnumerator AnimateStrike () {
		isStriking = true;
		connection = 1;
		for (float i = 0; i < 1; i += Time.deltaTime / StrikeLength) {
			connection = Mathf.Clamp01(1 - i);
			if (UseFlash && i > StrikeFlashHesitation)
			{
				mat.SetFloat("_RenderFlash", Mathf.RoundToInt(Mathf.Sin((i - StrikeFlashHesitation) * FlashSpeed)));
			}
			yield return null;
		}
		
		mat.SetFloat("_RenderFlash", 0);
		isStriking = false;
	}

    void Update()
    {
		if (!isStriking) {
			return;
		}
		LightningRenderer.positionCount = 0;
		int posCounter = 0;

		// Move points
		float distPerPoint = (startPoint.position - endPoint.position).magnitude / Path.Count;
		for (int i = 0; i < Path.Count - 1; i++)
		{
			// Adjust velocity
			Path[i].OffsetPos += Path[i].Velocity * Time.deltaTime;
			Path[i].Point.position = startPoint.position + Vector3.one * distPerPoint * i + Path[i].OffsetPos;
		}

		// Connect each point with the line renderer
		for (int i = 0; i < Path.Count - 1; i++)
		{
			Vector3 StartPos = Path[i].Point.position;
			Vector3 ToEndPosVector = Path[i + 1].Point.position - StartPos;

			// Split sub-lines into segments, then insert positions accordingly
			int segmentCount = Mathf.FloorToInt(ToEndPosVector.magnitude / TargetSegmentLength);
			float segmentLengthModifier = ToEndPosVector.magnitude / (TargetSegmentLength * segmentCount);
			LightningRenderer.positionCount += segmentCount + 1;
			for (int j = 0; j < segmentCount + 1; j++)
			{
				LightningRenderer.SetPosition(j + posCounter, StartPos + (ToEndPosVector.normalized * j * TargetSegmentLength * segmentLengthModifier));
			}
			posCounter += segmentCount;
		}
		LightningRenderer.SetPosition(posCounter + 1, Path[Path.Count - 1].Point.position);
		LightningRenderer.positionCount--;

		// Set line renderer width
		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(new Keyframe (Mathf.Clamp01(ConnectionToWidthCompression.Evaluate(connection) / 2 - 0.0002f), ConnectionToEndWidth.Evaluate(connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(0.5f - (ConnectionToGapSize.Evaluate(connection) / 2) - 0.0001f), ConnectionToMidWidth.Evaluate(connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(0.5f + (ConnectionToGapSize.Evaluate(connection) / 2) + 0.0001f), ConnectionToMidWidth.Evaluate(connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(1 - ConnectionToWidthCompression.Evaluate(connection) / 2 + 0.0002f), ConnectionToEndWidth.Evaluate(connection), 0, 0));
		LightningRenderer.widthCurve = curve;
	}
}
