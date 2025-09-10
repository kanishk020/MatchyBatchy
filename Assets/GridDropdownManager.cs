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

    private readonly List<int> allNumberOptions = new List<int> { 2, 3, 4, 5, 6, 7};
    private readonly List<int> evenNumberOptions = new List<int> { 2, 4, 6};
    private bool isUpdatingDropdowns = false;

    
    void Start()
    {

        if (rowsDropdown == null || columnsDropdown == null)
        {
            Debug.LogError("GridDropdownManager: Dropdown references are not set in the Inspector!", this);
            return;
        }

        PopulateDropdown(rowsDropdown, allNumberOptions);
        PopulateDropdown(columnsDropdown, allNumberOptions);

        rowsDropdown.onValueChanged.AddListener(delegate { OnSourceDropdownChanged(rowsDropdown, columnsDropdown); });
        columnsDropdown.onValueChanged.AddListener(delegate { OnSourceDropdownChanged(columnsDropdown, rowsDropdown); });
    }

    private void PopulateDropdown(TMP_Dropdown dropdown, List<int> options)
    {
        List<string> optionsAsStrings = options.Select(n => n.ToString()).ToList();
        dropdown.ClearOptions();
        dropdown.AddOptions(optionsAsStrings);
    }

    private void OnSourceDropdownChanged(TMP_Dropdown source, TMP_Dropdown target)
    {
        if (isUpdatingDropdowns) return;

        int selectedSourceValue = int.Parse(source.options[source.value].text);

        List<int> newTargetOptions = (selectedSourceValue % 2 != 0) ? evenNumberOptions : allNumberOptions;

        string previouslySelectedTargetText = target.options[target.value].text;

        isUpdatingDropdowns = true;

        PopulateDropdown(target, newTargetOptions);

        int newIndex = target.options.FindIndex(option => option.text == previouslySelectedTargetText);

        if (newIndex != -1)
        {
            target.value = newIndex;
        }
        else
        {
            target.value = 0;
        }

        target.RefreshShownValue();

        isUpdatingDropdowns = false;
    }
}

