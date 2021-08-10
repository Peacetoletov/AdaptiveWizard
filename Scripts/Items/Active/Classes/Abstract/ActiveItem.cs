using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Items {
    public abstract class ActiveItem
    {
        // possibly do something here in the future, otherwise turn this into an interface
        public abstract void Use();
    }
}