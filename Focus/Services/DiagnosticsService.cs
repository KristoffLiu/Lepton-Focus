using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI.Xaml.Media.Imaging;

namespace Focus.Services
{
    public class DiagnosticsService
    {
        static IReadOnlyList<ProcessDiagnosticInfo> processes = ProcessDiagnosticInfo.GetForProcesses();
        public async static void RequestAccessAsync()
        {
            DiagnosticAccessStatus diagnosticAccessStatus =
                await AppDiagnosticInfo.RequestAccessAsync();
            switch (diagnosticAccessStatus)
            {
                case DiagnosticAccessStatus.Allowed:
                    Debug.WriteLine("We can get diagnostics for all apps.");
                    break;
                case DiagnosticAccessStatus.Limited:
                    Debug.WriteLine("We can only get diagnostics for this app package.");
                    break;
            }
        }
        public async static void Update()
        {
            if (processes != null)
            {
                foreach (ProcessDiagnosticInfo process in processes)
                {
                    string exeName = process.ExecutableFileName;
                    string pid = process.ProcessId.ToString();

                    ProcessCpuUsageReport cpuReport = process.CpuUsage.GetReport();
                    TimeSpan userCpu = cpuReport.UserTime;
                    TimeSpan kernelCpu = cpuReport.KernelTime;

                    ProcessMemoryUsageReport memReport = process.MemoryUsage.GetReport();
                    ulong npp = memReport.NonPagedPoolSizeInBytes;
                    ulong pp = memReport.PagedPoolSizeInBytes;
                    ulong peakNpp = memReport.PeakNonPagedPoolSizeInBytes;
                    //...etc

                    ProcessDiskUsageReport diskReport = process.DiskUsage.GetReport();
                    long bytesRead = diskReport.BytesReadCount;
                    long bytesWritten = diskReport.BytesWrittenCount;
                    //...etc
                    if (process.IsPackaged)
                    {
                        IList<AppDiagnosticInfo> diagnosticInfos = process.GetAppDiagnosticInfos();
                        if (diagnosticInfos != null && diagnosticInfos.Count > 0)
                        {
                            AppDiagnosticInfo diagnosticInfo = diagnosticInfos.FirstOrDefault();
                            if (diagnosticInfo != null)
                            {
                                IList<AppResourceGroupInfo> groups = diagnosticInfo.GetResourceGroups();
                                if (groups != null && groups.Count > 0)
                                {
                                    AppResourceGroupInfo group = groups.FirstOrDefault();
                                    if (group != null)
                                    {
                                        string name = diagnosticInfo.AppInfo.DisplayInfo.DisplayName;
                                        string description = diagnosticInfo.AppInfo.DisplayInfo.Description;
                                        BitmapImage bitmapImage = await GetLogoAsync(diagnosticInfo);

                                        AppResourceGroupStateReport stateReport = group.GetStateReport();
                                        if (stateReport != null)
                                        {
                                            string executionStatus = stateReport.ExecutionState.ToString();
                                            string energyStatus = stateReport.EnergyQuotaState.ToString();
                                        }

                                        AppResourceGroupMemoryReport memoryReport = group.GetMemoryReport();
                                        if (memoryReport != null)
                                        {
                                            AppMemoryUsageLevel level = memoryReport.CommitUsageLevel;
                                            ulong limit = memoryReport.CommitUsageLimit;
                                            ulong totalCommit = memoryReport.TotalCommitUsage;
                                            ulong privateCommit = memoryReport.PrivateCommitUsage;
                                            ulong sharedCommit = totalCommit - privateCommit;
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }
        }
        private static async Task<BitmapImage> GetLogoAsync(AppDiagnosticInfo app)
        {
            RandomAccessStreamReference stream =
                app.AppInfo.DisplayInfo.GetLogo(new Size(64, 64));
            IRandomAccessStreamWithContentType content = await stream.OpenReadAsync();
            BitmapImage bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(content);
            return bitmapImage;
        }

    }

    public class AppDiagnosticsInfo
    {
        public string ID { get; set; }
        public string AppName { get; set; }
        public BitmapImage Logo { get; set; }
        public TimeSpan RunningTime { get; set; }
        public string StartTime { get; set; }
    }
}
