using System.Text;
using System.Text.RegularExpressions;

namespace ProjectFoundPhone.UI
{
    /// <summary>
    /// チャットテキストの解析・変換ユーティリティ。
    /// ChatController から抽出された純粋な文字列変換ロジック。
    /// </summary>
    public static class ChatTextParser
    {
        /// <summary>
        /// テキスト内のスレッドマークアップを TMP リッチテキストに変換する。
        /// [link:threadId:label] → クリック可能リンク
        /// [artifact:type:description] → 成果物カード表示
        /// </summary>
        public static string ParseThreadMarkup(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            // [link:threadId:label] → <link="thread:threadId"><color=#4A90D9><u>label</u></color></link>
            text = Regex.Replace(
                text,
                @"\[link:([^:]+):([^\]]+)\]",
                "<link=\"thread:$1\"><color=#4A90D9><u>$2</u></color></link>");

            // [artifact:type:description] → アイコン + 説明
            text = Regex.Replace(
                text,
                @"\[artifact:recording:([^\]]+)\]",
                "<color=#FF9800>\u25B6</color> <i>$1</i>");
            text = Regex.Replace(
                text,
                @"\[artifact:photo:([^\]]+)\]",
                "<color=#4CAF50>\u25A3</color> <i>$1</i>");
            text = Regex.Replace(
                text,
                @"\[artifact:sample:([^\]]+)\]",
                "<color=#9C27B0>\u25C6</color> <i>$1</i>");

            return text;
        }

        /// <summary>
        /// CreateMessageBubble が付加する名前リッチテキストプレフィックスを除去する。
        /// 古いセーブデータとの後方互換用。名前プレフィックスがなければテキストをそのまま返す。
        /// パターン: &lt;line-height=80%&gt;&lt;size=XX&gt;&lt;b&gt;名前&lt;/b&gt;&lt;/size&gt;\n&lt;/line-height&gt;本文
        /// </summary>
        private static readonly Regex s_NamePrefixPattern = new Regex(
            @"^<line-height=\d+%><size=[\d.]+><b>.+?</b></size>\n</line-height>",
            RegexOptions.Compiled | RegexOptions.Singleline);

        public static string StripNamePrefix(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return s_NamePrefixPattern.Replace(text, "", 1);
        }

        /// <summary>
        /// TMP リッチテキストの未閉タグ (b/i/u/s) を検出し、末尾にクローズタグを補完する。
        /// name + body を同一 TMP コンポーネントで結合する際のスコープ漏れを防止する。
        /// </summary>
        public static string CloseUnclosedRichTextTags(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string[] tags = { "b", "i", "u", "s" };
            var suffix = new StringBuilder();

            foreach (var tag in tags)
            {
                int openCount = 0;
                int closeCount = 0;
                string openTag = $"<{tag}>";
                string closeTag = $"</{tag}>";

                int idx = 0;
                while ((idx = text.IndexOf(openTag, idx, System.StringComparison.OrdinalIgnoreCase)) >= 0)
                {
                    openCount++;
                    idx += openTag.Length;
                }
                idx = 0;
                while ((idx = text.IndexOf(closeTag, idx, System.StringComparison.OrdinalIgnoreCase)) >= 0)
                {
                    closeCount++;
                    idx += closeTag.Length;
                }

                int unclosed = openCount - closeCount;
                for (int j = 0; j < unclosed; j++)
                {
                    suffix.Append(closeTag);
                }
            }

            return suffix.Length > 0 ? text + suffix.ToString() : text;
        }
    }
}
