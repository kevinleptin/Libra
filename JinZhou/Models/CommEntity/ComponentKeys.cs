using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JinZhou.Models.CommEntity
{
    public class ComponentKeys
    {
        private static object locker = new object();
        private static ComponentKeys _componentKeys = null;
        private ComponentKeys()
        {
            VerifyData = new ComponentVerifyData();
            AccessData = new ComponentAccessData();
            PreAuthData = new ComponentPreAuthData();

            LoadFromRedis();
        }

        public static ComponentKeys GetInstance()
        {
            if (_componentKeys == null)
            {
                lock (locker)
                {
                    if (_componentKeys == null)
                    {
                        _componentKeys = new ComponentKeys();
                    }
                }
            }

            return _componentKeys;
        }

        private void LoadFromRedis()
        {
            //TODO: 初始化时，尝试从Redis里读取数据
        }

        public ComponentVerifyData VerifyData { get; set; }
        public ComponentAccessData AccessData { get; set; }
        public ComponentPreAuthData PreAuthData { get; set; }
    }
}
