﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComputeSharp.Tests
{
    [TestClass]
    [TestCategory("StaticFunctions")]
    public class StaticFunctionsTests
    {
        [TestMethod]
        public void FloatToFloatFunc()
        {
            using ReadWriteBuffer<float> buffer = Gpu.Default.AllocateReadWriteBuffer<float>(1);

            Func<float, float> square = StaticMethodsContainer.Square;

            Gpu.Default.For(1, id => buffer[0] = square(3));

            float[] result = buffer.GetData();

            Assert.IsTrue(MathF.Abs(result[0] - 9) < 0.0001f);
        }

        [TestMethod]
        public void InternalFloatToFloatFunc()
        {
            using ReadWriteBuffer<float> buffer = Gpu.Default.AllocateReadWriteBuffer<float>(1);

            Func<float, float> square = StaticMethodsContainer.InternalSquare;

            Gpu.Default.For(1, id => buffer[0] = square(3));

            float[] result = buffer.GetData();

            Assert.IsTrue(MathF.Abs(result[0] - 9) < 0.0001f);
        }

        [TestMethod]
        public void ChangingFuncsFromSameMethod()
        {
            using ReadWriteBuffer<float> buffer = Gpu.Default.AllocateReadWriteBuffer<float>(1);

            Func<float, float> func = StaticMethodsContainer.Square;

            Gpu.Default.For(1, id => buffer[0] = func(3));

            float[] result = buffer.GetData();

            Assert.IsTrue(MathF.Abs(result[0] - 9) < 0.0001f);

            func = StaticMethodsContainer.Negate;

            Gpu.Default.For(1, id => buffer[0] = func(3));

            result = buffer.GetData();

            Assert.IsTrue(MathF.Abs(result[0] + 3) < 0.0001f);
        }

        [TestMethod]
        public void ChangingFuncsFromExternalMethod()
        {
            MethodRunner(StaticMethodsContainer.Square, 3, 9);

            MethodRunner(StaticMethodsContainer.Negate, 3, -3);
        }

        private void MethodRunner(Func<float, float> func, float input, float expected)
        {
            using ReadWriteBuffer<float> buffer = Gpu.Default.AllocateReadWriteBuffer<float>(1);

            Gpu.Default.For(1, id => buffer[0] = func(input));

            float[] result = buffer.GetData();

            Assert.IsTrue(MathF.Abs(result[0] - expected) < 0.0001f);
        }
    }
}
