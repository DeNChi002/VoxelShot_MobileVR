using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック:野営地戦車の砲弾
/// </summary>
public class GimmickTankBullet : BulletBase, IAttackable<int>, IDamageable<int>
{
	// ダメージ範囲
	static readonly float DAMAGE_AREA_RADUIUS = 6.0f;
	// 爆風の物理影響値
	static readonly float BOMB_FORCE = 10.0f;
	// ダメージ値
	static readonly int DAMAGE_POWER = 5;

	// 旋回速度
	static readonly float ROTATE_SPEED = 1.0f;
	// 補正値
	static readonly float FIX_POSITION_RATE = 1.0f;

	[SerializeField]
	GameObject prefabExplosion;

	[SerializeField]
	ParticleSystem effectFollow;

	[SerializeField]
	Rigidbody rigid;

	/// <summary>
	/// 損傷付与値の取得
	/// </summary>
	/// <returns>The attack value.</returns>
	public int GetAttackValue()
	{
		return bulletPower;
	}

	private void FixedUpdate()
	{
		//var rot = Quaternion.LookRotation(transform.position - ControllerManager.Instance.EyeCameraObj.transform.position);
		//transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * ROTATE_SPEED);

		//var pos = ControllerManager.Instance.EyeCameraObj.transform.position - transform.position;
		//pos.x = transform.position.x;
		//pos.z = transform.position.z;
		//pos.z = 0.0f;

		//rigid.AddForce(Vector3.Normalize(pos) * 0.02f, ForceMode.Impulse);
		//rigid.velocity = rigid.velocity + Vector3.Normalize(pos) * 0.01f;
		//transform.position = Vector3.Slerp( transform.position, ControllerManager.Instance.EyeCameraObj.transform.position, Time.deltaTime * FIX_POSITION_RATE);
	}

	void OnCollisionEnter(Collision c)
	{
		Instantiate(prefabExplosion, transform.position, prefabExplosion.transform.rotation);
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 20.0f, 1.0f);

		Collider[] targets = Physics.OverlapSphere(transform.position, DAMAGE_AREA_RADUIUS);
		foreach (Collider obj in targets)
		{
			IDamageable<int> i_damage = obj.gameObject.GetComponent<IDamageable<int>>();
			if (i_damage != null)
			{
				i_damage.BlowingOff(transform.position, BOMB_FORCE);
			}
		}
		Destroy(gameObject);
	}

	/// <summary>
	/// 損傷ダメージ
	/// </summary>
	/// <param name="_damageValue">Damage value.</param>
	public void Damage(int _damageValue)
	{
		Instantiate(prefabExplosion, transform.position, prefabExplosion.transform.rotation);
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 20.0f, 1.0f);

		Collider[] targets = Physics.OverlapSphere(transform.position, DAMAGE_AREA_RADUIUS);
		foreach (Collider obj in targets)
		{
			IDamageable<int> i_damage = obj.gameObject.GetComponent<IDamageable<int>>();
			if (i_damage != null)
			{
				i_damage.BlowingOff(transform.position, BOMB_FORCE);
			}

		}
		Destroy(gameObject);
	}

	/// <summary>
	/// ダメージ発生位置、威力設定
	/// </summary>
	/// <param name="_pos"></param>
	/// <param name="_power"></param>
	public void SetGenPos(Vector3 _pos, float _power)
	{

	}

	public void BlowingOff(Vector3 _pos, float _power)
	{

	}

	void Start()
	{
		bulletPower = DAMAGE_POWER;
	}
}
