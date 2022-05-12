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
	public float StrikeFadeSpeed = 1;
	public bool UseFlash = true;
	public float StrikeFlashHesitation = 0.1f;
	public float FlashSpeed = 100;


	public GameObject lightningbolt;
    private Material mat1;
    private Material mat2;
    private Material lmat;
    private BoltConnector bolt;
	float timer = 0;

    private void Start() {
        mat1 = obj1.GetComponent<Renderer>().material;
        mat2 = obj2.GetComponent<Renderer>().material;

		lmat = lightningbolt.GetComponent<Renderer>().material;
		bolt = lightningbolt.GetComponent<BoltConnector>();
	}

    void Update()
    {
        float sample = (Mathf.Sin(CompletenessSinSpeed * Time.time) + 1) / 2;
        mat1.SetFloat("_Completeness", sample);
        mat2.SetFloat("_Completeness", sample);

		if (timer >= StrikeCooldown) {
			bolt.Connection = 1;
			timer = 0;
		}
		bolt.Connection = Mathf.Clamp01(bolt.Connection - StrikeFadeSpeed * Time.deltaTime);
		if (UseFlash && bolt.Connection < 1 - StrikeFlashHesitation)
		{
			lmat.SetFloat("_RenderFlash", Mathf.RoundToInt(Mathf.Sin(bolt.Connection * FlashSpeed)));
		}
		else
		{
			lmat.SetFloat("_RenderFlash", 0);
		}
		timer += Time.deltaTime;
	}
}
