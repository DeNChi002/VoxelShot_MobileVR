using UnityEngine;
using System.Collections;
//using VRTK;

/// <summary>
/// 投擲武器基本クラス
/// </summary>
public class ThrowBase : MonoBehaviour, IAttackable<int>
{
	static readonly int BASE_POWER = 1;

	// リジッドボディ
	Rigidbody rigid = null;
	// 自身レンダラ
	MeshRenderer meshRenderer = null;

	//protected VRTK_ControllerEvents controllerEvents = null;
	GameObject currentUsingObject;
	bool isRelease = false;

	protected int basePower = BASE_POWER;

	/// <summary>
	/// 損傷付与値の取得
	/// </summary>
	/// <returns>The attack value.</returns>
	public int GetAttackValue()
	{
		return basePower;
	}

	protected void Start()
	{
		//base.Start();

		rigid = GetComponent<Rigidbody>();

		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material.SetFloat("_UseShiruetto", 0.0f);
	}

	// タッチ開始
	public void StartTouching(GameObject currentTouchingObject)
	{
		//base.StartTouching(currentTouchingObject);

		if (!isRelease)
		{
			meshRenderer.material.SetFloat("_UseShiruetto", 1.0f);
		}
	}

	// タッチ終了
	public void StopTouching(GameObject previousTouchingObject)
	{
		//base.StopTouching(previousTouchingObject);
		meshRenderer.material.SetFloat("_UseShiruetto", 0.0f);
	}

	public void StartUsing(GameObject usingObject)
	{
		//base.StartUsing(usingObject);

		//if (controllerEvents == null)
		//{
		//	currentUsingObject = usingObject;
		//	ControllerManager.Instance.SetVisible(currentUsingObject, false);
		//	controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();

		//	meshRenderer.material.SetFloat("_UseShiruetto", 0.0f);
		//	VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GRENADE_GRAB, transform.position, 20.0f, 1.0f);
		//}

		rigid.isKinematic = true;
	}
	
	public void Ungrabbed(GameObject previousGrabbingObject)
	{
		//base.Ungrabbed(previousGrabbingObject);

		ControllerManager.Instance.SetVisible(currentUsingObject, true);
		rigid.isKinematic = false;
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_THROWING, transform.position, 20.0f, 1.0f);
		
		meshRenderer.material.SetFloat("_UseShiruetto", 0.0f);
		isRelease = true;

		//controllerEvents = null;
	}
}
