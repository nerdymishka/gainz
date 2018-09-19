using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using NerdyMishka.Security.Cryptography;

namespace NerdyMishka.Windows
{
    public class VaultManager
    {
        [CLSCompliant(false)]
        public static VaultCredential Create() {
            return  new VaultCredential()
            {
                LastWritten = DateTime.Now,
                Type = CredentialsType.Generic,
                Flags = CredentialFlags.None,
                Persistence = Persistence.LocalMachine,
                Attributes = IntPtr.Zero,
                AttributeCount = 0
            };
        }

        [CLSCompliant(false)]
        public static void Write(VaultCredential credential)
        {
            var emptyValue = credential.Data == null || credential.Data.Length == 0;           

            var cred = credential;
            var length = (uint)cred.Data.Length;
            cred.LastWritten = DateTime.Now;
            //var fileTime = new FILETIME()

            IntPtr data = Marshal.AllocHGlobal((int)length);
            if(length > 0)
            {
                var bytes = cred.GetBlob();
                Marshal.Copy(bytes, 0, data, (int)length);
            }

            var native = new NativeCredential()
            {
                AttributeCount = (uint)cred.AttributeCount,
                Comment = Marshal.StringToCoTaskMemUni(cred.Comment),
                CredentialBlob = data,
                CredentialBlobSize = length,
                Attributes = IntPtr.Zero,
                Flags = (uint)cred.Flags,
                Persist = (uint)cred.Persistence,
                TargetAlias = Marshal.StringToCoTaskMemUni(cred.Alias),
                TargetName = Marshal.StringToCoTaskMemUni(cred.Key),
                Type = (uint)cred.Type,
                UserName = Marshal.StringToCoTaskMemUni(cred.UserName),
            };

            var isSet = WriteCredential(ref native, (uint)0);
            int errorCode = Marshal.GetLastWin32Error();
            if (isSet)
                return;

            throw new ExternalException($"Advapi32.dll -> CredWriteW failed to write credential. error code {errorCode}");
        }

        [CLSCompliant(false)]
        public static VaultCredential Read(string path, CredentialsType type = CredentialsType.Generic)
        {
            IntPtr nativeCredentialPointer;

            bool success = ReadCredential(path, type, 0, out nativeCredentialPointer);
            int errorCode = Marshal.GetLastWin32Error();
            if (success)
            {
                using (var handle = new CredentialHandle(nativeCredentialPointer))
                {
                    return handle.AllocateCredential();
                }
            }

            string message;
            switch (errorCode)
            {
                case 1168:
                    // crdential not found
                    return null;
                default:
                    message = $"Advapi32.dll -> CredWriteW failed to read credential. error code {errorCode}.";
                    throw new ExternalException(message);
            }
        }

        [CLSCompliant(false)]
        public static void Delete(string path, CredentialsType type = CredentialsType.Generic)
        {
            bool success = DeleteCredential(path, type, 0);
            int errorCode = Marshal.GetLastWin32Error();

            if (success)
                return;

            throw new ExternalException($"Advapi32.dll -> CredDeleteW failed to delete credential. error code {errorCode}");
        }


        [CLSCompliant(false)]
        public static VaultCredential[] List()
        {
            int count;
            int flags;

            if (6 <= Environment.OSVersion.Version.Major)
            {
                flags = 0x1;
            }
            else
            {
                string message = "Retrieving all credentials is only possible on Windows version Vista or later.";
                throw new Exception(message);
            }


            bool success = EnumerateCredentials(null, flags, out count, out IntPtr nextCredentialsPointer);
            int errorCode = Marshal.GetLastWin32Error();

            if (success)
            {
                using (var handle = new CredentialHandle(nextCredentialsPointer))
                {
                    return handle.AllocateCredentials(count);
                }
            }


            throw new ExternalException($"Advapi32.dll -> CredEnumerateW failed to read credentials. error code {errorCode}");
        }

        
        internal class CredentialHandle : Microsoft.Win32.SafeHandles.CriticalHandleMinusOneIsInvalid
        {

            public CredentialHandle(IntPtr handle)
            {
                this.SetHandle(handle);
            }

            public VaultCredential AllocateCredential()
            {
                if (this.IsInvalid)
                    throw new InvalidOperationException($"{typeof(CriticalHandle).FullName} handle is invalid");

                return this.AllocateCredentialFromHandle(this.handle);
            }

            public VaultCredential[] AllocateCredentials(int count)
            {
                if (this.IsInvalid)
                    throw new InvalidOperationException("Invalid CriticalHandle!");
                

                var credentials = new VaultCredential[count];
                for (int i = 0; i < count; i++)
                {
                    IntPtr nextPointer = Marshal.ReadIntPtr(handle, i * IntPtr.Size);
                    var credential = this.AllocateCredentialFromHandle(nextPointer);
                    if(credential.Key.Contains(":target="))
                    {
                        var path = credential.Key;
                        path = path.Substring(path.IndexOf("=") + 1);
                        credential.Key = path;
                    }
                    credentials[i] = credential;
                }

                return credentials;
            }

            private VaultCredential AllocateCredentialFromHandle(IntPtr handle)
            {
#if NET45
                var native = new NativeCredential();
                Marshal.PtrToStructure(handle, native);
#else 
                var native = Marshal.PtrToStructure<NativeCredential>(handle);
#endif
                var fileTime = (((long)native.LastWritten.dwHighDateTime) << 32) + native.LastWritten.dwLowDateTime;
                byte[] data = null;

                if (native.CredentialBlobSize > 0)
                {
                    data = new byte[native.CredentialBlobSize];
                    Marshal.Copy(native.CredentialBlob, data, 0, (int)native.CredentialBlobSize);
                }

                ProtectedMemoryString ps = null;
                if (data != null)
                {
                    ps = new ProtectedMemoryString(data, true);
                    Array.Clear(data, 0, data.Length);
                }

                return new VaultCredential()
                {
                    AttributeCount = (int)native.AttributeCount,
                    Attributes = native.Attributes,
                    Comment = Marshal.PtrToStringUni(native.Comment),
                    Flags = (CredentialFlags)native.Flags,
                    LastWritten = DateTime.FromFileTime(fileTime),
                    UserName = Marshal.PtrToStringUni(native.UserName),
                    Alias = Marshal.PtrToStringUni(native.TargetAlias),
                    Key = Marshal.PtrToStringUni(native.TargetName),
                    Length = (int)native.CredentialBlobSize,
                    Persistence = (Persistence)native.Persist,
                    Type = (CredentialsType)native.Type,
                    Data = ps
                };
            }


            protected override bool ReleaseHandle()
            {
                if (this.IsInvalid)
                    return false;

               
                FreeCredential(handle);
                //Marshal.FreeHGlobal(handle);
                this.SetHandleAsInvalid();
                return true;
            }
        }



        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct NativeCredential
        {
            public uint Flags;
            public uint Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }


        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool ReadCredential([In] string target, [In] CredentialsType type, [In] int reservedFlag, out IntPtr credentialPtr);

        [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool WriteCredential([In] ref NativeCredential userCredential, [In] UInt32 flags);

        [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
        private static extern bool FreeCredential([In] IntPtr credentialPointer);

        [DllImport("Advapi32.dll", EntryPoint = "CredDeleteW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool DeleteCredential([In] string target, [In] CredentialsType type, [In] int reservedFlag);

        [DllImport("Advapi32.dll", EntryPoint = "CredEnumerateW", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool EnumerateCredentials([In] string filter, [In] int flags, out int count, out IntPtr credentialPtrs);


    }
}