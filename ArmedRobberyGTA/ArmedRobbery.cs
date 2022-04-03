using CitizenFX.Core;
using FivePD.API;
using FivePD.API.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ArmedRobberyGTA
{
    [CalloutProperties("Armed Robbery (PSB Heist)", "DevKilo", "2.0")]
    public class ArmedRobbery : Callout
    {
        Random rnd = new();
        Ped suspect, suspect2, robber, robber2;

        readonly Vector3[] coords = new Vector3[]
        {
            new Vector3(247.05f,218.79f,106.29f) // Pacific Standard Bank
        };
        public ArmedRobbery()
        {
            InitInfo(coords[rnd.Next(coords.Length)]);
            ShortName = "Armed Robbery In Progress";
            CalloutDescription = "There is an active bank heist at the Pacific Standard Bank in Los Santos! Rescue the hostages and catch these baddies! Caution is advised. [~rDEV NOTE~s~: F6 = Negotiations Menu]";
            ResponseCode = 3;
            StartDistance = 120f;
        }
        public override async Task OnAccept()
        {
            InitBlip();

        }
        public override void OnStart(Ped closest)
        {
            base.OnStart(closest);

        }
        public override void OnCancelBefore()
        {
            base.OnCancelBefore();
        }
    }
}