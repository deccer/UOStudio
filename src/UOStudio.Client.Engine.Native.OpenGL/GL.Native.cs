using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace UOStudio.Client.Engine.Native.OpenGL
{
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1023:Dereference and access of symbols should be spaced correctly")]
    public unsafe partial class GL
    {
        public static void Clear(ClearBufferMask mask)
        {
            _clearDelegate(mask);
        }

        private static delegate* unmanaged<ClearBufferMask, void> _clearDelegate = &Clear_Lazy;

        [UnmanagedCallersOnly]
        private static void Clear_Lazy(ClearBufferMask mask)
        {
            _clearDelegate = (delegate* unmanaged<ClearBufferMask, void>)Sdl.GetProcAddress("glClear");
            _clearDelegate(mask);
        }

        public static void ClearColor(float red, float green, float blue, float alpha)
        {
            _clearColorDelegate(red, green, blue, alpha);
        }

        private static delegate* unmanaged<float, float, float, float, void> _clearColorDelegate = &ClearColor_Lazy;

        [UnmanagedCallersOnly]
        private static void ClearColor_Lazy(float red, float green, float blue, float alpha)
        {
            _clearColorDelegate =
                (delegate* unmanaged<float, float, float, float, void>)Sdl.GetProcAddress("glClearColor");
            _clearColorDelegate(red, green, blue, alpha);
        }

        private static byte* GetString_(StringName name)
        {
            return _getStringDelegate(name);
        }

        private static delegate* unmanaged<StringName, byte*> _getStringDelegate = &GetString_Lazy;

        [UnmanagedCallersOnly]
        private static byte* GetString_Lazy(StringName name)
        {
            _getStringDelegate = (delegate* unmanaged<StringName, byte*>)Sdl.GetProcAddress("glGetString");
            return _getStringDelegate(name);
        }

        private static byte* GetStringi(StringName name, int index)
        {
            return _getStringiDelegate(name, index);
        }

        private static delegate* unmanaged<StringName, int, byte*> _getStringiDelegate = &GetStringi_Lazy;

        [UnmanagedCallersOnly]
        private static byte* GetStringi_Lazy(StringName name, int index)
        {
            _getStringiDelegate = (delegate* unmanaged<StringName, int, byte*>)Sdl.GetProcAddress("glGetStringi");
            return _getStringiDelegate(name, index);
        }

        private static void GetInteger(ValueName valueName, int* data)
        {
            _getIntegervDelegate(valueName, data);
        }

        private static delegate* unmanaged<ValueName, int*, void> _getIntegervDelegate = &GetIntegerv_Lazy;

        [UnmanagedCallersOnly]
        private static void GetIntegerv_Lazy(ValueName valueName, int* data)
        {
            _getIntegervDelegate = (delegate* unmanaged<ValueName, int*, void>)Sdl.GetProcAddress("glGetIntegerv");
            _getIntegervDelegate(valueName, data);
        }

        private static void NamedBufferStorage(uint buffer, nint size, void* data, BufferStorageMask flags)
        {
            _namedBufferStorageDelegate(buffer, size, data, flags);
        }

        private static delegate* unmanaged<uint, nint, void*, BufferStorageMask, void> _namedBufferStorageDelegate =
            &NamedBufferStorage_Lazy;

        [UnmanagedCallersOnly]
        private static void NamedBufferStorage_Lazy(uint buffer, nint size, void* data, BufferStorageMask flags)
        {
            _namedBufferStorageDelegate =
                (delegate* unmanaged<uint, nint, void*, BufferStorageMask, void>)Sdl.GetProcAddress(
                    "glNamedBufferStorage");
            _namedBufferStorageDelegate(buffer, size, data, flags);
        }

        public static void NamedBufferSubData(uint buffer, int offset, int size, void* data)
        {
            _namedBufferSubDataDelegate(buffer, offset, size, data);
        }

        private static delegate* unmanaged<uint, int, int, void*, void> _namedBufferSubDataDelegate =
            &NamedBufferSubData_Lazy;

        [UnmanagedCallersOnly]
        private static void NamedBufferSubData_Lazy(uint buffer, int offset, int size, void* data)
        {
            _namedBufferSubDataDelegate =
                (delegate* unmanaged<uint, int, int, void*, void>)Sdl.GetProcAddress("glNamedBufferSubData");
            _namedBufferSubDataDelegate(buffer, offset, size, data);
        }

        public static void NamedBufferData(uint buffer, nint size, void* data, VertexBufferObjectUsage usage)
        {
            _namedBufferDataDelegate(buffer, size, data, usage);
        }

        private static delegate* unmanaged<uint, nint, void*, VertexBufferObjectUsage, void> _namedBufferDataDelegate =
            &NamedBufferData_Lazy;

        [UnmanagedCallersOnly]
        private static void NamedBufferData_Lazy(uint buffer, nint size, void* data, VertexBufferObjectUsage usage)
        {
            _namedBufferDataDelegate =
                (delegate* unmanaged<uint, nint, void*, VertexBufferObjectUsage, void>)Sdl.GetProcAddress(
                    "glNamedBufferData");
            _namedBufferDataDelegate(buffer, size, data, usage);
        }

        public static void NamedRenderbufferStorage(uint renderbuffer, SizedInternalFormat internalformat, int width,
            int height)
        {
            _namedRenderbufferStorageDelegate(renderbuffer, internalformat, width, height);
        }

        private static delegate* unmanaged<uint, SizedInternalFormat, int, int, void>
            _namedRenderbufferStorageDelegate = &NamedRenderbufferStorage_Lazy;

        [UnmanagedCallersOnly]
        private static void NamedRenderbufferStorage_Lazy(uint renderbuffer, SizedInternalFormat internalformat,
            int width, int height)
        {
            _namedRenderbufferStorageDelegate =
                (delegate* unmanaged<uint, SizedInternalFormat, int, int, void>)Sdl.GetProcAddress(
                    "glNamedRenderbufferStorage");
            _namedRenderbufferStorageDelegate(renderbuffer, internalformat, width, height);
        }

        public static void NamedFramebufferRenderbuffer(uint framebuffer, FramebufferAttachment attachment,
            RenderbufferTarget renderbuffertarget, uint renderbuffer)
        {
            _namedFramebufferRenderbufferDelegate(framebuffer, attachment, renderbuffertarget, renderbuffer);
        }

        private static delegate* unmanaged<uint, FramebufferAttachment, RenderbufferTarget, uint, void>
            _namedFramebufferRenderbufferDelegate = &NamedFramebufferRenderbuffer_Lazy;

        [UnmanagedCallersOnly]
        private static void NamedFramebufferRenderbuffer_Lazy(uint framebuffer, FramebufferAttachment attachment,
            RenderbufferTarget renderbuffertarget, uint renderbuffer)
        {
            _namedFramebufferRenderbufferDelegate =
                (delegate* unmanaged<uint, FramebufferAttachment, RenderbufferTarget, uint, void>)Sdl.GetProcAddress(
                    "glNamedFramebufferRenderbuffer");
            _namedFramebufferRenderbufferDelegate(framebuffer, attachment, renderbuffertarget, renderbuffer);
        }

        public static void CreateRenderbuffers(int n, uint* renderbuffers)
        {
            _createRenderbuffersDelegate(n, renderbuffers);
        }

        private static delegate* unmanaged<int, uint*, void> _createRenderbuffersDelegate = &CreateRenderbuffers_Lazy;

        [UnmanagedCallersOnly]
        private static void CreateRenderbuffers_Lazy(int n, uint* renderbuffers)
        {
            _createRenderbuffersDelegate =
                (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glCreateRenderbuffers");
            _createRenderbuffersDelegate(n, renderbuffers);
        }

        private static delegate* unmanaged<int, uint*, void> _createBuffersDelegate = &CreateBuffers_Lazy;
        private static void CreateBuffers(int n, uint* buffers)
        {
            _createBuffersDelegate(n, buffers);
        }

        [UnmanagedCallersOnly]
        private static void CreateBuffers_Lazy(int n, uint* buffers)
        {
            _createBuffersDelegate = (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glCreateBuffers");
            _createBuffersDelegate(n, buffers);
        }

        private static delegate* unmanaged<int, uint*, void> _deleteBuffersDelegate = &DeleteBuffers_Lazy;
        private static void DeleteBuffers(int n, uint* buffers)
        {
            _deleteBuffersDelegate(n, buffers);
        }

        [UnmanagedCallersOnly]
        private static void DeleteBuffers_Lazy(int n, uint* buffers)
        {
            _deleteBuffersDelegate = (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glDeleteBuffers");
            _deleteBuffersDelegate(n, buffers);
        }

        public static void CreateVertexArrays(int n, uint* vaos)
        {
            _createVertexArraysDelegate(n, vaos);
        }

        private static delegate* unmanaged<int, uint*, void> _createVertexArraysDelegate = &CreateVertexArrays_Lazy;

        [UnmanagedCallersOnly]
        private static void CreateVertexArrays_Lazy(int n, uint* vaos)
        {
            _createVertexArraysDelegate =
                (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glCreateVertexArrays");
            _createVertexArraysDelegate(n, vaos);
        }

        public static void BindVertexArray(uint vao)
        {
            _bindVertexArrayDelegate(vao);
        }

        private static delegate* unmanaged<uint, void> _bindVertexArrayDelegate = &BindVertexArray_Lazy;

        [UnmanagedCallersOnly]
        private static void BindVertexArray_Lazy(uint vao)
        {
            _bindVertexArrayDelegate = (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glBindVertexArray");
            _bindVertexArrayDelegate(vao);
        }

        public static void DeleteVertexArrays(int n, uint* arrays)
        {
            _deleteVertexArraysDelegate(n, arrays);
        }

        private static delegate* unmanaged<int, uint*, void> _deleteVertexArraysDelegate = &DeleteVertexArrays_Lazy;

        [UnmanagedCallersOnly]
        private static void DeleteVertexArrays_Lazy(int n, uint* arrays)
        {
            _deleteVertexArraysDelegate =
                (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glDeleteVertexArrays");
            _deleteVertexArraysDelegate(n, arrays);
        }

        public static void DrawArrays(PrimitiveType mode, int first, int count)
        {
            _drawArraysDelegate(mode, first, count);
        }

        private static delegate* unmanaged<PrimitiveType, int, int, void> _drawArraysDelegate = &DrawArrays_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawArrays_Lazy(PrimitiveType mode, int first, int count)
        {
            _drawArraysDelegate =
                (delegate* unmanaged<PrimitiveType, int, int, void>)Sdl.GetProcAddress("glDrawArrays");
            _drawArraysDelegate(mode, first, count);
        }

        public static void DrawElements(PrimitiveType primitiveType, int elementCount, DrawElementsType elementsType, void* indices)
        {
            _drawElementsDelegate(primitiveType, elementCount, elementsType, indices);
        }

        private static delegate* unmanaged<PrimitiveType, int, DrawElementsType, void*, void> _drawElementsDelegate =
            &DrawElements_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawElements_Lazy(PrimitiveType mode, int count, DrawElementsType type, void* indices)
        {
            _drawElementsDelegate =
                (delegate* unmanaged<PrimitiveType, int, DrawElementsType, void*, void>)Sdl.GetProcAddress(
                    "glDrawElements");
            _drawElementsDelegate(mode, count, type, indices);
        }

        public static void VertexArrayVertexBuffer(uint vao, uint bindingIndex, uint buffer, IntPtr offset, int stride)
        {
            _vertexArrayVertexBufferDelegate(vao, bindingIndex, buffer, offset, stride);
        }

        private static delegate* unmanaged<uint, uint, uint, IntPtr, int, void> _vertexArrayVertexBufferDelegate =
            &VertexArrayVertexBuffer_Lazy;

        [UnmanagedCallersOnly]
        private static void VertexArrayVertexBuffer_Lazy(uint vao, uint bindingIndex, uint buffer, IntPtr offset,
            int stride)
        {
            _vertexArrayVertexBufferDelegate =
                (delegate* unmanaged<uint, uint, uint, IntPtr, int, void>)Sdl.GetProcAddress(
                    "glVertexArrayVertexBuffer");
            _vertexArrayVertexBufferDelegate(vao, bindingIndex, buffer, offset, stride);
        }

        public static void VertexArrayVertexBuffers(uint vao, uint first, int count, uint* buffers, IntPtr* offsets,
            int* strides)
        {
            _vertexArrayVertexBuffersDelegate(vao, first, count, buffers, offsets, strides);
        }

        private static delegate* unmanaged<uint, uint, int, uint*, IntPtr*, int*, void>
            _vertexArrayVertexBuffersDelegate = &VertexArrayVertexBuffers_Lazy;

        [UnmanagedCallersOnly]
        private static void VertexArrayVertexBuffers_Lazy(uint vao, uint first, int count, uint* buffers,
            IntPtr* offsets, int* strides)
        {
            _vertexArrayVertexBuffersDelegate =
                (delegate* unmanaged<uint, uint, int, uint*, IntPtr*, int*, void>)Sdl.GetProcAddress(
                    "glVertexArrayVertexBuffers");
            _vertexArrayVertexBuffersDelegate(vao, first, count, buffers, offsets, strides);
        }

        public static void VertexArrayAttribBinding(uint vao, uint attributeIndex, uint bindingIndex)
        {
            _vertexArrayAttribBindingDelegate(vao, attributeIndex, bindingIndex);
        }

        private static delegate* unmanaged<uint, uint, uint, void> _vertexArrayAttribBindingDelegate =
            &VertexArrayAttribBinding_Lazy;

        [UnmanagedCallersOnly]
        private static void VertexArrayAttribBinding_Lazy(uint vao, uint attributeIndex, uint bindingIndex)
        {
            _vertexArrayAttribBindingDelegate =
                (delegate* unmanaged<uint, uint, uint, void>)Sdl.GetProcAddress("glVertexArrayAttribBinding");
            _vertexArrayAttribBindingDelegate(vao, attributeIndex, bindingIndex);
        }

        public static void VertexArrayAttribFormat(uint vao, uint attributeIndex, int size, VertexAttribType type,
            byte normalized, uint relativeOffset)
        {
            _vertexArrayAttribFormatDelegate(vao, attributeIndex, size, type, normalized, relativeOffset);
        }

        private static delegate* unmanaged<uint, uint, int, VertexAttribType, byte, uint, void>
            _vertexArrayAttribFormatDelegate = &VertexArrayAttribFormat_Lazy;

        [UnmanagedCallersOnly]
        private static void VertexArrayAttribFormat_Lazy(uint vao, uint attributeIndex, int size, VertexAttribType type,
            byte normalized, uint relativeOffset)
        {
            _vertexArrayAttribFormatDelegate =
                (delegate* unmanaged<uint, uint, int, VertexAttribType, byte, uint, void>)Sdl.GetProcAddress(
                    "glVertexArrayAttribFormat");
            _vertexArrayAttribFormatDelegate(vao, attributeIndex, size, type, normalized, relativeOffset);
        }

        public static void EnableVertexArrayAttrib(uint vao, uint index)
        {
            _enableVertexArrayAttribDelegate(vao, index);
        }

        private static delegate* unmanaged<uint, uint, void> _enableVertexArrayAttribDelegate =
            &EnableVertexArrayAttrib_Lazy;

        [UnmanagedCallersOnly]
        private static void EnableVertexArrayAttrib_Lazy(uint vao, uint index)
        {
            _enableVertexArrayAttribDelegate =
                (delegate* unmanaged<uint, uint, void>)Sdl.GetProcAddress("glEnableVertexArrayAttrib");
            _enableVertexArrayAttribDelegate(vao, index);
        }

        public static void DisableVertexArrayAttrib(uint vao, uint index)
        {
            _disableVertexArrayAttribDelegate(vao, index);
        }

        private static delegate* unmanaged<uint, uint, void> _disableVertexArrayAttribDelegate =
            &DisableVertexArrayAttrib_Lazy;

        [UnmanagedCallersOnly]
        private static void DisableVertexArrayAttrib_Lazy(uint vao, uint index)
        {
            _disableVertexArrayAttribDelegate =
                (delegate* unmanaged<uint, uint, void>)Sdl.GetProcAddress("glDisableVertexArrayAttrib");
            _disableVertexArrayAttribDelegate(vao, index);
        }

        public static void DisableVertexAttribArray(uint index)
        {
            _disableVertexAttribArrayDelegate(index);
        }

        private static delegate* unmanaged<uint, void> _disableVertexAttribArrayDelegate =
            &DisableVertexAttribArray_Lazy;

        [UnmanagedCallersOnly]
        private static void DisableVertexAttribArray_Lazy(uint index)
        {
            _disableVertexAttribArrayDelegate =
                (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glDisableVertexAttribArray");
            _disableVertexAttribArrayDelegate(index);
        }

        public static void VertexArrayElementBuffer(uint vao, uint buffer)
        {
            _vertexArrayElementBufferDelegate(vao, buffer);
        }

        private static delegate* unmanaged<uint, uint, void> _vertexArrayElementBufferDelegate =
            &VertexArrayElementBuffer_Lazy;

        [UnmanagedCallersOnly]
        private static void VertexArrayElementBuffer_Lazy(uint vao, uint buffer)
        {
            _vertexArrayElementBufferDelegate =
                (delegate* unmanaged<uint, uint, void>)Sdl.GetProcAddress("glVertexArrayElementBuffer");
            _vertexArrayElementBufferDelegate(vao, buffer);
        }

        public static void Viewport(int x, int y, int width, int height)
        {
            _viewportDelegate(x, y, width, height);
        }

        private static delegate* unmanaged<int, int, int, int, void> _viewportDelegate = &Viewport_Lazy;

        [UnmanagedCallersOnly]
        private static void Viewport_Lazy(int x, int y, int width, int height)
        {
            _viewportDelegate = (delegate* unmanaged<int, int, int, int, void>)Sdl.GetProcAddress("glViewport");
            _viewportDelegate(x, y, width, height);
        }

        public static void BindBufferBase(BufferTargetARB target, uint index, uint buffer)
        {
            _bindBufferBaseDelegate(target, index, buffer);
        }

        private static delegate* unmanaged<BufferTargetARB, uint, uint, void> _bindBufferBaseDelegate =
            &BindBufferBase_Lazy;

        [UnmanagedCallersOnly]
        private static void BindBufferBase_Lazy(BufferTargetARB target, uint index, uint buffer)
        {
            _bindBufferBaseDelegate =
                (delegate* unmanaged<BufferTargetARB, uint, uint, void>)Sdl.GetProcAddress("glBindBufferBase");
            _bindBufferBaseDelegate(target, index, buffer);
        }

        public static void ActiveTexture(TextureUnit texture)
        {
            _activeTextureDelegate(texture);
        }

        private static delegate* unmanaged<TextureUnit, void> _activeTextureDelegate = &ActiveTexture_Lazy;

        [UnmanagedCallersOnly]
        private static void ActiveTexture_Lazy(TextureUnit texture)
        {
            _activeTextureDelegate = (delegate* unmanaged<TextureUnit, void>)Sdl.GetProcAddress("glActiveTexture");
            _activeTextureDelegate(texture);
        }

        public static void AttachShader(uint program, uint shader)
        {
            _attachShaderDelegate(program, shader);
        }

        private static delegate* unmanaged<uint, uint, void> _attachShaderDelegate = &AttachShader_Lazy;

        [UnmanagedCallersOnly]
        private static void AttachShader_Lazy(uint program, uint shader)
        {
            _attachShaderDelegate = (delegate* unmanaged<uint, uint, void>)Sdl.GetProcAddress("glAttachShader");
            _attachShaderDelegate(program, shader);
        }

        public static void CompileShader(uint shader)
        {
            _compileShaderDelegate(shader);
        }

        private static delegate* unmanaged<uint, void> _compileShaderDelegate = &CompileShader_Lazy;

        [UnmanagedCallersOnly]
        private static void CompileShader_Lazy(uint shader)
        {
            _compileShaderDelegate = (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glCompileShader");
            _compileShaderDelegate(shader);
        }

        public static uint CreateProgram()
        {
            return _createProgramDelegate();
        }

        private static delegate* unmanaged<uint> _createProgramDelegate = &CreateProgram_Lazy;

        [UnmanagedCallersOnly]
        private static uint CreateProgram_Lazy()
        {
            _createProgramDelegate = (delegate* unmanaged<uint>)Sdl.GetProcAddress("glCreateProgram");
            return _createProgramDelegate();
        }

        public static uint CreateShader(ShaderType type)
        {
            return _createShaderDelegate(type);
        }

        private static delegate* unmanaged<ShaderType, uint> _createShaderDelegate = &CreateShader_Lazy;

        [UnmanagedCallersOnly]
        private static uint CreateShader_Lazy(ShaderType type)
        {
            _createShaderDelegate = (delegate* unmanaged<ShaderType, uint>)Sdl.GetProcAddress("glCreateShader");
            return _createShaderDelegate(type);
        }

        public static void DeleteProgram(uint program)
        {
            _deleteProgramDelegate(program);
        }

        private static delegate* unmanaged<uint, void> _deleteProgramDelegate = &DeleteProgram_Lazy;

        [UnmanagedCallersOnly]
        private static void DeleteProgram_Lazy(uint program)
        {
            _deleteProgramDelegate = (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glDeleteProgram");
            _deleteProgramDelegate(program);
        }

        public static void DeleteShader(uint shader)
        {
            _deleteShaderDelegate(shader);
        }

        private static delegate* unmanaged<uint, void> _deleteShaderDelegate = &DeleteShader_Lazy;

        [UnmanagedCallersOnly]
        private static void DeleteShader_Lazy(uint shader)
        {
            _deleteShaderDelegate = (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glDeleteShader");
            _deleteShaderDelegate(shader);
        }

        public static void DetachShader(uint program, uint shader)
        {
            _detachShaderDelegate(program, shader);
        }

        private static delegate* unmanaged<uint, uint, void> _detachShaderDelegate = &DetachShader_Lazy;

        [UnmanagedCallersOnly]
        private static void DetachShader_Lazy(uint program, uint shader)
        {
            _detachShaderDelegate = (delegate* unmanaged<uint, uint, void>)Sdl.GetProcAddress("glDetachShader");
            _detachShaderDelegate(program, shader);
        }

        public static void LinkProgram(uint program)
        {
            _linkProgramDelegate(program);
        }

        private static delegate* unmanaged<uint, void> _linkProgramDelegate = &LinkProgram_Lazy;

        [UnmanagedCallersOnly]
        private static void LinkProgram_Lazy(uint program)
        {
            _linkProgramDelegate = (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glLinkProgram");
            _linkProgramDelegate(program);
        }

        public static void ShaderSource(uint shader, int count, byte** str, int* length)
        {
            _shaderSourceDelegate(shader, count, str, length);
        }

        private static delegate* unmanaged<uint, int, byte**, int*, void> _shaderSourceDelegate = &ShaderSource_Lazy;

        [UnmanagedCallersOnly]
        private static void ShaderSource_Lazy(uint shader, int count, byte** str, int* length)
        {
            _shaderSourceDelegate =
                (delegate* unmanaged<uint, int, byte**, int*, void>)Sdl.GetProcAddress("glShaderSource");
            _shaderSourceDelegate(shader, count, str, length);
        }

        public static void UseProgram(uint program)
        {
            _useProgramDelegate(program);
        }

        private static delegate* unmanaged<uint, void> _useProgramDelegate = &UseProgram_Lazy;

        [UnmanagedCallersOnly]
        private static void UseProgram_Lazy(uint program)
        {
            _useProgramDelegate = (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glUseProgram");
            _useProgramDelegate(program);
        }

        public static void Uniform1f(int location, float v0)
        {
            _uniform1fDelegate(location, v0);
        }

        private static delegate* unmanaged<int, float, void> _uniform1fDelegate = &Uniform1f__Lazy;

        [UnmanagedCallersOnly]
        private static void Uniform1f__Lazy(int location, float v0)
        {
            _uniform1fDelegate = (delegate* unmanaged<int, float, void>)Sdl.GetProcAddress("glUniform1f");
            _uniform1fDelegate(location, v0);
        }

        public static void Uniform2f(int location, float v0, float v1)
        {
            _uniform2fDelegate(location, v0, v1);
        }

        private static delegate* unmanaged<int, float, float, void> _uniform2fDelegate = &Uniform2f__Lazy;

        [UnmanagedCallersOnly]
        private static void Uniform2f__Lazy(int location, float v0, float v1)
        {
            _uniform2fDelegate = (delegate* unmanaged<int, float, float, void>)Sdl.GetProcAddress("glUniform2f");
            _uniform2fDelegate(location, v0, v1);
        }

        public static void Uniform3f(int location, float v0, float v1, float v2)
        {
            _uniform3fDelegate(location, v0, v1, v2);
        }

        private static delegate* unmanaged<int, float, float, float, void> _uniform3fDelegate = &Uniform3f__Lazy;

        [UnmanagedCallersOnly]
        private static void Uniform3f__Lazy(int location, float v0, float v1, float v2)
        {
            _uniform3fDelegate = (delegate* unmanaged<int, float, float, float, void>)Sdl.GetProcAddress("glUniform3f");
            _uniform3fDelegate(location, v0, v1, v2);
        }

        public static void Uniform4f(int location, float v0, float v1, float v2, float v3)
        {
            _uniform4fDelegate(location, v0, v1, v2, v3);
        }

        private static delegate* unmanaged<int, float, float, float, float, void> _uniform4fDelegate = &Uniform4f__Lazy;

        [UnmanagedCallersOnly]
        private static void Uniform4f__Lazy(int location, float v0, float v1, float v2, float v3)
        {
            _uniform4fDelegate =
                (delegate* unmanaged<int, float, float, float, float, void>)Sdl.GetProcAddress("glUniform4f");
            _uniform4fDelegate(location, v0, v1, v2, v3);
        }

        public static void Uniform1i(int location, int v0)
        {
            _uniform1iDelegate(location, v0);
        }

        private static delegate* unmanaged<int, int, void> _uniform1iDelegate = &Uniform1i__Lazy;

        [UnmanagedCallersOnly]
        private static void Uniform1i__Lazy(int location, int v0)
        {
            _uniform1iDelegate = (delegate* unmanaged<int, int, void>)Sdl.GetProcAddress("glUniform1i");
            _uniform1iDelegate(location, v0);
        }

        public static void Uniform2i(int location, int v0, int v1)
        {
            _uniform2iDelegate(location, v0, v1);
        }

        private static delegate* unmanaged<int, int, int, void> _uniform2iDelegate = &Uniform2i__Lazy;

        [UnmanagedCallersOnly]
        private static void Uniform2i__Lazy(int location, int v0, int v1)
        {
            _uniform2iDelegate = (delegate* unmanaged<int, int, int, void>)Sdl.GetProcAddress("glUniform2i");
            _uniform2iDelegate(location, v0, v1);
        }

        public static void Uniform3i(int location, int v0, int v1, int v2)
        {
            _uniform3iDelegate(location, v0, v1, v2);
        }

        private static delegate* unmanaged<int, int, int, int, void> _uniform3iDelegate = &Uniform3i__Lazy;

        [UnmanagedCallersOnly]
        private static void Uniform3i__Lazy(int location, int v0, int v1, int v2)
        {
            _uniform3iDelegate = (delegate* unmanaged<int, int, int, int, void>)Sdl.GetProcAddress("glUniform3i");
            _uniform3iDelegate(location, v0, v1, v2);
        }

        public static void Uniform4i(int location, int v0, int v1, int v2, int v3)
        {
            _uniform4iDelegate(location, v0, v1, v2, v3);
        }

        private static delegate* unmanaged<int, int, int, int, int, void> _uniform4iDelegate = &Uniform4i__Lazy;

        [UnmanagedCallersOnly]
        private static void Uniform4i__Lazy(int location, int v0, int v1, int v2, int v3)
        {
            _uniform4iDelegate = (delegate* unmanaged<int, int, int, int, int, void>)Sdl.GetProcAddress("glUniform4i");
            _uniform4iDelegate(location, v0, v1, v2, v3);
        }

        private static void UniformMatrix4fv(int location, int count, byte transpose, float* value)
        {
            _uniformMatrix4fvDelegate(location, count, transpose, value);
        }

        private static delegate* unmanaged<int, int, byte, float*, void> _uniformMatrix4fvDelegate =
            &UniformMatrix4fv_Lazy;

        [UnmanagedCallersOnly]
        private static void UniformMatrix4fv_Lazy(int location, int count, byte transpose, float* value)
        {
            _uniformMatrix4fvDelegate =
                (delegate* unmanaged<int, int, byte, float*, void>)Sdl.GetProcAddress("glUniformMatrix4fv");
            _uniformMatrix4fvDelegate(location, count, transpose, value);
        }

        public static int GetAttribLocation(uint program, byte* name)
        {
            return _getAttribLocationDelegate(program, name);
        }

        private static delegate* unmanaged<uint, byte*, int> _getAttribLocationDelegate = &GetAttribLocation_Lazy;

        [UnmanagedCallersOnly]
        private static int GetAttribLocation_Lazy(uint program, byte* name)
        {
            _getAttribLocationDelegate =
                (delegate* unmanaged<uint, byte*, int>)Sdl.GetProcAddress("glGetAttribLocation");
            return _getAttribLocationDelegate(program, name);
        }

        public static void DrawBuffer(DrawBufferMode buf)
        {
            _drawBufferDelegate(buf);
        }

        private static delegate* unmanaged<DrawBufferMode, void> _drawBufferDelegate = &DrawBuffer_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawBuffer_Lazy(DrawBufferMode drawBufferMode)
        {
            _drawBufferDelegate = (delegate* unmanaged<DrawBufferMode, void>)Sdl.GetProcAddress("glDrawBuffer");
            _drawBufferDelegate(drawBufferMode);
        }

        private static void GetProgramiv(uint program, ProgramPropertyARB parameterName, int* parameters)
        {
            _getProgramivDelegate(program, parameterName, parameters);
        }

        private static delegate* unmanaged<uint, ProgramPropertyARB, int*, void> _getProgramivDelegate =
            &GetProgramiv_Lazy;

        [UnmanagedCallersOnly]
        private static void GetProgramiv_Lazy(uint program, ProgramPropertyARB parameterName, int* parameters)
        {
            _getProgramivDelegate =
                (delegate* unmanaged<uint, ProgramPropertyARB, int*, void>)Sdl.GetProcAddress("glGetProgramiv");
            _getProgramivDelegate(program, parameterName, parameters);
        }

        private static void GetProgramInfoLog(uint program, int bufferSize, int* length, byte* infoLog)
        {
            _getProgramInfoLogDelegate(program, bufferSize, length, infoLog);
        }

        private static delegate* unmanaged<uint, int, int*, byte*, void> _getProgramInfoLogDelegate =
            &GetProgramInfoLog_Lazy;

        [UnmanagedCallersOnly]
        private static void GetProgramInfoLog_Lazy(uint program, int bufSize, int* length, byte* infoLog)
        {
            _getProgramInfoLogDelegate =
                (delegate* unmanaged<uint, int, int*, byte*, void>)Sdl.GetProcAddress("glGetProgramInfoLog");
            _getProgramInfoLogDelegate(program, bufSize, length, infoLog);
        }

        private static void GetShaderiv(uint shader, ShaderParameterName pname, int* parameters)
        {
            _getShaderivDelegate(shader, pname, parameters);
        }

        private static delegate* unmanaged<uint, ShaderParameterName, int*, void> _getShaderivDelegate =
            &GetShaderiv_Lazy;

        [UnmanagedCallersOnly]
        private static void GetShaderiv_Lazy(uint shader, ShaderParameterName pname, int* parameters)
        {
            _getShaderivDelegate =
                (delegate* unmanaged<uint, ShaderParameterName, int*, void>)Sdl.GetProcAddress("glGetShaderiv");
            _getShaderivDelegate(shader, pname, parameters);
        }

        private static void GetShaderInfoLog(uint shader, int bufferSize, int* length, byte* infoLog)
        {
            _getShaderInfoLogDelegate(shader, bufferSize, length, infoLog);
        }

        private static delegate* unmanaged<uint, int, int*, byte*, void> _getShaderInfoLogDelegate =
            &GetShaderInfoLog_Lazy;

        [UnmanagedCallersOnly]
        private static void GetShaderInfoLog_Lazy(uint shader, int bufSize, int* length, byte* infoLog)
        {
            _getShaderInfoLogDelegate =
                (delegate* unmanaged<uint, int, int*, byte*, void>)Sdl.GetProcAddress("glGetShaderInfoLog");
            _getShaderInfoLogDelegate(shader, bufSize, length, infoLog);
        }

        public static void GetActiveAttrib(uint program, uint index, int bufSize, int* length, int* size,
            AttributeType* type, byte* name)
        {
            _getActiveAttribDelegate(program, index, bufSize, length, size, type, name);
        }

        private static delegate* unmanaged<uint, uint, int, int*, int*, AttributeType*, byte*, void>
            _getActiveAttribDelegate = &GetActiveAttrib_Lazy;

        [UnmanagedCallersOnly]
        private static void GetActiveAttrib_Lazy(uint program, uint index, int bufSize, int* length, int* size,
            AttributeType* type, byte* name)
        {
            _getActiveAttribDelegate =
                (delegate* unmanaged<uint, uint, int, int*, int*, AttributeType*, byte*, void>)Sdl.GetProcAddress(
                    "glGetActiveAttrib");
            _getActiveAttribDelegate(program, index, bufSize, length, size, type, name);
        }

        private static void GetActiveUniform(uint program, uint index, int bufSize, int* length, int* size,
            UniformType* type, byte* name)
        {
            _getActiveUniformDelegate(program, index, bufSize, length, size, type, name);
        }

        private static delegate* unmanaged<uint, uint, int, int*, int*, UniformType*, byte*, void>
            _getActiveUniformDelegate = &GetActiveUniform_Lazy;

        [UnmanagedCallersOnly]
        private static void GetActiveUniform_Lazy(uint program, uint index, int bufSize, int* length, int* size,
            UniformType* type, byte* name)
        {
            _getActiveUniformDelegate =
                (delegate* unmanaged<uint, uint, int, int*, int*, UniformType*, byte*, void>)Sdl.GetProcAddress(
                    "glGetActiveUniform");
            _getActiveUniformDelegate(program, index, bufSize, length, size, type, name);
        }

        private static int GetUniformLocation(uint program, byte* name)
        {
            return _getUniformLocationDelegate(program, name);
        }

        private static delegate* unmanaged<uint, byte*, int> _getUniformLocationDelegate = &GetUniformLocation_Lazy;

        [UnmanagedCallersOnly]
        private static int GetUniformLocation_Lazy(uint program, byte* name)
        {
            _getUniformLocationDelegate =
                (delegate* unmanaged<uint, byte*, int>)Sdl.GetProcAddress("glGetUniformLocation");
            return _getUniformLocationDelegate(program, name);
        }

        private static void GetActiveUniformBlockName(uint program, uint uniformBlockIndex, int bufSize, int* length,
            byte* uniformBlockName)
        {
            _getActiveUniformBlockNameDelegate(program, uniformBlockIndex, bufSize, length, uniformBlockName);
        }

        private static delegate* unmanaged<uint, uint, int, int*, byte*, void> _getActiveUniformBlockNameDelegate =
            &GetActiveUniformBlockName_Lazy;

        [UnmanagedCallersOnly]
        private static void GetActiveUniformBlockName_Lazy(uint program, uint uniformBlockIndex, int bufSize,
            int* length, byte* uniformBlockName)
        {
            _getActiveUniformBlockNameDelegate =
                (delegate* unmanaged<uint, uint, int, int*, byte*, void>)Sdl.GetProcAddress(
                    "glGetActiveUniformBlockName");
            _getActiveUniformBlockNameDelegate(program, uniformBlockIndex, bufSize, length, uniformBlockName);
        }

        public static void UniformBlockBinding(uint program, uint uniformBlockIndex, uint uniformBlockBinding)
        {
            _uniformBlockBindingDelegate(program, uniformBlockIndex, uniformBlockBinding);
        }

        private static delegate* unmanaged<uint, uint, uint, void> _uniformBlockBindingDelegate =
            &UniformBlockBinding_Lazy;

        [UnmanagedCallersOnly]
        private static void UniformBlockBinding_Lazy(uint program, uint uniformBlockIndex, uint uniformBlockBinding)
        {
            _uniformBlockBindingDelegate =
                (delegate* unmanaged<uint, uint, uint, void>)Sdl.GetProcAddress("glUniformBlockBinding");
            _uniformBlockBindingDelegate(program, uniformBlockIndex, uniformBlockBinding);
        }

        private static void GetActiveUniformBlockiv(uint program, uint uniformBlockIndex,
            UniformBlockPName parameterName, int* parameters)
        {
            _getActiveUniformBlockivDelegate(program, uniformBlockIndex, parameterName, parameters);
        }

        private static delegate* unmanaged<uint, uint, UniformBlockPName, int*, void> _getActiveUniformBlockivDelegate =
            &GetActiveUniformBlockiv_Lazy;

        [UnmanagedCallersOnly]
        private static void GetActiveUniformBlockiv_Lazy(uint program, uint uniformBlockIndex, UniformBlockPName pname,
            int* parameters)
        {
            _getActiveUniformBlockivDelegate =
                (delegate* unmanaged<uint, uint, UniformBlockPName, int*, void>)Sdl.GetProcAddress(
                    "glGetActiveUniformBlockiv");
            _getActiveUniformBlockivDelegate(program, uniformBlockIndex, pname, parameters);
        }

        private static void ObjectLabel(ObjectIdentifier identifier, uint name, int length, byte* label)
        {
            _objectLabelDelegate(identifier, name, length, label);
        }

        private static delegate* unmanaged<ObjectIdentifier, uint, int, byte*, void> _objectLabelDelegate =
            &ObjectLabel_Lazy;

        [UnmanagedCallersOnly]
        private static void ObjectLabel_Lazy(ObjectIdentifier identifier, uint name, int length, byte* label)
        {
            _objectLabelDelegate =
                (delegate* unmanaged<ObjectIdentifier, uint, int, byte*, void>)Sdl.GetProcAddress("glObjectLabel");
            _objectLabelDelegate(identifier, name, length, label);
        }

        private static void CreateTextures(TextureTarget target, int n, uint* textures)
        {
            _createTexturesDelegate(target, n, textures);
        }

        private static delegate* unmanaged<TextureTarget, int, uint*, void> _createTexturesDelegate =
            &CreateTextures_Lazy;

        [UnmanagedCallersOnly]
        private static void CreateTextures_Lazy(TextureTarget target, int n, uint* textures)
        {
            _createTexturesDelegate =
                (delegate* unmanaged<TextureTarget, int, uint*, void>)Sdl.GetProcAddress("glCreateTextures");
            _createTexturesDelegate(target, n, textures);
        }

        public static void TextureStorage1D(uint texture, int levels, SizedInternalFormat internalFormat, int width)
        {
            _textureStorage1DDelegate(texture, levels, internalFormat, width);
        }

        private static delegate* unmanaged<uint, int, SizedInternalFormat, int, void> _textureStorage1DDelegate =
            &TextureStorage1D_Lazy;

        [UnmanagedCallersOnly]
        private static void TextureStorage1D_Lazy(uint texture, int levels, SizedInternalFormat internalFormat,
            int width)
        {
            _textureStorage1DDelegate =
                (delegate* unmanaged<uint, int, SizedInternalFormat, int, void>)Sdl.GetProcAddress(
                    "glTextureStorage1D");
            _textureStorage1DDelegate(texture, levels, internalFormat, width);
        }

        public static void TextureStorage2D(uint texture, int levels, SizedInternalFormat internalFormat, int width,
            int height)
        {
            _textureStorage2DDelegate(texture, levels, internalFormat, width, height);
        }

        private static delegate* unmanaged<uint, int, SizedInternalFormat, int, int, void> _textureStorage2DDelegate =
            &TextureStorage2D_Lazy;

        [UnmanagedCallersOnly]
        private static void TextureStorage2D_Lazy(uint texture, int levels, SizedInternalFormat internalFormat,
            int width, int height)
        {
            _textureStorage2DDelegate =
                (delegate* unmanaged<uint, int, SizedInternalFormat, int, int, void>)Sdl.GetProcAddress(
                    "glTextureStorage2D");
            _textureStorage2DDelegate(texture, levels, internalFormat, width, height);
        }

        public static void TextureStorage3D(uint texture, int levels, SizedInternalFormat internalFormat, int width,
            int height, int depth)
        {
            _textureStorage3DDelegate(texture, levels, internalFormat, width, height, depth);
        }

        private static delegate* unmanaged<uint, int, SizedInternalFormat, int, int, int, void>
            _textureStorage3DDelegate = &TextureStorage3D_Lazy;

        [UnmanagedCallersOnly]
        private static void TextureStorage3D_Lazy(uint texture, int levels, SizedInternalFormat internalFormat,
            int width, int height, int depth)
        {
            _textureStorage3DDelegate =
                (delegate* unmanaged<uint, int, SizedInternalFormat, int, int, int, void>)Sdl.GetProcAddress(
                    "glTextureStorage3D");
            _textureStorage3DDelegate(texture, levels, internalFormat, width, height, depth);
        }

        public static void TextureParameterfv(uint texture, TextureParameterName parameterName, float* param)
        {
            _textureParameterfvDelegate(texture, parameterName, param);
        }

        private static delegate* unmanaged<uint, TextureParameterName, float*, void> _textureParameterfvDelegate =
            &TextureParameterfv_Lazy;

        [UnmanagedCallersOnly]
        private static void TextureParameterfv_Lazy(uint texture, TextureParameterName parameterName, float* param)
        {
            _textureParameterfvDelegate =
                (delegate* unmanaged<uint, TextureParameterName, float*, void>)Sdl.GetProcAddress(
                    "glTextureParameterfv");
            _textureParameterfvDelegate(texture, parameterName, param);
        }

        public static void TextureParameteriv(uint texture, TextureParameterName parameterName, int* param)
        {
            _textureParameterivDelegate(texture, parameterName, param);
        }

        private static delegate* unmanaged<uint, TextureParameterName, int*, void> _textureParameterivDelegate =
            &TextureParameteriv_Lazy;

        [UnmanagedCallersOnly]
        private static void TextureParameteriv_Lazy(uint texture, TextureParameterName parameterName, int* param)
        {
            _textureParameterivDelegate =
                (delegate* unmanaged<uint, TextureParameterName, int*, void>)Sdl.GetProcAddress("glTextureParameteriv");
            _textureParameterivDelegate(texture, parameterName, param);
        }

        public static void GenerateTextureMipmap(uint texture)
        {
            _generateTextureMipmapDelegate(texture);
        }

        private static delegate* unmanaged<uint, void> _generateTextureMipmapDelegate = &GenerateTextureMipmap_Lazy;

        [UnmanagedCallersOnly]
        private static void GenerateTextureMipmap_Lazy(uint texture)
        {
            _generateTextureMipmapDelegate =
                (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glGenerateTextureMipmap");
            _generateTextureMipmapDelegate(texture);
        }

        public static void TextureSubImage1D(uint texture, int level, int xOffset, int width, PixelFormat format,
            PixelType type, void* pixels)
        {
            _textureSubImage1DDelegate(texture, level, xOffset, width, format, type, pixels);
        }

        private static delegate* unmanaged<uint, int, int, int, PixelFormat, PixelType, void*, void>
            _textureSubImage1DDelegate = &TextureSubImage1D_Lazy;

        [UnmanagedCallersOnly]
        private static void TextureSubImage1D_Lazy(uint texture, int level, int xOffset, int width, PixelFormat format,
            PixelType type, void* pixels)
        {
            _textureSubImage1DDelegate =
                (delegate* unmanaged<uint, int, int, int, PixelFormat, PixelType, void*, void>)Sdl.GetProcAddress(
                    "glTextureSubImage1D");
            _textureSubImage1DDelegate(texture, level, xOffset, width, format, type, pixels);
        }

        public static void TextureSubImage2D(uint texture, int level, int xOffset, int yOffset, int width, int height,
            PixelFormat format, PixelType type, void* pixels)
        {
            _textureSubImage2DDelegate(texture, level, xOffset, yOffset, width, height, format, type, pixels);
        }

        private static delegate* unmanaged<uint, int, int, int, int, int, PixelFormat, PixelType, void*, void>
            _textureSubImage2DDelegate = &TextureSubImage2D_Lazy;

        [UnmanagedCallersOnly]
        private static void TextureSubImage2D_Lazy(uint texture, int level, int xOffset, int yOffset, int width,
            int height, PixelFormat format, PixelType type, void* pixels)
        {
            _textureSubImage2DDelegate = (delegate* unmanaged<uint, int, int, int, int, int, PixelFormat, PixelType, void*, void>)Sdl.GetProcAddress("glTextureSubImage2D");
            _textureSubImage2DDelegate(texture, level, xOffset, yOffset, width, height, format, type, pixels);
        }

        public static void TextureSubImage3D(uint texture, int level, int xOffset, int yOffset, int zOffset, int width,
            int height, int depth, PixelFormat format, PixelType type, void* pixels)
        {
            _textureSubImage3DDelegate(texture, level, xOffset, yOffset, zOffset, width, height, depth, format, type,
                pixels);
        }

        private static delegate* unmanaged<uint, int, int, int, int, int, int, int, PixelFormat, PixelType, void*, void>
            _textureSubImage3DDelegate = &TextureSubImage3D_Lazy;

        [UnmanagedCallersOnly]
        private static void TextureSubImage3D_Lazy(
            uint texture,
            int level,
            int xOffset,
            int yOffset,
            int zOffset,
            int width,
            int height,
            int depth,
            PixelFormat format,
            PixelType type,
            void* pixels)
        {
            _textureSubImage3DDelegate = (delegate* unmanaged<uint, int, int, int, int, int, int, int, PixelFormat, PixelType, void*, void>)Sdl.GetProcAddress("glTextureSubImage3D");
            _textureSubImage3DDelegate(texture, level, xOffset, yOffset, zOffset, width, height, depth, format, type, pixels);
        }

        public static void DeleteTextures(int n, uint* textures)
        {
            _deleteTexturesDelegate(n, textures);
        }

        private static delegate* unmanaged<int, uint*, void> _deleteTexturesDelegate = &DeleteTextures_Lazy;

        [UnmanagedCallersOnly]
        private static void DeleteTextures_Lazy(int n, uint* textures)
        {
            _deleteTexturesDelegate = (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glDeleteTextures");
            _deleteTexturesDelegate(n, textures);
        }

        public static void BindTextureUnit(uint unit, uint texture)
        {
            _bindTextureUnitDelegate(unit, texture);
        }

        private static delegate* unmanaged<uint, uint, void> _bindTextureUnitDelegate = &BindTextureUnit_Lazy;

        [UnmanagedCallersOnly]
        private static void BindTextureUnit_Lazy(uint unit, uint texture)
        {
            _bindTextureUnitDelegate = (delegate* unmanaged<uint, uint, void>)Sdl.GetProcAddress("glBindTextureUnit");
            _bindTextureUnitDelegate(unit, texture);
        }

        private static void DebugMessageCallback(IntPtr callback, void* userParam)
        {
            _debugMessageCallbackDelegate(callback, userParam);
        }

        private static delegate* unmanaged<IntPtr, void*, void> _debugMessageCallbackDelegate =
            &DebugMessageCallback_Lazy;

        [UnmanagedCallersOnly]
        private static void DebugMessageCallback_Lazy(IntPtr callback, void* userParam)
        {
            _debugMessageCallbackDelegate =
                (delegate* unmanaged<IntPtr, void*, void>)Sdl.GetProcAddress("glDebugMessageCallback");
            _debugMessageCallbackDelegate(callback, userParam);
        }

        public static void ColorMask(byte red, byte green, byte blue, byte alpha)
        {
            _colorMaskDelegate(red, green, blue, alpha);
        }

        private static delegate* unmanaged<byte, byte, byte, byte, void> _colorMaskDelegate = &ColorMask_Lazy;

        [UnmanagedCallersOnly]
        private static void ColorMask_Lazy(byte red, byte green, byte blue, byte alpha)
        {
            _colorMaskDelegate = (delegate* unmanaged<byte, byte, byte, byte, void>)Sdl.GetProcAddress("glColorMask");
            _colorMaskDelegate(red, green, blue, alpha);
        }

        public static void DepthMask(byte flag)
        {
            _depthMaskDelegate(flag);
        }

        private static delegate* unmanaged<byte, void> _depthMaskDelegate = &DepthMask_Lazy;

        [UnmanagedCallersOnly]
        private static void DepthMask_Lazy(byte flag)
        {
            _depthMaskDelegate = (delegate* unmanaged<byte, void>)Sdl.GetProcAddress("glDepthMask");
            _depthMaskDelegate(flag);
        }

        public static void Disable(EnableCap cap)
        {
            _disableDelegate(cap);
        }

        private static delegate* unmanaged<EnableCap, void> _disableDelegate = &Disable_Lazy;

        [UnmanagedCallersOnly]
        private static void Disable_Lazy(EnableCap cap)
        {
            _disableDelegate = (delegate* unmanaged<EnableCap, void>)Sdl.GetProcAddress("glDisable");
            _disableDelegate(cap);
        }

        public static void Enable(EnableCap cap)
        {
            _enableDelegate(cap);
        }

        private static delegate* unmanaged<EnableCap, void> _enableDelegate = &Enable_Lazy;

        [UnmanagedCallersOnly]
        private static void Enable_Lazy(EnableCap cap)
        {
            _enableDelegate = (delegate* unmanaged<EnableCap, void>)Sdl.GetProcAddress("glEnable");
            _enableDelegate(cap);
        }

        public static void BlendFunc(BlendingFactor sourceBlendingFactor, BlendingFactor destinationBlendingFactor)
        {
            _blendFuncDelegate(sourceBlendingFactor, destinationBlendingFactor);
        }

        private static delegate* unmanaged<BlendingFactor, BlendingFactor, void> _blendFuncDelegate = &BlendFunc_Lazy;

        [UnmanagedCallersOnly]
        private static void BlendFunc_Lazy(BlendingFactor sourceBlendingFactor,
            BlendingFactor destinationBlendingFactor)
        {
            _blendFuncDelegate =
                (delegate* unmanaged<BlendingFactor, BlendingFactor, void>)Sdl.GetProcAddress("glBlendFunc");
            _blendFuncDelegate(sourceBlendingFactor, destinationBlendingFactor);
        }

        private static delegate* unmanaged<BlendEquationMode, void> _blendEquationDelegate = &BlendEquation_Lazy;

        public static void BlendEquation(BlendEquationMode mode)
        {
            _blendEquationDelegate(mode);
        }

        [UnmanagedCallersOnly]
        private static void BlendEquation_Lazy(BlendEquationMode mode)
        {
            _blendEquationDelegate =
                (delegate* unmanaged<BlendEquationMode, void>)Sdl.GetProcAddress("glBlendEquation");
            _blendEquationDelegate(mode);
        }

        public static void VertexArrayBindingDivisor(uint vao, uint bindingIndex, uint divisor)
        {
            _vertexArrayBindingDivisorDelegate(vao, bindingIndex, divisor);
        }

        private static delegate* unmanaged<uint, uint, uint, void> _vertexArrayBindingDivisorDelegate =
            &VertexArrayBindingDivisor_Lazy;

        [UnmanagedCallersOnly]
        private static void VertexArrayBindingDivisor_Lazy(uint vao, uint bindingIndex, uint divisor)
        {
            _vertexArrayBindingDivisorDelegate =
                (delegate* unmanaged<uint, uint, uint, void>)Sdl.GetProcAddress("glVertexArrayBindingDivisor");
            _vertexArrayBindingDivisorDelegate(vao, bindingIndex, divisor);
        }

        public static void DrawArraysIndirect(PrimitiveType mode, void* indirect)
            => _drawArraysIndirectDelegate(mode, indirect);
        private static delegate* unmanaged<PrimitiveType, void*, void> _drawArraysIndirectDelegate = &DrawArraysIndirect_Lazy;
        [UnmanagedCallersOnly]
        private static void DrawArraysIndirect_Lazy(PrimitiveType mode, void* indirect)
        {
            _drawArraysIndirectDelegate = (delegate* unmanaged<PrimitiveType, void*, void>)Sdl.GetProcAddress("glDrawArraysIndirect");
            _drawArraysIndirectDelegate(mode, indirect);
        }

        public static void DrawElementsIndirect(PrimitiveType mode, DrawElementsType type, void* indirect)
            => _drawElementsIndirectDelegate(mode, type, indirect);
        private static delegate* unmanaged<PrimitiveType, DrawElementsType, void*, void> _drawElementsIndirectDelegate = &DrawElementsIndirect_Lazy;
        [UnmanagedCallersOnly]
        private static void DrawElementsIndirect_Lazy(PrimitiveType mode, DrawElementsType type, void* indirect)
        {
            _drawElementsIndirectDelegate = (delegate* unmanaged<PrimitiveType, DrawElementsType, void*, void>)Sdl.GetProcAddress("glDrawElementsIndirect");
            _drawElementsIndirectDelegate(mode, type, indirect);
        }

        public static void DrawArraysInstanced(PrimitiveType mode, int first, int count, int instanceCount)
        {
            _drawArraysInstancedDelegate(mode, first, count, instanceCount);
        }

        private static delegate* unmanaged<PrimitiveType, int, int, int, void> _drawArraysInstancedDelegate =
            &DrawArraysInstanced_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawArraysInstanced_Lazy(PrimitiveType mode, int first, int count, int instanceCount)
        {
            _drawArraysInstancedDelegate =
                (delegate* unmanaged<PrimitiveType, int, int, int, void>)Sdl.GetProcAddress("glDrawArraysInstanced");
            _drawArraysInstancedDelegate(mode, first, count, instanceCount);
        }

        public static void DrawElementsInstanced(
            PrimitiveType mode,
            int count,
            DrawElementsType type,
            int indexOffset,
            int instanceCount)
        {
            _drawElementsInstancedDelegate(mode, count, type, indexOffset, instanceCount);
        }

        private static delegate* unmanaged<PrimitiveType, int, DrawElementsType, int, int, void>
            _drawElementsInstancedDelegate = &DrawElementsInstanced_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawElementsInstanced_Lazy(PrimitiveType mode, int count, DrawElementsType type,
            int indicesOffset, int instanceCount)
        {
            _drawElementsInstancedDelegate =
                (delegate* unmanaged<PrimitiveType, int, DrawElementsType, int, int, void>)Sdl.GetProcAddress(
                    "glDrawElementsInstanced");
            _drawElementsInstancedDelegate(mode, count, type, indicesOffset, instanceCount);
        }

        public static void MultiDrawArraysIndirect(PrimitiveType primitiveType, void* indirect, int indirectDrawCount, int indirectStride)
            => _multiDrawArraysIndirectDelegate(primitiveType, indirect, indirectDrawCount, indirectStride);
        private static delegate* unmanaged<PrimitiveType, void*, int, int, void> _multiDrawArraysIndirectDelegate = &MultiDrawArraysIndirect_Lazy;
        [UnmanagedCallersOnly]
        private static void MultiDrawArraysIndirect_Lazy(PrimitiveType mode, void* indirect, int drawcount, int stride)
        {
            _multiDrawArraysIndirectDelegate = (delegate* unmanaged<PrimitiveType, void*, int, int, void>)Sdl.GetProcAddress("glMultiDrawArraysIndirect");
            _multiDrawArraysIndirectDelegate(mode, indirect, drawcount, stride);
        }

        public static void MultiDrawElementsIndirect(PrimitiveType primitiveType, DrawElementsType elementsType, void* indirect, int indirectDrawCount, int indirectStride)
            => _multiDrawElementsIndirectDelegate(primitiveType, elementsType, indirect, indirectDrawCount, indirectStride);
        private static delegate* unmanaged<PrimitiveType, DrawElementsType, void*, int, int, void> _multiDrawElementsIndirectDelegate = &MultiDrawElementsIndirect_Lazy;
        [UnmanagedCallersOnly]
        private static void MultiDrawElementsIndirect_Lazy(PrimitiveType mode, DrawElementsType type, void* indirect, int drawcount, int stride)
        {
            _multiDrawElementsIndirectDelegate = (delegate* unmanaged<PrimitiveType, DrawElementsType, void*, int, int, void>)Sdl.GetProcAddress("glMultiDrawElementsIndirect");
            _multiDrawElementsIndirectDelegate(mode, type, indirect, drawcount, stride);
        }

        public static void GetProgramInterfaceiv(uint program, ProgramInterface programInterface,
            ProgramInterfacePName parameterName, int* parameters)
        {
            _getProgramInterfaceivDelegate(program, programInterface, parameterName, parameters);
        }

        private static delegate* unmanaged<uint, ProgramInterface, ProgramInterfacePName, int*, void>
            _getProgramInterfaceivDelegate = &GetProgramInterfaceiv_Lazy;

        [UnmanagedCallersOnly]
        private static void GetProgramInterfaceiv_Lazy(uint program, ProgramInterface programInterface,
            ProgramInterfacePName parameterName, int* parameters)
        {
            _getProgramInterfaceivDelegate =
                (delegate* unmanaged<uint, ProgramInterface, ProgramInterfacePName, int*, void>)Sdl.GetProcAddress(
                    "glGetProgramInterfaceiv");
            _getProgramInterfaceivDelegate(program, programInterface, parameterName, parameters);
        }

        public static void GetProgramResourceName(uint program, ProgramInterface programInterface, uint index,
            int bufferSize, int* length, byte* name)
        {
            _getProgramResourceNameDelegate(program, programInterface, index, bufferSize, length, name);
        }

        private static delegate* unmanaged<uint, ProgramInterface, uint, int, int*, byte*, void>
            _getProgramResourceNameDelegate = &GetProgramResourceName_Lazy;

        [UnmanagedCallersOnly]
        private static void GetProgramResourceName_Lazy(uint program, ProgramInterface programInterface, uint index,
            int bufferSize, int* length, byte* name)
        {
            _getProgramResourceNameDelegate =
                (delegate* unmanaged<uint, ProgramInterface, uint, int, int*, byte*, void>)Sdl.GetProcAddress(
                    "glGetProgramResourceName");
            _getProgramResourceNameDelegate(program, programInterface, index, bufferSize, length, name);
        }

        public static int GetProgramResourceLocation(uint program, ProgramInterface programInterface, byte* name)
        {
            return _getProgramResourceLocationDelegate(program, programInterface, name);
        }

        private static delegate* unmanaged<uint, ProgramInterface, byte*, int> _getProgramResourceLocationDelegate =
            &GetProgramResourceLocation_Lazy;

        [UnmanagedCallersOnly]
        private static int GetProgramResourceLocation_Lazy(uint program, ProgramInterface programInterface, byte* name)
        {
            _getProgramResourceLocationDelegate =
                (delegate* unmanaged<uint, ProgramInterface, byte*, int>)Sdl.GetProcAddress(
                    "glGetProgramResourceLocation");
            return _getProgramResourceLocationDelegate(program, programInterface, name);
        }

        public static int GetProgramResourceLocationIndex(uint program, ProgramInterface programInterface, byte* name)
        {
            return _getProgramResourceLocationIndexDelegate(program, programInterface, name);
        }

        private static delegate* unmanaged<uint, ProgramInterface, byte*, int>
            _getProgramResourceLocationIndexDelegate = &GetProgramResourceLocationIndex_Lazy;

        [UnmanagedCallersOnly]
        private static int GetProgramResourceLocationIndex_Lazy(uint program, ProgramInterface programInterface,
            byte* name)
        {
            _getProgramResourceLocationIndexDelegate =
                (delegate* unmanaged<uint, ProgramInterface, byte*, int>)Sdl.GetProcAddress(
                    "glGetProgramResourceLocationIndex");
            return _getProgramResourceLocationIndexDelegate(program, programInterface, name);
        }

        public static void GetProgramResourceiv(uint program, ProgramInterface programInterface, uint index,
            int propCount, ProgramResourceProperty* properties, int count, int* length, int* parameters)
        {
            _getProgramResourceivDelegate(program, programInterface, index, propCount, properties, count, length,
                parameters);
        }

        private static delegate* unmanaged<uint, ProgramInterface, uint, int, ProgramResourceProperty*, int, int*, int*,
            void> _getProgramResourceivDelegate = &GetProgramResourceiv_Lazy;

        [UnmanagedCallersOnly]
        private static void GetProgramResourceiv_Lazy(uint program, ProgramInterface programInterface, uint index,
            int propCount, ProgramResourceProperty* properties, int count, int* length, int* parameters)
        {
            _getProgramResourceivDelegate =
                (delegate* unmanaged<uint, ProgramInterface, uint, int, ProgramResourceProperty*, int, int*, int*, void
                    >)Sdl.GetProcAddress("glGetProgramResourceiv");
            _getProgramResourceivDelegate(program, programInterface, index, propCount, properties, count, length,
                parameters);
        }

        public static void Scissor(int x, int y, int width, int height)
        {
            _scissorDelegate(x, y, width, height);
        }

        private static delegate* unmanaged<int, int, int, int, void> _scissorDelegate = &Scissor_Lazy;

        [UnmanagedCallersOnly]
        private static void Scissor_Lazy(int x, int y, int width, int height)
        {
            _scissorDelegate = (delegate* unmanaged<int, int, int, int, void>)Sdl.GetProcAddress("glScissor");
            _scissorDelegate(x, y, width, height);
        }

        public static void CullFace(CullFaceMode mode)
        {
            _cullFaceDelegate(mode);
        }

        private static delegate* unmanaged<CullFaceMode, void> _cullFaceDelegate = &CullFace_Lazy;

        [UnmanagedCallersOnly]
        private static void CullFace_Lazy(CullFaceMode mode)
        {
            _cullFaceDelegate = (delegate* unmanaged<CullFaceMode, void>)Sdl.GetProcAddress("glCullFace");
            _cullFaceDelegate(mode);
        }

        public static void FrontFace(FrontFaceDirection mode)
        {
            _frontFaceDelegate(mode);
        }

        private static delegate* unmanaged<FrontFaceDirection, void> _frontFaceDelegate = &FrontFace_Lazy;

        [UnmanagedCallersOnly]
        private static void FrontFace_Lazy(FrontFaceDirection mode)
        {
            _frontFaceDelegate = (delegate* unmanaged<FrontFaceDirection, void>)Sdl.GetProcAddress("glFrontFace");
            _frontFaceDelegate(mode);
        }

        private static void TexImage3D(TextureTarget target, int level, int internalformat, int width, int height,
            int depth, int border, PixelFormat format, PixelType type, void* pixels)
        {
            _texImage3DDelegate(target, level, internalformat, width, height, depth, border, format, type, pixels);
        }

        private static delegate* unmanaged<TextureTarget, int, int, int, int, int, int, PixelFormat, PixelType, void*,
            void> _texImage3DDelegate = &TexImage3D_Lazy;

        [UnmanagedCallersOnly]
        private static void TexImage3D_Lazy(
            TextureTarget target,
            int level,
            int internalformat,
            int width,
            int height,
            int depth,
            int border,
            PixelFormat format,
            PixelType type,
            void* pixels)
        {
            _texImage3DDelegate = (delegate* unmanaged<TextureTarget, int, int, int, int, int, int, PixelFormat, PixelType, void*, void>)Sdl.GetProcAddress("glTexImage3D");
            _texImage3DDelegate(target, level, internalformat, width, height, depth, border, format, type, pixels);
        }

        private static void CreateFramebuffers(int count, uint* framebuffers)
        {
            _createFramebuffersDelegate(count, framebuffers);
        }

        private static delegate* unmanaged<int, uint*, void> _createFramebuffersDelegate = &CreateFramebuffers_Lazy;

        [UnmanagedCallersOnly]
        private static void CreateFramebuffers_Lazy(int count, uint* framebuffers)
        {
            _createFramebuffersDelegate =
                (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glCreateFramebuffers");
            _createFramebuffersDelegate(count, framebuffers);
        }

        public static void NamedFramebufferTexture(uint framebuffer, FramebufferAttachment attachment, uint texture,
            int level)
        {
            _namedFramebufferTextureDelegate(framebuffer, attachment, texture, level);
        }

        private static delegate* unmanaged<uint, FramebufferAttachment, uint, int, void>
            _namedFramebufferTextureDelegate = &NamedFramebufferTexture_Lazy;

        [UnmanagedCallersOnly]
        private static void NamedFramebufferTexture_Lazy(uint framebuffer, FramebufferAttachment attachment,
            uint texture, int level)
        {
            _namedFramebufferTextureDelegate =
                (delegate* unmanaged<uint, FramebufferAttachment, uint, int, void>)Sdl.GetProcAddress(
                    "glNamedFramebufferTexture");
            _namedFramebufferTextureDelegate(framebuffer, attachment, texture, level);
        }

        private static void DeleteFramebuffers(int n, uint* framebuffers)
        {
            _deleteFramebuffersDelegate(n, framebuffers);
        }

        private static delegate* unmanaged<int, uint*, void> _deleteFramebuffersDelegate = &DeleteFramebuffers_Lazy;

        [UnmanagedCallersOnly]
        private static void DeleteFramebuffers_Lazy(int n, uint* framebuffers)
        {
            _deleteFramebuffersDelegate =
                (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glDeleteFramebuffers");
            _deleteFramebuffersDelegate(n, framebuffers);
        }

        public static FramebufferStatus CheckNamedFramebufferStatus(uint framebuffer, FramebufferTarget target)
        {
            return _checkNamedFramebufferStatusDelegate(framebuffer, target);
        }

        private static delegate* unmanaged<uint, FramebufferTarget, FramebufferStatus>
            _checkNamedFramebufferStatusDelegate = &CheckNamedFramebufferStatus_Lazy;

        [UnmanagedCallersOnly]
        private static FramebufferStatus CheckNamedFramebufferStatus_Lazy(uint framebuffer, FramebufferTarget target)
        {
            _checkNamedFramebufferStatusDelegate =
                (delegate* unmanaged<uint, FramebufferTarget, FramebufferStatus>)Sdl.GetProcAddress(
                    "glCheckNamedFramebufferStatus");
            return _checkNamedFramebufferStatusDelegate(framebuffer, target);
        }

        public static void BindFramebuffer(FramebufferTarget target, uint framebuffer)
        {
            _bindFramebufferDelegate(target, framebuffer);
        }

        private static delegate* unmanaged<FramebufferTarget, uint, void> _bindFramebufferDelegate =
            &BindFramebuffer_Lazy;

        [UnmanagedCallersOnly]
        private static void BindFramebuffer_Lazy(FramebufferTarget target, uint framebuffer)
        {
            _bindFramebufferDelegate =
                (delegate* unmanaged<FramebufferTarget, uint, void>)Sdl.GetProcAddress("glBindFramebuffer");
            _bindFramebufferDelegate(target, framebuffer);
        }

        public static void ClearNamedFramebufferfi(uint framebuffer, Buffer buffer, int drawBuffer, float depth,
            int stencil)
        {
            _clearNamedFramebufferfiDelegate(framebuffer, buffer, drawBuffer, depth, stencil);
        }

        private static delegate* unmanaged<uint, Buffer, int, float, int, void> _clearNamedFramebufferfiDelegate =
            &ClearNamedFramebufferfi_Lazy;

        [UnmanagedCallersOnly]
        private static void ClearNamedFramebufferfi_Lazy(uint framebuffer, Buffer buffer, int drawBuffer, float depth,
            int stencil)
        {
            _clearNamedFramebufferfiDelegate =
                (delegate* unmanaged<uint, Buffer, int, float, int, void>)Sdl.GetProcAddress(
                    "glClearNamedFramebufferfi");
            _clearNamedFramebufferfiDelegate(framebuffer, buffer, drawBuffer, depth, stencil);
        }

        public static void Finish()
        {
            _finishDelegate();
        }

        private static delegate* unmanaged<void> _finishDelegate = &Finish_Lazy;

        [UnmanagedCallersOnly]
        private static void Finish_Lazy()
        {
            _finishDelegate = (delegate* unmanaged<void>)Sdl.GetProcAddress("glFinish");
            _finishDelegate();
        }

        public static void Flush()
        {
            _flushDelegate();
        }

        private static delegate* unmanaged<void> _flushDelegate = &Flush_Lazy;

        [UnmanagedCallersOnly]
        private static void Flush_Lazy()
        {
            _flushDelegate = (delegate* unmanaged<void>)Sdl.GetProcAddress("glFlush");
            _flushDelegate();
        }

        public static void ClearNamedFramebufferiv(uint framebuffer, Buffer buffer, int drawBuffer, int* value)
        {
            _clearNamedFramebufferivDelegate(framebuffer, buffer, drawBuffer, value);
        }

        private static delegate* unmanaged<uint, Buffer, int, int*, void> _clearNamedFramebufferivDelegate =
            &ClearNamedFramebufferiv_Lazy;

        [UnmanagedCallersOnly]
        private static void ClearNamedFramebufferiv_Lazy(uint framebuffer, Buffer buffer, int drawBuffer, int* value)
        {
            _clearNamedFramebufferivDelegate =
                (delegate* unmanaged<uint, Buffer, int, int*, void>)Sdl.GetProcAddress("glClearNamedFramebufferiv");
            _clearNamedFramebufferivDelegate(framebuffer, buffer, drawBuffer, value);
        }

        public static void ClearNamedFramebufferfv(uint framebuffer, Buffer buffer, int drawBuffer, float* value)
        {
            _clearNamedFramebufferfvDelegate(framebuffer, buffer, drawBuffer, value);
        }

        private static delegate* unmanaged<uint, Buffer, int, float*, void> _clearNamedFramebufferfvDelegate =
            &ClearNamedFramebufferfv_Lazy;

        [UnmanagedCallersOnly]
        private static void ClearNamedFramebufferfv_Lazy(uint framebuffer, Buffer buffer, int drawBuffer, float* value)
        {
            _clearNamedFramebufferfvDelegate =
                (delegate* unmanaged<uint, Buffer, int, float*, void>)Sdl.GetProcAddress("glClearNamedFramebufferfv");
            _clearNamedFramebufferfvDelegate(framebuffer, buffer, drawBuffer, value);
        }

        public static void BlitNamedFramebuffer(uint readFramebuffer, uint drawFramebuffer, int srcX0, int srcY0,
            int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask,
            BlitFramebufferFilter filter)
        {
            _blitNamedFramebufferDelegate(readFramebuffer, drawFramebuffer, srcX0, srcY0, srcX1, srcY1, dstX0, dstY0,
                dstX1, dstY1, mask, filter);
        }

        private static delegate* unmanaged<uint, uint, int, int, int, int, int, int, int, int, ClearBufferMask,
            BlitFramebufferFilter, void> _blitNamedFramebufferDelegate = &BlitNamedFramebuffer_Lazy;

        [UnmanagedCallersOnly]
        private static void BlitNamedFramebuffer_Lazy(uint readFramebuffer, uint drawFramebuffer, int srcX0, int srcY0,
            int srcX1, int srcY1, int dstX0, int dstY0, int dstX1, int dstY1, ClearBufferMask mask,
            BlitFramebufferFilter filter)
        {
            _blitNamedFramebufferDelegate =
                (delegate* unmanaged<uint, uint, int, int, int, int, int, int, int, int, ClearBufferMask,
                    BlitFramebufferFilter, void>)Sdl.GetProcAddress("glBlitNamedFramebuffer");
            _blitNamedFramebufferDelegate(readFramebuffer, drawFramebuffer, srcX0, srcY0, srcX1, srcY1, dstX0, dstY0,
                dstX1, dstY1, mask, filter);
        }

        public static void DepthFunc(DepthFunction depthFunction)
        {
            _depthFuncDelegate(depthFunction);
        }

        private static delegate* unmanaged<DepthFunction, void> _depthFuncDelegate = &DepthFunc_Lazy;

        [UnmanagedCallersOnly]
        private static void DepthFunc_Lazy(DepthFunction depthFunction)
        {
            _depthFuncDelegate = (delegate* unmanaged<DepthFunction, void>)Sdl.GetProcAddress("glDepthFunc");
            _depthFuncDelegate(depthFunction);
        }

        public static void NamedFramebufferDrawBuffer(uint framebuffer, ColorBuffer buf)
            => _namedFramebufferDrawBufferDelegate(framebuffer, buf);
        private static delegate* unmanaged<uint, ColorBuffer, void> _namedFramebufferDrawBufferDelegate = &NamedFramebufferDrawBuffer_Lazy;
        [UnmanagedCallersOnly]
        private static void NamedFramebufferDrawBuffer_Lazy(uint framebuffer, ColorBuffer buf)
        {
            _namedFramebufferDrawBufferDelegate = (delegate* unmanaged<uint, ColorBuffer, void>)Sdl.GetProcAddress("glNamedFramebufferDrawBuffer");
            _namedFramebufferDrawBufferDelegate(framebuffer, buf);
        }

        private static void NamedFramebufferDrawBuffers(uint framebuffer, int n, ColorBuffer* buffers)
        {
            _namedFramebufferDrawBuffersDelegate(framebuffer, n, buffers);
        }

        private static delegate* unmanaged<uint, int, ColorBuffer*, void> _namedFramebufferDrawBuffersDelegate =
            &NamedFramebufferDrawBuffers_Lazy;

        [UnmanagedCallersOnly]
        private static void NamedFramebufferDrawBuffers_Lazy(uint framebuffer, int n, ColorBuffer* buffers)
        {
            _namedFramebufferDrawBuffersDelegate =
                (delegate* unmanaged<uint, int, ColorBuffer*, void>)Sdl.GetProcAddress("glNamedFramebufferDrawBuffers");
            _namedFramebufferDrawBuffersDelegate(framebuffer, n, buffers);
        }

        public static void NamedFramebufferReadBuffer(uint framebuffer, ColorBuffer src)
            => _namedFramebufferReadBufferDelegate(framebuffer, src);
        private static delegate* unmanaged<uint, ColorBuffer, void> _namedFramebufferReadBufferDelegate = &NamedFramebufferReadBuffer_Lazy;
        [UnmanagedCallersOnly]
        private static void NamedFramebufferReadBuffer_Lazy(uint framebuffer, ColorBuffer src)
        {
            _namedFramebufferReadBufferDelegate = (delegate* unmanaged<uint, ColorBuffer, void>)Sdl.GetProcAddress("glNamedFramebufferReadBuffer");
            _namedFramebufferReadBufferDelegate(framebuffer, src);
        }

        public static void DrawArraysInstancedBaseInstance(PrimitiveType mode, int first, int count, int instanceCount,
            uint baseInstance)
        {
            _drawArraysInstancedBaseInstanceDelegate(mode, first, count, instanceCount, baseInstance);
        }

        private static delegate* unmanaged<PrimitiveType, int, int, int, uint, void>
            _drawArraysInstancedBaseInstanceDelegate = &DrawArraysInstancedBaseInstance_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawArraysInstancedBaseInstance_Lazy(PrimitiveType mode, int first, int count,
            int instanceCount, uint baseInstance)
        {
            _drawArraysInstancedBaseInstanceDelegate =
                (delegate* unmanaged<PrimitiveType, int, int, int, uint, void>)Sdl.GetProcAddress(
                    "glDrawArraysInstancedBaseInstance");
            _drawArraysInstancedBaseInstanceDelegate(mode, first, count, instanceCount, baseInstance);
        }

        public static void DrawElementsBaseVertex(
            PrimitiveType primitiveType,
            int elementCount,
            DrawElementsType elementsType,
            int indices,
            int baseVertex)
        {
            _drawElementsBaseVertexDelegate(primitiveType, elementCount, elementsType, indices, baseVertex);
        }

        private static delegate* unmanaged<PrimitiveType, int, DrawElementsType, int, int, void>
            _drawElementsBaseVertexDelegate = &DrawElementsBaseVertex_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawElementsBaseVertex_Lazy(PrimitiveType mode, int count, DrawElementsType type,
            int indices, int baseVertex)
        {
            _drawElementsBaseVertexDelegate =
                (delegate* unmanaged<PrimitiveType, int, DrawElementsType, int, int, void>)Sdl.GetProcAddress(
                    "glDrawElementsBaseVertex");
            _drawElementsBaseVertexDelegate(mode, count, type, indices, baseVertex);
        }

        public static void DrawElementsInstancedBaseVertex(PrimitiveType primitiveType, int elementCount, DrawElementsType elementsType,
            void* indices, int instanceCount, int baseVertex)
        {
            _drawElementsInstancedBaseVertexDelegate(primitiveType, elementCount, elementsType, indices, instanceCount, baseVertex);
        }

        private static delegate* unmanaged<PrimitiveType, int, DrawElementsType, void*, int, int, void>
            _drawElementsInstancedBaseVertexDelegate = &DrawElementsInstancedBaseVertex_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawElementsInstancedBaseVertex_Lazy(PrimitiveType mode, int count, DrawElementsType type,
            void* indices, int instanceCount, int baseVertex)
        {
            _drawElementsInstancedBaseVertexDelegate =
                (delegate* unmanaged<PrimitiveType, int, DrawElementsType, void*, int, int, void>)Sdl.GetProcAddress(
                    "glDrawElementsInstancedBaseVertex");
            _drawElementsInstancedBaseVertexDelegate(mode, count, type, indices, instanceCount, baseVertex);
        }

        public static void ProgramParameteri(uint program, ProgramParameterPName programParameterName, int value)
        {
            _programParameteriDelegate(program, programParameterName, value);
        }

        private static delegate* unmanaged<uint, ProgramParameterPName, int, void> _programParameteriDelegate =
            &ProgramParameteri_Lazy;

        [UnmanagedCallersOnly]
        private static void ProgramParameteri_Lazy(uint program, ProgramParameterPName programParameterName, int value)
        {
            _programParameteriDelegate =
                (delegate* unmanaged<uint, ProgramParameterPName, int, void>)Sdl.GetProcAddress("glProgramParameteri");
            _programParameteriDelegate(program, programParameterName, value);
        }

        private static void CreateProgramPipelines(int n, uint* pipelines)
        {
            _createProgramPipelinesDelegate(n, pipelines);
        }

        private static delegate* unmanaged<int, uint*, void> _createProgramPipelinesDelegate =
            &CreateProgramPipelines_Lazy;

        [UnmanagedCallersOnly]
        private static void CreateProgramPipelines_Lazy(int n, uint* pipelines)
        {
            _createProgramPipelinesDelegate =
                (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glCreateProgramPipelines");
            _createProgramPipelinesDelegate(n, pipelines);
        }

        private static uint CreateShaderProgramv(ShaderType type, int count, byte** strings)
        {
            return _createShaderProgramvDelegate(type, count, strings);
        }

        private static delegate* unmanaged<ShaderType, int, byte**, uint> _createShaderProgramvDelegate =
            &CreateShaderProgramv_Lazy;

        [UnmanagedCallersOnly]
        private static uint CreateShaderProgramv_Lazy(ShaderType type, int count, byte** strings)
        {
            _createShaderProgramvDelegate =
                (delegate* unmanaged<ShaderType, int, byte**, uint>)Sdl.GetProcAddress("glCreateShaderProgramv");
            return _createShaderProgramvDelegate(type, count, strings);
        }

        public static void BindProgramPipeline(uint pipeline)
        {
            _bindProgramPipelineDelegate(pipeline);
        }

        private static delegate* unmanaged<uint, void> _bindProgramPipelineDelegate = &BindProgramPipeline_Lazy;

        [UnmanagedCallersOnly]
        private static void BindProgramPipeline_Lazy(uint pipeline)
        {
            _bindProgramPipelineDelegate = (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glBindProgramPipeline");
            _bindProgramPipelineDelegate(pipeline);
        }

        public static void DeleteProgramPipelines(int n, uint* pipelines)
        {
            _deleteProgramPipelinesDelegate(n, pipelines);
        }

        private static delegate* unmanaged<int, uint*, void> _deleteProgramPipelinesDelegate =
            &DeleteProgramPipelines_Lazy;

        [UnmanagedCallersOnly]
        private static void DeleteProgramPipelines_Lazy(int n, uint* pipelines)
        {
            _deleteProgramPipelinesDelegate =
                (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glDeleteProgramPipelines");
            _deleteProgramPipelinesDelegate(n, pipelines);
        }

        public static void UseProgramStages(uint pipeline, UseProgramStageMask stages, uint program)
        {
            _useProgramStagesDelegate(pipeline, stages, program);
        }

        private static delegate* unmanaged<uint, UseProgramStageMask, uint, void> _useProgramStagesDelegate =
            &UseProgramStages_Lazy;

        [UnmanagedCallersOnly]
        private static void UseProgramStages_Lazy(uint pipeline, UseProgramStageMask stages, uint program)
        {
            _useProgramStagesDelegate =
                (delegate* unmanaged<uint, UseProgramStageMask, uint, void>)Sdl.GetProcAddress("glUseProgramStages");
            _useProgramStagesDelegate(pipeline, stages, program);
        }

        public static void ProgramUniform1i(uint program, int location, int value)
        {
            _programUniform1iDelegate(program, location, value);
        }

        private static delegate* unmanaged<uint, int, int, void> _programUniform1iDelegate = &ProgramUniform1i_Lazy;

        [UnmanagedCallersOnly]
        private static void ProgramUniform1i_Lazy(uint program, int location, int value)
        {
            _programUniform1iDelegate =
                (delegate* unmanaged<uint, int, int, void>)Sdl.GetProcAddress("glProgramUniform1i");
            _programUniform1iDelegate(program, location, value);
        }

        private static delegate* unmanaged<uint, int, int, int*, void> _programUniform2ivDelegate = &ProgramUniform2iv_Lazy;
        public static void ProgramUniform2iv(uint program, int location, int count, int* value)
            => _programUniform2ivDelegate(program, location, count, value);
        [UnmanagedCallersOnly]
        private static void ProgramUniform2iv_Lazy(uint program, int location, int count, int* value)
        {
            _programUniform2ivDelegate = (delegate* unmanaged<uint, int, int, int*, void>)Sdl.GetProcAddress("glProgramUniform2iv");
            _programUniform2ivDelegate(program, location, count, value);
        }

        public static void ProgramUniform1f(uint program, int location, float value)
        {
            _programUniform1fDelegate(program, location, value);
        }

        private static delegate* unmanaged<uint, int, float, void> _programUniform1fDelegate = &ProgramUniform1f_Lazy;

        [UnmanagedCallersOnly]
        private static void ProgramUniform1f_Lazy(uint program, int location, float value)
        {
            _programUniform1fDelegate =
                (delegate* unmanaged<uint, int, float, void>)Sdl.GetProcAddress("glProgramUniform1f");
            _programUniform1fDelegate(program, location, value);
        }

        public static void ProgramUniform2f(uint program, int location, float v0, float v1)
        {
            _programUniform2fDelegate(program, location, v0, v1);
        }

        private static delegate* unmanaged<uint, int, float, float, void> _programUniform2fDelegate =
            &ProgramUniform2f_Lazy;

        [UnmanagedCallersOnly]
        private static void ProgramUniform2f_Lazy(uint program, int location, float v0, float v1)
        {
            _programUniform2fDelegate =
                (delegate* unmanaged<uint, int, float, float, void>)Sdl.GetProcAddress("glProgramUniform2f");
            _programUniform2fDelegate(program, location, v0, v1);
        }

        private static delegate* unmanaged<uint, int, float, float, float, void> _programUniform3fDelegate =
            &ProgramUniform3f_Lazy;

        public static void ProgramUniform3f(uint program, int location, float v0, float v1, float v2)
        {
            _programUniform3fDelegate(program, location, v0, v1, v2);
        }

        [UnmanagedCallersOnly]
        private static void ProgramUniform3f_Lazy(uint program, int location, float v0, float v1, float v2)
        {
            _programUniform3fDelegate =
                (delegate* unmanaged<uint, int, float, float, float, void>)Sdl.GetProcAddress("glProgramUniform3f");
            _programUniform3fDelegate(program, location, v0, v1, v2);
        }

        private static delegate* unmanaged<uint, int, float, float, float, float, void> _programUniform4fDelegate =
            &ProgramUniform4f_Lazy;

        public static void ProgramUniform4f(uint program, int location, float v0, float v1, float v2, float v3)
        {
            _programUniform4fDelegate(program, location, v0, v1, v2, v3);
        }

        [UnmanagedCallersOnly]
        private static void ProgramUniform4f_Lazy(uint program, int location, float v0, float v1, float v2, float v3)
        {
            _programUniform4fDelegate =
                (delegate* unmanaged<uint, int, float, float, float, float, void>)Sdl.GetProcAddress(
                    "glProgramUniform4f");
            _programUniform4fDelegate(program, location, v0, v1, v2, v3);
        }

        public static void ValidateProgramPipeline(uint pipeline)
        {
            _validateProgramPipelineDelegate(pipeline);
        }

        private static delegate* unmanaged<uint, void> _validateProgramPipelineDelegate = &ValidateProgramPipeline_Lazy;

        [UnmanagedCallersOnly]
        private static void ValidateProgramPipeline_Lazy(uint pipeline)
        {
            _validateProgramPipelineDelegate =
                (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glValidateProgramPipeline");
            _validateProgramPipelineDelegate(pipeline);
        }

        public static void GetProgramPipelineInfoLog(uint pipeline, int bufferSize, int* length, byte* infoLog)
        {
            _getProgramPipelineInfoLogDelegate(pipeline, bufferSize, length, infoLog);
        }

        private static delegate* unmanaged<uint, int, int*, byte*, void> _getProgramPipelineInfoLogDelegate =
            &GetProgramPipelineInfoLog_Lazy;

        [UnmanagedCallersOnly]
        private static void GetProgramPipelineInfoLog_Lazy(uint pipeline, int bufferSize, int* length, byte* infoLog)
        {
            _getProgramPipelineInfoLogDelegate =
                (delegate* unmanaged<uint, int, int*, byte*, void>)Sdl.GetProcAddress("glGetProgramPipelineInfoLog");
            _getProgramPipelineInfoLogDelegate(pipeline, bufferSize, length, infoLog);
        }

        public static void ProgramUniformMatrix4fv(uint program, int location, int count, byte transpose, float* value)
        {
            _programUniformMatrix4fvDelegate(program, location, count, transpose, value);
        }

        private static delegate* unmanaged<uint, int, int, byte, float*, void> _programUniformMatrix4fvDelegate =
            &ProgramUniformMatrix4fv_Lazy;

        [UnmanagedCallersOnly]
        private static void ProgramUniformMatrix4fv_Lazy(uint program, int location, int count, byte transpose,
            float* value)
        {
            _programUniformMatrix4fvDelegate =
                (delegate* unmanaged<uint, int, int, byte, float*, void>)Sdl.GetProcAddress(
                    "glProgramUniformMatrix4fv");
            _programUniformMatrix4fvDelegate(program, location, count, transpose, value);
        }

        public static void GenVertexArrays(int n, uint* arrays)
        {
            _genVertexArraysDelegate(n, arrays);
        }

        private static delegate* unmanaged<int, uint*, void> _genVertexArraysDelegate = &GenVertexArrays_Lazy;

        [UnmanagedCallersOnly]
        private static void GenVertexArrays_Lazy(int n, uint* arrays)
        {
            _genVertexArraysDelegate = (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glGenVertexArrays");
            _genVertexArraysDelegate(n, arrays);
        }

        private static void GenBuffers(int n, uint* buffers)
        {
            _genBuffersDelegate(n, buffers);
        }

        private static delegate* unmanaged<int, uint*, void> _genBuffersDelegate = &GenBuffers_Lazy;

        [UnmanagedCallersOnly]
        private static void GenBuffers_Lazy(int n, uint* buffers)
        {
            _genBuffersDelegate = (delegate* unmanaged<int, uint*, void>)Sdl.GetProcAddress("glGenBuffers");
            _genBuffersDelegate(n, buffers);
        }

        public static void BindBuffer(BufferTargetARB target, uint buffer)
        {
            _bindBufferDelegate(target, buffer);
        }

        private static delegate* unmanaged<BufferTargetARB, uint, void> _bindBufferDelegate = &BindBuffer_Lazy;

        [UnmanagedCallersOnly]
        private static void BindBuffer_Lazy(BufferTargetARB target, uint buffer)
        {
            _bindBufferDelegate = (delegate* unmanaged<BufferTargetARB, uint, void>)Sdl.GetProcAddress("glBindBuffer");
            _bindBufferDelegate(target, buffer);
        }

        public static void BufferData(BufferTargetARB target, nint size, void* data, BufferUsageARB usage)
        {
            _bufferDataDelegate(target, size, data, usage);
        }

        private static delegate* unmanaged<BufferTargetARB, nint, void*, BufferUsageARB, void> _bufferDataDelegate =
            &BufferData_Lazy;

        [UnmanagedCallersOnly]
        private static void BufferData_Lazy(BufferTargetARB target, nint size, void* data, BufferUsageARB usage)
        {
            _bufferDataDelegate =
                (delegate* unmanaged<BufferTargetARB, nint, void*, BufferUsageARB, void>)Sdl.GetProcAddress(
                    "glBufferData");
            _bufferDataDelegate(target, size, data, usage);
        }

        public static void VertexAttribPointer(uint index, int size, VertexAttribPointerType type, byte normalized,
            int stride, IntPtr offset)
        {
            _vertexAttribPointerDelegate(index, size, type, normalized, stride, offset);
        }

        private static delegate* unmanaged<uint, int, VertexAttribPointerType, byte, int, IntPtr, void>
            _vertexAttribPointerDelegate = &VertexAttribPointer_Lazy;

        [UnmanagedCallersOnly]
        private static void VertexAttribPointer_Lazy(uint index, int size, VertexAttribPointerType type,
            byte normalized, int stride, IntPtr offset)
        {
            _vertexAttribPointerDelegate =
                (delegate* unmanaged<uint, int, VertexAttribPointerType, byte, int, IntPtr, void>)Sdl.GetProcAddress(
                    "glVertexAttribPointer");
            _vertexAttribPointerDelegate(index, size, type, normalized, stride, offset);
        }

        public static void EnableVertexAttribArray(uint index)
        {
            _enableVertexAttribArrayDelegate(index);
        }

        private static delegate* unmanaged<uint, void> _enableVertexAttribArrayDelegate = &EnableVertexAttribArray_Lazy;

        [UnmanagedCallersOnly]
        private static void EnableVertexAttribArray_Lazy(uint index)
        {
            _enableVertexAttribArrayDelegate =
                (delegate* unmanaged<uint, void>)Sdl.GetProcAddress("glEnableVertexAttribArray");
            _enableVertexAttribArrayDelegate(index);
        }

        public static void BindTexture(TextureTarget target, uint texture)
        {
            _bindTextureDelegate(target, texture);
        }

        private static delegate* unmanaged<TextureTarget, uint, void> _bindTextureDelegate = &BindTexture_Lazy;

        [UnmanagedCallersOnly]
        private static void BindTexture_Lazy(TextureTarget target, uint texture)
        {
            _bindTextureDelegate = (delegate* unmanaged<TextureTarget, uint, void>)Sdl.GetProcAddress("glBindTexture");
            _bindTextureDelegate(target, texture);
        }

        private static void PushDebugGroup(DebugSource source, uint id, int length, byte* message)
        {
            _pushDebugGroupDelegate(source, id, length, message);
        }

        private static delegate* unmanaged<DebugSource, uint, int, byte*, void> _pushDebugGroupDelegate =
            &PushDebugGroup_Lazy;

        [UnmanagedCallersOnly]
        private static void PushDebugGroup_Lazy(DebugSource source, uint id, int length, byte* message)
        {
            _pushDebugGroupDelegate =
                (delegate* unmanaged<DebugSource, uint, int, byte*, void>)Sdl.GetProcAddress("glPushDebugGroup");
            _pushDebugGroupDelegate(source, id, length, message);
        }

        public static void PopDebugGroup()
        {
            _popDebugGroupDelegate();
        }

        private static delegate* unmanaged<void> _popDebugGroupDelegate = &PopDebugGroup_Lazy;

        [UnmanagedCallersOnly]
        private static void PopDebugGroup_Lazy()
        {
            _popDebugGroupDelegate = (delegate* unmanaged<void>)Sdl.GetProcAddress("glPopDebugGroup");
            _popDebugGroupDelegate();
        }

        public static void DrawElementsInstancedBaseVertexBaseInstance(
            PrimitiveType primitiveType,
            int elementCount,
            DrawElementsType elementsType,
            void* indices,
            int instanceCount,
            int baseVertex,
            uint baseInstance)
        {
            _drawElementsInstancedBaseVertexBaseInstanceDelegate(primitiveType, elementCount, elementsType, indices, instanceCount,
                baseVertex, baseInstance);
        }

        private static delegate* unmanaged<PrimitiveType, int, DrawElementsType, void*, int, int, uint, void>
            _drawElementsInstancedBaseVertexBaseInstanceDelegate = &DrawElementsInstancedBaseVertexBaseInstance_Lazy;

        [UnmanagedCallersOnly]
        private static void DrawElementsInstancedBaseVertexBaseInstance_Lazy(
            PrimitiveType primitiveType,
            int elementCount,
            DrawElementsType elementsType,
            void* indices,
            int instanceCount,
            int baseVertex,
            uint baseInstance)
        {
            _drawElementsInstancedBaseVertexBaseInstanceDelegate = (delegate* unmanaged<PrimitiveType, int, DrawElementsType, void*, int, int, uint, void>)Sdl.GetProcAddress("glDrawElementsInstancedBaseVertexBaseInstance");
            _drawElementsInstancedBaseVertexBaseInstanceDelegate(
                primitiveType,
                elementCount,
                elementsType,
                indices,
                instanceCount,
                baseVertex,
                baseInstance);
        }

        public static void DispatchCompute(uint numGroupsX, uint numGroupsY, uint numGroupsZ)
        {
            _dispatchComputeDelegate(numGroupsX, numGroupsY, numGroupsZ);
        }

        private static delegate* unmanaged<uint, uint, uint, void> _dispatchComputeDelegate = &DispatchCompute_Lazy;

        [UnmanagedCallersOnly]
        private static void DispatchCompute_Lazy(uint numGroupsX, uint numGroupsY, uint numGroupsZ)
        {
            _dispatchComputeDelegate =
                (delegate* unmanaged<uint, uint, uint, void>)Sdl.GetProcAddress("glDispatchCompute");
            _dispatchComputeDelegate(numGroupsX, numGroupsY, numGroupsZ);
        }

        public static void DispatchComputeIndirect(IntPtr indirect)
        {
            _dispatchComputeIndirectDelegate(indirect);
        }

        private static delegate* unmanaged<IntPtr, void> _dispatchComputeIndirectDelegate =
            &DispatchComputeIndirect_Lazy;

        [UnmanagedCallersOnly]
        private static void DispatchComputeIndirect_Lazy(IntPtr indirect)
        {
            _dispatchComputeIndirectDelegate =
                (delegate* unmanaged<IntPtr, void>)Sdl.GetProcAddress("glDispatchComputeIndirect");
            _dispatchComputeIndirectDelegate(indirect);
        }

        public static void MemoryBarrier(MemoryBarrierMask barriers)
        {
            _memoryBarrierDelegate(barriers);
        }

        private static delegate* unmanaged<MemoryBarrierMask, void> _memoryBarrierDelegate = &MemoryBarrier_Lazy;

        [UnmanagedCallersOnly]
        private static void MemoryBarrier_Lazy(MemoryBarrierMask barriers)
        {
            _memoryBarrierDelegate =
                (delegate* unmanaged<MemoryBarrierMask, void>)Sdl.GetProcAddress("glMemoryBarrier");
            _memoryBarrierDelegate(barriers);
        }

        public static void BindImageTexture(uint unit, uint texture, int level, byte layered, int layer,
            BufferAccess bufferAccess, SizedInternalFormat format)
        {
            _bindImageTextureDelegate(unit, texture, level, layered, layer, bufferAccess, format);
        }

        private static delegate* unmanaged<uint, uint, int, byte, int, BufferAccess, SizedInternalFormat, void>
            _bindImageTextureDelegate = &BindImageTexture_Lazy;

        [UnmanagedCallersOnly]
        private static void BindImageTexture_Lazy(
            uint unit,
            uint texture,
            int level,
            byte layered,
            int layer,
            BufferAccess bufferAccess,
            SizedInternalFormat format)
        {
            _bindImageTextureDelegate = (delegate* unmanaged<uint, uint, int, byte, int, BufferAccess, SizedInternalFormat, void>)Sdl.GetProcAddress("glBindImageTexture");
            _bindImageTextureDelegate(unit, texture, level, layered, layer, bufferAccess, format);
        }

        private static delegate* unmanaged<int, int*, void> _genQueriesDelegate = &GenQueries_Lazy;
        public static void GenQueries(int n, int* ids) => _genQueriesDelegate(n, ids);
        [UnmanagedCallersOnly]
        private static void GenQueries_Lazy(int n, int* ids)
        {
            _genQueriesDelegate = (delegate* unmanaged<int, int*, void>)Sdl.GetProcAddress("glGenQueries");
            _genQueriesDelegate(n, ids);
        }

        private static delegate* unmanaged<int, int*, void> _deleteQueriesDelegate = &DeleteQueries_Lazy;
        public static void DeleteQueries(int n, int* ids) => _deleteQueriesDelegate(n, ids);
        [UnmanagedCallersOnly]
        private static void DeleteQueries_Lazy(int n, int* ids)
        {
            _deleteQueriesDelegate = (delegate* unmanaged<int, int*, void>)Sdl.GetProcAddress("glDeleteQueries");
            _deleteQueriesDelegate(n, ids);
        }

        public static bool IsQuery(int id) => _isQueryDelegate(id) == 0;
        private static delegate* unmanaged<int, byte> _isQueryDelegate = &IsQuery_Lazy;
        [UnmanagedCallersOnly]
        private static byte IsQuery_Lazy(int id)
        {
            _isQueryDelegate = (delegate* unmanaged<int, byte>)Sdl.GetProcAddress("glIsQuery");
            return _isQueryDelegate(id);
        }

        private static delegate* unmanaged<QueryTarget, int, void> _beginQueryDelegate = &BeginQuery_Lazy;
        public static void BeginQuery(QueryTarget target, int id) => _beginQueryDelegate(target, id);
        [UnmanagedCallersOnly]
        private static void BeginQuery_Lazy(QueryTarget target, int id)
        {
            _beginQueryDelegate = (delegate* unmanaged<QueryTarget, int, void>)Sdl.GetProcAddress("glBeginQuery");
            _beginQueryDelegate(target, id);
        }

        private static delegate* unmanaged<QueryTarget, void> _endQueryDelegate = &EndQuery_Lazy;
        public static void EndQuery(QueryTarget target) => _endQueryDelegate(target);
        [UnmanagedCallersOnly]
        private static void EndQuery_Lazy(QueryTarget target)
        {
            _endQueryDelegate = (delegate* unmanaged<QueryTarget, void>)Sdl.GetProcAddress("glEndQuery");
            _endQueryDelegate(target);
        }

        private static delegate* unmanaged<QueryTarget, QueryParameterName, int*, void> _getQueryivDelegate = &GetQueryiv_Lazy;
        public static void GetQueryiv(QueryTarget target, QueryParameterName pname, int* parameters) => _getQueryivDelegate(target, pname, parameters);
        [UnmanagedCallersOnly]
        private static void GetQueryiv_Lazy(QueryTarget target, QueryParameterName pname, int* parameters)
        {
            _getQueryivDelegate = (delegate* unmanaged<QueryTarget, QueryParameterName, int*, void>)Sdl.GetProcAddress("glGetQueryiv");
            _getQueryivDelegate(target, pname, parameters);
        }

        private static delegate* unmanaged<int, QueryObjectParameterName, int*, void> _getQueryObjectivDelegate = &GetQueryObjectiv_Lazy;
        public static void GetQueryObjectiv(int id, QueryObjectParameterName pname, int* parameters) => _getQueryObjectivDelegate(id, pname, parameters);
        [UnmanagedCallersOnly]
        private static void GetQueryObjectiv_Lazy(int id, QueryObjectParameterName pname, int* parameters)
        {
            _getQueryObjectivDelegate = (delegate* unmanaged<int, QueryObjectParameterName, int*, void>)Sdl.GetProcAddress("glGetQueryObjectiv");
            _getQueryObjectivDelegate(id, pname, parameters);
        }

        private static delegate* unmanaged<int, QueryObjectParameterName, uint*, void> _getQueryObjectuivDelegate = &GetQueryObjectuiv_Lazy;
        public static void GetQueryObjectuiv(int id, QueryObjectParameterName pname, uint* parameters) => _getQueryObjectuivDelegate(id, pname, parameters);
        [UnmanagedCallersOnly]
        private static void GetQueryObjectuiv_Lazy(int id, QueryObjectParameterName pname, uint* parameters)
        {
            _getQueryObjectuivDelegate = (delegate* unmanaged<int, QueryObjectParameterName, uint*, void>)Sdl.GetProcAddress("glGetQueryObjectuiv");
            _getQueryObjectuivDelegate(id, pname, parameters);
        }

        public static void GetQueryBufferObjecti64v(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
            => _getQueryBufferObjecti64vDelegate(id, buffer, pname, offset);
        private static delegate* unmanaged<int, int, QueryObjectParameterName, IntPtr, void> _getQueryBufferObjecti64vDelegate = &GetQueryBufferObjecti64v_Lazy;
        [UnmanagedCallersOnly]
        private static void GetQueryBufferObjecti64v_Lazy(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
        {
            _getQueryBufferObjecti64vDelegate = (delegate* unmanaged<int, int, QueryObjectParameterName, IntPtr, void>)Sdl.GetProcAddress("glGetQueryBufferObjecti64v");
            _getQueryBufferObjecti64vDelegate(id, buffer, pname, offset);
        }

        public static void GetQueryBufferObjectiv(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
            => _getQueryBufferObjectivDelegate(id, buffer, pname, offset);
        private static delegate* unmanaged<int, int, QueryObjectParameterName, IntPtr, void> _getQueryBufferObjectivDelegate = &GetQueryBufferObjectiv_Lazy;
        [UnmanagedCallersOnly]
        private static void GetQueryBufferObjectiv_Lazy(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
        {
            _getQueryBufferObjectivDelegate = (delegate* unmanaged<int, int, QueryObjectParameterName, IntPtr, void>)Sdl.GetProcAddress("glGetQueryBufferObjectiv");
            _getQueryBufferObjectivDelegate(id, buffer, pname, offset);
        }

        public static void GetQueryBufferObjectui64v(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
            => _getQueryBufferObjectui64vDelegate(id, buffer, pname, offset);
        private static delegate* unmanaged<int, int, QueryObjectParameterName, IntPtr, void> _getQueryBufferObjectui64vDelegate = &GetQueryBufferObjectui64v_Lazy;
        [UnmanagedCallersOnly]
        private static void GetQueryBufferObjectui64v_Lazy(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
        {
            _getQueryBufferObjectui64vDelegate = (delegate* unmanaged<int, int, QueryObjectParameterName, IntPtr, void>)Sdl.GetProcAddress("glGetQueryBufferObjectui64v");
            _getQueryBufferObjectui64vDelegate(id, buffer, pname, offset);
        }

        public static void GetQueryBufferObjectuiv(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
            => _getQueryBufferObjectuivDelegate(id, buffer, pname, offset);
        private static delegate* unmanaged<int, int, QueryObjectParameterName, IntPtr, void> _getQueryBufferObjectuivDelegate = &GetQueryBufferObjectuiv_Lazy;
        [UnmanagedCallersOnly]
        private static void GetQueryBufferObjectuiv_Lazy(int id, int buffer, QueryObjectParameterName pname, IntPtr offset)
        {
            _getQueryBufferObjectuivDelegate = (delegate* unmanaged<int, int, QueryObjectParameterName, IntPtr, void>)Sdl.GetProcAddress("glGetQueryBufferObjectuiv");
            _getQueryBufferObjectuivDelegate(id, buffer, pname, offset);
        }

        public static void GetQueryObjecti64v(int id, QueryObjectParameterName pname, long* parameters)
            => _getQueryObjecti64vDelegate(id, pname, parameters);
        private static delegate* unmanaged<int, QueryObjectParameterName, long*, void> _getQueryObjecti64vDelegate = &GetQueryObjecti64v_Lazy;

        [UnmanagedCallersOnly]
        private static void GetQueryObjecti64v_Lazy(int id, QueryObjectParameterName pname, long* parameters)
        {
            _getQueryObjecti64vDelegate = (delegate* unmanaged<int, QueryObjectParameterName, long*, void>)Sdl.GetProcAddress("glGetQueryObjecti64v");
            _getQueryObjecti64vDelegate(id, pname, parameters);
        }

        public static void GetQueryObjectui64v(int id, QueryObjectParameterName pname, ulong* parameters)
            => _getQueryObjectui64vDelegate(id, pname, parameters);
        private static delegate* unmanaged<int, QueryObjectParameterName, ulong*, void> _getQueryObjectui64vDelegate = &GetQueryObjectui64v_Lazy;
        [UnmanagedCallersOnly]
        private static void GetQueryObjectui64v_Lazy(int id, QueryObjectParameterName pname, ulong* parameters)
        {
            _getQueryObjectui64vDelegate = (delegate* unmanaged<int, QueryObjectParameterName, ulong*, void>)Sdl.GetProcAddress("glGetQueryObjectui64v");
            _getQueryObjectui64vDelegate(id, pname, parameters);
        }

        private static delegate* unmanaged<uint, int, PixelFormat, PixelType, int, void*, void> _getTextureImageDelegate = &GetTextureImage_Lazy;
        private static void GetTextureImage(uint texture, int level, PixelFormat format, PixelType type, int bufSize, void* pixels)
            => _getTextureImageDelegate(texture, level, format, type, bufSize, pixels);
        [UnmanagedCallersOnly]
        private static void GetTextureImage_Lazy(uint texture, int level, PixelFormat format, PixelType type, int bufSize, void* pixels)
        {
            _getTextureImageDelegate = (delegate* unmanaged<uint, int, PixelFormat, PixelType, int, void*, void>)Sdl.GetProcAddress("glGetTextureImage");
            _getTextureImageDelegate(texture, level, format, type, bufSize, pixels);
        }

        private static delegate* unmanaged<uint, int, int, void*, void> _getCompressedTextureImageDelegate = &GetCompressedTextureImage_Lazy;
        private static void GetCompressedTextureImage(uint texture, int level, int bufSize, void* pixels)
            => _getCompressedTextureImageDelegate(texture, level, bufSize, pixels);
        [UnmanagedCallersOnly]
        private static void GetCompressedTextureImage_Lazy(uint texture, int level, int bufSize, void* pixels)
        {
            _getCompressedTextureImageDelegate = (delegate* unmanaged<uint, int, int, void*, void>)Sdl.GetProcAddress("glGetCompressedTextureImage");
            _getCompressedTextureImageDelegate(texture, level, bufSize, pixels);
        }
    }
}
