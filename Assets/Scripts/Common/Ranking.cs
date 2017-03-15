using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ranking : MonoBehaviour {

	/// <summary>
	/// ランキングデータ
	/// </summary>
	public class Data
	{
		private readonly int NAME_MAX_LEN = 11;
		private readonly int SCORE_COUNT_STOP = 999999;

		/// <summary>
		/// ランキング（重複あり）
		/// </summary>
		public int Rank
		{
			set { rank = value; }
			get { return rank; }
		}

		/// <summary>
		/// プレイヤーの名前
		/// </summary>
		public string PlayerName
		{
			set
			{
				string tmpName = value;
				if (value.Length > 10)
				{
					tmpName = value.Substring(0, NAME_MAX_LEN);
				}
				playerName = tmpName;
			}
			get { return playerName; }
		}

		/// <summary>
		/// スコア
		/// </summary>
		public int Score
		{
			set
			{
				score = Mathf.Clamp(value, 0, SCORE_COUNT_STOP);
			}
			get { return score; }
		}

		private int rank;
		private string playerName;
		private int score;


		/// <summary>
		/// ランキングデータ作成
		/// </summary>
		/// <param name="_rank">ランク</param>
		/// <param name="_playerName">プレイヤーの名前</param>
		/// <param name="_score">スコア</param>
		public Data(int _rank, string _playerName, int _score)
		{
			Rank = _rank;
			PlayerName = _playerName;
			Score = _score;
		}
	}


	/// <summary>
	/// ランキングを取得
	/// </summary>
	/// <param name="_startNum">開始順位</param>
	/// <param name="_contentLengh">要素数</param>
	/// <returns>ランキングデータ</returns>
	public Data[] GetRankingData(int _startNum, int _contentLengh)
	{
		Data[] rankingData = new Data[_contentLengh];

		//初期化
		for (int i = 0; i < _contentLengh; i++)
		{
			rankingData[i] = new Data(999, "", 0);
		}

		//ここにランキングを作る処理

		return rankingData;

	}
}
