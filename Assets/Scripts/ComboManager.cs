using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboManager : MonoBehaviour
{
    private Animator animator; // Reference to Animator for playing animations

    [SerializeField] private TMP_Text comboCount; // UI text showing the numeric combo count
    [SerializeField] private TMP_Text comboText;  // UI text showing the descriptive combo comment

    [SerializeField] List<string> comments; // List of comments/descriptions for combo stages

    private void Awake()
    {
        animator = GetComponent<Animator>(); // Cache Animator component
    }

    // Updates the combo display and plays the combo animation
    public void StartCounter(int count)
    {
        animator.StopPlayback(); // Ensure no other animations are playing

        // If count is within the range of available comments
        if (count < comments.Count)
        {
            comboCount.text = count.ToString();
            comboText.text = comments[count];
        }
        else
        {
            // If count exceeds available comments, use the last comment
            comboCount.text = count.ToString();
            comboText.text = comments[comments.Count - 1].ToString();
        }

        animator.Play("comboHit"); // Play the combo animation
    }
}
