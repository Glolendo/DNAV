using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button testButton;

    void Start()
    {
        if (testButton != null)
            testButton.onClick.AddListener(() => Debug.Log("Button pressed!"));
    }
}
