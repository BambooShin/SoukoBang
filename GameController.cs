// ---------------------------------------------------------  
// GameController.cs  
// �L�[�����v���C���[�̈ړ��ɂ���
// �쐬��:  20231024
// �쐬��:  �|��
// ---------------------------------------------------------  
// �X�V���e
// 20231130
// �EMathf.floor��ύX��(int)�������B
// �E�l�̎󂯓n�����v���p�e�B�ɂĎ���

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {


    #region �ϐ�
    // �t�B�[���h����N���X�̒�`
     static FieldArrayData _fieldArrayData;
    /// <summary>
    /// �Q�[���̏�ԊǗ��p�\����
    /// START : �Q�[���J�n�O����
    /// STOP : �Q�[����~���
    /// BLOCK_MOVE : �u���b�N�ړ�����
    /// PLAYER : �v���C���[���쏈��
    /// END : �Q�[���I�[�o�[����
    /// </summary>
    enum GameState {
        START,
        STOP,
        BLOCK_MOVE,
        PLAYER,
        END,
    }
    /// <summary>
    /// ���݂̃Q�[�����
    /// </summary>
    [SerializeField] private GameState _gameState = GameState.START;
    /// <summary>
    /// �Q�[���̏�Ԑݒ���s�����\�b�h
    /// �Q�[���̏�Ԃ͈ȉ��Q��
    /// START : �Q�[���J�n�O����
    /// STOP : �Q�[����~���
    /// BLOCK_MOVE : �u���b�N�ړ�����
    /// PLAYER : �v���C���[���쏈��
    /// END : �Q�[���I�[�o�[����
    /// </summary>
    /// <param name="gameState">�Q�[���̏�Ԃ��w��</param>
    private void SetGameState(GameState gameState) {
        this._gameState = gameState;
    }
    /// <summary>
    /// ���݂̃Q�[����Ԃ��擾����
    /// �Q�[���̏�Ԃ͈ȉ��Q��
    /// START : �Q�[���J�n�O����
    /// STOP : �Q�[����~���
    /// BLOCK_MOVE : �u���b�N�ړ�����
    /// PLAYER : �v���C���[���쏈��
    /// END : �Q�[���I�[�o�[����
    /// </summary>
    /// <returns>�Q�[�����</returns>
    private GameState GetGameState() {
        return this._gameState;
    }

    // �L�[�p�b�g�̓��͏��
    private bool _isInputState;
    //�s�������񐔂̍ő�l�̒萔
    private const int CONST_MOVECOUNTMAX = 99999;
    //�s��������
    public static int _moveCount = CONST_MOVECOUNTMAX;
    //�s�������񐔂̍ő�l
    private int _moveCountMax;
    //�^�C�g���V�[����
    private string _titleSceneName;
    //�Q�[���N���A�V�[����
    private string _gameClearSceneName;
    #endregion
    /// <summary>
    /// �萔�̎󂯓n��
    /// </summary>
    public int PropertyMoveCount
   {
        //�Ăяo��������score���Q�Ƃł���
        get {
            return _moveCount;
        }
        //value �̓Z�b�g���鑤�̐����Ȃǂ𔽉f����
        set {
            _moveCount = value;
        } 
    }
    /// <summary>
    /// �萔�̍ő�l�̎󂯓n��
    /// </summary>
    public int PropertyMoveCountMax {
        get {
            return _moveCountMax;
        }
        set {
            _moveCountMax = value;
        }
    }

    #region ���\�b�h

    /// <summary>  
    /// ����������  
    /// </summary>  
    private void Awake() {
        // �R���|�[�l���g�擾
        _fieldArrayData = GetComponent<FieldArrayData>();
        //�L�[�p�b�h�̓��͏��
        _isInputState = false;
        //�萔������
        _moveCount =�@0;
        //�萔�ő�l
        _moveCountMax = CONST_MOVECOUNTMAX;
        //�V�[����
        _titleSceneName = "TitleScene";
        _gameClearSceneName = "GameClearScene";
        
    }

    /// <summary>  
    /// �X�V����  
    /// </summary>  
    private void Update() {
        // �Q�[����Ԃɂ���ď����𕪂���
        switch (_gameState) {
            case GameState.START:
                SetGameState(GameState.PLAYER);
                break;
            case GameState.STOP:
                break;
            case GameState.PLAYER:
                //���E�L�[�܂���AD�L�[���󂯎�����Ƃ��̒l���i�[
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                //�㉺�L�[�܂���WS�L�[���󂯎�������̒l���i�[
                float verticalInput = Input.GetAxisRaw("Vertical");
                // �����͂�0���傫���ꍇ�͉E�Ɉړ�
                if (horizontalInput > 0 && !_isInputState) {
                    _fieldArrayData.PlayerMove(
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y),
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y + 1));
                    _isInputState = true;

                }
                // �����͂�0��菬�����ꍇ�͍��Ɉړ�
                else if (horizontalInput < 0 && !_isInputState) {
                    _fieldArrayData.PlayerMove(
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y),
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y - 1));
                    _isInputState = true;
                }
                // �c���͂�0���傫���ꍇ�͏�Ɉړ�
                if (verticalInput > 0 && !_isInputState) {
                    _fieldArrayData.PlayerMove(
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y),
                    (int)(_fieldArrayData.PlayerPosition.x - 1),
                    (int)(_fieldArrayData.PlayerPosition.y));
                    _isInputState = true;
                }
                // �c���͂�0��菬�����ꍇ�͉��Ɉړ�
                else if (verticalInput < 0 && !_isInputState) {
                    _fieldArrayData.PlayerMove(
                    (int)(_fieldArrayData.PlayerPosition.x),
                    (int)(_fieldArrayData.PlayerPosition.y),
                    (int)(_fieldArrayData.PlayerPosition.x + 1),
                    (int)(_fieldArrayData.PlayerPosition.y));
                    _isInputState = true;

                }
                // ���͏�Ԃ����������܂ōē��͂ł��Ȃ��悤�ɂ���
                if ((horizontalInput + verticalInput) == 0) {
                    _isInputState = false;
                }

                // �N���A����
                if (_fieldArrayData.GetGameClearJudgment()) {
                    _gameState = GameState.END;
                }

                //�ړ��\�ȏꏊ�Ƀv���C���[���ړ�����Ǝ萔���X�V�����
                if (_fieldArrayData.IsPlayerMoveCheck == true && Input.anyKeyDown ) {

                    //�萔�ő�l�𒴂���ƏI��
                    SceneChanger();

                    //�萔�̃J�E���g
                    _moveCount++;

                }

                break;
            case GameState.BLOCK_MOVE:
                break;
            case GameState.END:
                //�N���A��ʂɑJ�ڂ���
                SceneManager.LoadScene(_gameClearSceneName);
                break;
        }
        
        Debug.Log("state" + _isInputState);
    }
    
    private void SceneChanger()
    {
        //�萔�ő�l�𒴂���ƏI��
        if (_moveCount > _moveCountMax)
        {
            SceneManager.LoadScene(_titleSceneName);
        }
    }
    #endregion
}