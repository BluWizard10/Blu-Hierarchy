using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDK3.Components;
using static BluWizard.Hierarchy.IconManager.IconManager;

namespace BluWizard.Hierarchy.CustomComponents
{
    public static class CustomComponents
    {
        static CustomComponents()
        {
            IconSystem.SetCustomIcon(typeof(Camera), Resources.Load<Texture2D>("Icons/Test-Icon"));
        }
    }
}
