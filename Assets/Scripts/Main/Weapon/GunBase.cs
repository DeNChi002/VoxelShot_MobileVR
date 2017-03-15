using UnityEngine;
using System.Collections;
//using VRTK;

/// <summary>
/// 銃基本クラス
/// </summary>
public class GunBase : MonoBehaviour, IAttackable<int> {
	
	// 有効射程
	protected static readonly float EFFECTIVE_RANGE = 100.0f;
	// 銃弾の物理影響値
	static readonly float BULLET_FORCE = 0.5f;
	// マップ落下、一定距離離れた場合の復帰判定時間
	static readonly float FALL_RETURN_TIME = 3.0f;
	// 復帰判定距離
	static readonly float FALL_RETURN_DISTANCE = 1.5f;

	public bool IsReload {
		get { return isReload; }
	}

	[SerializeField] // 射撃時用ライト
	protected GameObject lightFire;

	//[SerializeField] // 掴み解除実行ボタン
	//protected VRTK_ControllerEvents.ButtonAlias unGrabButton = VRTK_ControllerEvents.ButtonAlias.Undefined;

	[SerializeField] // 弾丸オブジェクト
	protected GameObject bullet;

	[SerializeField] // 薬莢オブジェクト
	protected GameObject cartridge = null;

	[SerializeField] // 発射時エフェクト
	protected ParticleSystem effectFire;

	[SerializeField] // 残弾情報
	protected MagazineInfo magazineInfo;

	// インスタンス生成先
	protected Transform rootInstance = null;
	// リジッドボディ
	Rigidbody rigid = null;
	// 自身レンダラ
	MeshRenderer meshRenderer = null;
	// 自身コライダ
	MeshCollider meshCollider = null;

	// 弾丸単位威力
	protected int bulletPower = 1;
	// 着弾の物理影響値
	protected float bulletForce = 10.0f;

	//protected VRTK_ControllerActions controllerActions = null;
	//protected VRTK_ControllerEvents controllerEvents = null;

	// 使用元オブジェクト（コントローラー）
	GameObject currentUsingObject = null;

	protected int bulletNum = 1;
	protected int currentBulletNum;

	protected float reloadTime = 1.0f;
	protected bool isReload;
	
	// 銃SE
	protected string gunSeName = AUDIO_NAME.SE_GUN1;

	// 開始配置位置
	Vector3 startLocalPos;
	// 開始時親オブジェクト
	Transform startParent;

	// 地形に投げ出された状態
	bool isMapFall;
	// 落下時の復帰待ち時間
	float fallReturnTime;

	/// <summary>
	/// 損傷付与値の取得
	/// </summary>
	/// <returns>The attack value.</returns>
	public int GetAttackValue()
	{
		return bulletPower;
	}

	protected void Start()
	{
		SetData();

		rootInstance = (GameObject.Find("RootInstance") as GameObject).transform;
		rigid = GetComponent<Rigidbody>();

		meshRenderer = GetComponent<MeshRenderer>();
		meshRenderer.material.SetFloat("_UseShiruetto", 0.0f);

		meshCollider = GetComponent<MeshCollider>();

		currentBulletNum = bulletNum;
		magazineInfo.Set(bulletNum, reloadTime);

		startLocalPos = transform.localPosition;
		startParent = transform.parent;
	}

	// タッチ開始
	public void StartTouching(GameObject currentTouchingObject)
	{
		//base.StartTouching(currentTouchingObject);
		meshRenderer.material.SetFloat("_UseShiruetto", 1.0f);
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

		if (currentUsingObject == null || !currentUsingObject.Equals(usingObject))
		{
			//if (controllerEvents == null)
			//{
			//	controllerEvents = usingObject.GetComponent<VRTK_ControllerEvents>();
			//}
			currentUsingObject = usingObject;

			ControllerManager.Instance.SetVisible(currentUsingObject, false);
			VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GUN_GRAB, transform.position, 20.0f, 1.0f);

			meshRenderer.material.SetFloat("_UseShiruetto", 0.0f);
			meshCollider.enabled = false;
		}
		else
		{
			if (isReload)
			{   // 空撃ち時
				VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_BLANK_TRIGGER, transform.position);
				return;
			}
		}

		rigid.isKinematic = true;
		magazineInfo.SetDispEnable(true);

		OnTrigger();
	}

	public void StopUsing(GameObject previousUsingObject)
	{
		meshRenderer.material.SetFloat("_UseShiruetto", 0.0f);

		EndTrigger();
	}

	protected void Update()
	{
		//if (controllerEvents != null && controllerEvents.IsButtonPressed(unGrabButton) && IsGrabbed())
		//{   // 持っているものを手放す
		//	ForceStopInteracting();

		//	OnUnGrabbed();

		//	rigid.isKinematic = false;
		//	rigid.AddForce(controllerEvents.GetVelocity(), ForceMode.Impulse);

		//	// コントローラーモデル描画復帰
		//	ControllerManager.Instance.SetVisible(currentUsingObject, true);

		//	VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GUN_GRAB, transform.position, 20.0f, 1.0f);

		//	magazineInfo.SetDispEnable(false);

		//	controllerEvents = null;
		//	controllerActions = null;

		//	currentUsingObject = null;
		//	meshRenderer.material.SetFloat("_UseShiruetto", 0.0f);

		//	meshCollider.enabled = true;
		//}

		if (isReload)
		{
			if (!magazineInfo.IsReloading)
			{
				currentBulletNum = bulletNum;
				isReload = false;

				//if (IsGrabbed())
				//{
				//	VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_GUN_RELOAD, transform.position);
				//}

				OnReloadComplete();
			}
		}

		// 地形に落下した状態でプレイヤーから一定距離離れている場合
		// 初期位置に戻す
		if (isMapFall)
		{
			var distance = Vector3.Distance(transform.position, ControllerManager.Instance.EyeCameraObj.transform.position);

			if (distance >= FALL_RETURN_DISTANCE)
			{
				fallReturnTime += Time.deltaTime;

				if (fallReturnTime > FALL_RETURN_TIME)
				{
					transform.parent = startParent;
					transform.localPosition = startLocalPos;
					isMapFall = false;
					fallReturnTime = 0.0f;
				}
			}
		}
	}

	protected virtual void FireBullet()
	{
		effectFire.Play();

		GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
		bulletClone.SetActive(true);

		GameObject fireClone = Instantiate(effectFire.gameObject, effectFire.transform.position, effectFire.transform.rotation) as GameObject;
		fireClone.SetActive(true);

		RaycastHit hit;
		if (Physics.Raycast(bullet.transform.position, transform.right, out hit, EFFECTIVE_RANGE))
		{   // 着弾

			Collider[] targets = Physics.OverlapSphere(hit.point, 0.1f);
			foreach (Collider obj in targets)
			{
				if (!obj.transform.tag.Contains(TAG_NAME.TAG_ENEMY))
				{   // 敵以外
					Rigidbody rigid = hit.transform.GetComponent<Rigidbody>();
					if (rigid != null)
					{
						rigid.AddForceAtPosition(bullet.transform.forward * bulletForce, hit.point, ForceMode.Impulse);
					}
				}

				IDamageable<int> i_damage = obj.transform.gameObject.GetComponent<IDamageable<int>>();
				if (i_damage != null)
				{
					i_damage.SetGenPos(transform.position, BULLET_FORCE);
					i_damage.Damage(GetAttackValue());
				}
				break;
			}
			bulletClone.GetComponent<BulletBase>().Set(hit.point);
		}
		else
		{   // 着弾対象なし
			bulletClone.GetComponent<BulletBase>().Set(bullet.transform.position + (transform.right * EFFECTIVE_RANGE));
		}

		if (rootInstance == null)
		{
			rootInstance = GameObject.Find("RootInstance").transform;
		}

		fireClone.transform.parent = transform.parent;

		if (cartridge != null)
		{
			GameObject cartridgeClone = Instantiate(cartridge, cartridge.transform.position, cartridge.transform.rotation) as GameObject;
			cartridgeClone.SetActive(true);
			cartridgeClone.transform.parent = rootInstance.transform;
			cartridgeClone.transform.localScale = cartridge.transform.localScale;
		}

		//if(lightFire != null)
		//{
		//	lightFire.SetActive(true);
		//	StartCoroutine(SetLightFire());
		//}

		VR_AudioManager.Instance.PlaySE(gunSeName, transform.position);

		//Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
		//rb.AddForce(bullet.transform.forward * bulletSpeed);
		//Destroy(bulletClone, bulletLife);

		//controllerActions.TriggerHapticPulse((ushort)3900.0f, 0.07f, 0.02f);

		--currentBulletNum;
		if( currentBulletNum <= 0)
		{
			isReload = true;
		}
		
		magazineInfo.Sub();
	}

	void OnCollisionEnter(Collision collision)
	{
		// マップに落下した場合は位置を独立させる
		//if (collision.gameObject.tag.Contains(TAG_NAME.TAG_MAP) && !IsGrabbed())
		//{
		//	transform.parent = null;
		//	fallReturnTime = 0.0f;

		//	isMapFall = true;
		//}
	}

	protected IEnumerator SetLightFire()
	{
		yield return new WaitForSeconds(0.1f);

		lightFire.SetActive(false);
	}

	public void CallTrigger()
	{
		OnTrigger();
	}

	protected virtual void SetData() { }

	protected virtual void OnReloadComplete() { }
	protected virtual void OnTrigger() { }
	protected virtual void OnUnGrabbed() { }
	protected virtual void EndTrigger() { }
}
