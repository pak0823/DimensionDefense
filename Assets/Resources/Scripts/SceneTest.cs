using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTest : MonoBehaviour
{

    public void OnBtnSceneChange()
    {
        Shared.SceneFlowManager.ChangeScene("SampleScene");
    }
    
}
