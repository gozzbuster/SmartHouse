using App1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace App1
{
    class SwitchToggleCheck
    {
        ToggleSwitch[] toggleSwitch = new ToggleSwitch[6];
        public ToggleSwitch this[int index]
        {
            get
            {
                return toggleSwitch[index];
            }
        }
        public void AddElement(ToggleSwitch toggleSwitch)
        {
            for (int i = 0; i < this.toggleSwitch.Length; i++)
            {
                
            }

        }

    }
}
