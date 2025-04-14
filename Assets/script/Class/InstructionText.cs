using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent((typeof(TextMeshProUGUI)))]
public class InstructionText : MonoBehaviour
{
    public TextMeshProUGUI instructionText;
    public int lengthOfPixelX = 20;
    public float maxWidth = 800f;
    // Start is called before the first frame update'

    public void setContent(string _content)
    {
        if (this.instructionText != null)
        {
            this.instructionText.text = _content;

            // Calculate the desired width based on the content length
            float desiredWidth = _content.Length * this.lengthOfPixelX;

            // Clamp the desired width to the maximum width
            float finalWidth = Mathf.Min(desiredWidth, maxWidth);

            // Update the RectTransform size
            RectTransform rectTransform = this.instructionText.GetComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(finalWidth, rectTransform.sizeDelta.y);

            // Force the TextMeshProUGUI to update its layout
            this.instructionText.ForceMeshUpdate();
        }
    }
}
