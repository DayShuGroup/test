/*******************************************************************************
 * Copyright © 2016 NFine.Framework 版权所有
 * Author: NFine
 * Description: NFine快速开发平台
 * Website：http://www.nfine.cn
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Code;
namespace NFine.Web.Areas.SystemSecurity.Controllers
{
    public class ServerMonitoringController : ControllerBase
    {
        private SystemInfo _sysInfo = null;
        public SystemInfo sysInfo { get { return _sysInfo ?? new SystemInfo(); } }

        /// <summary>
        /// 获取CPU占用率
        /// </summary>
        /// <returns></returns>
        public ActionResult GetCpuLoad()
        {
            float cpu=sysInfo.CpuLoad;
            return Json(cpu, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取RAM占用率
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRAMLoad()
        {
            float ram = ((float)((sysInfo.PhysicalMemory-sysInfo.MemoryAvailable)*1.0)/sysInfo.PhysicalMemory)*100;
            return Json(ram, JsonRequestBehavior.AllowGet);
        }
    }
}
