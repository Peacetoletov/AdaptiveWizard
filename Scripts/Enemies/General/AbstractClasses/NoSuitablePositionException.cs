using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaptiveWizard.Assets.Scripts.Enemies.General.AbstractClasses
{
    public class NoSuitablePositionException : Exception
    {
        public NoSuitablePositionException(string message) : base(message) {
            // expand this if necessary
        }
    }
}