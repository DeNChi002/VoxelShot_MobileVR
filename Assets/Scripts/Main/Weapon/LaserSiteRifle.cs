using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using VRTK;

/// <summary>
/// 照準器付きライフル
/// </summary>
public class LaserSiteRifle : GunBase {

	// 弾数
	static readonly int BULLET_NUM = 30;
	// リロード時間
	static readonly float RELOAD_TIME = 4.5f;
	// 射撃間隔
	static readonly float RAPPID_INTERVAL = 0.1f;

	bool isTrigger = false;
	float rappidTime = 0.0f;

	[SerializeField, Header("照準器")]
	Transform siteTransform;

	[SerializeField, Header("ラインレンダラ")]
	LineRenderer lineRanderer;

	protected void Update()
	{
		base.Update();

		if (isTrigger)
		{
			rappidTime -= Time.deltaTime;

			if (rappidTime < 0.0f)
			{
				if (magazineInfo.IsReloading)
				{
					isTrigger = false;
					VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GUN_RELOAD, transform.position);
				}
				else
				{
					FireBullet();
					rappidTime = RAPPID_INTERVAL;
				}
			}
		}
		
		lineRanderer.SetPosition(0, siteTransform.position);
		lineRanderer.SetPosition(1, siteTransform.position + (siteTransform.right * EFFECTIVE_RANGE));
	}

	protected override void OnTrigger()
	{
		//if (controllerActions == null)
		//{
		//	bool isRight = VRTK_SDK_Bridge.IsControllerRightHand(usingObject);
		//	controllerActions = usingObject.GetComponent<VRTK_ControllerActions>();

		//	controllerActions.TriggerHapticPulse((ushort)1000.0f, 0.1f, 0.01f);
			
		//	lineRanderer.enabled = true;
		//	return;
		//}

		isTrigger = true;
		rappidTime = 0.0f;
	}

	protected override void EndTrigger()
	{
		base.EndTrigger();

		isTrigger = false;
	}

	protected override void OnUnGrabbed() {

		base.OnUnGrabbed();

		lineRanderer.enabled = false;
	}

	protected override void SetData()
	{
		bulletNum = BULLET_NUM;
		reloadTime = RELOAD_TIME;

		//gunSeName = AUDIO_NAME.SE_GUN_SILENCER;
	}
}
