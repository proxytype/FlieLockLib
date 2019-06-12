using System;
using System.IO;

namespace FileLockerLib
{

    public class FileLocker
    {
        public delegate void FileLockerMessage(FileLockerPayload payload);
        public event FileLockerMessage FileLockerHandler;

        private string _fileLock = string.Empty;
        private bool _isLock = false;
        private bool _enableEvents = false;

        public string fileLock { get { return _fileLock; } }
        public bool isLock { get { return _isLock; } }

        private FileStream stream;

        public enum FILE_LOCKER_MESSAGE_CODE
        {
            RELEASE_LOCK = 1,
            SUCCESS = 0,
            UNABLE_TO_LOCK = -1,
            UNABLE_TO_CREATE_LOCK = -2,
            UNABLE_DELETE_LOCK = -3,
            LOCK_ALREADY_BY_PROCESS = -4,
            LOCK_NOT_EXISTS = -5
        }

        public FileLocker(bool enableEvents)
        {
            _enableEvents = enableEvents;
        }

        public bool lockFile(string filePath)
        {

            FileLockerPayload payload = new FileLockerPayload();

            if (isLock)
            {

                payload.code = FILE_LOCKER_MESSAGE_CODE.LOCK_ALREADY_BY_PROCESS;
                payload.message = FILE_LOCKER_MESSAGE_CODE.LOCK_ALREADY_BY_PROCESS.ToString();

            }
            else
            {

                try
                {
                    _fileLock = filePath;

                    if (!File.Exists(filePath))
                    {
                        File.Create(filePath);
                    }
                    else
                    {
                        stream = File.Open(fileLock, FileMode.Open);
                    }

                    payload.code = FILE_LOCKER_MESSAGE_CODE.SUCCESS;
                    payload.message = FILE_LOCKER_MESSAGE_CODE.SUCCESS.ToString();

                    _isLock = true;
                }
                catch (DirectoryNotFoundException ex)
                {
                    payload.code = FILE_LOCKER_MESSAGE_CODE.UNABLE_TO_CREATE_LOCK;
                    payload.message = ex.Message;

                    _isLock = false;
                }
                catch (Exception ex)
                {
                    payload.code = FILE_LOCKER_MESSAGE_CODE.UNABLE_TO_CREATE_LOCK;
                    payload.message = ex.Message;

                    _isLock = false;
                }

            }


            if (_enableEvents)
            {
                FileLockerHandler(payload);
            }

            return _isLock;

        }

        public bool unlockFile()
        {
            if (_isLock)
            {

                if (stream != null)
                {
                    stream.Close();
                    stream.Dispose();
                    stream = null;
                }

                if (_enableEvents)
                {

                    FileLockerPayload payload = new FileLockerPayload();
                    payload.code = FILE_LOCKER_MESSAGE_CODE.RELEASE_LOCK;
                    payload.message = FILE_LOCKER_MESSAGE_CODE.RELEASE_LOCK.ToString();
                    FileLockerHandler(payload);
                }

                _isLock = false;
            }

            return isLock;
        }

        public bool deleteLockFile()
        {
            FileLockerPayload payload = new FileLockerPayload();
            bool isSuccess = false;

            if (_fileLock != string.Empty)
            {

                if (_isLock)
                {
                    payload.code = FILE_LOCKER_MESSAGE_CODE.LOCK_ALREADY_BY_PROCESS;
                    payload.message = FILE_LOCKER_MESSAGE_CODE.LOCK_ALREADY_BY_PROCESS.ToString();

                }
                else
                {

                    try
                    {
                        File.Delete(_fileLock);
                        isSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        payload.code = FILE_LOCKER_MESSAGE_CODE.UNABLE_DELETE_LOCK;
                        payload.message = ex.Message;

                        isSuccess = false;
                    }
                }
            }
            else {
                payload.code = FILE_LOCKER_MESSAGE_CODE.LOCK_NOT_EXISTS;
                payload.message = FILE_LOCKER_MESSAGE_CODE.LOCK_NOT_EXISTS.ToString();
            }

            if (_enableEvents)
            {
                FileLockerHandler(payload);
            }

            return isSuccess;
        }

    }
}
