using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoanPanel : MonoBehaviour {

	public Board board;
	public RawImage colorImage;
	public Text collateralsText;
	public Text termsText;
	public Button acceptButton;
	public Button rejectButton;
	public float cheapRate = 0.5f;
	public float regularRate = 2;
	public int loanAmount = 200000;
	public int loanTurns = 24;

	// Use this for initialization
	void Start () {
		rejectButton.onClick.AddListener(() => gameObject.SetActive(false));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Show(Piece gamePiece, bool studentLoan){
		colorImage.color = gamePiece.color;
		collateralsText.text = (gamePiece.properties.Count == 0 ? "None" : gamePiece.properties.Count + " Properties");

		float rate = (gamePiece.job == "Rich" || studentLoan ? cheapRate: regularRate);
		float repayment = loanAmount * ( (rate / 100) / ( 1 - Mathf.Pow(1 + rate / 100, -loanTurns) ) );

		termsText.text = "$" + loanAmount.ToString("n0") + " at " + rate.ToString("n1") + "% a turn for " + loanTurns + " turns\nYou repay $" + repayment.ToString("n0") + " each turn";

		acceptButton.onClick.RemoveAllListeners();
		acceptButton.onClick.AddListener(() => {
			gamePiece.hasLoan = true;
			gamePiece.loanRepayment = Mathf.CeilToInt(repayment);
			gamePiece.loanTurnsLeft = loanTurns;

			new Task(gamePiece.ChangeMoney(loanAmount, "Loan"));

			gameObject.SetActive(false);
			board.getLoanButton.gameObject.SetActive(false);
		});

	}
}
