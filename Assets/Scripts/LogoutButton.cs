using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LogoutButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Button button = GetComponent<Button> ();
		ShogiNetwork net = ShogiNetwork.Instance;
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener (() => net.LeaveRoom ());
	}
}
