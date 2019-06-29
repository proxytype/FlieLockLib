# FlieLockLib
Simple library for synchronize processes using file as lock.

![alt text](https://raw.githubusercontent.com/proxytype/FlieLockerLib/master/page-order-cpu-overheat.gif)

# usage:
```C#
 static FileLocker locker;
        static int releaseCounter = 0;
        static int releaseAfter = 10;

        static void Main(string[] args)
        {
            //the name of the lock need to be accepted by all processes
            string fileLockPath = @"C:\Users\proxytype\Desktop\filelocker.lck";
            //create new file locker
            locker = new FileLocker(true);
            //signin the filelocker events
            locker.FileLockerHandler += Locker_FileLockerHandler;

            //for the example the file going to be lock and release
            while (true) {

                locker.lockFile(fileLockPath);
                Random random = new Random();
                int value =  random.Next(100, 1000);
                Thread.Sleep(value);

                if (locker.isLock)
                {
                    releaseCounter = releaseCounter + 1;
                    if (releaseCounter > releaseAfter)
                    {
                        releaseCounter = 0;
                        locker.unlockFile();

                        Thread.Sleep(2000);
                    }

                }
                else {
                    Thread.Sleep(value);
                }
            }
        }

        //getting feedback from the locker
        private static void Locker_FileLockerHandler(FileLockerPayload payload)
        {
            Console.WriteLine(Process.GetCurrentProcess().ProcessName + " - Locker Response:" + payload.code.ToString() + " Message:" + payload.message);
```
