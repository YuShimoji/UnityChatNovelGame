namespace ProjectFoundPhone.Core
{
    /// <summary>
    /// Yarn ノード名からチャプター情報を抽出するユーティリティ。
    /// 命名規則: "Ch{N}_NodeName" (例: "Ch1_Opening", "Ch2_NewMembers")
    /// </summary>
    public static class YarnNodeHelper
    {
        /// <summary>
        /// ノード名からチャプター番号を返す。
        /// "Ch1_Opening" → 1, "Ch2_NewMembers" → 2, "FirstSlice" → 0
        /// </summary>
        public static int GetChapterNumber(string nodeName)
        {
            if (string.IsNullOrEmpty(nodeName) || nodeName.Length < 4)
                return 0;

            if (!nodeName.StartsWith("Ch"))
                return 0;

            int underscoreIndex = nodeName.IndexOf('_', 2);
            if (underscoreIndex <= 2)
                return 0;

            string numberPart = nodeName.Substring(2, underscoreIndex - 2);
            if (int.TryParse(numberPart, out int chapterNum))
                return chapterNum;

            return 0;
        }

        /// <summary>
        /// ノード名が指定チャプターに属するかを判定する。
        /// </summary>
        public static bool BelongsToChapter(string nodeName, int chapterNumber)
        {
            return GetChapterNumber(nodeName) == chapterNumber;
        }

        /// <summary>
        /// DebugHub 用: ソート順を返す。チャプター番号 or その他(90)。
        /// </summary>
        public static int GetSortOrder(string nodeName)
        {
            int chapter = GetChapterNumber(nodeName);
            return chapter > 0 ? chapter : 90;
        }
    }
}
