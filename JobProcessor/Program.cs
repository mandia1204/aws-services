using System;
using System.Threading.Tasks;

namespace JobProcessor
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("job  processor!");
            var processor = new JobProcessor();

            await processor.ProcessMessages();
            Console.WriteLine("Finished!");
            Console.ReadLine();
        }
    }
}
