using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class UserInterface : MonoBehaviour
{
    [Header("GENERAL")]
    public Camera UICamera;
    public GameObject BackgroundPanel;
    public GameObject CircleMenuElementPrefab;
    public bool UseGradient;
    public UnityStandardAssets.Characters.FirstPerson.FirstPersonController Player;  //CODE FOR STANDARD ASSETS CONTROLLER REMOVE WHEN WE BUILD OUR OWN CONTROLLER.

    [Header("BUTTONS")]
    public Color NormalButtonColor;
    public Color HighlightedButtonColor;
    public Gradient HighlightedButtonGradient = new Gradient();

    [Header("INFORMAL CENTER")]
    public Image InformalCenterBackground;
    public Text ItemName;
    public Text ItemDescription;
    public Image ItemIcon;

    private int currentMenuItemIndex;
    private int previousMenuItemIndex;
    private float calculatedMenuIndex;
    private float currentSelectionAngle;
    private Vector3 currentMousePosition;
    private List<CircularMenuElement> menuElements = new List<CircularMenuElement>();

    private static UserInterface instance;
    public static UserInterface Instance {get { return instance; } }
    public bool Active { get { return BackgroundPanel.activeSelf; } }
    public List<CircularMenuElement> MenuElements 
    { 
        get { return menuElements; } 
        set { menuElements = value; } 
    }

    private void Awake()
    {
        //Firstly we need to set our instance so we have it easier to access our script without having to use methods like Find() and so on.
        instance = this;
    }

    //This function is being called once right at the start, it will populate our buttons for us depending on how many buildingcomponents we are setting
    //up for our building system
    public void Initialize()
    {
        
        float rotationalIncrementalValue = 360f / menuElements.Count;
        float currentRotationValue = 0;
        float fillPercentageValue = 1f / menuElements.Count;

        for (int i = 0; i < menuElements.Count; i++)
        {
            GameObject menuElementGameObject = Instantiate(CircleMenuElementPrefab);
            menuElementGameObject.name = i + ": " + currentRotationValue;
            menuElementGameObject.transform.SetParent(BackgroundPanel.transform);

            MenuButton menuButton = menuElementGameObject.GetComponent<MenuButton>();

            menuButton.Recttransform.localScale = Vector3.one;
            menuButton.Recttransform.localPosition = Vector3.zero;
            menuButton.Recttransform.rotation = Quaternion.Euler(0f, 0f, currentRotationValue);
            currentRotationValue += rotationalIncrementalValue;

            menuButton.BackgroundImage.fillAmount = fillPercentageValue + 0.001f;
            menuElements[i].ButtonBackground = menuButton.BackgroundImage;

            menuButton.IconImage.sprite = menuElements[i].ButtonIcon;
            menuButton.IconRecttransform.rotation = Quaternion.identity;
        }

        BackgroundPanel.SetActive(false);
    }

    private void Update()
    {
        if(!Active)
        {
            return;
        }

        GetCurrentMenuElement();
        if(Input.GetMouseButton(0))
        {
            Select();
        }
    }

    private void GetCurrentMenuElement()
    {
        float rotationalIncrementalValue = 360f / menuElements.Count;
        currentMousePosition = new Vector2(Input.mousePosition.x - Screen.width / 2, Input.mousePosition.y - Screen.height / 2);

        currentSelectionAngle = 90 + rotationalIncrementalValue + Mathf.Atan2(currentMousePosition.y, currentMousePosition.x) * Mathf.Rad2Deg;
        currentSelectionAngle = (currentSelectionAngle + 360f) % 360f;

        currentMenuItemIndex = (int)(currentSelectionAngle / rotationalIncrementalValue);

        if(currentMenuItemIndex != previousMenuItemIndex)
        {
            menuElements[previousMenuItemIndex].ButtonBackground.color = NormalButtonColor;

            previousMenuItemIndex = currentMenuItemIndex;

            menuElements[currentMenuItemIndex].ButtonBackground.color = UseGradient ? HighlightedButtonGradient.Evaluate(1f / menuElements.Count 
            * currentMenuItemIndex) : HighlightedButtonColor;
            InformalCenterBackground.color = UseGradient ? HighlightedButtonGradient.Evaluate(1f / menuElements.Count * currentMenuItemIndex) :
            HighlightedButtonColor;
            RefreshInformalCenter();
        }
    }

    private void RefreshInformalCenter()
    {
        ItemName.text = menuElements[currentMenuItemIndex].Name;
        ItemDescription.text = menuElements[currentMenuItemIndex].Description;
        ItemIcon.sprite = menuElements[currentMenuItemIndex].ButtonIcon;
    }

    private void Select()
    {
        BuildingSystem.Instance.SwitchToIndex(currentMenuItemIndex);
        Deactivate();
    }

    public void Activate()
    {
        if(Active)
        {
            return;
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Player.m_MouseLook.SetCursorLock(false); //CODE FOR STANDARD ASSETS CONTROLLER REMOVE WHEN WE BUILD OUR OWN CONTROLLER.
        Player.m_MouseLook.UpdateCursorLock(); //CODE FOR STANDARD ASSETS CONTROLLER REMOVE WHEN WE BUILD OUR OWN CONTROLLER.
        BackgroundPanel.SetActive(true);
        RefreshInformalCenter();
    }

    public void Deactivate()
    {
        BackgroundPanel.SetActive(false);
        Player.m_MouseLook.SetCursorLock(true); //CODE FOR STANDARD ASSETS CONTROLLER REMOVE WHEN WE BUILD OUR OWN CONTROLLER.
        Player.m_MouseLook.UpdateCursorLock(); //CODE FOR STANDARD ASSETS CONTROLLER REMOVE WHEN WE BUILD OUR OWN CONTROLLER.
    }
}
