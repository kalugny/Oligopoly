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
	public string[] conversations;

	private bool m_inStore = false;

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

	public void OnPass(Piece gamePiece){
		Debug.Log("OnPass " + this.ToString());
	}

	public void OnLand(Piece gamePiece){
		Debug.Log("OnLand " + this.ToString());
		if (showCard){
			StartCoroutine(AnimateEncounter(gamePiece));
		}
		else{
			board.waitingForPlayer = true;
			gamePiece.cam.enabled = false;
		}

//		if (showCard){
//			PropertyCard card = board.propertyCard;
//			card.mainPanel.SetActive(true);
//
//			card.titleText.text = tileName;
//			card.titlePanel.color = color;
//			card.titleText.color = textColor;
//
//			bool jobActive = hasJob && gamePiece.job != "Rich" && jobHolder == null;
//			card.jobSection.SetActive(jobActive);
//			if (jobActive){
//				card.jobTitle.text = jobName;
//				card.salary.text = "$" + jobSalary + "/w";
//				card.applyForJobButton.onClick.AddListener(() => ApplyForJob(gamePiece));
//				card.applyForJobButton.interactable = true;
//				card.applyForJobButton.GetComponentInChildren<Text>().text = "Apply for the job";
//			}
//			card.productSection.SetActive(hasProduct);
//			if (hasProduct){
//				card.productText.text = productName;
//				card.productCost.text = "$" + productCost;
//				card.productText.GetComponent<Animator>().SetTrigger("Highlight");
//				card.productCost.GetComponent<Animator>().SetTrigger("Highlight");
//
//				gamePiece.money -= productCost;
//				if (owner != null){
//					owner.money += productCost;
//				}
//			}
//
//			card.propertySection.SetActive(isBuyable);
//			if (isBuyable){
//				if (owner == null){
//					card.buyPropertyButton.enabled = true;
//					card.proprtyButtonText.text = "Buy property for " + propertyCost;
//					card.buyPropertyButton.interactable = gamePiece.money >= propertyCost;
//					card.buyPropertyButton.onClick.RemoveAllListeners();
//					card.buyPropertyButton.onClick.AddListener(() => BuyProperty(gamePiece));
//					card.buyPropertyButton.image.color = Color.white;
//					card.proprtyButtonText.color = Color.black;
//				}
//				else {
//					card.proprtyButtonText.text = "Under new ownership!";
//					card.proprtyButtonText.color = Color.white;
//					card.buyPropertyButton.enabled = false;
//					card.buyPropertyButton.image.color = owner.color;
//				}
//			}
//
//			return false;
//		}
	}

	void BuyProperty(Piece gamePiece){
		gamePiece.money -= propertyCost;
		gamePiece.properties.Add(this);
		owner = gamePiece;
		SetFlag(gamePiece.color);
//		PropertyCard card = board.propertyCard;
//		card.proprtyButtonText.text = "Under new ownership!";
//		card.proprtyButtonText.color = Color.white;
//		card.buyPropertyButton.enabled = false;
//		card.buyPropertyButton.image.color = owner.color;
	}

	void ApplyForJob(Piece gamePiece){
//		Text buttonText = board.propertyCard.applyForJobButton.GetComponentInChildren<Text>();
//		foreach (string req in requirements){
//			if (!gamePiece.qualifications.Contains(req)){
//				Debug.Log ("Missing requirement: " + req);
//				buttonText.text = "Sorry, we're looking for someone who has a " + req;
//				return;
//			}
//		}
//
//		if (Random.value <= jobChances){
//			// got the job
//			gamePiece.job = jobName;
//			gamePiece.hasJob = true;
//			jobHolder = gamePiece;
//
//			buttonText.text = "You're hired!";
//
//		}
//		else {
//			buttonText.text = "Sorry, maybe next time...";
//		}
//
//		board.propertyCard.applyForJobButton.interactable = false;
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

	public IEnumerator AnimateEncounter(Piece gamePiece){

		m_inStore = true;

		RectTransform c = board.bgCanvas;
		c.gameObject.SetActive(true);
		c.SetParent(transform);
		c.anchoredPosition = screenPosition;

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

		board.continueButton.onClick.RemoveAllListeners();
		board.continueButton.onClick.AddListener(ContinuePressed);
		board.continueButton.interactable = false;

		c.GetComponent<Animator>().SetTrigger("Lower");

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

		string conversation = conversations[Random.Range(0, conversations.Length - 1)];
		List<KeyValuePair<SpeechBubble, string> >  convo = parseCoveration(conversation);

		foreach (KeyValuePair<SpeechBubble, string> line in convo){
			Task t = new Task(line.Key.Say(line.Value, board.textSpeed));
			while (t.Running){
				yield return new WaitForEndOfFrame();
			}
		}

		board.continueButton.interactable = true;
		while (m_inStore){
			yield return new WaitForEndOfFrame();
		}

		Animator animator = c.GetComponent<Animator>();
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
