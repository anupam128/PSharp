set steps=%1
set iters=%2
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\BuggyBenchmarks\Chord\bin\Debug\Chord.dll /max-steps:%steps% /i:%iters%
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\BuggyBenchmarks\ReplicatingStorage\bin\Debug\ReplicatingStorage.dll /max-steps:%steps% /i:%iters%
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\BuggyBenchmarks\Raft\bin\Debug\Raft.dll /max-steps:%steps% /i:%iters%
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\BuggyBenchmarks\RaftBackoff\bin\Debug\RaftBackoff.dll /max-steps:%steps% /i:%iters% 
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\BuggyBenchmarks\BoundedRaft\bin\Debug\BoundedRaft.dll /max-steps:%steps% /i:%iters% 
.\PSharpTester.exe /test:D:\vnext\PyraStor\NameNode.PSharp\bin\Debug\NameNode.PSharp.dll /max-steps:%steps% /i:%iters%
.\PSharpTester.exe /max-steps:%steps% /test:D:\BatchService\src\xstore\watask\PoolManager\PoolServer\IntegrationTests\bin\Debug\PSIntegrationTest.dll  /i:%iters% /method:DeletePool
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\SpinBenchmarks\SpinBenchmarks\bin\Debug\ProcessScheduler.dll /max-steps:%steps% /i:%iters%
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\SpinBenchmarks\LeaderElection\bin\Debug\LeaderElection.dll /max-steps:%steps% /i:%iters%
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\SpinBenchmarks\SlidingWindowProtocol\bin\Debug\SlidingWindowProtocol.dll /max-steps:%steps% /i:%iters%