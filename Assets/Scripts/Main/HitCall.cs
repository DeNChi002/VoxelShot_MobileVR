using UnityEngine;
using System.Collections;
using UnityEngine.Events;

/// <summary>
/// 判定伝達クラス
/// </summary>
public class HitCall : MonoBehaviour, IDamageable<int> {
	
	[SerializeField]
	GameObject callTarget;

	[SerializeField, Header("部位設定")]
	Region.TYPE regionType = Region.TYPE.NONE;

	/// <summary>
	/// 損傷ダメージ
	/// </summary>
	/// <param name="_damageValue">Damage value.</param>
	public void Damage(int _damageValue)
	{
		IRegionSettable i_region = callTarget.GetComponent<IRegionSettable>();
		if (i_region != null)
		{
			i_region.SetRegionType(regionType);
		}
		
		IDamageable<int> i_damage = callTarget.GetComponent<IDamageable<int>>();
		if (i_damage != null)
		{
			i_damage.Damage(_damageValue);
		}
	}

	public void SetGenPos(Vector3 _pos, float _power)
	{
		IDamageable<int> i_damage = callTarget.GetComponent<IDamageable<int>>();
		if (i_damage != null)
		{
			i_damage.SetGenPos(_pos, _power);
		}
	}

	public void BlowingOff(Vector3 _pos, float _power)
	{
		IDamageable<int> i_damage = callTarget.GetComponent<IDamageable<int>>();
		if (i_damage != null)
		{
			i_damage.BlowingOff(_pos, _power);
		}
	}
}
