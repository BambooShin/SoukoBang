// ---------------------------------------------------------  
// RankingManager.cs  
//   
// 作成日:  20231031
// 作成者:  竹内
// ---------------------------------------------------------  
using UnityEngine;
using System.Collections;

public class RankingManager : MonoBehaviour {
    #region 変数
    //GameControllerスクリプトの_moveCountを格納する変数
    private int _gameControllerMoveCount;
    #endregion

    #region メソッド  
    /// <summary>  
    /// 初期化処理  
    /// </summary>  
    private void Awake() {
        _gameControllerMoveCount = GameController._moveCount;
    }

    //ボタンに反応してランキングボードを呼び出す
    public void RankingDisplayButton() {
        naichilab.RankingLoader.Instance.SendScoreAndShowRanking(_gameControllerMoveCount);
    }

    #endregion
}
