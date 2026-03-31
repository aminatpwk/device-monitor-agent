using DeviceMonitorAgent.Domain.Models;
using System.Management;
using System.Net.Http.Json;
using DeviceMonitorAgent.Installer.CA.DTOs;
using WixToolset.Dtf.WindowsInstaller;

namespace DeviceMonitorAgent.Installer.CA;

public class RegistrationActions
{
    [CustomAction]
    public static ActionResult ReadSerialNumber(Session session)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(
                "SELECT SerialNumber FROM Win32_BIOS");

            foreach (ManagementObject obj in searcher.Get())
            {
                var serial = obj["SerialNumber"]?.ToString()
                             ?? Environment.MachineName;
                session["SERIAL_NUMBER"] = serial;
                break;
            }

            return ActionResult.Success;
        }
        catch (Exception ex)
        {
            session.Log($"WMI read failed, falling back to MachineName: {ex.Message}");
            session["SERIAL_NUMBER"] = Environment.MachineName;
            return ActionResult.Success;
        }
    }

    [CustomAction]
    public static ActionResult RegisterDevice(Session session)
    {
        try
        {
            var serialNumber = session["SERIAL_NUMBER"];

            using var client = new HttpClient();
            var payload = new
            {
                SerialNumber = serialNumber,
                MachineName = Environment.MachineName,
                RequestedAt = DateTime.UtcNow
            };

            var response = client
                .PostAsJsonAsync("https://insertplatformapihere/test", payload)
                .GetAwaiter().GetResult();  

            if (response.IsSuccessStatusCode)
            {
                var result = response.Content
                    .ReadFromJsonAsync<RegisterDeviceResponse>()
                    .GetAwaiter().GetResult();

                session["DEVICE_CODE"] = result!.DeviceCode.ToString();

                WriteToDatabase(result.DeviceCode);
                return ActionResult.Success;
            }

            return ActionResult.Success;
        }
        catch (Exception ex)
        {
            session.Log($"Registration error: {ex.Message}");
            return ActionResult.Success;
        }
    }

    private static void WriteToDatabase(Guid deviceCode)
    {
        var dbDir = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.CommonApplicationData),
            "DeviceMonitorAgent", "Data");

        Directory.CreateDirectory(dbDir);

        var dbPath = Path.Combine(dbDir, "agent_state.db");

        using var db = new LiteDB.LiteDatabase(dbPath);
        var col = db.GetCollection<AgentRegistration>("AgentRegistration");
        col.Upsert(new AgentRegistration
        {
            DeviceCode = deviceCode,
            SerialNumber = Environment.MachineName
        });
    }
}