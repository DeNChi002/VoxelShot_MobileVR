using UnityEngine;
using System.Collections;
//using VRTK;

/// <summary>
/// ショットガン
/// </summary>
public class ShotGun : GunBase {

	// 弾数
	static readonly int BULLET_NUM = 7;
	// リロード時間
	static readonly float RELOAD_TIME = 4.0f;

	[SerializeField]
	Transform[] arrayBulletPoint;

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

	protected override void FireBullet()
	{
		if (isReload)
		{   // 空撃ち時
			VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_BLANK_TRIGGER, transform.position);
			return;
		}

		effectFire.Play();

		for (int i = 0; i < arrayBulletPoint.Length; ++i)
		{
			GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
			bulletClone.SetActive(true);

			GameObject fireClone = Instantiate(effectFire.gameObject, effectFire.transform.position, effectFire.transform.rotation) as GameObject;
			fireClone.SetActive(true);
			
			Vector3 moveDir = (arrayBulletPoint[i].position - bullet.transform.position);
			moveDir.x *= Random.Range(0.75f, 1.0f);
			moveDir.y *= Random.Range(0.75f, 1.0f);

			RaycastHit hit;
			if (Physics.Raycast(bullet.transform.position, moveDir, out hit, EFFECTIVE_RANGE))
			{   // 着弾
				if (!hit.transform.tag.Contains(TAG_NAME.TAG_ENEMY))
				{   // 敵以外
					Rigidbody rigid = hit.transform.GetComponent<Rigidbody>();
					if (rigid != null)
					{
						rigid.AddForceAtPosition(bullet.transform.forward * bulletForce, hit.point, ForceMode.Impulse);
					}
				}

				IDamageable<int> idamage = hit.transform.gameObject.GetComponent<IDamageable<int>>();
				if (idamage != null)
				{
					idamage.Damage(GetAttackValue());
				}
				bulletClone.GetComponent<BulletBase>().Set(hit.point);
			}
			else
			{   // 着弾対象なし
				bulletClone.GetComponent<BulletBase>().Set(bullet.transform.position + (moveDir.normalized * EFFECTIVE_RANGE));
			}

			if (rootInstance == null)
			{
				rootInstance = GameObject.Find("RootInstance").transform;
			}

			//fireClone.transform.parent = rootInstance.transform;

			if (cartridge != null)
			{
				GameObject cartridgeClone = Instantiate(cartridge, cartridge.transform.position, cartridge.transform.rotation) as GameObject;
				cartridgeClone.SetActive(true);
				cartridgeClone.transform.parent = rootInstance.transform;
				cartridgeClone.transform.localScale = cartridge.transform.localScale;
			}
		}

        VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GUN1, transform.position);
        //controllerActions.TriggerHapticPulse((ushort)3900.0f, 0.07f, 0.02f);

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
