using UnityEngine;
using System.Collections;

public class Flag : MonoBehaviour {

	public Material flagMaterial;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetColor(Color color){
		flagMaterial.color = color;
	}
}
