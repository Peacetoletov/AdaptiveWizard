using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AdaptiveWizard.Assets.Scripts.Items.Passive.Classes.Abstract
{
    public abstract class PassiveItem
    {
        private static int staticID = 0;
        private int ID;     // ID will be used for deleting items

        public PassiveItem() {
            // set ID and increment staticID
            //Debug.Log("setting passive item ID");
            this.ID = ++staticID;
        }

        // TODO: uncomment and implement these in the distant future
        //public abstract string Description();
        //public abstract string FlavorText();
    }
}
