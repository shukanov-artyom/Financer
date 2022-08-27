using Concentus.Structs;

namespace Financer.Ogg.Decompressor.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string fileName = @"e:\down\male.ogg";
            using (DsReader dr = new DsReader(fileName))
            {
                if (dr.HasAudio)
                {
                    IntPtr format = dr.ReadFormat();
                    using (WaveWriter ww = new WaveWriter(File.Create(fileName + ".wav"),
                        AudioCompressionManager.FormatBytes(format)))
                    {
                        byte[] data = dr.ReadData();
                        ww.WriteData(data);
                    }
                }
            }


            using (FileStream fileIn = new FileStream("./file_14.oga", FileMode.Open))
            using (MemoryStream pcmStream = new MemoryStream())
            {
                OpusDecoder decoder = OpusDecoder.Create(48000, 1);
                OpusOggReadStream oggIn = new OpusOggReadStream(decoder, fileIn);
                while (oggIn.HasNextPacket)
                {
                    short[] packet = oggIn.DecodeNextPacket();
                    if (packet != null)
                    {
                        for (int i = 0; i < packet.Length; i++)
                        {
                            var bytes = BitConverter.GetBytes(packet[i]);
                            pcmStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                pcmStream.Position = 0;
                var wavStream = new RawSourceWaveStream(pcmStream, new WaveFormat(48000, 1));
                var sampleProvider = wavStream.ToSampleProvider();
                WaveFileWriter.CreateWaveFile16($"{filePath}{fileWav}", sampleProvider);
            }
        }
    }
}