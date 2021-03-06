﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetworkSocket.WebSocket.Fast
{
    /// <summary>
    /// 任务行为表
    /// 自带超时检测功能
    /// </summary>
    internal class TaskSetActionTable
    {
        /// <summary>
        /// 任务行为字典
        /// </summary>
        private readonly ConcurrentDictionary<long, TaskSetAction> table = new ConcurrentDictionary<long, TaskSetAction>();

        /// <summary>
        /// 超时时间
        /// </summary>       
        private int timeOut = 30 * 1000;

        /// <summary>
        /// 获取或设置超时时间(毫秒)
        /// 默认30秒
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public int TimeOut
        {
            get
            {
                return this.timeOut;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("TimeOut", "TimeOut的值必须大于0");
                }
                this.timeOut = value;
            }
        }

        /// <summary>
        /// 任务行为表
        /// </summary>
        public TaskSetActionTable()
        {
            Task.Factory.StartNew(() =>
            {
                var spinWait = new SpinWait();
                while (true)
                {
                    if (this.table.Count > 0)
                    {
                        this.CheckTaskActionTimeout();
                    }
                    spinWait.SpinOnce();
                }
            });
        }

        /// <summary>
        /// 检测任务行为的超时
        /// </summary>       
        private void CheckTaskActionTimeout()
        {
            foreach (var key in this.table.Keys)
            {
                TaskSetAction taskSetAction;
                if (this.table.TryGetValue(key, out taskSetAction))
                {
                    if (Environment.TickCount - taskSetAction.InitTime < TimeOut)
                    {
                        break;
                    }
                }

                if (this.table.TryRemove(key, out taskSetAction))
                {
                    taskSetAction.SetAction(SetTypes.SetTimeoutException, null);
                }
            }
        }

        /// <summary>
        /// 添加回调信息记录       
        /// </summary>
        /// <param name="key">键值</param>
        /// <param name="taskSetAction">设置行为</param>       
        /// <returns></returns>
        public void Add(long key, TaskSetAction taskSetAction)
        {
            this.table.TryAdd(key, taskSetAction);
        }

        /// <summary>      
        /// 获取并移除与key匹配值
        /// 如果没有匹配项，返回null
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public TaskSetAction Take(long key)
        {
            TaskSetAction taskSetAction;
            this.table.TryRemove(key, out taskSetAction);
            return taskSetAction;
        }

        /// <summary>
        /// 取出并移除全部的项
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TaskSetAction> TakeAll()
        {
            var values = this.table.ToArray().Select(item => item.Value);
            this.Clear();
            return values;
        }

        /// <summary>
        /// 清除所有数据
        /// </summary>
        public void Clear()
        {
            this.table.Clear();
        }
    }
}
