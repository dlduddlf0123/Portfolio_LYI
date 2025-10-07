using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class GugudanManager : MonoBehaviour
{
    DialogManager dialogMgr;
    AudioSource m_audio;

    public DeviceObserverSettings deviceObserver;

    public TextAsset csv_gugudanHeaders;

    public GameObject ARGugudan;
    public GameObject[] arr_cubeHeaders;
    public List<List<object>> list__gugudanHeaders = new List<List<object>>(); //불러올 대가리들 배열

   // public GugudanObject[][] gugudanObjects = new GugudanObject[9][];
    public GugudanObject currentGugudan;


    public Dictionary<int, AudioClip> dic_numSpeech = new Dictionary<int, AudioClip>();
    public AudioClip eun;
    public AudioClip nun;

    public Coroutine currentCoroutine = null;

    void Awake()
    {
        dialogMgr = GetComponent<DialogManager>();
        m_audio = GetComponent<AudioSource>();

        //for (int firstNum = 0; firstNum < 9; firstNum++)
        //{
        //    GugudanObject[] arr_dan = new GugudanObject[9];

        //    for (int secondNum = 0; secondNum < 9; secondNum++)
        //    {
        //        arr_dan[secondNum] = transform.GetChild(firstNum).GetChild(secondNum).GetChild(0).GetComponent<GugudanObject>();
        //        arr_dan[secondNum].SetGugudan(firstNum, secondNum);
        //    }

        //    gugudanObjects[firstNum] = arr_dan;
        //}

        deviceObserver.ToggleDeviceObserver(false);

        LoadGugudanData();
        LoadNumberSpeechAudioClip();
        //CreateGugudanHeaders();
    }

    //public void CreateGugudanHeaders()
    //{
    //    for (int dan = 0; dan < gugudanObjects.Length; dan++)
    //    {
    //        for (int lastNumber = 0; lastNumber < gugudanObjects[dan].Length; lastNumber++)
    //        {
    //            int headerNum = System.Convert.ToInt32(list__gugudanHeaders[dan+1][lastNumber+1]);
    //            GameObject header = Instantiate(arr_cubeHeaders[headerNum]);
    //            header.transform.SetParent(gugudanObjects[dan][lastNumber].transform);
    //            header.transform.localPosition = Vector3.zero;
    //            gugudanObjects[dan][lastNumber].cubeHeader = header.GetComponent<CubeCharacter>();
    //        }
    //    }
    //}

    public void LoadGugudanData()
    {
        list__gugudanHeaders = dialogMgr.ReadDialogRemovedDatas(csv_gugudanHeaders);
    }

    public void LoadNumberSpeechAudioClip()
    {
        for (int num = 1; num < 11; num++)
        {
            dic_numSpeech.Add(num, Resources.Load<AudioClip>("AudioClip/"+num));
        }
    }

    public void ReadText()
    {
        if (currentGugudan.currentCoroutine != null)
        {
            StopCoroutine(currentGugudan.currentCoroutine);
            currentGugudan.currentCoroutine = null;
        }
        currentGugudan.currentCoroutine = StartCoroutine(TextReadAction());
    }

    public void ReadQuestionText()
    {
        if (currentGugudan.currentCoroutine != null)
        {
            StopCoroutine(currentGugudan.currentCoroutine);
            currentGugudan.currentCoroutine = null;
        }
        currentGugudan.currentCoroutine = StartCoroutine(QuestionTextReadAction());
    }
    public void ReadResultText()
    {
        if (currentGugudan.currentCoroutine != null)
        {
            StopCoroutine(currentGugudan.currentCoroutine);
            currentGugudan.currentCoroutine = null;
        }
        currentGugudan.currentCoroutine = StartCoroutine(ResultTextReadAction());
    }


    IEnumerator TextReadAction()
    {
        m_audio.PlayOneShot(dic_numSpeech[currentGugudan.firstNum]);

        yield return new WaitForSeconds(dic_numSpeech[currentGugudan.firstNum].length);

        m_audio.PlayOneShot(dic_numSpeech[currentGugudan.secondNum]);

        yield return new WaitForSeconds(dic_numSpeech[currentGugudan.secondNum].length * 0.5f);

        switch (currentGugudan.secondNum)
        {
            case 1:
            case 3:
            case 6:
            case 7:
            case 8:
                m_audio.PlayOneShot(eun);
                yield return new WaitForSeconds(eun.length);
                break;
            case 2:
            case 4:
            case 5:
            case 9:
                m_audio.PlayOneShot(nun);
                yield return new WaitForSeconds(nun.length);
                break;
            default:
                m_audio.PlayOneShot(eun);
                yield return new WaitForSeconds(eun.length);
                break;
        }


        if (currentGugudan.resultNum <= 10)
        {
            m_audio.PlayOneShot(dic_numSpeech[currentGugudan.resultNum]);
            yield return new WaitForSeconds(dic_numSpeech[currentGugudan.resultNum].length);
        }
        else
        {
            int resultFirst = currentGugudan.resultNum / 10;
            int resultLast = currentGugudan.resultNum % 10;

            if (resultFirst < 2)
            {
                m_audio.PlayOneShot(dic_numSpeech[10]);
                yield return new WaitForSeconds(dic_numSpeech[10].length);
                m_audio.PlayOneShot(dic_numSpeech[resultLast]);
                yield return new WaitForSeconds(dic_numSpeech[resultLast].length);
            }
            else
            {
                m_audio.clip = dic_numSpeech[resultFirst];
                //m_audio.pitch = 1.1f;
                m_audio.Play();
                yield return new WaitForSeconds(dic_numSpeech[resultFirst].length * 0.6f);
                m_audio.clip = dic_numSpeech[10];
                m_audio.Play();
                yield return new WaitForSeconds(dic_numSpeech[10].length * 0.6f);

                if (resultLast != 0)
                {
                    m_audio.clip = dic_numSpeech[resultLast];
                    m_audio.Play();
                    yield return new WaitForSeconds(dic_numSpeech[resultLast].length);
                }
            }

        }

        yield return new WaitForSeconds(1f);
        currentGugudan.NextGugudan();
    }

    IEnumerator QuestionTextReadAction()
    {
        m_audio.PlayOneShot(dic_numSpeech[currentGugudan.firstNum]);

        yield return new WaitForSeconds(dic_numSpeech[currentGugudan.firstNum].length);

        m_audio.PlayOneShot(dic_numSpeech[currentGugudan.secondNum]);

        yield return new WaitForSeconds(dic_numSpeech[currentGugudan.secondNum].length * 0.5f);

        switch (currentGugudan.secondNum)
        {
            case 1:
            case 3:
            case 6:
            case 7:
            case 8:
                m_audio.PlayOneShot(eun);
                yield return new WaitForSeconds(eun.length);
                break;
            case 2:
            case 4:
            case 5:
            case 9:
                m_audio.PlayOneShot(nun);
                yield return new WaitForSeconds(nun.length);
                break;
            default:
                m_audio.PlayOneShot(eun);
                yield return new WaitForSeconds(eun.length);
                break;
        }
    }


    IEnumerator ResultTextReadAction()
    {
        if (currentGugudan.resultNum <= 10)
        {
            m_audio.PlayOneShot(dic_numSpeech[currentGugudan.resultNum]);
            yield return new WaitForSeconds(dic_numSpeech[currentGugudan.resultNum].length);
        }
        else
        {
            int resultFirst = currentGugudan.resultNum / 10;
            int resultLast = currentGugudan.resultNum % 10;

            if (resultFirst < 2)
            {
                m_audio.PlayOneShot(dic_numSpeech[10]);
                yield return new WaitForSeconds(dic_numSpeech[10].length);
                m_audio.PlayOneShot(dic_numSpeech[resultLast]);
                yield return new WaitForSeconds(dic_numSpeech[resultLast].length);
            }
            else
            {
                m_audio.clip = dic_numSpeech[resultFirst];
                //m_audio.pitch = 1.1f;
                m_audio.Play();
                yield return new WaitForSeconds(dic_numSpeech[resultFirst].length * 0.6f);
                m_audio.clip = dic_numSpeech[10];
                m_audio.Play();
                yield return new WaitForSeconds(dic_numSpeech[10].length * 0.6f);

                if (resultLast != 0)
                {
                    m_audio.clip = dic_numSpeech[resultLast];
                    m_audio.Play();
                    yield return new WaitForSeconds(dic_numSpeech[resultLast].length);
                }
            }
        }
    }

}
