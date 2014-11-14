using UnityEngine;
using System.Collections;

public class DieRoll : MonoBehaviour {

	public float minTimeToWaitAfterThrow = 0.1f;

	public Die[] dice;

	public Vector3 dieInitialForce;
	public Vector3 dieInitialTorque;

	public delegate void DieRollResult(int first, int second);

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	bool areDiceAtRest(){
		foreach (Die die in dice){
			if (die.GetResult() == 0){
				return false;
			}
		}
		return true;
	}

	public IEnumerator RollDice(DieRollResult cb){

		if (!areDiceAtRest()){
			return false;
		}

		foreach (Die die in dice){

			die.transform.position = transform.position;
			die.transform.rotation = Random.rotation;

			die.rigidbody.AddForce(dieInitialForce);
			die.rigidbody.AddTorque(dieInitialTorque);
			
		}
		yield return new WaitForSeconds(minTimeToWaitAfterThrow);

		while (!areDiceAtRest()){
			yield return new WaitForEndOfFrame();
		}

		cb(dice[0].GetResult(), dice[1].GetResult());
	}
}
