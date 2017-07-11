using UnityEngine;
using System.Collections;
using System.Collections.Generic;




namespace Skill.Framework.Managers
{
    public partial class UpgradeItem : Skill.Framework.IO.ISavable
    {
        public bool IsUpgraded { get; private set; }
        public bool IsCompleted { get; private set; }

        public UpgradeItem(System.DateTime startTime, float duration, int repeatCount = 1)
        {
            this.StartTime.FromSystem(startTime);
            this.Duration = duration;
            this.RepeatCount = repeatCount;

        }


        public void Check(System.DateTime currentTime)
        {
            if (_RepeatCount < 1) _RepeatCount = 1;
            IsUpgraded = false;
            if (UpgradeCount >= RepeatCount) return;
            System.TimeSpan delta = currentTime - _StartTime.ToSystem();
            if (delta.TotalSeconds > (Duration * (UpgradeCount + 1)))
            {
                IsUpgraded = true;
                UpgradeCount++;
                if (UpgradeCount >= RepeatCount) IsCompleted = true;
            }
            else
                IsUpgraded = false;
        }
    }
}
