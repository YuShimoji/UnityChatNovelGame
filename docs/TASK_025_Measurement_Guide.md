# TASK_025 GC Alloc削減 - 計測手順書

## 目的
TASK_025で実装したGC Alloc削減の効果を計測し、ベースライン（TASK_022）と比較する。

---

## 前提条件
- Unity Editor 2022.3以降がインストールされている
- プロジェクトが最新のmainブランチに同期されている
- `Assets/Scripts/Utils/PerformanceMonitor.cs`が存在する

---

## 計測手順

### Step 1: Unity Editorを起動
1. Unity Hubから本プロジェクトを開く
2. Editorの起動完了を待つ

### Step 2: DebugChatSceneを開く
1. Project ウィンドウで `Assets/Scenes/DebugChatScene.unity` を探す
2. ダブルクリックしてシーンを開く

### Step 3: Play Mode開始
1. Unity Editor上部の **Play ボタン（▶）** をクリック
2. `PerformanceMonitor`が自動で起動される（Console に `[PerformanceMonitor] Auto-initialized in DebugChatScene.` と表示される）

### Step 4: 計測完了を待つ
1. 初期遅延2秒 + 計測10秒 = 合計約12秒待つ
2. Console に `[PerformanceMonitor] Measurement finished.` と表示される
3. レポートが自動保存される（パス: `docs/reports/REPORT_TASK_022_PerformanceBaseline_RAW_YYYYMMDD_HHMMSS.md`）

### Step 5: Play Mode停止
1. Unity Editor上部の **Stop ボタン（■）** をクリック

### Step 6: レポート確認
1. プロジェクトルートの `docs/reports/` フォルダを開く
2. 最新のタイムスタンプ付きレポートファイルを開く
3. GC Alloc (KB/frame) の平均値を確認

---

## 期待される結果

### ベースライン (TASK_022)
- **GC Alloc**: 平均 22 KB/frame、最大 23 KB/frame
- **FPS**: 平均 184.8
- **Memory Used**: 平均 336 MB

### 目標 (TASK_025)
- **GC Alloc**: < 22 KB/frame (ベースラインより低減)
- **FPS**: 維持または向上
- **Memory Used**: 大きな変化なし

---

## トラブルシューティング

### PerformanceMonitorが起動しない
- Console に `[PerformanceMonitor] Skipping auto-init: Scene is not DebugChatScene.` と表示される場合
  → 正しいシーン（DebugChatScene）を開いているか確認

### レポートが生成されない
- Console にエラーメッセージがないか確認
- `docs/reports/` ディレクトリが存在するか確認（存在しない場合は自動作成される）

### GC Allocが0 KB/frameと表示される
- ProfilerRecorderが正しく動作していない可能性
- Unity Editorを再起動して再計測

---

## 次のアクション

計測完了後:
1. レポートの数値を `docs/reports/REPORT_TASK_025_GCAllocReduction.md` の **Measurement (After)** セクションに転記
2. Before/After比較を記載
3. 削減効果を評価（目標達成/未達成）
4. TASK_025をCLOSEDに更新
