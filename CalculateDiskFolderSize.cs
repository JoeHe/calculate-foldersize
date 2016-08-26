using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CalculateFileSize
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> folderList = new List<string>();
            folderList.Add(@"C:\p4S_Root\TestDev\utils\Loadtest");
            folderList.Add(@"C:\p4S_Root\TestDev\utils\Loadtest\WorkingFolder");
            folderList.Add(@"C:\p4S_Root\TestDev\utils\Loadtest\CommonLib");

            FileStream ostrm;
            StreamWriter writer;
            TextWriter oldOut = Console.Out;
            try
            {
                ostrm = new FileStream("./Redirect.txt", FileMode.OpenOrCreate, FileAccess.Write);
                writer = new StreamWriter(ostrm);
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot open Redirect.txt for writing");
                Console.WriteLine(e.Message);
                return;
            }
            Console.SetOut(writer);


            foreach (var folder in folderList)
            {
                DirectoryInfo di = new DirectoryInfo(folder);
                DirectoryInfo[] diarray=di.GetDirectories();
                Console.WriteLine("Folder [" + folder + "] size on disk list:");
                Console.WriteLine("===================================================================");
                Console.WriteLine("{0,-50} {1,15}", "Name", "Size");
                GetandPrintResult(diarray);
            }
                 
            Console.SetOut(oldOut);
            writer.Close();
            ostrm.Close();
            Console.WriteLine("Done");

            Console.WriteLine("please enter 'exit' to exit!");
            while (true)
            {
                string str = Console.ReadLine();
                if (str.Equals("exit", StringComparison.InvariantCultureIgnoreCase))
                {
                    Environment.Exit(0);
                }
            }

        }

        public static long GetFolderSize(string folder)
        {
            long folderSize = 0;
            try
            {
                //Checks if the path is valid or not
                if (!Directory.Exists(folder))
                    return folderSize;
                else
                {
                    try
                    {
                        foreach (string file in Directory.GetFiles(folder))
                        {
                            if (File.Exists(file))
                            {
                                FileInfo finfo = new FileInfo(file);
                                folderSize += finfo.Length;
                            }
                        }

                        //foreach (string dir in Directory.GetDirectories(folder))
                        folderSize += new DirectoryInfo(folder).GetFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
                    }
                    catch (NotSupportedException e)
                    {
                        Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine("Unable to calculate folder size: {0}", e.Message);
            }
            return folderSize;

        }

        public static string ConvertFilsize(long byteCount)
        {
            string size = "0 Bytes";
            if (byteCount >= 1073741824.0)
                size = String.Format("{0:##.##}", byteCount / 1073741824.0) + " GB";
            else if (byteCount >= 1048576.0)
                size = String.Format("{0:##.##}", byteCount / 1048576.0) + " MB";
            else if (byteCount >= 1024.0)
                size = String.Format("{0:##.##}", byteCount / 1024.0) + " KB";
            else if (byteCount > 0 && byteCount < 1024.0)
                size = byteCount.ToString() + " Bytes";

            return size;
        }

        public static void GetandPrintResult(DirectoryInfo[] directories)
        {
            long totalsize = 0;          
            DateTime starttime = DateTime.Now;
            foreach (var subfolder in directories)
            {
                long sizebyte = GetFolderSize(subfolder.FullName);
                string sizeconverted = ConvertFilsize(sizebyte);
                Console.WriteLine("{0,-50} {1,15}", subfolder.Name, sizeconverted);

                totalsize += sizebyte;
            }
            Console.WriteLine("===================================================================");
            DateTime endtime = DateTime.Now;
            Console.WriteLine("Total size: " + ConvertFilsize(totalsize) + ", Consume time: " + (endtime - starttime));
            Console.WriteLine();
            Console.WriteLine();
        }
    }
}
