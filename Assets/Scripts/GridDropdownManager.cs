using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;

public class GridDropdownManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("The dropdown for selecting the number of rows.")]
    public TMP_Dropdown rowsDropdown;

    [Tooltip("The dropdown for selecting the number of columns.")]
    public TMP_Dropdown columnsDropdown;

    // All possible options for rows/columns
    private readonly List<int> allNumberOptions = new List<int> { 2, 3, 4, 5, 6, 7 };
    // Only even numbers (used when needed)
    private readonly List<int> evenNumberOptions = new List<int> { 2, 4, 6 };
    private bool isUpdatingDropdowns = false; // Prevents recursive updates

    void Start()
    {
        // Ensure dropdown references are assigned
        if (rowsDropdown == null || columnsDropdown == null)
        {
            Debug.LogError("GridDropdownManager: Dropdown references are not set in the Inspector!", this);
            return;
        }

        // Fill both dropdowns with initial options
        PopulateDropdown(rowsDropdown, allNumberOptions);
        PopulateDropdown(columnsDropdown, allNumberOptions);

        // Add listeners for when a value changes in either dropdown
        rowsDropdown.onValueChanged.AddListener(delegate { OnSourceDropdownChanged(rowsDropdown, columnsDropdown); });
        columnsDropdown.onValueChanged.AddListener(delegate { OnSourceDropdownChanged(columnsDropdown, rowsDropdown); });
    }

    // Populates a dropdown with a list of integer options
    private void PopulateDropdown(TMP_Dropdown dropdown, List<int> options)
    {
        List<string> optionsAsStrings = options.Select(n => n.ToString()).ToList();
        dropdown.ClearOptions();
        dropdown.AddOptions(optionsAsStrings);
    }

    // Handles logic when one dropdown changes, updating the other
    private void OnSourceDropdownChanged(TMP_Dropdown source, TMP_Dropdown target)
    {
        if (isUpdatingDropdowns) return; // Prevents infinite loop when updating both dropdowns

        // Get currently selected value from the source dropdown
        int selectedSourceValue = int.Parse(source.options[source.value].text);

        // If source is odd → restrict target to even options, otherwise allow all options
        List<int> newTargetOptions = (selectedSourceValue % 2 != 0) ? evenNumberOptions : allNumberOptions;

        // Remember what was selected in the target before refresh
        string previouslySelectedTargetText = target.options[target.value].text;

        isUpdatingDropdowns = true;

        // Repopulate the target dropdown with new options
        PopulateDropdown(target, newTargetOptions);

        // Try to restore the previous target selection if it still exists
        int newIndex = target.options.FindIndex(option => option.text == previouslySelectedTargetText);

        if (newIndex != -1)
        {
            target.value = newIndex; // Restore old selection
        }
        else
        {
            target.value = 0; // Fallback to first option
        }

        target.RefreshShownValue(); // Update UI display

        isUpdatingDropdowns = false;
    }
}
