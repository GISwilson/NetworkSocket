﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkSocket.WebSocket.Fast
{
    /// <summary>
    /// 定义基于Json文本协议通讯的WebSocket服务
    /// </summary>
    public interface IFastWebSocketServer
    {
        /// <summary>
        /// 获取或设置Json序列化工具       
        /// </summary>
        IJsonSerializer JsonSerializer { get; set; }

        /// <summary>
        /// 获取或设置Api行为特性过滤器提供者
        /// </summary>
        IFilterAttributeProvider FilterAttributeProvider { get; set; }      
    }
}
