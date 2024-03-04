﻿using LenovoController.Providers;

namespace LenovoController.Features
{
    public enum FnLockState
    {
        Off,
        On
    }

    public class FnLockFeature : AbstractDriverFeature<FnLockState>
    {
        public FnLockFeature() : base(DriverProvider.EnergyDriver, 0x831020E8)
        {
        }

        protected override byte GetInternalStatus()
        {
            return 0x2;
        }

        protected override byte[] ToInternal(FnLockState state)
        {
            switch (state)
            {
                case FnLockState.Off:
                    return [0xF];
                case FnLockState.On:
                    return [0xE];
                default:
                    throw new Exception("Invalid state");
            }
        }

        protected override FnLockState FromInternal(uint state)
        {
            state = ReverseEndianness(state);
            if (GetNthBit(state, 18))
                return FnLockState.On;
            return FnLockState.Off;
        }
    }
}