// ---------------------------------------------------------  
// GameController.cs  
// キー操作やプレイヤーの移動について
// 作成日:  20231024
// 作成者:  竹内
// ---------------------------------------------------------  
// 更新内容
// 20231130
// ・Vector3Intを取り入れる予定だったが(int)を知り、軽さの改善につながることから実装。

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {


    #region 変数（変更箇所あり）  
    // フィールド操作クラスの定義
    static FieldArrayData _fieldArrayData;
    /// <summary>
    /// ゲームの状態管理用構造体
    /// START : ゲーム開始前処理
    /// STOP : ゲーム停止状態
    /// BLOCK_MOVE : ブロック移動処理
    /// PLAYER : プレイヤー操作処理
    /// END : ゲームオーバー処理
    /// </summary>
    enum GameState {
        START,
        STOP,
        BLOCK_MOVE,
        PLAYER,
        END,
    }
    /// <summary>
    /// 現在のゲーム状態
    /// </summary>
    [SerializeField] private GameState _gameState = GameState.START;
    /// <summary>
    /// ゲームの状態設定を行うメソッド
    /// ゲームの状態は以下参照
    /// START : ゲーム開始前処理
    /// STOP : ゲーム停止状態
    /// BLOCK_MOVE : ブロック移動処理
    /// PLAYER : プレイヤー操作処理
    /// END : ゲームオーバー処理
    /// </summary>
    /// <param name="gameState">ゲームの状態を指定</param>
    private void SetGameState(GameState gameState) {
        this._gameState = gameState;
    }
    /// <summary>
    /// 現在のゲーム状態を取得する
    /// ゲームの状態は以下参照
    /// START : ゲーム開始前処理
    /// STOP : ゲーム停止状態
    /// BLOCK_MOVE : ブロック移動処理
    /// PLAYER : プレイヤー操作処理
    /// END : ゲームオーバー処理
    /// </summary>
    /// <returns>ゲーム状態</returns>
    private GameState GetGameState() {
        return this._gameState;
    }

    // キーパットの入力状態
    private bool _isInputState;

    #region 変更箇所
    //行動した回数
    public static int _moveCount;
    //行動した回数の最大値
    private int _moveCountMax;
    //タイトルシーン名
    private string _titleSceneName;
    //ゲームクリアシーン名
    private string _gameClearSceneName;
    #endregion
    #endregion

    public int PropertyMoveCount
   {
        // 通称ゲッター。呼び出した側がscoreを参照できる
        get {
            return _moveCount;
        }
        // 通称セッター。value はセットする側の数字などを反映する
        set {
            _moveCount = value;
        } 
    }

    public int PropertyMoveCountMax {
        get {
            return _moveCountMax;
        }
        set {
            _moveCountMax = value;
        }
    }

    #region メソッド(変更箇所あり)  

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    private void Awake() {
        // コンポーネント取得
        _fieldArrayData = GetComponent<FieldArrayData>();

        #region 変更箇所
        //キーパッドの入力状態
        _isInputState = false;
        //手数初期化
        _moveCount = 0;
        //手数最大値
        _moveCountMax = 99;
        //シーン名
        _titleSceneName = "TitleScene";
        _gameClearSceneName = "GameClearScene";
        #endregion
    }

    /// <summary>  
    /// 更新処理  
    /// </summary>  
    private void Update() {
        // ゲーム状態によって処理を分ける
        switch (_gameState) {
            case GameState.START:
                SetGameState(GameState.PLAYER);
                break;
            case GameState.STOP:
                break;
            case GameState.PLAYER:
                //左右キーまたはADキーを受け取ったときの値を格納
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                //上下キーまたはWSキーを受け取った時の値を格納
                float verticalInput = Input.GetAxisRaw("Vertical");
                // 横入力が0より大きい場合は右に移動
                if (horizontalInput > 0 && !_isInputState) {
                    _fieldArrayData.PlayerMove(
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y),
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y + 1));
                    _isInputState = true;

                }
                // 横入力が0より小さい場合は左に移動
                else if (horizontalInput < 0 && !_isInputState) {
                    _fieldArrayData.PlayerMove(
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y),
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y - 1));
                    _isInputState = true;
                }
                // 縦入力が0より大きい場合は上に移動
                if (verticalInput > 0 && !_isInputState) {
                    _fieldArrayData.PlayerMove(
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y),
                    (int)(_fieldArrayData.PlayerPosition.x - 1),
                    (int)(_fieldArrayData.PlayerPosition.y));
                    _isInputState = true;
                }
                // 縦入力が0より小さい場合は下に移動
                else if (verticalInput < 0 && !_isInputState) {
                    _fieldArrayData.PlayerMove(
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y),
                    (int)(_fieldArrayData.PlayerPosition.x + 1),
                    (int)(_fieldArrayData.PlayerPosition.y));
                    _isInputState = true;

                }
                // 入力状態が解除されるまで再入力できないようにする
                if ((horizontalInput + verticalInput) == 0) {
                    _isInputState = false;
                }
                // クリア判定
                if (_fieldArrayData.GetGameClearJudgment()) {
                    _gameState = GameState.END;
                }

                #region　変更箇所

                //移動可能な場所にプレイヤーが移動すると手数が更新される
                if ((Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.LeftArrow)
                    || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.DownArrow)
                    || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A)
                    || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D)) && FieldArrayData._isPlayerMoveCheck == true) {

                    //手数のカウント
                    _moveCount++;

                    if (_moveCount > _moveCountMax) {
                        SceneManager.LoadScene(_titleSceneName);
                    }

                }
                #endregion


                break;
            case GameState.BLOCK_MOVE:
                break;
            case GameState.END:
                //クリア画面に遷移する
                SceneManager.LoadScene(_gameClearSceneName);
                break;
        }
    }
    #endregion
}