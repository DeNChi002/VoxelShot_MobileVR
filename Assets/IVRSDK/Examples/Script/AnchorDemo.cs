using UnityEngine;
using System.Collections;

public class AnchorDemo : MonoBehaviour
{

    private bool m_focus;
    //public GameObject holde;
	// Use this for initialization
	void Start () {

		VREventListener.Get(gameObject).OnClickEvent = AnchorWidegt_OnClickEvent;
//		VREventListener.Get(gameObject).onHover = Anchor_OnHover;
		VREventListener.Get(gameObject).onDrag = Anchor_OnDrag;

		IVRTouchPad.Instance.AddKeyEvent(Back,IVRTouchPad.KeyLayout.Layout_1);
	}
	bool Back()
	{
        Application.Quit();
		return true;

	}
    private void Anchor_OnDrag(GameObject go, Vector2 delta)
    {
        UnityEngine.UI.Text text = GetComponentInChildren<UnityEngine.UI.Text>();
		text.text = "onDrag " + delta;
		Debug.Log("onDrag " + delta);
    }

    private void Anchor_OnHover(GameObject go, bool state)
    {
        UnityEngine.UI.Text text = GetComponentInChildren<UnityEngine.UI.Text>();
        m_focus = state;
		text.text = "onHover " + state + " " + VREventListener.Get(gameObject).hitPoint;
    }

    private void AnchorWidegt_OnClickEvent(GameObject go)
    {
		UnityEngine.UI.Text text = GetComponentInChildren<UnityEngine.UI.Text>();
		if(go)
		{

			text.text = "OnClick";
			Debug.Log("holde.SetActive(true)");
		}
		else
		{
			text.text = "Null OnClick";
		}
    }
	
	// Update is called once per frame
	void Update () {
        if (m_focus)
        {
            UnityEngine.UI.Text text = GetComponentInChildren<UnityEngine.UI.Text>();
			text.text = "onHover " + m_focus + " " + VREventListener.Get(gameObject).hitPoint;
        }
	}
}
