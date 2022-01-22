using System;

namespace UOStudio.Client.Engine.Native.OpenGL
{
    public unsafe partial class GL
    {
        public static uint CreateRenderbuffer()
        {
            uint name = 0;
            GL.CreateRenderbuffers(1, &name);
            return name;
        }

        public static void GenQueries(int n, int[] ids)
        {
            fixed (int* idsPtr = &ids[0])
            {
                GenQueries(n, idsPtr);
            }
        }

        public static void GetQueryObjecti(int id, QueryObjectParameterName pname, Span<int> parameters)
        {
            fixed (int* parametersPtr = parameters)
            {
                GetQueryObjectiv(id, pname, parametersPtr);
            }
        }
        public static void GetQueryObjecti(int id, QueryObjectParameterName pname, int[] parameters)
        {
            fixed (int* parametersPtr = parameters)
            {
                GetQueryObjectiv(id, pname, parametersPtr);
            }
        }
        public static void GetQueryObjecti(int id, QueryObjectParameterName pname, ref int parameters)
        {
            fixed (int* parametersPtr = &parameters)
            {
                GetQueryObjectiv(id, pname, parametersPtr);
            }
        }
        public static void GetQueryObjectui(int id, QueryObjectParameterName pname, Span<uint> parameters)
        {
            fixed (uint* parametersPtr = parameters)
            {
                GetQueryObjectuiv(id, pname, parametersPtr);
            }
        }
        public static void GetQueryObjectui(int id, QueryObjectParameterName pname, uint[] parameters)
        {
            fixed (uint* parametersPtr = parameters)
            {
                GetQueryObjectuiv(id, pname, parametersPtr);
            }
        }
        public static void GetQueryObjectui(int id, QueryObjectParameterName pname, ref uint parameters)
        {
            fixed (uint* parametersPtr = &parameters)
            {
                GetQueryObjectuiv(id, pname, parametersPtr);
            }
        }

        public static void GetQueryObjecti64(int id, QueryObjectParameterName pname, Span<long> parameters)
        {
            fixed (long* parameters_ptr = parameters)
            {
                GetQueryObjecti64v(id, pname, parameters_ptr);
            }
        }
        public static void GetQueryObjecti64(int id, QueryObjectParameterName pname, long[] parameters)
        {
            fixed (long* parameters_ptr = parameters)
            {
                GetQueryObjecti64v(id, pname, parameters_ptr);
            }
        }
        public static void GetQueryObjecti64(int id, QueryObjectParameterName pname, ref long parameters)
        {
            fixed (long* parameters_ptr = &parameters)
            {
                GetQueryObjecti64v(id, pname, parameters_ptr);
            }
        }
        public static void GetQueryObjectui64(int id, QueryObjectParameterName pname, Span<ulong> parameters)
        {
            fixed (ulong* parameters_ptr = parameters)
            {
                GetQueryObjectui64v(id, pname, parameters_ptr);
            }
        }
        public static void GetQueryObjectui64(int id, QueryObjectParameterName pname, ulong[] parameters)
        {
            fixed (ulong* parameters_ptr = parameters)
            {
                GetQueryObjectui64v(id, pname, parameters_ptr);
            }
        }
        public static void GetQueryObjectui64(int id, QueryObjectParameterName pname, ref ulong parameters)
        {
            fixed (ulong* parameters_ptr = &parameters)
            {
                GL.GetQueryObjectui64v(id, pname, parameters_ptr);
            }
        }

        public static void GetQueryBufferObjecti64(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
        {
            GetQueryBufferObjecti64v(id, buffer, pname, offset);
        }
        public static void GetQueryBufferObjecti(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
        {
            GetQueryBufferObjectiv(id, buffer, pname, offset);
        }
        public static void GetQueryBufferObjectui64(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
        {
            GetQueryBufferObjectui64v(id, buffer, pname, offset);
        }
        public static void GetQueryBufferObjectui(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
        {
            GetQueryBufferObjectuiv(id, buffer, pname, offset);
        }
    }
}
