using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyController : MonoBehaviour {

	[SerializeField] InputField nameTxt;
	[SerializeField] InputField roomTxt;
	[SerializeField] Button button;

	// Use this for initialization
	void Start () {
	
	}

	void Init() {
		Transform canvas = GameObject.Find ("Canvas").transform;
		nameTxt = canvas.FindChild ("NameField").GetComponent<InputField> ();
		roomTxt = canvas.FindChild ("RoomField").GetComponent<InputField> ();
		button = canvas.FindChild ("Button").GetComponent<Button> ();
		button.onClick.RemoveAllListeners ();
		ShogiNetwork shogi = ShogiNetwork.Instance;
		button.onClick.AddListener (() => shogi.JoinRoom (nameTxt.text, roomTxt.text));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
