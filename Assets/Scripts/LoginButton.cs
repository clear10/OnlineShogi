using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoginButton : MonoBehaviour {

	[SerializeField] LobbyController controller;
	
	// Use this for initialization
	void Start () {
		Button button = GetComponent<Button> ();
		ShogiNetwork net = ShogiNetwork.Instance;
		button.onClick.RemoveAllListeners ();
		//button.onClick.AddListener (() => Debug.Log("hoge"));		
		button.onClick.AddListener (() => net.JoinRoom(controller));
	}
}
