using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	public Transform tileContainer;
	public Transform gamePieceContainer;
	public DieRoll dieRoll;
	public PropertyCard propertyCard;
	public GameObject gamePiecePrefab;
	public GameObject topHatPrefab;
	public Color[] gamePieceColors;
	public List<int> cornerTiles;
	public float hatRaffleMinTime = 5;
	public float hatRaffleMaxTime = 6;

	public int startingMoney = 200;
	public int startingMoneyRich = 200000;
	public float ratioOfPropertyRichStartsWith = 0.5f;

	public List<Tile> tiles;
	public List<Piece> gamePieces;

	private int m_turn = 0;
	private bool m_waitingForPlayer = false;
	
	void Awake () {
		foreach (Transform t in tileContainer){
			Tile tile = t.GetComponent<Tile>();
			tile.board = this;
			tiles.Add(tile);
		}

		foreach (Color c in gamePieceColors){
			GameObject go = Instantiate(gamePiecePrefab, tiles[0].transform.position, Quaternion.identity) as GameObject;
			go.transform.parent = gamePieceContainer;
			go.GetComponentInChildren<Renderer>().material.color = c;	

			Piece p = go.GetComponent<Piece>();
			p.board = this;
			gamePieces.Add(p);
		}

	}

	void Start(){
		foreach (Piece p in gamePieces){
			tiles[0].piecesHere.Add(p);
		}

		StartCoroutine(tiles[0].AnimateArragement(tiles[0].ArrangePieces(gamePieces.Count), 0));

		StartCoroutine(SelectRich());

	}

	IEnumerator SelectRich(){
		float runTime = Random.Range(hatRaffleMinTime, hatRaffleMaxTime);
		int pos = Random.Range (0, gamePieces.Count - 1);
		float timeScale = 0.01f;

		GameObject hat = Instantiate(topHatPrefab) as GameObject;

		for (float t = 0; t < runTime; t += timeScale){
			pos = (pos + 1) % gamePieces.Count;
			gamePieces[pos].PutHat(hat);
			timeScale *= 1.2f;
			yield return new WaitForSeconds(timeScale);
		}

		gamePieces[pos].hasJob = true;
		gamePieces[pos].job = "Rich";

		foreach (Piece p in gamePieces){
			if (p.job == "Rich"){
				p.money = startingMoneyRich;
				List<Tile> buyable = new List<Tile>();
				foreach (Tile t in tiles){
					if (t.isBuyable){
						buyable.Add(t);
					}
				}
				int richProperties = Mathf.FloorToInt(buyable.Count * ratioOfPropertyRichStartsWith);

				while (p.properties.Count < richProperties){
					Tile t = buyable[Random.Range(0, buyable.Count - 1)];
					if (!p.properties.Contains(t)){
						p.properties.Add(t);
					}
				}
			}
			else {
				p.money = startingMoney;
			}
		}

		m_waitingForPlayer = true;
	}

	void RollResult(int first, int second){

		Debug.Log("Dice: " + first + ", " + second);
		int total = first + second;

		Task moveTask = new Task(gamePieces[m_turn].Move (total));
		m_turn = (m_turn + 1) % gamePieces.Count;

		moveTask.Finished += (bool manual) => {
			m_waitingForPlayer = true;
		};

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp(KeyCode.Space) && m_waitingForPlayer){
			m_waitingForPlayer = false;
			StartCoroutine(dieRoll.RollDice(RollResult));
		}
	}
}
