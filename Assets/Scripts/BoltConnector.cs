using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltConnector : MonoBehaviour
{
	public class BoltPoint {
		public Transform Point;
		public Vector3 Velocity;
		public Vector3 LinePos;
		public Vector3 WavePos;
		public Vector3 RandPos;
	}

	public float StrikeLength = 1;
	public bool UseFlash = true;
		
	public float StrikeFlashHesitation = 0.1f;
	public float FlashSpeed = 100;
	public AnimationCurve FlashShaderInput;

	public float ShockFrequency = 2;
	public float ShocksWidth;

	public float AAAAAAAAAAAAAAAAAAAA = 1f;
	public float ShockNoiseScale = 1;
	public float ShockSaveAmplitude = 1.5f;

	[Tooltip("Distance in units to scatter each point by")]
	[Range(0, 2)]
	public float ScatterDist;
	public int PointsPerUnit;

    public float TargetSegmentLength;
	public AnimationCurve ConnectionToWidthCompression;
	public AnimationCurve ConnectionToGapSize;
	public AnimationCurve ConnectionToMidWidth;
	public AnimationCurve ConnectionToEndWidth;

	[SerializeField] private float connection;
    private LineRenderer LightningRenderer;
    private Material mat;
	private List<BoltPoint> Path = new List<BoltPoint>();
	private Transform startPoint;
	private Transform endPoint;
	private bool pathReversed = false;
	bool striking = false;
	float strikeTimer = 0;
	float lastStrikeTime = 0;
	float noiseSeed = 0;
	Coroutine strikeCoroutine;

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

		lastStrikeTime = Time.time;

		// Spawn path with number of scattered points based on the distance between the start and end points
		int pointCount = Mathf.CeilToInt((startPoint.position - endPoint.position).magnitude * PointsPerUnit);
		CreatePath(pointCount);

		ShockPath();

		// Start animating points
		UpdatePath();
		if (striking)
		{
			StopCoroutine(strikeCoroutine);
		}
		strikeCoroutine = StartCoroutine(AnimateStrike());
	}

	void CreatePath(int pointCount)
	{
		pathReversed = false;
		if (pointCount < Path.Count)
		{
			for (int i = Path.Count - pointCount; i > 0; i--)
			{
				Destroy(Path[i].Point.gameObject);
				Path.RemoveAt(i);
			}
		}
		else if (pointCount > Path.Count)
		{
			for (int i = 0; i < pointCount - Path.Count; i++)
			{
				BoltPoint point = new BoltPoint();
				point.Point = new GameObject("Bolt Point").transform;
				Path.Add(point);
			}
		}
		
		// Scatter path points, but not start and end points
		for (int i = 1; i < Path.Count - 2; i++)
		{
			Path[i].RandPos = Statics.RandVectorPosNeg();
		}
	}

	void ShockPath()
	{
		pathReversed = !pathReversed;
		noiseSeed = Random.Range(-10000, 10000);
	}

	void UpdatePath()
	{
		float distPerPoint = (startPoint.position - endPoint.position).magnitude / (Path.Count - 1);
		for (int i = 1; i < Path.Count - 1; i++)
		{
			Vector3 linePos = -(startPoint.position - endPoint.position).normalized * distPerPoint * i;
			float angle = Mathf.Atan2(startPoint.position.y - endPoint.position.y, endPoint.position.x - startPoint.position.x) * Mathf.Rad2Deg;
			float waveFrequencySample = ((Mathf.PerlinNoise(noiseSeed * 4.24f, distPerPoint * i * ShockNoiseScale + (pathReversed ? 0 : ShockNoiseScale)) - 0.5f) / 2) * ShockSaveAmplitude;
			Vector3 wavePos = Quaternion.AngleAxis(angle, Vector3.forward) * Vector3.up * waveFrequencySample;
			Path[i].LinePos = startPoint.position + linePos;
			Path[i].WavePos = wavePos;
			Path[i].Point.position = Path[i].WavePos + Path[i].LinePos + Path[i].RandPos * ScatterDist;
		}
		Path[0].Point.position = startPoint.position;
		Path[Path.Count - 1].Point.position = endPoint.position;
	}

	void UpdateRenderPoints()
	{
		LightningRenderer.positionCount = 1;
		int posCounter = 0;

		for (int i = 0; i < Path.Count - 1; i++)
		{
			Vector3 StartPos = Path[i].Point.position;
			Vector3 ToEndPosVector = Path[i + 1].Point.position - StartPos;

			// Split sub-lines into segments, then insert positions accordingly
			int segmentCount = Mathf.FloorToInt(ToEndPosVector.magnitude / TargetSegmentLength);
			float segmentLengthModifier = ToEndPosVector.magnitude / (TargetSegmentLength * segmentCount);
			LightningRenderer.positionCount += segmentCount;
			for (int j = 0; j < segmentCount; j++)
			{
				LightningRenderer.SetPosition(j + posCounter, StartPos + (ToEndPosVector.normalized * j * TargetSegmentLength * segmentLengthModifier));
			}
			posCounter += segmentCount;
		}
		LightningRenderer.SetPosition(posCounter, Path[Path.Count - 1].Point.position);
	}

	void UpdateRenderWidth()
	{
		AnimationCurve curve = new AnimationCurve();
		curve.AddKey(new Keyframe(Mathf.Clamp01(ConnectionToWidthCompression.Evaluate(connection) / 2 - 0.0002f), ConnectionToEndWidth.Evaluate(connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(0.5f - (ConnectionToGapSize.Evaluate(connection) / 2) - 0.0001f), ConnectionToMidWidth.Evaluate(connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(0.5f + (ConnectionToGapSize.Evaluate(connection) / 2) + 0.0001f), ConnectionToMidWidth.Evaluate(connection), 0, 0));
		curve.AddKey(new Keyframe(Mathf.Clamp01(1 - ConnectionToWidthCompression.Evaluate(connection) / 2 + 0.0002f), ConnectionToEndWidth.Evaluate(connection), 0, 0));
		LightningRenderer.widthCurve = curve;
	}

	IEnumerator AnimateStrike () {
		connection = 1;
		striking = true;
		for (float i = 0; i < StrikeLength; i += Time.deltaTime) {
			// Connection
			connection = Mathf.Clamp01(1 - (i / StrikeLength));
			yield return null;
		}
		striking = false; 
	}

	private void Update()
	{
		if (!striking)
		{
			LightningRenderer.positionCount = 0;
			mat.SetFloat("_RenderFlash", 0);
			return;
		}

		// Shocks
		if (strikeTimer > 1)
		{
			strikeTimer = 0;
			ShockPath();
		}
		strikeTimer += Time.deltaTime * ShockFrequency;

		// Move points
		UpdatePath();

		// Connect each point with the line renderer
		UpdateRenderPoints();

		// Set line renderer width
		UpdateRenderWidth();

		// Update material
		if (UseFlash && Time.time - lastStrikeTime > StrikeFlashHesitation)
		{
			mat.SetFloat("_RenderFlash", Mathf.RoundToInt(FlashShaderInput.Evaluate ((Time.time * FlashSpeed) % 1)));
		}
	}
}
