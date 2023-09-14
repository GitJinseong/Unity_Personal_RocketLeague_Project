using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerDataManager_Choi : MonoBehaviour
{
    #region [�̱��� & ���� �����]
    // ##################################################################################################
    // ��[�̱��� & ���� �����]
    // ##################################################################################################
        private static PlayerDataManager_Choi m_instance; // �̱����� �Ҵ�� static ����
        public static PlayerDataManager_Choi instance
        {
            get
            {
                // ���� �̱��� ������Ʈ�� �Ҵ��� ���� �ʾҴٸ�
                if (m_instance == null)
                {
                    // ������ ������Ʈ�� ã�� �Ҵ�
                    m_instance = FindObjectOfType<PlayerDataManager_Choi>();
                }

                // �̱��� ������Ʈ�� ��ȯ
                return m_instance;
            }
        }
        [Header("Temps")]
        private Dictionary<string, int> temp_IndexDictionary; // �ӽ÷� �÷��̾� �ε����� �����ϴ� ��ųʸ�
    #endregion

    #region [������ ����Ŭ �޼���]
    // ##################################################################################################
    // ��[������ ����Ŭ �޼���]
    // ##################################################################################################
        private void Awake()
        {
        
        }
    #endregion

    #region [�÷��̾� ������ �޼���]
    // ##################################################################################################
    // ��[�÷��̾� ������ �޼���]
    // ##################################################################################################
    // PlayerCustomizing_Choi���� Ŀ���͸���¡ ���� �� ����� �ش� �����͸�
    // �迭�� �޾Ƽ� PlayerPref�� �����ϴ� �Լ�
    public void SetPlayerPrefForIndex()
    {
        // CustomizingManage_Choi�� ����� �� ī�װ��� ���� �ε��� ���ΰ� �����
        // ��ųʸ��� ȣ��
        temp_IndexDictionary = CustomizingManager_Choi.instance.GetIndexDictionary();
            
        // ��ųʸ��� ����Ǿ� �ִ� ������ foreach�� ��ȸ�� ��
        // PlayerPref�� �����Ѵ�. ex) 
        foreach (KeyValuePair<string, int> value in temp_IndexDictionary)
        {
            PlayerPrefs.SetInt(value.Key, value.Value); // CarFrames, 0 �� ���� ���·� �Է��� ����
            // ����� �޼��� ���
            Debug.Log($"SaveDataForIndex(): �� PlayerPrefs ���� ���� �� " +
                $"Ű: {value.Key}, ��: {value.Value}");
        }

        // �� ���� PlayerPrefs ����
        PlayerPrefs.Save();
    } // SaveData()

    public int GetPlayerPrefForIndex(string key)
    {
        // PlayerPrefs�� ����� Index�� ������
        // �����ε带 �ؼ� �������� �� -1�� ��ȯ
        int temp_Value = PlayerPrefs.GetInt(key, -1);
        // PlayerPrefs�� ����� Index�� �������µ� �������� ���
        if (temp_Value != -1)
        {
            // ����� �޼��� ���
            Debug.Log($"GetPlayerPrefForIndex(): �� PlayerPrefs �ε� ���� �� " +
                $"Ű: {key}, ��: {temp_Value}");
        }

        // PlayerPrefs�� ����� Index�� �������� ������ ���
        else
        {
            // ����� �޼��� ���
            Debug.Log($"GetPlayerPrefForIndex(): �� PlayerPrefs �ε� ���� �� " +
                $"Ű: {key} �� Ű�� �ùٸ��� Ȯ�����ּ��� �� ��ũ��Ʈ: PlayerDataManager_Choi");
        }

        // ã�� Index ��ȯ
        return temp_Value;
    } // GetPlayerPrefForIndex()

    #endregion
}
