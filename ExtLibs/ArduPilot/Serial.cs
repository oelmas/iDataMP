﻿using System;
using System.Text;

namespace MissionPlanner
{
    public class Serial
    {
        public static void print(object v)
        {
            if (v.GetType().IsArray)
                Console.Write(Encoding.UTF8.GetString((byte[])v));
            else
                Console.Write(v);
        }

        public static void println(object v)
        {
            if (v.GetType().IsArray)
                Console.WriteLine(Encoding.UTF8.GetString((byte[])v));
            else
                Console.WriteLine(v);
        }
    }
}