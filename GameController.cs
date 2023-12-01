// ---------------------------------------------------------  
// GameController.cs  
// キー操作やプレイヤーの移動について
// 作成日:  20231024
// 作成者:  竹内
// ---------------------------------------------------------  
// 更新内容
// 20231130
// ・Mathf.floorを変更し(int)を実装。
// ・値の受け渡しをプロパティにて実装

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {


    #region 変数
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
    //行動した回数の最大値の定数
    private const int CONST_MOVECOUNTMAX = 99999;
    //行動した回数
    public static int _moveCount = CONST_MOVECOUNTMAX;
    //行動した回数の最大値
    private int _moveCountMax;
    //タイトルシーン名
    private string _titleSceneName;
    //ゲームクリアシーン名
    private string _gameClearSceneName;
    #endregion
    /// <summary>
    /// 手数の受け渡し
    /// </summary>
    public int PropertyMoveCount
   {
        //呼び出した側がscoreを参照できる
        get {
            return _moveCount;
        }
        //value はセットする側の数字などを反映する
        set {
            _moveCount = value;
        } 
    }
    /// <summary>
    /// 手数の最大値の受け渡し
    /// </summary>
    public int PropertyMoveCountMax {
        get {
            return _moveCountMax;
        }
        set {
            _moveCountMax = value;
        }
    }

    #region メソッド

    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    private void Awake() {
        // コンポーネント取得
        _fieldArrayData = GetComponent<FieldArrayData>();
        //キーパッドの入力状態
        _isInputState = false;
        //手数初期化
        _moveCount =　0;
        //手数最大値
        _moveCountMax = CONST_MOVECOUNTMAX;
        //シーン名
        _titleSceneName = "TitleScene";
        _gameClearSceneName = "GameClearScene";
        
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

                //移動可能な場所にプレイヤーが移動すると手数が更新される
                if (_fieldArrayData.IsPlayerMoveCheck == true && Input.anyKeyDown ) {

                    //手数最大値を超えると終了
                    SceneChanger();

                    //手数のカウント
                    _moveCount++;

                }

                break;
            case GameState.BLOCK_MOVE:
                break;
            case GameState.END:
                //クリア画面に遷移する
                SceneManager.LoadScene(_gameClearSceneName);
                break;
        }
        
        Debug.Log("state" + _isInputState);
    }
    
    private void SceneChanger()
    {
        //手数最大値を超えると終了
        if (_moveCount > _moveCountMax)
        {
            SceneManager.LoadScene(_titleSceneName);
        }
    }
    #endregion
}