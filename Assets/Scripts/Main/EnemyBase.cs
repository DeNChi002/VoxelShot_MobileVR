using UnityEngine;
using System.Collections;

/// <summary>
/// 敵基本クラス
/// </summary>
public class EnemyBase : MonoBehaviorExpansion, IDamageable<int>, IRegionSettable
{
	static readonly float ATTACK_INTERVAL = 5.0f;
	static readonly int SCORE_HEAD_SHOT = 50;

	// 歩行時ボイス再生判定間隔
	static readonly float VOICE_INTERVAL_WALK = 5.0f;
	// 歩行時ボイス判定有効距離（対プレイヤー位置）
	static readonly float VOICE_CHECK_DISTANCE = 15.0f;

	[SerializeField, Header("初期化種別")]
	InitType initType;

	[SerializeField, Header("追加SE種別")]
	SoundType soundType;

	[SerializeField] // 耐久値
	protected int hp;

	[SerializeField] // ベーススコア
	protected int baseScore;

	[SerializeField, Header("目標地点到達後、攻撃するか")]
	protected bool isAttacker = true;

	[SerializeField, Header("経路移動しない")]
	protected bool isDontMove;

	[SerializeField] // 自身レンダラ
	SkinnedMeshRenderer thisRenderer;

	[SerializeField] // 差分マテリアル
	protected Material[] arrayChangeMat;

	[SerializeField]
	protected UnityEngine.AI.NavMeshAgent agent;

	[SerializeField]
	protected Animator animator;

	[SerializeField]
	Transform modelRoot;

	[SerializeField]
	Rigidbody hitColliderRigid;

	// 移動先
	Transform moveTarget;

	// ボーナススコア設定値
	protected int bonusScore;

	// 最終ダメージ部位
	protected Region.TYPE regionType = Region.TYPE.NONE;
	// ダメージ発生源位置
	protected Vector3 damageGenPos;
	protected float damageGenPower;

	// 吹き飛び状態
	bool isBlowing;

	// ダメージひるみ状態
	bool isDamageAnim;

	// 歩行時ボイス
	string[] VOICE_WALK_LIST = {

		AUDIO_NAME.SE_ZOMBI_WALK_0,
		AUDIO_NAME.SE_ZOMBI_WALK_1,
		AUDIO_NAME.SE_ZOMBI_WALK_2,
		AUDIO_NAME.SE_ZOMBI_WALK_3,
		AUDIO_NAME.SE_ZOMBI_WALK_4,
		AUDIO_NAME.SE_ZOMBI_WALK_5,
		AUDIO_NAME.SE_ZOMBI_WALK_6,
	};

	// 死亡時ボイス
	string[] VOICE_DEAD_LIST = {

		AUDIO_NAME.SE_ZOMBI_DEAD_0,
		AUDIO_NAME.SE_ZOMBI_DEAD_1,
		AUDIO_NAME.SE_ZOMBI_DEAD_2,
		AUDIO_NAME.SE_ZOMBI_DEAD_3
	};

	// 音声再生タイミングチェック時間
	float voiceCheckTime;

	// 攻撃間隔
	float attackInterval;

	// 初期化種別
	public enum InitType {
		NORMAL	= 0,
		SCALE_Y = 1,
	};

	// 追加SE種別
	public enum SoundType {
		NONE		= 0,
		CHAINSAW	= 1,
	};

	/// <summary>
	/// 出現時データ設定
	/// </summary>
	public void SetPopData( Transform _moveTarget, int _bonusScore, bool _isAttacker )
	{
		moveTarget = _moveTarget;
		bonusScore = _bonusScore;
		isAttacker = _isAttacker;
	}

	/// <summary>
	/// 損傷ダメージ
	/// </summary>
	/// <param name="_damageValue">Damage value.</param>
	public void Damage( int _damageValue)
	{
        //体力が0の場合死亡
		if (hp > 0 && (hp -= _damageValue) <= 0)
		{
			Dead();
		}
		else
		{
			if (agent.enabled)
			{
				//agent.Stop();
				//animator.speed = 0.0f;
				//transform.Rotate(new Vector3(0.0f, Random.Range(-5.0f, 5.0f), 0.0f));

				if (!isDamageAnim)
				{
					animator.SetTrigger("KnockbackTop");
					animator.SetTrigger("KnockbackBottom");

					isDamageAnim = true;

					WaitAfter(0.1f, () =>
					{
						if (hp > 0)
						{
							//agent.Resume();
							//animator.speed = 1.0f;
						}

						isDamageAnim = false;
					});
				}
			}
		}
	}

	public void SetRegionType(Region.TYPE _type)
	{
		regionType = _type;
	}

	public void SetGenPos(Vector3 _pos, float _power)
	{
		damageGenPos = _pos;
		damageGenPower = _power;

		//var damageObj = Resources.Load("FX_Damage") as GameObject;
		//var obj = Instantiate( damageObj );
		//obj.transform.position = damageGenPos;
		//Debug.Break();
	}

	public void BlowingOff(Vector3 _pos, float _power)
	{
		damageGenPos = _pos;
		damageGenPower = _power;

		isBlowing = true;

		Dead();
	}

	/// <summary>
	/// スコア算出
	/// </summary>
	int CalcScore()
	{
		int score = baseScore;

		// ボーナススコア
		score += bonusScore;

		if (regionType == Region.TYPE.HEAD)
		{   // ヘッドショットスコア
			score += SCORE_HEAD_SHOT;
		}

		return score;
	}

	/// <summary>
	/// 撃破時
	/// </summary>
    void Dead()
    {
		VR_AudioManager.Instance.PlaySE(VOICE_DEAD_LIST[Random.Range(0, VOICE_DEAD_LIST.Length)], transform.position, 10.0f, 2.0f);

		animator.enabled = false;
        agent.enabled = false;

        foreach (Rigidbody rigid in transform.GetComponentsInChildren<Rigidbody>())
        {
            rigid.isKinematic = false;

			if( damageGenPower > 0.0f)
			{
				Vector3 forceDir = (transform.position - damageGenPos).normalized;
				forceDir.x = forceDir.x * damageGenPower;
				forceDir.y = (forceDir.y + Random.Range(1.0f, 2.5f)) * damageGenPower;
				forceDir.z = forceDir.z * damageGenPower;

				rigid.AddForce( forceDir, ForceMode.Impulse);
			}
        }

        foreach (BoxCollider col in transform.GetComponentsInChildren<BoxCollider>())
        {
            if (!col.transform.name.Contains("jnt"))
                col.enabled = false;
            else
                col.enabled = true;
        }

        foreach (CapsuleCollider col in transform.GetComponentsInChildren<CapsuleCollider>())
        {
            col.enabled = true;
        }

        foreach (SphereCollider col in transform.GetComponentsInChildren<SphereCollider>())
        {
            col.enabled = true;
        }

        WaitAfter( 3.0f, ()=> { Destroy(gameObject); } );
		//VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_BOMB1, transform.position, 20.0f, 1.0f);

		if (!isBlowing)
		{
			// 撃破スコア表示
			int currentScore = CalcScore();
			var effectScore = Resources.Load("EffectDefeatScore") as GameObject;
			var score = Instantiate(effectScore, new Vector3(transform.position.x, transform.position.y + 2.0f, transform.position.z), Quaternion.identity);
			score.GetComponent<EffectDefeatScore>().Set(currentScore);

			// スコア加算通知
			GameManager.Instance.AddScore(currentScore);
		}
	}

	void CheckWalkVoice()
	{
		if ( Vector3.Distance( transform.position, ControllerManager.Instance.EyeCameraObj.transform.position ) < VOICE_CHECK_DISTANCE)
		{
			if (hp > 0)
			{
				voiceCheckTime -= Time.deltaTime;

				if (voiceCheckTime <= 0.0f)
				{
					VR_AudioManager.Instance.PlaySEAtLimit( 5, VOICE_WALK_LIST[ Random.Range(0, VOICE_WALK_LIST.Length) ], transform.position, 1.5f, 2.0f);
					voiceCheckTime = VOICE_INTERVAL_WALK * Random.Range(0.8f, 1.2f);
				}
			}
		}
	}

	void Start()
	{
        //NavMeshAgentに目標座標を代入
		agent.destination = moveTarget.position;

		thisRenderer.material = arrayChangeMat[Random.Range(0, arrayChangeMat.Length)];

		StartCoroutine(MoveNormalSpeed(agent));

        //Rigidbodyを取得
		Rigidbody[] arrayRigid = transform.GetComponentsInChildren<Rigidbody>();

		foreach (Rigidbody rigid in arrayRigid)
		{
			rigid.isKinematic = true;
		}

		foreach (BoxCollider col in transform.GetComponentsInChildren<BoxCollider>())
		{
			if (col.transform.name.Contains("jnt"))
				col.enabled = false;
		}

		foreach (CapsuleCollider col in transform.GetComponentsInChildren<CapsuleCollider>())
		{
			col.enabled = false;
		}

		foreach (SphereCollider col in transform.GetComponentsInChildren<SphereCollider>())
		{
			col.enabled = false;
		}

		voiceCheckTime = VOICE_INTERVAL_WALK * Random.Range(0.8f, 1.2f);

		if (isDontMove)
		{
			agent.Stop();
			agent.enabled = false;
		}

        //敵の方向はプレイヤーカメラの方を見続けるようにする
		transform.LookAt(ControllerManager.Instance.EyeCameraObj.transform);

		//WaitAfter(3.0f, ()=> { Dead(); });

		switch (initType)
		{
			case InitType.SCALE_Y:
				iTween.ScaleTo(gameObject, iTween.Hash("y", transform.localScale.y, "time", 3.0f));
				transform.SetLocalScaleY(0.0f);
				break;
		}

		switch (soundType) {
			case SoundType.CHAINSAW:
				VR_AudioManager.Instance.PlaySE(AUDIO_NAME.SE_CHAINSAW_LOOP, transform.position, 3.0f, 1.0f, gameObject, true, true);
				break;
		}
	}

    //NavMeshAgentの動作
	IEnumerator MoveNormalSpeed(UnityEngine.AI.NavMeshAgent agent)
	{
		if (agent.enabled)
		{
			agent.autoTraverseOffMeshLink = false; // OffMeshLinkによる移動を禁止

			while (true)
			{
				// OffmeshLinkに乗るまで普通に移動
				yield return new WaitWhile(() => agent.isOnOffMeshLink == false);

				// OffMeshLinkに乗ったので、NavmeshAgentによる移動を止めて、
				// OffMeshLinkの終わりまでNavmeshAgent.speedと同じ速度で移動
				agent.Stop();

				yield return new WaitWhile(() =>
				{
					modelRoot.position = Vector3.MoveTowards(
						modelRoot.position,
											agent.currentOffMeshLinkData.endPos, agent.speed * Time.deltaTime);
					return Vector3.Distance(modelRoot.position, agent.currentOffMeshLinkData.endPos) > 0.1f;
				});

				// NavmeshAgentを到達した事にして、Navmeshを再開
				//agent.CompleteOffMeshLink();
				//agent.Resume();
			}
		}
	}

    //敵撃破時のメソッド
	private void OnDestroy()
	{
		switch (soundType)
		{
			case SoundType.CHAINSAW:
				VR_AudioManager.Instance.StopSE(AUDIO_NAME.SE_CHAINSAW_LOOP);
				break;
		}
	}

	void Update()
	{
		CheckWalkVoice();

		//if (Input.GetKeyDown(KeyCode.D))
		//{
		//	Damage(1);
		//}

		if (Time.frameCount % 60 == 0)
		{
			if (!Mathf.Approximately(agent.destination.x, moveTarget.position.x)
				|| !Mathf.Approximately(agent.destination.y, moveTarget.position.y)
				|| !Mathf.Approximately(agent.destination.z, moveTarget.position.z))
			{
				if (agent.enabled)
				{
					agent.destination = moveTarget.position;
				}
			}
		}

		if (agent.enabled && !agent.pathPending)
		{
			if (agent.remainingDistance <= agent.stoppingDistance)
			{
				if (!agent.hasPath || agent.velocity.sqrMagnitude == 0.0f)
				{
					if (isAttacker)
					{
						if (attackInterval <= 0.0f)
						{
							animator.SetTrigger("Attack");

							// ダメージ表示　一旦消し
							//WaitAfter( 1.5f, ()=>{

							//	HealthDisp.Instance.Sub();
							//});

							attackInterval = ATTACK_INTERVAL;
						}
					}
				}
			}
		}
	}

	void FixedUpdate()
	{
		if (hitColliderRigid != null)
		{
			hitColliderRigid.MovePosition( modelRoot.transform.position );
			hitColliderRigid.MoveRotation( modelRoot.transform.rotation );
		}
	}
}