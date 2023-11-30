// ---------------------------------------------------------  
// ScoreUI.cs  
//   
// 作成日:  
// 作成者:  
// ---------------------------------------------------------  
using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{

    #region 変数  
    //手数表示用テキスト
    [SerializeField] private TextMeshProUGUI _moveCounterText;
    //GameControllerクラスを定義
    private GameController _gameController;
    #endregion

    #region メソッド  
    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    private void Awake() {
        // コンポーネント取得
        _gameController = GetComponent<GameController>();
        this._gameController = FindObjectOfType<GameController>(); // インスタンス化
        _gameController.PropertyMoveCount = _gameController.
            //FUCK
    }

    /// <summary>  
    /// 更新処理  
    /// </summary>  
    void Update ()
     {
        //手数を表示
        _moveCounterText.text = "手数:" + _gameController.PropertyMoveCount;
    }
  
    #endregion
}
