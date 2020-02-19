using ColossalFramework;
using ColossalFramework.Math;
using Harmony;
using MoreOutsideInteraction.CustomAI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace MoreOutsideInteraction.Patch
{
    [HarmonyPatch]
    public static class OutsideConnectionAIModifyMaterialBufferPatch
    {
        public static MethodBase TargetMethod()
        {
            return typeof(OutsideConnectionAI).GetMethod("ModifyMaterialBuffer", BindingFlags.Public | BindingFlags.Instance, null, new Type[] { typeof(ushort), typeof(Building).MakeByRefType(), typeof(TransferManager.TransferReason), typeof(int).MakeByRefType() }, null);
        }
        public static bool Prefix(ushort buildingID, ref Building data, TransferManager.TransferReason material, ref int amountDelta)
        {
            if ((data.m_flags & Building.Flags.IncomingOutgoing) == Building.Flags.Incoming)
            {
                if (material == TransferManager.TransferReason.Garbage)
                {
                    if (data.m_garbageBuffer < 0)
                    {
                        DebugLog.LogToFileOnly("Error: garbarge < 0 in outside building, should be wrong");
                        amountDelta = 0;
                    }
                    else
                    {
                        if (data.m_garbageBuffer + amountDelta <= 0)
                        {
                            amountDelta = -data.m_garbageBuffer;
                        }
                        else
                        {

                        }
                        data.m_garbageBuffer = (ushort)(data.m_garbageBuffer + amountDelta);
                    }
                }
                else if (material == TransferManager.TransferReason.Crime)
                {
                    if (amountDelta < 0)
                    {
                        if (data.m_crimeBuffer < 0)
                        {
                            DebugLog.LogToFileOnly("Error: crime < 0 in outside building, should be wrong");
                            amountDelta = 0;
                        }
                        else
                        {
                            if (data.m_crimeBuffer + amountDelta * 100 <= 0)
                            {
                                amountDelta = -data.m_crimeBuffer / 100;
                            }
                            else
                            {

                            }
                            data.m_crimeBuffer = (ushort)(data.m_crimeBuffer + amountDelta * 100);
                        }
                    }
                }
                else if (material == TransferManager.TransferReason.Sick)
                {
                    if (amountDelta < 0)
                    {
                        if (data.m_customBuffer2 < 0)
                        {
                            DebugLog.LogToFileOnly("Error: sick < 0 in outside building, should be wrong");
                            amountDelta = 0;
                        }
                        else
                        {
                            if (data.m_customBuffer2 + amountDelta * 100 <= 0)
                            {
                                amountDelta = -data.m_customBuffer2 / 100;
                            }
                            else
                            {

                            }
                            data.m_customBuffer2 = (ushort)(data.m_customBuffer2 + amountDelta * 100);
                        }
                    }
                }
                else if (material == TransferManager.TransferReason.Fire)
                {
                    if (data.m_electricityBuffer < 0)
                    {
                        DebugLog.LogToFileOnly("Error: fire < 0 in outside building, should be wrong");
                        amountDelta = 0;
                    }
                    else
                    {
                        if (data.m_electricityBuffer + amountDelta * 100 <= 0)
                        {
                            amountDelta = -data.m_electricityBuffer / 100;
                        }
                        else
                        {

                        }
                        data.m_electricityBuffer = (ushort)(data.m_electricityBuffer + amountDelta * 100);
                    }
                }
                else
                {
                }
            }
            else
            {
                if (material == TransferManager.TransferReason.GarbageMove)
                {
                    if (data.m_garbageBuffer < 0)
                    {
                        DebugLog.LogToFileOnly("Error: garbarge < 0 in outside building, should be wrong");
                        amountDelta = 0;
                    }
                    else
                    {
                        if (data.m_garbageBuffer + amountDelta <= 0)
                        {
                            amountDelta = -data.m_garbageBuffer;
                        }
                        else
                        {

                        }
                        data.m_garbageBuffer = (ushort)(data.m_garbageBuffer + amountDelta);
                    }
                }
                else if (material == TransferManager.TransferReason.DeadMove)
                {
                    if (data.m_customBuffer1 < 0)
                    {
                        DebugLog.LogToFileOnly("Error: dead < 0 in outside building, should be wrong");
                        amountDelta = 0;
                    }
                    else
                    {
                        if (data.m_customBuffer1 + amountDelta <= 0)
                        {
                            amountDelta = -data.m_customBuffer1;
                        }
                        else
                        {

                        }
                        data.m_customBuffer1 = (ushort)(data.m_customBuffer1 + amountDelta);
                    }
                }
                else if (material == TransferManager.TransferReason.Garbage)
                {
                    amountDelta = 0;
                }
                else
                {
                    //do nothing
                }
            }
            return false;
        }
    }
}
