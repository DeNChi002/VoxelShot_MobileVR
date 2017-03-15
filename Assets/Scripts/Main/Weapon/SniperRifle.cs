using UnityEngine;
using System.Collections;
//using VRTK;

/// <summary>
/// スナイパーライフル
/// </summary>
public class SniperRifle : GunBase {

	// 弾数
	static readonly int BULLET_NUM = 5;
	// リロード時間
	static readonly float RELOAD_TIME = 1.5f;

	protected void Start()
	{
		base.Start();
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
		FireBullet();
	}

	protected override void EndTrigger()
	{
		base.EndTrigger();
	}

	protected override void SetData()
	{
		bulletNum = BULLET_NUM;
		reloadTime = RELOAD_TIME;

		gunSeName = AUDIO_NAME.SE_GUN_SILENCER;
	}
}
