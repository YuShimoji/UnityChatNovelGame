using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace ProjectFoundPhone.Data
{
    /// <summary>
    /// チャットメッセージの種類
    /// </summary>
    public enum ChatMessageType
    {
        Normal,
        System,
        Image
    }

    /// <summary>
    /// 1件のチャットメッセージを表すシリアライズ可能なクラス
    /// </summary>
    [Serializable]
    public class ChatMessage
    {
        public ChatMessageType Type;
        public string CharacterID;
        public string Text;
        /// <summary>
        /// 画像メッセージの場合のリソースパス（Resources/ 配下の相対パス）
        /// </summary>
        public string ImageResourcePath;
        /// <summary>
        /// 矛盾指摘システム用の識別タグ（Yarn #line: タグまたはカスタムID）
        /// </summary>
        public string LineTag;
    }

    /// <summary>
    /// ゲームの保存データを表すクラス
    /// Newtonsoft.Json によるJSON形式シリアライズ対応
    /// </summary>
    [Serializable]
    public class SaveData
    {
        #region Save Metadata
        /// <summary>
        /// セーブデータのバージョン（将来の互換性のため）
        /// </summary>
        public int Version = 1;

        /// <summary>
        /// セーブ作成日時（ISO 8601形式）
        /// </summary>
        public string SaveDateTime;

        /// <summary>
        /// セーブスロット番号
        /// </summary>
        public int SlotNumber;
        #endregion

        #region Scenario Progress
        /// <summary>
        /// 現在のYarnノード名
        /// </summary>
        public string CurrentNodeName;

        /// <summary>
        /// Yarn変数のディクショナリ（変数名 -> 値）
        /// </summary>
        public Dictionary<string, object> YarnVariables = new Dictionary<string, object>();
        #endregion

        #region Topic System
        /// <summary>
        /// 獲得済みトピックのIDリスト
        /// </summary>
        public List<string> UnlockedTopicIDs = new List<string>();
        #endregion

        #region Contradiction System
        /// <summary>
        /// 発見済み矛盾ペアのIDリスト
        /// </summary>
        public List<string> DiscoveredContradictionIDs = new List<string>();

        /// <summary>
        /// 蓄積した HalluciCoin
        /// </summary>
        public int HalluciCoin;
        #endregion

        #region Chat History
        /// <summary>
        /// チャット履歴のメッセージリスト
        /// </summary>
        public List<ChatMessage> ChatHistory = new List<ChatMessage>();
        #endregion

        #region Synthesis System
        /// <summary>
        /// 使用済み（合成済み）のレシピIDリスト
        /// </summary>
        public List<string> UsedRecipeIDs = new List<string>();
        #endregion

        #region Constructor
        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public SaveData()
        {
            SaveDateTime = DateTime.Now.ToString("o"); // ISO 8601形式
            SlotNumber = 0;
        }

        /// <summary>
        /// スロット番号を指定するコンストラクタ
        /// </summary>
        /// <param name="slotNumber">セーブスロット番号</param>
        public SaveData(int slotNumber)
        {
            SaveDateTime = DateTime.Now.ToString("o");
            SlotNumber = slotNumber;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// セーブデータが有効かどうかを判定
        /// </summary>
        /// <returns>有効な場合true</returns>
        public bool IsValid()
        {
            return Version > 0 && !string.IsNullOrEmpty(SaveDateTime);
        }

        /// <summary>
        /// セーブデータの概要を取得（UI表示用）
        /// </summary>
        /// <returns>セーブデータの概要文字列</returns>
        public string GetSummary()
        {
            DateTime saveTime;
            if (DateTime.TryParse(SaveDateTime, out saveTime))
            {
                return $"Slot {SlotNumber} - {saveTime:yyyy/MM/dd HH:mm} - Topics: {UnlockedTopicIDs.Count}";
            }
            return $"Slot {SlotNumber} - Invalid Date - Topics: {UnlockedTopicIDs.Count}";
        }
        #endregion
    }
}
