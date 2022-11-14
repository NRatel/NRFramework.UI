// https://github.com/NRatel/NRFramework.UI

using System;
using TMPro;

namespace NRFramework
{
    public abstract partial class UIView
    {
        static public event Action<TMP_Dropdown, int> onTMPDropdownValueChangedGlobalEvent;

        protected void BindEvent(TMP_Dropdown tmpDropdown)
        {
            tmpDropdown.onValueChanged.AddListener((value) =>
            {
                onTMPDropdownValueChangedGlobalEvent?.Invoke(tmpDropdown, value);
                OnValueChanged(tmpDropdown, value);
            });
        }

        protected void BindEvent(TMP_InputField tmpInputField)
        {
            tmpInputField.onValueChanged.AddListener((value) => { OnValueChanged(tmpInputField, value); });
        }

        protected void UnbindEvent(TMP_Dropdown tmpDropdown)
        {
            tmpDropdown.onValueChanged.RemoveAllListeners();
        }

        protected void UnbindEvent(TMP_InputField tmpInputField)
        {
            tmpInputField.onValueChanged.RemoveAllListeners();
        }

        protected virtual void OnValueChanged(TMP_Dropdown dropdown, int value) { }

        protected virtual void OnValueChanged(TMP_InputField inputField, string value) { }
    }
}