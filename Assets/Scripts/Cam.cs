using UnityEngine;
using System.Collections;

public class Cam : MonoBehaviour {

	public Board board;
	public float closeUpDistance = 0.1f;
	public float zoomOutDistance = 1f;


	// Use this for initialization
	void Start () {
		Vector3 goPos = board.tiles[0].transform.position;
		transform.LookAt(goPos);
		float distanceFromObject = Vector3.Distance(goPos, transform.position);
		transform.Translate(Vector3.forward * (distanceFromObject - closeUpDistance), Space.Self);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
