using UnityEngine;
using System.Collections;
using Steamworks;
using UnityEditor;

/// <summary>
/// スチームAPI管理エディタ
/// </summary>
[CustomEditor(typeof(SteamManager))]
public class SteamManagerEditor : Editor {

	public string LeaderboardName { get; set; }
	public int MockScoreUploadNum { get; set; }
	public int DownloadStartNum { get; set; }
	public int DownloadEndNum { get; set; }

	public override void OnInspectorGUI()
	{
		SteamManager code = target as SteamManager;

		//base.OnInspectorGUI();

		EditorGUILayout.BeginVertical(GUI.skin.box);
		{
			if (!Application.isPlaying)
			{   // 未実行時
				EditorGUILayout.LabelField("[API初期化]");
				EditorGUILayout.BeginVertical(GUI.skin.box);
				{
					EditorGUILayout.LabelField("未実行");
				}
				EditorGUILayout.EndVertical();
			}
			else
			{
				if (!SteamManager.Initialized)
				{
					EditorGUILayout.LabelField("[API初期化]");
					EditorGUILayout.BeginVertical(GUI.skin.box);
					{
						EditorGUILayout.LabelField("処理中、もしくは失敗");
					}
					EditorGUILayout.EndVertical();
				}
				else
				{
					EditorGUILayout.LabelField("[API初期化]");
					EditorGUILayout.BeginVertical(GUI.skin.box);
					{
						EditorGUILayout.LabelField("初期化済み: PersonaName:" + SteamFriends.GetPersonaName());
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();


					EditorGUILayout.LabelField("[現在のリーダーボード]");
					EditorGUILayout.BeginVertical(GUI.skin.box);
					{
						EditorGUILayout.LabelField(SteamManager.Instance.Leaderboard.IsFind ? SteamManager.Instance.Leaderboard.CurrentLeaderboardName : "未取得");

						LeaderboardName = EditorGUILayout.TextField( "リーダーボード名", LeaderboardName );
						if (GUILayout.Button("取得"))
						{
							SteamManager.Instance.Leaderboard.FindLeaderboard(LeaderboardName);
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();


					EditorGUILayout.LabelField("[スコア送信テスト]");
					EditorGUILayout.BeginVertical(GUI.skin.box);
					{
						MockScoreUploadNum = EditorGUILayout.IntField(MockScoreUploadNum);
						if (GUILayout.Button("送信"))
						{
							SteamManager.Instance.Leaderboard.UploadScore(MockScoreUploadNum);
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();

					EditorGUILayout.LabelField("[エントリダウンロード]");
					EditorGUILayout.BeginVertical(GUI.skin.box);
					{
						DownloadStartNum = EditorGUILayout.IntField("開始", DownloadStartNum, GUILayout.ExpandWidth(false));
						DownloadEndNum = EditorGUILayout.IntField("終了", DownloadEndNum, GUILayout.ExpandWidth(false));

						if (GUILayout.Button("エントリ取得:グローバル"))
						{
							SteamManager.Instance.Leaderboard.DownloadScoreGlobal( DownloadStartNum, DownloadEndNum );

							//LeaderboardManager.Instance.GetRankingData(0, LeaderboardManager.GetRankingType.GLOBAL, null, null);
						}
						if (GUILayout.Button("エントリ取得:ユーザ周囲"))
						{
							SteamManager.Instance.Leaderboard.DownloadScoreGlobalAroundUser( DownloadStartNum, DownloadEndNum );
						}
						if (GUILayout.Button("エントリ取得:ユーザ自身"))
						{
							CSteamID[] ids = { SteamUser.GetSteamID() };
							SteamManager.Instance.Leaderboard.DownloadScoreUsers( ids );

							//LeaderboardManager.Instance.GetRankingData(0, LeaderboardManager.GetRankingType.USER_CURRENT, null, null);
						}
						EditorGUILayout.Space();
						EditorGUILayout.LabelField("エントリ取得数:" + SteamManager.Instance.Leaderboard.CurrentDownloadEntryCnt);
						for (int i = 0; i < SteamManager.Instance.Leaderboard.CurrentDownloadEntryCnt; ++i)
						{
							LeaderboardEntry_t leaderboardEntry;
							bool ret = SteamUserStats.GetDownloadedLeaderboardEntry(SteamManager.Instance.Leaderboard.SteamLeaderboardEntries, i, out leaderboardEntry, null, 0);

							if (ret)
							{
								EditorGUILayout.LabelField("PersonaName:" + SteamFriends.GetFriendPersonaName(leaderboardEntry.m_steamIDUser) + " Score:" + leaderboardEntry.m_nScore + " GlobalRank:" + leaderboardEntry.m_nGlobalRank );
							}
						}
					}
					EditorGUILayout.EndVertical();
					EditorGUILayout.Space();

					EditorUtility.SetDirty(target);
				}
			}
		}
		EditorGUILayout.EndVertical();
	}
}