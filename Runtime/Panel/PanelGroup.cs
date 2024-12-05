using System;
using UnityEngine;

/// <summary>
/// Script:  PanelGroup.cs
/// Created: 11/27/2024 3:50:38 PM
/// Author:  abuffie
/// </summary>
namespace Aarware.Core{
    public class PanelGroup : MonoBehaviour{

        [SerializeField] PanelController[] group;
        [SerializeField] int openIndex => Array.FindIndex(group,pc=>!pc.Closed);
        public void OpenNext(int dir_count=1){
            if (group == null || group.Length == 0){
                return;
            }
            int openAt = Mathf.Clamp(dir_count % group.Length, 0, group.Length - 1);

            
            if(openIndex>-1 && openIndex < group.Length){
                group[openIndex].Close();
            }
            
            group[openAt].Open();
        }

    }
}
