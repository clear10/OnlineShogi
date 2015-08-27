using UnityEngine;
using System.Collections;

public class ScrollViewController : MonoBehaviour {

	Transform content;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void StartController() {
		content = transform.GetChild (0);
		History history = History.Instance;
		int count = history.Count ();
		//Debug.Log (count);
		GameObject prefab = Resources.Load<GameObject> ("Node");
		string str = history.Dump ();
		string[] words = str.Split ('\n');
		for (int i = 0; i<count; i++) {
			GameObject go = Instantiate(prefab) as GameObject;
			go.transform.SetParent(content, false);
			ScrollHistoryNode node = go.GetComponent<ScrollHistoryNode>();
			node.SetIndex(i);
			node.SetText(words[i]);
		}
		Init ();
	}

	void Init() {
		UserInfo first = GameLogic.Instance.GetFirstUser ();
		UserInfo last = GameLogic.Instance.GetLastUser ();
		first.InitHolds ();
		last.InitHolds ();
		PieceController.Instance.SetFirstPosition ();
	}

	public void ReproduceState(int index) {
		Init ();
		History history = History.Instance;
		PieceController pc = PieceController.Instance;
		for(int i = 0; i<=index; i++) {
			HistoryInfo info = history.Get(i);
			Piece piece = pc.GetByIndex(info.PieceId-1);
			piece.Move(info.Position, false);
			if(info.IsPromote)
				piece.Promote();
		}
	}
}
