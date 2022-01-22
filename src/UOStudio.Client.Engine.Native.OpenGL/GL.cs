using System;
using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Native.OpenGL
{
    public partial class GL
    {
        public delegate void GLDebugProc(DebugSource source, DebugType type, uint id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam);

        public static unsafe string GetString(StringName stringName)
        {
            var result = GetString_(stringName);
            return Marshal.PtrToStringAnsi((IntPtr)result);
        }

        public static unsafe string GetString(StringName stringName, int index)
        {
            var result = GetStringi(stringName, index);
            return Marshal.PtrToStringAnsi((IntPtr)result);
        }

        public static unsafe int GetInteger(ValueName valueName)
        {
            var result = 0;
            GetInteger(valueName, &result);
            return result;
        }

        public static unsafe int GetInteger(ValueNameGpuMemoryInfoNvx valueName)
        {
            var result = 0;
            GetInteger((ValueName)valueName, &result);
            return result;
        }

        public static unsafe void NamedBufferStorage(uint buffer, nint size, IntPtr data, BufferStorageMask flags)
        {
            var dataPtr = (void*)data;
            NamedBufferStorage(buffer, size, dataPtr, flags);
        }

        public static unsafe void NamedBufferStorage<T>(uint buffer, T[] data, BufferStorageMask flags)
            where T : unmanaged
        {
            var size = (nint)(data.Length * sizeof(T));
            fixed (void* dataPtr = data)
            {
                NamedBufferStorage(buffer, size, dataPtr, flags);
            }
        }

        public static unsafe void NamedBufferSubData<T>(uint buffer, int offset, T[] data)
            where T : unmanaged
        {
            var size = data.Length * sizeof(T);
            fixed (void* dataPtr = data)
            {
                NamedBufferSubData(buffer, offset, size, dataPtr);
            }
        }

        public static unsafe void NamedBufferSubData(uint buffer, int offset, int size, IntPtr data)
        {
            NamedBufferSubData(buffer, offset, size, (void*)data);
        }

        public static unsafe void NamedBufferData(uint buffer, nint size, IntPtr data, VertexBufferObjectUsage usage)
        {
            var dataPtr = (void*)data;
            NamedBufferData(buffer, size, dataPtr, usage);
        }

        public static unsafe void NamedBufferData<T1>(uint buffer, Span<T1> data, VertexBufferObjectUsage usage)
            where T1 : unmanaged
        {
            var size = (nint)(data.Length * sizeof(T1));
            fixed (void* dataPtr = data)
            {
                NamedBufferData(buffer, size, dataPtr, usage);
            }
        }

        public static unsafe void NamedBufferData<TData>(uint buffer, TData[] data, VertexBufferObjectUsage usage)
            where TData : unmanaged
        {
            var size = (nint)(data.Length * sizeof(TData));
            fixed (void* dataPtr = data)
            {
                NamedBufferData(buffer, size, dataPtr, usage);
            }
        }

        public static unsafe uint CreateBuffer()
        {
            uint buffer;
            CreateBuffers(1, &buffer);
            return buffer;
        }

        public static unsafe uint CreateVertexArray()
        {
            uint buffer;
            CreateVertexArrays(1, &buffer);
            return buffer;
        }

        public static unsafe void DeleteBuffer(in uint buffer)
        {
            fixed (uint* buffersPtr = &buffer)
            {
                DeleteBuffers(1, buffersPtr);
            }
        }

        public static unsafe void VertexArrayVertexBuffers(
            uint vao,
            uint first,
            int count,
            in uint buffers,
            in IntPtr offsets,
            in int strides)
        {
            fixed (uint* buffersPtr = &buffers)
            fixed (IntPtr* offsetsPtr = &offsets)
            fixed (int* stridesPtr = &strides)
            {
                VertexArrayVertexBuffers(vao, first, count, buffersPtr, offsetsPtr, stridesPtr);
            }
        }

        public static unsafe void ShaderSource(uint shader, string shaderText)
        {
            var shaderTextPtr = Marshal.StringToCoTaskMemAnsi(shaderText);
            var length = shaderText.Length;
            GL.ShaderSource(shader, 1, (byte**)&shaderTextPtr, &length);
            Marshal.FreeCoTaskMem(shaderTextPtr);
        }

        public static unsafe string GetShaderInfoLog(uint shader, int bufferSize, ref int length)
        {
            string infoLog;
            fixed (int* lengthPtr = &length)
            {
                var infoLogPtr = Marshal.AllocCoTaskMem(bufferSize);
                GetShaderInfoLog(shader, bufferSize, lengthPtr, (byte*)infoLogPtr);
                infoLog = Marshal.PtrToStringUTF8(infoLogPtr)!;
                Marshal.FreeCoTaskMem(infoLogPtr);
            }

            return infoLog;
        }

        public static unsafe void GetShader(uint shader, ShaderParameterName parameterName, ref int parameters)
        {
            fixed (int* parametersPtr = &parameters)
            {
                GetShaderiv(shader, parameterName, parametersPtr);
            }
        }

        public static unsafe void GetProgram(uint program, ProgramPropertyARB parameterName, ref int parameters)
        {
            fixed (int* parametersPtr = &parameters)
            {
                GetProgramiv(program, parameterName, parametersPtr);
            }
        }

        public static unsafe string GetActiveUniform(
            uint program,
            uint index,
            int bufferSize,
            ref int length,
            ref int size,
            ref UniformType type)
        {
            string name;
            fixed (int* lengthPtr = &length)
            fixed (int* sizePtr = &size)
            fixed (UniformType* typePtr = &type)
            {
                var namePtr = Marshal.AllocCoTaskMem(bufferSize);
                GetActiveUniform(program, index, bufferSize, lengthPtr, sizePtr, typePtr, (byte*)namePtr);
                name = Marshal.PtrToStringUTF8(namePtr)!;
                Marshal.FreeCoTaskMem(namePtr);
            }

            return name;
        }

        public static unsafe int GetUniformLocation(uint program, string name)
        {
            var namePtr = Marshal.StringToCoTaskMemUTF8(name);
            var returnValue = GetUniformLocation(program, (byte*)namePtr);
            Marshal.FreeCoTaskMem(namePtr);
            return returnValue;
        }

        public static unsafe string GetActiveUniformBlockName(uint program, uint uniformBlockIndex, int bufferSize, ref int length)
        {
            string uniformBlockName;
            fixed (int* lengthPtr = &length)
            {
                var uniformBlockNamePtr = Marshal.AllocCoTaskMem(bufferSize);
                GetActiveUniformBlockName(program, uniformBlockIndex, bufferSize, lengthPtr, (byte*)uniformBlockNamePtr);
                uniformBlockName = Marshal.PtrToStringUTF8(uniformBlockNamePtr)!;
                Marshal.FreeCoTaskMem(uniformBlockNamePtr);
            }
            return uniformBlockName;
        }

        public static unsafe void GetActiveUniformBlock(uint program, uint uniformBlockIndex, UniformBlockPName parameterName, ref int parameters)
        {
            fixed (int* parametersPtr = &parameters)
            {
                GetActiveUniformBlockiv(program, uniformBlockIndex, parameterName, parametersPtr);
            }
        }

        public static unsafe string GetProgramInfoLog(uint program, int bufferSize, ref int length)
        {
            string infoLog;
            fixed (int* lengthPtr = &length)
            {
                var infoLogPtr = Marshal.AllocCoTaskMem(bufferSize);
                GetProgramInfoLog(program, bufferSize, lengthPtr, (byte*)infoLogPtr);
                infoLog = Marshal.PtrToStringUTF8(infoLogPtr)!;
                Marshal.FreeCoTaskMem(infoLogPtr);
            }
            return infoLog;
        }

        public static unsafe void ObjectLabel(ObjectIdentifier identifier, uint name, string label)
        {
            var labelLength = label.Length;
            if (labelLength > 0)
            {
                var labelPtr = Marshal.StringToHGlobalAnsi(label);
                ObjectLabel(identifier, name, labelLength, (byte*)labelPtr);
                Marshal.FreeHGlobal(labelPtr);
            }
        }

        public static unsafe uint CreateTexture(TextureTarget target)
        {
            uint texture;
            CreateTextures(target, 1, &texture);
            return texture;
        }

        public static unsafe void TextureParameter(uint texture, TextureParameterName parameterName, in int param)
        {
            fixed (int* paramPtr = &param)
            {
                TextureParameteriv(texture, parameterName, paramPtr);
            }
        }

        public static unsafe void TextureParameter(uint texture, TextureParameterName parameterName, in float param)
        {
            fixed (float* paramPtr = &param)
            {
                TextureParameterfv(texture, parameterName, paramPtr);
            }
        }

        public static unsafe void TextureSubImage1D(uint texture, int level, int xOffset, int width, PixelFormat format, PixelType type, IntPtr pixels)
        {
            var pixelPtr = (void*)pixels;
            TextureSubImage1D(texture, level, xOffset, width, format, type, pixelPtr);
        }

        public static unsafe void TextureSubImage1D<TPixel>(uint texture, int level, int xOffset, int width, PixelFormat format, PixelType type, in TPixel pixels)
            where TPixel : unmanaged
        {
            fixed (void* pixelsPtr = &pixels)
            {
                TextureSubImage1D(texture, level, xOffset, width, format, type, pixelsPtr);
            }
        }

        public static unsafe void TextureSubImage2D(uint texture, int level, int xOffset, int yOffset, int width, int height, PixelFormat format, PixelType type, IntPtr pixels)
        {
            var pixelPtr = (void*)pixels;
            TextureSubImage2D(texture, level, xOffset, yOffset, width, height, format, type, pixelPtr);
        }

        public static unsafe void TextureSubImage2D<TPixel>(uint texture, int level, int xOffset, int yOffset, int width, int height, PixelFormat format, PixelType type, in TPixel pixels)
            where TPixel : unmanaged
        {
            fixed (void* pixelPtr = &pixels)
            {
                TextureSubImage2D(texture, level, xOffset, yOffset, width, height, format, type, pixelPtr);
            }
        }

        public static unsafe void TextureSubImage3D(uint texture, int level, int xOffset, int yOffset, int zOffset, int width, int height, int depth, PixelFormat format, PixelType type, IntPtr pixels)
        {
            var pixelPtr = (void*)pixels;
            TextureSubImage3D(texture, level, xOffset, yOffset, zOffset, width, height, depth, format, type, pixelPtr);
        }

        public static unsafe void TextureSubImage3D<TPixel>(uint texture, int level, int xOffset, int yOffset, int zOffset, int width, int height, int depth, PixelFormat format, PixelType type, in TPixel pixels)
            where TPixel : unmanaged
        {
            fixed (void* pixelPtr = &pixels)
            {
                TextureSubImage3D(texture, level, xOffset, yOffset, zOffset, width, height, depth, format, type, pixelPtr);
            }
        }

        public static unsafe void DeleteTexture(in uint textures)
        {
            fixed(uint* texturesHandle = &textures)
            {
                DeleteTextures(1, texturesHandle);
            }
        }

        public static unsafe void DebugMessageCallback(GLDebugProc callback, IntPtr userParam)
        {
            var userParamPtr = (void*)userParam;
            var callbackPtr = Marshal.GetFunctionPointerForDelegate(callback);
            DebugMessageCallback(callbackPtr, userParamPtr);
        }

        public static void VertexArrayAttribFormat(uint vao, uint attributeIndex, int size, VertexAttribType type, bool normalized, uint relativeOffset)
        {
            var normalizedByte = (byte)(normalized ? 1 : 0);
            VertexArrayAttribFormat(vao, attributeIndex, size, type, normalizedByte, relativeOffset);
        }

        public static unsafe void DeleteVertexArray(in uint vao)
        {
            fixed(uint* vaoPtr = &vao)
            {
                DeleteVertexArrays(1, vaoPtr);
            }
        }

        public static unsafe void GetProgramInterface(uint program, ProgramInterface programInterface, ProgramInterfacePName parameterName, ref int parameters)
        {
            fixed (int* parametersPtr = &parameters)
            {
                GetProgramInterfaceiv(program, programInterface, parameterName, parametersPtr);
            }
        }

        public static unsafe string GetProgramResourceName(uint program, ProgramInterface programInterface, uint index, int bufferSize, ref int length)
        {
            string name;
            fixed (int* lengthPtr = &length)
            {
                var namePtr = Marshal.AllocCoTaskMem(bufferSize);
                GetProgramResourceName(program, programInterface, index, bufferSize, lengthPtr, (byte*)namePtr);
                name = Marshal.PtrToStringUTF8(namePtr)!;
                Marshal.FreeCoTaskMem(namePtr);
            }
            return name;
        }

        public static unsafe int GetProgramResourceLocation(uint program, ProgramInterface programInterface, string resourceName)
        {
            var namePtr = Marshal.StringToCoTaskMemUTF8(resourceName);
            var resourceLocation = GetProgramResourceLocation(program, programInterface, (byte*)namePtr);
            Marshal.FreeCoTaskMem(namePtr);
            return resourceLocation;
        }

        public static unsafe int GetProgramResourceLocationIndex(uint program, ProgramInterface programInterface, string name)
        {
            var namePtr = Marshal.StringToCoTaskMemUTF8(name);
            var returnValue = GetProgramResourceLocationIndex(program, programInterface, (byte*)namePtr);
            Marshal.FreeCoTaskMem(namePtr);
            return returnValue;
        }

        public static unsafe void GetProgramResource(
            uint program,
            ProgramInterface programInterface,
            uint index,
            Span<ProgramResourceProperty> properties,
            ref int length,
            Span<int> parameters)
        {
            fixed (int* lengthPtr = &length)
            fixed (ProgramResourceProperty* propsPtr = properties)
            fixed (int* parametersPtr = parameters)
            {
                var propCount = properties.Length;
                var count = parameters.Length;

                GetProgramResourceiv(program, programInterface, index, propCount, propsPtr, count, lengthPtr, parametersPtr);
            }
        }

        public static unsafe string GetActiveAttrib(
            uint program,
            uint index,
            int bufSize,
            ref int length,
            ref int size,
            ref AttributeType type)
        {
            string name;
            fixed (int* lengthPtr = &length)
            fixed (int* sizePtr = &size)
            fixed (AttributeType* typePtr = &type)
            {
                var namePtr = Marshal.AllocCoTaskMem(bufSize);
                GetActiveAttrib(program, index, bufSize, lengthPtr, sizePtr, typePtr, (byte*)namePtr);
                name = Marshal.PtrToStringUTF8(namePtr)!;
                Marshal.FreeCoTaskMem(namePtr);
            }
            return name;
        }

        public static unsafe void TexImage3D<TPixel>(
            TextureTarget target,
            int level,
            int internalformat,
            int width,
            int height,
            int depth,
            int border,
            PixelFormat format,
            PixelType type,
            TPixel[] pixels)
            where TPixel : unmanaged
        {
            fixed (void* pixelsPtr = &pixels[0])
            {
                TexImage3D(target, level, internalformat, width, height, depth, border, format, type, pixelsPtr);
            }
        }

        public static unsafe void DrawElements(
            PrimitiveType primitiveType,
            int elementCount,
            DrawElementsType elementsType,
            nint offset)
        {
            var indices = (void*)offset;
            DrawElements(primitiveType, elementCount, elementsType, indices);
        }

        public static unsafe void DrawElementsInstancedBaseVertex(
            PrimitiveType primitiveType,
            int elementCount,
            DrawElementsType elementsType,
            nint offset,
            int instanceCount,
            int baseVertex)
        {
            var indices = (void*)offset;
            DrawElementsInstancedBaseVertex(primitiveType, elementCount, elementsType, indices, instanceCount, baseVertex);
        }

        public static unsafe uint CreateFramebuffer()
        {
            uint id;
            CreateFramebuffers(1, &id);
            return id;
        }

        public static unsafe void DeleteFramebuffer(in uint framebuffers)
        {
            fixed(uint* framebufferPtr = &framebuffers)
            {
                DeleteFramebuffers(1, framebufferPtr);
            }
        }

        public static unsafe void ClearNamedFramebufferf(uint framebuffer, Buffer buffer, int drawBuffer, in float value)
        {
            fixed (float* valuePtr = &value)
            {
                ClearNamedFramebufferfv(framebuffer, buffer, drawBuffer, valuePtr);
            }
        }

        public static unsafe void ClearNamedFramebufferi(uint framebuffer, Buffer buffer, int drawBuffer, in int value)
        {
            fixed (int* valuePtr = &value)
            {
                ClearNamedFramebufferiv(framebuffer, buffer, drawBuffer, valuePtr);
            }
        }

        public static unsafe void NamedFramebufferDrawBuffers(uint framebuffer, int n, in ColorBuffer buffer)
        {
            fixed (ColorBuffer* buffersPtr = &buffer)
            {
                NamedFramebufferDrawBuffers(framebuffer, n, buffersPtr);
            }
        }

        public static unsafe uint CreateProgramPipeline()
        {
            uint pipeline;
            CreateProgramPipelines(1, &pipeline);
            return pipeline;
        }

        public static unsafe void DeleteProgramPipeline(uint programPipeline)
        {
            DeleteProgramPipelines(1, &programPipeline);
        }

        public static unsafe uint CreateShaderProgram(ShaderType shaderType, string shaderText)
        {
            var shaderTextPtr = Marshal.StringToCoTaskMemAnsi(shaderText);
            var shaderProgram = GL.CreateShaderProgramv(shaderType, 1, (byte**)&shaderTextPtr);
            Marshal.FreeCoTaskMem(shaderTextPtr);
            return shaderProgram;
        }

        public static unsafe void ProgramUniform2i(uint program, int location, int x, int y)
        {
            int count = 1;
            var values = new[] { x, y };
            fixed (int* valuesPtr = &values[0])
            {
                ProgramUniform2iv(program, location, count, valuesPtr);
            }
        }

        public static unsafe void ProgramUniformMatrix4f(uint program, int location, bool transpose, in float value)
        {
            fixed (float* valuePtr = &value)
            {
                var transposeByte = (byte)(transpose ? 1 : 0);
                ProgramUniformMatrix4fv(program, location, 1, transposeByte, valuePtr);
            }
        }

        public static unsafe void UniformMatrix4f(int location, bool transpose, in float value)
        {
            fixed (float* valuePtr = &value)
            {
                var transposeByte = (byte)(transpose ? 1 : 0);
                UniformMatrix4fv(location, 1, transposeByte, valuePtr);
            }
        }

        public static unsafe uint GenVertexArray()
        {
            uint vertexArrays;
            GenVertexArrays(1, &vertexArrays);
            return vertexArrays;
        }

        public static unsafe uint GenBuffer()
        {
            uint buffers;
            GenBuffers(1, &buffers);
            return buffers;
        }

        public static unsafe void BufferData<T>(BufferTargetARB target, T[] data, BufferUsageARB usage)
            where T : unmanaged
        {
            var size = data.Length * sizeof(T);
            fixed (void* dataPtr = data)
            {
                BufferData(target, size, dataPtr, usage);
            }
        }

        public static unsafe void PushDebugGroup(DebugSource source, uint id, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            var messagePtr = Marshal.StringToCoTaskMemUTF8(message);
            PushDebugGroup(source, id, message.Length, (byte*)messagePtr);
            Marshal.FreeCoTaskMem(messagePtr);
        }

        public static unsafe void DrawArraysIndirect(PrimitiveType mode, IntPtr indirect)
        {
            var indirectPtr = (void*)indirect;
            DrawArraysIndirect(mode, indirectPtr);
        }

        public static unsafe void DrawArraysIndirect<TIndirect>(PrimitiveType mode, in TIndirect indirect)
            where TIndirect : unmanaged
        {
            fixed (void* indirectPtr = &indirect)
            {
                DrawArraysIndirect(mode, indirectPtr);
            }
        }

        public static unsafe void DrawElementsIndirect(PrimitiveType mode, DrawElementsType type, IntPtr indirect)
        {
            var indirectPtr = (void*)indirect;
            DrawElementsIndirect(mode, type, indirectPtr);
        }

        public static unsafe void DrawElementsIndirect<TIndirect>(
            PrimitiveType mode,
            DrawElementsType type,
            in TIndirect indirect)
            where TIndirect : unmanaged
        {
            fixed (void* indirectPtr = &indirect)
            {
                DrawElementsIndirect(mode, type, indirectPtr);
            }
        }

        public static unsafe void MultiDrawArraysIndirect(
            PrimitiveType primitiveType,
            IntPtr indirect,
            int indirectDrawCount,
            int indirectStride)
        {
            var indirectPtr = (void*)indirect;
            MultiDrawArraysIndirect(primitiveType, indirectPtr, indirectDrawCount, indirectStride);
        }

        public static unsafe void MultiDrawArraysIndirect<TIndirect>(
            PrimitiveType primitiveType,
            ReadOnlySpan<TIndirect> indirect,
            int indirectDrawCount,
            int indirectStride)
            where TIndirect : unmanaged
        {
            fixed (void* indirectPtr = indirect)
            {
                MultiDrawArraysIndirect(primitiveType, indirectPtr, indirectDrawCount, indirectStride);
            }
        }
        public static unsafe void MultiDrawArraysIndirect<TIndirect>(
            PrimitiveType primitiveType,
            TIndirect[] indirect,
            int indirectDrawCount,
            int indirectStride)
            where TIndirect : unmanaged
        {
            fixed (void* indirectPtr = indirect)
            {
                MultiDrawArraysIndirect(primitiveType, indirectPtr, indirectDrawCount, indirectStride);
            }
        }
        public static unsafe void MultiDrawArraysIndirect<TIndirect>(
            PrimitiveType primitiveType,
            in TIndirect indirect,
            int indirectDrawCount,
            int indirectStride)
            where TIndirect : unmanaged
        {
            fixed (void* indirectPtr = &indirect)
            {
                MultiDrawArraysIndirect(primitiveType, indirectPtr, indirectDrawCount, indirectStride);
            }
        }

        public static unsafe void MultiDrawElementsIndirect(
            PrimitiveType primitiveType,
            DrawElementsType elementsType,
            IntPtr indirect,
            int indirectDrawCount,
            int indirectStride)
        {
            var indirectPtr = (void*)indirect;
            MultiDrawElementsIndirect(primitiveType, elementsType, indirectPtr, indirectDrawCount, indirectStride);
        }

        public static unsafe void MultiDrawElementsIndirect<TIndirect>(
            PrimitiveType primitiveType,
            DrawElementsType elementsType,
            ReadOnlySpan<TIndirect> indirect,
            int indirectDrawCount,
            int indirectStride)
            where TIndirect : unmanaged
        {
            fixed (void* indirectPtr = indirect)
            {
                MultiDrawElementsIndirect(primitiveType, elementsType, indirectPtr, indirectDrawCount, indirectStride);
            }
        }

        public static unsafe void MultiDrawElementsIndirect<TIndirect>(
            PrimitiveType primitiveType,
            DrawElementsType elementsType,
            TIndirect[] indirect,
            int indirectDrawCount,
            int indirectStride)
            where TIndirect : unmanaged
        {
            fixed (void* indirectPtr = indirect)
            {
                MultiDrawElementsIndirect(primitiveType, elementsType, indirectPtr, indirectDrawCount, indirectStride);
            }
        }

        public static unsafe void MultiDrawElementsIndirect<TIndirect>(
            PrimitiveType primitiveType,
            DrawElementsType elementsType,
            in TIndirect indirect,
            int indirectDrawCount,
            int indirectStride)
            where TIndirect : unmanaged
        {
            fixed (void* indirectPtr = &indirect)
            {
                MultiDrawElementsIndirect(primitiveType, elementsType, indirectPtr, indirectDrawCount, indirectStride);
            }
        }
    }
}
