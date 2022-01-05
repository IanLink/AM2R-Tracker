using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public struct ItemDataSingle
{
    public string title;
    public RectTransform rectTransform;
    public Image itemImage;
}

[System.Serializable]
public struct ItemDataMultiple
{
    public string title;
    public short quantity, maxQuantity;
    public RectTransform rectTransform;
    public Image itemImage;
    public Text text;
    
}

public class TrackerScript : MonoBehaviour
{
    [SerializeField]
    private Image background;
    [SerializeField]
    private Image singleFlashImage;
    [SerializeField]
    private Image multipleFlashImage;
    [SerializeField]
    private RectTransform selector;
    [SerializeField]
    private RectTransform quantitator;
    [SerializeField]
    private ItemDataSingle[] singleItems; 
    [SerializeField]
    private ItemDataMultiple[] multipleItems;
    [SerializeField]
    private sbyte singleItemsRowLength;
    [SerializeField]
    private sbyte multipleItemsRowLength;
    [SerializeField]
    private Vector2 deadzone;
    [SerializeField]
    private Color deactiveColor;
    [SerializeField]
    private Color clearColor;
    [SerializeField]
    private Color halfWhiteColor;

    private float clickTime = 0f;
    private sbyte singlePosition = 3;
    private sbyte multiplePosition = 1;
    private bool holdingJoystickX = false;
    private bool holdingJoystickY = false;
    private bool holdingDpadX = false;
    private bool holdingDpadY = false;

    //TO DO
    //Create method to change resolution and layout
    //Create click function
    //Cursor reset while inactive
    //Text 2 character format

    // Update is called once per frame
    void Update()
    {
        //Single Item
        //Horizontal Joystick
        if (Mathf.Abs(Input.GetAxisRaw("Mouse X")) >= deadzone.x && !holdingJoystickX)
        {
            sbyte targetPosition = singlePosition;
            targetPosition += (sbyte)Mathf.Sign(Input.GetAxisRaw("Mouse X"));//Doesn't wants to work in a single line
            SetSelectorSingle(targetPosition, true);           
        }
        if (Mathf.Abs(Input.GetAxisRaw("Mouse X")) < deadzone.x && holdingJoystickX)
        {
            holdingJoystickX = false;
        }

        //Vertical Joystick
        if (Mathf.Abs(Input.GetAxisRaw("Mouse Y")) >= deadzone.y && !holdingJoystickY)
        {
            sbyte targetPosition = singlePosition;
            targetPosition += (sbyte)(Mathf.Sign(Input.GetAxisRaw("Mouse Y")) * singleItemsRowLength);//Doesn't wants to work in a single line
            SetSelectorSingle(targetPosition, false);
        }
        if (Mathf.Abs(Input.GetAxisRaw("Mouse Y")) < deadzone.y && holdingJoystickY)
        {
            holdingJoystickY = false;
        }

        //Circle Button
        if (Input.GetButtonDown("R3"))
        {
            SelectItem();
        }

        //Flash image color lerp
        if (singleFlashImage.color != clearColor)
        {
            singleFlashImage.color = Color.Lerp(singleFlashImage.color, clearColor, Time.deltaTime * 1.5f);
        }
        //Multiple flash image color lerp
        if (multipleFlashImage.color != clearColor)
        {
            multipleFlashImage.color = Color.Lerp(multipleFlashImage.color, clearColor, Time.deltaTime * 2f);
        }

        //Multiple Item
        if (Mathf.Abs(Input.GetAxisRaw("Dpad Horizontal")) >= deadzone.x && !holdingDpadX)
        {
            
            sbyte targetPosition = multiplePosition;
            targetPosition += (sbyte)Mathf.Sign(Input.GetAxisRaw("Dpad Horizontal"));//Doesn't wants to work in a single line
            SetSelectorMultiple(targetPosition);
            //Debug.Log("Switching quantitator to:" + targetPosition);
        }
        if (Mathf.Abs(Input.GetAxisRaw("Dpad Horizontal")) < deadzone.x && holdingDpadX)
        {
            holdingDpadX = false;
        }

        //Vertical Joystick
        if (Mathf.Abs(Input.GetAxisRaw("Dpad Vertical")) >= deadzone.y && !holdingDpadY)
        {
            SetItemQuantity((short)Mathf.Sign(Input.GetAxisRaw("Dpad Vertical")));
        }
        if (Mathf.Abs(Input.GetAxisRaw("Dpad Vertical")) < deadzone.y && holdingDpadY)
        {
            holdingDpadY = false;
        }

    }

    public void SetSelectorSingle(sbyte position, bool horizontal)
    {
        if (position >= 0 && position < singleItems.Length)
        {
            singlePosition = position;
            CopyRectTransform(selector, singleItems[singlePosition].rectTransform);
            singleFlashImage.color = clearColor;
            if (horizontal)
            {
                holdingJoystickX = true;
            }
            else
            {
                holdingJoystickY = true;
            }
        }
    }
    
    public void SetSelectorMultiple(sbyte position)
    {
        if (position >= 0 && position < multipleItems.Length)
        {
            multiplePosition = position;
            CopyRectTransform(quantitator, multipleItems[multiplePosition].rectTransform);
            multipleFlashImage.color = clearColor;
            holdingDpadX = true;
        }
    }

    public void CopyRectTransform(RectTransform rect1, RectTransform rect2)
    {
        rect1.anchorMin = rect2.anchorMin;
        rect1.anchorMax = rect2.anchorMax;
        rect1.offsetMin = rect2.offsetMin;
        rect1.offsetMax = rect2.offsetMax;
    }

    public void SelectItem()
    {
        if (singleFlashImage.color != Color.white && singleFlashImage.color != clearColor)
        {
            singleFlashImage.color = clearColor;
        }
        if (singleItems[singlePosition].itemImage.color == deactiveColor)//If inactive
        {
            singleFlashImage.color = Color.white;
            singleItems[singlePosition].itemImage.color = Color.white;
        }
        else if (singleItems[singlePosition].itemImage.color == Color.white)//If active
        {
            singleItems[singlePosition].itemImage.color = deactiveColor;
        }
    }

    public void SetItemQuantity(short value)
    {
        short maxTargetValue = multipleItems[multiplePosition].maxQuantity;
        short targetValue = multipleItems[multiplePosition].quantity;
        targetValue += value;//Still doesn't work otherwise
        if (targetValue >= 0 && targetValue <= maxTargetValue)
        {
            multipleItems[multiplePosition].quantity = targetValue;
            multipleItems[multiplePosition].text.text = string.Format("{0:00}/{1:00}", targetValue, maxTargetValue);
            multipleItems[multiplePosition].itemImage.color = Color.HSVToRGB(0f, 0f, 0.3f + (0.7f * targetValue / maxTargetValue));
            if (value >= 0)
            {
                multipleFlashImage.color = halfWhiteColor;
            }
        }
        holdingDpadY = true;
    }

    public void DefaultRes()
    {
        if (DoubleClick())
        {
            Screen.SetResolution(532, 228, false);
        }
    }

    public void Reset()
    {
        if (DoubleClick())
        {
            SceneManager.LoadScene(0);
        }
    }

    public bool DoubleClick()
    {
        if (clickTime == 0f || Time.time - clickTime > 3f)
        {
            clickTime = Time.time;
            return false;
        }
        else
        {
            return true;
        }
    }

}
