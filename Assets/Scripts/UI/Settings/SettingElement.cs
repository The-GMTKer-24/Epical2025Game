using System;
using Factory_Elements.Settings;
using UnityEngine;

namespace UI.Inventory
{
    public interface ISettingElement
    {
        public void SetSetting(ISetting setting);
    }
}