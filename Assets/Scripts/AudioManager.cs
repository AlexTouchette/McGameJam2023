using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource desert;
    public AudioSource waves;

    private bool wavesIsPlaying;
    private bool desertIsPlaying;

    private TileManager m_Tm;

    // Start is called before the first frame update
    void Start()
    {
        wavesIsPlaying = false;
        desertIsPlaying = true;
        
        m_Tm = GameObject.Find("Grid").GetComponent<TileManager>();
    }

    public void CheckWaterDistance()
    {
        if ((m_Tm.CurrentPosition.x <= 10 || m_Tm.CurrentPosition.x >= 89 || m_Tm.CurrentPosition.y <= 10 
             || m_Tm.CurrentPosition.y >= 89) && !wavesIsPlaying)
        {
            desert.Stop();
            waves.Play();
            wavesIsPlaying = true;
            desertIsPlaying = false;
        }
        else if(m_Tm.CurrentPosition.x > 10 && m_Tm.CurrentPosition.x < 89 && m_Tm.CurrentPosition.y > 10 
                && m_Tm.CurrentPosition.y < 89 && !desertIsPlaying)
        {
            desert.Play();
            waves.Stop();
            wavesIsPlaying = false;
            desertIsPlaying = true;
        }
    }
}
