# SP-023 / SP-024 表示系デモ計画（修正版・2026-04-20）

**リポジトリ正本**: 本ファイル（リモート同期・再開用）。開発機に `~/.claude/plans/display-batch-showcase.md` を置いている場合は内容を揃える。

---

## 旧案との関係（superseded）

本ファイルは以前の「表示系一括ブロック + SP-024 を統合デモに含める」案を **実装現況と矛盾するため撤回**し、同パスで正本を更新した。

- **撤回した旧方針**: SP-024 S1/S2/S4/S5 を一括実装し、`SetTime` / `MarkDelivered` / `MarkRead` / `DeleteLastMessage` 等を統合 Yarn に組み込む。`UIFontConfig.asset` の `responsiveMinScale` を 0.78 起点で 0.88 へ変更する前提。`CharacterProfile` に IconSide / OnlineStatus を追加しキャラ asset を一時編集して revert。

上記は **checked-in コード・asset が根拠のとき未成立または誤前提** が多い（下記「齟齬」参照）。旧文面の詳細は本リポジトリ履歴またはローカルバックアップを参照。

---

## 監査結果（短）

| 論点 | 結論 |
|------|------|
| Block 2 の `SP023_NarrationMarginDemo.yarn`（6 メッセージ） | Narration×2・通常・BubbleMargin×2・自動リセットまで含み、**局所検収として妥当**。メッセージ数不足ではない（総合デモとは別スコープ）。 |
| 「全体的に小さい」原因 | **コード既定値だけでは説明できない**。checked-in `ChatUIConfig.asset` は `messageFontSize: 26` / `bubbleMaxWidthPx: 600`。加えて Game View の Scale や Canvas 幅、`UIFontConfig` のレスポンシブ縮小が効く UI では **実機・Game View 条件が結果を左右する**。 |
| SP-024 の表示フラグ・Yarn コマンド | `docs/StorySpec/24_chat_immersion.md` は **draft**。`ScenarioManager` には `SetTime` / `MarkDelivered` 等の **登録なし**。Yarn active にも当該コマンド利用例 **なし**。**実装済みとは言えない**。 |
| CharacterProfile の IconSide / defaultOnlineStatus | **現コードに存在しない**（仕様書案のみ）。 |
| SavedChatMessage の timestamp / deliveryStatus / isDeleted | **現コードに存在しない**（仕様書案のみ）。 |
| 統合デモに含めてよい範囲 | **実装済みの SP-023 コマンド**（`BubbleStyle` / `Narration` / `BubbleMargin`）と、追加実装後にのみ成立する項目。**SP-024 は Block 7–9 の実装完了まで統合デモから外す**。 |

**注意**: Yarn が登録されていて再生前提は満たしていても、ユーザーによる **画面検収（見た目の OK/NG）は別**。

---

## 一致している点（現物との整合）

- `Assets/Resources/Yarn/active/SP023_NarrationMarginDemo.yarn` は `SP023_NarrationMargin_Start` で、6 メッセージ構成とコメント上の検収ポイントが Block 2 目的と一致。
- `Assets/Resources/ChatUIConfig.asset` に `messageFontSize: 26` / `bubbleMaxWidthPx: 600` が **実在**。
- `Assets/Resources/UIFontConfig.asset` に `responsiveMinScale: 0.9` が **実在**（`UIFontConfig.cs` の既定 0.78 は **フォールバック時** の話。通常は Resources の asset が使われる）。
- `ScenarioManager.RegisterCustomCommands` に `BubbleStyle` / `Narration` / `BubbleMargin` が **登録済み**。
- `Assets/Resources/BubbleStyles/` の checked-in は現時点 **default / narration のみ**（プリセット 4 種は未追加）。

---

## 齟齬がある点（前回プランとの差）

| 項目 | 齟齬 |
|------|------|
| SP-024 を統合デモに含める | **未実装**。draft 仕様だけではデモに組み込めない。 |
| `UIFontConfig` を 0.78→0.88 に | **checked-in asset は既に 0.9**。0.78 は .cs 既定であり、asset 実値との混同。 |
| 手順で `showTimestamp` 等を ChatUIConfig でオン | **`ChatUIConfig.cs` に当該フィールドが無い**（実装後まで手順に書けない）。 |
| キャラ asset の IconSide / DefaultOnlineStatus を一時編集 | **フィールド未実装**のため、現時点では **存在しない操作**。 |
| 統合デモ 25–30 メッセージ・7 セクション | SP-024 除外と **15–18 メッセージ / 約4セクション** へ縮小する方針が現況に合う。 |

---

## 推奨する修正版方針（正本）

1. **Block 2**: これまでどおり **局所検収を継続**。`SP023_NarrationMargin_Start` で 6 メッセージ確認。総合デモの不足とはみなさない。
2. **統合デモ（SP-023 のみ）**: **実装済み表示系 + 依存なしで追加可能なもの**に限定。
   - **候補A（asset のみ）**: `ChatUIConfig.asset` の可読性調整（`messageFontSize` / `bubbleMaxWidthPx` / `bubbleTextPadding` / `systemMessageFontSize` 等）。**数値は「改善候補」として複数案を試す前提で断定しない**。Game View Scale の見直しは **コード変更なし**で先に実施してよい。
   - **候補B（要実装）**: SP-023 Block 3 **IconSide**（`CharacterProfile` 拡張 + `ConfigureBubble`）。実装コミット後に統合デモへ組み込む。
   - **候補C（要 asset 追加）**: Block 6 **プリセット 4 種**（`thought` / `shout` / `whisper` / `announcement` の BubbleStylePreset）。`BubbleStyleDatabase` の自動収集に合わせ `Assets/Resources/BubbleStyles/` へ追加。
3. **SP-024（タイムスタンプ・既読・オンライン・削除痕）**: **統合デモから除外**。実装順は `docs/runtime-state.md` / `docs/HANDOFF.md` の Block 7–9（S3→S1+S2→S4+S5）に合わせ、**実装・コマンド登録・データ構造が入った後**に別 Yarn で検収。
4. **`UIFontConfig`**: **本当にチャット本文が縮小されている原因だと確認できた場合のみ**触る。現 checked-in は `responsiveMinScale: 0.9`。変更時は Before/After を記録すること。
5. **`git revert`**: **実在するフィールドを一時変更して試す場合のみ**記載する。未実装フィールドの「revert 手順」は書かない。

---

## 実装・検収スコープ（修正版）

### 継続（そのまま）

| 内容 | 正 |
|------|-----|
| Block 1 デモ | `SP023_BubbleStyle_Start`（`SP023_BubbleStyleDemo.yarn`） |
| Block 2 デモ | `SP023_NarrationMargin_Start`（`SP023_NarrationMarginDemo.yarn`） |

### 一括で「表示系」としてまとめる（SP-023 のみ）

次を **同一セッションまたは近接コミット**で扱う想定。**SP-024 は含めない**。

| 順 | 内容 | 依存 |
|----|------|------|
| 1 | （任意）`ChatUIConfig.asset` の可読性候補値の試行 | なし |
| 2 | Block 3: IconSide 実装 + 必要なら検証用キャラ SO 上で ID 付きプロファイル調整 | コード |
| 3 | Block 6: プリセット 4 種の .asset 追加 | asset のみ（DB 自動収集） |

### 統合デモ Yarn（新規・案）

- **パス案**: `Assets/Resources/Yarn/active/SP023_DisplayShowcaseDemo.yarn`
- **エントリ**: `SP023_DisplayShowcase_Start`
- **目安**: **15–18 メッセージ**、**約4セクション**（セクション見出しは `<<SystemMessage "...">>` 等、**現行コマンドのみ**）。

**前提**: 次のコマンド以外を使わないこと: `BubbleStyle` / `Narration` / `BubbleMargin` / `SystemMessage` / `StartWait` / 通常ダイアログ行 / `$speaker`。

**セクション構成（例）**

1. **可読性の目安（任意の ChatUIConfig 調整後）** — 通常 NPC / Player の短文。
2. **Narration + BubbleMargin（Block 2 の縮約）** — Narration 1、`BubbleMargin` 2 パターン、リセット確認（**6 メッセージフルではなく統合向けに圧縮してよい**）。
3. **BubbleStyle プリセット** — `default` / `narration` は既存。Block 6 完了後は `thought` / `shout` / `whisper` / `announcement` を各 1 メッセージずつ。
4. **IconSide** — Block 3 完了後、**実装された** `CharacterProfile` のフィールドに合わせて Bernardo 等で左右差を確認（キャラ ID は **リポジトリ実在の** `Assets/Resources/Characters/*.asset` の `m_CharacterID` に合わせる）。

**明示的に書かないこと**: `<<SetTime>>` / `<<MarkDelivered>>` / `<<MarkRead>>` / `<<DeleteLastMessage>>` / `<<SetOnlineStatus>>`（未登録のため）。

---

## ユーザー向けハンズオン（修正版・最小）

1. Unity で `Assets/Scenes/DebugChatScene.unity` を開く。
2. Hierarchy の `ScenarioManager` で Start Node を **`SP023_NarrationMargin_Start`**（Block 2 単独）または **`SP023_DisplayShowcase_Start`**（統合デモ追加後）に設定。
3. Play。Console に **未定義 Yarn コマンド**が出ないこと。
4. 見づらい場合は **Game View の Scale を上げる**（0.43x のままでは asset を盛っても見え方が変わらないことがある）。
5. `ChatUIConfig.asset` を調整する場合は、**変更前後の数値とスクリーンショット**を残し、単一パラメータずつ変えて原因切り分けする。

---

## Critical Files（修正版）

### 参照（監査・実装の根拠）

- `Assets/Resources/Yarn/active/SP023_NarrationMarginDemo.yarn`
- `Assets/Resources/Yarn/active/SP023_BubbleStyleDemo.yarn`
- `Assets/Resources/ChatUIConfig.asset`
- `Assets/Resources/UIFontConfig.asset`
- `Assets/Scripts/Core/ScenarioManager.cs`（コマンド登録）
- `Assets/Scripts/Data/CharacterProfile.cs`
- `Assets/Scripts/Data/SaveData.cs`
- `docs/StorySpec/23_text_presentation.md`（draft）
- `docs/StorySpec/24_chat_immersion.md`（draft）

### 変更が入る可能性があるもの（作業単位ごと）

| 作業 | ファイル例 |
|------|------------|
| 可読性のみ | `Assets/Resources/ChatUIConfig.asset` |
| IconSide（Block 3） | `CharacterProfile.cs`, `ChatController.cs`（ConfigureBubble）、該当キャラ `.asset` |
| プリセット 4 種（Block 6） | `Assets/Resources/BubbleStyles/*.asset` |
| 統合デモ | `Assets/Resources/Yarn/active/SP023_DisplayShowcaseDemo.yarn`（新規） |

---

## Verification（修正版）

- Block 2: `SP023_NarrationMargin_Start` が **中断なく最後まで**進む（未定義コマンドなし）。
- 統合デモ追加後: `SP023_DisplayShowcase_Start` が **SP-023 コマンドのみ**で完走。
- SP-024 関連は **別ブランチ／別コミット**で、`ScenarioManager` にコマンドが追加された時点で初めて Yarn に登場させる。

---

## ユーザーへ返す短文（コピー用）

SP-024 のタイムスタンプ・既読・オンライン・削除痕は **まだコードに無く**、`display-batch-showcase` の旧案は **仕様書と実装が混線**していたため更新した。Block 2 の 6 メッセージは **局所検収として継続で妥当**。統合デモは **SP-023 実装済み範囲だけ**にし、15–18 メッセージ・4 セクション程度へ縮小。可読性は **`ChatUIConfig.asset` の実値（例: font 26 / maxWidth 600）と Game View Scale** を先に確認し、`UIFontConfig` は checked-in が **すでに minScale 0.9** なので、必要になったときだけ触る。
