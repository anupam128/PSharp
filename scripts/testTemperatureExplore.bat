set steps=%1
set iters=%2
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\BuggyBenchmarks\Chord\bin\Debug\Chord.dll /explore /max-steps:%steps% /i:%iters% 
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\BuggyBenchmarks\ReplicatingStorage\bin\Debug\ReplicatingStorage.dll /explore /max-steps:%steps% /i:%iters% 
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\BuggyBenchmarks\Raft\bin\Debug\Raft.dll /explore /max-steps:%steps% /i:%iters% 
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\SpinBenchmarks\SpinBenchmarks\bin\Debug\ProcessScheduler.dll /explore /max-steps:%steps% /i:%iters% 
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\SpinBenchmarks\LeaderElection\bin\Debug\LeaderElection.dll /explore /max-steps:%steps% /i:%iters% 
.\PSharpTester.exe /test:D:\PSharp\Samples\Experimental\LivenessBenchmarks\SpinBenchmarks\SlidingWindowProtocol\bin\Debug\SlidingWindowProtocol.dll /explore /max-steps:%steps% /i:%iters% 
.\PSharpTester.exe /test:D:\vnext\PyraStor\NameNode.PSharp\bin\Debug\NameNode.PSharp.dll /explore /max-steps:%steps% /i:%iters% 