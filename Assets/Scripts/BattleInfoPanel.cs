using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleInfoPanel : MonoBehaviour {

	[SerializeField] Text userName;
	[SerializeField] Text turnCount;

	bool isInited;
	int watcher;

	public bool Inited{ get { return isInited; } }
	public int Watcher{ get { return watcher; } }

	static BattleInfoPanel instance;
	
	public static BattleInfoPanel Instance {
		get {
			if(instance == null) {
				GameObject go = new GameObject("battle");
				instance = go.AddComponent<BattleInfoPanel>();
			}
			return instance;
		}
	}

	public void SetPanel(string name, int count) {
		userName.text = name;
		turnCount.text = count.ToString ();

		if (!isInited)
			isInited = true;
	}

	public void IncreaseWatcher() {
		watcher++;
	}

	void Awake() {
		if (instance == null)
			instance = this;
		isInited = false;
		watcher = 0;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
