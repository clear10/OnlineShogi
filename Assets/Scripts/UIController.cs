using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

	public List<string> keys;
	public List<Text> texts;

	Dictionary<string, Text> ui;
	Dictionary<string, int> holds;

	// Use this for initialization
	void Start () {
		Init ();
	}

	void Init() {
		if (keys == null || texts == null)
		return;
		if (keys.Count <= 0 || texts.Count <= 0)
			return;
		if (keys.Count != texts.Count) {
			Debug.LogWarning("keysとtextsのサイズを一致させてください");
			string s = (keys.Count < texts.Count) ? "keys" : "texts";
			Debug.LogWarning(s + "のサイズ分だけ作成します");
		}

		ui = new Dictionary<string, Text> ();
		holds = new Dictionary<string, int> ();
		
		for (int i=0;;) {
			InitButton(keys[i], texts[i]);
			AddUI(keys[i], texts[i]);
			i++;
			if(i >= keys.Count || i >= texts.Count) break;
		}

		RefreshAll ();
	}

	public void AddUI(string key, Text txt) {
		if(ui.ContainsKey(key)) return;
		ui.Add(key, txt);
		holds.Add (key, 0);
	}

	void InitButton(string kind, Text text) {
		UIPiece button = text.transform.parent.GetComponent<UIPiece> ();
		button.SetKind (kind);
	}

	public void RefreshAll() {
		foreach (string key in ui.Keys) {
			ui[key].text = holds[key].ToString();
		}
	}

	public void UpdateUI(string key, int value) {
		if (!holds.ContainsKey (key))
			return;
		holds [key] = value;
		RefreshAll ();
		//Debug.LogError ("UpdateUI called");
	}
}
