using System;

namespace EsvBible.Service.Setting
{
    public interface IOutputSetting : IDefaultOptimization
    {
        /// <summary>
        ///     Timeout in seconds
        /// </summary>
        Int32 Timeout { get; set; }
    }
}