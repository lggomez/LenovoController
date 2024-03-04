﻿using LenovoController.Providers;

namespace LenovoController.Features
{
    public enum AlwaysOnUsbState
    {
        Off,
        OnWhenSleeping,
        OnAlways
    }

    public class AlwaysOnUsbFeature : AbstractDriverFeature<AlwaysOnUsbState>
    {
        public AlwaysOnUsbFeature() : base(DriverProvider.EnergyDriver, 0x831020E8)
        {
        }

        protected override byte GetInternalStatus()
        {
            return 0x2;
        }

        protected override byte[] ToInternal(AlwaysOnUsbState state)
        {
            return state switch
            {
                AlwaysOnUsbState.Off => [0xB, 0x12],
                AlwaysOnUsbState.OnWhenSleeping => [0xA, 0x12],
                AlwaysOnUsbState.OnAlways => [0xA, 0x13],
                _ => throw new Exception("Invalid state"),
            };
        }

        protected override AlwaysOnUsbState FromInternal(uint state)
        {
            state = ReverseEndianness(state);
            if (GetNthBit(state, 31)) // is on?
            {
                if (GetNthBit(state, 23))
                    return AlwaysOnUsbState.OnAlways;
                return AlwaysOnUsbState.OnWhenSleeping;
            }

            return AlwaysOnUsbState.Off;
        }
    }
}