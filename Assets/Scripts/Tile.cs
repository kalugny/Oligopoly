using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	const float PIECE_WIDTH = 0.08f;

	public Board board;
	public GameObject flagPrefab;
	public GameObject flag = null;
	public Vector3 flagPosition;
	public Vector3 flagRotation;
	public Vector3 screenPosition;
	public float direction = 0;
	public List<Piece> piecesHere;
	public string tileName;
	public Color color;
	public Color textColor;
	public bool isBuyable;
	public int propertyCost;
	public bool hasProduct;
	public string productName;
	public int productCost;
	public bool uniqueProduct;
	public bool immuneToAnticonsumerism = false;
	public bool hasJob;
	public string jobName;
	public int jobSalary;
	public Piece jobHolder = null;
	public string[] requirements;
	public float jobChances;
	public bool showCard = true;
	public Piece owner = null;
	public Vector3 salesmanStartPos;
	public Vector3 salesmanEndPos;
	public string[] introConversations;
	public string[] richIntroCoversations;
	public string[] ownerIntroConversatios;
	public string[] saleConversations;
	public string[] noSaleCoversations;
	public string[] jobSuccessConversations;
	public string[] jobFailConversations;
	public string[] jobNotQualifyConversations;
	public string[] buySuccessCoversations;
	public string[] buyFailConversations;


	private bool m_inStore = false;
	private bool m_wantJob = false;
	private bool m_wantBuy = false;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void AddPiece(Piece p, float animTime){
		piecesHere.Add(p);
		StartCoroutine(AnimateArragement(ArrangePieces(piecesHere.Count), animTime));
	}

	public void RemovePiece(Piece p, float animTime){
		piecesHere.Remove (p);

		StartCoroutine(AnimateArragement(ArrangePieces(piecesHere.Count), animTime));
	}

	public Vector3[] ArrangePieces(int count){
		float radius = 2 * Mathf.Sqrt(count) * PIECE_WIDTH;
		float angle = 2 * Mathf.PI / count;
		Vector3[] positions = new Vector3[count];
		for (int i = 0; i < count; i++){
			Vector3 circlePos = radius * (new Vector3(Mathf.Sin(angle * i + direction * Mathf.Deg2Rad), 
			                                          0, 
			                                          Mathf.Cos(angle * i + direction * Mathf.Deg2Rad)));
			positions[i] = transform.position + circlePos;

		}

		return positions;
	}

	public IEnumerator AnimateArragement(Vector3[] positions, float animTime){

		Vector3[] origPositions = new Vector3[piecesHere.Count];
		for (int i = 0; i < piecesHere.Count; i++){
			origPositions[i] = piecesHere[i].transform.position;
		}

		for (float t = 0; t < animTime; t += Time.deltaTime){
			for (int i = 0; i < piecesHere.Count; i++){
				piecesHere[i].transform.position = Vector3.Lerp(origPositions[i], positions[i], t / animTime);
			}
			yield return new WaitForEndOfFrame();
		}
		for (int i = 0; i < piecesHere.Count; i++){
			piecesHere[i].transform.position = positions[i];
		}
	}

	public virtual void OnPass(Piece gamePiece){
	}

	public virtual void OnLand(Piece gamePiece){
		if (showCard){
			StartCoroutine(AnimateEncounter(gamePiece));
		}
		else{
			board.waitingForPlayer = true;
			gamePiece.cam.enabled = false;
		}
	}

	void BuyProperty(Piece gamePiece){
		gamePiece.money -= propertyCost;
		gamePiece.properties.Add(this);
		owner = gamePiece;
		SetFlag(gamePiece.color);
	}

	void GetJob(Piece gamePiece){
		gamePiece.job = jobName;
		gamePiece.hasJob = true;
		jobHolder = gamePiece;
		gamePiece.salary = Mathf.RoundToInt(jobSalary * (gamePiece.negotiationSkills ? 1.5f : 1));
	}

	public void SetFlag(Color color){
		if (flag == null){
			flag = Instantiate(flagPrefab) as GameObject;
			flag.transform.parent = transform;
			flag.transform.localPosition = flagPosition;
			flag.transform.localRotation =  Quaternion.Euler(flagRotation);
		}
		flag.GetComponent<Flag>().SetColor(color);
	}

	public virtual bool ShouldBuy(Piece gamePiece){

		if (!hasProduct) return false;

		if (gamePiece.qualifications.Contains(productName) && uniqueProduct){
			return false;
		}

		if (!immuneToAnticonsumerism && gamePiece.anticonsumerism && Random.value > 0.5f) {
			return false;
		}
		return true;
	}

	public IEnumerator AnimateEncounter(Piece gamePiece){

		m_inStore = true;
		m_wantBuy = false;
		m_wantJob = false;

		RectTransform c = board.bgCanvas;
		c.gameObject.SetActive(true);
		c.SetParent(transform);
		c.localRotation = Quaternion.Euler(0, 270 + direction, 0);
		c.localPosition = screenPosition;


		board.bgHeader.color = color;
		Text headerText = board.bgHeader.GetComponentInChildren<Text>();
		headerText.color = textColor;
		headerText.text = tileName;

		bool jobActive = hasJob && gamePiece.job != "Rich" && jobHolder == null;
		board.helpWantedButton.gameObject.SetActive(jobActive);

		bool buyActive = isBuyable && owner == null;
		board.forSaleButton.gameObject.SetActive(buyActive);
		if (buyActive){
			board.forSaleButton.GetComponentInChildren<Text>().text = "$" + propertyCost.ToString("n0");
		}

		bool isUni = this is UniversityTile;
		board.getLoanButton.image.sprite = isUni ? board.uniLoanSprite : board.regLoanSprite;
		board.getLoanButton.gameObject.SetActive(!gamePiece.hasLoan);

		board.continueButton.onClick.RemoveAllListeners();
		board.continueButton.onClick.AddListener(ContinuePressed);
		board.continueButton.interactable = false;

		board.helpWantedButton.onClick.RemoveAllListeners();
		board.helpWantedButton.onClick.AddListener(() => m_wantJob = true);
		board.helpWantedButton.interactable = false;

		board.forSaleButton.onClick.RemoveAllListeners();
		board.forSaleButton.onClick.AddListener(() => m_wantBuy = true);
		board.forSaleButton.interactable = false;

		board.getLoanButton.onClick.RemoveAllListeners();
		board.getLoanButton.onClick.AddListener(() => {
			board.loanPanel.gameObject.SetActive(true);
			board.loanPanel.Show (gamePiece, isUni);
		});
		board.getLoanButton.interactable = false;

		c.GetComponentInChildren<Animator>().SetTrigger("Lower");

		c.GetComponent<Canvas>().worldCamera = gamePiece.cam;

		GameObject salesman = board.salesman;
		salesman.SetActive(true);
		salesman.transform.parent = transform;

		for (float t = 0; t < board.salesmanWalkLength; t += Time.deltaTime){
			if (!board.walkAnim.isPlaying){
				board.walkAnim.Play("walk");
			}
			salesman.transform.localPosition = Vector3.Lerp(salesmanStartPos, salesmanEndPos, t / board.salesmanWalkLength);
			yield return new WaitForEndOfFrame();
		}
		salesman.transform.localPosition = salesmanEndPos;

		string[] introConvos = introConversations;
		if (gamePiece.job == "Rich"){
			introConvos = richIntroCoversations;
		}
		if (gamePiece == owner){
			introConvos = ownerIntroConversatios;
		}
		Task introConvo = new Task(ChooseAndAnimateConversation(introConvos));
		while (introConvo.Running){
			yield return new WaitForEndOfFrame();
		}

		if (ShouldBuy(gamePiece)){
		
			Task saleConvo = new Task(ChooseAndAnimateConversation(saleConversations));
			while (saleConvo.Running){
				yield return new WaitForEndOfFrame();
			}

			if (owner != gamePiece){
				if (gamePiece.discountedTile != this){
					StartCoroutine(gamePiece.ChangeMoney(-productCost, productName));
				}
				else {
					gamePiece.discountedTile = null;
				}
			}

			if (owner != null){
				StartCoroutine(owner.ChangeMoney(productCost, "Profit"));
			}

			for (float t = 0; t < board.purchasePanelAnimTime; t += Time.deltaTime){
				board.purchasePanel.transform.localScale = Vector3.Lerp(new Vector3(0, 0, 1), Vector3.one, t / board.purchasePanelAnimTime);
				yield return new WaitForEndOfFrame();
			}
			board.purchasePanel.transform.localScale = Vector3.one;

			if (!gamePiece.qualifications.Contains(productName)){
				gamePiece.qualifications.Add(productName);
			}

			while (Input.GetKey(KeyCode.Space)){
				yield return new WaitForEndOfFrame();
			}
			while (!Input.GetKey(KeyCode.Space)){
				yield return new WaitForEndOfFrame();
			}

			for (float t = 0; t < board.purchasePanelAnimTime; t += Time.deltaTime){
				board.purchasePanel.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0, 0, 1), t / board.purchasePanelAnimTime);
				yield return new WaitForEndOfFrame();
			}
			board.purchasePanel.transform.localScale = new Vector3(0, 0, 1);
		}
		else {
			Task noSaleConvo = new Task(ChooseAndAnimateConversation(noSaleCoversations));
			while (noSaleConvo.Running){
				yield return new WaitForEndOfFrame();
			}
		}

		board.continueButton.interactable = true;
		board.helpWantedButton.interactable = true;
		board.forSaleButton.interactable = true;
		board.getLoanButton.interactable = true;

		while (m_inStore){

			if (m_wantBuy){
				m_wantBuy = false;
				board.forSaleButton.interactable = false;
				board.continueButton.interactable = false;

				bool canBuy = gamePiece.money >= propertyCost;

				Task buyConvoTask = new Task(ChooseAndAnimateConversation(canBuy ? buySuccessCoversations : buyFailConversations));
				while (buyConvoTask.Running){
					yield return new WaitForEndOfFrame();
				}

				if (canBuy){
					BuyProperty(gamePiece);
				}

				board.continueButton.interactable = true;
			}

			if (m_wantJob){
				m_wantJob = false;
				board.helpWantedButton.interactable = false;
				board.continueButton.interactable = false;

				string[] hireConvos = jobSuccessConversations;
				bool gotJob = true;

				foreach (string req in requirements){
					if (!gamePiece.qualifications.Contains(req)){
						Debug.Log ("Missing requirement: " + req);
						hireConvos = jobNotQualifyConversations;
						gotJob = false;
						break;
					}
				}

				if (gotJob && Random.value > jobChances){
					hireConvos = jobFailConversations;
					gotJob = false;
				}

				Task hireConvoTask = new Task(ChooseAndAnimateConversation(hireConvos));
				while (hireConvoTask.Running){
					yield return new WaitForEndOfFrame();
				}

				if (gotJob){
					GetJob(gamePiece);
				}

				board.continueButton.interactable = true;
			}

			yield return new WaitForEndOfFrame();
		}

		Animator animator = c.GetComponentInChildren<Animator>();
		animator.SetTrigger("Raise");
		while (!animator.GetCurrentAnimatorStateInfo(0).IsName("disabled")){
			yield return new WaitForEndOfFrame();
		}

		c.gameObject.SetActive(false);

		board.waitingForPlayer = true;
		gamePiece.cam.enabled = false;
		salesman.SetActive(false);

	}

	public void ContinuePressed(){
		m_inStore = false;
	}

	IEnumerator ChooseAndAnimateConversation(string[] conversations){

		string conversation = conversations[Random.Range(0, conversations.Length - 1)];

		List<KeyValuePair<SpeechBubble, string> >  convo = parseCoveration(conversation);
		
		foreach (KeyValuePair<SpeechBubble, string> line in convo){
			Task t = new Task(line.Key.Say(line.Value, board.textSpeed));
			while (t.Running){
				yield return new WaitForEndOfFrame();
			}
		}
	}


	List<KeyValuePair<SpeechBubble, string> > parseCoveration(string coversation){
		string [] lines = coversation.Split(';');


		List<KeyValuePair<SpeechBubble, string> > convo = new List<KeyValuePair<SpeechBubble, string>>();
		foreach(string line in lines){
			string v = line.Substring(2);

			if (line.StartsWith("P:")){
				convo.Add(new KeyValuePair<SpeechBubble, string>(board.playerBubble, v));
			}
			else if (line.StartsWith("S:")){
				convo.Add(new KeyValuePair<SpeechBubble, string>(board.salesmanBubble, v));
			}
			else {
				Debug.LogWarning ("Unknown speaker in line " + line);
			}
		}

		return convo;
	}
	
}
