using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableARwithDelay : MonoBehaviour
{

    public GameObject[] Ambient_Targets;
    public float Spatial_audio_delay = 15.0f;

    public string[] Audio_paths;
    private bool flag;



    // Start is called before the first frame update
    void Start()
    {
        /*
        Audio_paths = new string[5];
        Audio_paths[0] = "Restaurant";
        Audio_paths[1] = "Cars moving";
        Audio_paths[2] = "Kitchen-cutlery";
        Audio_paths[3] = "Chopping-meat";
        Audio_paths[4] = null;
        */
        flag = true;
        
        for(int i = 0; i < Ambient_Targets.Length; i++)
        {
            Transform[] ts = Ambient_Targets[i].transform.GetComponentsInChildren<Transform>(true);
            foreach(Transform t in ts)
            {
                if(t.gameObject.name == "SFX Loop Close" || t.gameObject.name == "SFX Loop Middle" || t.gameObject.name == "SFX Loop Far")
                {
                    t.gameObject.GetComponent<AudioSource>().mute = true;
                    Debug.Log("Child " + i + " is " + t.gameObject.name + " " + t.gameObject.activeSelf);
                }
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (flag)
        {
            StartCoroutine(WaitforIntro(Spatial_audio_delay));
            flag = false;
        }
        
    }

    IEnumerator WaitforIntro(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        for (int i = 0; i < Ambient_Targets.Length; i++)
        {
            Transform[] ts = Ambient_Targets[i].transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "SFX Loop Close" || t.gameObject.name == "SFX Loop Middle" || t.gameObject.name == "SFX Loop Far")
                {
                    /*
                    AudioClip clip = Resources.Load<AudioClip>(Audio_paths[i]);
                    t.gameObject.GetComponent<AudioSource>().clip = clip;
                    t.gameObject.GetComponent<AudioSource>().Play();
                    */
                    t.gameObject.GetComponent<AudioSource>().mute = false;
                    Debug.Log("Deactivated Child " + i + " is " + t.gameObject.name + " " + t.gameObject.activeSelf);
                }
            }
        }
    }
}
