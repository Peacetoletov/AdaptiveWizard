using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaptiveWizard.Assets.Scripts.Enemies.General.Interfaces
{
    public interface IState
    {
        int OnEnter();
        int Update();
        int OnLeave();
    }
}