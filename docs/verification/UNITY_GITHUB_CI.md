# Unity テストを GitHub Actions で回す（EditMode / PlayMode）

リポジトリのワークフロー:

- [`.github/workflows/unity-editmode-tests.yml`](../../.github/workflows/unity-editmode-tests.yml)
- [`.github/workflows/unity-playmode-tests.yml`](../../.github/workflows/unity-playmode-tests.yml)

## 前提

`game-ci/unity-test-runner` を使用。**Unity ライセンス**がシークレットとして設定されている場合のみ、テストジョブが実行される。未設定の場合は **readiness** ジョブが `can_run=false` とし、**skip** ジョブが説明を Summary に出す。

## 推奨シークレット（いずれか一方）

**Personal（推奨メッセージに記載のセット）**

- `UNITY_LICENSE`（.ulf ファイルの内容）
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

または **Professional**

- `UNITY_SERIAL`
- `UNITY_EMAIL`
- `UNITY_PASSWORD`

設定場所: GitHub リポジトリ → **Settings → Secrets and variables → Actions**。

## 確認方法

1. `main` に push するか、Actions から **workflow_dispatch** で手動実行
2. Summary に「Unity EditMode / PlayMode readiness」の `can_run` を確認
3. ジョブが走った場合、Artifacts にテスト結果が添付される

## バージョン

プロジェクトの Editor 版は `ProjectSettings/ProjectVersion.txt` の `m_EditorVersion` とワークフローの `unityVersion` を揃える（現行 **6000.3.6f1**）。

## 並行監査メモ（好機）

- DQT / Ch2 / Ch3 の目視は Editor 作業。結果は `docs/verification/` に日付付きで残すと SUBSEQUENT / EN-012 と整合しやすい。
