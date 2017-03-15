using UnityEngine;
using System.Collections;
//using VRTK;

/// <summary>
/// ガトリングガン
/// </summary>
public class GatlingGun : GunBase {

	// 弾数
	static readonly int BULLET_NUM = 500;
	// リロード時間
	static readonly float RELOAD_TIME = 7.0f;
	// 射撃間隔
	static readonly float RAPPID_INTERVAL = 0.05f;

	// 回転速度
	static readonly float ROT_ADD_SPEED = 30.0f;
	static readonly float ROT_SUB_SPEED = 1.0f;

	// 回転最高速度
	static readonly float ROT_SPEED_MAX = 15.0f;

	// 銃身パーツ
	[SerializeField]
	GameObject barrelParts;

	bool isTrigger = false;

	float barrelAngle = 0.0f;
	float addAngle = 0.0f;

	float rappidTime = 0.0f;
	MeshRenderer partsMeshRenderer = null;

	// タッチ開始
	public void StartTouching(GameObject currentTouchingObject)
	{
		base.StartTouching(currentTouchingObject);
		partsMeshRenderer.material.SetFloat("_UseShiruetto", 1.0f);
	}

	// タッチ終了
	public void StopTouching(GameObject previousTouchingObject)
	{
		base.StopTouching(previousTouchingObject);
		partsMeshRenderer.material.SetFloat("_UseShiruetto", 0.0f);
	}

	public void StopUsing(GameObject previousUsingObject)
	{
		base.StopUsing(previousUsingObject);
		partsMeshRenderer.material.SetFloat("_UseShiruetto", 0.0f);
	}

	protected void Start()
	{
		base.Start();

		partsMeshRenderer = barrelParts.GetComponent<MeshRenderer>();
		partsMeshRenderer.material.SetFloat("_Outline", 0.0f);
	}

	protected void Update()
	{
		base.Update();

		if (isTrigger)
		{
			addAngle = Mathf.Clamp(addAngle + (ROT_ADD_SPEED * Time.deltaTime), 0.0f, ROT_SPEED_MAX);

			// 銃身の回転速度が一定以上であれば射撃判定
			if (addAngle >= ROT_SPEED_MAX)
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

		}else
		{
			addAngle = Mathf.Clamp(addAngle - (ROT_SUB_SPEED * Time.deltaTime), -ROT_SPEED_MAX, 0.0f);
		}
		barrelAngle = Mathf.Clamp(barrelAngle + addAngle, 0.0f, ROT_SPEED_MAX);
		barrelParts.transform.Rotate(new Vector3(barrelAngle, 0.0f, 0.0f));
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

		addAngle = 0.0f;
		rappidTime = 0.0f;
	}

	protected override void EndTrigger()
	{
		base.EndTrigger();

		isTrigger = false;
		addAngle = 0.0f;
	}

	protected override void SetData()
	{
		bulletNum = BULLET_NUM;
		reloadTime = RELOAD_TIME;
	}
}
