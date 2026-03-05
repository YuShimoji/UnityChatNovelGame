# Task 057 Report: MessageBubble オブジェクトプーリング導入

## 実装完了日
2026-02-25

## 変更ファイル
- `Assets/Scripts/UI/MessageBubblePool.cs` (既存実装)
- `Assets/Scripts/UI/ChatController.cs` (既存統合済み)

## 実装内容

### 1. MessageBubblePool クラス実装

#### 主要機能
- **初期容量**: デフォルト10個のバブルを事前生成
- **最大容量**: 50個まで拡張可能
- **自動拡張**: プール枯渇時に新規生成（最大容量まで）
- **LRU再利用**: 最大容量到達時は最も古いアクティブオブジェクトを再利用

#### プールAPI
- `Get(Transform parent)`: プールからオブジェクト取得
- `Return(GameObject obj)`: プールにオブジェクト返却
- `ReturnAll()`: 全アクティブオブジェクトを返却
- `Clear()`: プール完全クリア

#### 状態管理
- `ActiveCount`: アクティブなバブル数
- `PooledCount`: プール内の待機バブル数
- `TotalCount`: 総バブル数

### 2. ChatController 統合

#### プール初期化
- `EnsureMessageBubblePool()`: 未設定時に自動でプールコンポーネントを追加
- `SetPrefab()`: Prefab参照をプールに同期

#### メッセージ追加（プール経由）
- `CreateMessageBubble()`: `m_MessageBubblePool.Get()` でバブル取得
- `AddImageMessage()`: 画像バブルもプール対応
- `AddSystemMessage()`: システムメッセージもプール対応

#### メッセージクリア（プール返却）
- `ClearMessages()`: `m_MessageBubblePool.Return()` でバブル返却
- フォールバック: プール未設定時は従来通り `Destroy()` を使用

### 3. オブジェクト状態リセット
`Return()` 時に以下をリセット:
- Transform: 位置・回転・スケール
- TextMeshProUGUI: テキストクリア
- 親オブジェクト: PoolContainer に移動
- Active状態: false に設定

## DoD 検証
- [x] AddMessage/ClearMessages がプール経由で動作
- [x] Destroy 常用を回避し、再利用経路がある
- [x] コンパイルエラー 0（静的検証済み）

## Layer B 検証（未実施）
- Profiler での GC/CPU 改善確認: タスク定義により Layer B は実測待ち
- 推奨検証項目:
  - メッセージ100件追加時の GC.Alloc 削減率
  - ClearMessages() 実行時の CPU スパイク削減

## パフォーマンス期待値
- **GC削減**: Instantiate/Destroy 削減により GC.Alloc 大幅減少
- **CPU削減**: オブジェクト生成・破棄コスト削減
- **メモリ**: 初期容量分のメモリを事前確保（トレードオフ）

## 関連ファイル
- `Assets/Scripts/UI/MessageBubblePool.cs` - プール本体
- `Assets/Scripts/UI/ChatController.cs` - プール統合
- `Assets/Prefabs/UI/MessageBubble.prefab` - プール対象Prefab

## 備考
- プール機能は既存コードと完全互換
- プール未設定時は自動フォールバック
- 最大容量到達時のLRU再利用により、メモリ使用量を制限
