using CitizenFX.Core;
using CitizenFX.Core.Native;
using FivePD.API;
using FivePD.API.Utils;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ArmedRobberyGTA
{
    [Guid("6965CF80-62E5-4AA2-B4A1-334356CED995")]
    [CalloutProperties("Armed Robbery (PSB Heist)", "DevKilo", "2.0")]
    public class ArmedRobbery : Callout
    {
        Random rnd = new();
        Ped suspect, suspect2, suspect3, suspect4, robber, hacker, hostage, hostage2, hostage3, hostage4;

        bool loopedThroughDB = false;
        bool breakDoor1DB = false;

        int totalPlayerChances = 3;
        int chancesLeft = 3;

        Vector3 suspectPosition = new(237.37f, 216.18f, 106.29f);
        float suspectHeading = 96.03f;

        Vector3 suspect2Position = new(262.43f, 208.81f, 106.28f);
        float suspect2Heading = 151.94f;

        Vector3 suspect3Position = new(252.71f, 217.55f, 106.29f);
        float suspect3Heading = 138.4f;

        Vector3 suspect4Position = new(241.69f, 220.21f, 106.29f);
        float suspect4Heading = 207.48f;

        Vector3 robberPosition = new(252.79f, 219.72f, 106.29f);
        float robberHeading = 150.17f;

        Vector3 hackerPosition = new(258.34f, 218.57f, 106.29f);
        float hackerHeading = 132.99f;

        Vector3 hostagePosition = new(245.75f, 214.96f, 106.29f);
        float hostageHeading = 337f;

        Vector3 hostage2Position = new(247.04f, 214.38f, 106.29f);

        Vector3 hostage3Position = new(249.1f, 213.58f, 106.29f);

        Vector3 hostage4Position = new(250.98f, 212.86f, 106.29f);

        public enum PedType
        {
            Robber,
            Civilian,
            Police
        }
        Vector3[] doorCoords =
{
                new(256.8f,220.37f,106.29f),
                new(261.97f,221.95f,106.28f),
                new(255.39f,224.66f,101.88f),
                new(251.92f,221.36f,101.68f),
                new(261f,214.55f,101.68f)
            };

        Vector4[] CashTrolleyCoords =
        {
            new(254.78f,216.59f,100.68f,0f),
            new(263.68f,215.69f,100.68f,0f),
            new(259.5f,217.32f,100.68f,0f)
        };
        List<Prop> doors = new List<Prop>();
        List<Entity> cashTrolleys = new List<Entity>();
        public ArmedRobbery()
        {
            InitInfo(new Vector3(247.05f, 218.79f, 106.29f));
            ShortName = "Armed Robbery In Progress";
            CalloutDescription = "There is an active bank heist at the Pacific Standard Bank in Los Santos! Rescue the hostages and catch these baddies! Caution is advised. [~rDEV NOTE~s~: F6 = Negotiations Menu]";
            ResponseCode = 3;
            StartDistance = 120f;
        }
        public override async Task OnAccept()
        {
            InitBlip();
            
            Debug.WriteLine("Test");
            Spawn();

        }
        private async Task Spawn()
        {
            await SpawnScene();
            SpawnTasks();
        }

        public async Task SpawnScene()
        {
            // Spawn suspect
            Debug.WriteLine("Before spawn suspect");
            Model suspectModel = new Model(GetRandomPed(PedType.Robber));
            suspect = await World.CreatePed(suspectModel, suspectPosition, suspectHeading);
            Debug.WriteLine("After spawn suspect");
            suspect.AlwaysKeepTask = true;
            suspect.BlockPermanentEvents = true;
            // Spawn suspect2
            Model suspect2Model = new(GetRandomPed(PedType.Robber));
            suspect2 = await World.CreatePed(suspect2Model, suspect2Position, suspect2Heading);
            suspect2.AlwaysKeepTask = true;
            suspect2.BlockPermanentEvents = true;
            // Spawn suspect3
            suspect3 = await World.CreatePed(new(GetRandomPed(PedType.Robber)),suspect3Position, suspect3Heading);
            suspect3.AlwaysKeepTask = true;
            suspect3.BlockPermanentEvents = true;
            // Spawn suspect4
            suspect4 = await World.CreatePed(new(GetRandomPed(PedType.Robber)),suspect4Position, suspect4Heading);
            suspect4.AlwaysKeepTask = true;
            suspect4.BlockPermanentEvents = true;
            // Spawn robber
            robber = await World.CreatePed(new(GetRandomPed(PedType.Robber)), robberPosition, robberHeading);
            robber.AlwaysKeepTask = true;
            robber.BlockPermanentEvents = true;
            // Spawn hacker
            hacker = await World.CreatePed(new(PedHash.Hacker), hackerPosition, hackerHeading);
            hacker.AlwaysKeepTask = true;
            hacker.BlockPermanentEvents = true;
           
            // Spawn hostage
            hostage = await World.CreatePed(new(RandomUtils.GetRandomPed()), hostagePosition, hostageHeading);
            hostage.AlwaysKeepTask = true;
            hostage.BlockPermanentEvents = true;
            // Spawn hostage2
            hostage2 = await World.CreatePed(new(RandomUtils.GetRandomPed()), hostage2Position, hostageHeading);
            hostage2.AlwaysKeepTask = true;
            hostage2.BlockPermanentEvents = true;
            // Spawn hostage3
            hostage3 = await World.CreatePed(new(RandomUtils.GetRandomPed()), hostage3Position, hostageHeading);
            hostage3.AlwaysKeepTask = true;
            hostage3.BlockPermanentEvents = true;
            // Spawn hostage4
            hostage4 = await World.CreatePed(new(RandomUtils.GetRandomPed()), hostage4Position, hostageHeading);
            hostage4.AlwaysKeepTask = true;
            hostage4.BlockPermanentEvents = true;

            // Spawn Objects
           
           foreach (Vector4 trolley in CashTrolleyCoords)
            {
                int numOfType = Function.Call<int>(Hash.GET_CLOSEST_OBJECT_OF_TYPE, trolley.X, trolley.Y, trolley.Z, 2f, Function.Call<uint>(Hash.GET_HASH_KEY, "ch_prop_cash_low_trolly_01c"), false, false, false);
                if (numOfType > 0)
                {
                    // Deal with it
                    Entity newTrolley = Entity.FromHandle(numOfType);
                    int index = Array.IndexOf(CashTrolleyCoords, trolley);
                    //CashTrolleyCoords[index] = new(newTrolley.Position.X,newTrolley.Position.Y,newTrolley.Position.Z)

                    cashTrolleys.Add(newTrolley);
                } else
                {
                    int newTrolley = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "ch_prop_cash_low_trolly_01c"), trolley.X, trolley.Y, trolley.Z, true, true, false);
                    Entity trolleyEntity = Entity.FromHandle(newTrolley);
                    
                    cashTrolleys.Add(trolleyEntity);
                    trolleyEntity.Heading = trolley.W;
                    Debug.WriteLine("Created trolley!");
                }
            }


        }

        public void SpawnTasks()
        {
            // Hostage Task
            //hostage.Task.HandsUp(-1);
            hostage.Task.HandsUp(-1);
            hostage2.Task.HandsUp(-1);
            hostage3.Task.HandsUp(-1);
            hostage4.Task.HandsUp(-1);

            // Suspect Task
            suspect4.Weapons.Give(WeaponHash.AssaultSMG, 255, true, true);
            suspect4.Task.AimAt(hostage, -1);
            suspect3.Weapons.Give(WeaponHash.SMG, 255, true, true);
            suspect3.Task.AimAt(hostage4, -1);
            suspect.Weapons.Give(WeaponHash.AssaultSMG, 255, true, true);
            suspect.Task.WanderAround(suspectPosition, 5f);
            suspect2.Weapons.Give(WeaponHash.SMG, 255, true, true);
            suspect2.Task.WanderAround(suspect2Position, 5f);

            // Suspect and Suspect2 require tasks still

            // Robber Task

            // Hacker Task
            BeginRobbery();
        }


        public override void OnStart(Ped closest)
        {
            base.OnStart(closest);
            Tick += LoopedThrough;
        }

        private async Task LockDoors()
        {
            Prop[] allProps = World.GetAllProps();

            for (int i = 0; i < doorCoords.Length; i++)
            {
                foreach (Prop prop in allProps)
                {
                    if (prop.Position.DistanceTo(doorCoords[i]) < 1f)
                    {
                        doors.Insert(i, prop);
                        Debug.WriteLine("Door " + i.ToString() + " Added!");
                        prop.IsPositionFrozen = true;
                    }
                }

            }
        }

        private async Task BeginRobbery()
        {
            await LockDoors();
            await BreakDoor1();
            await BreakDoor2();
            await OpenVault();
            await BreakDoor3();
            await BreakDoor4();
            await Rob();
        }

        private async Task Rob()
        {
            CashGrab(hacker, cashTrolleys[1]);
            await CashGrab(robber, cashTrolleys[0]);
            

            await CashGrab(robber, cashTrolleys[2]);
            
        }

        private async Task BreakDoor1()
        {
            Debug.WriteLine("BreakDoor1");
            int door1handle = doors[0].Handle;
            Entity door1 = Entity.FromHandle(door1handle);

            
            Vector3 door1Pos = new(256.73f,219.9f,106.29f);
            hacker.Task.RunTo(door1Pos);
            await WaitForPedAtPosition(hacker,door1Pos,0.5f);
            //hacker.Position = new(256.84f, 219.96f, 106.29f);
            hacker.Task.AchieveHeading(341.16f);
            await BaseScript.Delay(750);
            //hacker.Task.PlayAnimation("missfbi_s4mop", "plant_bomb_b");
            int bagscene = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, door1Pos.X, door1Pos.Y, door1Pos.Z, hacker.Rotation.X, hacker.Rotation.Y, hacker.Rotation.Z, 2, false, false, 1065353216, 0, 1.3);
            int bag = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_p_m_bag_var22_arm_s"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, false);
            Debug.WriteLine("After bag");
            Function.Call(Hash.SET_ENTITY_COLLISION, bag, false, true);
            Debug.WriteLine("After bag collision");
            Entity bagentity = Entity.FromHandle(bag);
            door1.SetNoCollision(bagentity, true);
            int thermite = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_prop_heist_thermite"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, true);
            Debug.WriteLine("After thermite");
            Entity thermiteEntity = Entity.FromHandle(thermite);
            door1.SetNoCollision(thermiteEntity,true);
            door1.SetNoCollision(hacker, true);
            Function.Call(Hash.SET_ENTITY_COLLISION, thermite, false, true);
            
            
            Debug.WriteLine("Before bone index");
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, thermite, hacker, Function.Call<int>(Hash.GET_PED_BONE_INDEX,hacker, 28422), 0, 0, 0, 0, 0, 200.0, true, true, false, true, 1, true);
            Debug.WriteLine("After bone index");
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, bagscene, "anim@heists@ornate_bank@thermal_charge", "thermal_charge", 1.5, -4.0, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, bagscene, "anim@heists@ornate_bank@thermal_charge", "bag_thermal_charge", 4.0, -8.0, 1);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, hacker, 5, 0, 0, 0);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, bagscene);
            await BaseScript.Delay(5000);
            thermiteEntity.Detach();
            thermiteEntity.IsPositionFrozen = true;
            bagentity.Delete();
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, bagscene);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, hacker, 5, 82, 0, 0);
            Vector3 awayFromExplosionPos = new(256.22f, 212.49f, 106.29f);
            thermiteEntity.PositionNoOffset = new(256.83f, 220.35f, hacker.Position.Z + 0.2f);
            hacker.Task.RunTo(awayFromExplosionPos);
            await BaseScript.Delay(5000);
            //World.AddExplosion(thermiteEntity.Position, ExplosionType.StickyBomb, 5f, 2f, hacker, true, false);
            Function.Call(Hash.ADD_EXPLOSION, thermiteEntity.Position.X, thermiteEntity.Position.Y, thermiteEntity.Position.Z, 2, 0.2f, true, false, 1f, true);
            //Function.Call(Hash.USE_PARTICLE_FX_ASSET, "wpn_flare");
            //int effect = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_AT_COORD, "proj_heist_flare_trail", thermiteEntity.Position.X,thermiteEntity.Position.Y,thermiteEntity.Position.Z, 0f, 0f, 0f, 5f, false, false, false, false);
            hacker.Task.ClearAll();
            thermiteEntity.Delete();
            door1.IsPositionFrozen = false;
            //Function.Call(Hash.REMOVE_PARTICLE_FX, effect,0);

            ////robber.Task.PlayAnimation("anim@heists@ornate_bank@thermal_charge", "thermal_charge");
            await BaseScript.Delay(5000);
            hacker.Task.ClearAll();
        }

        private async Task BreakDoor2()
        {
            Vector3 runToPos = new(261.95f,223.22f,106.28f);
            hacker.Task.RunTo(runToPos);
            await WaitForPedAtPosition(hacker, runToPos, 0.3f);
            hacker.Task.AchieveHeading(247.85f);
            await BaseScript.Delay(750);
            hacker.CanRagdoll = false;

            int bagscene = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, runToPos.X + 0.5f, runToPos.Y - 0.1f, runToPos.Z + 0.4f, hacker.Rotation.X, hacker.Rotation.Y, hacker.Rotation.Z, 2, false, false, 1065353216, 0, 1.3);
            int bag = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_p_m_bag_var22_arm_s"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, false);
            Debug.WriteLine("After bag");
            Function.Call(Hash.SET_ENTITY_COLLISION, bag, false, true);
            Debug.WriteLine("After bag collision");
            Entity bagentity = Entity.FromHandle(bag);

            int laptop = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_prop_hst_laptop"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, true);
            Debug.WriteLine("After thermite");
            Entity thermiteEntity = Entity.FromHandle(laptop);
            Function.Call(Hash.SET_ENTITY_COLLISION, laptop, false, true);

            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, bagscene, "anim@heists@ornate_bank@hack", "hack_enter", 1.5, -4.0, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, bagscene, "anim@heists@ornate_bank@hack", "hack_enter_bag", 4.0, -8.0, 1);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, laptop, bagscene, "anim@heists@ornate_bank@hack", "hack_enter_laptop", 4.0, -8.0, 1);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, hacker, 5, 0, 0, 0);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, bagscene);
            await BaseScript.Delay(6000);
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, bagscene);

            int hackscene = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, runToPos.X + 0.5f, runToPos.Y - 0.1f, runToPos.Z + 0.4f, hacker.Rotation.X, hacker.Rotation.Y, hacker.Rotation.Z, 2, false, true, 1065353216, 0, 1);
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, hackscene, "anim@heists@ornate_bank@hack", "hack_loop", 0, 0, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, hackscene, "anim@heists@ornate_bank@hack", "hack_loop_bag", 4.0, -8.0, 1);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, laptop, hackscene, "anim@heists@ornate_bank@hack", "hack_loop_laptop", 1.0, -0.0, 1);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, hackscene);
            await BaseScript.Delay(10000);
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, hackscene);

            int hackexit = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, runToPos.X + 0.5f, runToPos.Y - 0.1f, runToPos.Z + 0.4f, hacker.Rotation.X, hacker.Rotation.Y, hacker.Rotation.Z, 2, false, false, 1065353216, -1, 1.3);
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, hackexit, "anim@heists@ornate_bank@hack", "hack_exit", 0, 0, -1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, hackexit, "anim@heists@ornate_bank@hack", "hack_exit_bag", 4.0, -8.0, 1);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, laptop, hackexit, "anim@heists@ornate_bank@hack", "hack_exit_laptop", 4.0, -8.0, 1);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, hackexit);
            await BaseScript.Delay(6000);
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, hackexit);
            bagentity.Delete();
            Entity laptope = Entity.FromHandle(laptop);
            laptope.Delete();
            Entity vaultdoor = Entity.FromHandle(doors[1].Handle);
            vaultdoor.IsPositionFrozen = false;
            //Function.Call(Hash.ADD_EXPLOSION, 261.92f,221.77f,106.28f + 0.2f, 2, 0.5f, false, false, 0f, true);
            await BaseScript.Delay(750);
        }

        private async Task OpenVault()
        {
            Vector3 runToPos = new(253.33f,228.41f,101.68f);
            hacker.Task.RunTo(runToPos);
            await WaitForPedAtPosition(hacker, runToPos, 1f);
            hacker.Task.AchieveHeading(69.45f);
            await BaseScript.Delay(750);

            int bagscene = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, runToPos.X - 0.5f, runToPos.Y, runToPos.Z + 0.4f, hacker.Rotation.X, hacker.Rotation.Y, hacker.Rotation.Z, 2, false, false, 1065353216, 0, 1.3);
            int bag = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_p_m_bag_var22_arm_s"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, false);
            Debug.WriteLine("After bag");
            Function.Call(Hash.SET_ENTITY_COLLISION, bag, false, true);
            Debug.WriteLine("After bag collision");
            Entity bagentity = Entity.FromHandle(bag);
            
            int laptop = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_prop_hst_laptop"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, true);
            Debug.WriteLine("After thermite");
            Entity thermiteEntity = Entity.FromHandle(laptop);
            Function.Call(Hash.SET_ENTITY_COLLISION, laptop, false, true);

            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, bagscene, "anim@heists@ornate_bank@hack", "hack_enter", 1.5, -4.0, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, bagscene, "anim@heists@ornate_bank@hack", "hack_enter_bag", 4.0, -8.0, 1);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, laptop, bagscene, "anim@heists@ornate_bank@hack", "hack_enter_laptop", 4.0, -8.0, 1);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, hacker, 5, 0, 0, 0);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, bagscene);
            await BaseScript.Delay(6000);
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, bagscene);

            int hackscene = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, runToPos.X - 0.5f,runToPos.Y,runToPos.Z + 0.4f,hacker.Rotation.X,hacker.Rotation.Y,hacker.Rotation.Z,2,false,true, 1065353216,0,1);
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, hackscene, "anim@heists@ornate_bank@hack", "hack_loop", 0, 0, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, hackscene, "anim@heists@ornate_bank@hack", "hack_loop_bag", 4.0, -8.0, 1);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, laptop, hackscene, "anim@heists@ornate_bank@hack", "hack_loop_laptop", 1.0, -0.0, 1);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, hackscene);
            await BaseScript.Delay(10000);
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, hackscene);

            int hackexit = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, runToPos.X - 0.5f, runToPos.Y, runToPos.Z + 0.4f, hacker.Rotation.X, hacker.Rotation.Y, hacker.Rotation.Z, 2, false, false, 1065353216, -1, 1.3);
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, hackexit, "anim@heists@ornate_bank@hack", "hack_exit", 0, 0, -1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, hackexit, "anim@heists@ornate_bank@hack", "hack_exit_bag", 4.0, -8.0, 1);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, laptop, hackexit, "anim@heists@ornate_bank@hack", "hack_exit_laptop", 4.0, -8.0, 1);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, hackexit);
            await BaseScript.Delay(6000);
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, hackexit);
            bagentity.Delete();
            Entity laptope = Entity.FromHandle(laptop);
            laptope.Delete();
            Entity vaultdoor = Entity.FromHandle(doors[2].Handle);
            vaultdoor.IsPositionFrozen = false;
            Function.Call(Hash.ADD_EXPLOSION, 253.43f, 223.46f, 101.68f, 2, 0.5f, false, false, 0f, true);
            await BaseScript.Delay(750);
            
            vaultdoor.IsPositionFrozen = true;
        }

        private async Task BreakDoor3()
        {
            Debug.WriteLine("BreakDoor3");

            int door3Handle = doors[3].Handle;
            Entity door2 = Entity.FromHandle(door3Handle);
            Vector3 door3Pos = new(252.67f,221.39f,101.68f);

            hacker.Task.RunTo(door3Pos);
            await WaitForPedAtPosition(hacker, door3Pos, 0.5f);
            hacker.Task.AchieveHeading(160.11f);
            await BaseScript.Delay(750);
            int bagscene = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, door3Pos.X, door3Pos.Y, door3Pos.Z, hacker.Rotation.X, hacker.Rotation.Y, hacker.Rotation.Z, 2, false, false, 1065353216, 0, 1.3);
            int bag = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_p_m_bag_var22_arm_s"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, false);
            Debug.WriteLine("After bag");
            Function.Call(Hash.SET_ENTITY_COLLISION, bag, false, true);
            Debug.WriteLine("After bag collision");
            Entity bagentity = Entity.FromHandle(bag);
            door2.SetNoCollision(bagentity, true);
            int thermite = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_prop_heist_thermite"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, true);
            Debug.WriteLine("After thermite");
            Entity thermiteEntity = Entity.FromHandle(thermite);
            door2.SetNoCollision(thermiteEntity, true);
            door2.SetNoCollision(hacker, true);
            Function.Call(Hash.SET_ENTITY_COLLISION, thermite, false, true);

            Debug.WriteLine("Before bone index");
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, thermite, hacker, Function.Call<int>(Hash.GET_PED_BONE_INDEX, hacker, 28422), 0, 0, 0, 0, 0, 200.0, true, true, false, true, 1, true);
            Debug.WriteLine("After bone index");
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, bagscene, "anim@heists@ornate_bank@thermal_charge", "thermal_charge", 1.5, -4.0, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, bagscene, "anim@heists@ornate_bank@thermal_charge", "bag_thermal_charge", 4.0, -8.0, 1);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, hacker, 5, 0, 0, 0);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, bagscene);
            await BaseScript.Delay(5000);
            thermiteEntity.Detach();
            thermiteEntity.IsPositionFrozen = true;
            bagentity.Delete();
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, bagscene);
            Vector3 awayFromExplosionPos = new(255.05f,227.93f,101.68f);
            thermiteEntity.PositionNoOffset = new(252.54f,220.92f, hacker.Position.Z + 0.2f);
            hacker.Task.RunTo(awayFromExplosionPos);
            await BaseScript.Delay(5000);
            //World.AddExplosion(thermiteEntity.Position, ExplosionType.StickyBomb, 5f, 2f, hacker, true, false);
            Function.Call(Hash.ADD_EXPLOSION, thermiteEntity.Position.X, thermiteEntity.Position.Y, thermiteEntity.Position.Z, 2, 0.2f, true, false, 1f, true);
            //Function.Call(Hash.USE_PARTICLE_FX_ASSET, "wpn_flare");
            //int effect = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_AT_COORD, "proj_heist_flare_trail", thermiteEntity.Position.X,thermiteEntity.Position.Y,thermiteEntity.Position.Z, 0f, 0f, 0f, 5f, false, false, false, false);
            hacker.Task.ClearAll();
            thermiteEntity.Delete();
            door2.IsPositionFrozen = false;
            //Function.Call(Hash.REMOVE_PARTICLE_FX, effect,0);

            ////robber.Task.PlayAnimation("anim@heists@ornate_bank@thermal_charge", "thermal_charge");
            await BaseScript.Delay(5000);
            hacker.Task.ClearAll();
        }

        private async Task BreakDoor4()
        {
            Debug.WriteLine("BreakDoor4");

            int door4Handle = doors[4].Handle;
            Entity door3 = Entity.FromHandle(door4Handle);
            Vector3 door4Pos = new(261.05f,215.2f,101.68f);

            hacker.Task.RunTo(door4Pos);
            await WaitForPedAtPosition(hacker, door4Pos, 0.5f);
            hacker.Task.AchieveHeading(251.2f);
            await BaseScript.Delay(750);
            int bagscene = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, door4Pos.X, door4Pos.Y, door4Pos.Z, hacker.Rotation.X, hacker.Rotation.Y, hacker.Rotation.Z, 2, false, false, 1065353216, 0, 1.3);
            int bag = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_p_m_bag_var22_arm_s"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, false);
            Debug.WriteLine("After bag");
            Function.Call(Hash.SET_ENTITY_COLLISION, bag, false, true);
            Debug.WriteLine("After bag collision");
            Entity bagentity = Entity.FromHandle(bag);
            door3.SetNoCollision(bagentity, true);
            int thermite = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_prop_heist_thermite"), hacker.Position.X, hacker.Position.Y, hacker.Position.Z + 0.2, true, true, true);
            Debug.WriteLine("After thermite");
            Entity thermiteEntity = Entity.FromHandle(thermite);
            door3.SetNoCollision(thermiteEntity, true);
            door3.SetNoCollision(hacker, true);
            Function.Call(Hash.SET_ENTITY_COLLISION, thermite, false, true);

            Debug.WriteLine("Before bone index");
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, thermite, hacker, Function.Call<int>(Hash.GET_PED_BONE_INDEX, hacker, 28422), 0, 0, 0, 0, 0, 200.0, true, true, false, true, 1, true);
            Debug.WriteLine("After bone index");
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, hacker, bagscene, "anim@heists@ornate_bank@thermal_charge", "thermal_charge", 1.5, -4.0, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, bagscene, "anim@heists@ornate_bank@thermal_charge", "bag_thermal_charge", 4.0, -8.0, 1);
            Function.Call(Hash.SET_PED_COMPONENT_VARIATION, hacker, 5, 0, 0, 0);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, bagscene);
            await BaseScript.Delay(5000);
            thermiteEntity.Detach();
            thermiteEntity.IsPositionFrozen = true;
            bagentity.Delete();
            Function.Call(Hash.NETWORK_STOP_SYNCHRONISED_SCENE, bagscene);
            Vector3 awayFromExplosionPos = new(255.36f,218.05f,101.68f);
            thermiteEntity.PositionNoOffset = new(261.4f, 215.04f, hacker.Position.Z + 0.2f);
            hacker.Task.RunTo(awayFromExplosionPos);
            await BaseScript.Delay(5000);
            //World.AddExplosion(thermiteEntity.Position, ExplosionType.StickyBomb, 5f, 2f, hacker, true, false);
            Function.Call(Hash.ADD_EXPLOSION, thermiteEntity.Position.X, thermiteEntity.Position.Y, thermiteEntity.Position.Z, 2, 0.2f, true, false, 1f, true);
            //Function.Call(Hash.USE_PARTICLE_FX_ASSET, "wpn_flare");
            //int effect = Function.Call<int>(Hash.START_PARTICLE_FX_LOOPED_AT_COORD, "proj_heist_flare_trail", thermiteEntity.Position.X,thermiteEntity.Position.Y,thermiteEntity.Position.Z, 0f, 0f, 0f, 5f, false, false, false, false);
            hacker.Task.ClearAll();
            thermiteEntity.Delete();
            door3.IsPositionFrozen = false;
            //Function.Call(Hash.REMOVE_PARTICLE_FX, effect,0);

            ////robber.Task.PlayAnimation("anim@heists@ornate_bank@thermal_charge", "thermal_charge");
            await BaseScript.Delay(5000);
            hacker.Task.ClearAll();
        }

        private async Task CashGrab(Ped ped, Entity trolley)
        {
            Debug.WriteLine("CashGrab");
            float rot = Function.Call<float>(Hash.GET_ENTITY_HEADING, trolley);
            Vector3 targetRotation = new(0f, 0f, rot);
            Vector3 animPos = Function.Call<Vector3>(Hash.GET_ANIM_INITIAL_OFFSET_POSITION, "anim@heists@ornate_bank@grab_cash", "intro", trolley.Position.X, trolley.Position.Y, trolley.Position.Z, targetRotation, 0, 2);
            Debug.WriteLine("running to trolley" + trolley.Position.ToString());
            //Function.Call(Hash.TASK_GO_STRAIGHT_TO_COORD, ped, animPos, 0.025f, 5000, rot, 0.05f);
            //await WaitForPedAtPosition(ped, animPos, 0.2f);
            Function.Call(Hash.REQUEST_MODEL, Function.Call<uint>(Hash.GET_HASH_KEY, "ch_p_m_bag_var02_arm_s"));
            ped.Task.RunTo(animPos);
            await WaitForPedAtPosition(ped, animPos, 0.3f);
            ped.Task.AchieveHeading(rot);
            await BaseScript.Delay(750);
            Debug.WriteLine("Continuing");
            while (!Function.Call<bool>(Hash.HAS_MODEL_LOADED,Function.Call<uint>(Hash.GET_HASH_KEY, "ch_p_m_bag_var02_arm_s")))
            {
                await BaseScript.Delay(100);
            }
            int bag = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "ch_p_m_bag_var02_arm_s"), ped.Position.X, ped.Position.Y, ped.Position.Z + 0.2, true, true, false);
            int scene = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, trolley.Position.X, trolley.Position.Y, trolley.Position.Z, trolley.Rotation.X, trolley.Rotation.Y, trolley.Rotation.Z, 2, false, false, 1065353216, 0f, 1.3f);
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, ped, scene, "anim@heists@ornate_bank@grab_cash", "intro", 1.5f, -4f, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, scene, "anim@heists@ornate_bank@grab_cash", "bag_intro", 4.0f, -8.0f, 1);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, scene);
            await BaseScript.Delay(1500);
            CashAppear(ped);
            int scene2 = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, trolley.Position.X, trolley.Position.Y, trolley.Position.Z, trolley.Rotation.X, trolley.Rotation.Y, trolley.Rotation.Z, 2, true, true, 1065353216, 0f, 1.3f);
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, ped, scene2, "anim@heists@ornate_bank@grab_cash", "grab", 1.5f, -4f, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, scene2, "anim@heists@ornate_bank@grab_cash", "bag_grab", 4.0f, -8.0f, 1);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, trolley, scene2, "anim@heists@ornate_bank@grab_cash", "cart_cash_dissapear", 4.0f, -8.0f, 1);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, scene2);
            await BaseScript.Delay(37000);
            int scene3 = Function.Call<int>(Hash.NETWORK_CREATE_SYNCHRONISED_SCENE, trolley.Position.X, trolley.Position.Y, trolley.Position.Z, trolley.Rotation.X, trolley.Rotation.Y, trolley.Rotation.Z, 2, false, false, 1065353216, 0f, 1.3f);
            Function.Call(Hash.NETWORK_ADD_PED_TO_SYNCHRONISED_SCENE, ped, scene3, "anim@heists@ornate_bank@grab_cash", "exit", 1.5f, -4f, 1, 16, 1148846080, 0);
            Function.Call(Hash.NETWORK_ADD_ENTITY_TO_SYNCHRONISED_SCENE, bag, scene3, "anim@heists@ornate_bank@grab_cash", "bag_exit", 4f, -8f, 1);
            Function.Call(Hash.NETWORK_START_SYNCHRONISED_SCENE, scene3);
            await BaseScript.Delay(1800);
            Entity.FromHandle(bag).Delete();
        }


        private async Task CashAppear(Ped ped)
        {
            int grabcash = Function.Call<int>(Hash.CREATE_OBJECT, Function.Call<uint>(Hash.GET_HASH_KEY, "hei_prop_heist_cash_pile"), ped.Position.X, ped.Position.Y, ped.Position.Z, true);
            Entity grabobj = Entity.FromHandle(grabcash);
            grabobj.IsPositionFrozen = true;
            grabobj.IsInvincible = true;
            grabobj.SetNoCollision(ped, true);
            grabobj.IsVisible = false;
            Function.Call(Hash.ATTACH_ENTITY_TO_ENTITY, grabobj, ped, Function.Call<int>(Hash.GET_PED_BONE_INDEX, ped, 60309), 0f, 0f, 0f, 0f, 0f, 0f, false, false, false, false, 0, true);

            int startedGrabbing = Function.Call<int>(Hash.GET_GAME_TIMER);
            Tick += async () =>
            {
                
                if (Function.Call<int>(Hash.GET_GAME_TIMER) - startedGrabbing < 37000)
                {
                    await BaseScript.Delay(1);
                    if (Function.Call<bool>(Hash.HAS_ANIM_EVENT_FIRED, ped, Function.Call<uint>(Hash.GET_HASH_KEY, "CASH_APPEAR")))
                    {
                        if (!grabobj.IsVisible)
                        {
                            grabobj.IsVisible = true;
                        }
                    }

                    if (Function.Call<bool>(Hash.HAS_ANIM_EVENT_FIRED, ped, Function.Call<uint>(Hash.GET_HASH_KEY, "RELEASE_CASH_DESTROY")))
                    {
                        if (grabobj.IsVisible)
                        {
                            grabobj.IsVisible = false;
                        }
                    }
                }
                grabobj.Delete();
            };

            
        }


        private async Task WaitForPedAtPosition(Ped ped, Vector3 position, float buffer)
        {
            bool found = false;
            bool db = false;
            while (!found)
            {
                if (!db)
                {
                    db = true;
                    found = await IsPedAtPosition(ped,position,buffer);
                    if (found) return;
                    db = false;
                }

                await BaseScript.Delay(500);
            }
        }

        private async Task<bool> IsPedAtPosition(Ped ped, Vector3 position, float buffer)
        {
            await BaseScript.Delay(0);
            if (ped.Position.DistanceTo(position) < buffer)
            {
                return true;
            } else
            {
                return false;
            }
        }

        

        private async Task BreakDoor1Broken()
        {
            try
            {
                Debug.WriteLine("BreakDoor1");
                Vector3 door1Pos = new(256.6f, 219.8f, 106.29f);
                TaskSequence taskSequence = new TaskSequence();
                taskSequence.AddTask.RunTo(door1Pos);
                taskSequence.AddTask.AchieveHeading(341.16f);
                taskSequence.AddTask.PlayAnimation("missfbi_s4mop", "plant_bomb_a");
                hacker.Task.PerformSequence(taskSequence);
                
                Debug.WriteLine(hacker.TaskSequenceProgress.ToString());
            } catch (Exception ex)
            {
                Debug.WriteLine("Error!");
                Debug.WriteLine(ex.Message);
            }
        }


        public async Task LoopedThrough()
        {
            if (loopedThroughDB) return;
            foreach (Ped player in AssignedPlayers)
            {
                if (loopedThroughDB) return;

                if (chancesLeft <= 0)
                {
                    loopedThroughDB = true;
                    await KillAHostage();
                }

                if (player.Position.DistanceTo(suspectPosition) < 5f)
                {
                    loopedThroughDB = true;
                    await WarnPlayer(suspect,player,suspectPosition);
                }
                if (player.Position.DistanceTo(suspect2Position) < 8f)
                {
                    loopedThroughDB = true;
                    await WarnPlayer(suspect2,player,suspect2Position);
                }
            }
        }

        public async Task WarnPlayer(Ped ped, Ped player, Vector3 pedposition)
        {
            Location = player.Position;
            chancesLeft--;
            ShowDialog("~r~Suspect~f~: Yo! Back it up or I'm gonna ~r~shoot~f~!", 5000, 50f);
            Task<bool> task = RunTo(ped, pedposition);

            await task.ContinueWith(async t =>
            {
                Debug.WriteLine("Yeah!!");
                ped.Task.AimAt(player, 10000);
                await BaseScript.Delay(10000);
                loopedThroughDB = false;
            });
            
            
        }

        public async Task KillAHostage()
        {
            List<Ped> list = new List<Ped>();
            if (!hostage.IsDead)
                list.Add(hostage);
            if (!hostage2.IsDead) list.Add(hostage2);
            if (!hostage3.IsDead) list.Add(hostage3);
            if (!hostage4.IsDead) list.Add(hostage4);

            Ped chosenPed = list[new Random().Next(list.Count)];
            suspect3.Task.ShootAt(chosenPed, 1000, FiringPattern.SingleShot);
            
            chancesLeft = 1;
            loopedThroughDB = false;
            await BaseScript.Delay(3000);
            if (!chosenPed.IsDead)
            {
                chosenPed.Task.HandsUp(-1);
                suspect3.Task.AimAt(list[list.Count - 1], -1);
            }
        }

        public async Task<bool> RunTo(Ped ped, Vector3 position)
        {
            ped.Task.ClearAllImmediately();
            ped.Task.RunTo(position);
            
            while (true)
            {
                if (ped.Position.DistanceTo(position) < 2f)
                {
                    return true;
                }
                await BaseScript.Delay(200);
            }

        }

        public override void OnCancelBefore()
        {
            base.OnCancelBefore();
            suspect?.Delete();
            suspect2?.Delete();
            suspect3?.Delete();
            suspect4?.Delete();
            hostage?.Delete();
            hostage2?.Delete();
            hostage3?.Delete();
            hostage4?.Delete();
            robber?.Delete();
            hacker?.Delete();
            foreach (Prop door in doors)
            {
                Entity doore = Entity.FromHandle(door.Handle);
                doore.IsPositionFrozen = false;
            }
            foreach (Entity cashTrolley in cashTrolleys)
            {
                cashTrolley?.Delete();
            }
        }

        public override void OnCancelAfter()
        {
            base.OnCancelAfter();
            
        }

        private PedHash GetRandomPed(PedType pedType)
        {
            if (pedType == PedType.Robber)
            {
                return PedHash.PestContGunman;
            } else if (pedType == PedType.Civilian)
            {
                List<PedHash> list = new List<PedHash>
                {
                    PedHash.MilitaryBum,
                    PedHash.FilmDirector,
                    PedHash.FemBarberSFM,
                    PedHash.AmandaTownley,
                    PedHash.Agent14,
                    PedHash.ArmGoon01GMM,
                    PedHash.Ashley,
                    PedHash.Bankman,
                    PedHash.Bartender01SFY,
                    PedHash.Beach01AFY,
                    PedHash.Beach01AMM,
                    PedHash.Beachvesp02AMY,
                    PedHash.Brad,
                    PedHash.Car3Guy1,
                    PedHash.ChiBoss01GMM,
                    PedHash.Clay,
                    PedHash.Dale,
                    PedHash.DrFriedlander,
                    PedHash.EdToh,
                    PedHash.Eastsa02AMY,
                    PedHash.FibMugger01,
                    PedHash.G,
                    PedHash.Glenstank01,
                    PedHash.Guido01,
                    PedHash.Hipster01AFY,
                    PedHash.Hipster01AMY,
                    PedHash.HughCutscene,
                    PedHash.JoeMinuteman,
                    PedHash.Josh,
                    PedHash.KerryMcintosh,
                    PedHash.Marston01
                };
                return list[new Random().Next(list.Count)];
            } else // Change this later
            {
                List<PedHash> list = new List<PedHash>
                {

                };
                return list[new Random().Next(list.Count)];
            }
        }

    }
}