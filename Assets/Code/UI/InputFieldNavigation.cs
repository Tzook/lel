using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputFieldNavigation : MonoBehaviour
{

    private EventSystem system;

    void Start ()
    {
        system = EventSystem.current;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && system.currentSelectedGameObject != null)
        {
            bool isShiftPressed = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
            Selectable current = system.currentSelectedGameObject.GetComponent<Selectable>();
            Selectable next = (isShiftPressed ? current.FindSelectableOnUp() : current.FindSelectableOnDown());
            InputField nextAsInput = next as InputField;

            // next is null - return back as far as you can
            if (nextAsInput == null)
            {
                Selectable prev = (isShiftPressed ? current.FindSelectableOnDown() : current.FindSelectableOnUp());
                InputField prevAsInput = prev as InputField;
                if (prevAsInput == null)
                {
                    // both next AND prev are not input fields - do nothing
                    return;
                }

                while (prevAsInput != null)
                {
                    current = prev;
                    prev = (isShiftPressed ? current.FindSelectableOnDown() : current.FindSelectableOnUp());
                    prevAsInput = prev as InputField;
                }
                next = current;
                nextAsInput = current as InputField;
            }
            Debug.Log("Selecting next: " + nextAsInput.ToString());
            next.Select();
        }
    }
}