using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
//using System.Threading.Tasks;
using System.Web;


using System.Threading;
using System.IO;

namespace NFine.Code
{

    ///  
    /// 系统信息类 - 获取CPU、内存、磁盘、进程信息 
    ///  
    public class SystemInfo
    {
        private int m_ProcessorCount = 0;   //CPU个数 
        private PerformanceCounter pcCpuLoad;   //CPU计数器 
        private long m_PhysicalMemory = 0;   //物理内存 

        private const int GW_HWNDFIRST = 0;
        private const int GW_HWNDNEXT = 2;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 268435456;
        private const int WS_BORDER = 8388608;

        #region AIP声明 
        [DllImport("IpHlpApi.dll")]
        extern static public uint GetIfTable(byte[] pIfTable, ref uint pdwSize, bool bOrder);

        [DllImport("User32")]
        private extern static int GetWindow(int hWnd, int wCmd);

        [DllImport("User32")]
        private extern static int GetWindowLongA(int hWnd, int wIndx);

        [DllImport("user32.dll")]
        private static extern bool GetWindowText(int hWnd, StringBuilder title, int maxBufSize);

        [DllImport("user32", CharSet = CharSet.Auto)]
        private extern static int GetWindowTextLength(IntPtr hWnd);
        #endregion

        #region 构造函数 
        ///  
        /// 构造函数，初始化计数器等 
        ///  
        public SystemInfo()
        {
            //初始化CPU计数器 
            pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            pcCpuLoad.MachineName = ".";
            pcCpuLoad.NextValue();

            //CPU个数 
            m_ProcessorCount = Environment.ProcessorCount;

            //获得物理内存 
            ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    m_PhysicalMemory = long.Parse(mo["TotalPhysicalMemory"].ToString());
                }
            }
        }
        #endregion

        #region CPU个数 
        ///  
        /// 获取CPU个数 
        ///  
        public int ProcessorCount
        {
            get
            {
                return m_ProcessorCount;
            }
        }
        #endregion

        #region CPU占用率 
        ///  
        /// 获取CPU占用率 
        ///  
        public float CpuLoad
        {
            get
            {
                return pcCpuLoad.NextValue();
            }
        }
        #endregion

        #region 可用内存 
        ///  
        /// 获取可用内存 
        ///  
        public long MemoryAvailable
        {
            get
            {
                long availablebytes = 0;
                //ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_PerfRawData_PerfOS_Memory"); 
                //foreach (ManagementObject mo in mos.Get()) 
                //{ 
                //    availablebytes = long.Parse(mo["Availablebytes"].ToString()); 
                //} 
                ManagementClass mos = new ManagementClass("Win32_OperatingSystem");
                foreach (ManagementObject mo in mos.GetInstances())
                {
                    if (mo["FreePhysicalMemory"] != null)
                    {
                        availablebytes = 1024 * long.Parse(mo["FreePhysicalMemory"].ToString());
                    }
                }
                return availablebytes;
            }
        }
        #endregion

        #region 物理内存 
        ///  
        /// 获取物理内存 
        ///  
        public long PhysicalMemory
        {
            get
            {
                return m_PhysicalMemory;
            }
        }
        #endregion

        //#region 获得分区信息 
        /////  
        ///// 获取分区信息 
        /////  
        //public List GetLogicalDrives()
        //{
        //    List drives = List();
        //    ManagementClass diskClass = new ManagementClass("Win32_LogicalDisk");
        //    ManagementObjectCollection disks = diskClass.GetInstances();
        //    foreach (ManagementObject disk in disks)
        //    {
        //        // DriveType.Fixed 为固定磁盘(硬盘) 
        //        if (int.Parse(disk["DriveType"].ToString()) == (int)DriveType.Fixed)
        //        {
        //            drives.Add(new DiskInfo(disk["Name"].ToString(), long.Parse(disk["Size"].ToString()), long.Parse(disk["FreeSpace"].ToString())));
        //        }
        //    }
        //    return drives;
        //}
        /////  
        ///// 获取特定分区信息 
        /////  
        ///// 盘符 
        //public List GetLogicalDrives(char DriverID)
        //{
        //    List drives = new List();
        //    WqlObjectQuery wmiquery = new WqlObjectQuery("SELECT * FROM Win32_LogicalDisk WHERE DeviceID = ’" + DriverID + ":’");
        //    ManagementObjectSearcher wmifind = new ManagementObjectSearcher(wmiquery);
        //    foreach (ManagementObject disk in wmifind.Get())
        //    {
        //        if (int.Parse(disk["DriveType"].ToString()) == (int)DriveType.Fixed)
        //        {
        //            drives.Add(new DiskInfo(disk["Name"].ToString(), long.Parse(disk["Size"].ToString()), long.Parse(disk["FreeSpace"].ToString())));
        //        }
        //    }
        //    return drives;
        //}
        //#endregion

        //#region 获得进程列表 
        /////  
        ///// 获得进程列表 
        /////  
        //public List GetProcessInfo()
        //{
        //    List pInfo = new List();
        //    Process[] processes = Process.GetProcesses();
        //    foreach (Process instance in processes)
        //    {
        //        try
        //        {
        //            pInfo.Add(new ProcessInfo(instance.Id,
        //                instance.ProcessName,
        //                instance.TotalProcessorTime.TotalMilliseconds,
        //                instance.WorkingSet64,
        //                instance.MainModule.FileName));
        //        }
        //        catch { }
        //    }
        //    return pInfo;
        //}
        /////  
        ///// 获得特定进程信息 
        /////  
        ///// 进程名称 
        //public List GetProcessInfo(string ProcessName)
        //{
        //    List pInfo = new List();
        //    Process[] processes = Process.GetProcessesByName(ProcessName);
        //    foreach (Process instance in processes)
        //    {
        //        try
        //        {
        //            pInfo.Add(new ProcessInfo(instance.Id,
        //                instance.ProcessName,
        //                instance.TotalProcessorTime.TotalMilliseconds,
        //                instance.WorkingSet64,
        //                instance.MainModule.FileName));
        //        }
        //        catch { }
        //    }
        //    return pInfo;
        //}
        //#endregion

        #region 结束指定进程 
        ///  
        /// 结束指定进程 
        ///  
        /// 进程的 Process ID 
        public static void EndProcess(int pid)
        {
            try
            {
                Process process = Process.GetProcessById(pid);
                process.Kill();
            }
            catch { }
        }
        #endregion


        //#region 查找所有应用程序标题 
        /////  
        ///// 查找所有应用程序标题 
        /////  
        ///// 应用程序标题范型 
        //public static List FindAllApps(int Handle)
        //{
        //    List Apps = new List();

        //    int hwCurr;
        //    hwCurr = GetWindow(Handle, GW_HWNDFIRST);

        //    while (hwCurr > 0)
        //    {
        //        int IsTask = (WS_VISIBLE | WS_BORDER);
        //        int lngStyle = GetWindowLongA(hwCurr, GWL_STYLE);
        //        bool TaskWindow = ((lngStyle & IsTask) == IsTask);
        //        if (TaskWindow)
        //        {
        //            int length = GetWindowTextLength(new IntPtr(hwCurr));
        //            StringBuilder sb = new StringBuilder(2 * length + 1);
        //            GetWindowText(hwCurr, sb, sb.Capacity);
        //            string strTitle = sb.ToString();
        //            if (!string.IsNullOrEmpty(strTitle))
        //            {
        //                Apps.Add(strTitle);
        //            }
        //        }
        //        hwCurr = GetWindow(hwCurr, GW_HWNDNEXT);
        //    }

        //    return Apps;
        //}
        //#endregion
    }


    /// <summary>
    /// 获取运行环境 帮助类型
    /// </summary>
    public class EnvironmentHelper
    {

        //PerformanceCounter cpu = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        //ComputerInfo cinf = new ComputerInfo();

        /// <summary>
        /// 获取机器名
        /// </summary>
        /// <returns></returns>
        public static string GetCurMachineName()
        {
            return Environment.MachineName;
        }
        /// <summary>
        /// 获取域名
        /// </summary>
        /// <returns></returns>
        public static string GetUseDomianName()
        {
            return Environment.UserDomainName;
        }
        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        /// <returns></returns>
        public static string GetCurLoginUserName()
        {
            return Environment.UserName;
        }
        /// <summary>
        /// 操作系统名称及版本
        /// </summary>
        /// <returns></returns>
        public static string GetOSNameAndVersion()
        {
            return Environment.OSVersion.VersionString;
        }

        /// <summary>
        /// 操作系统名称
        /// </summary>
        /// <returns></returns>
        /// 
       
        public static OSInfo GetOSVersion()
        {
            OSInfo info = new OSInfo();
           
            #region 思路
            /*********************************  System.PlatformID枚举值及其含义  **********************************/
            //Win32S 操作系统为 Win32s（Win32 子集）类型。
            //Win32s 是运行于 Windows 16 位版本上的层，它提供对 32 位应用程序的访问。 
            //Win32Windows 操作系统为 Windows 95 或较新的版本。 
            //Win32NT 操作系统为 Windows NT 或较新的版本。 
            //WinCE 操作系统为 Windows CE。 
            //Unix 操作系统为 Unix。 
            //Xbox 开发平台为 Xbox 360。
            /*********************************   Windows操作系统的版本号一览  **********************************/
            //操作系统         PlatformID     主版本号    副版本号
            // Windows95          1               4           0
            // Windows98          1               4           10
            // WindowsMe          1               4           90
            // WindowsNT3.5       2               3           0
            // WindowsNT4.0       2               4           0
            // Windows2000        2               5           0
            // WindowsXP          2               5           1
            // Windows2003        2               5           2
            // WindowsVista       2               6           0
            // Windows7           2               6           1
            // Windows8
            #endregion
            PlatformID platId = Environment.OSVersion.Platform;
            int majorVer = Environment.Version.Major;
            int minorVer = Environment.Version.Minor;
            switch (platId)
            {
                case PlatformID.Win32S:
                    break;
                case PlatformID.Win32Windows:
                    if (minorVer==0)
                    {
                        info.OSName = "Windows95";
                       
                    }
                    else if(minorVer==10)
                    {
                        info.OSName = "Windows98";
                    }
                    else
                    {
                        info.OSName = "WindowsMe";
                    }
                    info.majorVer = majorVer.ToString();
                    info.minorVer = minorVer.ToString();
                    break;
                case PlatformID.Win32NT:
                    info.majorVer = majorVer.ToString();
                    info.minorVer = minorVer.ToString();
                    if (majorVer == 6 && minorVer == 1)
                    {
                        info.OSName = "Windows7";
                        break;
                    }
                    if (majorVer == 6 && minorVer == 0)
                    {
                        info.OSName = "WindowsVista";
                        break;
                    }
                    if (majorVer == 5 && minorVer == 2)
                    {
                        info.OSName = "Windows2003";
                        break;
                    }
                    if (majorVer == 5 && minorVer == 1)
                    {
                        info.OSName = "WindowsXP";
                        break;
                    }
                    if (majorVer == 5 && minorVer == 0)
                    {
                        info.OSName = "Windows2000";
                        break;
                    }
                    if (majorVer == 4)
                    {
                        info.OSName = "WindowsNT4.0";
                        break;
                    }
                    if (majorVer == 3)
                    {
                        info.OSName = "WindowsNT3.5";
                        break;
                    }
                    break;
                case PlatformID.WinCE:
                    info.OSName = "Windows CE";
                    break;
                case PlatformID.Unix:
                    info.OSName = "Unix";
                    break;
                case PlatformID.Xbox:
                    info.OSName = "Xbox 360";
                    break;
                case PlatformID.MacOSX:
                    info.OSName = "Mac";
                    break;
                default:
                    break;
            }


            return info;
          
        }
         


    }
    public class OSInfo
    {
        /// <summary>
        /// 操作系统名称
        /// </summary>
        public string OSName { get; set; }
        /// <summary>
        /// 主版本号
        /// </summary>
        public string majorVer { get; set; }
        /// <summary>
        /// 副版本号
        /// </summary>
        public string minorVer { get; set; }
    }
         
}
