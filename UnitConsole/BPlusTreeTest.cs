using BigCookieKit.Algorithm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitConsole
{
    public static class BPlusTreeTest
    {
        public static void Example()
        {
            Console.WriteLine("程序初始占用内存:{0}MB", (GC.GetTotalMemory(true) / 1024));

            var tree = new BPlusTree<int, int?>(5);

            Console.WriteLine("创建BPlusTree完成！");
            Console.WriteLine("程序占用内存:{0}MB", (GC.GetTotalMemory(true) / 1024));

            //tree.Insert(0, 0);
            //tree.Insert(1, 1);
            //tree.Insert(2, 2);
            //tree.Insert(3, 3);
            //tree.Insert(4, 4);
            //tree.Insert(5, 5);
            //tree.Insert(6, 6);
            //tree.Insert(7, 7);
            //tree.Insert(11, 11);
            //tree.Insert(12, 12);
            //tree.Insert(13, 13);
            //tree.Insert(14, 14);
            //tree.Insert(15, 15);

            //tree.Insert(8, 8);
            //tree.Insert(9, 9);
            //tree.Insert(10, 10);

            for (int i = 0; i < 500; i++)
            {
                tree.Insert(i, i);
                Console.WriteLine("BPlusTree添加索引:{0}内容:{1}", i, i);
                Console.WriteLine("程序占用内存:{0}MB", (GC.GetTotalMemory(true) / 1024));
                Console.ReadKey();
            }
        }
    }
}
