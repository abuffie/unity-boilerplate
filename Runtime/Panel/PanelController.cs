/// <summary>
/// Script:  PanelController.cs
/// Created: 10/12/2022 4:05:31 PM
/// Author:  abuffie
/// </summary>

using System.Collections;
using System;
using UnityEngine;
namespace Aarware.Core{
[RequireComponent(typeof(CanvasGroup))]
public class PanelController : MonoBehaviour{
    [SerializeField]                    bool        instant     = true;
    [SerializeField]                    bool        hideOnStart = true;
    [SerializeField, Range(.5f, 10f)]   float       speed       = 5f;
    [SerializeField]                    CanvasGroup group;

    public delegate void PanelControllerEvent();       
    public event PanelControllerEvent   OnOpenBegin;
    public event PanelControllerEvent   OnOpenComplete;        
    public event PanelControllerEvent   OnCloseBegin;
    public event PanelControllerEvent   OnCloseComplete;

    public bool Closed => group.alpha<=0f;
    public bool Locked => !group.interactable;

    void Awake() {
        if(group==null){
            group = GetComponent<CanvasGroup>();
        }
    }
    void Start() {
        if(hideOnStart){
            Close(true);
        }else{
            Open(true);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="instant"></param>
    public void Open(Action<Action> CustomFade, Action CallBack=null){OnOpenBegin?.Invoke();CustomFade.Invoke(()=>{
            group.interactable=true;
            CallBack?.Invoke();
            OnOpenComplete?.Invoke();
    }); }
    public void Open(bool instant=false) => Open(instant, null);
    public void Open(bool _instant, Action CallBack=null){        
        gameObject.SetActive(true); 
        StopAllCoroutines();
        if(instant || _instant){
            SetInstant(true);
            CallBack?.Invoke();
            OnOpenComplete?.Invoke();
            return;
        }
        OnOpenBegin?.Invoke();
        StartCoroutine(Fade(true, ()=>{
            group.interactable=true;
            CallBack?.Invoke();
            OnOpenComplete?.Invoke();
        }));
    }

    public void Close(bool instant=false) => Close(instant, null);
    public void Close(bool instant, Action CallBack=null){
        gameObject.SetActive(true); 
        StopAllCoroutines();
        if(instant){
            SetInstant(false);
            CallBack?.Invoke();
            OnCloseComplete?.Invoke();
            return;
        }
        OnCloseBegin?.Invoke();
        StartCoroutine(Fade(false, ()=>{
            group.interactable      = false;
            group.blocksRaycasts    = false;
            gameObject.SetActive(false); 
            CallBack?.Invoke();
            OnCloseComplete?.Invoke();
        }));
    }

    public void Lock(bool locked=true) => group.interactable=locked;
    
    IEnumerator Fade(bool fadeIn, Action internalComplete=null){
        // set up for fade actions
        int dir                 = fadeIn ? 1 : -1;
        float target            = fadeIn ? 1f : 0f;
        group.interactable      = false;
        group.blocksRaycasts    = true;

        // run the fade
        while(group.alpha != target){
            group.alpha = Mathf.Clamp( (group.alpha + (Time.deltaTime*speed)*dir), 0f, 1f);
            yield return null;
        }
        // internal complete actions
        internalComplete?.Invoke();
    }

    void SetInstant(bool state){
        group.alpha             = state?1:0;
        group.blocksRaycasts    = state;
        group.interactable      = state;
        gameObject.SetActive(state); 
    }

    void OnValidate() {
        if(group==null){
            group = GetComponent<CanvasGroup>();
        }
    }
}
}