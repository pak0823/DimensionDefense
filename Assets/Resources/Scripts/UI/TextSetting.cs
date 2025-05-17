using UnityEngine;
using UnityEngine.UI;

public class TextSetting : MonoBehaviour
{
    public Text NORMALTEXT;
    public Text SPECIALTEXT;

    private void Awake()
    {
        Shared.TextSetting = this;
    }

    public void SetTextAlpha(int _cost)
    {
        Color normalColor = NORMALTEXT.color;
        Color specialColor = SPECIALTEXT.color;

        if (_cost >= 10 && _cost < 30)
            normalColor.a = 1f;
        else if( _cost >= 30)
            specialColor.a = 1f;
        else
        {
            normalColor.a = 0.2f;
            specialColor.a = 0.2f;
        }

        NORMALTEXT.color = normalColor;
        SPECIALTEXT.color = specialColor;

    }


}
