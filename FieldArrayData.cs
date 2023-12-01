// ---------------------------------------------------------  
// FieldArrayData.cs  
// �Ֆʂɂ��Ăƈړ��̉�
// �쐬��:  20231024
// �쐬��:  �|��
// ---------------------------------------------------------  
// �X�V���e
// 20231130
//�EGameController�X�N���v�g�̃L�[�̎擾���v���p�e�B�ō��W��n���d�l�ɕύX
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FieldArrayData : MonoBehaviour {
    #region �ϐ�

    // �^�O���X�g�̖��O�ɕR�Â��ԍ�
    private const int NO_BLOCK = 0;
    private const int STATIC_BLOCK = 1;
    private const int MOVE_BLOCK = 2;
    private const int PLAYER = 3;
    private const int TARGET = 4;
    /// <summary>
    /// �V�[���ɔz�u����I�u�W�F�N�g�̃��[�g���q�G�����L�[����ݒ肷��
    /// </summary>
    [Header("�z�u����I�u�W�F�N�g�̐e�I�u�W�F�N�g��ݒ�")]
    [SerializeField] private GameObject _fieldRootObject;
    /// <summary>
    /// �t�B�[���h�̃I�u�W�F�N�g���X�g
    /// 0 ��
    /// 1 �����Ȃ��u���b�N
    /// 2 �����u���b�N
    /// 3 �v���C���[
    /// 4 �^�[�Q�b�g
    /// </summary>
    private string[] _fieldObjectTagList = { "", "StaticBlock", "MoveBlock"
            , "Player", "TargetPosition" };
    [Header("�����Ȃ��I�u�W�F�N�g��ݒ�(Tag�����ʂ���)")]
    [SerializeField] private GameObject _staticBlock;
    [Header("�����I�u�W�F�N�g��ݒ�(Tag�����ʂ���)")]
    [SerializeField] private GameObject _moveBlock;
    [Header("�v���C���[�I�u�W�F�N�g��ݒ�(Tag�����ʂ���)")]
    [SerializeField] private GameObject _player;
    [Header("�^�[�Q�b�g�I�u�W�F�N�g��ݒ�(Tag�����ʂ���)")]
    [SerializeField] private GameObject _target;
    /// <summary>
    /// �t�B�[���h�f�[�^�p�̕ϐ����`
    /// </summary>
    private int[,] _fieldData = {
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        };
    // �c���̍ő吔
    private int _horizontalMaxCount = 0;
    private int _verticalMaxCount = 0;

    /// <summary>
    /// �^�[�Q�b�g�f�[�^�p�̕ϐ����`
    /// ������g_fieldData�𕡐�����
    /// ���t�B�[���h�f�[�^�͏�ɕω����邪
    /// �@�^�[�Q�b�g�p�f�[�^�͓������Ȃ����Ƃ�
    /// �@�^�[�Q�b�g�ɃI�u�W�F�N�g���d�Ȃ��Ă���������悤�ɂ���
    /// �@�N���A����͂��̃^�[�Q�b�g�f�[�^���g��
    /// </summary>
    private int[,] _targetData = {
    { 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0 },
    { 0, 0, 0, 0, 0, 0 },
    };
    // �u���b�N���^�[�Q�b�g�ɓ�������
    private int _targetClearCount = 0;
    // �^�[�Q�b�g�̍ő吔
    private int _targetMaxCount = 0;
    //GameController�Ɏ����Ă����v���C���[�s���ۃt���O(�ړ��ł���Ƃ��ɐ؂�ւ���)
    private bool _isPlayerMoveCheck;

    #endregion

    #region �v���p�e�B
    /// <summary>
    /// �v���C���[�̈ʒu���
    /// </summary>
    public Vector2 PlayerPosition {
        get; set;
    }
    /// <summary>
    /// �v���C���[�̍s���ۂ̎󂯓n��
    /// </summary>
    public bool IsPlayerMoveCheck {
        get => _isPlayerMoveCheck;
        set => _isPlayerMoveCheck = value;
    }


    #endregion

    #region ���\�b�h
    /// <summary>
    /// fieldRootObject�̔z���ɂ���I�u�W�F�N�g�̃^�O��ǂݎ��
    /// X��Y���W�����fieldData�֊i�[���܂��ifieldData�͏㏑���폜���܂��j
    /// fieldData��fieldData[Y,X]�ŕR�Â��Ă���
    /// �t�B�[���h�������Ɏg��
    /// </summary>
    /// <param name="fieldRootObject">�t�B�[���h�I�u�W�F�N�g�̃��[�g�I�u�W�F�N�g��ݒ�</param>
    public void ImageToArray() {
        // �t�B�[���h�̏c�Ɖ��̍ő吔���擾�i�t�B�[���h�̑傫�����擾�j
        foreach (Transform fieldObject in _fieldRootObject.transform) {
            /* 
            * �c�����Ɋւ��Ă͍��W�̌��ˍ�����
            * ���ɍs���ق�y�͌����Ă����̂�-�����邱�Ƃ�
            * y�̈ʒu���t�]�����Ă���
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
                // �^�[�Q�b�g�̍ő�J�E���g
                _targetMaxCount++;
            }
            // �t�B�[���h�f�[�^���^�[�Q�b�g�p�̃f�[�^�ɃR�s�[����
            _targetData = (int[,])_fieldData.Clone();
        }
    }
    /// <summary>
    /// �t�B�[���h�̃T�C�Y��ݒ肷��
    /// �t�B�[���h�̏������Ɏg��
    /// </summary>
    public void SetFieldMaxSize() {
        // �t�B�[���h�̏c�Ɖ��̍ő吔���擾�i�t�B�[���h�̑傫�����擾�j
        foreach (Transform fieldObject in _fieldRootObject.transform) {
            /* 
            * �c�����Ɋւ��Ă͍��W�̌��ˍ�����
            * ���ɍs���ق�y�͌����Ă����̂�-�����邱�Ƃ�
            * y�̈ʒu���t�]�����Ă���
            */
            int positionX = Mathf.FloorToInt(fieldObject.position.x);
            int positionY = Mathf.FloorToInt(-fieldObject.position.y);
            // ���̍ő吔��ݒ肷��
            if (_horizontalMaxCount < positionX) {
                _horizontalMaxCount = positionX;
            }
            // �c�̍ő吔��ݒ肷��
            if (_verticalMaxCount < positionY) {
                _verticalMaxCount = positionY;
            }
        }
        // �t�B�[���h�z��̏�����
        _fieldData = new int[_verticalMaxCount + 1, _horizontalMaxCount + 1];
    }
    /// <summary>
    /// ����N����
    /// �V�[���ɔz�u���ꂽ�I�u�W�F�N�g�����ɔz��f�[�^�𐶐�����
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
            // �z����o�͂���e�X�g
            print("Field------------------------------------------");
            for (int y = 0; y <= _verticalMaxCount; y++) {
                string outPutString = "";
                for (int x = 0; x <= _horizontalMaxCount; x++) {
                    outPutString += _fieldData[y, x];
                }
                print(outPutString);
            }
            print("Field------------------------------------------");
            print("�v���C���[�|�W�V����:" + PlayerPosition);
        }
    }

    /// <summary>
    /// �t�B�[���h�I�u�W�F�N�g����w�肵�����W�̃I�u�W�F�N�g���擾����
    /// tagId��-1�̏ꍇ���ׂẴ^�O��ΏۂɌ�������
    /// �����Ƀq�b�g���Ȃ��ꍇNull��Ԃ�
    /// </summary>
    /// <param name="tagId">�����Ώۂ̃^�O���w��</param>
    /// <param name="fieldRow">�c�ʒu</param>
    /// <param name="fieldCol">���ʒu</param>
    /// <returns></returns>
    public GameObject GetFieldObject(int tagId, int fieldRow, int fieldCol) {
        foreach (Transform fieldObject in _fieldRootObject.transform) {
            if (tagId != -1 && fieldObject.tag != _fieldObjectTagList[tagId]) {
                continue;
            }
            /* 
            * �c�����Ɋւ��Ă͍��W�̌��ˍ�����
            * ���ɍs���ق�y�͌����Ă����̂�-�����邱�Ƃ�
            * y�̈ʒu���t�]�����Ă���
            */
            if (fieldObject.transform.position.x == fieldCol &&
            fieldObject.transform.position.y == -fieldRow) {
                return fieldObject.gameObject;
            }
        }
        return null;
    }
    /// <summary>
    /// �I�u�W�F�N�g���ړ�����
    /// �f�[�^���㏑������̂ňړ��ł��邩�ǂ�����������
    /// �ړ��\�ȏꍇ���������s���Ă�������
    /// </summary>
    /// <param name="preRow">�ړ��O�c���</param>
    /// <param name="preCol">�ړ��O�����</param>
    /// <param name="nextRow">�ړ���c���</param>
    /// <param name="nextCol">�ړ��㉡���</param>
    public void MoveData(int preRow, int preCol, int nextRow, int nextCol) {
        // �I�u�W�F�N�g���ړ�����
        GameObject moveObject =
        GetFieldObject(_fieldData[preRow, preCol], preRow, preCol);
        if (moveObject != null) {
            /* 
            * �c�����Ɋւ��Ă͍��W�̌��ˍ�����
            * ���ɍs���ق�y�͌����Ă����̂�-�����邱�Ƃ�
            * y�̈ʒu���t�]�����Ă���
            */
            // ���W���Ȃ̂ōŏ��̈�����X
            moveObject.transform.position = new Vector2(nextCol, -nextRow);
        }
        // �㏑������̂ŗv����
        _fieldData[nextRow, nextCol] = _fieldData[preRow, preCol];
        // �ړ������猳�̃f�[�^�͍폜����
        _fieldData[preRow, preCol] = NO_BLOCK;
    }
    /// <summary>
    /// �u���b�N���ړ����邱�Ƃ��\���`�F�b�N����
    /// true�̏ꍇ�ړ��\�@flase�̏ꍇ�ړ��s�\
    /// </summary>
    /// <param name="y">�ړ���Y���W</param>
    /// <param name="x">�ړ���X���W</param>
    /// <returns>�u���b�N�ړ��̉�</returns>
    public bool BlockMoveCheck(int y, int x) {
        // �^�[�Q�b�g�u���b�N��������
        if (_targetData[y, x] == TARGET) {
            // �^�[�Q�b�g�N���A�J�E���g���グ��
            _targetClearCount++;
            return true;
        }

        return _fieldData[y, x] == NO_BLOCK;
    }
    /// <summary>
    /// �u���b�N���ړ�����(�u���b�N�ړ��`�F�b�N�����{)
    /// </summary>
    /// <param name="preRow">�ړ��O�c���</param>
    /// <param name="preCol">�ړ��O�����</param>
    /// <param name="nextRow">�ړ���c���</param>
    /// <param name="nextCol">�ړ��㉡���</param>
    public bool BlockMove(int preRow, int preCol, int nextRow, int nextCol) {
        // ���E���O�G���[
        if (nextRow < 0 || nextCol < 0 ||
        nextRow > _verticalMaxCount || nextCol > _horizontalMaxCount) {
            return false;
        }
        bool moveFlag = BlockMoveCheck(nextRow, nextCol);
        // �ړ��\���`�F�b�N����
        if (moveFlag) {
            // �ړ����\�ȏꍇ�ړ�����
            MoveData(preRow, preCol, nextRow, nextCol);
        }
        return moveFlag;
    }

    /// <summary>
    /// �v���C���[���ړ����邱�Ƃ��\���`�F�b�N����
    /// true�̏ꍇ�ړ��\�@flase�̏ꍇ�ړ��s�\
    /// </summary>
    /// <param name="preRow">�ړ��O�c���</param>
    /// <param name="preCol">�ړ��O�����</param>
    /// <param name="nextRow">�ړ���c���</param>
    /// <param name="nextCol">�ړ��㉡���</param>
    /// <returns>�v���C���[�ړ��̉�</returns>

    public bool PlayerMoveCheck(int preRow, int preCol, int nextRow, int nextCol) {
        /* �v���C���[�̈ړ��悪�����u���b�N�̎�
        * �u���b�N���ړ����鏈�������{����
        */
        if (_fieldData[nextRow, nextCol] == MOVE_BLOCK) {
            bool isBlockMoveFlag = BlockMove(nextRow, nextCol,
            nextRow + (nextRow - preRow),
            nextCol + (nextCol - preCol));
            // �^�[�Q�b�g�u���b�N���ړ��ł���ړ��u���b�N��������
            if (_targetData[nextRow, nextCol] == TARGET && isBlockMoveFlag) {
                // �^�[�Q�b�g�N���A�J�E���g��������
                _targetClearCount--;
            }
            return isBlockMoveFlag;
        }
        // �v���C���[�̈ړ��悪��̎��ړ�����
        // �v���C���[�̈ړ��悪�^�[�Q�b�g�̎��ړ�����
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
    /// �Q�[���N���A�̔���
    /// </summary>
    /// <returns>�Q�[���N���A�̗L��</returns>
    public bool GetGameClearJudgment() {
        // �^�[�Q�b�g�N���A���ƃ^�[�Q�b�g�̍ő吔����v������Q�[���N���A
        if (_targetClearCount == _targetMaxCount) {
            return true;
        }
        return false;
    }
    /// <summary>
    /// �v���C���[���ړ�����(�v���C���[�ړ��`�F�b�N�����{)
    /// </summary>
    /// <param name="preRow">�ړ��O�c���</param>
    /// <param name="preCol">�ړ��O�����</param>
    /// <param name="nextRow">�ړ���c���</param>
    /// <param name="nextCol">�ړ��㉡���</param>
    public void PlayerMove(int preRow, int preCol, int nextRow, int nextCol) {

        // �ړ��\���`�F�b�N����
        if (PlayerMoveCheck(preRow, preCol, nextRow, nextCol)) {
            // �ړ����\�ȏꍇ�ړ�����
            MoveData(preRow, preCol, nextRow, nextCol);
            // �v���C���[�̈ʒu���X�V����
            // ���W���Ȃ̂ōŏ��̈�����X
            PlayerPosition = new Vector2Int(nextRow, nextCol);
        }
    }
    #endregion
}