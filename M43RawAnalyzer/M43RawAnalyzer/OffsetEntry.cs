using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace M43RawAnalyzer
{
    class OffsetEntry
    {
        private string name;
        private int[] offsets;

        public OffsetEntry(string TheName, int[] TheOffsets) {
            name = TheName;
            offsets = TheOffsets;
        }

        public string GetLensName(FileStream fileStream) {
            for (int i = 0; i < offsets.Length; i++) {
                if (IsLensName(Peek(fileStream, offsets[i]))) {
                    return Peek(fileStream, offsets[i]);
                }
            }
            return "";
        }

        private string Peek(FileStream fileStream, int offset) {
            fileStream.Seek(offset, 0);
            BinaryReader binaryReaderLensName = new BinaryReader(fileStream);
            return new string(Util.ReplaceNULWithBlanks(binaryReaderLensName.ReadChars(34)));
        }

        private bool IsLensName(string stringToTest) {
            if (stringToTest.StartsWith("LUMIX")) {
                return true;
            } else if (stringToTest.StartsWith("LEICA")) {
                return true;
            } else if (stringToTest.StartsWith("OLYMPUS")) {
                return true;
            } else if (stringToTest.StartsWith("SIGMA")) {
                return true;
            } else {
                return false;
            }
        }
    }
}
