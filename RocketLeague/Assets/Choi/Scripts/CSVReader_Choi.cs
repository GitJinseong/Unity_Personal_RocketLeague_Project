using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Rewired.ComponentControls.Data;

public class CSVReader_Choi : MonoBehaviour
{
    #region 싱글톤 선언
    private static CSVReader_Choi m_instance; // 싱글톤이 할당될 static 변수
    public static CSVReader_Choi instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 CSVReader 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<CSVReader_Choi>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    #endregion

    public const char DELIMITER = ','; // CSV 파일에서 사용하는 구분자 (기본값은 콤마)

    // csv 파일의 정보를 행과 열로 구분하여 저장할 딕셔너리
    // 행은 키 값이 되고, 열은 키 값 내부의 값이 된다.
    public Dictionary<string, List<string>> dataDictionary;

    // CSV 파일을 읽는 함수
    // csvFileName에 "CSVFiles"와 같은 Resources 안에 있는 디렉토리명과
    // WheelList.csv"과 같이 csv 파일의 이름을 입력한다.
    public Dictionary<string, List<string>> ReadCSVFile(string csvFileName)
    {
        dataDictionary = new Dictionary<string, List<string>>();
        TextAsset filePath = Resources.Load<TextAsset>(csvFileName);
        // 디버그용 변수
        bool isCSVReadSuccessful = false;
        if (filePath != null)
        {
            {
                string[] lines = filePath.text.Split('\n'); // 줄 바꿈으로 행 구분을 위해 추가

                // lines의 길이가 1 이상일 경우
                if (lines.Length > 0)
                {
                    string[] headers = lines[0].Split(DELIMITER); // 문자열을 ',' 기준으로 자름

                    foreach (string header in headers)
                    {
                        dataDictionary.Add(header, new List<string>()); // dataDictionary에 행 이름을 키 값으로 리스트 추가
                    }

                    // 첫번째 행[0]을 헤더로 사용하고 두 번째[1] 부터 데이터 행으로 사용하기 위해
                    // index를 1 부터 시작
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i];
                        string[] values = line.Split(DELIMITER);

                        for (int j = 0; j < values.Length; j++) 
                        {
                            // 헤더 리스트에 값 추가
                            dataDictionary[headers[j]].Add(values[j]);
                        }
                    }
                }
            }

            isCSVReadSuccessful = true;
        }

        Debug.Log($"CSV 파일 찾기 : {isCSVReadSuccessful}");
        return dataDictionary;
    }

    // 변환된 csv 파일의 정보가 저장된 딕셔너리 내부의 값을 출력하는 함수.
    // 매개변수로 csv 파일에서 변환된 딕셔너리를 넣어야 한다.
    // "행:열1,열2,열3"과 같이 출력된다.
    // 매개 변수로 받을 딕셔너리의 구조는 <string, List<string>> 이어야 한다.
    public void PrintData(Dictionary<string, List<string>> dictionary)
    {
        // 딕셔너리의 각 항목을 출력
        foreach (KeyValuePair<string, List<string>> entry in dictionary)
        {
            string category = entry.Key;
            List<string> values = entry.Value;

            Debug.Log(category + ": " + string.Join(", ", values));
        }
    }

}