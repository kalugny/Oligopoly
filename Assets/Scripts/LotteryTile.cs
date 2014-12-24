using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LotteryTile : Tile {

	public override void OnLand (Piece gamePiece){

		gamePiece.cam.enabled = false;

		StartCoroutine(AnimateCard(gamePiece));
	}

	IEnumerator AnimateCard(Piece gamePiece){

		int prize = 0;
		string msg = "You didn't win\n\nBetter luck next time!";
		if (Random.Range(1, 13000000) == 1){
			prize = 50000000;
			msg = "OH MY GOD!\nYOU WON $50,000,000!\nThe odds for that were 1 in 13,000,000!\nYou are one lucky SOB!";
		}
		else if (Random.Range(1, 2000000) == 1) {
			prize = 50000;
			msg = "You won $50,000! The odds for that were 1 in 2,000,000!";
		}
		else if (Random.Range(1, 1000) == 1){
			prize = 100;
			msg = "You won $100!";
		}
		else if (Random.Range(1, 50) == 1) {
			prize = 50;
			msg = "You won $50!";
		}
		else if (Random.Range(1, 10) == 1){
			prize = 1;
			msg = "You won back your $1";
		}

		board.lotteryCard.GetComponentInChildren<Text>().text = msg;

		Animator anim = board.lotteryCard.GetComponent<Animator>();
		anim.SetTrigger("Draw");

		while (anim.GetCurrentAnimatorStateInfo(0).IsName("LottoDraw")){
			yield return new WaitForEndOfFrame();
		}

		if (prize > 0){
			StartCoroutine(gamePiece.ChangeMoneyMultiple(new int[]{-productCost, prize}, new string[]{"Lottery Ticket", "Prize"}));
		}
		else {
			StartCoroutine(gamePiece.ChangeMoney(-productCost, "Lottery Ticket"));
		}

		while (!Input.GetMouseButton(0)){
			yield return new WaitForEndOfFrame();
		}

		anim.SetTrigger("Fold");

		board.waitingForPlayer = true;
	}

}
