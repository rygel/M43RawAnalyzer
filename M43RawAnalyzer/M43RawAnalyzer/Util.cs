using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace M43RawAnalyzer {
    class Util {
        
        public static char[] ReplaceNULWithBlanks(char[] input) {
            for (int i = 0; i < input.Length; i++) {
                if (input[i] == 0) {
                    input[i] = ' ';
                }
            }
            return input;
        }

    }
}
