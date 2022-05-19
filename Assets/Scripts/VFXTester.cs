using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXTester : MonoBehaviour
{
    [Header("Spawn Effect Core")]
    public float CompletenessSinSpeed = 1;
    public GameObject obj1;
    public GameObject obj2;

	[Header("Lightning Bolt")]
	public float StrikeCooldown = 1;
	public Transform strikeStartPoint;
	public Transform strikeStartOrbitPoint;
	public Transform strikeTargetPoint;
	public BoltConnector LightningBolt;

    private Material mat1;
    private Material mat2;
	float timer = 0;

    private void Start() {
        mat1 = obj1.GetComponent<Renderer>().material;
        mat2 = obj2.GetComponent<Renderer>().material;
	}

    void Update()
    {
        float sample = (Mathf.Sin(CompletenessSinSpeed * Time.time) + 1) / 2;
        mat1.SetFloat("_Completeness", sample);
        mat2.SetFloat("_Completeness", sample);

		if (timer > StrikeCooldown) {
			timer = 0;
			LightningBolt.InitializeStrike(strikeStartPoint, strikeTargetPoint);
		}
		timer += Time.deltaTime;

		strikeStartPoint.transform.position = strikeStartOrbitPoint.transform.position /*+ new Vector3 (Mathf.Cos (Time.time * 3), Mathf.Sin (Time.time * 3), 0) * 3*/;
	}
}
