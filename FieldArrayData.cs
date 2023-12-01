// ---------------------------------------------------------  
// FieldArrayData.cs  
// 盤面についてと移動の可否
// 作成日:  20231024
// 作成者:  竹内
// ---------------------------------------------------------  
// 更新内容
// 20231130
//・GameControllerスクリプトのキーの取得をプロパティで座標を渡す仕様に変更
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldArrayData : MonoBehaviour {
    #region 変数

    // タグリストの名前に紐づく番号
    private const int NO_BLOCK = 0;
    private const int STATIC_BLOCK = 1;
    private const int MOVE_BLOCK = 2;
    private const int PLAYER = 3;
    private const int TARGET = 4;
    /// <summary>
    /// シーンに配置するオブジェクトのルートをヒエラルキーから設定する
    /// </summary>
    [Header("配置するオブジェクトの親オブジェクトを設定")]
    [SerializeField] private GameObject _fieldRootObject;
    /// <summary>
    /// フィールドのオブジェクトリスト
    /// 0 空欄
    /// 1 動かないブロック
    /// 2 動くブロック
    /// 3 プレイヤー
    /// 4 ターゲット
    /// </summary>
    private string[] _fieldObjectTagList = { "", "StaticBlock", "MoveBlock"
            , "Player", "TargetPosition" };
    [Header("動かないオブジェクトを設定(Tagを識別する)")]
    [SerializeField] private GameObject _staticBlock;
    [Header("動くオブジェクトを設定(Tagを識別する)")]
    [SerializeField] private GameObject _moveBlock;
    [Header("プレイヤーオブジェクトを設定(Tagを識別する)")]
    [SerializeField] private GameObject _player;
    [Header("ターゲットオブジェクトを設定(Tagを識別する)")]
    [SerializeField] private GameObject _target;
    /// <summary>
    /// フィールドデータ用の変数を定義
    /// </summary>
    private int[,] _fieldData = {
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        };
    // 縦横の最大数
    private int _horizontalMaxCount = 0;
    private int _verticalMaxCount = 0;

    /// <summary>
    /// ターゲットデータ用の変数を定義
    /// 初期にg_fieldDataを複製する
    /// ※フィールドデータは常に変化するが
    /// 　ターゲット用データは動かさないことで
    /// 　ターゲットにオブジェクトが重なっても動かせるようにする
    /// 　クリア判定はこのターゲットデータを使う
    /// </summary>
    private int[,] _targetData = {
    { 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0 },
    };
    // ブロックがターゲットに入った数
    private int _targetClearCount = 0;
    // ターゲットの最大数
    private int _targetMaxCount = 0;
    //GameControllerに持っていくプレイヤー行動可否フラグ(移動できるときに切り替える)
    private bool _isPlayerMoveCheck;

    #endregion

    #region プロパティ
    /// <summary>
    /// プレイヤーの位置情報
    /// </summary>
    public Vector2 PlayerPosition {
        get; set;
    }
    /// <summary>
    /// プレイヤーの行動可否の受け渡し
    /// </summary>
    public bool IsPlayerMoveCheck {
        get => _isPlayerMoveCheck;
        set => _isPlayerMoveCheck = value;
    }


    #endregion

    #region メソッド
    /// <summary>
    /// fieldRootObjectの配下にあるオブジェクトのタグを読み取り
    /// XとY座標を基にfieldDataへ格納します（fieldDataは上書き削除します）
    /// fieldDataはfieldData[Y,X]で紐づいている
    /// フィールド初期化に使う
    /// </summary>
    /// <param name="fieldRootObject">フィールドオブジェクトのルートオブジェクトを設定</param>
    public void ImageToArray() {
        // フィールドの縦と横の最大数を取得（フィールドの大きさを取得）
        foreach (Transform fieldObject in _fieldRootObject.transform) {
            /* 
            * 縦方向に関しては座標の兼ね合い上
            * 下に行くほどyは減っていくので-をつけることで
            * yの位置を逆転させている
            */
            int fieldCol = Mathf.FloorToInt(fieldObject.position.x);
            int fieldRow = Mathf.FloorToInt(-fieldObject.position.y);
            if (_fieldObjectTagList[STATIC_BLOCK].Equals(fieldObject.tag)) {
                _fieldData[fieldRow, fieldCol] = STATIC_BLOCK;
            } else if (_fieldObjectTagList[MOVE_BLOCK].Equals(fieldObject.tag)) {
                _fieldData[fieldRow, fieldCol] = MOVE_BLOCK;
            } else if (_fieldObjectTagList[PLAYER].Equals(fieldObject.tag)) {
                _fieldData[fieldRow, fieldCol] = PLAYER;
                PlayerPosition = new Vector2(fieldRow, fieldCol);
            } else if (_fieldObjectTagList[TARGET].Equals(fieldObject.tag)) {
                _fieldData[fieldRow, fieldCol] = TARGET;
                // ターゲットの最大カウント
                _targetMaxCount++;
            }
            // フィールドデータをターゲット用のデータにコピーする
            _targetData = (int[,])_fieldData.Clone();
        }
    }
    /// <summary>
    /// フィールドのサイズを設定する
    /// フィールドの初期化に使う
    /// </summary>
    public void SetFieldMaxSize() {
        // フィールドの縦と横の最大数を取得（フィールドの大きさを取得）
        foreach (Transform fieldObject in _fieldRootObject.transform) {
            /* 
            * 縦方向に関しては座標の兼ね合い上
            * 下に行くほどyは減っていくので-をつけることで
            * yの位置を逆転させている
            */
            int positionX = Mathf.FloorToInt(fieldObject.position.x);
            int positionY = Mathf.FloorToInt(-fieldObject.position.y);
            // 横の最大数を設定する
            if (_horizontalMaxCount < positionX) {
                _horizontalMaxCount = positionX;
            }
            // 縦の最大数を設定する
            if (_verticalMaxCount < positionY) {
                _verticalMaxCount = positionY;
            }
        }
        // フィールド配列の初期化
        _fieldData = new int[_verticalMaxCount + 1, _horizontalMaxCount + 1];
    }
    /// <summary>
    /// 初回起動時
    /// シーンに配置されたオブジェクトを元に配列データを生成する
    /// </summary>
    private void Awake() {
        SetFieldMaxSize();
        ImageToArray();
    }
    private void Update() {
        if (IsPlayerMoveCheck) {
            Debug.Log("true");
            //UnityEditor.EditorApplication.isPaused = true;
        } else {
            Debug.Log("false");
        }

        if (Input.GetKeyDown(KeyCode.H)) {
            // 配列を出力するテスト
            print("Field------------------------------------------");
            for (int y = 0; y <= _verticalMaxCount; y++) {
                string outPutString = "";
                for (int x = 0; x <= _horizontalMaxCount; x++) {
                    outPutString += _fieldData[y, x];
                }
                print(outPutString);
            }
            print("Field------------------------------------------");
            print("プレイヤーポジション:" + PlayerPosition);
        }
    }

    /// <summary>
    /// フィールドオブジェクトから指定した座標のオブジェクトを取得する
    /// tagIdが-1の場合すべてのタグを対象に検索する
    /// 検索にヒットしない場合Nullを返す
    /// </summary>
    /// <param name="tagId">検索対象のタグを指定</param>
    /// <param name="fieldRow">縦位置</param>
    /// <param name="fieldCol">横位置</param>
    /// <returns></returns>
    public GameObject GetFieldObject(int tagId, int fieldRow, int fieldCol) {
        foreach (Transform fieldObject in _fieldRootObject.transform) {
            if (tagId != -1 && fieldObject.tag != _fieldObjectTagList[tagId]) {
                continue;
            }
            /* 
            * 縦方向に関しては座標の兼ね合い上
            * 下に行くほどyは減っていくので-をつけることで
            * yの位置を逆転させている
            */
            if (fieldObject.transform.position.x == fieldCol &&
            fieldObject.transform.position.y == -fieldRow) {
                return fieldObject.gameObject;
            }
        }
        return null;
    }
    /// <summary>
    /// オブジェクトを移動する
    /// データを上書きするので移動できるかどうか検査して
    /// 移動可能な場合処理を実行してください
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public void MoveData(int preRow, int preCol, int nextRow, int nextCol) {
        // オブジェクトを移動する
        GameObject moveObject =
        GetFieldObject(_fieldData[preRow, preCol], preRow, preCol);
        if (moveObject != null) {
            /* 
            * 縦方向に関しては座標の兼ね合い上
            * 下に行くほどyは減っていくので-をつけることで
            * yの位置を逆転させている
            */
            // 座標情報なので最初の引数はX
            moveObject.transform.position = new Vector2(nextCol, -nextRow);
        }
        // 上書きするので要注意
        _fieldData[nextRow, nextCol] = _fieldData[preRow, preCol];
        // 移動したら元のデータは削除する
        _fieldData[preRow, preCol] = NO_BLOCK;
    }
    /// <summary>
    /// ブロックを移動することが可能かチェックする
    /// trueの場合移動可能　flaseの場合移動不可能
    /// </summary>
    /// <param name="y">移動先Y座標</param>
    /// <param name="x">移動先X座標</param>
    /// <returns>ブロック移動の可否</returns>
    public bool BlockMoveCheck(int y, int x) {
        // ターゲットブロックだったら
        if (_targetData[y, x] == TARGET) {
            // ターゲットクリアカウントを上げる
            _targetClearCount++;
            return true;
        }

        return _fieldData[y, x] == NO_BLOCK;
    }
    /// <summary>
    /// ブロックを移動する(ブロック移動チェックも実施)
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public bool BlockMove(int preRow, int preCol, int nextRow, int nextCol) {
        // 境界線外エラー
        if (nextRow < 0 || nextCol < 0 ||
        nextRow > _verticalMaxCount || nextCol > _horizontalMaxCount) {
            return false;
        }
        bool moveFlag = BlockMoveCheck(nextRow, nextCol);
        // 移動可能かチェックする
        if (moveFlag) {
            // 移動が可能な場合移動する
            MoveData(preRow, preCol, nextRow, nextCol);
        }
        return moveFlag;
    }

    /// <summary>
    /// プレイヤーを移動することが可能かチェックする
    /// trueの場合移動可能　flaseの場合移動不可能
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    /// <returns>プレイヤー移動の可否</returns>

    public bool PlayerMoveCheck(int preRow, int preCol, int nextRow, int nextCol) {
        /* プレイヤーの移動先が動くブロックの時
        * ブロックを移動する処理を実施する
        */
        if (_fieldData[nextRow, nextCol] == MOVE_BLOCK) {
            bool isBlockMoveFlag = BlockMove(nextRow, nextCol,
            nextRow + (nextRow - preRow),
            nextCol + (nextCol - preCol));
            // ターゲットブロックかつ移動できる移動ブロックだったら
            if (_targetData[nextRow, nextCol] == TARGET && isBlockMoveFlag) {
                // ターゲットクリアカウントを下げる
                _targetClearCount--;
            }
            return isBlockMoveFlag;
        }
        // プレイヤーの移動先が空の時移動する
        // プレイヤーの移動先がターゲットの時移動する
        if (_fieldData[nextRow, nextCol] == NO_BLOCK ||
        _fieldData[nextRow, nextCol] == TARGET) {
            IsPlayerMoveCheck = true;
            return true;
        }
        if (_fieldData[nextRow, nextCol] == STATIC_BLOCK || _fieldData[preRow, preCol] == TARGET) {
            IsPlayerMoveCheck = false;
        }
        /*
        if (_fieldData[nextRow, nextCol] == STATIC_BLOCK || _fieldData[preRow, preCol] == TARGET) {
            _isPlayerMoveCheck = false;
            return false;
        }
        */
        return false;
    }
    /// <summary>
    /// ゲームクリアの判定
    /// </summary>
    /// <returns>ゲームクリアの有無</returns>
    public bool GetGameClearJudgment() {
        // ターゲットクリア数とターゲットの最大数が一致したらゲームクリア
        if (_targetClearCount == _targetMaxCount) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// プレイヤーを移動する(プレイヤー移動チェックも実施)
    /// </summary>
    /// <param name="preRow">移動前縦情報</param>
    /// <param name="preCol">移動前横情報</param>
    /// <param name="nextRow">移動後縦情報</param>
    /// <param name="nextCol">移動後横情報</param>
    public void PlayerMove(int preRow, int preCol, int nextRow, int nextCol) {

        // 移動可能かチェックする
        if (PlayerMoveCheck(preRow, preCol, nextRow, nextCol)) {
            // 移動が可能な場合移動する
            MoveData(preRow, preCol, nextRow, nextCol);
            // プレイヤーの位置を更新する
            // 座標情報なので最初の引数はX
            PlayerPosition = new Vector2Int(nextRow, nextCol);
        }
    }
    #endregion
}