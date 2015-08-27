using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LobbyController : MonoBehaviour {

	[SerializeField] InputField nameTxt;
	[SerializeField] InputField roomTxt;

	public string NameText{ get { return nameTxt.text; } }
	public string RoomText{ get { return roomTxt.text; } }

	// Use this for initialization
	void Start () {
		Init ();
	}

	void Init() {
		//Debug.Log ("Init");
		Transform canvas = GameObject.Find ("Canvas").transform;
		if(nameTxt == null)
			nameTxt = canvas.FindChild ("NameField").GetComponent<InputField> ();
		if(roomTxt == null)
			roomTxt = canvas.FindChild ("RoomField").GetComponent<InputField> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
