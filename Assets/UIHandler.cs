using Unity​Engine.UIElements;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    // provide reference in editor inspector (from step 2)
    public UIDocument UIDocument;
    Button button;
    Label label;
    TextField textField;
    void Start()
    {
        var root = UIDocument.rootVisualElement;
        // get ui elements by name
        textField = root.Q<TextField>("SampleTextField");
        button = root.Q<Button>("SampleButton");
        label = root.Q<Label>("MyLabel");

        // add event handler
        button.clickable.clicked += Button_clicked;
    }

    private void Button_clicked()
    {
        label.text = textField.text;
        gameObject.SetActive(false);
    }
}