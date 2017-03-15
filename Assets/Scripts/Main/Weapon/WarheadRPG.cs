using UnityEngine;
using System.Collections;

/// <summary>
/// RPG弾頭
/// </summary>
public class WarheadRPG : BulletBase, IAttackable<int> {

	// ダメージ範囲
	static readonly float DAMAGE_AREA_RADUIUS = 6.0f;
	// 爆風の物理影響値
	static readonly float BOMB_FORCE = 10.0f;
	// ダメージ値
	static readonly int DAMAGE_POWER = 5;

	[SerializeField]
	GameObject prefabExplosion;

	[SerializeField]
	ParticleSystem effectFollow;

	/// <summary>
	/// 損傷付与値の取得
	/// </summary>
	/// <returns>The attack value.</returns>
	public int GetAttackValue()
	{
		return bulletPower;
	}

	void OnCollisionEnter(Collision c)
	{
		Instantiate(prefabExplosion, transform.position, prefabExplosion.transform.rotation);
		VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_EXPLOSION, transform.position, 20.0f, 1.0f);

		Collider[] targets = Physics.OverlapSphere(transform.position, DAMAGE_AREA_RADUIUS);
		foreach (Collider obj in targets)
		{
			IRegionSettable i_region = obj.gameObject.GetComponent<IRegionSettable>();
			if (i_region != null)
			{
				i_region.SetRegionType(Region.TYPE.NONE);
			}

			IDamageable<int> i_damage = obj.gameObject.GetComponent<IDamageable<int>>();
			if (i_damage != null)
			{
				i_damage.SetGenPos(transform.position, BOMB_FORCE);
				i_damage.Damage(GetAttackValue());
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
		Destroy(gameObject);
	}

	void Start()
	{
		bulletPower = DAMAGE_POWER;
	}
}
