﻿// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from shared/winerror.h in the Windows SDK for Windows 10.0.22000.0
// Original source is Copyright © Microsoft. All rights reserved.

namespace TerraFX.Interop.DirectX
{
    internal static partial class D2DERR
    {
        [NativeTypeName("#define D2DERR_INSUFFICIENT_DEVICE_CAPABILITIES _HRESULT_TYPEDEF_(0x88990026L)")]
        public const int D2DERR_INSUFFICIENT_DEVICE_CAPABILITIES = unchecked((int)(0x88990026));
    }
}