using UnityEngine;
using System.Collections;
using Steamworks;

/// <summary>
/// スチームAPIリーダーボード機能
/// </summary>
public class SteamLeaderboard {

	public bool IsFind { get { return steamLeaderboard != null; } }
	public int CurrentDownloadEntryCnt { get; set; }
	public string CurrentLeaderboardName { get { return SteamUserStats.GetLeaderboardName((SteamLeaderboard_t)steamLeaderboard); } }
	// 取得エントリ
	public SteamLeaderboardEntries_t SteamLeaderboardEntries { get; set; }

	public delegate void CallBackFind(LeaderboardFindResult_t _find);
	public delegate void CallBackUpload(LeaderboardScoreUploaded_t _upload);
	public delegate void CallBackDownload(LeaderboardScoresDownloaded_t _download);

	// リーダーボード情報
	SteamLeaderboard_t? steamLeaderboard = null;

	// コールバック
	CallResult<LeaderboardFindResult_t> leaderboardFindResult;
	CallResult<LeaderboardScoreUploaded_t> leaderboardScoreUploaded;
	CallResult<LeaderboardScoresDownloaded_t> leaderboardScoresDownloaded;

	CallBackFind callBackFind = null;
	CallBackUpload callBackUpload = null;
	CallBackDownload callBackDownload = null;

	/// <summary>
	/// リーダーボード取得
	/// </summary>
	/// <param name="_leaderBoardName">設定を行うランキングの識別子</param>
	public void FindLeaderboard( string _leaderBoardName, CallBackFind _callback = null )
	{
		steamLeaderboard = null;
		callBackFind = _callback;

		SteamAPICall_t handle = SteamUserStats.FindOrCreateLeaderboard(_leaderBoardName, ELeaderboardSortMethod.k_ELeaderboardSortMethodDescending, ELeaderboardDisplayType.k_ELeaderboardDisplayTypeNumeric);
		leaderboardFindResult.Set(handle);
	}

	/// <summary>
	/// ランキングへ値送信
	/// </summary>
	/// <param name="_score">現在のランキングへ保存する int32値</param>
	public bool UploadScore(int _score, CallBackUpload _callback = null)
	{
		if (steamLeaderboard == null)
		{
			return false;
		}
		callBackUpload = _callback;

		SteamAPICall_t handle = SteamUserStats.UploadLeaderboardScore((SteamLeaderboard_t)steamLeaderboard, ELeaderboardUploadScoreMethod.k_ELeaderboardUploadScoreMethodForceUpdate, _score, null, 0);
		leaderboardScoreUploaded.Set(handle);

		return true;
	}

	/// <summary>
	/// エントリ取得:指定スチームユーザID配列
	/// </summary>
	/// <returns><c>true</c>, if score users was downloaded, <c>false</c> otherwise.</returns>
	/// <param name="_arrayIDs">ユーザID配列</param>
	public bool DownloadScoreUsers( CSteamID[] _arrayIDs, CallBackDownload _callback = null )
	{
		if (steamLeaderboard == null)
		{
			return false;
		}
		callBackDownload = _callback;

		CurrentDownloadEntryCnt = 0;

		//CSteamID[] a = { SteamUser.GetSteamID() };
		SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntriesForUsers((SteamLeaderboard_t)steamLeaderboard, _arrayIDs, _arrayIDs.Length);
		leaderboardScoresDownloaded.Set(handle);

		return true;
	}

	/// <summary>
	/// エントリ取得:グローバルランキングの指定位置
	/// </summary>
	/// <returns><c>true</c>, if score global was downloaded, <c>false</c> otherwise.</returns>
	/// <param name="_rangeStart">取得開始位置</param>
	/// <param name="_rangeEnd">取得終了位置</param>
	public bool DownloadScoreGlobal( int _rangeStart, int _rangeEnd, CallBackDownload _callback = null )
	{
		if (steamLeaderboard == null)
		{
			return false;
		}
		callBackDownload = _callback;

		CurrentDownloadEntryCnt = 0;

		SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries((SteamLeaderboard_t)steamLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobal, _rangeStart, _rangeEnd);
		leaderboardScoresDownloaded.Set(handle);

		return true;
	}

	/// <summary>
	/// エントリ取得:ユーザーのグルーバルランキング位置からの指定相対数
	/// -4, 5でユーザー自身のランキングと、その位置から前4・後5の合計10個分エントリを取得する
	/// 前指定が条件を満たさない場合、その超過分は後方取得に置き換えられる
	/// </summary>
	/// <returns><c>true</c>, if scores was downloaded, <c>false</c> otherwise.</returns>
	/// <param name="_rangeStart">取得開始位置</param>
	/// <param name="_rangeEnd">取得終了位置</param>
	public bool DownloadScoreGlobalAroundUser( int _rangeStart, int _rangeEnd, CallBackDownload _callback = null )
	{
		if (steamLeaderboard == null)
		{
			return false;
		}
		callBackDownload = _callback;

		CurrentDownloadEntryCnt = 0;

		SteamAPICall_t handle = SteamUserStats.DownloadLeaderboardEntries((SteamLeaderboard_t)steamLeaderboard, ELeaderboardDataRequest.k_ELeaderboardDataRequestGlobalAroundUser, _rangeStart, _rangeEnd);
		leaderboardScoresDownloaded.Set(handle);

		return true;
	}

	/// <summary>
	/// 初期化
	/// </summary>
	public void Init()
	{
		leaderboardFindResult = CallResult<LeaderboardFindResult_t>.Create(OnLeaderboardFindResult);
		leaderboardScoreUploaded = CallResult<LeaderboardScoreUploaded_t>.Create(OnLeaderboardScoreUploaded);
		leaderboardScoresDownloaded = CallResult<LeaderboardScoresDownloaded_t>.Create(OnLeaderboardScoresDownloaded);
	}

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public SteamLeaderboard()
	{
		Init();
	}

	#region SteamAPIコールバック

	/// <summary>
	/// Ons the leaderboard find result.
	/// </summary>
	/// <param name="pCallback">P callback.</param>
	/// <param name="bIOFailure">If set to <c>true</c> b IOFailure.</param>
	private void OnLeaderboardFindResult(LeaderboardFindResult_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_bLeaderboardFound != 0)
		{
			steamLeaderboard = pCallback.m_hSteamLeaderboard;
		}

		if (callBackFind != null)
		{
			callBackFind(pCallback);
		}
	}

	/// <summary>
	/// Ons the leaderboard score uploaded.
	/// </summary>
	/// <param name="pCallback">P callback.</param>
	/// <param name="bIOFailure">If set to <c>true</c> b IOF ailure.</param>
	private void OnLeaderboardScoreUploaded(LeaderboardScoreUploaded_t pCallback, bool bIOFailure)
	{
		//m_nGlobalRankPrevious 0: 新規
		Debug.Log("[" + LeaderboardScoreUploaded_t.k_iCallback + " - LeaderboardScoreUploaded] - " + pCallback.m_bSuccess + " -- " + pCallback.m_hSteamLeaderboard + " -- " + pCallback.m_nScore + " -- " + pCallback.m_bScoreChanged + " -- " + pCallback.m_nGlobalRankNew + " -- " + pCallback.m_nGlobalRankPrevious);

		if (callBackUpload != null)
		{
			callBackUpload( pCallback );
		}
	}

	/// <summary>
	/// Ons the leaderboard scores downloaded.
	/// </summary>
	/// <param name="pCallback">P callback.</param>
	/// <param name="bIOFailure">If set to <c>true</c> b IOF ailure.</param>
	private void OnLeaderboardScoresDownloaded(LeaderboardScoresDownloaded_t pCallback, bool bIOFailure)
	{
		Debug.Log("[" + LeaderboardScoresDownloaded_t.k_iCallback + " - LeaderboardScoresDownloaded] - " + pCallback.m_hSteamLeaderboard + " -- " + pCallback.m_hSteamLeaderboardEntries + " -- " + pCallback.m_cEntryCount);
		SteamLeaderboardEntries = pCallback.m_hSteamLeaderboardEntries;
		CurrentDownloadEntryCnt = pCallback.m_cEntryCount;

		if (callBackDownload != null)
		{
			callBackDownload( pCallback );
		}
	}

	#endregion
}