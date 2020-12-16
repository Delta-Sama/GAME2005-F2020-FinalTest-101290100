using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonManagerScript : MonoBehaviour
{
    public Button Button;
    public string scene1;
    void Start()
    {
        Button btn = Button.GetComponent<Button>();
        btn.onClick.AddListener(TaskOnClick);
    }
    void TaskOnClick()
    {
        SceneManager.LoadScene(scene1);
    }
}