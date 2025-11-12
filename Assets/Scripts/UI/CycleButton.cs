using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CycleButton : MonoBehaviour {

    [Header("Text")]
    public TextMeshProUGUI optionText;

    [Header("Options")]
    public List<string> options = new List<string>();
    public int selectedOption;

    private void Update() {

        optionText.text = options[selectedOption];

    }

    public void OnClick() {

        selectedOption++;

        if (selectedOption > options.Count - 1)
            selectedOption = 0;
        else if (selectedOption < 0)
            selectedOption = options.Count - 1;

    }

    public void ClearOptions() {

        options.Clear();

    }

    public void AddOptions(string newOption) {

        options.Add(newOption);

    }

}
