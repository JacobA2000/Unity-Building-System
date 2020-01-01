using UnityEngine;
using UnityEngine.UI;

//This class needs to be attached to our Button Prefab and then setup there. 
//it will avoid us having to call .GetComponent a million times when initializing!
public class MenuButton : MonoBehaviour
{
    public RectTransform Recttransform;
    public Image BackgroundImage;
    public Image IconImage;
    public RectTransform IconRecttransform;
}
