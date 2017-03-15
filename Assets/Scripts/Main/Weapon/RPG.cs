using UnityEngine;
using System.Collections;
//using VRTK;

/// <summary>
/// RPG（携帯対戦車グレネードランチャー）
/// </summary>
public class RPG : GunBase {

	// 弾数
	static readonly int BULLET_NUM = 1;
	// リロード時間
	static readonly float RELOAD_TIME = 6.0f;

	[SerializeField]
	GameObject warheadParts;
	
	MeshRenderer partsMeshRenderer = null;

	float bulletSpeed = 10.0f;
	float bulletLife = 10.0f;

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

	protected override void OnReloadComplete()
	{
		warheadParts.SetActive(true);
	}

	protected void Start()
	{
		base.Start();

		partsMeshRenderer = warheadParts.GetComponent<MeshRenderer>();
		partsMeshRenderer.material.SetFloat("_UseShiruetto", 0.0f);
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

		if (warheadParts.activeSelf)
		{
			FireBullet();
		}
	}

	protected override void EndTrigger()
	{
		base.EndTrigger();
	}

	protected override void FireBullet()
	{
		if (isReload)
		{   // 空撃ち時
			VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_BLANK_TRIGGER, transform.position);
			return;
		}

		effectFire.Play();
		warheadParts.SetActive(false);

		GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
		bulletClone.SetActive(true);
		bulletClone.GetComponent<Rigidbody>().isKinematic = false;

		GameObject fireClone = Instantiate(effectFire.gameObject, effectFire.transform.position, effectFire.transform.rotation) as GameObject;
		fireClone.SetActive(true);

		if (rootInstance != null)
		{
			bulletClone.transform.parent = rootInstance.transform;
			fireClone.transform.parent = rootInstance.transform;
		}
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GUN_RPG, transform.position);
		bulletClone.transform.localScale = bullet.transform.localScale;

		bulletSpeed = 2500.0f;
		Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
		rb.AddForce(bullet.transform.right * bulletSpeed);
		Destroy(bulletClone, bulletLife);

		//controllerActions.TriggerHapticPulse((ushort)3900.0f, 0.5f, 0.02f);

		--currentBulletNum;
		if (currentBulletNum <= 0)
		{
			isReload = true;
		}

		magazineInfo.Sub();
	}

	protected override void SetData()
	{
		bulletNum = BULLET_NUM;
		reloadTime = RELOAD_TIME;
	}
}
