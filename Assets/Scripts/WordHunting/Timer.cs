using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] private Slider TimerSlider;
    private float time = 0f;

    // Start is called before the first frame update
    void Start()
    {
        TimerSlider  = GetComponent<Slider>();
        TimerSlider.value = TimerSlider.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if(WHGameManager.Instance.GetGameState() == Define.game_state.Start)
        {
            time += Time.deltaTime;
            TimerSlider.value = TimerSlider.maxValue - time;

            if (TimerSlider.value <= 0)
            {
                WHGameManager.Instance.GameEnd();
            }
        }
    }
}
