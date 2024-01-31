using Microsoft.Win32;

using System;

namespace Raid.Toolkit.Extensibility.Shared
{
	public record FileAssociation(string Extension, string ProgId, string FileTypeDescription, string ExecutableFilePath, string ExecutableArguments);

	public class FileAssociations
	{
		// needed so that Explorer windows get refreshed after the registry is updated
		[System.Runtime.InteropServices.DllImport("Shell32.dll")]
		private static extern int SHChangeNotify(int eventId, int flags, IntPtr item1, IntPtr item2);

		private const int SHCNE_ASSOCCHANGED = 0x8000000;
		private const int SHCNF_FLUSH = 0x1000;

		public static void EnsureAssociationsSet(params FileAssociation[] associations)
		{
			bool madeChanges = false;
			foreach (var association in associations)
			{
				madeChanges |= SetAssociation(
					association.Extension,
					association.ProgId,
					association.FileTypeDescription,
					association.ExecutableFilePath,
					association.ExecutableArguments);
			}

			if (madeChanges)
			{
				_ = SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_FLUSH, IntPtr.Zero, IntPtr.Zero);
			}
		}

		public static bool SetAssociation(string extension, string progId, string fileTypeDescription, string applicationFilePath, string applicationArgs)
		{
			bool madeChanges = false;
			if (!string.IsNullOrEmpty(progId))
				madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + extension, progId);
			if (!string.IsNullOrEmpty(fileTypeDescription))
				madeChanges |= SetKeyDefaultValue(@"Software\Classes\" + progId, fileTypeDescription);
			madeChanges |= SetKeyDefaultValue($@"Software\Classes\{progId}\shell\open\command", "\"" + applicationFilePath + "\" " + applicationArgs + " \"%1\"");
			return madeChanges;
		}

		private static bool SetKeyDefaultValue(string keyPath, string value)
		{
			using var key = Registry.CurrentUser.CreateSubKey(keyPath);
			if (key.GetValue(null) as string != value)
			{
				key.SetValue(null, value);
				return true;
			}

			return false;
		}
	}
}
