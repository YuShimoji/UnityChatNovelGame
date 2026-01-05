1. 推論ボード (Deduction Board) 仕様
プレイヤーが脳内で行う「点と点を繋ぐ作業」を可視化・ゲーム化した画面です。

A. UI/UX コンセプト
チャット画面の上にオーバーレイ、または別画面として呼び出します。

レイアウト: 画面下部に「所持トピック（インベントリ）」、画面中央に広い「作業スペース（ボード）」を配置。

操作:

インベントリからトピックアイコンをボードにドラッグ＆ドロップ。

ボード上の**トピックAをトピックBに重ねる（または線を引く）**ことで「合成」判定を行う。

フィードバック:

成功: 光るエフェクトと共に2つのトピックが統合され、新しい「結論トピック」が生成される（元のトピックは消えずに残る）。

失敗: 「関連性が見出せない」といった短いテキストを表示し、弾かれる。

B. データ構造 (ScriptableObject)
合成レシピを管理するためのデータベース設計です。

C#

// トピック単体の定義
public class TopicData : ScriptableObject {
    public string id;           // 例: "T_Photo_1004"
    public string title;        // 例: "10月4日の写真"
    public Sprite icon;         // サムネイル
    public string description;  // 詳細テキスト
}

// 合成レシピの定義（組み合わせテーブル）
public class SynthesisRecipe : ScriptableObject {
    public TopicData inputA;
    public TopicData inputB;
    public TopicData resultOutput; // 生成される新トピック
    public string successMessage;  // 成功時のカットイン演出用テキスト
}
C. ロジック (Unity C#)
「トピックは消費されない」という要件を満たすため、アイテム管理ではなく**「ロック解除管理」**として実装します。

所持チェック: HashSet<string> unlockedTopicIDs で管理。

合成判定:

プレイヤーが A と B を重ねる。

RecipeList を検索し、A+B (または B+A) の組み合わせが存在するかチェック。

存在し、かつ resultOutput をまだ持っていない場合 → 新トピック獲得処理へ。

既に持っている場合 → 「既にこの結論は出ている」と表示。

2. Yarn Spinner との連携ロジック
Unity側の「推論ボード」での進捗を、Yarn Spinnerの「シナリオ分岐」にどう反映させるかの技術仕様です。

A. 変数同期 (Variable Sync)
Yarn Spinnerは変数を文字列（$variable_name）で管理するため、C#側でトピック獲得時に自動的にYarn変数をセットするブリッジを作ります。

命名規則: トピックIDの頭に topic_ を付けたものをYarn変数とする。

Unity: T_Photo_1004

Yarn: $topic_T_Photo_1004 (bool)

実装イメージ (C#):

C#

public void OnTopicUnlocked(TopicData newTopic) {
    // C#側のリストに追加
    playerData.AddTopic(newTopic);

    // Yarn側の変数をtrueにする
    string yarnVarName = $"$topic_{newTopic.id}";
    yarnRunner.VariableStorage.SetValue(yarnVarName, true);

    // 必要ならYarnのノードを即時実行（「わかったぞ！」などの独り言）
    yarnRunner.StartDialogue("System_TopicUnlocked");
}
B. シナリオ内での判定 (Yarn Script)
ライター（またはプランナー）は、以下のように記述するだけで分岐を作れます。

コード スニペット

// 相手: 「証拠はあるのか？」

-> 特にない
    相手: じゃあ話にならないな。
-> [if $topic_Contradiction_A == true] 矛盾を示す証拠を出す <<if $topic_Contradiction_A>>
    Wait: 15
    相手: …なんだこれは。どこで手に入れた？
    // ここでフラグを活用した深い分岐へ
C. 「トピックの提示」コマンド
チャット中に特定のトピックを相手に突きつける処理です。

カスタムコマンド: <<PresentTopic>>

これを実行すると、Yarnの進行が一時停止し、Unity側の「トピック選択画面」がオーバーレイ表示される。

プレイヤーがトピックを選ぶと、そのIDがYarn変数 $selected_topic_id に格納されて会話が再開する。

コード スニペット

相手: 他に気になることは？
<<PresentTopic>> // プレイヤーに選ばせる
<<jump EvaluateTopic>> // 選んだ結果を判定するノードへ
3. その他、仕様書に含めるべきトピック
プロジェクトを完遂させるために、システム仕様以外で定義しておくべき項目リストです。これらは後回しにすると手戻りの原因になります。

A. 演出・サウンド仕様 (Audio & Visuals)
通知（Notification）:

アプリ内通知（上から降りてくるバナー）の挙動。

ローカルプッシュ通知（アプリを閉じている時の通知）の文言パターン。

サウンド:

重要：「無音」の扱い。ホラー/サスペンスでは「BGMが突然止まる」演出が効果的。その制御方法。

タイピング音、受信音のバリエーション（キャラごとに変えるか？）。

B. チュートリアルと導入 (Onboarding)
「拾った」体験の設計:

ゲーム開始時、タイトル画面を出さずにいきなり「ロック画面」から始めるか？

合成の誘導:

最初の1回だけは、強制的にボードを開かせてAとBを合成させるフローが必要。

C. 異常系・メタ要素 (Meta Elements)
グリッチ演出:

画面が乱れる、ノイズが走るなどのシェーダーエフェクトの仕様（Duskwood的な恐怖演出）。

アプリの中断・再開:

チャットの途中でアプリを落とした場合、再開時にどう復帰するか（直前のメッセージから？ 会話の冒頭から？）。

D. デバッグ機能 (Debug Tools)
開発効率を上げるため、以下の機能が必要です。

時間操作: 待ち時間を強制的に0にするボタン。

トピック全開放: デバッグ用に全トピックを持っている状態にするチート。

Yarnノードジャンプ: 特定の会話シーンから即座に開始する機能。