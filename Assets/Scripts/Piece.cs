using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {

	public Transform model;
	public Camera cam;
	public Board board;
	public Color color;
	public Animation anim;
	public float walkTime;
	public Vector3 hatPosition = new Vector3(-.02f, 0.61f, 0);
	public Vector3 hatRotation = new Vector3(90, -13.5f, 0);
	public float rotationTime = 0.5f;
	public Text moneyText;
	public List<string> qualifications;

	public bool hasJob;
	public string job;
	public int money;
	public List<Tile> properties;

	private int currentTileIndex = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (moneyText){
			moneyText.text = "$" + string.Format("{0:n0}", money);
		}
	}

	public void SetColor(Color color){
		this.color = color;
		model.renderer.material.color = color;
	}

	public void PutHat(GameObject hat){
		hat.transform.parent = model.transform;

		hat.transform.localPosition = hatPosition;
		hat.transform.localRotation = Quaternion.Euler(hatRotation);

	}

	public IEnumerator Move(int numberOfSteps){

		cam.camera.enabled = true;

		for (int i = 0; i < numberOfSteps; i++){
			Tile startTile = board.tiles[currentTileIndex];
			int nextTileIndex = currentTileIndex + 1;
			if (nextTileIndex >= board.tiles.Count){
				nextTileIndex = 0;
			}
			Tile endTile = board.tiles[nextTileIndex];


			anim.Stop();
			anim.Play("walk");

			startTile.RemovePiece(this, walkTime);
			endTile.AddPiece(this, walkTime);
			endTile.OnPass(this);

			yield return new WaitForSeconds(walkTime);

			currentTileIndex = nextTileIndex;

			if (board.cornerTiles.Contains(currentTileIndex)){
				Quaternion startRot = transform.localRotation;
				Quaternion endRot = startRot * Quaternion.Euler(0, 90, 0);
				for (float t = 0; t < rotationTime; t += Time.deltaTime){
					transform.localRotation = Quaternion.Slerp(startRot, endRot, t / rotationTime);
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForEndOfFrame();
				transform.localRotation = endRot;
			}
		}
		board.tiles[currentTileIndex].OnLand(this);

//		cam.camera.enabled = false;
	}
	
}
