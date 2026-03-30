# Operator Workflow

人間オペレーターの実ワークフロー・痛点・品質基準を保持する正本。

## 全体フロー (コンテンツ制作パイプライン)

1. シナリオ設計 (手動: ビート表 → セクションビート)
2. Yarn 執筆 (手動: VSCode + Yarn Spinner Extension)
3. 静的バリデーション (自動: YarnContentValidator — Editor メニュー)
4. SO 自動生成 (自動: YarnSOGenerator — Tools > FoundPhone > Yarn SO Generator)
5. Unity 再生確認 (手動: ContentAuthoring シーン)
6. 調整 (手動: Inspector + Yarn 編集)
7. ビルド (未設定)

## 工程ごとの痛点

### S-2 / Yarn 執筆
- wiki (docs/wiki/) は作成済みだが、実際にユーザーが使ってフローを回した実績がない
- 03a_ch1_section_beats.md は DRAFT v2 でユーザーレビュー待ち

### S-5 / Unity 再生確認
- 微修正→手動検証ループが5セッション分の時間を消費した (session 13-17)
- 改善策: 値の調整は Inspector で自律的に行い、セッションに持ち込まない

### S-6 / 調整
- ScriptableObject (.asset) とコード (.cs) のデフォルト値のずれが検出しにくい
- session 14 nightshift で .asset だけ膨張し、.cs の revert では戻らなかった

## 品質目標

- Ch1 (Day1-3) を新規プレイヤーとして通しプレイできる
- 制作フローの摩擦が特定され、改善候補がリスト化されている
- UI の値調整はコード変更なしで完結する

## 手動工程 / 自動化禁止工程

- フォント/色/タイミングの感性調整: Inspector のみ。コード変更禁止
- ストーリーの品質判断: 人間レビュー必須
- モバイルビルドの実機確認: 実デバイス必須

## 運用ルール

- 「どこで困ったか」「何が痛点か」を一度説明されたらここへ固定する
- rejected でも工程が消えるとは限らない。代替運用が必要ならここへ残す
