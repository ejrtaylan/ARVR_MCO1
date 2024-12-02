using System;
using Lean.Common;
using UnityEngine;

public class CustomBrickLogic : MonoBehaviour {
    void OnEnable() {
        Lean.Touch.LeanSelectableByFinger.OnAnySelected += HandleSelected;
        Lean.Touch.LeanSelectableByFinger.OnAnyDeselected += HandleDeselected;
    }

    void OnDisable() {
        Lean.Touch.LeanSelectableByFinger.OnAnySelected -= HandleSelected;
        Lean.Touch.LeanSelectableByFinger.OnAnyDeselected -= HandleDeselected;
    }

    private void HandleSelected(LeanSelect select, LeanSelectable selectable)
    {
        throw new NotImplementedException();
    }

    private void HandleDeselected(LeanSelect select, LeanSelectable selectable)
    {
        throw new NotImplementedException();
    }
}