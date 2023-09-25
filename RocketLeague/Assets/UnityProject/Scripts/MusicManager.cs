using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class MusicManager : MonoBehaviour
{
    AudioSource musicSource;
    public AudioClip[] musicClip;
    public Sprite[] musicImages;
    public Image albumCover;
    public TMP_Text songName;
    int lastIdx;
    

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
        musicSource=GetComponent<AudioSource>();
        musicSource.clip=musicClip[0];
        albumCover.sprite=musicImages[0];
        songName.text=musicClip[0].name;
        musicSource.Play();
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

           while(rand==lastIdx)
            {
                rand = Random.Range(0, musicClip.Length);

            }
           lastIdx = rand;
            musicSource.clip=musicClip[rand];
            albumCover.sprite=musicImages[rand];
            songName.text=musicClip[rand].name;

            musicSource.Play();
        }
        else if(!musicSource.isPlaying)
        {
            int rand = Random.Range(0, musicClip.Length);

            while (rand==lastIdx)
            {
                rand = Random.Range(0, musicClip.Length);

            }
            lastIdx = rand;
            musicSource.clip=musicClip[rand];
            albumCover.sprite=musicImages[rand];
            songName.text=musicClip[rand].name;

            musicSource.Play();
        }
    }

    void ShuffleIdx(AudioClip a, AudioClip b)
    {
        AudioClip temp = a;
        a = b;
        b = temp;
    }
}
