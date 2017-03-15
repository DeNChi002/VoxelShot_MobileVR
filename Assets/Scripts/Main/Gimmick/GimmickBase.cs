using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック基本クラス
/// </summary>
public class GimmickBase : MonoBehaviorExpansion, IDamageable<int> {
	
	[SerializeField, Header("待ち時間")]
	protected　float delay = 0.0f;

	[SerializeField, Header("破壊時スコア")]
	protected　int breakScore = 0;

	[SerializeField, Header("耐久値")]
	protected int hp = 1;

	protected Vector3 enterPos;
	protected bool isBreak; 	// 損壊状態
	protected bool isStarted;   //  開始状態

	protected Vector3 posGenDamage;
	protected float powerGenDamage;

	/// <summary>
	/// 手動ダメージ
	/// </summary>
	public void OnManualDamage()
	{
		//OnDamage();
	}

	/// <summary>
	/// 損壊時の処理
	/// </summary>
	protected virtual void ToBreak()
	{
		isBreak = true;

		// 破壊時のスコアが設定されていれば
		if (breakScore > 0)
		{
			var resScore = Resources.Load("EffectDefeatScore") as GameObject;
			// 撃破スコア表示
			var scoreObj = Instantiate(resScore, new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z), Quaternion.identity);
			scoreObj.GetComponent<EffectDefeatScore>().Set(breakScore);
			// スコア加算通知
			GameManager.Instance.AddScore(breakScore);
		}
	}

	/// <summary>
	/// 損傷ダメージ
	/// </summary>
	/// <param name="_damageValue">Damage value.</param>
	public void Damage(int _damageValue)
	{
		if (hp > 0 && (hp -= _damageValue) <= 0)
		{
			ToBreak();
		}
	}

	/// <summary>
	/// ダメージ発生位置、威力設定
	/// </summary>
	/// <param name="_pos"></param>
	/// <param name="_power"></param>
	public void SetGenPos(Vector3 _pos, float _power)
	{
		posGenDamage = _pos;
		powerGenDamage = _power;
	}

	public void BlowingOff(Vector3 _pos, float _power)
	{

	}

	void OnTriggerEnter(Collider other)
	{
		IAttackable<int> i_attack = other.gameObject.GetComponent<IAttackable<int>>();

		if (i_attack != null && hp>0)
		{
			enterPos = other.transform.position;
			Damage(i_attack.GetAttackValue());
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		IAttackable<int> i_attack = collision.gameObject.GetComponent<IAttackable<int>>();

		if (i_attack != null && hp>0)
		{
			enterPos = collision.transform.position;
			Damage(i_attack.GetAttackValue());
		}
	}

	protected void Start()
	{
		InitStatus();

		if (delay > 0.0f)
		{
			WaitAfter(delay, () => { GimmickStart(); });
		}
		else
		{
			GimmickStart();
		}
	}

	// 各設定値初期化
	protected virtual void InitStatus() { }
	// ギミック動作開始時
	protected virtual void GimmickStart() 
	{
		isStarted = true;
	}
}
