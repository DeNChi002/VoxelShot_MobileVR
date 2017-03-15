using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BulletCall : MonoBehaviour, IDamageable<int> {

	[SerializeField]
	private UnityEvent events = new UnityEvent();

	public void OnThrowEnter()
	{
		events.Invoke();
	}

	void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag.Contains(TAG_NAME.TAG_BULLET))
		{
			events.Invoke();
		}
	}

	/// <summary>
	/// 損傷ダメージ
	/// </summary>
	/// <param name="_damageValue">Damage value.</param>
	public void Damage(int _damageValue)
	{
		events.Invoke();
	}

	public void SetGenPos(Vector3 _pos, float _power)
	{

	}

	public void BlowingOff(Vector3 _pos, float _power)
	{

	}
}
