using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CountDownText : MonoBehaviour
{
    private int countDown;
    private TextMeshProUGUI _countDownText;
    private Animator _countDownAnimator;
    private static readonly int Countdown = Animator.StringToHash("Countdown");
    private static readonly int GO = Animator.StringToHash("GO");

    private void Start()
    {
        _countDownText = GetComponent<TextMeshProUGUI>();
        _countDownAnimator = GetComponent<Animator>();
        
    }

    public void StartCountDown(int timeCount)
    {
        
        countDown = timeCount;
        StartCoroutine(CountDownCoroutine());
    }



    private IEnumerator CountDownCoroutine()
    {
        _countDownAnimator.enabled = true;   
        while (countDown>0)
        {
            _countDownText.text = countDown.ToString();
            _countDownAnimator.SetTrigger(Countdown);
            yield return new WaitForSeconds(1);
            countDown--;
        }
        
        _countDownText.text = "GO!";
        _countDownAnimator.SetTrigger(GO);
        _countDownAnimator.enabled = true;
        if(!LocalPlayerPointer.Instance.roomPlayer.isSpectator)
            LocalPlayerPointer.Instance.gamePlayerGameObject.GetComponent<CarController>().ActivateCar(0);
    }
}
