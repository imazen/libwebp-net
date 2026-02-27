using System;
using Imazen.WebP;

namespace ConsumptionTest
{
    class Program
    {
        static int Main()
        {
            try
            {
                // These calls trigger native library loading via NativeLibraryLoader.
                // If the NuGet package is correctly structured:
                //   .NET Core: deps.json resolves native files from runtimes/{rid}/native/ in NuGet cache
                //   .NET Framework: .targets copies native files to output directory
                string decoderVersion = SimpleDecoder.GetDecoderVersion();
                Console.WriteLine($"Decoder version: {decoderVersion}");

                string encoderVersion = SimpleEncoder.GetEncoderVersion();
                Console.WriteLine($"Encoder version: {encoderVersion}");

                Console.WriteLine("Package consumption test PASSED");
                return 0;
            }
            catch (DllNotFoundException ex)
            {
                Console.Error.WriteLine($"FAILED (DllNotFoundException): {ex.Message}");
                return 1;
            }
            catch (NotSupportedException ex)
            {
                Console.Error.WriteLine($"FAILED (ABI mismatch): {ex.Message}");
                return 1;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"FAILED ({ex.GetType().Name}): {ex.Message}");
                return 1;
            }
        }
    }
}
