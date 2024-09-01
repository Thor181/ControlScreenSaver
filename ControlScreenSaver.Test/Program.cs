namespace ControlScreenSaver.Test
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //ScreenSaver.On();

            await Task.Delay(2000);

            ScreenSaver.Off();
            Console.Beep();
        }
    }
}
