﻿using Abp.Dependency;
using Castle.MicroKernel;
using Castle.Windsor;
using System;

namespace Core.Messages.Bus
{
    internal class MessageScope : IMessageScope
    {
        private readonly IIocManager _iocManager;

        private readonly IKernel _kernel;

        public MessageScope(IIocManager iocManager, IKernel kernel)
        {
            _iocManager = iocManager;
            _kernel = kernel;
            _iocManager.IocContainer.Kernel.AddChildKernel(_kernel);
        }



        public void Release(IMessageHandler handler)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(_kernel));
            }
            _kernel.ReleaseComponent(handler);
        }

        public object Resolve(Type type)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(_kernel));
            }
            return _kernel.Resolve(type);
        }

        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _iocManager.IocContainer.Kernel.RemoveChildKernel(_kernel);
                    _kernel.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~MessageScope() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
