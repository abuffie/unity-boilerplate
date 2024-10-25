/// <summary>
/// Script:  ConfimButton.cs
/// Created: 6/13/2022 10:37:00 AM
/// Author:  AaronBuffie
/// 
/// A simple button extention that requires the button to be held for a set duration before calling the 'On Click ()' action
/// Ideal use would be for any button that results in a unrevesable action removing the need for a second confirmation
/// </summary>
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

namespace Aarware.Core{
public class ConfirmButton : Button, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler{
    

    [SerializeField] Image      progressImage;
    [SerializeField] Gradient   progressColors;
    [SerializeField,Tooltip("Some effect feedback to enable on confirm")] GameObject enabledEffect;

    [SerializeField, Range(0.2f,3f), Tooltip("How many seconds you have to hold the button")]   float duration      = 1.5f;        
    [SerializeField, Tooltip("How many seconds before the button re-enables")]                  float reEnableWait  = 3f;
    [SerializeField, Tooltip("Trigger click event on release or immediately after duration")]   bool  onRelease     = true;
    [SerializeField, Tooltip("Disable button after confirm")]                                   bool  disable       = true;    
    [SerializeField, Tooltip("Button re-enables after confirm")]                                bool  reEnable      = true;
    
    
    Coroutine countDownRoutine;
    Coroutine reEnableRoutine;
    bool actionReady=false;

    protected override void Awake() {
        ClearCountDown();
        base.Awake();
    }    

    public override void OnPointerDown(PointerEventData eventData)  => RunCountDown();
    public override void OnPointerUp(PointerEventData eventData)    => RunAction();
    public override void OnSubmit(BaseEventData eventData)          => RunCountDown();
    public override void OnPointerExit(PointerEventData eventData)  => ClearCountDown();
    public override void OnPointerClick(PointerEventData eventData) => ClearCountDown();


    void RunCountDown(){
        if(!interactable){return;}
        ClearCountDown();
        if(reEnableRoutine!=null)   { StopCoroutine(reEnableRoutine);  }
        countDownRoutine = StartCoroutine(CountDown());
    }
    
    void ClearCountDown(){
        if(countDownRoutine!=null)  { StopCoroutine(countDownRoutine); }
        progressImage.fillAmount=0f;
        actionReady=false;
    }

    void RunReEnable(){        
        if(reEnable){ reEnableRoutine=StartCoroutine(Renable());  }
    }


    IEnumerator CountDown(){
        float t=0f;
        while(progressImage.fillAmount<1){
            t+=Time.deltaTime;
            float p = t/duration;
            progressImage.color = progressColors.Evaluate(p);
            progressImage.fillAmount = Mathf.Lerp(0f, 1f, p);
            yield return null;
        }
                
        actionReady=true; 
        if(!onRelease){
            RunAction();
        }
    }

    void RunAction(){
        if(actionReady){
            onClick?.Invoke();
            if(disable){interactable=false;}
            if(enabledEffect!=null){enabledEffect.SetActive(true);}     
            ClearCountDown();
            RunReEnable();
        }
    }

    IEnumerator Renable(){
        yield return new WaitForSeconds(reEnableWait);
        interactable=true;
        if(enabledEffect!=null){enabledEffect.SetActive(false);} // make sure its disabled  
    }

}
}