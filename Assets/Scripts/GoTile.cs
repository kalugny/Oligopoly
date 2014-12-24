using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoTile : Tile {

	public override void OnPass(Piece gamePiece){
		Debug.Log ("Pass Go");

		List<int> amounts = new List<int>();
		List<string> reasons = new List<string>();
		if (gamePiece.hasJob){
			amounts.Add(gamePiece.salary);
			reasons.Add("Salary");

			if (gamePiece.overtime){
				amounts.Add (gamePiece.salary);
				reasons.Add ("Overtime");
			}
		}

		if (gamePiece.hasLoan){
			amounts.Add(-gamePiece.loanRepayment);
			reasons.Add("Loan payment");
		}


		StartCoroutine(gamePiece.ChangeMoneyMultiple(amounts.ToArray(), reasons.ToArray()));
	}
}
