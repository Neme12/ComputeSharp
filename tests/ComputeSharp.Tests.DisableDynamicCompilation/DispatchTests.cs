﻿using System;
using System.Linq;
using ComputeSharp.Interop;
using ComputeSharp.Tests.Attributes;
using ComputeSharp.Tests.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComputeSharp.Tests.DisableDynamicCompilation;

[TestClass]
[TestCategory("Dispatch")]
public partial class DispatchTests
{
    private static readonly bool IsDynamicCompilationDisabled =
#if DISABLE_RUNTIME_SHADER_COMPILATION_SUPPORT
        true;
#else
        false;
#endif

    [CombinatorialTestMethod]
    [AllDevices]
    public void ComputeShader_Test_Ok(Device device)
    {
        if (!IsDynamicCompilationDisabled)
        {
            Assert.Inconclusive();
        }

        float[] array = Enumerable.Range(0, 128).Select(static i => (float)i).ToArray();

        using ReadWriteBuffer<float> buffer = device.Get().AllocateReadWriteBuffer(array);

        device.Get().For(128, 1, 1, 32, 1, 1, new ComputeShader(buffer, 2.0f));

        float[] result = buffer.ToArray();

        foreach (ref float value in array.AsSpan())
        {
            value *= 2.0f;
        }

        CollectionAssert.AreEqual(result, array);
    }

    [CombinatorialTestMethod]
    [AllDevices]
    [ExpectedException(typeof(NotSupportedException))]
    public void ComputeShader_Test_Fail(Device device)
    {
        if (!IsDynamicCompilationDisabled)
        {
            Assert.Inconclusive();
        }

        using ReadWriteBuffer<float> buffer = device.Get().AllocateReadWriteBuffer<float>(128);

        device.Get().For(128, 1, 1, 64, 1, 1, new ComputeShader(buffer, 2.0f));
    }

    [AutoConstructor]
    [EmbeddedBytecode(32, 1, 1)]
    internal readonly partial struct ComputeShader : IComputeShader
    {
        public readonly ReadWriteBuffer<float> buffer;
        public readonly float factor;

        /// <inheritdoc/>
        public void Execute()
        {
            buffer[ThreadIds.X] *= factor;
        }
    }

    [CombinatorialTestMethod]
    [AllDevices]
    public void PixelShader_Test_Ok(Device device)
    {
        if (!IsDynamicCompilationDisabled)
        {
            Assert.Inconclusive();
        }

        using ReadWriteTexture2D<Rgba32, float4> texture = device.Get().AllocateReadWriteTexture2D<Rgba32, float4>(128, 128);

        device.Get().ForEach(texture, new PixelShader(0.3f, 0.6f));

        Rgba32[,] result = texture.ToArray();

        for (int i = 0; i < texture.Height; i++)
        {
            for (int j = 0; j < texture.Width; j++)
            {
                byte r = (byte)(j / (float)texture.Width * 255f);
                byte g = (byte)(i / (float)texture.Height * 255f);
                const byte b = (byte)(0.3f * 255f);
                const byte a = (byte)(0.6f * 255f);
                Rgba32 pixel = result[i, j];

                // Use a delta of 1 to account for small differences of precision when rounding. This test is just
                // about validating the shader runs anyway, the actual accuracy is tested in other shader tests.
                Assert.IsTrue(Math.Abs(r - pixel.R) <= 1);
                Assert.IsTrue(Math.Abs(g - pixel.G) <= 1);
                Assert.AreEqual(pixel.B, b);
                Assert.AreEqual(pixel.A, a);
            }
        }
    }

    [AutoConstructor]
    [EmbeddedBytecode(8, 8, 1)]
    internal readonly partial struct PixelShader : IPixelShader<float4>
    {
        private readonly float b;
        private readonly float a;

        /// <inheritdoc/>
        public float4 Execute()
        {
            return new(ThreadIds.Normalized.X, ThreadIds.Normalized.Y, b, a);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void ReflectionService_ComputeShader()
    {
        if (!IsDynamicCompilationDisabled)
        {
            Assert.Inconclusive();
        }

        ReflectionServices.GetShaderInfo<ComputeShader>(out _);
    }

    [TestMethod]
    [ExpectedException(typeof(NotSupportedException))]
    public void ReflectionService_PixelShader()
    {
        if (!IsDynamicCompilationDisabled)
        {
            Assert.Inconclusive();
        }

        ReflectionServices.GetShaderInfo<PixelShader, float4>(out _);
    }
}