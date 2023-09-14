
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.Rendering.DebugUI;

public class CustomizingManager_Choi : MonoBehaviour
{
    #region [�̱��� & ���� �����]
    // ##################################################################################################
    // ��[�̱��� & ���� �����]
    // ##################################################################################################
        // �̱��� ����
        private static CustomizingManager_Choi m_instance; // �̱����� �Ҵ�� static ����
        public static CustomizingManager_Choi instance
        {
            get
            {
                // ���� �̱��� ������Ʈ�� �Ҵ��� ���� �ʾҴٸ�
                if (m_instance == null)
                {
                    // ������ ������Ʈ�� ã�� �Ҵ�
                    m_instance  = FindObjectOfType<CustomizingManager_Choi>();
                }
            
                // �̱��� ������Ʈ�� ��ȯ
                return m_instance;
            }
        }

        private delegate void CustomizingFunc(); // ��������Ʈ ����

        [Header("CSVFileReader")]
        private string defaultDirectory = "CSVFiles/"; // �⺻ ���丮 ���
        private string[] csvFileList =
        {
            "CarFrameList", "ColorList", "FlagList", "MarkList", 
            "WheelList_FL", "WheelList_FR", "WheelList_BL", "WheelList_BR"
        }; // CSV ���ϸ� ����Ʈ
        private string[] categoryList =
        {
            "CarFrames", "Colors", "Flags", "Marks",
            "Wheels_FL", "Wheels_FR", "Wheels_BL", "Wheels_BR"
        };
        private Dictionary<string, List<string>> temp_DataDictionary; // CSV ���� �ӽ� ����� ��ųʸ�
        private Dictionary<string, Dictionary<string, List<string>>> dataDictionary; // CSV ���ϵ��� ����

        [Header("Delegate")]
        private static Dictionary<string, CustomizingFunc> customizingFuncs = 
            new Dictionary<string, CustomizingFunc>(); // Ű ������ �Լ��� �����ϱ� ���� ��ųʸ� ����

        [Header("Parents")]
        private List<GameObject> parents; // �θ���� �����ϴ� ����Ʈ

        [Header("PlayerPrefab")]
        private string[] CarTypes = {"BlueCar", "OrangeCar"}; // CreateObjectWithCustomizing()���� �÷��̾�
                                                              // ������Ʈ�� ������ �� ȣ��Ǵ� �迭 
                                                              // Resource ������ ���� �̸��� ������ �����ؾ���
        private Dictionary<string, int> 
        temp_IndexDictionary = new Dictionary<string, int>(); // �ε����� �����ϴ� ��ųʸ�(�÷��̾� ������ ���� ����)
                                                              // ī�װ��� Ű ������ �����Ѵ�.
    #endregion

    #region [������ ����Ŭ �޼���]
    // ##################################################################################################
    // ��[������ ����Ŭ �޼���]
    // ##################################################################################################
        private void Awake()
        {
            // csvFileList�� �ִ� ��� CSV ���� �ε�
            ReadCSVFileAndSave();

            // parents ����Ʈ�� GameObject �θ���� �߰�
            InputParents();

            // ������Ʈ Ǯ���� ���� ������Ʈ ����
            CreateObjectPools();

            // ���ϴ� ������Ʈ ��������
            // �Ű������� (ī�װ� / ã�� ���� ������Ʈ��) �����
            Debug.Log($"������ ������Ʈ ������: {FindTargetObject("Wheels_FR", "DefaultWheel_FR").name}");

            // ���� ���� ��������
            Debug.Log($"CarFrames�� ���� ���� �������� : {GetCurrentObject("CarFrames").name}");

            // ���� �ε��� ����
            SaveDataForPlayerPrefab();

            // PlayerPrefab�� ����Ǿ� �ִ� Index ��������
            GetDataForPlayerPrefab("CarFrames");

            // PlayerPrefab�� ����Ǿ� �ִ� Index ���� �����ͼ� ���� ������Ʈ ���� �� ���� �����ϱ� 
            CreateObjectWithCustomizing(0,0);
        } // Awake()
    #endregion

    #region [�ν��Ͻ� ȣ�� �޼���]
    // ##################################################################################################
    // ��[�ν��Ͻ� ȣ�� �޼���]
    // ##################################################################################################
    // csvFileList�� �ִ� ��� CSV ������ �о dataDictionary�� �����ϴ� �Լ�
    // �����ϴ� ��ũ��Ʈ �ν��Ͻ�: CSVReader_Choi
        private void ReadCSVFileAndSave()
            {
                // ������ ��ųʸ� �ʱ�ȭ
                dataDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                // csvFileList�� �ִ� ��� CSV ���ϵ��� dataDictionary�� ����
                for (int i = 0; i < csvFileList.Length; i++)
                {
                    temp_DataDictionary = 
                    CSVReader_Choi.instance.ReadCSVFile(defaultDirectory + csvFileList[i]);
                    dataDictionary.Add(csvFileList[i], temp_DataDictionary);
                }
            } // ReadCSVFileAndSave()

        // temp_IndexDictionary�� ����� ������ PlayerPrefab�� �����ϴ� �Լ�
        // �����ϴ� ��ũ��Ʈ �ν��Ͻ�: PlayerDataManager_Choi
        public void SaveDataForPlayerPrefab()
        {
            // �ν��Ͻ��� ȣ���� �� temp_IndexDictionary�� �ִ� ������ PlayerPrefab�� ����
            PlayerDataManager_Choi.instance.SetPlayerPrefForIndex();
        } // SaveDataForPlayerPrefab()

        // PlayerPrefab�� ����� �ε����� ȣ���ϴ� �Լ�
        // �����ϴ� ��ũ��Ʈ �ν��Ͻ�: PlayerDataManager_Choi
        public int GetDataForPlayerPrefab(string key)
        {
            // PlayerPrefab�� ����� �ε����� Ű������ ȣ��
            int temp_Index = PlayerDataManager_Choi.instance.GetPlayerPrefForIndex(key);
            
            // ȣ���� Index �� ��ȯ
            return temp_Index;
        } // GetDataForPlayerPrefab()
    #endregion

    #region [������Ʈ ���� �޼���]
    // ##################################################################################################
    // ��[������Ʈ ���� �޼���]
    // ##################################################################################################
        // ������Ʈ Ǯ���� ���� ������Ʈ ���� �Լ�
        private void CreateObjectPools()
        {
            // �ӽ� ���� ����
            GameObject temp_parent = new GameObject();
            string temp_Category = "";
            string temp_DataDictionaryKey = "";

            // CSV ���� ���� ��ŭ for�� �ݺ�
            for (int i = 0; i < csvFileList.Length; i++)
            {
                // �ӽ� temp ������ �� �Ҵ�
                temp_parent = parents[i]; // �θ� �Ҵ�
                temp_Category = csvFileList[i]; // ī�װ� �Ҵ�
                temp_DataDictionaryKey = GetKeyForDataDictionary(temp_Category); // ���� Ű �Ҵ�
                // dataDictionary[Ű ��]�� �ִ� ������ �������� ������Ʈ ����
                for (int j = 0; j < dataDictionary[temp_Category][temp_DataDictionaryKey].Count; j++)
                {
                    // ������Ʈ �ν��Ͻ� ���� �Լ� ȣ��
                    CreateInstantiate(temp_Category, temp_DataDictionaryKey, j, temp_parent);
                }
            }
        } // CreateObjectPools()

        // ������Ʈ �ν��Ͻ� ���� �Լ�
        private void CreateInstantiate(string category, string key, int index, GameObject parent)
        {
            // ������ �ν��Ͻ� ������Ʈ ����
            GameObject temp_Prefab = GetPrefab(category, key, index);

            // �ν��Ͻ� ���� ������
            if (temp_Prefab != null)
            {
                // ������Ʈ ���� & ������ ����
                GameObject temp_Obj = Instantiate(temp_Prefab, AdjustChildPosition(parent, temp_Prefab), 
                    temp_Prefab.transform.rotation, parent.transform);
                // ������Ʈ �̸� ����
                temp_Obj.name = temp_Prefab.name;
                // ������Ʈ ��Ȱ��ȭ
                temp_Obj.SetActive(true);

                Debug.Log("CreateInstantiate(): �� ������Ʈ �ν��Ͻ� ������ �����Ͽ����ϴ�.");
            }

            // �ν��Ͻ� ���� ���н�
            else
            {
                // ����� �޼��� ���
                Debug.Log("CreateInstantiate(): �� ������Ʈ �ν��Ͻ� ������ �����Ͽ����ϴ�. �� " +
                    "Prefab�� ������ �� �����ϴ�. �� ��ũ��Ʈ: CustomizingManager_Choi");
                // ����
                return;
            }
        } // CreateInstantiate()
        
    // PlayerPrefab�� ����Ǿ� �ִ� Index�� �������� ������Ʈ�� �����ϰ�
    // ������ ������ ������Ʈ�� �����ϴ� �Լ�
    // *teamID: [0]�� ��� / [1]�� ������ ���̴�.
    public void CreateObjectWithCustomizing(int pvID, int teamID)
    {
        // �ӽ� ���� ����
        string temp_Category = "";
        string temp_DictionaryKey = "";
        int temp_Index = 0;
        // Resources.Load<GameObject>(string)���� �������� �˻� �� ������
        GameObject temp_Prefab = Resources.Load<GameObject>(CarTypes[teamID]);
        // Pos, Rotation ���� �⺻���� �÷��̾� ������Ʈ ����(���/������)
        GameObject temp_PlayerObj = Instantiate(temp_Prefab, Vector3.zero,
           Quaternion.identity);
        // ������Ʈ �̸� ����(BlueCar/OrangeCar)
        temp_PlayerObj.name = CarTypes[teamID];
        // categoryList[] ��ŭ ��ȸ 
        for (int i = 0; i < categoryList.Length; i++)
        {
            // �ӽ� ������ Ű & ��ųʸ� Ű �Ҵ�
            temp_Category = categoryList[i];
            temp_DictionaryKey = GetKeyForDataDictionary(csvFileList[i]);
            // �Ҵ�� Ű�� PlayerPrefab�� ����� Index ȣ��
            temp_Index = GetDataForPlayerPrefab(temp_Category);
            // ������Ʈ �ν��Ͻ� ���� �Լ� ȣ��, ������ ������ �÷��̾� ������Ʈ�� �θ�� ����
            //
            // �Ʒ� ȣ�� �κп��� ��ųʸ� Ű���� ã�������ٴ� �����߻�
            //Debug.Log("Dictionary Contents: " + dataDictionary[temp_DictionaryKey].Count);
            //CreateInstantiate(temp_Category, temp_DictionaryKey, temp_Index, temp_PlayerObj);
        }
    }
    #endregion

    #region [������Ʈ ȣ�� �޼���]
    // ##################################################################################################
    // ��[������Ʈ ȣ�� �޼���]
    // ##################################################################################################
    // �÷��̾� ������Ʈ�� �������� �Լ�
    // ���� ���漭�� ����� pv.IsMine���� �����ؾ���
        private GameObject GetPlayerObject(int pvID)
        {
            // "Player" �±׷� �÷��̾� ������Ʈ�� �˻�
            GameObject temp_PlayerObject = GameObject.FindGameObjectWithTag("Player");
            // �÷��̾� ������Ʈ�� ã�� ���
            if (temp_PlayerObject != null)
            {
                Debug.Log($"GetPlayerObject(): �� ViewID[{pvID}]: �÷��̾� ������Ʈ {temp_PlayerObject.name} ȣ�� �Ϸ� �� ");
            }

            // �÷��̾� ������Ʈ�� ã�� ���� ���
            else
            {
                // ����� �޼��� ���
                Debug.Log($"GetPlayerObject(): �� ViewID[{pvID}]: �÷��̾� ������Ʈ ȣ�� ���� �� " +
                    $"'Player' �±׸� ���� ������Ʈ�� ã�� �� �����ϴ�. �� ��ũ��Ʈ: CustomizingManager_Choi");
            }

            // ã�� �÷��̾� ������Ʈ�� ��ȯ
            return temp_PlayerObject;
        } // GetPlayerObject()

        // ��� �Լ��� ����Ͽ� ���ϴ� ���� �ڽ� ������Ʈ�� ã�� �Լ�
        // �θ� ������Ʈ�� ��� ���� ������ �ִ� �ڽ��� Ž���Ѵ�.
        private Transform FindChildRescursive(Transform parent, string targetName)
        {
            // parent�� ��� �ڽ��� ��ȸ
            foreach (Transform child in parent)
            {
                // ���ϴ� �ڽ� ������Ʈ�� ã�� ���
                if (child.name == targetName)
                {
                    // ã�� �ڽ� ������Ʈ�� ��ȯ
                    return child;
                }

                // ã�� ���� ���
                else
                {
                    // �ڽ� ������Ʈ�� ���� �ڽĵ��� �˻��ϱ� ���� ��� ȣ��
                    // FindChildRescursive()�� �Ű������� child�� �־� ���� ������ Ž���Ѵ�
                    Transform foundChild = FindChildRescursive(child, targetName);
                    // ���ϴ� �ڽ� ������Ʈ�� ã�� ���
                    if (foundChild != null)
                    {
                        // ã�� �ڽ� ������Ʈ ��ȯ
                        return foundChild;
                    }
                }
            }

            // targetName�� ��ġ�ϴ� �ڽ� ������Ʈ�� ã�� ���� ���
            // ��͸� ���� null ��ȯ
            return null;
        } // FindChildRescursive()

        // �ڽ� ������Ʈ�� �������� �Լ�
        private GameObject GetChildObject(GameObject parentObj, string childName)
        {
            Transform temp_ParentTransform = parentObj.transform;

            // ��� �Լ��� ����Ͽ� ���ϴ� �ڽ� ������Ʈ�� ã��
            Transform temp_ChildTransform = FindChildRescursive(parentObj.transform, childName);

            // �ڽ� ������Ʈ�� Ʈ�������� ã�� ���
            if (temp_ChildTransform != null)
            {
                Debug.Log($"GetChildObject(): �� {parentObj.name} �ڽ� ������Ʈ Ʈ������ " +
                    $"{childName} ȣ�� �Ϸ�");
            }

            // �ڽ� ������Ʈ Ʈ�������� ã�� ���� ���
            else
            {
                // ����� �޼��� ���
                Debug.Log($"GetChildObject(): �� {parentObj.name} �ڽ� ������Ʈ Ʈ������ ȣ�� ���� �� " +
                    $"�ڽ� {childName}�� ã�� �� �����ϴ�. �� ��ũ��Ʈ: CustomizingManager_Choi");
            }

            // temp_Obj�� ã�Ƴ� �ڽ� ������Ʈ �߰�
            GameObject temp_Obj = temp_ChildTransform.gameObject;

            // ã�� �ڽ� ������Ʈ ��ȯ
            return temp_Obj;
        } // GetChildObject()

        // �θ� ������Ʈ�� Ȱ��ȭ�� �ڽ� ���� ������Ʈ�� ã�Ƽ� ��ȯ�ϴ� �Լ�
        // ���� ������Ʈ�� ã�� ��� �ε����� temp_IndexDictionary�� ����ȴ�.
        private GameObject GetChildForActiveTrue(Transform parent)
        {
            int temp_Index = -1;
            string temp_Key = parent.name; // �ε��� ��ųʸ� ����� Ű ��
            // parent ������Ʈ�� �ڽ��� ���� ��ȸ
            foreach(Transform child in parent)
            {
                temp_Index += 1;
                // �ڽ� ������Ʈ�� Ȱ��ȭ ������ ���
                if (child.gameObject.activeSelf)
                {
                    // ���� �ε����� IndexDictionary�� ����
                    temp_IndexDictionary[temp_Key] = temp_Index;
                    // ã�� �ڽ� ������Ʈ�� ��ȯ
                    return child.gameObject;
                }
            }
            // ����� �޼��� ���
            Debug.Log($"GetChildForActiveTrue(): �� Ȱ��ȭ�� {parent}�� �ڽ� ������Ʈ �˻� ���� �� " +
                $"ã�� �ڽ� ������Ʈ�� Active ���¸� Ȯ���ϼ���. �� ��ũ��Ʈ: CustomizingManager_Choi");

            // ã�� ���� ��� null ��ȯ
            return null;
        } // GetChildForActiveTrue()

        // �÷��̾��� ���� ���� ������Ʈ�� �������� �Լ�
        // �Ű������� ī�װ��� ������ �ش� ī�װ��� Ȱ��ȭ�� ������Ʈ�� ��ȯ
        private GameObject GetCurrentObject(string category)
        {
            // �θ��� �÷��̾� ������Ʈ�� ã��(���� ����ID�־����)
            GameObject temp_ParentObj = GetPlayerObject(0);
            // ī�װ��� �ش��ϴ� ������Ʈ ��������
            GameObject temp_CategoryObj = FindChildRescursive(temp_ParentObj.transform, category).gameObject;
            // ī�װ��� �ִ� Ȱ��ȭ�� ���� ���� ������Ʈ ��������
            GameObject temp_Obj = GetChildForActiveTrue(temp_CategoryObj.transform);
            // ������Ʈ�� ���� �������� ���
            if (temp_ParentObj != null && temp_CategoryObj != null && temp_Obj != null)
            {
                Debug.Log($"GetCurrentObject(): �� {temp_ParentObj.name} �ڽ� ������Ʈ " +
                    $"{temp_CategoryObj.name}�� Ȱ��ȭ�� ���� ���� ������Ʈ {temp_Obj.name} ȣ�� �Ϸ�");
            }

            // �ϳ��� ������Ʈ�� �������� ���� ���
            else
            {
                // ����� �޼��� ���
                Debug.Log($"GetCurrentObject(): �� {temp_ParentObj.name} �ڽ� ������Ʈ " +
                    $"{temp_CategoryObj.name}�� �� Ȱ��ȭ�� ���� ���� ������Ʈ�� ã�� �� �����ϴ�. " +
                    $"�� ����: temp_ParentObj = {temp_ParentObj != null}, " +
                    $"temp_CategoryObj = {temp_CategoryObj != null}, " +
                    $"temp_Obj = {temp_Obj != null} " +
                    $"��ũ��Ʈ: CustomizingManager_Choi");
            }

            // ã�� ���� ���� ������Ʈ ��ȯ
            return temp_Obj;
        } // GetCurrentObject()

        // ���ϴ� �÷��̾� ���� ������Ʈ�� �������� �Լ�
        // �Ű������� ī�װ��� ���ϴ� ������Ʈ �̸��� �ִ´�
        // ��� �Լ��� ȣ���Ͽ� ��� ���������� Ž���ؼ� ���� ������ �����´�.
        private GameObject FindTargetObject(string category, string targetObjName)
        {
            // �÷��̾� ������Ʈ�� ������
            GameObject temp_PlayerObj = GetPlayerObject(0); // �Ű������� ���� ����� ID �־����

            // ���� ������ �������� ���� �÷��̾��� �ڽ��� ī�װ� ������Ʈ�� ������
            GameObject temp_CategoryObj = GetChildObject(temp_PlayerObj, category);

            // ī�װ� ������Ʈ�� �ڽ����� �ִ� ���� ������Ʈ�� ������
            GameObject temp_Obj = GetChildObject(temp_CategoryObj, targetObjName);

            // ���� ���� ������Ʈ�� ������ ���
            if (temp_PlayerObj != null && temp_CategoryObj != null && temp_Obj != null)
            {
                Debug.Log($"FindTargetObject(): �� ��� {category}/{targetObjName} �� " +
                    $"���� ���� {targetObjName} ȣ�� �Ϸ�");
            }

            // ���� ���� ������Ʈ�� �������� ���� ���
            else
            {
                // ����� �޼��� ���
                Debug.Log($"FindTargetObject(): �� ��� {category}/{targetObjName} �� " +
                    $"���� ���� {targetObjName} ȣ�� ���� �� {targetObjName}�� ������ �� �����ϴ�. �� " +
                    $"����: temp_PlayerObj = {temp_PlayerObj != null}, " +
                    $"temp_CategoryObj = {temp_CategoryObj != null}, temp_Obj = {temp_Obj != null} �� " +
                    $"��ũ��Ʈ: CustomizingManager_Choi");
            }

            // ���� ������Ʈ ��ȯ
            return temp_Obj;
        } // GetCurrentObject()

        // ���ϴ� ī�װ��� �������� ��ȯ�ϴ� �Լ�
        private GameObject GetPrefab(string category, string Key, int index)
        {
            // CSV ������ ��ȯ�ϴ� �������� ������ ����� ������ �ذ��ϱ� ���� Trim() �Լ� ȣ��
            string temp_Name = dataDictionary[category][Key][index].Trim();
            GameObject temp_Prefab = Resources.Load<GameObject>(temp_Name);

            // ������ �ε� ������
            if (temp_Prefab != null)
            {
                Debug.Log($"GetPrefab(): �� ��� {category}/{temp_Name} �� ������ �ε� ����");
            }

            // ������ �ε� ���н�
            else
            {
                Debug.Log($"GetPrefab(): �� ��� {category}/{temp_Name} �� ������ �ε� ���� �� " +
                    $"Resources ������ ��ġ�ϴ� Prefab�� �����ϴ�. �� ��ũ��Ʈ: CustomizingManager_Choi");
            }

            // ������ ��ȯ
            return temp_Prefab;
        } //GetPrefab()
    #endregion

    #region [������Ʈ ���� �޼���]
    // ##################################################################################################
    // ��[������Ʈ ���� �޼���]
    // ##################################################################################################
    // ������Ʈ ��� �Լ�
        private void ToggleObject(GameObject currentObj, GameObject newObj)
        {
            currentObj.SetActive(false); // ���� ������Ʈ ��Ȱ��ȭ
            newObj.SetActive(true); // ���ο� ������Ʈ Ȱ��ȭ

            // �Ʒ��� category�� �ٲ� ������Ʈ�� ���� ������ playerPrefab�� �����ϱ�

        } //ToggleObject()

        // �θ�� �ڽİ��� �������� �����ϴ� �Լ�
        private Vector3 AdjustChildPosition(GameObject parent, GameObject child)
        {
          // Ʈ������ ȣ��
            Vector3 parentPos = parent.transform.position;
            Vector3 childPos = child.transform.position;

            // ������ ����(�θ�Pos + �ڽ�Pos)
            childPos = parentPos + childPos;

            // ������ ������ ��ȯ
            return childPos;
        } // AdjustChildPosition()
    #endregion

    #region [���� ���� �޼���]
    // ##################################################################################################
    // ��[���� ���� �޼���]
    // ##################################################################################################
        // Parents ����Ʈ�� �θ���� �߰��ϴ� �Լ�
        private void InputParents()
        {
            // ����Ʈ �ʱ�ȭ
            parents = new List<GameObject>();
            // �ӽ� ���� ����
            GameObject temp_Obj = new GameObject();
            // �� ī�װ��� �ִ� �θ� ������Ʈ���� ã�� parents�� �߰�
            for (int i = 0; i < categoryList.Length; i++)
            {
                temp_Obj = GameObject.Find(categoryList[i]);
                if (temp_Obj != null)
                {
                    parents.Add(temp_Obj);
                }
            }
        } // InputParents()    

          // dataDictionary ���ٿ� Key�� ��ȯ�ϴ� �Լ�
          // "PrefabName"���� �ѹ��� �����ϸ� ������ �� ���� foreach��
          // Key�� ã�Ƽ� �����Ѵ�.
        private string GetKeyForDataDictionary(string category)
        {
            string temp_DataDictionaryKey = "";

            foreach (KeyValuePair<string, List<string>> value in dataDictionary[category])
            {
                // Key ���� PrefabName�� ���
                if (value.Key.Contains("PrefabName"))
                {
                    // Ű �� �߰�
                    temp_DataDictionaryKey = value.Key;

                    // �� ���� �����ϰ� ����
                    break;
                }
            }

            // Ű ��ȯ
            return temp_DataDictionaryKey;
        } // GetKeyForDataDictionary()
        
        // PlayerDataManager_Choi���� PlayerPrefab�� �����ϱ� ����
        // �� ī�װ��� ���� �ε����� ����� temp_IndexDictionary�� ��ȯ�ϴ� �Լ�
        public Dictionary<string, int> GetIndexDictionary()
        {
            // temp_IndexDictionary�� ��ȯ
            return temp_IndexDictionary;
        }
    #endregion

    #region [�̻�� �޼���]
    // ##################################################################################################
    // ��[�̻�� �޼���]
    // ##################################################################################################
    // ��������Ʈ ȣ�� �Լ�
        private void CallDelegateFunc(string funcName)
        {
            // ��������Ʈ �Լ� �ȿ� �ش��ϴ� �Լ����� �ִ��� Ȯ��
            if (customizingFuncs.ContainsKey(funcName))
            {
                CustomizingFunc func = customizingFuncs[funcName];
                func(); // �Լ� ȣ��
            }
        } // CallDelegateFunc()
    #endregion
}
