using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class UIPiece : MonoBehaviour {

	bool isSelected = false;
	string kind;

	Piece target;

	// Use this for initialization
	void Start () {
		target = null;
		Button button = GetComponent<Button> ();
		button.onClick.RemoveAllListeners ();
		button.onClick.AddListener (() => this.OnButtonClicked ());
	}

	public void SetKind(string s) {
		kind = s;
	}

	public void OnButtonClicked() {
		UserInfo me = GameLogic.Instance.GetMe ();
		if (me.GetRole () != UserInfo.Role.Player)
			return;
		if (!me.IsTurn)
			return;
		if (me.IsActed)
			return;

		if (isSelected)
			UnSelect ();
		else {
			PickUpKind ();
		}
	}

	void PickUpKind() {
		Debug.Log ("Select");
		isSelected = true;
		PieceController controller = PieceController.Instance;
		controller.CloseGuideArea ();
		Piece piece = controller.FindPassivePiece (kind);
		if (piece == null) {
			isSelected = false;
			return;
		}
		target = piece;
		piece.PickUp ();
	}

	void UnSelect() {
		Debug.Log ("UnSelect");
		isSelected = false;
		if (target == null)
			return;
		target.UnSelect ();
		target = null;
	}
}
