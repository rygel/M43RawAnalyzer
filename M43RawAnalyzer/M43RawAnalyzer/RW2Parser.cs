using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using ExifLib;

namespace M43RawAnalyzer
{
    /// <summary>
    /// This class uses code created by Raphaël Rigo for his Panasonic RW2 files lens distortion correction information parser, see http://syscall.eu/#pana.
    /// </summary>
    class RW2Parser
    {
        public struct Distortion {
            public double scale;
            public double a, b, c;
            public double n;
        };

        private Dictionary<string,OffsetEntry> offsets = new Dictionary<string,OffsetEntry>();

        public RW2Parser() {
            offsets.Add("Panasonic DMC-GH1", new OffsetEntry("Panasonic DMC-GH1", new int[2] { 0x28A6, 0x28BC }));
            offsets.Add("Panasonic DMC-GH2", new OffsetEntry("Panasonic DMC-GH2", new int[1] { 0x2964 }));
            offsets.Add("Panasonic DMC-GH3", new OffsetEntry("Panasonic DMC-GH3", new int[1] { 0x3308 }));
            offsets.Add("Panasonic DMC-G1", new OffsetEntry("Panasonic DMC-G1", new int[1] { 0x2844 }));
            offsets.Add("Panasonic DMC-G2", new OffsetEntry("Panasonic DMC-G2", new int[2] { 0x2974, 0x28EC }));
            offsets.Add("Panasonic DMC-G3", new OffsetEntry("Panasonic DMC-G3", new int[1] { 0x2994 }));
            offsets.Add("Panasonic DMC-G5", new OffsetEntry("Panasonic DMC-G5", new int[1] { 0x3218 }));
            offsets.Add("Panasonic DMC-G10", new OffsetEntry("Panasonic DMC-G10", new int[1] { 0x28EC }));
            offsets.Add("Panasonic DMC-GF1", new OffsetEntry("Panasonic DMC-GF1", new int[1] { 0x28BC }));
            offsets.Add("Panasonic DMC-GF2", new OffsetEntry("Panasonic DMC-GF2", new int[1] { 0x2970 }));
            offsets.Add("Panasonic DMC-GF3", new OffsetEntry("Panasonic DMC-GF3", new int[1] { 0x2994 }));
            offsets.Add("Panasonic DMC-GF5", new OffsetEntry("Panasonic DMC-GF5", new int[1] { 0x31F4 }));
            offsets.Add("Panasonic DMC-GX1", new OffsetEntry("Panasonic DMC-GX1", new int[1] { 0x29E8 }));
        }

        public void Parse2(string filename)
        {
            int cameraNameOffset = 0x32E;
            int distortionCorrectionOffset = 0x340;
            UInt16[] distortionValues = new UInt16[16];
            string cameraName;
            string lensName;
            Distortion distortion;
            Distortion distortion2;
            double focalLength = 0;
            double aperture = 0;
            byte[] fileBytes = File.ReadAllBytes(filename);
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 32; i++) {
                sb.Append(Convert.ToString(fileBytes[distortionCorrectionOffset + i], 16).PadLeft(2, '0'));
            }

            FileStream fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            fs.Seek(cameraNameOffset, 0);
            BinaryReader binaryReader = new BinaryReader(fs);
            cameraName = (new string(Util.ReplaceNULWithBlanks(binaryReader.ReadChars(18)))).Trim();
            for (int i = 0; i < 16; i++)
            {
                distortionValues[i] = ReverseBytes(binaryReader.ReadUInt16());
            }

            //fs.Seek(GetLensOffsetFromCameraName((new string(Util.ReplaceNULWithBlanks(cameraName)).Trim())), 0);
            //BinaryReader binaryReaderLensName = new BinaryReader(fs);
            //lensName = binaryReaderLensName.ReadChars(34);
            lensName = offsets[cameraName].GetLensName(fs);

            distortion.n = ReverseBytes(distortionValues[12]);
            distortion.scale = 1.0 / (1.0 + (ReverseBytes(distortionValues[5]) / 32768.0));
            distortion.a = distortion.scale * (ReverseBytes(distortionValues[8]) / 32768.0);
            distortion.b = distortion.scale * (ReverseBytes(distortionValues[4]) / 32768.0);
            distortion.c = distortion.scale * (ReverseBytes(distortionValues[11]) / 32768.0);

            distortion2.n = distortionValues[12];
            distortion2.scale = 1.0 / (1.0 + (distortionValues[5] / 32768.0));
            distortion2.a = distortion2.scale * (distortionValues[8] / 32768.0);
            distortion2.b = distortion2.scale * (distortionValues[4] / 32768.0);
            distortion2.c = distortion2.scale * (distortionValues[11] / 32768.0);
            //EM5 vielleicht:0xC20E oder 0x2854 bis 0x2863

            ExifReader reader = null;
            try
            {
                reader = new ExifReader(filename, 0x800);

                //string CameraMaker;
                //string CameraModel;
                //reader.GetTagValue<String>(ExifTags.Make, out CameraMaker);
                //reader.GetTagValue<String>(ExifTags.Model, out CameraModel);
                reader.GetTagValue<Double>(ExifTags.FocalLength, out focalLength);
                reader.GetTagValue<Double>(ExifTags.ApertureValue, out aperture);

            }
            catch (Exception ex)
            {
                // Something didn't work!


                if (reader != null)
                    reader.Dispose();
            }

            //using (FileStream fs2 = new FileStream("test.ab", FileMode.OpenOrCreate))
            //{
            //    using (BinaryWriter w = new BinaryWriter(fs2))
            //    {
            //        for (int i = 0; i < 16; i++)
            //        {
            //            w.Write(distortionValues[i]);
            //        }
            //    }
            //}
            //distortionValues[10]= 1;
            //int checksumResult = Verify_checksums(distortionValues);

            WriteOutputToFile(cameraName, lensName, focalLength.ToString(), aperture.ToString(), distortionValues, distortion, distortion2, filename);
        }

        public void WriteOutputToFile(String cameraName, String lensName, String focalLength, String aperture, UInt16[] distortionValues, Distortion distortion, Distortion dist2, String filename)
        {
            bool fileExists = File.Exists("results.txt");
            TextWriter tw = new StreamWriter("results.txt", true, Encoding.UTF8);
            if (!fileExists)
            {
                tw.Write(GetHeader());
            }
            tw.Write(cameraName);
            tw.Write("\t'");
            tw.Write(lensName);
            tw.Write("\t'");
            tw.Write(focalLength);
            tw.Write("\t'");
            //tw.Write(aperture);
            //tw.Write("\t'");
            tw.Write("{0}\t'{1:F15}\t'{2:F15}\t'{3:F15}\t'{4:F15}\t'", distortion.n, distortion.scale, distortion.a, distortion.b, distortion.c);
            tw.Write("{0}\t'{1:F15}\t'{2:F15}\t'{3:F15}\t'{4:F15}\t'", dist2.n, dist2.scale, dist2.a, dist2.b, dist2.c);

            for (int i = 0; i < 16; i++)
            {
                tw.Write(Convert.ToString(distortionValues[i], 16).PadLeft(4, '0'));
                if (i < 15) tw.Write("\t'");
            }
            tw.Write("\t");
            tw.WriteLine(Path.GetFileName(filename));

            // close the stream
            tw.Close();
        }

        // reverse byte order (16-bit)
        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }

        int Verify_checksums(UInt16[] data)
        {
            byte[] data8 = new byte[32];
            int j = 0;
            for (int i = 0; i < data.Length; i++) {
               byte[] temp = BitConverter.GetBytes( data[i] );
                data8[j]   = temp[0];
                data8[j+1] = temp[1];
                j = j + 2;
            }
            
            byte[] even = new byte[16];
            byte[] odd = new byte[16];
            UInt16 csum1, csum2, csum3, csum4;
            int res;

            for (int i = 0; i < 16; i++) {
                even[i] = data8[i*2];
                odd[i] = data8[i*2+1];
            }
            byte[] csum1Array = new byte[12];
            byte[] csum2Array = new byte[12];
            byte[] csum3Array = new byte[14];
            byte[] csum4Array = new byte[14];
            Array.Copy(data8, 4, csum1Array, 0, 12);
            Array.Copy(data8, 16, csum2Array, 0, 12);
            Array.Copy(data8, 1, csum3Array, 0, 14);
            Array.Copy(data8, 1, csum4Array, 0, 14);

            csum1=Checksum(csum1Array,12);
            csum2 = Checksum(csum2Array, 12);
            csum3 = Checksum(csum3Array, 14);
            csum4 = Checksum(csum4Array, 14);
            res = 0;
            res ^= (csum1 ^ data[1]);
            res ^= (csum2 ^ data[14]);
            res ^= (csum3 ^ data[0]);
            res ^= (csum4 ^ data[15]);
            return res;
        }

        UInt16 Checksum(byte[] data, int len)
        {
            
            UInt16 csum=0;

            for (int i = 0; i < len; i++) {
                csum = (UInt16)((73 * csum + data[i]) % 0xFFEF);
            }
            return csum;
        }

        public String GetHeader()
        {
            string header;
            header = "Camera____________\tLens______________________________\tFL\tn___\tsc________________\ta________________\tb_______________\tc_______________\tn2__\tsc2_______________\ta2_______________\tb2______________\tc2______________\t0___\t1___\t2___\t3___\t4___\t5___\t6___\t7___\t8___\t9___\t10__\t11__\t12__\t13__\t14__\t15__\tFilename";
            return header+ "\n";
        }

        public int GetLensOffsetFromCameraName(string cameraName)
        {
            if (cameraName.Equals("Panasonic DMC-GH1"))
            {
                return 0x28A6;
            }
            else if (cameraName.Equals("Panasonic DMC-GH2"))
            {
                return 0x2964;
            }
            else if (cameraName.Equals("Panasonic DMC-GH3"))
            {
                return 0x3308;
            }
            else if (cameraName.Equals("Panasonic DMC-GF1"))
            {
                return 0x28BC;
            }
            else if (cameraName.Equals("Panasonic DMC-GF2"))
            {
                return 0x2970;
            }
            else if (cameraName.Equals("Panasonic DMC-GF3"))
            {
                return 0x2994;
            }
            else if (cameraName.Equals("Panasonic DMC-GF5"))
            {
                return 0x31F4;
            }
            else if (cameraName.Equals("Panasonic DMC-G10"))
            {
                return 0x28EC;
            }
            else if (cameraName.Equals("Panasonic DMC-G1"))
            {
                return 0x2844;
            }
            else if (cameraName.Equals("Panasonic DMC-G2"))
            {
                return 0x2974;
            }
            else if (cameraName.Equals("Panasonic DMC-G3"))
            {
                return 0x2994;
            }
            else if (cameraName.Equals("Panasonic DMC-G5"))
            {
                return 0x3218;
            }
            else if (cameraName.Equals("Panasonic DMC-GX1"))
            {
                return 0x29E8;
            }
            return 0;
        }
    


    }
}
