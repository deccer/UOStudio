using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace UOStudio.Core
{
    public static unsafe class MemoryManager
    {
        private static readonly bool IsMonoRuntime = Type.GetType("Mono.Runtime") != null;

        static MemoryManager()
        {
            Console.WriteLine("Platform: {0}", IsMonoRuntime ? "Mono" : ".NET");
        }

        [MethodImpl(256)]
        public static void* AsPointer<T>(ref T value)
        {
            var typedReference = __makeref(value);

            return (void*)*((IntPtr*)&typedReference + (IsMonoRuntime ? 1 : 0));
        }

        public static T ToStruct<T>(IntPtr ptr) => ToStruct<T>(ptr, SizeOf<T>());

        [MethodImpl(256)]
        public static T ToStruct<T>(IntPtr ptr, int size)
        {
            var str = (byte*)ptr;

            T result = default;
            var resultPtr = (byte*)AsPointer(ref result);
            Buffer.MemoryCopy(str, resultPtr, size, size);

            return result;
        }

        [MethodImpl(256)]
        public static T As<T>(object v)
        {
            var size = SizeOf<T>();

            return Reinterpret<object, T>(v, size);
        }

        [MethodImpl(256)]
        public static int SizeOf<T>()
        {
            var doubleStruct = DoubleStruct<T>.Value;
            var tRef0 = __makeref(doubleStruct.First);
            var tRef1 = __makeref(doubleStruct.Second);
            IntPtr ptrToT0, ptrToT1;

            if (IsMonoRuntime)
            {
                ptrToT0 = *((IntPtr*)&tRef0 + 1);
                ptrToT1 = *((IntPtr*)&tRef1 + 1);
            }
            else
            {
                ptrToT0 = *(IntPtr*)&tRef0;
                ptrToT1 = *(IntPtr*)&tRef1;
            }

            return (int)((byte*)ptrToT1 - (byte*)ptrToT0);
        }


        [MethodImpl(256)]
        public static TResult Reinterpret<TInput, TResult>(TInput value, int sizeBytes)
        {
            TResult result = default;

            var resultRef = __makeref(result);
            var valueRef = __makeref(value);

            var offset = IsMonoRuntime ? 1 : 0;

            var resultPtr = (byte*)*((IntPtr*)&resultRef + offset);
            var valuePtr = (byte*)*((IntPtr*)&valueRef + offset);

            Buffer.MemoryCopy(valuePtr, resultPtr, sizeBytes, sizeBytes);

            return result;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct DoubleStruct<T>
        {
            public T First;
            public T Second;
            public static readonly DoubleStruct<T> Value;
        }
    }
}
