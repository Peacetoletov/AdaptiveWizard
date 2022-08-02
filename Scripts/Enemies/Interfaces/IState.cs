using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdaptiveWizard.Assets.Scripts.Enemies.Interfaces
{
    public interface IState
    {
        int OnEnter();
        int Update();
        int OnLeave();
    }
}