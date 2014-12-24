using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class KarmaTile : Tile {

	public Tile ArtSchool;
	public Tile FineTile;

	public enum KARMA_CARDS : int {
		ANTICONSUMERISM = 0,
		ART_SCHOLARSHIP,
		LITIGATION,
		OVERTIME,
		RECESSION,
		BEAUTY_CONTEST,
		JOB,
		NEGOTIATION,

		NUMBER_OF_KARMA_CARDS
	};

	public override void OnLand (Piece gamePiece){

		gamePiece.cam.enabled = false;
		
		StartCoroutine(AnimateCard(gamePiece));
	}
	
	IEnumerator AnimateCard(Piece gamePiece){

		Text cardText = board.karmaCard.GetComponentInChildren<Text>();
		Animator anim = board.karmaCard.GetComponent<Animator>();

		switch (Random.Range(0, (int)KARMA_CARDS.NUMBER_OF_KARMA_CARDS)){
		case (int)KARMA_CARDS.ANTICONSUMERISM:
			gamePiece.anticonsumerism = true;
			cardText.text = "You read about Anti-Consumerism on Wikipedia.\nFrom now on you have a 50% chance not to buy something when you land on a store tile.";
			
			anim.SetTrigger("Draw");
			
			while (anim.GetCurrentAnimatorStateInfo(0).IsName("KarmaCardDraw")){
				yield return new WaitForEndOfFrame();
			}
			
			while (!Input.GetMouseButton(0)){
				yield return new WaitForEndOfFrame();
			}
			
			anim.SetTrigger("Fold");

			board.waitingForPlayer = true;

			break;

		case (int)KARMA_CARDS.ART_SCHOLARSHIP:

			cardText.text = "A napkin doodle you drew got you an Art Scholarship!\nGo to Art School and study for free";
			gamePiece.discountedTile = ArtSchool;

			int moves = board.tiles.IndexOf(ArtSchool) - board.tiles.IndexOf(this);
			if (moves < 0) {
				moves += board.tiles.Count;
			}

			anim.SetTrigger("Draw");
			
			while (anim.GetCurrentAnimatorStateInfo(0).IsName("KarmaCardDraw")){
				yield return new WaitForEndOfFrame();
			}
			
			while (!Input.GetMouseButton(0)){
				yield return new WaitForEndOfFrame();
			}
						
			anim.SetTrigger("Fold");

			Task moveTask = new Task(gamePiece.Move(moves));

			while (moveTask.Running){
				yield return new WaitForEndOfFrame();
			}

			break;

		case (int)KARMA_CARDS.LITIGATION:

			string msg = "Your friend the lawyer volunteered to cancel the last fine you got ";

			if (!gamePiece.qualifications.Contains("Fine")){
				msg += "but you didn't get any fines.\nStill, a nice offer.";
			}
			else {
				if (Random.value < 0.1f){
					msg += "but he messed up and now the fine is doubled.";
					StartCoroutine(gamePiece.ChangeMoney(-FineTile.productCost, "Double Fine"));
				}
				else if (Random.value < 0.5f) {
					msg += "but he couldn't. Sorry!";
				}
				else {
					msg += "- you get back the money plus an apology!";
					StartCoroutine(gamePiece.ChangeMoney(FineTile.productCost, "Your money back + apology"));
					gamePiece.qualifications.Remove("Fine");
				}
			}

			cardText.text = msg;

			anim.SetTrigger("Draw");
			
			while (anim.GetCurrentAnimatorStateInfo(0).IsName("KarmaCardDraw")){
				yield return new WaitForEndOfFrame();
			}
			
			while (!Input.GetMouseButton(0)){
				yield return new WaitForEndOfFrame();
			}
			
			anim.SetTrigger("Fold");
			
			board.waitingForPlayer = true;

			break;

		case (int)KARMA_CARDS.OVERTIME:

			if (gamePiece.hasJob){
				cardText.text = "You worked overtime this week.\nYou miss one turn, but get double the salary";
				gamePiece.overtime = true;
			}
			else {
				cardText.text = "You wanted to go look for a job today, but American Idol was on TV.\nYou miss a turn";
			}
			gamePiece.missTurn = true;
			
			anim.SetTrigger("Draw");
			
			while (anim.GetCurrentAnimatorStateInfo(0).IsName("KarmaCardDraw")){
				yield return new WaitForEndOfFrame();
			}
			
			while (!Input.GetMouseButton(0)){
				yield return new WaitForEndOfFrame();
			}
			
			anim.SetTrigger("Fold");
			
			board.waitingForPlayer = true;
			
			break;

		case (int)KARMA_CARDS.BEAUTY_CONTEST:
			
			cardText.text = "You got 2nd place at a beauty contest!\nYou win the admiration of those around you.";

			anim.SetTrigger("Draw");
			
			while (anim.GetCurrentAnimatorStateInfo(0).IsName("KarmaCardDraw")){
				yield return new WaitForEndOfFrame();
			}
			
			while (!Input.GetMouseButton(0)){
				yield return new WaitForEndOfFrame();
			}
			
			anim.SetTrigger("Fold");
			
			board.waitingForPlayer = true;
			
			break;
			
		case (int)KARMA_CARDS.RECESSION:
			
			cardText.text = "Recession has hit the market. Everyone you know loses their jobs.";

			foreach (Piece p in board.gamePieces){
				if (p.job != "Rich"){
					p.hasJob = false;
					p.job = "";
				}
			}

			anim.SetTrigger("Draw");
			
			while (anim.GetCurrentAnimatorStateInfo(0).IsName("KarmaCardDraw")){
				yield return new WaitForEndOfFrame();
			}
			
			while (!Input.GetMouseButton(0)){
				yield return new WaitForEndOfFrame();
			}
			
			anim.SetTrigger("Fold");
			
			board.waitingForPlayer = true;
			
			break;
			
		case (int)KARMA_CARDS.JOB:

			cardText.text = "You studied Engineering by yourself for free using Coursera.";

			gamePiece.qualifications.Add("Engineering Degree");

			anim.SetTrigger("Draw");
			
			while (anim.GetCurrentAnimatorStateInfo(0).IsName("KarmaCardDraw")){
				yield return new WaitForEndOfFrame();
			}
			
			while (!Input.GetMouseButton(0)){
				yield return new WaitForEndOfFrame();
			}
			
			anim.SetTrigger("Fold");
			
			board.waitingForPlayer = true;
			
			break;
			
		case (int)KARMA_CARDS.NEGOTIATION:
			

			cardText.text = "Apperantly, it's OK to negotiate your salary!\nNext time you get a job, you get 1.5 times the salary";

			gamePiece.negotiationSkills = true;

			anim.SetTrigger("Draw");
			
			while (anim.GetCurrentAnimatorStateInfo(0).IsName("KarmaCardDraw")){
				yield return new WaitForEndOfFrame();
			}
			
			while (!Input.GetMouseButton(0)){
				yield return new WaitForEndOfFrame();
			}
			
			anim.SetTrigger("Fold");
			
			board.waitingForPlayer = true;
			
			break;
		
		}
	}
}
