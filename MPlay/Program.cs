using NAudio.Wave;
using static System.Console;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace MusikWChecker
{
    class Program
    {
        private const string UrlMusikW = "http://musikw.tk";
        //private const string UrlMusikW = "http://marciuscarneiro-001-site7.btempurl.com";

        static void Main(string[] args)
        {
            // if (args.Length == 0)
            // {
            //     showCredits();
            //     return;
            // };
            var subdirs = args.Contains("-s") || args.Contains("-S");
            var searchParam = args.FirstOrDefault(a => a.ToLowerInvariant() != "-s");
            if (string.IsNullOrWhiteSpace(searchParam))
            {
                showCredits();
                return;
            }
            var dir = Path.GetDirectoryName(searchParam);
            if (string.IsNullOrWhiteSpace(dir)) dir = ".";
            var pattern = Path.GetFileName(searchParam);
            var files = Directory
                .GetFiles(dir, pattern, subdirs ? SearchOption.AllDirectories :
                    SearchOption.TopDirectoryOnly);
            if (files.Any())
            {
                WriteLine("Iniciando....");
                var index = 0;
                while (index < files.Length)
                {
                    try
                    {
                        if (index < 0) index = 0;
                        var file = files[index];
                        Write("Tocando: ");
                        ForegroundColor = ConsoleColor.Green;
                        WriteLine(Path.GetFileName(file));
                        ResetColor();
                        WriteLine($@"[{Path.GetDirectoryName(Path.GetRelativePath(".", file))}\]");
                        ForegroundColor = ConsoleColor.DarkGray;
                        WriteLine($@"[{Path.GetFullPath(file)}]");
                        ForegroundColor = ConsoleColor.Blue;
                        PlayAudio(file, ref index);
                        ResetColor();
                    }
                    catch (Exception ex)
                    {
                        ForegroundColor = ConsoleColor.Red;
                        WriteLine($"{ex.Message}\n");
                        ResetColor();
                    }
                    index++;
                }
            }
            else
                WriteLine("Nenhuma música!");
        }

        private static void showCredits()
        {
            WriteLine("Marcius Bezerra's Console Play Music!");
            WriteLine("use: [-s] mplay file");
            WriteLine("-s: Procurar também em subdiretórios");
        }

        private static void PlayAudio(string file, ref int index)
        {
            using var audioFile = new AudioFileReader(file);
            using var outputDevice = new WaveOutEvent();
            outputDevice.Init(audioFile);
            outputDevice.Play();
            while ((outputDevice.PlaybackState == PlaybackState.Playing))
            {
                if (KeyAvailable)
                {
                    var key = ReadKey(true).Key;
                    if (key == ConsoleKey.RightArrow)
                        audioFile.Skip(30);
                    if (key == ConsoleKey.LeftArrow)
                        audioFile.Skip(-30);
                    if (key == ConsoleKey.UpArrow)
                    {
                        index -= 2;
                        break;
                    }
                    if (key == ConsoleKey.DownArrow)
                        break;
                }
                SetCursorPosition(0, CursorTop);
                Write($"{audioFile.CurrentTime} - {audioFile.TotalTime}");
                Thread.Sleep(500);
            }
            WriteLine("\n");
        }
    }
}
