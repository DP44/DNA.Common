using System;

namespace DNA.Runtime.InteropServices.Win32
{
	[Flags]
	public enum Win32FileAccess : uint
	{
		Default = 0U,
		Delete = 65536U,
		ReadControl = 131072U,
		WriteDAC = 262144U,
		WriteOwner = 524288U,
		Synchronize = 1048576U,
		StandardRightsRequired = 983040U,
		StandardRightsRead = 131072U,
		StandardRightsWrite = 131072U,
		StandardRightsExecute = 131072U,
		StandardRightSall = 2031616U,
		SpecificRightSall = 65535U,
		ReadData = 1U,
		ListDirectory = 1U,
		WriteData = 2U,
		AddFile = 2U,
		AppendData = 4U,
		AddSubdirectorY = 4U,
		CreatePipeInstance = 4U,
		ReadEA = 8U,
		WriteEA = 16U,
		Execute = 32U,
		Traverse = 32U,
		DeleteChild = 64U,
		ReadAttributes = 128U,
		WriteAttributes = 256U,
		AllAccess = 2032127U,
		GenericRead = 1179785U,
		GenericWrite = 1179926U,
		GenericExecute = 1179808U
	}
}
