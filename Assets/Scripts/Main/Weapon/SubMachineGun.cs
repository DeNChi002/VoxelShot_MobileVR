using UnityEngine;
using System.Collections;
//using VRTK;

/// <summary>
/// サブマシンガン
/// </summary>
public class SubMachineGun : GunBase {

	// 弾数
	static readonly int BULLET_NUM = 30;
	// リロード時間
	static readonly float RELOAD_TIME = 3.0f;
	// 射撃間隔
	static readonly float RAPPID_INTERVAL = 0.08f;

	bool isTrigger = false;
	float rappidTime = 0.0f;

	protected void Update ()
	{
		base.Update();

		if( isTrigger)
		{
			rappidTime -= Time.deltaTime;

			if(rappidTime < 0.0f)
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
	}

	protected override void OnTrigger()
	{
		//if (controllerActions == null)
		//{
		//	bool isRight = VRTK_SDK_Bridge.IsControllerRightHand(usingObject);
		//	controllerActions = usingObject.GetComponent<VRTK_ControllerActions>();

		//	controllerActions.TriggerHapticPulse((ushort)1000.0f, 0.1f, 0.01f);
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

	protected override void SetData()
	{
		bulletNum = BULLET_NUM;
		reloadTime = RELOAD_TIME;
	}
}
