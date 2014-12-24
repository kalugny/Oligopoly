using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Board : MonoBehaviour {

	public Transform tileContainer;
	public Transform gamePieceContainer;
	public DieRoll dieRoll;
	public GameObject lotteryCard;
	public GameObject karmaCard;
	public GameObject gamePiecePrefab;
	public GameObject topHatPrefab;
	public GameObject moneyPanelPrefab;
	public GameObject moneyTextPrefab;
	public RectTransform uiPanel; 
	public Color[] gamePieceColors;
	public List<int> cornerTiles;
	public float hatRaffleMinTime = 5;
	public float hatRaffleMaxTime = 6;
	public float delayAfterRichSelection = 1;
	public RectTransform bgCanvas;
	public GameObject salesman;
	public Animation walkAnim;
	public float salesmanWalkLength = 1;
	public SpeechBubble salesmanBubble;
	public SpeechBubble playerBubble;
	public float textSpeed = 0.5f;
	public Button forSaleButton;
	public Button helpWantedButton;
	public Button continueButton;
	public Image bgHeader; 
	public Image purchasePanel;
	public float purchasePanelAnimTime = 0.3f;
	public float moneyTextAnimTime = 0.4f;
	public float moneyTextAnimHeight = 50;
	public Button getLoanButton;
	public LoanPanel loanPanel;
	public Sprite uniLoanSprite;
	public Sprite regLoanSprite;

	public int startingMoney = 200;
	public int startingMoneyRich = 200000;
	public float ratioOfPropertyRichStartsWith = 0.5f;

	public List<Tile> tiles;
	public List<Piece> gamePieces;

	private int m_turn = 0;
	public bool waitingForPlayer = false;
	
	void Awake () {
		foreach (Transform t in tileContainer){
			Tile tile = t.GetComponent<Tile>();
			tile.board = this;
			tiles.Add(tile);
		}

		foreach (Color c in gamePieceColors){
			GameObject go = Instantiate(gamePiecePrefab, tiles[0].transform.position, Quaternion.identity) as GameObject;
			go.transform.parent = gamePieceContainer;

			Piece p = go.GetComponent<Piece>();
			p.board = this;
			p.SetColor(c);
			GameObject moneyPanel = Instantiate(moneyPanelPrefab) as GameObject;
			moneyPanel.transform.SetParent(uiPanel);
			moneyPanel.GetComponent<Image>().color = new Color(c.r, c.g, c.b, 0.8f);
			p.moneyText = moneyPanel.GetComponentInChildren<Text>();
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

	void CloseCard(){
		waitingForPlayer = true;
	}

	IEnumerator SelectRich(){
		float runTime = Random.Range(hatRaffleMinTime, hatRaffleMaxTime);
		int pos = Random.Range (0, gamePieces.Count - 1);
		float timeScale = 0.01f;

		GameObject hat = Instantiate(topHatPrefab) as GameObject;

		gamePieces[0].cam.camera.enabled = true;

		for (float t = 0; t < runTime; t += timeScale){
			pos = (pos + 1) % gamePieces.Count;
			gamePieces[pos].PutHat(hat);
			timeScale *= 1.2f;
			yield return new WaitForSeconds(timeScale);
		}

		yield return new WaitForSeconds(delayAfterRichSelection);
		// TODO: Put some gui here explaining

		gamePieces[0].cam.camera.enabled = false;

		// gamePieces[pos].hasJob = true;
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
						t.owner = p;
						t.SetFlag(p.color);
					}

				}
			}
			else {
				p.money = startingMoney;
			}
		}

		waitingForPlayer = true;
	}

	void RollResult(int first, int second){

		Debug.Log("Dice: " + first + ", " + second);
		int total = first + second;

		if (Input.GetKey(KeyCode.Q)){
			total = 2;
		}
		if (Input.GetKey(KeyCode.W)){
			total = 7;
		}
		if (Input.GetKey(KeyCode.E)){
			total = 42;
		}

		new Task(gamePieces[m_turn].Move (total));
		m_turn = (m_turn + 1) % gamePieces.Count;
		while (gamePieces[m_turn].missTurn){
			gamePieces[m_turn].missTurn = false;
			m_turn = (m_turn + 1) % gamePieces.Count;
		}

	}
	
	// Update is called once per frame
	void Update () {

		// getLoanButton.gameObject.SetActive(waitingForPlayer);

		if (Input.GetKeyUp(KeyCode.Space) && waitingForPlayer){
			waitingForPlayer = false;
			StartCoroutine(dieRoll.RollDice(RollResult));
		}

		if (Input.GetKey(KeyCode.S)){
			Time.timeScale = 2f;
		}
		else {
			Time.timeScale = 1;
		}
	}
}
