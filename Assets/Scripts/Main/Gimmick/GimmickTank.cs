using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ギミック:野営地戦車
/// </summary>
public class GimmickTank : GimmickBase {

	static readonly int DEAD_BOMB_POWER = 5;

	// ダメージ範囲
	static readonly float DAMAGE_AREA_RADUIUS = 7.0f;
	// 爆風の物理影響値
	static readonly float BOMB_FORCE = 8.0f;

	// 旋回速度
	static readonly float ROTATE_SPEED = 1.0f;
	// 搭乗員ゾンビ生成間隔
	static readonly float CREATE_INTERVAL = 7.0f;
	// 搭乗員ゾンビ出現時移動量
	static readonly float MOVE_CREW_UP = 1.5f;

	[SerializeField] // 搭乗員ゾンビ
	GameObject prefabCrewZombi;

	[SerializeField] // 爆発エフェクト表示位置
	Transform[] arrayBombPoint;

	[SerializeField] // 損壊後コライダー
	BoxCollider[] arrayAfterCol;

	[SerializeField] // 炎上エフェクト
	GameObject[] arrayFire;

	[SerializeField] // ハッチ蓋
	Transform[] arrayLid;

	[SerializeField] // 搭乗員ゾンビ生成位置
	Transform[] arrayPopCrewPoint;

	[SerializeField] // 戦車本体部
	Transform tankTransform;

	[SerializeField] // 砲台部
	Transform turret;

	[SerializeField] // 戦車本体部リジッドボディ
	Rigidbody rigidTank;

	[SerializeField] // 砲台部リジッドボディ
	Rigidbody rigidTurret;

	[SerializeField] // 砲台部判定ルート
	GameObject rootTurretColider;

	[SerializeField] // 走行時煙エフェクト
	ParticleSystem[] arrayEffectSmoke;

	[SerializeField] // 砲弾発射時
	ParticleSystem effectBulletFire;

	[SerializeField] // パス再生時間
	float pathMoveTime = 100;

	[SerializeField] // 走行パス
	public string PathName = "Target Path Name";

	[SerializeField]
	NavMeshObstacle navMeshObstacle;

	[SerializeField]
	protected ParticleSystem prefabExplosion;

	[SerializeField] // 弾丸オブジェクト
	protected GameObject bullet;

	// 搭乗員インスタンス配列
	GameObject[] arrayCrewInstance;
	// 生成時間
	float[] arrayCreateCnt = { 5.0f, 7.0f };
	// ハッチ開閉フラグ
	bool[] arrayOpenLid;

	// 配列定義
	enum CrewIdx {
		RIGHT	= 0,
		LEFT	= 1,
		MAX		= 2,
	};

	/// <summary>
	/// 損壊時の処理
	/// </summary>
	protected override void ToBreak()
	{
		base.ToBreak();

		// 走行エフェクト停止
		foreach (ParticleSystem smoke in arrayEffectSmoke)
		{
			smoke.Stop();
		}
		navMeshObstacle.enabled = true;
		// パス移動停止
		iTween.Stop(tankTransform.gameObject);

		foreach (Transform t in arrayBombPoint)
		{
			WaitAfter(Random.Range(0.0f, 0.1f), () => { Instantiate(prefabExplosion, t.position, prefabExplosion.transform.rotation); });
		}

		VR_AudioManager.Instance.StopSE(AUDIO_NAME.SE_TANK_MOVE);
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 1.0f);

		foreach ( GameObject go in arrayFire )
		{
			go.SetActive(true);
		}

		Collider[] targets = Physics.OverlapSphere(tankTransform.position, DAMAGE_AREA_RADUIUS);

		foreach (Collider obj in targets)
		{
			if (!gameObject.Equals(obj.gameObject))
			{
				IRegionSettable i_region = obj.gameObject.GetComponent<IRegionSettable>();
				if (i_region != null)
				{
					i_region.SetRegionType(Region.TYPE.NONE);
				}

				IDamageable<int> idamage = obj.gameObject.GetComponent<IDamageable<int>>();
				if (idamage != null)
				{
					idamage.Damage(DEAD_BOMB_POWER);
				}
				else
				{
					if (!obj.tag.Contains(TAG_NAME.TAG_GUN)
					   && !obj.tag.Contains(TAG_NAME.TAG_THROW))
					{
						Rigidbody rigid = obj.GetComponent<Rigidbody>();
						if (rigid != null)
						{
							Vector3 forceDir = (obj.transform.position - transform.position).normalized;
							forceDir.x = forceDir.x * BOMB_FORCE;
							forceDir.y = (forceDir.y + Random.Range(0.8f, 2.5f)) * BOMB_FORCE;
							forceDir.z = forceDir.z * BOMB_FORCE;

							rigid.AddForceAtPosition(forceDir, transform.position, ForceMode.Impulse);
						}
					}
				}
			}
		}

		rootTurretColider.SetActive(false);

		foreach (BoxCollider col in arrayAfterCol)
		{
			col.enabled = true;
		}

		var rigidTurret = turret.gameObject.AddComponent<Rigidbody>();
		rigidTurret.mass = 100;
	}

	void OnPathMoveEnd()
	{
		foreach ( ParticleSystem smoke in arrayEffectSmoke )
		{
			smoke.Stop();
		}
		navMeshObstacle.enabled = true;
		VR_AudioManager.Instance.StopSE(AUDIO_NAME.SE_TANK_MOVE);
	}

	protected override void GimmickStart()
	{
		base.GimmickStart();

		//navMeshObstacle.enabled = false;

		arrayCrewInstance = new GameObject[(int)CrewIdx.MAX];
		arrayOpenLid = new bool[(int)CrewIdx.MAX];

		iTween.MoveTo( tankTransform.gameObject, iTween.Hash(
			"path", iTweenPath.GetPath(PathName),
			"time", pathMoveTime,
			"lookTime", 1.1f,
			"easeType", iTween.EaseType.linear,
			"oncomplete", "OnPathMoveEnd",
			"oncompletetarget", gameObject,
			"orienttopath", true));

		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_TANK_MOVE, transform.position, 17.5f, 1.0f, gameObject, true, true);

		WaitAfter( 20.0f, () => { ShotBullet(); });
		WaitAfter( 40.0f, () => { ShotBullet(); });
		WaitAfter( 60.0f, () => { ShotBullet(); });
	}

	/// <summary>
	/// 搭乗員ゾンビ生成
	/// </summary>
	/// <param name="_idx">対象配列要素</param>
	/// <returns></returns>
	GameObject CreateCrew( int _idx)
	{
		iTween.Stop(arrayLid[_idx].gameObject);
		iTween.RotateTo(arrayLid[_idx].gameObject, iTween.Hash("x", 100.0f, "time", 1.0f, "isLocal", true));
		arrayOpenLid[_idx] = true;

		Vector3 pos = arrayPopCrewPoint[_idx].position;
		pos.y -= MOVE_CREW_UP;

		var obj = Instantiate( prefabCrewZombi, turret );
		obj.transform.position = pos;

		iTween.MoveTo(obj, iTween.Hash("y", (obj.transform.localPosition.y + MOVE_CREW_UP), "time", 1.0f, "isLocal", true));
		obj.transform.SetEulerAnglesY(0.0f);
		obj.GetComponent<EnemyBase>().SetPopData(arrayPopCrewPoint[_idx], 0, false);
		
		return obj;
	}

	/// <summary>
	/// 砲弾発射
	/// </summary>
	private void ShotBullet()
	{
		if (isBreak)
		{   // 損壊後は処理しない
			return;
		}

		GameObject bulletClone = Instantiate(bullet, bullet.transform.position, bullet.transform.rotation) as GameObject;
		bulletClone.SetActive(true);
		bulletClone.GetComponent<Rigidbody>().isKinematic = false;

		var pos = ControllerManager.Instance.EyeCameraObj.transform.position - bullet.transform.position;
		pos = Vector3.Normalize(pos);

		float bulletSpeed = 2200.0f;
		Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
		rb.AddForce(pos * bulletSpeed);
		Destroy(bulletClone, 10.0f);

		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_TANK_CANNON, transform.position, 20.0f, 1.0f);

		effectBulletFire.Play();
	}

	private void FixedUpdate()
	{
		rigidTank.MovePosition( tankTransform.position );
		rigidTank.MoveRotation( tankTransform.rotation );

		rigidTurret.MovePosition( turret.position );
		rigidTurret.MoveRotation(turret.rotation);
	}

	void Update()
	{
		if (isBreak)
		{	// 損壊後は処理しない
			return;
		}

		if (!isStarted)
		{
			return;
		}

		// プレイヤー方向へ砲台部を旋回
		var rot = Quaternion.LookRotation(turret.position - ControllerManager.Instance.EyeCameraObj.transform.position);
		turret.rotation = Quaternion.Slerp(turret.rotation, rot, Time.deltaTime * ROTATE_SPEED);

		for (int i = 0; i < arrayOpenLid.Length; ++i)
		{
			if (arrayOpenLid[i] && arrayCrewInstance[i] == null)
			{
				iTween.Stop(arrayLid[i].gameObject);
				iTween.RotateAdd(arrayLid[i].gameObject, iTween.Hash("x", -100.0f, "time", 1.0f, "isLocal", true, "EaseType", iTween.EaseType.easeOutBounce));

				arrayOpenLid[i] = false;
			}
		}

		if (Timeboard.Instance.CurrentDispTime > 0)
		{
			for (int i = 0; i < arrayCreateCnt.Length; ++i)
			{   // 対象が生成されていない場合
				if (arrayCrewInstance[i] == null)
				{
					arrayCreateCnt[i] -= Time.deltaTime;

					if (arrayCreateCnt[i] <= 0.0f)
					{
						arrayCrewInstance[i] = CreateCrew(i);
						arrayCreateCnt[i] = CREATE_INTERVAL;
					}
				}
				else
				{   // プレイヤー方向を向く
					var rotTarget = Quaternion.LookRotation(arrayCrewInstance[i].transform.position - ControllerManager.Instance.EyeCameraObj.transform.position);

					arrayCrewInstance[i].transform.rotation = rotTarget;
					arrayCrewInstance[i].transform.AddEulerAnglesY(180.0f);
				}
			}
		}
	}
}
