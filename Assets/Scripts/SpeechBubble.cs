using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SpeechBubble : MonoBehaviour {

	public float maxWidth = 100;
	public float margin = 10;
	public float growTime = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	public IEnumerator Say(string text, float time){
		Text textComponent = GetComponentInChildren<Text>();

		textComponent.text = text;
		float width = Mathf.Min (textComponent.preferredWidth, maxWidth) + margin;

		GetComponent<RectTransform>().sizeDelta = new Vector2(width, margin);

		float height = textComponent.preferredHeight + margin;

		textComponent.text = "";

		textComponent.CrossFadeAlpha(1, 0.2f, false);

		for (float t = 0; t < growTime; t += Time.deltaTime){
			GetComponent<RectTransform>().sizeDelta = new Vector2(t / growTime * width, t / growTime * height);
			yield return new WaitForEndOfFrame();
		}
		GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

		for (float t = 0; t < time && !Input.GetKey(KeyCode.Space); t += Time.deltaTime){
			int charsToShow = Mathf.FloorToInt(text.Length * t / time);
			textComponent.text = text.Substring(0, charsToShow);
			yield return new WaitForEndOfFrame();
		}
		textComponent.text = text;

		while (Input.GetKey(KeyCode.Space)){
			yield return new WaitForEndOfFrame();
		}
		while (!Input.GetKey(KeyCode.Space)){
			yield return new WaitForEndOfFrame();
		}

		textComponent.CrossFadeAlpha(0, 0.2f, false);

		for (float t = 0; t < growTime; t += Time.deltaTime){
			GetComponent<RectTransform>().sizeDelta = new Vector2(width - t / growTime * width, height - t / growTime * height);
			yield return new WaitForEndOfFrame();
		}
		GetComponent<RectTransform>().sizeDelta = Vector2.zero;

	}
}
