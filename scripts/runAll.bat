call ..\scripts\testTemperature.bat 500 100 >outTemperature.txt 2>bugsTemperature.txt
call ..\scripts\testCycle.bat 500 100 >outRandom.txt 2>bugsRandom.txt
call ..\scripts\testCyclePCT.bat 500 100 >outPCT.txt 2>bugsPCT.txt

call ..\scripts\testTemperature.bat 500 1000 >outTemperature1.txt 2>bugsTemperature1.txt
call ..\scripts\testCycle.bat 500 1000 >outRandom1.txt 2>bugsRandom1.txt
call ..\scripts\testCyclePCT.bat 500 1000 >outPCT1.txt 2>bugsPCT1.txt

call ..\scripts\testTemperature.bat 10000 10000 >outTemperature2.txt 2>bugsTemperature2.txt
call ..\scripts\testCycle.bat 10000 10000 >outRandom2.txt 2>bugsRandom2.txt
call ..\scripts\testCyclePCT.bat 10000 10000 >outPCT2.txt 2>bugsPCT2.txt
