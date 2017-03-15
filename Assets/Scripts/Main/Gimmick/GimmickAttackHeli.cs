using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ギミック:軍事基地攻撃ヘリ
/// </summary>
public class GimmickAttackHeli : GimmickBase
{
	[SerializeField, Header("メインローター")]
	GameObject rotorMain;

	[SerializeField, Header("テールローター")]
	GameObject rotorTail;

	[SerializeField, Header("ミサイル")]
	GimmickHeliMissile[] arrayMissile;

	[SerializeField] // 登場時起動パス
	public string AppearMovePathName = "Target Path Name";
	[SerializeField] // 
	public float AppearMovePathTime = 10.0f;

	[SerializeField] // 対空移動パス
	public string LoopMovePathName = "Target Path Name";
	[SerializeField] // 
	public float LoopMovePathTime = 30.0f;

	// 回転最大速度
	static readonly float ROT_SPEED_MAX = 750.0f;

	// 旋回速度
	static readonly float ROTATE_SPEED = 3.0f;

	enum State
	{
		STANDBY = 0,
		APPEAR_MOVE = 1,
		LOOP_MOVE = 2,
		BREAK = 3,
	};
	State currentState = State.STANDBY;

	float rotSpeed;
	Vector3 moveStartPos;
	float moveStartTime;
	float circleMoveAngle;

	/// <summary>
	/// 損壊時の処理
	/// </summary>
	protected override void ToBreak()
	{
		base.ToBreak();
	}

	protected override void GimmickStart()
	{
		base.GimmickStart();

		currentState = State.APPEAR_MOVE;
		rotSpeed = 0.0f;

		iTween.MoveTo(gameObject, iTween.Hash(
			"path", iTweenPath.GetPath(AppearMovePathName),
			"time", AppearMovePathTime,
			"lookTime", 1.0f,
			"easeType", iTween.EaseType.easeOutExpo,
			"oncomplete", "OnTakeOff",
			"oncompletetarget", gameObject));

		WaitAfter( 10.0f, ()=> {

			//iTween.Pause(gameObject);

			for (int i = 0; i < arrayMissile.Length; ++i)
			{
				arrayMissile[i].Shot( transform, i * 0.1f );
			}
		});
	}

	void OnTakeOff()
	{
		currentState = State.LOOP_MOVE;
		moveStartPos = transform.localPosition;
		moveStartTime = Time.time;

		iTween.MoveTo(gameObject, iTween.Hash(
			"path", iTweenPath.GetPath(LoopMovePathName),
			"time", LoopMovePathTime,
			"lookTime", 1.0f,
			"easeType", iTween.EaseType.linear,
			"oncomplete", "OnTakeOff",
			"oncompletetarget", gameObject));
	}

	void Update()
	{
		switch (currentState)
		{
			case State.STANDBY:
				break;
			case State.APPEAR_MOVE:
				//rotSpeed = Mathf.Clamp(rotSpeed += Time.deltaTime * 4.0f, 0.0f, ROT_SPEED_MAX);
				
				//if(rotSpeed >= ROT_SPEED_MAX)
				//{
				//	currentState = State.TAKE_OFF;

				//	iTween.MoveTo(gameObject, iTween.Hash("y", 335.0f, "isLocal", true, "easeType", iTween.EaseType.easeInOutQuart, "time", 2.5f, "onComplete", "OnTakeOff"));
				//}
				break;
			case State.LOOP_MOVE:
				//transform.Rotate(new Vector3(0.05f + Random.Range(0.0f, 0.02f), 0.0f, 0.0f));
				break;
				//transform.localPosition = moveStartPos + new Vector3(0.0f, Mathf.Sin(Time.time - moveStartTime) / 2.5f, 0.0f);
				
				//Vector3 playerPos = ControllerManager.Instance.EyeCameraObj.transform.position;
				//transform.position = new Vector3(playerPos.x + 10.0f * Mathf.Cos(circleMoveAngle), transform.position.y, playerPos.z + 10.0f * Mathf.Sin(circleMoveAngle) );

				//circleMoveAngle += Time.deltaTime;

				//var rot = Quaternion.LookRotation(transform.position - ControllerManager.Instance.EyeCameraObj.transform.position);
				//transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * ROTATE_SPEED);

			case State.BREAK:
				break;
		}
		
		var rot = Quaternion.LookRotation(transform.position - ControllerManager.Instance.EyeCameraObj.transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * ROTATE_SPEED);

		rotorMain.transform.Rotate(new Vector3(0.0f, -ROT_SPEED_MAX * Time.deltaTime, 0.0f));
		rotorTail.transform.Rotate(new Vector3(-ROT_SPEED_MAX * Time.deltaTime * 2.0f, 0.0f, 0.0f));
	}
}