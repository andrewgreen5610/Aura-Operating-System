using System;
using Aura_OS;
using Aura_OS.IO;
using Cosmos.HAL.BlockDevice;

unsafe class RAMDisk : BlockDevice
{
    private byte[] Data;
    public RAMDisk(ulong blockCount)
    {
        Data = new byte[blockCount * 512];
        this.mBlockCount = blockCount;
        this.mBlockSize = 512;
        
        
    }
    public override void ReadBlock(ulong aBlockNo, ulong aBlockCount, byte[] aData)
    {
        uint dats = (uint)aBlockCount * 512;
        ulong start = aBlockNo * 512;
        for (uint i = 0; i < dats; i++)
        {
            aData[i] = Data[start + i];
        }
    }
    public override void WriteBlock(ulong aBlockNo, ulong aBlockCount, byte[] aData)
    {
        uint dats = (uint)aBlockCount * 512;
        ulong start = aBlockNo * 512;
        for (uint i = 0; i < dats; i++)
        {
            Data[start + i] = aData[i];
        }
    }
}