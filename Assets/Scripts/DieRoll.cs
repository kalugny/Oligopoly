using UnityEngine;
using System.Collections;

public class DieRoll : MonoBehaviour {

	public float minTimeToWaitAfterThrow = 0.1f;
	public float timeBetweenDieThrows = 0.1f;
	public float distanceFromDice = 0.5f;
	public float delayAfterDiceResult = 0.5f;

	public Die[] dice;
	public Camera diceCam;

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

	Vector3 avgDicePos(){
		Vector3 avg = Vector3.zero;
		foreach (Die d in dice){
			avg += d.transform.position;
		}
		avg /= dice.Length;
		return avg;

	}

	public IEnumerator RollDice(DieRollResult cb){

		if (!areDiceAtRest()){
			return false;
		}

		Vector3 savedCamPos = diceCam.transform.position;
		Quaternion savedCamRot = diceCam.transform.rotation;
		diceCam.camera.enabled = true;

		foreach (Die die in dice){

			die.transform.position = transform.position;
			die.transform.rotation = Random.rotation;

			die.rigidbody.AddForce(dieInitialForce);
			die.rigidbody.AddTorque(dieInitialTorque);

			yield return new WaitForSeconds(timeBetweenDieThrows);
			
		}

		yield return new WaitForSeconds(minTimeToWaitAfterThrow);

		while (!areDiceAtRest()){
			Vector3 avgPos = avgDicePos();
			diceCam.transform.position = Vector3.Lerp(savedCamPos, avgPos, distanceFromDice);
			diceCam.transform.LookAt(avgPos);
			yield return new WaitForEndOfFrame();
		}

		yield return new WaitForSeconds(delayAfterDiceResult);

		diceCam.camera.enabled = false;

		diceCam.transform.position = savedCamPos;
		diceCam.transform.rotation = savedCamRot;

		cb(dice[0].GetResult(), dice[1].GetResult());
	}
}
