using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

	[SerializeField] Text fu;
	[SerializeField] Text kyosha;
	[SerializeField] Text keima;
	[SerializeField] Text gin;
	[SerializeField] Text kin;
	[SerializeField] Text hisya;
	[SerializeField] Text kaku;

	[SerializeField] bool isMine;

	// Use this for initialization
	void Start () {
		if (fu != null)
			fu.text = "0";
		if (kyosha != null)
			kyosha.text = "0";
		if (keima != null)
			keima.text = "0";
		if (gin != null)
			gin.text = "0";
		if (kin != null)
			kin.text = "0";
		if (hisya != null)
			hisya.text = "0";
		if (kaku != null) 
			kaku.text = "0";

		//GameManager.Instance.RegistUI (this, isMine);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void UpdateUI(Piece.Kind kind, int num) {
		Text target = null;
		switch (kind) {
		case Piece.Kind.fu:
			target = fu;
			break;
		case Piece.Kind.kyosha:
			target = kyosha;
			break;
		case Piece.Kind.keima:
			target = keima;
			break;
		case Piece.Kind.gin:
			target = gin;
			break;
		case Piece.Kind.kin:
			target = kin;
			break;
		case Piece.Kind.hisya:
			target = hisya;
			break;
		case Piece.Kind.kaku:
			target = kaku;
			break;
		default:
			break;
		}
		if (target == null)
			return;
		target.text = num.ToString ();
	}
}
