using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Tile : MonoBehaviour {

	const float PIECE_WIDTH = 0.08f;

	public Board board;
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
	public bool showCard = true;

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
		float radius = count * PIECE_WIDTH;
		float angle = 2 * Mathf.PI / count;
		Vector3[] positions = new Vector3[count];
		for (int i = 0; i < count; i++){
			Vector3 circlePos = radius * (new Vector3(Mathf.Sin(angle * i + direction * Mathf.Deg2Rad), 0, Mathf.Cos(angle * i + direction * Mathf.Deg2Rad)));
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
			PropertyCard card = board.propertyCard;
			card.mainPanel.SetActive(true);

			card.titleText.text = tileName;
			card.titlePanel.color = color;
			card.titleText.color = textColor;
			card.jobSection.SetActive(hasJob);
			if (hasJob){
				card.jobTitle.text = jobName;
				card.salary.text = "$" + jobSalary + "/w";
			}
			card.productSection.SetActive(hasProduct);
			if (hasProduct){
				card.productText.text = productName;
				card.productCost.text = "$" + productCost;
				card.productText.GetComponent<Animator>().SetTrigger("Highlight");
				card.productCost.GetComponent<Animator>().SetTrigger("Highlight");

				gamePiece.money -= productCost;
			}

			card.propertySection.SetActive(isBuyable);
			if (isBuyable){
				card.proprtyButtonText.text = "Buy property for " + propertyCost;
			}


		}
	}

	public bool QualifyForJob(Piece gamePiece){
		return true;
	}
}
