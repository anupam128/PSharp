﻿using System;
using System.Collections.Generic;
using Microsoft.PSharp;

namespace Chord1
{
    public class Finger
    {
        public int Start;
        public int End;
        public MachineId Node;

        public Finger(int start, int end, MachineId node)
        {
            this.Start = start;
            this.End = end;
            this.Node = node;
        }
    }
}
