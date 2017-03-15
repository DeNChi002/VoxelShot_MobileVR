using UnityEngine;
using System.Collections;
using System;
using Steamworks;

/// <summary>
/// SteamAPI リーダボード管理クラス
/// </summary>
public class LeaderboardManager : SingletonMonoBehaviour<LeaderboardManager>
{
	// グローバル順位取得開始位置
	public static readonly int GLOBAL_RANKING_START_NUM = 0;
	// グローバル順位取得数
	public static readonly int GLOBAL_RANKING_GET_NUM = 10;

	// ランキング名称
	private static readonly string[] LEADERBOARD_NANE = {
		"ClayShooting Easy Score",
		"ClayShooting Normal Score",
		"ClayShooting Hard Score",

		"Japanese archery Easy Score",
		"Japanese archery Normal Score",
		"Japanese archery Hard Score",

		"TableTennis Easy Score",
		"TableTennis Normal Score",
		"TableTennis Hard Score",

		"Home run competition Easy Score",
		"Home run competition Normal Score",
		"Home run competition Hard Score",

		"Football Easy Score",
		"Football Normal Score",
		"Football Hard Score",

		"Boxing Easy Score",
		"Boxing Normal Score",
		"Boxing Hard Score",

		"Street basketball Easy Score",
		"Street basketball Normal Score",
		"Street basketball Hard Score",

		"Bowling Easy Score",
		"Bowling Normal Score",
		"Bowling Hard Score",


		"------ Blank1 SlotA",
		"------ Blank1 SlotB",
		"------ Blank1 SlotC",

		"------ Blank2 SlotA",
		"------ Blank2 SlotB",
		"------ Blank2 SlotC",
	};
	// Steamリーダーボードからのスコア初期化が完了している状態か
	public bool IsEntrieInit { get; set; }

	// 取得するランキングタイプ
	public enum GetRankingType
	{
		USER_CURRENT = 0,
		GLOBAL = 1,
	};


	/// <summary>
	/// ランキングデータ取得
	/// </summary>
	/// <param name="_gameNo">Game no.</param>
	/// <param name="_diff">Diff.</param>
	/// <param name="_type">Type.</param>
	/// <param name="_success">Success.</param>
	/// <param name="_failure">Failure.</param>
	public void GetRankingData( int _gameNo, int _diff, GetRankingType _type, Action<Ranking.Data[]> _success, Action _failure )
	{
		if (!SteamManager.Initialized)
		{   // SteamAPIの初期化が行われていない
			if (_failure != null)
				_failure();
			return;
		}

		StartCoroutine( _DownloadEntry(_gameNo, _diff, _type, _success, _failure) );
	}

	IEnumerator _DownloadEntry( int _gameNo, int _diff, GetRankingType _type, Action<Ranking.Data[]> _success, Action _failure )
	{
		bool retFind = false;

		string leaderboard_name = LEADERBOARD_NANE[_gameNo];

		// リーダーボード取得
		SteamManager.Instance.Leaderboard.FindLeaderboard(leaderboard_name, (_find) =>
		{
			switch (_type)
			{	// ユーザー自身順位
				case GetRankingType.USER_CURRENT:
					{
						CSteamID[] ids = { SteamUser.GetSteamID() };
						SteamManager.Instance.Leaderboard.DownloadScoreUsers(ids, (_data) => { retFind = true; });

					}
					break;
				// グローバル順位
				case GetRankingType.GLOBAL:
					SteamManager.Instance.Leaderboard.DownloadScoreGlobal(GLOBAL_RANKING_START_NUM, GLOBAL_RANKING_GET_NUM, (_data) => { retFind = true; });
					break;
			}
		});

		while (!retFind)
		{	// 取得待ち
			yield return new WaitForEndOfFrame();
		};

		Ranking.Data[] arrayRankingData = new Ranking.Data[SteamManager.Instance.Leaderboard.CurrentDownloadEntryCnt];
		for (int i = 0; i < arrayRankingData.Length; ++i)
		{
			LeaderboardEntry_t leaderboardEntry;
			bool ret = SteamUserStats.GetDownloadedLeaderboardEntry(SteamManager.Instance.Leaderboard.SteamLeaderboardEntries, i, out leaderboardEntry, null, 0);

			if (ret)
			{
				arrayRankingData[i] = new Ranking.Data( leaderboardEntry.m_nGlobalRank, SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser), leaderboardEntry.m_nScore );
			}
		}

		if (_success != null)
		{
			_success( arrayRankingData );
		}
	}

	/// <summary>
	/// スコア送信
	/// </summary>
	/// <param name="_gameNo">ゲーム番号 SaveDataManager - AthleticEvent.</param>
	/// <param name="_diffNo">難易度要素数 SaveDataManager - Difficulty.</param>
	/// <param name="_score">スコア</param>
	public void UploadScore( int _gameNo, int _diff, int _score )
	{
		if (!SteamManager.Initialized)
		{   // SteamAPIの初期化が行われていない
			return;
		}

		if (!IsEntrieInit)
		{	// スコア初期化が完了していない場合は処理しない
			return;
		}

		string leaderboard_name = LEADERBOARD_NANE[_gameNo];

		//// 一応キー存在チェック
		//if (!SaveManager.SaveData.Instance.SteamEntrieScore.ContainsKey(leaderboard_name))
		//{
		//	return;
		//}
		//// ローカル値がSteamリーダーボードのスコア以下なら処理しない
		//if (_score < SaveManager.SaveData.Instance.SteamEntrieScore[leaderboard_name])
		//{
		//	return;
		//}
		//// リーダーボード取得
		//SteamManager.Instance.Leaderboard.FindLeaderboard(leaderboard_name, (_find) => 
		//{
		//	// スコア送信
		//	SteamManager.Instance.Leaderboard.UploadScore(_score, (_upload) =>
		//	{
		//		if (_upload.m_bSuccess == 1)
		//		{   // 登録成功
		//			SaveManager.SaveData.Instance.SteamEntrieScore[name] = _score;
		//			SaveManager.Instance.Save();
		//		}
		//	});
		//});
	}

    /// <summary>
    /// リーダーボードからスコアの初期化
    /// </summary>
    /// <returns>The entrie.</returns>
    IEnumerator _CheckEntrie()
	{
		if (!SteamManager.Initialized)
		{	// SteamAPIの初期化が行われていない
			yield return null;
		}

		bool retFind = false;
		CSteamID[] Ids = { SteamUser.GetSteamID() };

        // 通常エリア分
		for (int i = 0; i < LEADERBOARD_NANE.Length; ++i)
		{
			string leaderboard_name = LEADERBOARD_NANE[i];

			// リーダーボード取得
			SteamManager.Instance.Leaderboard.FindLeaderboard(leaderboard_name, (_find) => { retFind = true; });

			retFind = false;
			while ( !retFind )
			{
				yield return new WaitForEndOfFrame();
			};

			// ユーザー自身スコア取得
			if (!SteamManager.Instance.Leaderboard.DownloadScoreUsers(Ids, (_download) => { retFind = true; }))
			{
				yield break;
			}

			retFind = false;
			while (!retFind)
			{
				yield return new WaitForEndOfFrame();
			};

			//if (SteamManager.Instance.Leaderboard.CurrentDownloadEntryCnt > 0)
			//{
			//	LeaderboardEntry_t leaderboardEntry;
			//	bool ret = SteamUserStats.GetDownloadedLeaderboardEntry(SteamManager.Instance.Leaderboard.SteamLeaderboardEntries, 0, out leaderboardEntry, null, 0);

			//	if (ret)
			//	{
			//		SaveManager.SaveData.Instance.SteamEntrieScore[leaderboard_name] = leaderboardEntry.m_nScore;
			//	}
			//}
			//else
			//{   // スコア未登録
			//	if (!SaveManager.SaveData.Instance.SteamEntrieScore.ContainsKey(leaderboard_name))
			//	{
			//		SaveManager.SaveData.Instance.SteamEntrieScore.Add(leaderboard_name, 0);
			//	}
			//}
			//SaveManager.Instance.Save();
		}

        IsEntrieInit = true;
	}

	override protected void Awake ()
    {
		//base.Awake();
		
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

	void Start()
	{
		IsEntrieInit = false;

		// Steam登録データの初期化が済んでいるか判定
		//for (int i = 0; i < LEADERBOARD_NANE.Length; ++i)
		//{
		//	if (!SaveManager.SaveData.Instance.SteamEntrieScore.ContainsKey(LEADERBOARD_NANE[i]))
		//	{
		//		StartCoroutine(_CheckEntrie());
		//		return;
		//	}
		//}

		//SaveManager.Instance.Save();
		IsEntrieInit = true;
	}
}
