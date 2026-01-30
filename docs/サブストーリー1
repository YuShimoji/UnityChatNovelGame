探索スレッドシステム 仕様書
1. システム概要 (Overview)
作中の「裏掲示板」や「探索依頼アプリ」、あるいは「ドローン操作画面」のような体裁をとるサブコンテンツ群です。

構造: メインストーリー（タイムライン）とは独立した「クエスト」として管理。

目的: 探索を成功させ、メインストーリーの真相解明に必要な「高レアリティのトピック（証拠）」を入手すること。

体験: プレイヤーは現場に行けないため、チャットで送られてくる写真やテキストを頼りに、危険な場所にいる人物に指示を出す。

2. UI仕様：スレッド選択画面 (Mission Board)
プレイヤーが探索を開始するエントリーポイントとなる画面です。

表示要素
リスト形式、または地図上のピンとして表示します。

タイトル: 「廃病院の地下」「深夜の樹海」など

サムネイル: 現場の不気味な写真

メタ情報（ユーザー提示用）:

難易度: ★☆☆☆☆ 〜 ★★★★★

目安時間: 「約 5分」「長編：15分」

リスク: 「即死トラップあり」「SAN値減少注意」など

ステータス:

🔒 Locked: 条件未達（鍵アイコンと解放条件のヒントを表示）

🆕 New: 解放済み・未プレイ

✅ Cleared: 攻略済み（再読可能）

⚠️ Failed: 失敗（リトライ可能）

解放ロジック (Unlock Conditions)
メインストーリーや推論ボードと連動させます。

例1: 推論ボードで「廃病院の噂」というトピックを生成すると、スレッド「廃病院」がUnlockされる。

例2: 特定のキャラの信頼度が上がると、「ねえ、ここ行ってきてあげるよ」とスレッドが出現する。

3. ゲームプレイ仕様：遠隔ナビゲーション
通常の会話とは異なる、TRPG（テーブルトークRPG）的なルールを適用します。

A. 探索者のステータス管理 (HP/Sanity)
スレッド内限定で、探索を行うキャラクターに**「生存値（HP）」や「正気度（Sanity）」、あるいは「スマホのバッテリー残量」**を設定します。

減少条件: 怖い体験をする、怪我をする、無駄な移動をする。

ゲームオーバー: 値が0になると「連絡が途絶えました…」となり、探索失敗（BAD END）。最初からやり直し。

B. 分岐とナビゲーション
Yarn Spinnerを使用しますが、選択肢の重みが異なります。

NPC: 「道が二手に分かれてる。右からは風の音がするけど、左は嫌な予感がする…どっち？」

[右へ進め] → 安全。何もなし。

[左へ進め] → 重要アイテム発見！ だがダメージを受ける（ハイリスク・ハイリターン）。

[待機しろ] → 安全だが時間を消費。

C. リアルタイム演出（短縮版）
探索の臨場感を出すため、短いウェイト演出を挟みます。

プレイヤー: 「そのドアを開けてみて」

システム: Waiting... (3秒〜5秒)

NPC: 「（送信された画像：暗い部屋の写真）」

NPC: 「うわっ！何かいる！」

4. データ構造 (ScriptableObject)
探索スレッドのメタデータを管理するための定義です。

C#

[CreateAssetMenu(menuName = "Game/ExplorationThread")]
public class ExplorationThreadData : ScriptableObject {
    [Header("Basic Info")]
    public string threadID;
    public string title;
    [TextArea] public string description;
    public Sprite thumbnail;

    [Header("Meta Data")]
    public int difficultyLevel; // 1-5 stars
    public string estimatedTime; // "5 mins"

    [Header("Requirements")]
    // このスレッドを遊ぶために必要なトピック
    public TopicData requiredTopicToUnlock;
    
    [Header("Execution")]
    // このスレッドに対応するYarnのノード名
    public string startYarnNode;
    
    [Header("Rewards")]
    // クリア時に得られるトピック
    public TopicData[] rewardTopics;
}
5. Yarn Spinner での実装パターン
通常の会話と区別するため、探索モード専用のコマンドを用意します。

コード スニペット

title: Exploration_Hospital_Start
---
// 探索モード開始（HPバーなどを表示）
<<StartExploration "Char_B" HP:100>>

Char_B: 着いたよ。真っ暗だ。
Char_B: 入口の扉、鍵がかかってるみたい。
<<SendImage "Hospital_Gate">>

-> 無理やりこじ開けろ
    <<Damage 10>> // ダメージ処理
    Char_B: いッ…！ 手を切ったかも。でも開いたよ。
-> 窓を探せ
    Char_B: わかった、裏に回ってみる。
    <<Wait 5>> // 5秒待機
    Char_B: 窓が開いてた。ここから入るね。

// ...（探索続く）...

// クリア判定
<<GiveTopic "Topic_Hospital_Karte">> // カルテを入手
<<EndExploration Success>>
===
6. このシステムのメリットと拡張性
「見守る」没入感: 自分は安全圏にいて、相手だけが危険な目に遭っているという状況は、「助けてあげなきゃ」という強い動機付け（および背徳感）を生みます。

失敗の許容: メインストーリーでBAD ENDになるとやり直しが苦痛ですが、サブスレッドなら「失敗した！もう一回！」とカジュアルにリトライさせやすく、「死にゲー」的な遊びを提供できます。

世界観の深掘り: 本筋に関係ない「地元の怪談」や「廃墟の落書き」などを配置することで、物語の奥行き（Lore）を表現できます。