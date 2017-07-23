﻿using System.Management.Automation;
using PrtgAPI.PowerShell.Progress;
using PrtgAPI.Tests.UnitTests.InfrastructureTests.Support.Progress;

namespace PrtgAPI.Tests.UnitTests.InfrastructureTests.Support
{
    public class MockProgressWriter : IProgressWriter
    {
        public static void Bind()
        {
            ProgressManager.CustomWriter = new MockProgressWriter();
        }

        public void WriteProgress(ProgressRecord progressRecord)
        {
            ProgressQueue.Enqueue(progressRecord);
        }

        public void WriteProgress(long sourceId, ProgressRecord progressRecord)
        {
            ProgressQueue.Enqueue(progressRecord);
        }
    }
}
