using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

[RequireComponent(typeof(UnityEngine.UI.Text))]
public class ShowInputInfo : MonoBehaviour, IOnHoverHandler, IDragHandler, IScrollHandler, 
    IPointerClickHandler, IBeginDragHandler
{

    private static ShowInputInfo mInstance;
    public static ShowInputInfo Instance
    {
        get { return mInstance; }
    }

    private bool isHover = false;
    private UnityEngine.UI.Text mText;

    // Use this for initialization
    void Start () {
        mInstance = this;
        mText = GetComponent<UnityEngine.UI.Text>();
    }

	// Update is called once per frame
	void Update () {
        mText.text = OnHoverString + "\n" + DragString + "\n" + ScrollString + "\n";
    }

    public void SetShowInfo(string msg)
    {
        if (!isHover)
            mText.text += msg;
    }

    private string OnHoverString = "Try to hove me!";
    private string DragString = string.Empty;
    private string ScrollString = string.Empty;

    void IOnHoverHandler.OnHover(BaseEventData eventData)
    {
        IVRRayPointerEventData pointer = eventData as IVRRayPointerEventData;
        if (pointer.HitResults.Contains(gameObject))
        {
            isHover = true;
            OnHoverString = "hit point : " + pointer.HitPoints[pointer.HitResults.IndexOf(gameObject)].ToString();
        }
        else
        {
            OnHoverString = "Try to hove me!";
            isHover = false;
            DragString = string.Empty;
            ScrollString = string.Empty;
        }
    }

    private Vector2 startDragPosition = Vector3.zero;

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        IVRRayPointerEventData pointer = eventData as IVRRayPointerEventData;
        DragString = "Drag:" + (pointer.TouchPadPosition - startDragPosition).ToString();
    }

    void IScrollHandler.OnScroll(PointerEventData eventData)
    {
        ScrollString = "Scroll:" + eventData.scrollDelta;
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        mText.text += "Click";
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        IVRRayPointerEventData pointer = eventData as IVRRayPointerEventData;
        startDragPosition = pointer.TouchPadPosition;
    }
}
