using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Piece : MonoBehaviour {

	public Transform model;
	public Board board;
	public Animation anim;
	public float walkTime;
	public Vector3 hatPosition = new Vector3(-.02f, 0.61f, 0);
	public Vector3 hatRotation = new Vector3(90, -13.5f, 0);

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

	}

	public void PutHat(GameObject hat){
		hat.transform.parent = model.transform;

		hat.transform.localPosition = hatPosition;
		hat.transform.localRotation = Quaternion.Euler(hatRotation);

	}

	public IEnumerator Move(int numberOfSteps){

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
				transform.Rotate(0, 90, 0);	
			}
		}
		board.tiles[currentTileIndex].OnLand(this);
	}
	
}
