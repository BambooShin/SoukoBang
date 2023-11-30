// ---------------------------------------------------------  
// TitleScene.cs  
//   
// 作成日:  20231026
// 作成者:  竹内
// ---------------------------------------------------------  
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TitleScene : MonoBehaviour
{
    #region 変数
    //ランキング用キャンバスを格納
    [SerializeField] private Canvas _rankingCanvas;
    #endregion

    #region メソッド  
    //ゲームクリア画面からタイトル画面へ遷移する
    public void TitleSceneChangeButton() {
        SceneManager.LoadScene("TitleScene");
    }
    //スタートボタン選択でステージに移動
    public void StageSceneChangeButton() {
        SceneManager.LoadScene("StageScene");
    }
    //ランキングボタン選択でランキング用キャンバスを表示
    public void RankingCanvasDisplayButton() {
        _rankingCanvas.enabled = true;
    }
    #endregion
}
