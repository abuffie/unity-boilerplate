/// <summary>
/// Script:  ConfimButton.cs
/// Created: 6/13/2022 10:37:00 AM
/// Author:  AaronBuffie
/// 
/// A simple button extention that requires the button to be held for a set duration before calling the 'On Click ()' action
/// Ideal use would be for any button that results in a unrevesable action removing the need for a second confirmation
/// </summary>
namespace Aarware.Core {

    using UnityEngine;
    using UnityEngine.UI;
    using UnityEngine.EventSystems;
    using System.Collections;
    using System;


    public class ConfirmButton : Button, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler{
        

        [SerializeField] Image      progressImage;
        [SerializeField] Gradient   progressColors;
        [SerializeField] GameObject enabledEffect; // some effect feedback to enable

        [SerializeField, Range(0.2f,2f)]    float duration      = 1.25f;        
        [SerializeField]                    float reEnableWait  = 3f;
        [SerializeField]                    bool  onRelease     = true;
        [SerializeField]                    bool  disable       = true;    
        [SerializeField]                    bool  reEnable      = true;
        
        
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
            if(disable){
                interactable=false;
            }       
            actionReady=true; 
            if(!onRelease){
                RunAction();
            }
        }

        void RunAction(){
            if(actionReady){
                onClick?.Invoke();
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
        

        public void Test(){
            Debug.Log("TEST");
        }
    
    }
}