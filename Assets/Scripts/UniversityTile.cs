using UnityEngine;
using System.Collections;

public class UniversityTile : Tile {

	public string Degree;

	public override void OnLand (Piece gamePiece)
	{
		base.OnLand (gamePiece);

		gamePiece.qualifications.Add(Degree);
	}
}
