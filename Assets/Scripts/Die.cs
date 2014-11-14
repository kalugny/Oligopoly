using UnityEngine;
using System.Collections;

public class Die : MonoBehaviour {

	public float minSpeedForResult = 0.001f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public int GetResult() {
	
		if (rigidbody.velocity.magnitude > minSpeedForResult) return 0;

		if (Vector3.Angle(transform.up, Vector3.up) <= 45) return 1;

		if (Vector3.Angle(-transform.up, Vector3.up) <= 45) return 6;

		if (Vector3.Angle(transform.right, Vector3.up) <= 45) return 4;

		if (Vector3.Angle(-transform.right, Vector3.up) <= 45) return 3;

		if (Vector3.Angle(transform.forward, Vector3.up) <= 45) return 2;

		if (Vector3.Angle(-transform.forward, Vector3.up) <= 45) return 5;

		return 0;
	}
}
