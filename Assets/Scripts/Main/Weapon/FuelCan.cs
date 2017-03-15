using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 配置物:燃料缶
/// </summary>
public class FuelCan : ThrowBase, IDamageable<int>
{
	// ダメージ範囲
	static readonly float DAMAGE_AREA_RADUIUS = 5.0f;
	// 爆風の物理影響値
	static readonly float BOMB_FORCE = 8.0f;

	[SerializeField]
	protected ParticleSystem prefabExplosion;

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
					idamage.Damage(GetAttackValue());
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
		Destroy(gameObject);
	}

	public void Ungrabbed(GameObject previousGrabbingObject)
	{
		base.Ungrabbed(previousGrabbingObject);

		transform.parent = null;
	}

	public void SetGenPos(Vector3 _pos, float _power)
	{

	}

	public void BlowingOff(Vector3 _pos, float _power)
	{

	}

	protected void Start()
	{
		base.Start();

		basePower = 2;
	}
}
