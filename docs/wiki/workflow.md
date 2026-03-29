# ファイル配置とワークフロー

## ディレクトリ構造

```
Assets/Resources/
  Yarn/
    Project.yarnproject     # sourceFiles: ["active/**/*.yarn"]
    active/                 # コンパイル対象
      Ch1_Day1.yarn         # Chapter 1 Day 1
      Ch2_LocationConfusion.yarn
      Ch3_InstitutionalFragments.yarn
      EngineTestKit.yarn    # エンジン機能テスト
      SubthreadTest.yarn    # スレッドテスト
    archive/                # コンパイル対象外 (旧モック保管)
  Characters/               # CharacterProfile SO
  Topics/                   # TopicData SO
  Channels/                 # ChannelData SO
  ChatUIConfig.asset        # UI設定 (フォントサイズ、色、パディング等)
```

## コンテンツ制作パイプライン

```
シナリオ設計 → Yarn執筆 → Validator → SO自動生成 → Unity再生確認 → 調整
  [手動]        [手動]      [自動]       [自動]         [手動]        [手動]
```

### Step by Step

1. **シナリオ設計**: ビート表を書く (docs/StorySpec/03_chapter_beats.md 参照)
2. **Yarn 執筆**: VS Code + Yarn Spinner Extension で `.yarn` ファイルを作成
3. **静的バリデーション**: `Tools > FoundPhone > Yarn Content Validator` を実行
4. **SO 自動生成**: `Tools > FoundPhone > Yarn SO Generator` で不足SOを検出・生成
5. **再生確認**: ContentAuthoring シーンで Play
6. **調整**: テンポ・演出・選択肢を調整

## VS Code での編集

### セットアップ

1. VS Code に [Yarn Spinner Extension](https://marketplace.visualstudio.com/items?itemName=SecretLab.yarn-spinner) をインストール
2. プロジェクトルートを開く
3. `Assets/Resources/Yarn/active/` の `.yarn` ファイルを編集

### ノードグラフ表示

`Ctrl+Shift+P` > `Yarn Spinner: Show Graph` でノード接続を可視化。

### 構文ハイライト

Yarn Spinner Extension が自動的にハイライトを適用。

## 新しい Yarn ファイルの追加

1. `Assets/Resources/Yarn/active/` に `.yarn` ファイルを新規作成
2. Unity に戻ると自動的にコンパイルされる
3. エラーがあれば Console に表示される

### 不要になった Yarn ファイルの退避

`active/` から `archive/` に移動するだけでコンパイル対象外になります。

## ScriptableObject の管理

### 手動作成

- **CharacterProfile**: `Create > Project FoundPhone > Character Profile`
- **TopicData**: `Create > Project FoundPhone > Topic Data`
- **ChannelData**: `Create > Project FoundPhone > Channel Data`

### 自動検出

`Tools > FoundPhone > Yarn SO Generator` で Yarn ファイルを走査し、不足している SO を自動検出・生成。
