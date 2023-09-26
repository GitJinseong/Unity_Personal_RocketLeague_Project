using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MusicManager : MonoBehaviour
{

    public static MusicManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<MusicManager>();
            }
            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    public CanvasGroup musicUiObj;
    public static MusicManager m_instance;   // 싱글톤이 할당될 static 변수
    AudioSource musicSource;
    public AudioClip[] musicClip;
    public Sprite[] musicImages;
    public Image albumCover;
    public TMP_Text songName;
    int lastIdx;
    bool musicUiFade = false;
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance != this)
        {
            Destroy(gameObject); // 현재 인스턴스가 싱글톤 인스턴스가 아닌 경우 파괴
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i<musicClip.Length; i++)
        {
            int randomIdx1 = Random.Range(0, musicClip.Length);
            int randomIdx2 = Random.Range(0, musicClip.Length);
            ShuffleIdx(musicClip[randomIdx1], musicClip[randomIdx2]);
        }
        int randomsong=Random.Range(0, musicClip.Length);
        musicSource=GetComponent<AudioSource>();
        musicSource.clip=musicClip[randomsong];
        albumCover.sprite=musicImages[randomsong];
        songName.text=musicClip[randomsong].name;
        musicSource.Play();
            StartCoroutine(FadeRoutine());

    }

    // Update is called once per frame
    void Update()
    {
        //if(ChatManager.instance!=null)
        //{

        //if(ChatManager.instance.chatOpen)
        //{
        //    return;
        //}
        //}
        if(Input.GetKeyDown(KeyCode.N))
        {
            int rand=Random.Range(0, musicClip.Length);
            musicUiFade=false;
           while (rand==lastIdx)
            {
                rand = Random.Range(0, musicClip.Length);

            }
           lastIdx = rand;
            musicSource.clip=musicClip[rand];
            albumCover.sprite=musicImages[rand];
            songName.text=musicClip[rand].name;

            musicSource.Play();
            musicUiObj.alpha=1;
            StopAllCoroutines();
            StartCoroutine(FadeRoutine());
        }
        else if(!musicSource.isPlaying)
        {
            int rand = Random.Range(0, musicClip.Length);
            musicUiFade=false;

            while (rand==lastIdx)
            {
                rand = Random.Range(0, musicClip.Length);

            }
            lastIdx = rand;
            musicSource.clip=musicClip[rand];
            albumCover.sprite=musicImages[rand];
            songName.text=musicClip[rand].name;

            musicSource.Play();
            musicUiObj.alpha=1;
            StopAllCoroutines();

            StartCoroutine(FadeRoutine());
        }

        if(musicUiFade)
        {
            musicUiObj.alpha=Mathf.Lerp(musicUiObj.alpha, 0,Time.deltaTime*10);
        }
       
    }
    

    IEnumerator FadeRoutine()
    {
        yield return new WaitForSeconds(2);
        musicUiFade=true;
        yield return new WaitForSeconds(1); 
        musicUiFade=false;
    }
    void ShuffleIdx(AudioClip a, AudioClip b)
    {
        AudioClip temp = a;
        a = b;
        b = temp;
    }
}
