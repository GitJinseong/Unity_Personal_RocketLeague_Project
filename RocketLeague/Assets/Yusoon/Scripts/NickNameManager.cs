using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NickNameManager : MonoBehaviour
{
    public TMP_Text nickNameText;
    public TMP_InputField nickNameInputField;
    public GameObject nickNameUi;
    string savedNickName;

    private void Awake()
    {
        savedNickName = PlayerPrefs.GetString("PlayerNickName");
        if (string.IsNullOrEmpty(savedNickName))
        {
            // 저장된 닉네임이 없으면 입력 필드를 활성화합니다.
            nickNameUi.gameObject.SetActive(true);
        }
        else
        {
            // 저장된 닉네임이 있으면 텍스트로 표시합니다.
            nickNameText.text = savedNickName;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            SaveNickName();
            
        }

    }
    // 입력 필드에서 닉네임을 저장하는 함수
    public void SaveNickName()
    {
        string inputNickName = nickNameInputField.text;
        // 입력된 닉네임을 PlayerPrefs에 저장합니다.
        PlayerPrefs.SetString("PlayerNickName", inputNickName);

        // 입력 필드를 비활성화하고 텍스트로 표시합니다.
        nickNameInputField.gameObject.SetActive(false);
        nickNameText.text = inputNickName;
        nickNameUi.SetActive(false);
    }
}
